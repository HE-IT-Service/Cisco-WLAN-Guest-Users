using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CiscoWLANGuestUsers;

namespace CiscoWLANGuestUsersTest
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CreateTestUser()
        {
            Control c = new Control();
            c.RunScript("10.96.80.5", "sec65Guest57_Fehler", "UnitTest", "TestPassword", "Test by Unit Test", 4);
        }
    }
}
