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
            _cmd = new NodeViewModel.AddChoiceUndoableCommand(node, outgoingConnectors, entry, dialogue, content);
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
            private string _content;

            public AddChoiceUndoableCommand(NodeViewModel node, ImpObservableCollection<ConnectorViewModel> outgoingConnectors, DialogueEntry entry, Dialogue dialogue, string content)
            {
                _outgoingConnectors = outgoingConnectors;
                _dialogue = dialogue;
                _dialogueEntry = entry;
                _content = content;
                _node = node;
            }

            public void Execute()
            {
                _connector = new ConnectorViewModel(_dialogue.AddChoice(_dialogueEntry, _content), _node);
                _outgoingConnectors.Add(_connector); //TODO create new choice in DialogueManager
            }

            public void Undo()
            {
                _dialogue.RemoveChoice(_content);
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
                _node._content = _newContent;
                _node._dialogueEntry.Content = _newContent;
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                _node._content = _oldContent;
                _node._dialogueEntry.Content = _oldContent;
            }
        }

        #endregion Undoable Commands

        #region Internal Data Members

        private string _content = string.Empty;
        private double x = 0;
        private double y = 0;
        private bool isSelected = false;

        private ConnectorViewModel incomingConnector = null; //TODO How is this represented in DialogueManager
        private ImpObservableCollection<ConnectorViewModel> outgoingConnectors = null;
        private AddChoiceCommand _addChoiceCommand = null;
        private Dialogue _dialogue = null;
        private DialogueEntry _dialogueEntry = null;
        private CommandExecutor _cmdExec = null;

        #endregion Internal Data Members

        public NodeViewModel(CommandExecutor cmdExec)
        {
            _cmdExec = cmdExec;

            incomingConnector = new ConnectorViewModel(null, this);
        }

        public NodeViewModel(CommandExecutor cmdExec, DialogueEntry entry, Dialogue dialogue)
        {
            _content = entry.Content;
            _dialogue = dialogue;
            _dialogueEntry = entry;
            _cmdExec = cmdExec;

            incomingConnector = new ConnectorViewModel(null, this);

            for (int i = 0; i < entry.NumChoices; ++i)
            {
                Choice choice = entry.Choice(i);
                if (choice != null)
                {
                    OutgoingConnectors.Add(new ConnectorViewModel(choice, this));
                }
            }
        }

        public DialogueEntry DialogueEntry
        {
            get
            {
                return _dialogueEntry;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (_content == value)
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
                return x;
            }
            set
            {
                if (x == value)
                {
                    return;
                }

                x = value;

                OnPropertyChanged("X");
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (y == value)
                {
                    return;
                }

                y = value;

                OnPropertyChanged("Y");
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
                    if (connector.AttachedConnection != null)
                    {
                        attachedConnections.Add(connector.AttachedConnection);
                    }
                }

                if (incomingConnector != null)
                {
                    attachedConnections.Add(incomingConnector.AttachedConnection);
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
                    _addChoiceCommand = new AddChoiceCommand(_cmdExec, this, OutgoingConnectors, _dialogueEntry, _dialogue, "New choice");
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