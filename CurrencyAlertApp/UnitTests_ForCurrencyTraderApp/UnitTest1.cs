using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CurrencyAlertApp;
using CurrencyAlertApp.DataAccess;

namespace UnitTests_ForCurrencyTraderApp
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        // Dummy test to confirm unit tests are wired up correctly
        public void TestMethod1()
        {
            //arrange 

            // act         
            int result = DataStore.MulitplyNumbers(10, 2);

            //assert
            int expected = 20;
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        // // Dummy test to confirm unit tests are wired up correctly
        public void TestMethod2()
        {
            //arrange 

            // act         
            int result = DataStore.MulitplyNumbers(10, 2);

            //assert
            int expected = 200000;
            Assert.AreNotEqual(expected, result);
        }
    }
}
