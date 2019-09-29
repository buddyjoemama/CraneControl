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
            var board = ControlBoard.Initialize();

        }

        [TestMethod]
        public void TestPVT()
        {
            Driver.OperatePVT(PvtActions.Right);
            Thread.Sleep(2000);
            Driver.OperatePVT(PvtActions.Off);
        }
    }
}
