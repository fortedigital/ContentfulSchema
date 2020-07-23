using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using Xunit;

namespace Forte.ContentfulSchema.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void IsGenericShouldReturnTrueForListOfBasicType()
        {
            var testType = new List<int>().GetType();
            var isGenericType = testType.IsGenericType(typeof(List<>));

            Assert.True(isGenericType);
        }

        [Fact]
        public void IsGenericShouldReturnTrueForListTypeWithNoTypeSpecified()
        {
            var isGenericType = typeof(List<>).IsGenericType(typeof(List<>));
            Assert.True(isGenericType);
        }

        [Fact]
        public void IsEnumerableShouldReturnTrueForArrayOfStrings()
        {
            var arr = new string[] { };
            var isEnumerable = arr.GetType().IsEnumerable();

            Assert.True(isEnumerable);
        }

        [Fact]
        public void IsEnumerableShouldReturnTrueForArrayOfContentTypes()
        {
            var arr = new ContentType[] { };
            var isEnumerable = arr.GetType().IsEnumerable();

            Assert.True(isEnumerable);
        }

        [Fact]
        public void IsEnumerableOfStringShouldReturnTrueForListOfStrings()
        {
            var testType = new List<string>().GetType();
            var isEnumerableOfString = testType.IsEnumerableOf<string>();

            Assert.True(isEnumerableOfString);
        }

        [Fact]
        public void IsEnumerableOfStringShouldReturnTrueForArrayOfStrings()
        {
            var testType = new string[]{}.GetType();
            var isEnumerableOfString = testType.IsEnumerableOf<string>();

            Assert.True(isEnumerableOfString);
        }

        [Fact]
        public void IsEnumerableOfIntShouldReturnFalseForListOfStrings()
        {
            var testType = new List<string>().GetType();
            var isEnumerableOfInt = testType.IsEnumerableOf<int>();

            Assert.False(isEnumerableOfInt);
        }

        [Fact]
        public void GetEnumerableElementTypeShouldReturnStringTypeForListOfStrings()
        {
            var testType = new List<string>().GetType();
            var enumerableElementType = testType.GetEnumerableElementType();
            
            Assert.Equal(typeof(string), enumerableElementType);
        }

        [Fact]
        public void GetEnumerableElementTypeShouldReturnStringTypeForStringArrayType()
        {
            var testType = new string[] { }.GetType();
            var enumerableElementType = testType.GetEnumerableElementType();
            var expectedType = typeof(string);

            Assert.Equal(expectedType, enumerableElementType);
        }

        [Fact]
        public void GetEnumerableElementTypeShouldReturnIntTypeForIntArrayType()
        {
            var testType = new int[] { }.GetType();
            var enumerableElementType = testType.GetEnumerableElementType();
            var expectedType = typeof(int);

            Assert.Equal(expectedType, enumerableElementType);
        }

        [Fact]
        public void GetEnumerableElementTypeShouldThrowInvalidOperationExceptionForNotEnumerableType()
        {
            var testType = typeof(string);
            Assert.Throws<InvalidOperationException>(() => testType.GetEnumerableElementType());
        }
    }
}