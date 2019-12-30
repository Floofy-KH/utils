using System.Collections.Generic;

namespace DialogueEditor
{
    public interface IUndoableCommand
    {
        void Execute();

        void Undo();

        void Redo();
    }

    public class CommandExecutor
    {
        private Stack<IUndoableCommand> _undoStack;
        private Stack<IUndoableCommand> _redoStack;

        public int UndoCount { get { return _undoStack.Count; } }
        public int RedoCount { get { return _redoStack.Count; } }

        public CommandExecutor()
        {
            _undoStack = new Stack<IUndoableCommand>();
            _redoStack = new Stack<IUndoableCommand>();
        }

        public void ExecuteCommand(IUndoableCommand cmd)
        {
            if (cmd == null)
            {
                return;
            }

            cmd.Execute();
            _undoStack.Push(cmd);
        }

        public void UndoLatest()
        {
            if (_undoStack.Count == 0)
            {
                return;
            }

            IUndoableCommand cmd = _undoStack.Pop();
            cmd.Undo();
            _redoStack.Push(cmd);
        }

        public void RedoLatest()
        {
            if (_redoStack.Count == 0)
            {
                return;
            }

            IUndoableCommand cmd = _redoStack.Pop();
            cmd.Redo();
            _undoStack.Push(cmd);
        }
    }
}