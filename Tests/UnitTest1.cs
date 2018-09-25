using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialLib;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestPort()
        {
            Assert.IsTrue(Driver.TestPort("COM9"));
            Assert.IsFalse(Driver.TestPort("COM4"));
        }
    }
}
