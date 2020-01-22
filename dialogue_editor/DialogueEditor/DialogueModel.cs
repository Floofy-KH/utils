using floofy;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace DialogueEditor
{
    public class DialogueItem : INotifyPropertyChanged
    {
        private string _name;

        public string DialogueName
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("DialogueName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class DeleteDialogueCmd : IUndoableCommand
    {
        private string _dialogueName;
        private ObservableCollection<DialogueItem> _dlgItems;
        private DialogueManager _mgr;
        private int _index = -1;
        private DialogueItem _removedItem = null;

        public DeleteDialogueCmd(string name, ObservableCollection<DialogueItem> dialogueItems, DialogueManager mgr)
        {
            _dialogueName = name;
            _dlgItems = dialogueItems;
            _mgr = mgr;
        }

        public void Execute()
        {
            for (int i = 0; i < _dlgItems.Count; ++i)
            {
                if (_dlgItems[i].DialogueName == _dialogueName)
                {
                    _mgr.RemoveDialogue(_dialogueName);
                    _removedItem = _dlgItems[i];
                    _dlgItems.RemoveAt(i);
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

            _mgr.AddDialogue(_removedItem.DialogueName); //TODO do this propely...
            _dlgItems.Insert(_index, _removedItem);
        }
    }

    public class AddDialogueCmd : IUndoableCommand
    {
        private string _dialogueName;
        private ObservableCollection<DialogueItem> _dlgItems;
        private DialogueManager _mgr;
        private DialogueItem _addedItem = null;

        public AddDialogueCmd(string name, ObservableCollection<DialogueItem> dialogueItems, DialogueManager mgr)
        {
            _dialogueName = name;
            _dlgItems = dialogueItems;
            _mgr = mgr;
        }

        public void Execute()
        {
            if (_addedItem == null)
            {
                _addedItem = new DialogueItem { DialogueName = _dialogueName };
            }
            _dlgItems.Add(_addedItem);
            var dlg = _mgr.AddDialogue(_dialogueName);
            //TODO remove this when we have proper participant support in UI
            dlg.AddParticipant("Part1");
            dlg.AddParticipant("Part2");
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

            _mgr.RemoveDialogue(_addedItem.DialogueName);
            _dlgItems.Remove(_addedItem);
        }
    }

    public class DialogueModel : HandlesPropertyChanged
    {
        public ObservableCollection<DialogueItem> DlgItems { get; set; }

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
            DlgItems = new ObservableCollection<DialogueItem>();
            PopulateDialogueList();
        }

        public void Add(string dlgName)
        {
            _cmdExec.ExecuteCommand(new AddDialogueCmd(dlgName, DlgItems, _mgr));
        }

        public void Remove(string dlgName)
        {
            _cmdExec.ExecuteCommand(new DeleteDialogueCmd(dlgName, DlgItems, _mgr));
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
                DlgItems.Add(new DialogueItem { DialogueName = _mgr.Dialogue(i).Name });
            }
        }
    }
}