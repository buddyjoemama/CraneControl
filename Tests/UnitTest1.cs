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
                    context.CraneOperations.ToDictionary(k => k.OpCode, v=>v);

                Driver.OperateCrane(new ControlboardOperation
                {
                    Action = CraneOperationAction.On,
                    Operation = allOps[CraneOperations.BoomDown]
                });

                Driver.OperateCrane(new ControlboardOperation
                {
                    Action = CraneOperationAction.Off,
                    Operation = allOps[CraneOperations.BoomDown]
                });

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
    }
}
