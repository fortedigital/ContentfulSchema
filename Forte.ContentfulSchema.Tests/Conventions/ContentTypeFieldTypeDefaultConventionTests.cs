using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Conventions;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Conventions
{
    public class ContentTypeFieldTypeDefaultConventionTests
    {
        public static IEnumerable<object[]> TypesWithLinks => new[]
        {
            new[] {typeof(ContentTypeWithAsset)},
            new[] {typeof(ContentTypeWithEntryLink)},
            new[] {typeof(ContentTypeWithLink)},
            new[] {typeof(ContentTypeWithLinkToNotContentType)},
            new[] {typeof(ContentTypeWithLinkToSealedNotContentType)},
            new[] {typeof(ContentTypeWithInheritedLink)},
            new[] {typeof(ContentTypeWithIndirectlyInheritedLink)},
        };

        private static readonly Dictionary<Type, string> ContentTypeNameLookUp = new Dictionary<Type, string>
        {
            {typeof(EmptyContentType), nameof(EmptyContentType)},
            {typeof(ContentTypeWithString), nameof(ContentTypeWithString)},
            {typeof(ContentTypeWithInteger), nameof(ContentTypeWithInteger)},
            {typeof(ContentTypeWithFloat), nameof(ContentTypeWithFloat)},
            {typeof(ContentTypeWithDecimal), nameof(ContentTypeWithDecimal)},
            {typeof(ContentTypeWithBool), nameof(ContentTypeWithBool)},
            {typeof(ContentTypeWithChar), nameof(ContentTypeWithChar)},
            {typeof(ContentTypeWithByte), nameof(ContentTypeWithByte)},
            {typeof(ContentTypeWithDouble), nameof(ContentTypeWithDouble)},
            {typeof(ContentTypeWithLong), nameof(ContentTypeWithLong)},
            {typeof(ContentTypeWithSByte), nameof(ContentTypeWithSByte)},
            {typeof(ContentTypeWithShort), nameof(ContentTypeWithShort)},
            {typeof(ContentTypeWithUint), nameof(ContentTypeWithUint)},
            {typeof(ContentTypeWithULong), nameof(ContentTypeWithULong)},
            {typeof(ContentTypeWithUShort), nameof(ContentTypeWithUShort)},
            {typeof(ChildOfEmptyContentType), nameof(ChildOfEmptyContentType)},
            {typeof(ContentTypeWithInheritedProperty), nameof(ContentTypeWithInheritedProperty)},
            {typeof(GrandChildOfEmptyContentType), nameof(GrandChildOfEmptyContentType)},
            {typeof(CustomizedContentType), nameof(CustomizedContentType)},
            {typeof(ContentTypeWithTwoLinks), nameof(ContentTypeWithTwoLinks)},
            {typeof(ContentTypeWithAsset), nameof(ContentTypeWithAsset)},
            {typeof(ContentTypeWithEntryLink), nameof(ContentTypeWithEntryLink)},
            {typeof(ContentTypeWithLink), nameof(ContentTypeWithLink)},
            {typeof(ContentTypeWithLinkToNotContentType), nameof(ContentTypeWithLinkToNotContentType)},
            {typeof(ContentTypeWithLinkToSealedNotContentType), nameof(ContentTypeWithLinkToSealedNotContentType)},
            {typeof(ContentTypeWithInheritedLink), nameof(ContentTypeWithInheritedLink)},
            {typeof(ContentTypeWithIndirectlyInheritedLink), nameof(ContentTypeWithIndirectlyInheritedLink)},
            {typeof(NotContentType), nameof(NotContentType)},
            {typeof(SealedNotContentType), nameof(SealedNotContentType)},
            {typeof(ContentTypeWithStringList), nameof(ContentTypeWithStringList)},
            {typeof(ContentTypeWithStringArray), nameof(ContentTypeWithStringArray)},
            {typeof(ContentTypeWithGenericArray), nameof(ContentTypeWithGenericArray)},
        };

        public static readonly IEnumerable<object[]> PropertyInfoAndNamesPairs =
            TestDataForFieldTypeRecognition.PropertyInfoAndNamePairs;

        [Theory]
        [MemberData(nameof(PropertyInfoAndNamesPairs))]
        public void GetFieldTypeShouldReturnAdequateFieldTypeForProperty(PropertyInfoAndNamePair pair)
        {
            var convention = ContentTypeFieldTypeConvention.Default;
            var fieldType = convention.GetFieldType(pair.TestPropertyInfo, ContentTypeNameLookUp);

            Assert.Equal(pair.TypeName, fieldType);
        }

        [Theory]
        [MemberData(nameof(TypesWithLinks))]
        public void GetFieldTypeShouldReturnLinkTypeForTypesWithLinks(Type testType)
        {
            var convention = ContentTypeFieldTypeConvention.Default;
            var fieldType = convention.GetFieldType(testType.GetProperties().First(), ContentTypeNameLookUp);

            Assert.Equal(SystemFieldTypes.Link, fieldType);
        }

        [Fact]
        public void GetFieldTypeShouldReturnArrayTypeForListOfBasicTypesProperty()
        {
            var testType = typeof(ContentTypeWithStringList);
            var convention = ContentTypeFieldTypeConvention.Default;
            var fieldType = convention.GetFieldType(testType.GetProperties().First(), ContentTypeNameLookUp);

            Assert.Equal(SystemFieldTypes.Array, fieldType);
        }

        [Fact]
        public void GetFieldTypeShouldReturnArrayTypeForArrayOfBasicTypesProperty()
        {
            var testType = typeof(ContentTypeWithStringArray);
            var convention = ContentTypeFieldTypeConvention.Default;
            var fieldType = convention.GetFieldType(testType.GetProperties().First(), ContentTypeNameLookUp);

            Assert.Equal(SystemFieldTypes.Array, fieldType);
        }

        [Fact]
        public void GetFieldTypeShouldReturnArrayTypeForArrayOfGenericTypesProperty()
        {
            var testType = typeof(ContentTypeWithGenericArray);
            var convention = ContentTypeFieldTypeConvention.Default;
            var fieldType = convention.GetFieldType(testType.GetProperties().First(), ContentTypeNameLookUp);

            Assert.Equal(SystemFieldTypes.Array, fieldType);
        }

        [Fact]
        public void GetFieldTypeShouldReturnArrayTypeForListOfGenericTypesProperty()
        {
            var testType = typeof(ContentTypeWithGenericList);
            var convention = ContentTypeFieldTypeConvention.Default;
            var fieldType = convention.GetFieldType(testType.GetProperties().First(), ContentTypeNameLookUp);

            Assert.Equal(SystemFieldTypes.Array, fieldType);
        }

        [Fact]
        public void GetLinkTypeShouldReturnNullForNonLinkProperty()
        {
            var testProperty = typeof(ContentTypeWithString).GetProperties().First();
            var convention = ContentTypeFieldTypeConvention.Default;
            var linkType = convention.GetLinkType(testProperty, ContentTypeNameLookUp);

            Assert.Null(linkType);
        }

        [Fact]
        public void GetLinkTypeShouldReturnAssetTypeForAssetProperty()
        {
            var testProperty = typeof(ContentTypeWithAsset).GetProperties().First();
            var convention = ContentTypeFieldTypeConvention.Default;
            var linkType = convention.GetLinkType(testProperty, ContentTypeNameLookUp);

            Assert.Equal(SystemLinkTypes.Asset,linkType);
        }

        [Fact]
        public void GetArrayTypeShouldReturnTupleOfSymbolAndEntryForArrayOfStringsProperty()
        {
            var testProperty = typeof(ContentTypeWithStringArray).GetProperties().First();
            var convention = ContentTypeFieldTypeConvention.Default;
            var (type, linkType) = convention.GetArrayType(testProperty, ContentTypeNameLookUp); 

            Assert.Equal(SystemFieldTypes.Symbol, type);
            Assert.Null(linkType);
        }

        [Fact]
        public void GetArrayTypeShouldReturnTupleOfLinkAndEntryForListOfContentTypesProperty()
        {
            var testProperty = typeof(ContentTypeWithGenericList).GetProperties().First();
            var convention = ContentTypeFieldTypeConvention.Default;
            var (type, linkType) = convention.GetArrayType(testProperty, ContentTypeNameLookUp); 

            Assert.Equal(SystemFieldTypes.Link, type);
            Assert.Equal(SystemLinkTypes.Entry,linkType);
        }

        [Fact]
        public void GetArrayTypeShouldReturnTupleOfSymbolAndNullForListOfStringsProperty()
        {
            var testProperty = typeof(ContentTypeWithStringList).GetProperties().First();
            var convention = ContentTypeFieldTypeConvention.Default;
            var (type, linkType) = convention.GetArrayType(testProperty, ContentTypeNameLookUp); 

            Assert.Equal(SystemFieldTypes.Symbol, type);
            Assert.Null(linkType);
        }
    }
}