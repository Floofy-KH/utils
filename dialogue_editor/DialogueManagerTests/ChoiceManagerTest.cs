using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChoiceManagerTests
{
    [TestClass]
    public class ChoiceManagerTest
    {
        [TestMethod]
        public void ManagerBeginsWithZeroChoices()
        {
            var mgr = new ChoiceManager();

            Assert.AreEqual(mgr.NumChoices, 0);
        }

        [TestMethod]
        public void AddingChoicesIncreasesChoiceCount()
        {
            var mgr = new ChoiceManager();

            mgr.AddChoice("New Choice 1");
            Assert.AreEqual(mgr.NumChoices, 1);

            mgr.AddChoice("New Choice 2");
            Assert.AreEqual(mgr.NumChoices, 2);
        }

        [TestMethod]
        public void RemovingChoicesDecreasesChoiceCount()
        {
            var mgr = new ChoiceManager();

            mgr.AddChoice("New Choice 1");
            mgr.AddChoice("New Choice 2");

            mgr.RemoveChoice("New Choice 1");
            Assert.AreEqual(mgr.NumChoices, 1);

            mgr.RemoveChoice("New Choice 2");
            Assert.AreEqual(mgr.NumChoices, 0);
        }

        [TestMethod]
        public void RetrievingNonExistantChoiceReturnsNull()
        {
            var mgr = new ChoiceManager();

            mgr.AddChoice("A Name");

            var dlgName = mgr.Choice("Not A Name");
            Assert.IsNull(dlgName);
        }

        [TestMethod]
        public void CanGetChoiceByIndex()
        {
            var mgr = new ChoiceManager();

            var addedDlg = mgr.AddChoice("A Name");

            var retrievedDlg = mgr.Choice(0);

            Assert.AreEqual(addedDlg, retrievedDlg);
        }

        [TestMethod]
        public void CanGetChoiceByName()
        {
            var mgr = new ChoiceManager();

            var addedDlg = mgr.AddChoice("A Name");

            var retrievedDlg = mgr.Choice("A Name");

            Assert.AreEqual(addedDlg, retrievedDlg);
        }
    }
}