using System;
using System.Runtime.InteropServices;
using System.Text;

namespace floofy
{
    public class DialogueManager
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr newDialogueManager();

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int freeDialogueManager(IntPtr mgr);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int writeDialogues(IntPtr mgr, string filePath, int filePathSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr readDialogues(string filePath, int filePathSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addNewDialogue(IntPtr mgr, string name, int nameSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool addExistingDialogue(IntPtr mgr, IntPtr dlg);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeDialogue(IntPtr mgr, string name, int nameSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int numDialogues(IntPtr mgr);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueFromName(IntPtr mgr, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueFromIndex(IntPtr mgr, int index);

        #endregion PInvoke

        public DialogueManager()
        {
            _ptr = newDialogueManager();
        }

        public static DialogueManager Load(string filepath)
        {
            var val = readDialogues(filepath, filepath.Length);
            if (val == null)
            {
                return null;
            }
            else
            {
                return new DialogueManager { _ptr = val };
            }
        }

        ~DialogueManager()
        {
            //freeDialogueManager(_ptr);
        }

        public int NumDialogues
        {
            get
            {
                return numDialogues(_ptr);
            }
        }

        public Dialogue AddDialogue(string name)
        {
            var dlg = new Dialogue(addNewDialogue(_ptr, name, name.Length));
            if (dlg._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return dlg;
            }
        }

        public bool AddDialogue(Dialogue dlg)
        {
            if (dlg == null || dlg._ptr == null)
            {
                return false;
            }

            return addExistingDialogue(_ptr, dlg._ptr);
        }

        public Dialogue Dialogue(string name)
        {
            var dlg = new Dialogue(dialogueFromName(_ptr, name, name.Length));
            if (dlg._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return dlg;
            }
        }

        public Dialogue Dialogue(int index)
        {
            var dlg = new Dialogue(dialogueFromIndex(_ptr, index));
            if (dlg._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return dlg;
            }
        }

        public void RemoveDialogue(string name)
        {
            Dialogue dlg = this.Dialogue(name);
            if (dlg != null)
            {
                removeDialogue(_ptr, name, name.Length);
            }
        }

        public bool Write(string filepath)
        {
            return Convert.ToBoolean(writeDialogues(_ptr, filepath, filepath.Length));
        }

        public IntPtr _ptr;
    }

    public class Dialogue
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addParticipant(IntPtr dialogue, string name, int nameSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int numParticipants(IntPtr dialogue);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr participantFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr participantFromName(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeParticipant(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addDialogueEntry(IntPtr dialogue, IntPtr part, string content, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int numDialogueEntries(IntPtr dialogue);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueEntryFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeDialogueEntry(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addChoiceWithDest(IntPtr dialogue,
                                              IntPtr dialogueEntry,
                                              string name,
                                              int size,
                                              IntPtr destDialogueEntry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addChoice(IntPtr dialogue,
                                              IntPtr dialogueEntry,
                                              string name,
                                              int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int numChoices(IntPtr dialogue);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr choiceFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr choiceFromName(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeChoice(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dialogueName(IntPtr dialogue, StringBuilder name, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueName(IntPtr dialogue, StringBuilder name, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void freeDialogue(IntPtr dialogue);

        #endregion PInvoke

        public Dialogue(IntPtr ptr)
        {
            _ptr = ptr;
        }

        ~Dialogue()
        {
            //freeDialogue(_ptr);
        }

        public int NumParticipants
        {
            get
            {
                return numParticipants(_ptr);
            }
        }

        public int NumEntries
        {
            get
            {
                return numDialogueEntries(_ptr);
            }
        }

        public int NumChoices
        {
            get
            {
                return numChoices(_ptr);
            }
        }

        public string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder(1024);
                dialogueName(_ptr, sb, 1024);
                return sb.ToString();
            }

            set
            {
                StringBuilder sb = new StringBuilder(value);
                setDialogueName(_ptr, sb, value.Length);
            }
        }

        public Participant AddParticipant(string name)
        {
            var part = new Participant(addParticipant(_ptr, name, name.Length));
            if (part._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return part;
            }
        }

        public Participant Participant(int index)
        {
            var part = new Participant(participantFromIndex(_ptr, index));
            if (part._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return part;
            }
        }

        public Participant Participant(string name)
        {
            var part = new Participant(participantFromName(_ptr, name, name.Length));
            if (part._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return part;
            }
        }

        public void RemoveParticipant(string name)
        {
            removeParticipant(_ptr, name, name.Length);
        }

        public DialogueEntry AddEntry(Participant participant, string content)
        {
            var entry = new DialogueEntry(addDialogueEntry(_ptr, participant._ptr, content, content.Length));
            if (entry._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return entry;
            }
        }

        public DialogueEntry Entry(int index)
        {
            var entry = new DialogueEntry(dialogueEntryFromIndex(_ptr, index));
            if (entry._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return entry;
            }
        }

        public void RemoveEntry(int index)
        {
            removeDialogueEntry(_ptr, index);
        }

        public Choice AddChoice(DialogueEntry src, string name, DialogueEntry dst)
        {
            var choice = new Choice(addChoiceWithDest(_ptr, src._ptr, name, name.Length, dst._ptr));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public Choice AddChoice(DialogueEntry src, string name)
        {
            var choice = new Choice(addChoice(_ptr, src._ptr, name, name.Length));
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

        public void RemoveChoice(string name)
        {
            removeChoice(_ptr, name, name.Length);
        }

        public override bool Equals(object obj)
        {
            var dlg = obj as Dialogue;

            if (dlg == null)
            {
                return false;
            }

            return dlg._ptr == _ptr;
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public IntPtr _ptr;
    }

    public class Participant
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void participantName(IntPtr participant, StringBuilder name, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setParticipantName(IntPtr participant, StringBuilder name, int bufferSize);

        #endregion PInvoke

        public Participant(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public string Name
        {
            get
            {
                StringBuilder sb = new StringBuilder(1024);
                participantName(_ptr, sb, 1024);
                return sb.ToString();
            }

            set
            {
                StringBuilder sb = new StringBuilder(value);
                setParticipantName(_ptr, sb, value.Length);
            }
        }

        public override bool Equals(object obj)
        {
            var part = obj as Participant;

            if (part == null)
            {
                return false;
            }

            return part._ptr == _ptr;
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public IntPtr _ptr;
    }

    public class DialogueEntry
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dialogueEntryContent(IntPtr entry, StringBuilder content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryContent(IntPtr entry, StringBuilder content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dialogueEntryNumChoices(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueEntryChoiceFromIndex(IntPtr entry, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueEntryActiveParticipant(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryActiveParticipant(IntPtr entry, IntPtr participant);

        #endregion PInvoke

        public DialogueEntry(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public int NumChoices
        {
            get
            {
                return dialogueEntryNumChoices(_ptr);
            }
        }

        public string Content
        {
            get
            {
                StringBuilder sb = new StringBuilder(1024);
                dialogueEntryContent(_ptr, sb, 1024);
                return sb.ToString();
            }

            set
            {
                StringBuilder sb = new StringBuilder(value);
                setDialogueEntryContent(_ptr, sb, value.Length);
            }
        }

        public Participant ActiveParticipant
        {
            get
            {
                if (_ptr == IntPtr.Zero)
                {
                    return null;
                }
                else
                {
                    return new Participant(dialogueEntryActiveParticipant(_ptr));
                }
            }

            set
            {
                if (_ptr != IntPtr.Zero)
                {
                    setDialogueEntryActiveParticipant(_ptr, value._ptr);
                }
            }
        }

        public Choice Choice(int index)
        {
            return new Choice(dialogueEntryChoiceFromIndex(_ptr, index));
        }

        public override bool Equals(object obj)
        {
            var entry = obj as DialogueEntry;

            if (entry == null)
            {
                return false;
            }

            return entry._ptr == _ptr;
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public IntPtr _ptr;
    }

    public class Choice
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void choiceContent(IntPtr choice, StringBuilder content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setChoiceContent(IntPtr choice, StringBuilder content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr choiceSrcEntry(IntPtr choice);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr choiceDstEntry(IntPtr choice);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setChoiceDstEntry(IntPtr choice, IntPtr dst);

        #endregion PInvoke

        public Choice(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public string Content
        {
            get
            {
                StringBuilder sb = new StringBuilder(1024);
                choiceContent(_ptr, sb, 1024);
                return sb.ToString();
            }

            set
            {
                StringBuilder sb = new StringBuilder(value);
                setChoiceContent(_ptr, sb, value.Length);
            }
        }

        public DialogueEntry SourceEntry
        {
            get
            {
                var entry = new DialogueEntry(choiceSrcEntry(_ptr));
                if (entry._ptr == IntPtr.Zero)
                {
                    return null;
                }
                else
                {
                    return entry;
                }
            }
        }

        public DialogueEntry DestinationEntry
        {
            get
            {
                var entry = new DialogueEntry(choiceDstEntry(_ptr));
                if (entry._ptr == IntPtr.Zero)
                {
                    return null;
                }
                else
                {
                    return entry;
                }
            }

            set
            {
                setChoiceDstEntry(_ptr, value._ptr);
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