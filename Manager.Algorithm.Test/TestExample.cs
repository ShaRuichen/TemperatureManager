using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Manager.Algorithm.Test
{
    [TestClass]
    public class TestExample
    {
        [TestMethod]
        [DataRow(3, 4, 5)]
        [DataRow(5, 12, 13)]
        [DataRow(6, 8, 10)]
        public void Test_CountVectorLength(double x, double y, double result)
        {
            Assert.AreEqual(Example.CountVectorLength(x, y), result);
        }
    }
}
