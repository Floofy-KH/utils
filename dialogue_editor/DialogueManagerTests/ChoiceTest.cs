using floofy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChoiceManagerTests
{
    [TestClass]
    public class ChoiceTest
    {
        private ChoiceManager _mgr;
        private string _choiceName = "A Choice";
        private Choice _choice;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _mgr = new ChoiceManager();
            _choice = _mgr.AddChoice(_choiceName);
        }

        [TestMethod]
        public void NameReturnsCorrectValue()
        {
            Assert.AreEqual(_choice.Name, _choiceName);
        }

        [TestMethod]
        public void CanSetName()
        {
            string newName = "Some new name";
            _choice.Name = newName;

            Assert.AreEqual(_choice.Name, newName);
        }
    }
}