using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialogueManagerTests
{
    [TestClass]
    public class DialogueEntryTest
    {
        private DialogueManager _mgr;
        private string _dlgName = "A Dialogue";
        private Dialogue _dlg;
        private string _partName = "A Participant";
        private Participant _part;
        private string entryContent1 = "Some content";
        private string entryContent2 = "Some other content";
        private DialogueEntry _entry1;
        private DialogueEntry _entry2;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mgr = new DialogueManager();
            _dlg = _mgr.AddDialogue(_dlgName);
            _part = _dlg.AddParticipant(_partName);
            _entry1 = _dlg.AddEntry(_part, entryContent1);
            _entry2 = _dlg.AddEntry(_part, entryContent2);
        }

        [TestMethod]
        public void ContentReturnsCorrectValue()
        {
            Assert.AreEqual(_entry1.Content, entryContent1);
            Assert.AreEqual(_entry2.Content, entryContent2);
        }

        [TestMethod]
        public void CanSetContents()
        {
            string newContents = "New contents";
            _entry1.Content = newContents;
            Assert.AreEqual(_entry1.Content, newContents);
        }

        [TestMethod]
        public void NumChoicesReturnsCorrectValue()
        {
            Assert.AreEqual(_entry1.NumChoices, 0);

            _dlg.AddChoice(_entry1, "A choice", _entry2);
            Assert.AreEqual(_entry1.NumChoices, 1);

            _dlg.AddChoice(_entry1, "Another choice", _entry2);
            Assert.AreEqual(_entry1.NumChoices, 2);
        }

        [TestMethod]
        public void CanGetChoicesByIndex()
        {
            var choice1 = _dlg.AddChoice(_entry1, "A choice", _entry2);
            Assert.AreEqual(_entry1.Choice(0), choice1);

            var choice2 = _dlg.AddChoice(_entry1, "Another choice", _entry2);
            Assert.AreEqual(_entry1.Choice(1), choice2);
        }

        [TestMethod]
        public void ActiveParticipantReturnsCorrectValue()
        {
            Assert.AreEqual(_part, _entry1.ActiveParticipant);
        }
    }
}