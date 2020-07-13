using floofy;
using System;
using System.Diagnostics;
using System.Windows;

namespace DialogueEditor
{
    /// <summary>
    /// Defines a connection between two connectors (aka connection points) of two nodes.
    /// </summary>
    public sealed class ConnectionViewModel : HandlesPropertyChanged
    {
        #region Internal Data Members

        private class SetChoiceDestUndoableCommand : IUndoableCommand
        {
            private ConnectorViewModel _destConnector = null;
            private ConnectionViewModel _connection = null;
            private ConnectionViewModel _prevConnection = null;
            private Choice _choice = null;

            public SetChoiceDestUndoableCommand(ConnectorViewModel destConnector, ConnectionViewModel connection, Choice choice)
            {
                _destConnector = destConnector;
                _connection = connection;
                _choice = choice;
            }

            public void Execute()
            {
                if (_destConnector == _connection.DestConnector)
                {
                    return;
                }

                if (_connection.DestConnector != null)
                {
                    _prevConnection = _connection.DestConnector.AttachedConnection;
                    _connection.DestConnector.AttachedConnection = null;
                    _connection.DestConnector.HotspotUpdated += new EventHandler<EventArgs>(_connection.destConnector_HotspotUpdated);
                }

                _connection.destConnector = _destConnector;

                if (_connection.destConnector != null)
                {
                    Trace.Assert(_connection.destConnector.AttachedConnection == null);

                    _connection.destConnector.AttachedConnection = _connection;
                    _connection.destConnector.HotspotUpdated += new EventHandler<EventArgs>(_connection.destConnector_HotspotUpdated);
                    _connection.DestConnectorHotspot = _connection.destConnector.Hotspot;

                    _choice.DestinationEntry = _destConnector.ParentNode.DialogueEntry;
                }

                _connection.OnPropertyChanged("DestConnector");
            }

            public void Undo()
            {
                //TODO do it
            }

            public void Redo()
            {
                //Execute();
            }
        }

        private ConnectorViewModel sourceConnector = null;
        private ConnectorViewModel destConnector = null;

        private Point sourceConnectorHotspot;
        private Point destConnectorHotspot;

        private CommandExecutor _cmdExe = null;

        private Choice _choice = null;

        #endregion Internal Data Members

        public ConnectionViewModel(CommandExecutor cmdExe, Choice choice)
        {
            _cmdExe = cmdExe;
            _choice = choice;
        }

        /// <summary>
        /// The source connector the connection is attached to.
        /// </summary>
        public ConnectorViewModel SourceConnector
        {
            get
            {
                return sourceConnector;
            }
            set
            {
                if (sourceConnector == value)
                {
                    return;
                }

                if (sourceConnector != null)
                {
                    Trace.Assert(sourceConnector.AttachedConnection == this);

                    sourceConnector.AttachedConnection = null;
                    sourceConnector.HotspotUpdated -= new EventHandler<EventArgs>(sourceConnector_HotspotUpdated);
                }

                sourceConnector = value;

                if (sourceConnector != null)
                {
                    Trace.Assert(sourceConnector.AttachedConnection == null);

                    sourceConnector.AttachedConnection = this;
                    sourceConnector.HotspotUpdated += new EventHandler<EventArgs>(sourceConnector_HotspotUpdated);
                    this.SourceConnectorHotspot = sourceConnector.Hotspot;
                }

                OnPropertyChanged("SourceConnector");
            }
        }

        /// <summary>
        /// The destination connector the connection is attached to.
        /// </summary>
        public ConnectorViewModel DestConnector
        {
            get
            {
                return destConnector;
            }
            set
            {
                _cmdExe.ExecuteCommand(new SetChoiceDestUndoableCommand(value, this, _choice));
            }
        }

        /// <summary>
        /// The source and dest hotspots used for generating connection points.
        /// </summary>
        public Point SourceConnectorHotspot
        {
            get
            {
                return sourceConnectorHotspot;
            }
            set
            {
                sourceConnectorHotspot = value;

                OnPropertyChanged("SourceConnectorHotspot");
            }
        }

        public Point DestConnectorHotspot
        {
            get
            {
                return destConnectorHotspot;
            }
            set
            {
                destConnectorHotspot = value;

                OnPropertyChanged("DestConnectorHotspot");
            }
        }

        #region Private Methods

        /// <summary>
        /// Event raised when the hotspot of the source connector has been updated.
        /// </summary>
        private void sourceConnector_HotspotUpdated(object sender, EventArgs e)
        {
            this.SourceConnectorHotspot = this.SourceConnector.Hotspot;
        }

        /// <summary>
        /// Event raised when the hotspot of the dest connector has been updated.
        /// </summary>
        private void destConnector_HotspotUpdated(object sender, EventArgs e)
        {
            this.DestConnectorHotspot = this.DestConnector.Hotspot;
        }

        #endregion Private Methods
    }
}