using NUnit.Framework;

namespace BusinessRulesNew.test
{
    [TestFixture]
    class ValidateHelperTest
    {
        [Test]
        public void IsNumericTest_ExpectedBehavior()
        {
            // Arrange
            string strVal = "1.4";
            string strMsgId = "";

            // Act
            var result = ValidateHelper.IsNumeric(strVal, ref strMsgId);

            // Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsValidDateTest_ExpectedBehavior()
        {
            // Arrange
            string txtFrom = "2020/07/01";
            string txtTo = "2020/07/30";
            string strMsgId = "";

            // Act
            var result = ValidateHelper.IsValidDate(txtFrom, txtTo, ref strMsgId);

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsChineseTest_ExpectedBehavior()
        {
            // Arrange
            string strVal = "測試驗證中文";

            // Act
            var result = ValidateHelper.IsChinese(strVal);

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsNumTest_ExpectedBehavior()
        {
            // Arrange
            string strVal = "12345";

            // Act
            var result = ValidateHelper.IsNum(strVal);

            // Assert
            Assert.AreEqual(true, result);
        }

    }
}
