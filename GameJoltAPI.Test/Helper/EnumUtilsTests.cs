using System;
using FluentAssertions;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace Jolt.NET.Helper.Tests
{
    [TestFixture]
    public class EnumUtilsTests
    {
        internal enum internalEnum
        {
            [Display(Description = "Testbeschreibung")]
            test1,
            test2
        }

        [Test] public void 
        ParseEnumDescription_ValidDescription_ReturnEnum()
        {
            var result = EnumUtils.ParseEnumDescription<internalEnum>("Testbeschreibung");
            result.ShouldBeEquivalentTo(internalEnum.test1);
        }

        [Test] public void 
        ParseEnumDescription_EmptyDescription_ShouldReturnEnumByName()
        {
            var result = EnumUtils.ParseEnumDescription<internalEnum>("test2");
            result.ShouldBeEquivalentTo(internalEnum.test2);
        }

        [Test] public void
        ParseEnumDescription_InvalidType_ShouldThrow()
        {
            var ex = Assert.Catch<InvalidOperationException>(() => EnumUtils.ParseEnumDescription<string>("test"));
            ex.GetType().Should().Be(typeof(InvalidOperationException));
        }


        [TestCase("Testbeschreibung", internalEnum.test1)]
        [TestCase("test2", internalEnum.test2)]
        public void
        GetDescription_ValidEnum_ReturnString(string expected, object _enum)
        {
            var result = EnumUtils.GetDescription((internalEnum)_enum);
            result.ShouldBeEquivalentTo(expected);
        }

        [Test] public void
        GetDescription_NullParameter_ShouldThrow()
        {
            var ex = Assert.Catch<Exception>(() => EnumUtils.GetDescription(null));
            ex.Message.Should().Contain("Parameter value has to be provided");
        }
    }
}