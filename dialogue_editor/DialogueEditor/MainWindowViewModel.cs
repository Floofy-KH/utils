using floofy;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

        private bool _successful = false;
        public bool Successful { get { return _successful; } }

        private MainWindowViewModel m_model;

        public SaveCommand(MainWindowViewModel model)
        {
            m_model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (m_model.CurrentFile == null)
            {
                var saveFileDlg = new SaveFileDialog()
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (saveFileDlg.ShowDialog() == true)
                {
                    m_model.CurrentFile = saveFileDlg.FileName;
                    Save();
                    if (!_successful)
                    {
                        m_model.CurrentFile = null;
                    }
                }
            }
            else
            {
                Save();
            }
        }

        private void Save()
        {
            if (!m_model.DlgModel.Save(m_model.CurrentFile))
            {
                MessageBox.Show("An error occurred while saving the file.", "Error Saving");
                _successful = false;
            }
            m_model.Dirty = false;
            _successful = true;
        }
    }

    public class SaveAsCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private bool _successful = false;
        public bool Successful { get { return _successful; } }

        private MainWindowViewModel m_model;

        public SaveAsCommand(MainWindowViewModel model)
        {
            m_model = model;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var saveFileDlg = new SaveFileDialog()
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDlg.ShowDialog() == true)
            {
                var oldFile = m_model.CurrentFile;
                m_model.CurrentFile = saveFileDlg.FileName;
                Save();
                if (!_successful)
                {
                    m_model.CurrentFile = oldFile;
                }
            }
        }

        private void Save()
        {
            if (!m_model.DlgModel.Save(m_model.CurrentFile))
            {
                MessageBox.Show("An error occurred while saving the file.", "Error Saving");
                _successful = false;
            }
            m_model.Dirty = false;
            _successful = true;
        }
    }

    #endregion Commands

    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : HandlesPropertyChanged
    {
        #region Internal Data Members

        private CommandExecutor _cmdExec;
        private ICommand _undoCommand;
        private ICommand _redoCommand;
        private ICommand _saveCommand;
        private ICommand _saveAsCommand;
        private DialogueModel _dlgModel = null;

        //private Point currentPoint = new Point();

        #endregion Internal Data Members

        public DialogueModel DlgModel
        {
            get { return _dlgModel; }
            set
            {
                _dlgModel = value;
                OnPropertyChanged("DlgModel");
            }
        }

        public ICommand UndoCommand { get { return _undoCommand; } }
        public ICommand RedoCommand { get { return _redoCommand; } }
        public ICommand SaveCommand { get { return _saveCommand; } }
        public ICommand SaveAsCommand { get { return _saveAsCommand; } }
        public bool Dirty { get; set; }
        public string CurrentFile { get; set; }

        public MainWindowViewModel()
        {
            _cmdExec = new CommandExecutor();

            DlgModel = new DialogueModel(_cmdExec);

            _undoCommand = new UndoCommand(_cmdExec);
            _redoCommand = new RedoCommand(_cmdExec);
            _saveCommand = new SaveCommand(this);
            _saveAsCommand = new SaveAsCommand(this);
            Dirty = false;
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
                DlgModel.Network.Connections.Remove(draggedOutConnector.AttachedConnection);
            }

            //
            // Create a new connection to add to the view-model.
            //
            var connection = new ConnectionViewModel(_cmdExec, draggedOutConnector.Choice)
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
            DlgModel.Network.Connections.Add(connection);

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
            if (connectorDraggedOver == null ||
                connectorDraggedOut.IsChoice == connectorDraggedOver.IsChoice ||
                connectorDraggedOut.ParentNode == connectorDraggedOver.ParentNode)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                DlgModel.Network.Connections.Remove(newConnection);
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
                DlgModel.Network.Connections.Remove(existingConnection);
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
            var nodesCopy = DlgModel.Network.Nodes.ToArray();

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
            DlgModel.Network.Connections.RemoveRange(node.AttachedConnections);

            //
            // Remove the node from the network.
            //
            DlgModel.Network.Nodes.Remove(node);
        }

        /// <summary>
        /// Create a node and add it to the view-model.
        /// </summary>
        public NodeViewModel CreateNode(string name, Point nodeLocation)
        {
            return DlgModel.CreateNode(name, nodeLocation);
        }

        #region Private Methods

        public bool Save()
        {
            if (_saveCommand.CanExecute(null))
            {
                _saveCommand.Execute(null);
                return (_saveCommand as SaveCommand).Successful;
            }
            return false;
        }

        public bool SaveAs()
        {
            if (_saveAsCommand.CanExecute(null))
            {
                _saveAsCommand.Execute(null);
                return (_saveAsCommand as SaveAsCommand).Successful;
            }
            return false;
        }

        public void AddNewDialogue()
        {
            Dirty = true;

            HashSet<string> dlgNames = new HashSet<string>();

            foreach (var dlg in DlgModel.DlgItems)
            {
                dlgNames.Add(dlg.Name);
            }

            string baseName = "Dialogue ";
            int curIndex = 1;

            while (true)
            {
                string name = baseName + curIndex;
                if (!dlgNames.Contains(name))
                {
                    DlgModel.Add(name);
                    break;
                }
                ++curIndex;
            }
        }

        public void OpenFile(string filepath)
        {
            var mgr = DialogueManager.LoadFile(filepath);
            if (mgr == null)
            {
                MessageBox.Show(string.Format("Failed to load {0}", filepath), "Loading Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                CurrentFile = filepath;
                DlgModel = new DialogueModel(mgr, _cmdExec);
            }
        }

        public void OpenDialogue(string dialogueName)
        {
            DlgModel.OpenDialogue(dialogueName);
        }

        public void RemoveItem(string itemName)
        {
            DlgModel.Remove(itemName);
        }

        #endregion Private Methods
    }
}