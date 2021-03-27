using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialogueManagerTests
{
    [TestClass]
    public class DialogueTest
    {
        public DialogueTest()
        {
        }

        private DialogueManager _mgr;
        private string _dlgName = "A Dialogue";
        private Dialogue _dlg;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mgr = new DialogueManager();
            _dlg = _mgr.AddDialogue(_dlgName);
        }

        [TestMethod]
        public void NameReturnsCorrectValue()
        {
            Assert.AreEqual(_dlg.Name, _dlgName);
        }

        [TestMethod]
        public void CanSetName()
        {
            string newName = "A new name";
            _dlg.Name = newName;
            Assert.AreEqual(_dlg.Name, newName);
        }

        [TestMethod]
        public void NumParticipantsReturnsZeroByDefault()
        {
            Assert.AreEqual(_dlg.NumParticipants, 0);
        }

        [TestMethod]
        public void AddingParticipantsIncreasesNumParticipants()
        {
            _dlg.AddParticipant("A Participant");
            Assert.AreEqual(_dlg.NumParticipants, 1);

            _dlg.AddParticipant("Another Participant");
            Assert.AreEqual(_dlg.NumParticipants, 2);
        }

        [TestMethod]
        public void RemovingParticipantsDecreasesNumParticipants()
        {
            _dlg.AddParticipant("A Participant");
            _dlg.AddParticipant("Another Participant");
            _dlg.RemoveParticipant("A Participant");
            Assert.AreEqual(_dlg.NumParticipants, 1);
            _dlg.RemoveParticipant("Another Participant");
            Assert.AreEqual(_dlg.NumParticipants, 0);
        }

        [TestMethod]
        public void RetrievingNonExistantParticipantReturnsNull()
        {
            _dlg.AddParticipant("A Name");

            var partName = _dlg.Participant("Not A Name");
            Assert.IsNull(partName);
        }

        [TestMethod]
        public void CanGetParticipantByIndex()
        {
            var addedPart = _dlg.AddParticipant("A Participant");

            var retrievedPart = _dlg.Participant(0);
            Assert.AreEqual(addedPart, retrievedPart);
        }

        [TestMethod]
        public void CanGetParticipantByName()
        {
            var addedPart = _dlg.AddParticipant("A Participant");

            var retrievedPart = _dlg.Participant("A Participant");
            Assert.AreEqual(addedPart, retrievedPart);
        }

        [TestMethod]
        public void NumEntriesReturnsZeroByDefault()
        {
            Assert.AreEqual(_dlg.NumEntries, 0);
        }

        [TestMethod]
        public void AddingEntrysIncreasesNumEntries()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            _dlg.AddEntry(addedPart, "An Entry");
            Assert.AreEqual(_dlg.NumEntries, 1);

            _dlg.AddEntry(addedPart, "Another Entry");
            Assert.AreEqual(_dlg.NumEntries, 2);
        }

        [TestMethod]
        public void RemovingEntrysDecreasesNumEntries()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            _dlg.AddEntry(addedPart, "An Entry");
            _dlg.AddEntry(addedPart, "Another Entry");
            _dlg.RemoveEntry(1);
            Assert.AreEqual(_dlg.NumEntries, 1);
            _dlg.RemoveEntry(0);
            Assert.AreEqual(_dlg.NumEntries, 0);
        }

        [TestMethod]
        public void CanGetEntryByIndex()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry = _dlg.AddEntry(addedPart, "An Entry");

            var retrievedEntry = _dlg.Entry(0);
            Assert.AreEqual(addedEntry, retrievedEntry);
        }

        [TestMethod]
        public void NumChoicesReturnsZeroByDefault()
        {
            Assert.AreEqual(_dlg.NumParticipants, 0);
        }

        [TestMethod]
        public void AddingChoicesIncreasesNumChoices()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry1 = _dlg.AddEntry(addedPart, "An Entry");
            var addedEntry2 = _dlg.AddEntry(addedPart, "Another Entry");

            _dlg.AddChoice(addedEntry1, "A Choice", addedEntry2);
            Assert.AreEqual(_dlg.NumChoices, 1);

            _dlg.AddChoice(addedEntry2, "Another Choice", addedEntry1);
            Assert.AreEqual(_dlg.NumChoices, 2);
        }

        [TestMethod]
        public void RemovingChoicesDecreasesNumChoices()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry1 = _dlg.AddEntry(addedPart, "An Entry");
            var addedEntry2 = _dlg.AddEntry(addedPart, "Another Entry");

            var choice1 = _dlg.AddChoice(addedEntry1, "A Choice", addedEntry2);
            var choice2 = _dlg.AddChoice(addedEntry2, "Another Choice", addedEntry1);
            _dlg.RemoveChoice(choice1);
            Assert.AreEqual(_dlg.NumChoices, 1);
            _dlg.RemoveChoice(choice2);
            Assert.AreEqual(_dlg.NumChoices, 0);
        }

        [TestMethod]
        public void RetrievingNonExistantChoiceReturnsNull()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry1 = _dlg.AddEntry(addedPart, "An Entry");
            var addedEntry2 = _dlg.AddEntry(addedPart, "Another Entry");
            _dlg.AddChoice(addedEntry1, "A Name", addedEntry2);

            var choiceName = _dlg.Choice(2);
            Assert.IsNull(choiceName);
        }

        [TestMethod]
        public void CanGetChoiceByIndex()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry1 = _dlg.AddEntry(addedPart, "An Entry");
            var addedEntry2 = _dlg.AddEntry(addedPart, "Another Entry");
            var addedChoice = _dlg.AddChoice(addedEntry1, "A Choice", addedEntry2);

            var retrievedChoice = _dlg.Choice(0);
            Assert.AreEqual(addedChoice, retrievedChoice);
        }
    }
}