//using Contentful.Core.Models;
//using Forte.ContentfulSchema.Core;
//using System.Collections.Generic;
//using Contentful.Core.Models.Management;
//using Newtonsoft.Json;
//using Xunit;
//using Xunit.Abstractions;

//namespace Forte.ContentfulSchema.Tests
//{
//    public class FieldComparerTests
//    {
//        private readonly FieldComparer _fieldComparer;

//        public FieldComparerTests()
//        {
//            _fieldComparer = new FieldComparer();
//        }

//        [Theory]
//        [MemberData(nameof(GetDifferentObjects))]
//        public void ShouldReturnFalseForDifferentObjects(FieldPair pair)
//        {
//            Assert.False(_fieldComparer.Equals(pair.First, pair.Second),
//                $"Comparing objects: {pair.PrettyPrint()}");
//        }

//        [Theory]
//        [MemberData(nameof(GetSameObjects))]
//        public void ShouldReturnTrueForSameObjects(FieldPair pair)
//        {
//            Assert.True(_fieldComparer.Equals(pair.First, pair.Second),
//                $"Comparing objects: {pair.PrettyPrint()}");
//        }

//        // ReSharper disable once MemberCanBePrivate.Global
//        public static IEnumerable<object[]> GetDifferentObjects()
//        {
//            int testDataId = 0;

//            int getDataId()
//            {
//                return testDataId++;
//            }

//            yield return new object[]
//            {
//                new FieldPair(getDataId(), "Compare Id property")
//                {
//                    First = new Field {Id = "1"},
//                    Second = new Field {Id = "2"},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId(), "Compare name property")
//                {
//                    First = new Field {Name = "First"},
//                    Second = new Field {Name = "Second"},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId(), "Compare disabled property")
//                {
//                    First = new Field {Disabled = true},
//                    Second = new Field {Disabled = false},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {LinkType = "Link One"},
//                    Second = new Field {LinkType = "Link Two"},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Localized = true},
//                    Second = new Field {Localized = false},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Omitted = true},
//                    Second = new Field {Omitted = false},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Required = true},
//                    Second = new Field {Required = false},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Type = "Type One"},
//                    Second = new Field {Type = "Type Two"},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Items = new Schema {LinkType = "Schema Link One"}},
//                    Second = new Field {Items = new Schema {LinkType = "Schema Link Two"}},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Items = new Schema {Type = "Schema Type One"}},
//                    Second = new Field {Items = new Schema {Type = "Schema Type Two"}},
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field
//                    {
//                        Items = new Schema {Validations = new List<IFieldValidator> {new UniqueValidator()}}
//                    },
//                    Second = new Field
//                    {
//                        Items = new Schema {Validations = new List<IFieldValidator> {new UniqueValidator()}}
//                    },
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field
//                    {
//                        Items = new Schema {Validations = new List<IFieldValidator> {new RangeValidator(5, 10)}}
//                    },
//                    Second = new Field
//                    {
//                        Items = new Schema {Validations = new List<IFieldValidator> {new UniqueValidator()}}
//                    },
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field
//                    {
//                        Items = new Schema {Validations = new List<IFieldValidator> {new RangeValidator(5, 10)}}
//                    },
//                    Second = new Field
//                    {
//                        Items = new Schema {Validations = new List<IFieldValidator> { }}
//                    },
//                }
//            };
//        }

//        public static IEnumerable<object[]> GetSameObjects()
//        {
//            int testDataId = 0;

//            int getDataId()
//            {
//                return testDataId++;
//            }

//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Id = "1"},
//                    Second = new Field {Id = "1"}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Name = "Name"},
//                    Second = new Field {Name = "Name"}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Disabled = true},
//                    Second = new Field {Disabled = true}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {LinkType = "LinkType"},
//                    Second = new Field {LinkType = "LinkType"}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Localized = true},
//                    Second = new Field {Localized = true}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Omitted = true},
//                    Second = new Field {Omitted = true}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Required = true},
//                    Second = new Field {Required = true}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Type = "FieldType"},
//                    Second = new Field {Type = "FieldType"}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Items = new Schema {LinkType = "SchemaLink"}},
//                    Second = new Field {Items = new Schema {LinkType = "SchemaLink"}}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Items = new Schema {Type = "SchemaType"}},
//                    Second = new Field {Items = new Schema {Type = "SchemaType"}}
//                }
//            };
//            yield return new object[]
//            {
//                new FieldPair(getDataId())
//                {
//                    First = new Field {Items = new Schema {Validations = null}},
//                    Second = new Field {Items = new Schema {Validations = null}}
//                }
//            };
//        }
//    }

//    public class FieldPair : IXunitSerializable
//    {
//        public Field First { get; set; }

//        public Field Second { get; set; }

//        private int TestDataId { get; set; }

//        private string description;

//        public FieldPair()
//        {
//        }

//        public FieldPair(int testDataId, string description = null)
//        {
//            TestDataId = testDataId;
//            this.description = description;
//        }

//        public void Deserialize(IXunitSerializationInfo info)
//        {
//            var jsonFirst = info.GetValue<string>("first");
//            First = JsonConvert.DeserializeObject<Field>(jsonFirst);

//            var jsonSecond = info.GetValue<string>("second");
//            Second = JsonConvert.DeserializeObject<Field>(jsonSecond);


//            TestDataId = info.GetValue<int>("TestDataId");
//            description = info.GetValue<string>(nameof(description));
//        }

//        public void Serialize(IXunitSerializationInfo info)
//        {
//            var jsonFirst = JsonConvert.SerializeObject(First);
//            info.AddValue("first", jsonFirst, typeof(string));

//            var jsonSecond = JsonConvert.SerializeObject(Second);
//            info.AddValue("second", jsonSecond, typeof(string));

//            info.AddValue("TestDataId", TestDataId, typeof(int));

//            info.AddValue(nameof(description), description, typeof(string));
//        }

//        public string PrettyPrint()
//        {
//            return JsonConvert.SerializeObject((First: First, Second: Second), Formatting.Indented);
//        }

//        public override string ToString()
//        {
//            return $"Id: {TestDataId}. {description}";
//        }
//    }
//}