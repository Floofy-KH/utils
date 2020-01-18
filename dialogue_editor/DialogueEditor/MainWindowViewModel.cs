using floofy;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;

namespace DialogueEditor
{
    #region Commands

    public class UndoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CommandExecutor _cmdExec;

        public UndoCommand(CommandExecutor cmdExec)
        {
            _cmdExec = cmdExec;
        }

        public bool CanExecute(object parameter)
        {
            return _cmdExec != null && _cmdExec.UndoCount > 0;
        }

        public void Execute(object parameter)
        {
            _cmdExec.UndoLatest();
        }
    }

    public class RedoCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private CommandExecutor _cmdExec;

        public RedoCommand(CommandExecutor cmdExec)
        {
            _cmdExec = cmdExec;
        }

        public bool CanExecute(object parameter)
        {
            return _cmdExec != null && _cmdExec.RedoCount > 0;
        }

        public void Execute(object parameter)
        {
            _cmdExec.RedoLatest();
        }
    }

    public class SaveCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public SaveCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }

    public class SaveAsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public SaveAsCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
        }
    }

    #endregion Commands

    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : HandlesPropertyChanged
    {
        #region Internal Data Members

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel network = null;

        public DialogueModel DlgModel { get; set; }
        public ICommand UndoCommand { get { return _undoCommand; } }
        public ICommand RedoCommand { get { return _redoCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }
        public ICommand SaveAsCommand { get { return _saveAsCommand; } }
        public bool Dirty { get; set; }

        private CommandExecutor _cmdExec;
        private ICommand _undoCommand;
        private ICommand _redoCommand;
        private ICommand _saveCommand;
        private ICommand _saveAsCommand;
        private string _currentFile = null;

        //private Point currentPoint = new Point();

        #endregion Internal Data Members

        public MainWindowViewModel()
        {
            _cmdExec = new CommandExecutor();

            DlgModel = new DialogueModel(_cmdExec);

            _undoCommand = new UndoCommand(_cmdExec);
            _redoCommand = new RedoCommand(_cmdExec);
            _saveCommand = new SaveCommand();
            _saveAsCommand = new SaveAsCommand();
            Dirty = false;

            // Add some test data to the view-model.
            PopulateWithTestData();
        }

        /// <summary>
        /// This is the network that is displayed in the window.
        /// It is the main part of the view-model.
        /// </summary>
        public NetworkViewModel Network
        {
            get
            {
                return network;
            }
            set
            {
                network = value;

                OnPropertyChanged("Network");
            }
        }

        /// <summary>
        /// Called when the user has started to drag out a connector, thus creating a new connection.
        /// </summary>
        public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
        {
            if (draggedOutConnector.AttachedConnection != null)
            {
                //
                // There is an existing connection attached to the connector that has been dragged out.
                // Remove the existing connection from the view-model.
                //
                this.Network.Connections.Remove(draggedOutConnector.AttachedConnection);
            }

            //
            // Create a new connection to add to the view-model.
            //
            var connection = new ConnectionViewModel
            {
                //
                // Link the source connector to the connector that was dragged out.
                //
                SourceConnector = draggedOutConnector,

                //
                // Set the position of destination connector to the current position of the mouse cursor.
                //
                DestConnectorHotspot = curDragPoint
            };

            //
            // Add the new connection to the view-model.
            //
            this.Network.Connections.Add(connection);

            return connection;
        }

        /// <summary>
        /// Called as the user continues to drag the connection.
        /// </summary>
        public void ConnectionDragging(ConnectionViewModel connection, Point curDragPoint)
        {
            //
            // Update the destination connection hotspot while the user is dragging the connection.
            //
            connection.DestConnectorHotspot = curDragPoint;
        }

        /// <summary>
        /// Called when the user has finished dragging out the new connection.
        /// </summary>
        public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                this.Network.Connections.Remove(newConnection);
                return;
            }

            //
            // The user has dragged the connection on top of another valid connector.
            //

            var existingConnection = connectorDraggedOver.AttachedConnection;
            if (existingConnection != null)
            {
                //
                // There is already a connection attached to the connector that was dragged over.
                // Remove the existing connection from the view-model.
                //
                this.Network.Connections.Remove(existingConnection);
            }

            //
            // Finalize the connection by attaching it to the connector
            // that the user dropped the connection on.
            //
            newConnection.DestConnector = connectorDraggedOver;
        }

        /// <summary>
        /// Delete the currently selected nodes from the view-model.
        /// </summary>
        public void DeleteSelectedNodes()
        {
            // Take a copy of the nodes list so we can delete nodes while iterating.
            var nodesCopy = this.Network.Nodes.ToArray();

            foreach (var node in nodesCopy)
            {
                if (node.IsSelected)
                {
                    DeleteNode(node);
                }
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(NodeViewModel node)
        {
            //
            // Remove all connections attached to the node.
            //
            this.Network.Connections.RemoveRange(node.AttachedConnections);

            //
            // Remove the node from the network.
            //
            this.Network.Nodes.Remove(node);
        }

        /// <summary>
        /// Create a node and add it to the view-model.
        /// </summary>
        public NodeViewModel CreateNode(string name, Point nodeLocation)
        {
            var node = new NodeViewModel(name)
            {
                X = nodeLocation.X,
                Y = nodeLocation.Y
            };

            node.OutgoingConnectors.Add(new ConnectorViewModel());

            //
            // Add the new node to the view-model.
            //
            this.Network.Nodes.Add(node);

            return node;
        }

        #region Private Methods

        /// <summary>
        /// A function to conveniently populate the view-model with test data.
        /// </summary>
        private void PopulateWithTestData()
        {
            //
            // Create a network, the root of the view-model.
            //
            this.Network = new NetworkViewModel();

            //
            // Create some nodes and add them to the view-model.
            //
            var node1 = CreateNode("Node1", new Point(10, 10));
            var node2 = CreateNode("Node2", new Point(200, 10));

            //
            // Create a connection between the nodes.
            //
            var connection = new ConnectionViewModel
            {
                SourceConnector = node1.OutgoingConnectors[0],
                DestConnector = node2.IncomingConnector
            };

            //
            // Add the connection to the view-model.
            //
            this.Network.Connections.Add(connection);
        }

        public bool Save()
        {
            if (_currentFile == null)
            {
                return SaveAs();
            }
            else
            {
                if (!DlgModel.Save(_currentFile))
                {
                    MessageBox.Show("An error occurred while saving the file.", "Error Saving");
                    return false;
                }
                Dirty = false;
                return true;
            }
        }

        public bool SaveAs()
        {
            var saveFileDlg = new SaveFileDialog()
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDlg.ShowDialog() == true)
            {
                _currentFile = saveFileDlg.FileName;
                return Save();
            }

            return false;
        }

        public void AddNewDialogue()
        {
            Dirty = true;

            DlgModel.Add("New Dialogue");
        }

        public void OpenFile(string filepath)
        {
            var mgr = DialogueManager.Load(filepath);
            if (mgr == null)
            {
                MessageBox.Show(string.Format("Failed to load {0}", filepath), "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                _currentFile = filepath;
                DlgModel = new DialogueModel(mgr, _cmdExec);
            }
        }

        public void OpenDialogue(string dialogueName)
        {
        }

        public void RemoveItem(string itemName)
        {
            DlgModel.Remove(itemName);
        }

        #endregion Private Methods
    }
}