using floofy;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Utils;

namespace DialogueEditor
{
    #region Commands

    public class AddChoiceCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CommandExecutor _cmdExec = null;
        private NodeViewModel.AddChoiceUndoableCommand _cmd = null;

        public AddChoiceCommand(CommandExecutor cmdExec, NodeViewModel node, ImpObservableCollection<ConnectorViewModel> outgoingConnectors, DialogueEntry entry, Dialogue dialogue, string content)
        {
            _cmdExec = cmdExec;
            _cmd = new NodeViewModel.AddChoiceUndoableCommand(_cmdExec, node, outgoingConnectors, entry, dialogue, content);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _cmdExec.ExecuteCommand(_cmd);
        }
    }

    #endregion Commands

    public sealed class NodeViewModel : HandlesPropertyChanged
    {
        #region Undoable Commands

        public class AddChoiceUndoableCommand : IUndoableCommand
        {
            private ImpObservableCollection<ConnectorViewModel> _outgoingConnectors;
            private ConnectorViewModel _connector = null;
            private Dialogue _dialogue = null;
            private DialogueEntry _dialogueEntry = null;
            private NodeViewModel _node;
            private CommandExecutor _cmdExec = null;
            private string _content;

            public AddChoiceUndoableCommand(CommandExecutor cmdExec, NodeViewModel node, ImpObservableCollection<ConnectorViewModel> outgoingConnectors, DialogueEntry entry, Dialogue dialogue, string content)
            {
                _outgoingConnectors = outgoingConnectors;
                _dialogue = dialogue;
                _dialogueEntry = entry;
                _content = content;
                _node = node;
                _cmdExec = cmdExec;
            }

            public void Execute()
            {
                _connector = new ConnectorViewModel(_cmdExec, _dialogue.AddChoice(_dialogueEntry, _content), _node);
                _outgoingConnectors.Add(_connector);
            }

            public void Undo()
            {
                _dialogue.RemoveChoice(_connector.DialogueChoice);
                _outgoingConnectors.Remove(_connector);
                _connector = null;
            }

            public void Redo()
            {
                Execute();
            }
        }

        public class SetNodeContentUndoableCommand : IUndoableCommand
        {
            private string _oldContent, _newContent;
            private NodeViewModel _node;

            public SetNodeContentUndoableCommand(string newContent, NodeViewModel node)
            {
                _newContent = newContent;
                _node = node;
                _oldContent = _node.Content;
            }

            public void Execute()
            {
                _node._dialogueEntry.Content = _newContent;
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                _node._dialogueEntry.Content = _oldContent;
            }
        }

        public class SetNodePositionUndoableCommand : IUndoableCommand
        {
            private Vector2 _oldPosition, _newPosition;
            private NodeViewModel _node;

            public SetNodePositionUndoableCommand(Vector2 newPosition, NodeViewModel node)
            {
                _newPosition = newPosition;
                _node = node;
                _oldPosition.x = node.X;
                _oldPosition.y = node.Y;
            }

            public void Execute()
            {
                _node._dialogueEntry.Pos = _newPosition;
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                _node._dialogueEntry.Pos = _oldPosition;
            }
        }

        public class SetNodeReactionsUndoableCommand : IUndoableCommand
        {
            private Reaction _oldL, _newL;
            private Reaction _oldR, _newR;
            private NodeViewModel _node;

            public SetNodeReactionsUndoableCommand(Reaction newL, Reaction newR, NodeViewModel node)
            {
                _newL = newL;
                _newR = newR;
                _node = node;
                _oldL = node.LeftReaction;
                _oldR = node.RightReaction;
            }

            public void Execute()
            {
                _node._dialogueEntry.LeftReaction = _newL;
                _node._dialogueEntry.RightReaction = _newR;
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                _node._dialogueEntry.LeftReaction = _oldL;
                _node._dialogueEntry.RightReaction = _oldR;
            }
        }

        #endregion Undoable Commands

        #region Internal Data Members

        private bool isSelected = false;

        private ConnectorViewModel incomingConnector = null; //TODO How is this represented in DialogueManager
        private ImpObservableCollection<ConnectorViewModel> outgoingConnectors = null;
        private AddChoiceCommand _addChoiceCommand = null;
        private Dialogue _dialogue = null;
        private DialogueEntry _dialogueEntry = null;
        private CommandExecutor _cmdExec = null;
        private NetworkViewModel _network = null;

        #endregion Internal Data Members

        public NodeViewModel(CommandExecutor cmdExec, DialogueEntry entry, Dialogue dialogue, NetworkViewModel network)
        {
            _dialogue = dialogue;
            _dialogueEntry = entry;
            _cmdExec = cmdExec;
            _network = network;

            incomingConnector = new ConnectorViewModel(_cmdExec, null, this);

            for (int i = 0; i < entry.NumChoices; ++i)
            {
                DialogueChoice choice = entry.Choice(i);
                if (choice != null)
                {
                    OutgoingConnectors.Add(new ConnectorViewModel(_cmdExec, choice, this));
                }
            }
        }

        public NetworkViewModel Network
        {
            get
            {
                return _network;
            }
        }

        public DialogueEntry DialogueEntry
        {
            get
            {
                return _dialogueEntry;
            }
        }

        public ParticipantViewModel ActiveParticipant
        {
            get
            {
                return new ParticipantViewModel(_cmdExec, _dialogueEntry.ActiveParticipant, _dialogue);
            }

            set
            {
                _dialogueEntry.ActiveParticipant = value.Participant;

                OnPropertyChanged("ActiveParticipant");
            }
        }

        public string Content
        {
            get
            {
                return _dialogueEntry.Content;
            }
            set
            {
                if (_dialogueEntry.Content == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetNodeContentUndoableCommand(value, this));

                OnPropertyChanged("Name");
            }
        }

        public double X
        {
            get
            {
                return _dialogueEntry.Pos.x;
            }
            set
            {
                if (_dialogueEntry.Pos.x == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetNodePositionUndoableCommand(new Vector2 { x = value, y = _dialogueEntry.Pos.y }, this));

                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get
            {
                return _dialogueEntry.Pos.y;
            }
            set
            {
                if (_dialogueEntry.Pos.y == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetNodePositionUndoableCommand(new Vector2 { x = _dialogueEntry.Pos.x, y = value }, this));

                OnPropertyChanged("Y");
            }
        }

        public Reaction LeftReaction
        {
            get
            {
                return _dialogueEntry.LeftReaction;
            }
            set
            {
                if (_dialogueEntry.LeftReaction == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetNodeReactionsUndoableCommand(value, RightReaction, this));

                OnPropertyChanged("LeftReaction");
            }
        }

        public Reaction RightReaction
        {
            get
            {
                return _dialogueEntry.RightReaction;
            }
            set
            {
                if (_dialogueEntry.RightReaction == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetNodeReactionsUndoableCommand(LeftReaction, value, this));

                OnPropertyChanged("RightReaction");
            }
        }

        public ImpObservableCollection<ConnectorViewModel> OutgoingConnectors
        {
            get
            {
                if (outgoingConnectors == null)
                {
                    outgoingConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    outgoingConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(connectors_ItemsAdded);
                    outgoingConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(connectors_ItemsRemoved);
                }

                return outgoingConnectors;
            }
        }

        public ConnectorViewModel IncomingConnector
        {
            get
            {
                return incomingConnector;
            }
        }

        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                List<ConnectionViewModel> attachedConnections = new List<ConnectionViewModel>();

                foreach (var connector in this.OutgoingConnectors)
                {
                    if (connector.AttachedConnections != null)
                    {
                        attachedConnections.AddRange(connector.AttachedConnections);
                    }
                }

                if (incomingConnector != null)
                {
                    attachedConnections.AddRange(incomingConnector.AttachedConnections);
                }

                return attachedConnections;
            }
        }

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                {
                    return;
                }

                isSelected = value;

                OnPropertyChanged("IsSelected");
            }
        }

        public ICommand AddChoice
        {
            get
            {
                if (_addChoiceCommand == null)
                {
                    _addChoiceCommand = new AddChoiceCommand(_cmdExec, this, OutgoingConnectors, _dialogueEntry, _dialogue, "");
                }

                return _addChoiceCommand;
            }
        }

        #region Private Methods

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void connectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void connectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
            }
        }

        #endregion Private Methods
    }
}