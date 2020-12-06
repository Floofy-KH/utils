using floofy;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DialogueEditor
{
    public class ChoiceViewModel : HandlesPropertyChanged
    {
        #region Undoable Commands

        internal class SetNameUndoableCommand : IUndoableCommand
        {
            private Choice _choice = null;
            private string _name, _oldName;

            public SetNameUndoableCommand(Choice choice, string name)
            {
                _choice = choice;
                _name = name;
                _oldName = choice.Name;
            }

            public void Execute()
            {
                _choice.Name = _name;
            }

            public void Undo()
            {
                _choice.Name = _oldName;
            }

            public void Redo()
            {
                Execute();
            }
        }

        #endregion Undoable Commands

        public Choice _choice;
        private CommandExecutor _cmdExec = null;

        public ChoiceViewModel(CommandExecutor cmdExec, Choice choice)
        {
            _choice = choice;
            _cmdExec = cmdExec;
        }

        public string Name
        {
            get { return _choice.Name; }

            set
            {
                if (_choice.Name != value)
                {
                    _cmdExec.ExecuteCommand(new SetNameUndoableCommand(_choice, value));
                    OnPropertyChanged("Name");
                }
            }
        }
    }

    public class ChoiceModel : HandlesPropertyChanged
    {
        #region Undoable Commands

        internal class DeleteChoiceCmd : IUndoableCommand
        {
            private string _choiceName;
            private ChoiceModel _model;
            private ChoiceManager _mgr;
            private int _index = -1;
            private ChoiceViewModel _removedItem = null;

            public DeleteChoiceCmd(string name, ChoiceModel model, ChoiceManager mgr)
            {
                _choiceName = name;
                _model = model;
                _mgr = mgr;
            }

            public void Execute()
            {
                for (int i = 0; i < _model.ChoiceItems.Count; ++i)
                {
                    if (_model.ChoiceItems[i].Name == _choiceName)
                    {
                        _mgr.RemoveChoice(_choiceName);
                        _removedItem = _model.ChoiceItems[i];
                        _model.ChoiceItems.RemoveAt(i);
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

                _mgr.AddChoice(_removedItem._choice);
                _model.ChoiceItems.Insert(_index, _removedItem);
            }
        }

        internal class AddChoiceCmd : IUndoableCommand
        {
            private string _choiceName;
            private ObservableCollection<ChoiceViewModel> _choiceItems;
            private ChoiceManager _mgr;
            private ChoiceViewModel _addedItem = null;
            private CommandExecutor _cmdExec = null;

            public AddChoiceCmd(CommandExecutor cmdExec, string name, ObservableCollection<ChoiceViewModel> choiceItems, ChoiceManager mgr)
            {
                _cmdExec = cmdExec;
                _choiceName = name;
                _choiceItems = choiceItems;
                _mgr = mgr;
            }

            public void Execute()
            {
                var choice = _mgr.AddChoice(_choiceName);
                if (_addedItem == null)
                {
                    _addedItem = new ChoiceViewModel(_cmdExec, choice);
                }
                _choiceItems.Add(_addedItem);
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

                _mgr.RemoveChoice(_addedItem.Name);
                _choiceItems.Remove(_addedItem);
            }
        }

        #endregion Undoable Commands

        public ObservableCollection<ChoiceViewModel> ChoiceItems { get; set; }

        private ChoiceManager _mgr = null;
        private CommandExecutor _cmdExec = null;

        public ChoiceModel(CommandExecutor cmdExec) : this(new ChoiceManager(), cmdExec)
        {
        }

        public ChoiceModel(ChoiceManager mgr, CommandExecutor cmdExec)
        {
            _mgr = mgr;
            _cmdExec = cmdExec;
            ChoiceItems = new ObservableCollection<ChoiceViewModel>();
            PopulateChoiceList();
        }

        public void Add(string choiceName)
        {
            _cmdExec.ExecuteCommand(new AddChoiceCmd(_cmdExec, choiceName, ChoiceItems, _mgr));
        }

        public void Remove(string choiceName)
        {
            _cmdExec.ExecuteCommand(new DeleteChoiceCmd(choiceName, this, _mgr));
        }

        public bool Save(string file)
        {
            return _mgr.Write(file);
        }

        private void PopulateChoiceList()
        {
            ChoiceItems.Clear();
            if (_mgr == null)
            {
                Debug.Assert(false);
                return;
            }

            for (int i = 0; i < _mgr.NumChoices; ++i)
            {
                ChoiceItems.Add(new ChoiceViewModel(_cmdExec, _mgr.Choice(i)));
            }
        }
    }
}