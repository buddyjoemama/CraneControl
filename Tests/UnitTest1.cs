using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CraneWeb.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SerialLib;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Driver.Off();

            using (CraneDbContext context = new CraneDbContext())
            {
                Dictionary<CraneOperations, CraneOperation> allOps =
                    context.CraneOperations.OrderBy(s=>s.ActionSource)
                        .ToDictionary(k => k.OpCode, v=>v);

                foreach(var op in allOps)
                {
                    Driver.OperateCrane(new ControlboardOperation
                    {
                        Action = CraneOperationAction.On,
                        Operation = op.Value
                    });

                    Thread.Sleep(1000);
                }
            }

            Driver.Off();
        }

        [TestMethod]
        public void TestPort()
        {
            Assert.IsTrue(Driver.TestPort("COM9"));
            Assert.IsFalse(Driver.TestPort("COM4"));
        }
    }
}
