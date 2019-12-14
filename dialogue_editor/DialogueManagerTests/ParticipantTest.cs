using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DialogueManagerTests
{
    [TestClass]
    public class ParticipantTest
    {
        private DialogueManager _mgr;
        private string _dlgName = "A Dialogue";
        private Dialogue _dlg;
        private string _partName = "A Participant";
        private Participant _part;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mgr = new DialogueManager();
            _dlg = _mgr.AddDialogue(_dlgName);
            _part = _dlg.AddParticipant(_partName);
        }

        [TestMethod]
        public void NameReturnsCorrectValue()
        {
            Assert.AreEqual(_partName, _part.Name);
        }

        [TestMethod]
        public void CanSetName()
        {
            string newName = "A new name";
            _part.Name = newName;
            Assert.AreEqual(_part.Name, newName);
        }
    }
}