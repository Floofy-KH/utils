using floofy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Utils;

namespace DialogueEditor
{
    /// <summary>
    /// Defines a network of nodes and connections between the nodes.
    /// </summary>
    public sealed class NetworkViewModel
    {
        #region Undoable Commands

        public class AddParticipantUndoableCommand : IUndoableCommand
        {
            private Dialogue _dialogue = null;
            private Participant _participant = null;
            private ParticipantViewModel _partViewModel = null;
            private NetworkViewModel _viewModel = null;
            private CommandExecutor _cmdExec = null;

            public AddParticipantUndoableCommand(CommandExecutor cmdExec, Dialogue dialogue, NetworkViewModel viewModel)
            {
                _dialogue = dialogue;
                _viewModel = viewModel;
                _cmdExec = cmdExec;
            }

            public void Execute()
            {
                _participant = _dialogue.AddParticipant("New participant");
                _partViewModel = new ParticipantViewModel(_cmdExec, _participant, _dialogue);
                _viewModel.participants.Add(_partViewModel);
            }

            public void Undo()
            {
                if (_participant != null)
                {
                    _dialogue.RemoveParticipant(_participant.Name);
                    _viewModel.participants.Remove(_partViewModel);
                    _participant = null;
                }
            }

            public void Redo()
            {
                Execute();
            }
        }

        #endregion Undoable Commands

        #region Internal Data Members

        private ImpObservableCollection<NodeViewModel> nodes = null;
        private ImpObservableCollection<ConnectionViewModel> connections = null;
        private ImpObservableCollection<ParticipantViewModel> participants = null;

        private Dialogue _dialogue = null;
        private CommandExecutor _cmdExec = null;

        #endregion Internal Data Members

        public NetworkViewModel(CommandExecutor cmdExe, Dialogue dialogue)
        {
            _dialogue = dialogue;
            _cmdExec = cmdExe;

            if (_dialogue == null)
            {
                Debug.Assert(false);
                return;
            }

            var entryToModel = new Dictionary<DialogueEntry, NodeViewModel>();

            for (int i = 0; i < _dialogue.NumEntries; ++i)
            {
                DialogueEntry entry = _dialogue.Entry(i);
                if (entry != null)
                {
                    var model = new NodeViewModel(cmdExe, entry, _dialogue, this);
                    Nodes.Add(model);

                    entryToModel.Add(entry, model);
                }
            }

            for (int i = 0; i < _dialogue.NumChoices; ++i)
            {
                DialogueChoice choice = _dialogue.Choice(i);
                if (choice != null)
                {
                    var connection = new ConnectionViewModel(cmdExe, choice);
                    Connections.Add(connection);

                    if (choice.SourceEntry != null && choice.DestinationEntry != null)
                    {
                        NodeViewModel srcNode = entryToModel[choice.SourceEntry];
                        foreach (ConnectorViewModel connector in srcNode.OutgoingConnectors)
                        {
                            if (connector.DialogueChoice.Equals(choice))
                            {
                                connection.SourceConnector = connector;
                                break;
                            }
                        }

                        NodeViewModel dstNode = entryToModel[choice.DestinationEntry];
                        connection.DestConnector = dstNode.IncomingConnector;
                    }
                }
            }

            for (int i = 0; i < _dialogue.NumParticipants; ++i)
            {
                Participant participant = _dialogue.Participant(i);
                if (participant != null)
                {
                    var model = new ParticipantViewModel(cmdExe, participant, _dialogue);
                    Participants.Add(model);
                }
            }
        }

        public ImpObservableCollection<NodeViewModel> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ImpObservableCollection<NodeViewModel>();
                    nodes.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(nodes_ItemsRemoved);
                }

                return nodes;
            }
        }

        public ImpObservableCollection<ConnectionViewModel> Connections
        {
            get
            {
                if (connections == null)
                {
                    connections = new ImpObservableCollection<ConnectionViewModel>();
                    connections.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(connections_ItemsRemoved);
                }

                return connections;
            }
        }

        public ImpObservableCollection<ParticipantViewModel> Participants
        {
            get
            {
                if (participants == null)
                {
                    participants = new ImpObservableCollection<ParticipantViewModel>();
                    participants.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(participants_ItemsRemoved);
                }

                return participants;
            }
        }

        public void AddNewParticipant()
        {
            _cmdExec.ExecuteCommand(new AddParticipantUndoableCommand(_cmdExec, _dialogue, this));
        }

        #region Private Methods

        private void connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectionViewModel connection in e.Items)
            {
                connection.SourceConnector = null;
                connection.DestConnector = null;
            }
        }

        private void nodes_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (NodeViewModel node in e.Items)
            {
                for (int i = 0; i < node.DialogueEntry.NumChoices; ++i)
                {
                    _dialogue.RemoveChoice(node.DialogueEntry.Choice(i));
                }
                _dialogue.RemoveEntry(node.DialogueEntry);
            }
        }

        private void participants_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
        }

        #endregion Private Methods
    }
}