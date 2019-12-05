using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialogueManagerTests
{
    [TestClass]
    public class DialogueManagerTest
    {
        [TestMethod]
        public void ManagerBeginsWithZeroDialogues()
        {
            var mgr = new DialogueManager();

            Assert.AreEqual(mgr.NumDialogues, 0);
        }

        [TestMethod]
        public void AddingDialoguesIncreasesDialogueCount()
        {
            var mgr = new DialogueManager();

            mgr.AddDialogue("New Dialogue 1");
            Assert.AreEqual(mgr.NumDialogues, 1);

            mgr.AddDialogue("New Dialogue 2");
            Assert.AreEqual(mgr.NumDialogues, 2);
        }

        [TestMethod]
        public void RemovingDialoguesDecreasesDialogueCount()
        {
            var mgr = new DialogueManager();

            mgr.AddDialogue("New Dialogue 1");
            mgr.AddDialogue("New Dialogue 2");

            mgr.RemoveDialogue("New Dialogue 1");
            Assert.AreEqual(mgr.NumDialogues, 1);

            mgr.RemoveDialogue("New Dialogue 2");
            Assert.AreEqual(mgr.NumDialogues, 0);
        }

        [TestMethod]
        public void RetrievingNonExistantDialogueReturnsNull()
        {
            var mgr = new DialogueManager();

            var dlg0 = mgr.Dialogue(0);
            Assert.IsNull(dlg0);

            var dlg1 = mgr.Dialogue(1);
            Assert.IsNull(dlg1);

            mgr.AddDialogue("A Name");

            var dlg2 = mgr.Dialogue(2);
            Assert.IsNull(dlg2);

            var dlgName = mgr.Dialogue("Not A Name");
            Assert.IsNull(dlgName);
        }

        [TestMethod]
        public void CanGetDialogueByIndex()
        {
            var mgr = new DialogueManager();

            var addedDlg = mgr.AddDialogue("A Name");

            var retrievedDlg = mgr.Dialogue(0);

            Assert.AreEqual(addedDlg, retrievedDlg);
        }

        [TestMethod]
        public void CanGetDialogueByName()
        {
            var mgr = new DialogueManager();

            var addedDlg = mgr.AddDialogue("A Name");

            var retrievedDlg = mgr.Dialogue("A Name");

            Assert.AreEqual(addedDlg, retrievedDlg);
        }
    }
}