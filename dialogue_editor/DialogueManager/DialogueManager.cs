using System;
using System.Runtime.InteropServices;
using System.Text;

namespace floofy
{
    public struct Vector2
    {
        public double x;
        public double y;
    }

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
        private static extern IntPtr readDialoguesFromFile(string filePath, int filePathSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr readDialoguesFromContents(string contents, int contentsPathSize);

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

        public static DialogueManager LoadFile(string filepath)
        {
            var val = readDialoguesFromFile(filepath, filepath.Length);
            if (val == null)
            {
                return null;
            }
            else
            {
                return new DialogueManager { _ptr = val };
            }
        }

        public static DialogueManager LoadContents(string content)
        {
            var val = readDialoguesFromContents(content, content.Length);
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
        private static extern IntPtr addDialogueChoiceWithDest(IntPtr dialogue,
                                              IntPtr dialogueEntry,
                                              string name,
                                              int size,
                                              IntPtr destDialogueEntry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr addDialogueChoice(IntPtr dialogue,
                                              IntPtr dialogueEntry,
                                              string name,
                                              int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int numDialogueChoices(IntPtr dialogue);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueChoiceFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueChoiceFromName(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void removeDialogueChoice(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dialogueName(IntPtr dialogue, byte[] name, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueName(IntPtr dialogue, byte[] name, int bufferSize);

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
                return numDialogueChoices(_ptr);
            }
        }

        public string Name
        {
            get
            {
                byte[] utf8 = new byte[1024];
                dialogueName(_ptr, utf8, 1024);
                return Encoding.UTF8.GetString(utf8).Trim('\0');
            }

            set
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(value);
                setDialogueName(_ptr, utf8, utf8.Length);
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

        public DialogueChoice AddChoice(DialogueEntry src, string name, DialogueEntry dst)
        {
            var choice = new DialogueChoice(addDialogueChoiceWithDest(_ptr, src._ptr, name, name.Length, dst._ptr));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public DialogueChoice AddChoice(DialogueEntry src, string name)
        {
            var choice = new DialogueChoice(addDialogueChoice(_ptr, src._ptr, name, name.Length));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public DialogueChoice Choice(int index)
        {
            var choice = new DialogueChoice(dialogueChoiceFromIndex(_ptr, index));
            if (choice._ptr == IntPtr.Zero)
            {
                return null;
            }
            else
            {
                return choice;
            }
        }

        public DialogueChoice Choice(string name)
        {
            var choice = new DialogueChoice(dialogueChoiceFromName(_ptr, name, name.Length));
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
            removeDialogueChoice(_ptr, name, name.Length);
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
        private static extern void participantName(IntPtr participant, byte[] name, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setParticipantName(IntPtr participant, byte[] name, int bufferSize);

        #endregion PInvoke

        public Participant(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public string Name
        {
            get
            {
                byte[] utf8 = new byte[1024];
                participantName(_ptr, utf8, 1024);
                return Encoding.UTF8.GetString(utf8).Trim('\0');
            }

            set
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(value);
                setParticipantName(_ptr, utf8, utf8.Length);
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

    public enum Reaction
    {
        None,
        Happy,
        Sad,
        Angry,
        Surprised
    }

    public class DialogueEntry
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dialogueEntryContent(IntPtr entry, byte[] content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryContent(IntPtr entry, byte[] content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dialogueEntryNumDialogueChoices(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueEntryDialogueChoiceFromIndex(IntPtr entry, int index);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueEntryActiveParticipant(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryActiveParticipant(IntPtr entry, IntPtr participant);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double dialogueEntryPositionX(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern double dialogueEntryPositionY(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryPosition(IntPtr entry, double x, double y);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dialogueEntryLReaction(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryLReaction(IntPtr entry, int reaction);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int dialogueEntryRReaction(IntPtr entry);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueEntryRReaction(IntPtr entry, int reaction);

        #endregion PInvoke

        public DialogueEntry(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public int NumChoices
        {
            get
            {
                return dialogueEntryNumDialogueChoices(_ptr);
            }
        }

        public string Content
        {
            get
            {
                byte[] utf8 = new byte[1024];
                dialogueEntryContent(_ptr, utf8, 1024);
                return Encoding.UTF8.GetString(utf8).Trim('\0');
            }

            set
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(value);
                setDialogueEntryContent(_ptr, utf8, utf8.Length);
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

        public DialogueChoice Choice(int index)
        {
            return new DialogueChoice(dialogueEntryDialogueChoiceFromIndex(_ptr, index));
        }

        public Vector2 Pos
        {
            get
            {
                return new Vector2 { x = dialogueEntryPositionX(_ptr), y = dialogueEntryPositionY(_ptr) };
            }
            set
            {
                setDialogueEntryPosition(_ptr, value.x, value.y);
            }
        }

        public Reaction LeftReaction
        {
            get
            {
                return (Reaction)dialogueEntryLReaction(_ptr);
            }
            set
            {
                setDialogueEntryLReaction(_ptr, (int)value);
            }
        }

        public Reaction RightReaction
        {
            get
            {
                return (Reaction)dialogueEntryRReaction(_ptr);
            }
            set
            {
                setDialogueEntryRReaction(_ptr, (int)value);
            }
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

    public class DialogueChoice
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void dialogueChoiceContent(IntPtr choice, byte[] content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueChoiceContent(IntPtr choice, byte[] content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueChoiceSrcEntry(IntPtr choice);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueChoiceDstEntry(IntPtr choice);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void setDialogueChoiceDstEntry(IntPtr choice, IntPtr dst);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void assignDialogueChoiceGuid(IntPtr choice);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool dialogueChoiceGuidAssigned(IntPtr choice);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr dialogueChoiceGuid(IntPtr choice);

        #endregion PInvoke

        public DialogueChoice(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public string Content
        {
            get
            {
                byte[] utf8 = new byte[1024];
                dialogueChoiceContent(_ptr, utf8, 1024);
                return Encoding.UTF8.GetString(utf8).Trim('\0');
            }

            set
            {
                byte[] utf8 = Encoding.UTF8.GetBytes(value);
                setDialogueChoiceContent(_ptr, utf8, utf8.Length);
            }
        }

        public DialogueEntry SourceEntry
        {
            get
            {
                var entry = new DialogueEntry(dialogueChoiceSrcEntry(_ptr));
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
                var entry = new DialogueEntry(dialogueChoiceDstEntry(_ptr));
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
                setDialogueChoiceDstEntry(_ptr, value == null ? IntPtr.Zero : value._ptr);
            }
        }

        public Guid Guid
        {
            get
            {
                if (dialogueChoiceGuidAssigned(_ptr))
                {
                    return new Guid(dialogueChoiceGuid(_ptr));
                }
                else
                {
                    return null;
                }
            }
        }

        public void AssignGuid()
        {
            assignDialogueChoiceGuid(_ptr);
        }

        public override bool Equals(object obj)
        {
            var choice = obj as DialogueChoice;

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

    public class Guid
    {
        #region PInvoke

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool guidsAreEqual(IntPtr lhs, IntPtr rhs);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void guidToString(IntPtr guid, byte[] content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr guidFromString(byte[] content, int bufferSize);

        [DllImport("DialogueManager.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool guidIsValid(IntPtr guid);

        #endregion PInvoke

        public Guid()
        {
            _ptr = IntPtr.Zero;
        }

        public Guid(IntPtr ptr)
        {
            _ptr = ptr;
        }

        public Guid(string guidStr)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(guidStr);
            _ptr = guidFromString(utf8, utf8.Length);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Guid;
            if (other == null)
            {
                return false;
            }

            return guidsAreEqual(_ptr, other._ptr);
        }

        public override int GetHashCode()
        {
            return _ptr.GetHashCode();
        }

        public override string ToString()
        {
            if (_ptr == IntPtr.Zero)
                return "";

            int size = 8 + 1 + 4 + 1 + 4 + 1 + 4 + 1 + 12;
            byte[] sb = new byte[size * 2]; // UTF8 -> UTF16, double the chars? Maybe?...
            guidToString(_ptr, sb, size);
            return Encoding.UTF8.GetString(sb).Trim('\0');
        }

        public bool IsValid()
        {
            return _ptr != IntPtr.Zero && guidIsValid(_ptr);
        }

        public IntPtr _ptr;
    }
}