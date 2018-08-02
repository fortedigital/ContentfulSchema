using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Tests;

namespace Forte.ContentfulSchema.Tests.Conventions{
    public class PropertyInfoAndNamePair
    {
        public PropertyInfo TestPropertyInfo { get; set; }
        public string TypeName { get; set; }
    }

    public static class TestDataForFieldTypeRecognition
    {
        public static IEnumerable<object[]> PropertyInfoAndNamePairs => new[]
        {
            new[] {
                new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithBool).GetProperties().First(),
                        TypeName = SystemFieldTypes.Boolean }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithFloat).GetProperties().First(),
                        TypeName = SystemFieldTypes.Number }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithDouble).GetProperties().First(),
                        TypeName = SystemFieldTypes.Number }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithInteger).GetProperties().First(),
                        TypeName = SystemFieldTypes.Integer }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithLong).GetProperties().First(),
                        TypeName = SystemFieldTypes.Integer }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithString).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithDecimal).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithChar).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithByte).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithSByte).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithUint).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithULong).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithUShort).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ChildOfEmptyContentType).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(ContentTypeWithInheritedProperty).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(GrandChildOfEmptyContentType).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                new[] {
                    new PropertyInfoAndNamePair { 
                        TestPropertyInfo = typeof(CustomizedContentType).GetProperties().First(),
                        TypeName = SystemFieldTypes.Symbol }},
                };
    }
}