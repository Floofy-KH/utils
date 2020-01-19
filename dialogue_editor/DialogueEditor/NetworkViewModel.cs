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
        #region Internal Data Members

        /// <summary>
        /// The collection of nodes in the network.
        /// </summary>
        private ImpObservableCollection<NodeViewModel> nodes = null;

        /// <summary>
        /// The collection of connections in the network.
        /// </summary>
        private ImpObservableCollection<ConnectionViewModel> connections = null;

        #endregion Internal Data Members

        private Dialogue _dialogue = null;

        public NetworkViewModel(Dialogue dialogue)
        {
            _dialogue = dialogue;

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
                    var model = new NodeViewModel(entry, _dialogue);
                    Nodes.Add(model);

                    entryToModel.Add(entry, model);
                }
            }

            for (int i = 0; i < _dialogue.NumChoices; ++i)
            {
                Choice choice = _dialogue.Choice(i);
                if (choice != null)
                {
                    var connection = new ConnectionViewModel();
                    Connections.Add(connection);

                    if (choice.SourceEntry != null && choice.DestinationEntry != null)
                    {
                        NodeViewModel srcNode = entryToModel[choice.SourceEntry];
                        foreach (ConnectorViewModel connector in srcNode.OutgoingConnectors)
                        {
                            if (connector.Choice == choice)
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
        }

        /// <summary>
        /// The collection of nodes in the network.
        /// </summary>
        public ImpObservableCollection<NodeViewModel> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ImpObservableCollection<NodeViewModel>();
                }

                return nodes;
            }
        }

        /// <summary>
        /// The collection of connections in the network.
        /// </summary>
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

        #region Private Methods

        /// <summary>
        /// Event raised then Connections have been removed.
        /// </summary>
        private void connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectionViewModel connection in e.Items)
            {
                connection.SourceConnector = null;
                connection.DestConnector = null;
            }
        }

        #endregion Private Methods
    }
}