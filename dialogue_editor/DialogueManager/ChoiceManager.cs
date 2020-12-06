using System;
using System.Runtime.InteropServices;
using System.Text;

namespace floofy
{
    public class ChoiceManager
    {
        #region PInvoke

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newChoiceManager();

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int freeChoiceManager(IntPtr mgr);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int writeChoices(IntPtr mgr, string filePath, int filePathSize);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr readChoicesFromFile(string filePath, int filePathSize);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr readChoicesFromContents(string contents, int contentsPathSize);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addNewChoice(IntPtr mgr, string name, int nameSize);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool addExistingChoice(IntPtr mgr, IntPtr choice);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeChoice(IntPtr mgr, string name, int nameSize);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int numChoices(IntPtr mgr);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr choiceFromName(IntPtr mgr, string name, int size);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr choiceFromIndex(IntPtr mgr, int index);

        #endregion PInvoke

        public ChoiceManager()
        {
            _ptr = newChoiceManager();
        }

        public static ChoiceManager LoadFile(string filepath)
        {
            var val = readChoicesFromFile(filepath, filepath.Length);
            if (val == null)
            {
                return null;
            }
            else
            {
                return new ChoiceManager { _ptr = val };
            }
        }

        public static ChoiceManager LoadContents(string content)
        {
            var val = readChoicesFromContents(content, content.Length);
            if (val == null)
            {
                return null;
            }
            else
            {
                return new ChoiceManager { _ptr = val };
            }
        }

        ~ChoiceManager()
        {
            //freeChoiceManager(_ptr);
        }

        public int NumChoices
        {
            get
            {
                return numChoices(_ptr);
            }
        }

        public Choice AddChoice(string name)
        {
            var choice = new Choice(addNewChoice(_ptr, name, name.Length));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public bool AddChoice(Choice choice)
        {
            if (choice == null || choice._ptr == null)
            {
                return false;
            }

            return addExistingChoice(_ptr, choice._ptr);
        }

        public Choice Choice(string name)
        {
            var choice = new Choice(choiceFromName(_ptr, name, name.Length));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public Choice Choice(int index)
        {
            var choice = new Choice(choiceFromIndex(_ptr, index));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public void RemoveChoice(string name)
        {
            Choice choice = this.Choice(name);
            if (choice != null)
            {
                removeChoice(_ptr, name, name.Length);
            }
        }

        public bool Write(string filepath)
        {
            return Convert.ToBoolean(writeChoices(_ptr, filepath, filepath.Length));
        }

        public IntPtr _ptr;
    }

    public class Choice
    {
        #region PInvoke

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeChoice(IntPtr Choice);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void choiceName(IntPtr Choice, StringBuilder name, int size);

        [DllImport("ChoiceManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setChoiceName(IntPtr Choice, StringBuilder name, int size);

        #endregion PInvoke

        public Choice(IntPtr ptr)
        {
            _ptr = ptr;
        }

        ~Choice()
        {
            //freeChoice(_ptr);
        }

        public string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder(1024);
                choiceName(_ptr, sb, 1024);
                return sb.ToString();
            }

            set
            {
                StringBuilder sb = new StringBuilder(value);
                setChoiceName(_ptr, sb, value.Length);
            }
        }

        public override bool Equals(object obj)
        {
            var choice = obj as Choice;

            if (choice == null)
            {
                return false;
            }

            return choice._ptr == _ptr;
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public IntPtr _ptr;
    }
}