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
            var part0 = _dlg.Participant(0);
            Assert.IsNull(part0);

            var part1 = _dlg.Participant(1);
            Assert.IsNull(part1);

            _dlg.AddParticipant("A Name");

            var part2 = _dlg.Participant(2);
            Assert.IsNull(part2);

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
        public void RetrievingNonExistantEntryReturnsNull()
        {
            var entry0 = _dlg.Entry(0);
            Assert.IsNull(entry0);

            var entry1 = _dlg.Entry(1);
            Assert.IsNull(entry1);

            var addedPart = _dlg.AddParticipant("A Participant");
            _dlg.AddEntry(addedPart, "A Name");

            var entry2 = _dlg.Entry(2);
            Assert.IsNull(entry2);
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
        public void CanGetEntryByName()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry = _dlg.AddEntry(addedPart, "An Entry");

            var retrievedEntry = _dlg.AddEntry(addedPart, "Another Entry");
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

            _dlg.AddChoice(addedEntry1, "A Choice", addedEntry2);
            _dlg.AddChoice(addedEntry2, "Another Choice", addedEntry1);
            _dlg.RemoveChoice("A Choice");
            Assert.AreEqual(_dlg.NumChoices, 1);
            _dlg.RemoveChoice("Another Choice");
            Assert.AreEqual(_dlg.NumChoices, 0);
        }

        [TestMethod]
        public void RetrievingNonExistantChoiceReturnsNull()
        {
            var choice0 = _dlg.Choice(0);
            Assert.IsNull(choice0);

            var choice1 = _dlg.Choice(1);
            Assert.IsNull(choice1);

            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry1 = _dlg.AddEntry(addedPart, "An Entry");
            var addedEntry2 = _dlg.AddEntry(addedPart, "Another Entry");
            _dlg.AddChoice(addedEntry1, "A Name", addedEntry2);

            var choice2 = _dlg.Choice(2);
            Assert.IsNull(choice2);

            var choiceName = _dlg.Choice("Not A Name");
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

        [TestMethod]
        public void CanGetChoiceByName()
        {
            var addedPart = _dlg.AddParticipant("A Participant");
            var addedEntry1 = _dlg.AddEntry(addedPart, "An Entry");
            var addedEntry2 = _dlg.AddEntry(addedPart, "Another Entry");
            var addedChoice = _dlg.AddChoice(addedEntry1, "A Choice", addedEntry2);

            var retrievedChoice = _dlg.Choice("A Choice");
            Assert.AreEqual(addedChoice, retrievedChoice);
        }
    }
}