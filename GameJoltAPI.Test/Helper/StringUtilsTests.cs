using NUnit.Framework;
using FluentAssertions;

namespace Jolt.NET.Helper.Tests
{
    [TestFixture]
    public class StringUtilsTests
    {
        [TestCase("http://www.example.org/find/something/useful", 
                  "http://www.example.org", new [] { "find", "something", "useful" })]
        [TestCase("http://www.example.org/test", "http://www.example.org", new [] { "test" })]
        [TestCase("http://www.example.org", "http://www.example.org", null)]
        public void
        UrlCombine_CorrectCombined(string expected, string basePath, string[] urlParts)
        {
            var result = StringUtils.UrlCombine(basePath, urlParts);
            result.Should().BeEquivalentTo(expected);
        }
    }
}