using floofy;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DialogueEditor
{
    public class DialogueItem
    {
        public string DialogueName { get; set; }
    }

    public class DeleteDialogueCmd : IUndoableCommand
    {
        private string _dialogueName;
        private ObservableCollection<DialogueItem> _dlgItems;
        private int _index = -1;
        private DialogueItem _removedItem = null;

        public DeleteDialogueCmd(string name, ObservableCollection<DialogueItem> dialogueItems)
        {
            _dialogueName = name;
            _dlgItems = dialogueItems;
        }

        public void Execute()
        {
            for (int i = 0; i < _dlgItems.Count; ++i)
            {
                if (_dlgItems[i].DialogueName == _dialogueName)
                {
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

            _dlgItems.Insert(_index, _removedItem);
        }
    }

    public class AddDialogueCmd : IUndoableCommand
    {
        private string _dialogueName;
        private ObservableCollection<DialogueItem> _dlgItems;
        private DialogueItem _addedItem = null;

        public AddDialogueCmd(string name, ObservableCollection<DialogueItem> dialogueItems)
        {
            _dialogueName = name;
            _dlgItems = dialogueItems;
        }

        public void Execute()
        {
            if (_addedItem == null)
            {
                _addedItem = new DialogueItem { DialogueName = _dialogueName };
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
            _cmdExec.ExecuteCommand(new AddDialogueCmd(dlgName, DlgItems));
        }

        public void Remove(string dlgName)
        {
            _cmdExec.ExecuteCommand(new DeleteDialogueCmd(dlgName, DlgItems));
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