using System;
using System.Runtime.InteropServices;

namespace floofy
{
    public class DialogueManager
    {
        #region PInvoke

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr newDialogueManager();

        [DllImport("DialogueManager.dll")]
        public static extern int freeDialogueManager(IntPtr mgr);

        [DllImport("DialogueManager.dll")]
        public static extern int writeDialogues(IntPtr mgr, string filePath, int filePathSize);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr readDialogues(string filePath, int filePathSize);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr addDialogue(IntPtr mgr, string name, int nameSize);

        [DllImport("DialogueManager.dll")]
        public static extern void removeDialogue(IntPtr mgr, string name, int nameSize);

        [DllImport("DialogueManager.dll")]
        public static extern int numDialogues(IntPtr mgr);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr dialogueFromName(IntPtr mgr, string name, int size);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr dialogueFromIndex(IntPtr mgr, int index);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr addParticipant(IntPtr dialogue, string name, int nameSize);

        [DllImport("DialogueManager.dll")]
        public static extern int numParticipants(IntPtr dialogue);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr participantFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr participantFromName(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll")]
        public static extern void removeParticipant(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr addDialogueEntry(IntPtr dialogue, IntPtr part, string name, int size);

        [DllImport("DialogueManager.dll")]
        public static extern int numDialogueEntries(IntPtr dialogue);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr dialogueEntryFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll")]
        public static extern void removeDialogueEntry(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr addChoice(IntPtr dialogue,
                                              IntPtr dialogueEntry,
                                              string name,
                                              int size,
                                              IntPtr destDialogueEntry);

        [DllImport("DialogueManager.dll")]
        public static extern int numChoices(IntPtr dialogue);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr choiceFromIndex(IntPtr dialogue, int index);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr choiceFromName(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll")]
        public static extern void removeChoice(IntPtr dialogue, string name, int size);

        [DllImport("DialogueManager.dll")]
        public static extern void dialogueName(IntPtr dialogue, string name, int bufferSize);

        [DllImport("DialogueManager.dll")]
        public static extern void participantName(IntPtr participant, string name, int bufferSize);

        [DllImport("DialogueManager.dll")]
        public static extern void dialogueEntryContent(IntPtr entry, string content, int bufferSize);

        [DllImport("DialogueManager.dll")]
        public static extern int dialogueEntryNumChoices(IntPtr entry);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr dialogueEntryChoiceFromIndex(IntPtr entry, int index);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr dialogueEntryActiveParticipant(IntPtr entry);

        [DllImport("DialogueManager.dll")]
        public static extern void choiceContent(IntPtr choice, string content, int bufferSize);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr choiceSrcEntry(IntPtr choice);

        [DllImport("DialogueManager.dll")]
        public static extern IntPtr choiceDstEntry(IntPtr choice);

        #endregion PInvoke

        public DialogueManager()
        {
            _ptr = newDialogueManager();
        }

        public DialogueManager(string filepath)
        {
            _ptr = readDialogues(filepath, filepath.Length);
        }

        ~DialogueManager()
        {
            freeDialogueManager(_ptr);
        }

        public Dialogue addDialogue(string name)
        {
            return new Dialogue(addDialogue(_ptr, name, name.Length));
        }

        public Dialogue dialogue(string name)
        {
            return new Dialogue(dialogueFromName(_ptr, name, name.Length));
        }

        public Dialogue dialogue(int index)
        {
            return new Dialogue(dialogueFromIndex(_ptr, index));
        }

        public void removeDialogue(string name)
        {
            removeDialogue(_ptr, name, name.Length);
        }

        public int numDialogues()
        {
            return numDialogues(_ptr);
        }

        public bool write(string filepath)
        {
            return Convert.ToBoolean(writeDialogues(_ptr, filepath, filepath.Length));
        }

        private IntPtr _ptr;
    }

    public class Dialogue
    {
        public Dialogue(IntPtr ptr)
        {
            _ptr = ptr;
        }

        private IntPtr _ptr;
    }

    public class Participant
    {
    }

    public class DialogueEntry
    {
    }

    public class Choice
    {
    }
}