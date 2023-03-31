using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CiscoWLANGuestUsers;
using ESCPOS_NET;

namespace CiscoWLANGuestUsersTest
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CreateTestUser()
        {
            Control c = new Control();
            c.CreateWLCGuestUser("10.96.80.5", "sec65Guest57_Fehler", "UnitTest", "TestPassword", "Test by Unit Test", 4);
        }

        [TestMethod]
        public void PrintTestLabel()
        {
            Control c = new Control();
            c.PrintTicket(c.GetNetworkPrinter("10.96.5.252"), "TestUser", "TestPW");
        }
    }
}
