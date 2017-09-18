using Contentful.Core.Models;
using Forte.ContentfulSchema.Core;
using System.Collections.Generic;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Xunit;

namespace Forte.ContentfulSchema.Tests
{
    public class FieldComparerTests
    {
        private readonly FieldComparer _fieldComparer;

        public FieldComparerTests()
        {
            _fieldComparer = new FieldComparer();
        }

        [Theory]
        [MemberData(nameof(GetDifferentObjects))]
        public void ShouldReturnFalseForDifferentObjects((Field first, Field second) pair)
        {
            Assert.False(_fieldComparer.Equals(pair.first, pair.second),
                $"Comparing objects: {pair.PrettyPrint()}");
        }

        [Theory]
        [MemberData(nameof(GetSameObjects))]
        public void ShouldReturnTrueForSameObjects((Field first, Field second) pair)
        {
            Assert.True(_fieldComparer.Equals(pair.first, pair.second),
                $"Comparing objects: {pair.PrettyPrint()}");
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> GetDifferentObjects => new[]
        {
            new object[] {(first: new Field {Id = "1"}, second: new Field {Id = "2"})},
            new object[] {(first: new Field {Name = "First"}, second: new Field {Name = "Second"})},
            new object[] {(first: new Field {Disabled = true}, second: new Field {Disabled = false})},
            new object[] {(first: new Field {LinkType = "Link One"}, second: new Field {LinkType = "Link Two"})},
            new object[] {(first: new Field {Localized = true}, second: new Field {Localized = false})},
            new object[] {(first: new Field {Omitted = true}, second: new Field {Omitted = false})},
            new object[] {(first: new Field {Required = true}, second: new Field {Required = false})},
            new object[] {(first: new Field {Type = "Type One"}, second: new Field {Type = "Type Two"})},
            new object[]
            {
                (
                first: new Field {Items = new Schema { LinkType = "Schema Link One"}},
                second: new Field {Items = new Schema { LinkType = "Schema Link Two"}}
                )
            },
            new object[]
            {
                (
                first: new Field {Items = new Schema { Type = "Schema Type One"}},
                second: new Field {Items = new Schema { Type = "Schema Type Two"}}
                )
            },
            new object[]
            {
                (
                first: new Field {Items = new Schema { Validations = new List<IFieldValidator> {new UniqueValidator() }}},
                second: new Field {Items = new Schema { Validations = new List<IFieldValidator> { new UniqueValidator() }}}
                )
            },
            new object[]
            {
                (
                first: new Field {Items = new Schema { Validations = new List<IFieldValidator> {new RangeValidator(5, 10)} }},
                second: new Field {Items = new Schema { Validations = new List<IFieldValidator> { new UniqueValidator() }}}
                )
            },
            new object[]
            {
                (
                first: new Field {Items = new Schema { Validations = new List<IFieldValidator> {new RangeValidator(5, 10)} }},
                second: new Field {Items = new Schema { Validations = new List<IFieldValidator> {}}}
                )
            },
        };

        public static IEnumerable<object[]> GetSameObjects => new[]
        {
            new object[] {(first: new Field {Id = "1"}, second: new Field {Id = "1"})},
            new object[] {(first: new Field {Name = "Name"}, second: new Field {Name = "Name"})},
            new object[] {(first: new Field {Disabled = true}, second: new Field {Disabled = true})},
            new object[] {(first: new Field {LinkType = "LinkType"}, second: new Field {LinkType = "LinkType"})},
            new object[] {(first: new Field {Localized = true}, second: new Field {Localized = true})},
            new object[] {(first: new Field {Omitted = true}, second: new Field {Omitted = true})},
            new object[] {(first: new Field {Required = true}, second: new Field {Required = true})},
            new object[] {(first: new Field {Type = "FieldType"}, second: new Field {Type = "FieldType"})},
            new object[]
            {
                (
                first: new Field {Items = new Schema { LinkType = "SchemaLink"}},
                second: new Field {Items = new Schema { LinkType = "SchemaLink"}}
                )
            },
            new object[]
            {
                (
                first: new Field {Items = new Schema { Type = "SchemaType"}},
                second: new Field {Items = new Schema { Type = "SchemaType"}}
                )
            },
            new object[]
            {
                (
                first: new Field {Items = new Schema { Validations = null }},
                second: new Field {Items = new Schema { Validations = null }}
                )
            },
        };
    }

    internal static class FieldTupleExtensions
    {
        internal static string PrettyPrint(this (Field first, Field second) pair)
        {
            return JsonConvert.SerializeObject(pair, Formatting.Indented);
        }
    }
}