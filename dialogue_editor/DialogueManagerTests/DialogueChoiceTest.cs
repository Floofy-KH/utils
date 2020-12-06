using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialogueManagerTests
{
    [TestClass]
    public class DialogueChoiceTest
    {
        private DialogueManager _mgr;
        private string _dlgName = "A Dialogue";
        private Dialogue _dlg;
        private string _partName = "A Participant";
        private Participant _part;
        private string entryContent = "Some content";
        private string entryContent2 = "Some other content";
        private DialogueEntry _entry1;
        private DialogueEntry _entry2;
        private string choiceContent = "A choice";
        private DialogueChoice _choice;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mgr = new DialogueManager();
            _dlg = _mgr.AddDialogue(_dlgName);
            _part = _dlg.AddParticipant(_partName);
            _entry1 = _dlg.AddEntry(_part, entryContent);
            _entry2 = _dlg.AddEntry(_part, entryContent2);
            _choice = _dlg.AddChoice(_entry1, choiceContent, _entry2);
        }

        [TestMethod]
        public void ContentReturnsCorrectValue()
        {
            Assert.AreEqual(_choice.Content, choiceContent);
        }

        [TestMethod]
        public void CanSetContent()
        {
            string newContent = "Some new content";
            _choice.Content = newContent;

            Assert.AreEqual(_choice.Content, newContent);
        }

        [TestMethod]
        public void SourceEntryReturnsCorrectValue()
        {
            Assert.AreEqual(_choice.SourceEntry, _entry1);
        }

        [TestMethod]
        public void DestinationEntryReturnsCorrectValue()
        {
            Assert.AreEqual(_choice.DestinationEntry, _entry2);
        }
    }
}