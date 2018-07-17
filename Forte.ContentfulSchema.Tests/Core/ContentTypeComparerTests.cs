//using Contentful.Core.Models;
//using Forte.ContentfulSchema.Core;
//using Moq;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Reflection;
//using Xunit;
//using Xunit.Abstractions;

//namespace Forte.ContentfulSchema.Tests
//{
//    public class ContentTypeComparerTests
//    {
//        private readonly ContentTypeComparer _comparer;

//        public ContentTypeComparerTests()
//        {
//            _comparer = new ContentTypeComparer(new FieldComparer());
//        }

//        [Theory]
//        [MemberData(nameof(DifferentContentTypes))]
//        public void ShouldReturnFalseWhenObjetsPropertiesAreDifferent(ContentPair pair)
//        {
//            Assert.False(_comparer.Equals(pair.First, pair.Second),
//                $"Comparing objects: {pair.PrettyPrint()}");
//        }

//        [Theory]
//        [MemberData(nameof(SameContentTypes))]
//        public void ShouldReturnTrueWhenObjectsPropertiesAreEqual(ContentPair pair)
//        {
//            Assert.True(_comparer.Equals(pair.First, pair.Second),
//                $"Comparing objects: {pair.PrettyPrint()}");
//        }

//        [Theory]
//        [InlineData(false)]
//        [InlineData(true)]
//        public void ShouldReturnValueDependingFromTheFieldComparerResult(bool areEqual)
//        {
//            var fieldComparer = new Mock<IEqualityComparer<Field>>();
//            fieldComparer.Setup(m => m.Equals(It.IsAny<Field>(), It.IsAny<Field>()))
//                .Returns(areEqual);

//            var customComparer = new ContentTypeComparer(fieldComparer.Object);

//            var firstContentType = ContentTypeBuilder.New.WithFields(new Field {Id = "1"}).Build();
//            var secondContentType = ContentTypeBuilder.New.WithFields(new Field {Id = "1"}).Build();

//            var result = customComparer.Equals(firstContentType, secondContentType);

//            Assert.Equal(areEqual, result);
//        }

//        public static IEnumerable<object[]> DifferentContentTypes
//        {
//            get
//            {
//                return new[]
//                {
//                    new object[]
//                    {
//                        new ContentPair
//                        {
//                            First = ContentTypeBuilder.New.WithId("123").Build(),
//                            Second = ContentTypeBuilder.New.WithId("321").Build()
//                        }
//                    },
//                    new object[]
//                    {
//                        new ContentPair
//                        {
//                            First = ContentTypeBuilder.New.WithDescription("First description").Build(),
//                            Second = ContentTypeBuilder.New.WithDescription("Second description").Build()
//                        }
//                    },
//                    new object[]
//                    {
//                        new ContentPair
//                        {
//                            First = ContentTypeBuilder.New.WithDisplayField("First display").Build(),
//                            Second = ContentTypeBuilder.New.WithDisplayField("Second display").Build()
//                        }
//                    },
//                    new object[]
//                    {
//                        new ContentPair
//                        {
//                            First = ContentTypeBuilder.New.WithName("First name").Build(),
//                            Second = ContentTypeBuilder.New.WithName("Second name").Build()
//                        }
//                    },
//                };
//            }
//        }

//        public static IEnumerable<object[]> SameContentTypes => new[]
//        {
//            new object[]
//            {
//                new ContentPair
//                {
//                    First = ContentTypeBuilder.New.WithId("123").Build(),
//                    Second = ContentTypeBuilder.New.WithId("123").Build()
//                }
//            },
//            new object[]
//            {
//                new ContentPair
//                {
//                    First = ContentTypeBuilder.New.WithDescription("Description").Build(),
//                    Second = ContentTypeBuilder.New.WithDescription("Description").Build()
//                }
//            },
//            new object[]
//            {
//                new ContentPair
//                {
//                    First = ContentTypeBuilder.New.WithDisplayField("Display").Build(),
//                    Second = ContentTypeBuilder.New.WithDisplayField("Display").Build()
//                }
//            },
//            new object[]
//            {
//                new ContentPair
//                {
//                    First = ContentTypeBuilder.New.WithName("Name").Build(),
//                    Second = ContentTypeBuilder.New.WithName("Name").Build()
//                }
//            },
//        };
//    }

//    public class ContentPair : IXunitSerializable
//    {
//        public ContentType First { get; set; }

//        public ContentType Second { get; set; }

//        public string PrettyPrint()
//        {
//            return JsonConvert.SerializeObject((First: First, Second: Second), Formatting.Indented);
//        }

//        public void Deserialize(IXunitSerializationInfo info)
//        {
//            var jsonFirst = info.GetValue<string>("first");
//            First = JsonConvert.DeserializeObject<ContentType>(jsonFirst);

//            var jsonSecond = info.GetValue<string>("second");
//            Second = JsonConvert.DeserializeObject<ContentType>(jsonSecond);
//        }

//        public void Serialize(IXunitSerializationInfo info)
//        {
//            var jsonFirst = JsonConvert.SerializeObject(First);
//            info.AddValue("first", jsonFirst, typeof(string));

//            var jsonSecond = JsonConvert.SerializeObject(Second);
//            info.AddValue("second", jsonSecond, typeof(string));
//        }
//    }

//    internal class ContentTypeBuilder
//    {
//        private readonly ContentType _currentBuild;

//        private ContentTypeBuilder()
//        {
//            _currentBuild = new ContentType
//            {
//                SystemProperties = new SystemProperties(),
//                Fields = new List<Field>(),
//                Description = string.Empty,
//                DisplayField = string.Empty,
//                Name = string.Empty
//            };
//        }

//        public static ContentTypeBuilder New => new ContentTypeBuilder();

//        public ContentTypeBuilder WithId(string id)
//        {
//            _currentBuild.SystemProperties.Id = id;
//            return this;
//        }

//        public ContentTypeBuilder WithDescription(string description)
//        {
//            _currentBuild.Description = description;
//            return this;
//        }

//        public ContentTypeBuilder WithDisplayField(string displayField)
//        {
//            _currentBuild.DisplayField = displayField;
//            return this;
//        }

//        public ContentTypeBuilder WithFields(params Field[] fields)
//        {
//            _currentBuild.Fields.AddRange(fields);
//            return this;
//        }

//        public ContentTypeBuilder WithName(string name)
//        {
//            _currentBuild.Name = name;
//            return this;
//        }

//        public ContentType Build()
//        {
//            var contentType = new ContentType
//            {
//                SystemProperties = new SystemProperties {Id = _currentBuild.SystemProperties.Id},
//                Fields = new List<Field>(_currentBuild.Fields),
//                Description = string.Copy(_currentBuild.Description),
//                DisplayField = string.Copy(_currentBuild.DisplayField),
//                Name = string.Copy(_currentBuild.Name)
//            };

//            return contentType;
//        }
//    }
//}