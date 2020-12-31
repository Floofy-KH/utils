using floofy;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace DialogueEditor
{
    /// <summary>
    /// Defines a connector (aka connection point) that can be attached to a node and is used to connect the node to another node.
    /// </summary>
    public sealed class ConnectorViewModel : HandlesPropertyChanged
    {
        #region Internal Data Members

        /// <summary>
        /// The hotspot (or center) of the connector.
        /// This is pushed through from ConnectorItem in the UI.
        /// </summary>
        private Point hotspot;

        private DialogueChoice _dialogueChoice;

        private CommandExecutor _cmdExec = null;

        #endregion Internal Data Members

        public class SetConnectorContentUndoableCommand : IUndoableCommand
        {
            private string _oldContent, _newContent;
            private ConnectorViewModel _connector;

            public SetConnectorContentUndoableCommand(string newContent, ConnectorViewModel connector)
            {
                _newContent = newContent;
                _connector = connector;
                _oldContent = _connector.Content;
            }

            public void Execute()
            {
                _connector._dialogueChoice.Content = _newContent;
            }

            public void Redo()
            {
                Execute();
            }

            public void Undo()
            {
                _connector._dialogueChoice.Content = _oldContent;
            }
        }

        public class GuidCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            public GuidCommand(DialogueChoice dlgChoice)
            {
                _dlgChoice = dlgChoice;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (_dlgChoice.Guid == null)
                {
                    _dlgChoice.AssignGuid();
                }
                else
                {
                    Clipboard.SetText(_dlgChoice.Guid.ToString());
                }
            }

            private DialogueChoice _dlgChoice;
        }

        public DialogueChoice DialogueChoice
        {
            get { return _dialogueChoice; }
        }

        public bool IsChoice
        {
            get
            {
                return _dialogueChoice != null;
            }
        }

        public ConnectorViewModel(CommandExecutor cmdExec, DialogueChoice dlgChoice, NodeViewModel parent)
        {
            _cmdExec = cmdExec;

            _dialogueChoice = dlgChoice;

            ParentNode = parent;

            AttachedConnections = new List<ConnectionViewModel>();
        }

        /// <summary>
        /// The connection that is attached to this connector, or null if no connection is attached.
        /// </summary>
        public List<ConnectionViewModel> AttachedConnections
        {
            get;
            internal set;
        }

        /// <summary>
        /// The parent node that the connector is attached to, or null if the connector is not attached to any node.
        /// </summary>
        public NodeViewModel ParentNode
        {
            get;
            internal set;
        }

        /// <summary>
        /// The hotspot (or center) of the connector.
        /// This is pushed through from ConnectorItem in the UI.
        /// </summary>
        public Point Hotspot
        {
            get
            {
                return hotspot;
            }
            set
            {
                if (hotspot == value)
                {
                    return;
                }

                hotspot = value;

                OnHotspotUpdated();
            }
        }

        public string Content
        {
            get
            {
                return _dialogueChoice.Content;
            }
            set
            {
                if (_dialogueChoice.Content == value)
                {
                    return;
                }

                _cmdExec.ExecuteCommand(new SetConnectorContentUndoableCommand(value, this));

                OnPropertyChanged("Name");
            }
        }

        public string Guid
        {
            get
            {
                if (_dialogueChoice.Guid == null)
                {
                    return "Assign ID";
                }
                else
                {
                    return "Copy ID";
                }
            }
        }

        public ICommand GuidClicked
        {
            get
            {
                return new GuidCommand(_dialogueChoice);
            }
        }

        /// <summary>
        /// Event raised when the connector hotspot has been updated.
        /// </summary>
        public event EventHandler<EventArgs> HotspotUpdated;

        #region Private Methods

        /// <summary>
        /// Called when the connector hotspot has been updated.
        /// </summary>
        private void OnHotspotUpdated()
        {
            OnPropertyChanged("Hotspot");

            if (HotspotUpdated != null)
            {
                HotspotUpdated(this, EventArgs.Empty);
            }
        }

        #endregion Private Methods
    }
}