using floofy;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

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
            _mgr.AddDialogue(_dialogueName);
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

    public class DialogueModel
    {
        public ObservableCollection<DialogueItem> DlgItems { get; set; }

        private DialogueManager _mgr = null;
        private CommandExecutor _cmdExec = null;

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