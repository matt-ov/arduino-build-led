using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ArduinoBuildLED.Tests
{
    [TestFixture]
    public class BasicTests
    {
        [Test]
        public void AdditionTest()
        {
            int i = 1 + 1;
            Assert.IsTrue(i == 2);
        }

        [Test]
        public void AdditionTest2()
        {
            int i = 1 + 1;
            Assert.IsTrue(i == 3);
        }

        [Test]
        public void AdditionTest3()
        {
            int i = 1 + 2;
            Assert.IsTrue(i == 3);
        }
    }
}
