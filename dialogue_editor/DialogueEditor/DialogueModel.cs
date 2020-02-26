using floofy;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace DialogueEditor
{
    public class DialogueViewModel : HandlesPropertyChanged
    {
        #region Undoable Commands

        internal class SetNameUndoableCommand : IUndoableCommand
        {
            private Dialogue _dialogue = null;
            private string _name, _oldName;

            public SetNameUndoableCommand(Dialogue dialogue, string name)
            {
                _dialogue = dialogue;
                _name = name;
                _oldName = dialogue.Name;
            }

            public void Execute()
            {
                _dialogue.Name = _name;
            }

            public void Undo()
            {
                _dialogue.Name = _oldName;
            }

            public void Redo()
            {
                Execute();
            }
        }

        #endregion Undoable Commands

        public Dialogue _dialogue;
        private CommandExecutor _cmdExec = null;

        public DialogueViewModel(CommandExecutor cmdExec, Dialogue dialogue)
        {
            _dialogue = dialogue;
            _cmdExec = cmdExec;
        }

        public string Name
        {
            get { return _dialogue.Name; }

            set
            {
                if (_dialogue.Name != value)
                {
                    _cmdExec.ExecuteCommand(new SetNameUndoableCommand(_dialogue, value));
                    OnPropertyChanged("Name");
                }
            }
        }
    }

    public class DialogueModel : HandlesPropertyChanged
    {
        #region Undoable Commands

        internal class DeleteDialogueCmd : IUndoableCommand
        {
            private string _dialogueName;
            private DialogueModel _model;
            private DialogueManager _mgr;
            private int _index = -1;
            private DialogueViewModel _removedItem = null;

            public DeleteDialogueCmd(string name, DialogueModel model, DialogueManager mgr)
            {
                _dialogueName = name;
                _model = model;
                _mgr = mgr;
            }

            public void Execute()
            {
                for (int i = 0; i < _model.DlgItems.Count; ++i)
                {
                    if (_model.DlgItems[i].Name == _dialogueName)
                    {
                        _mgr.RemoveDialogue(_dialogueName);
                        _removedItem = _model.DlgItems[i];
                        _model.DlgItems.RemoveAt(i);
                        _index = i;
                        return;
                    }
                }
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                if (_index < 0 || _removedItem == null)
                {
                    Debug.Assert(false);
                    return;
                }

                _mgr.AddDialogue(_removedItem._dialogue);
                _model.DlgItems.Insert(_index, _removedItem);
            }
        }

        internal class AddDialogueCmd : IUndoableCommand
        {
            private string _dialogueName;
            private ObservableCollection<DialogueViewModel> _dlgItems;
            private DialogueManager _mgr;
            private DialogueViewModel _addedItem = null;
            private CommandExecutor _cmdExec = null;

            public AddDialogueCmd(CommandExecutor cmdExec, string name, ObservableCollection<DialogueViewModel> dialogueItems, DialogueManager mgr)
            {
                _cmdExec = cmdExec;
                _dialogueName = name;
                _dlgItems = dialogueItems;
                _mgr = mgr;
            }

            public void Execute()
            {
                var dlg = _mgr.AddDialogue(_dialogueName);
                if (_addedItem == null)
                {
                    _addedItem = new DialogueViewModel(_cmdExec, dlg);
                }
                _dlgItems.Add(_addedItem);
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                if (_addedItem == null)
                {
                    Debug.Assert(false);
                    return;
                }

                _mgr.RemoveDialogue(_addedItem.Name);
                _dlgItems.Remove(_addedItem);
            }
        }

        #endregion Undoable Commands

        public ObservableCollection<DialogueViewModel> DlgItems { get; set; }

        public NetworkViewModel Network
        {
            get
            {
                return _network;
            }
            set
            {
                _network = value;

                OnPropertyChanged("Network");
            }
        }

        private NetworkViewModel _network = null;
        private DialogueManager _mgr = null;
        private CommandExecutor _cmdExec = null;
        private Dialogue _currentDlg = null;

        public DialogueModel(CommandExecutor cmdExec) : this(new DialogueManager(), cmdExec)
        {
        }

        public DialogueModel(DialogueManager mgr, CommandExecutor cmdExec)
        {
            _mgr = mgr;
            _cmdExec = cmdExec;
            DlgItems = new ObservableCollection<DialogueViewModel>();
            PopulateDialogueList();
        }

        public void Add(string dlgName)
        {
            _cmdExec.ExecuteCommand(new AddDialogueCmd(_cmdExec, dlgName, DlgItems, _mgr));
        }

        public void Remove(string dlgName)
        {
            _cmdExec.ExecuteCommand(new DeleteDialogueCmd(dlgName, this, _mgr));
        }

        public bool Save(string file)
        {
            return _mgr.Write(file);
        }

        public void OpenDialogue(string dlgName)
        {
            _currentDlg = _mgr.Dialogue(dlgName);
            if (_currentDlg != null)
            {
                Network = new NetworkViewModel(_cmdExec, _currentDlg);
            }
            else
            {
                Network = null;
            }
        }

        public NodeViewModel CreateNode(string name, Point nodeLocation)
        {
            var node = new NodeViewModel(_cmdExec, _currentDlg.AddEntry(_currentDlg.Participant(0), name), _currentDlg)
            {
                X = nodeLocation.X,
                Y = nodeLocation.Y
            };

            Network.Nodes.Add(node);

            return node;
        }

        private void PopulateDialogueList()
        {
            DlgItems.Clear();
            if (_mgr == null)
            {
                Debug.Assert(false);
                return;
            }

            for (int i = 0; i < _mgr.NumDialogues; ++i)
            {
                DlgItems.Add(new DialogueViewModel(_cmdExec, _mgr.Dialogue(i)));
            }
        }
    }
}