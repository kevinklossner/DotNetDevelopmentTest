using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GroceryRegisterSimulation;
using GroceryRegisterSimulation.Fakes;

namespace GroceryRegisterTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class StoreCustomerTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestGetCompletionTime_StraightTimeWithoutStart_AssertEqual()
        {
            IStoreCustomer customer = new StubStoreCustomer(3, 5);
            var time = customer.GetCompletionTime(false);
            Assert.AreEqual(5, time);
        }

        [TestMethod]
        public void TestGetCompletionTime_StraightTimeWithStart_AssertEqual()
        {
            IStoreCustomer customer = new StubStoreCustomer(3,5);
            customer.SetStartTime(3);
            var time = customer.GetCompletionTime(false);
            Assert.AreEqual(8,time);
        }
        [TestMethod]
        public void TestGetCompletionTime_TrainingTimeWithoutStart_AssertEqual()
        {
            IStoreCustomer customer = new StubStoreCustomer(3, 3);
            var time = customer.GetCompletionTime(true);
            Assert.AreEqual(6, time);
        }

        [TestMethod]
        public void TestGetCompletionTime_TrainingTimeWithStart_AssertEqual()
        {
            IStoreCustomer customer = new StubStoreCustomer(3, 5);
            customer.SetStartTime(3);
            var time = customer.GetCompletionTime(false);
            Assert.AreEqual(8, time);
        }

    }
}
