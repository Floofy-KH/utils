using floofy;

namespace DialogueEditor
{
    public class ParticipantViewModel : HandlesPropertyChanged
    {
        #region Undoable Commands

        public class SetNameUndoableCommand : IUndoableCommand
        {
            private Dialogue _dialogue = null;
            private Participant _participant = null;
            private string _name, _oldName;

            public SetNameUndoableCommand(Dialogue dialogue, Participant participant, string name)
            {
                _dialogue = dialogue;
                _participant = participant;
                _name = name;
                _oldName = _participant.Name;
            }

            public void Execute()
            {
                _participant.Name = _name;
            }

            public void Undo()
            {
                _participant.Name = _oldName;
            }

            public void Redo()
            {
                Execute();
            }
        }

        #endregion Undoable Commands

        #region Internal Data Members

        private Dialogue _dialogue = null;
        private Participant _participant = null;
        private CommandExecutor _cmdExec = null;

        #endregion Internal Data Members

        public ParticipantViewModel(CommandExecutor cmdExec, Participant participant, Dialogue dialogue)
        {
            _dialogue = dialogue;
            _participant = participant;
            _cmdExec = cmdExec;
        }

        public Participant Participant
        {
            get
            {
                return _participant;
            }
        }

        public string Name
        {
            get
            {
                return _participant.Name;
            }
            set
            {
                if (_participant.Name == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetNameUndoableCommand(_dialogue, _participant, value));

                OnPropertyChanged("Name");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ParticipantViewModel))
            {
                return false;
            }

            return ((ParticipantViewModel)obj)._participant.Equals(_participant);
        }
    }
}