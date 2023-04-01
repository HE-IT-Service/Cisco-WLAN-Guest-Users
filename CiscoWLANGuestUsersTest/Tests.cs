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
            c.CreateWLCGuestUser("IP", "Community", "UnitTest", "TestPassword", "Test by Unit Test", 4);
        }

        [TestMethod]
        public void PrintTestLabel()
        {
            Control c = new Control();
            c.PrintTicket(c.GetNetworkPrinter("IP"), "TestUser", "TestPW", "TestWLAN");
        }
        [TestMethod]
        public void OpenCashDrawer()
        {
            Control c = new Control();
            c.OpenCashDrawer(c.GetNetworkPrinter("IP"));
        }
    }
}
