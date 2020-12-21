using Marketplace.Common.Mappers;
using Marketplace.Common.Models;
using NUnit.Framework;

namespace Tests
{
    public class EncodingTests
    {
        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        [TestCase("test_example")]
        [TestCase("/dsf/df//df/e/")]
        [TestCase("12345!@#$%^&*()")]
        [TestCase("")]
        [TestCase(null)]
        public void encoding_and_decoding_state(string testString)
        {
            var state = new SSOState() { Path = testString };
            var afterCoding = Coding.DecodeState(Coding.EncodeState(state));
            Assert.AreEqual(testString, afterCoding.Path);
        }
    }
}
