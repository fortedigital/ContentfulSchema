using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.Core;
using Newtonsoft.Json;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Core
{
    public class ContentTypeDefinitionTests
    {
        [Theory]
        [MemberData(nameof(ContentTypeModificators))]
        public void ShouldReturnTrueWhenUpdatingWithDifferentProperties(Action<ContentType> setDifferentProperty)
        {
            var originalContentType = CreateDefaultContentType();
            var contentTypeClone = CloneContentType(originalContentType);
            var definition = new ContentTypeDefinition(contentTypeClone, null);
            
            setDifferentProperty(originalContentType);
            Assert.True(definition.Update(originalContentType));
        }

        [Fact]
        public void ShouldReturnFalseWhenUpdatingWithTheSameProperties()
        {
            var contentType = CreateDefaultContentType();
            var definition = new ContentTypeDefinition(contentType, null);
            var sameContentType = CreateDefaultContentType();
            Assert.False(definition.Update(sameContentType));
        }

        [Theory]
        [MemberData(nameof(FieldsModificators))]
        public void ShouldReturnTrueForUpdatedFields(Action<List<Field>> modifyFields)
        {
            var originalContentType = CreateDefaultContentType();
            var contentTypeClone = CloneContentType(originalContentType);
            var definition = new ContentTypeDefinition(contentTypeClone, null);
            modifyFields(originalContentType.Fields);

            Assert.True(definition.Update(originalContentType));
        }

        [Fact]
        public void ShouldReturnTrueWhenUpdatingEditorInterface()
        {
            var baseEditorInterface = new EditorInterface{ Controls = new List<EditorInterfaceControl>() };
            var newEditorInterface = CreateDefaultNewEditorInterface();
            var definition = new ContentTypeDefinition(null, newEditorInterface);

            baseEditorInterface.Controls.AddRange(new List<EditorInterfaceControl>{ new EditorInterfaceControl { FieldId = "EditorField1", WidgetId = "OldWidgetId1" }, new EditorInterfaceControl { FieldId = "EditorField2", WidgetId = "OldWidgetId2" } });
            
            Assert.True(definition.Update(baseEditorInterface));
            for (var i = 0; i < baseEditorInterface.Controls.Count; i++)
            {
                Assert.Equal(definition.InferredEditorInterface.Controls[i].FieldId,
                    baseEditorInterface.Controls[i].FieldId);
                Assert.Equal(definition.InferredEditorInterface.Controls[i].WidgetId,
                    baseEditorInterface.Controls[i].WidgetId);
            }
        }

        [Theory]
        [MemberData(nameof(NotUpdatableEditorControls))]
        public void ShouldReturnFalseWhenUpdatingSameOrIncorrectEditorInterfaces(List<EditorInterfaceControl> controls)
        {
            var baseEditorInterface = new EditorInterface();
            var newEditorInterface = CreateDefaultNewEditorInterface();
            var definition = new ContentTypeDefinition(null, newEditorInterface);

            baseEditorInterface.Controls = controls;

            Assert.False(definition.Update(baseEditorInterface));
        }

        public static IEnumerable<object[]> NotUpdatableEditorControls => new[]
        {
            new[]
            {
                new List<EditorInterfaceControl>
                {
                    new EditorInterfaceControl {FieldId = "EditorField1", WidgetId = "NewWidgetId1"},
                    new EditorInterfaceControl {FieldId = "EditorField2", WidgetId = "NewWidgetId2"}
                }
            },
            new[]
            {
                new List<EditorInterfaceControl>
                {
                    new EditorInterfaceControl()
                }
            },
            new[]
            {
                new List<EditorInterfaceControl>
                {
                    new EditorInterfaceControl{FieldId = "DifferentField", WidgetId = "DifferentWidgetId"}
                }
            }, 
            new[]
            {
                new List<EditorInterfaceControl>
                {
                    new EditorInterfaceControl{FieldId = "EditorField1", WidgetId = "NewWidgetId1"}
                }
            }, 
        };

        public static IEnumerable<object[]> ContentTypeModificators => new[]
        {
            new Action<ContentType>[] {ct => ct.SystemProperties.Id = "321"},
            new Action<ContentType>[] {ct => ct.Name = "NewName"},
            new Action<ContentType>[] {ct => ct.Description = "NewDescription"},
            new Action<ContentType>[] {ct => ct.DisplayField = "NewDisplayField"}
        };

        public static IEnumerable<object[]> FieldsModificators => new[]
        {
           new Action<List<Field>>[] { f =>
                {
                    f.Clear();
                    f.Add(new Field {Id = "NewField1"});
                }
            },
            new Action<List<Field>>[] { f => f[0].Name = "ChangedFieldName" },
            new Action<List<Field>>[] { f => f[0].Type = "Text" },
            new Action<List<Field>>[] { f => f[0].Omitted = true },
            new Action<List<Field>>[] { f => f[0].Localized = true },
            new Action<List<Field>>[] { f => f[0].Required = true },
            new Action<List<Field>>[] { f => f[0].LinkType = "Entry" },
            new Action<List<Field>>[] { f => f[0].Disabled = true },
            new Action<List<Field>>[] { f => f[0].Validations = new List<IFieldValidator>(){new UniqueValidator()} },
            new Action<List<Field>>[] { f => f[0].Items = new Schema(){LinkType = "TestLinkType"}},
            new Action<List<Field>>[] { f => f[0].Items = new Schema(){Type = "TestType"}},
            new Action<List<Field>>[] { f => f[0].Items = new Schema(){Validations = new List<IFieldValidator>(){new UniqueValidator()}}},
            new Action<List<Field>>[] { f => f.Clear() },
            new Action<List<Field>>[] { f => f.AddRange(new List<Field>{ new Field {Id = "NewField1"}, new Field {Id = "NewField2"} }) },
            new Action<List<Field>>[] { f => f.Reverse() },
        };

        private static ContentType CreateDefaultContentType()
        {
            return new ContentType
            {    
                SystemProperties = new SystemProperties{Id = "123"},
                Description = "Description",
                DisplayField = "DisplayField",
                Name = "Name",
                Fields = new List<Field>{ new Field{ Id = "field1" }, new Field { Id = "field2" } }
            };
        }

        private static ContentType CloneContentType(ContentType ct)
        {
            return JsonConvert.DeserializeObject<ContentType>(JsonConvert.SerializeObject(ct));
        }

        private static EditorInterface CreateDefaultNewEditorInterface()
        {
            var editor = new EditorInterface{Controls = new List<EditorInterfaceControl>()};
            editor.Controls.AddRange(new List<EditorInterfaceControl>{ new EditorInterfaceControl { FieldId = "EditorField1", WidgetId = "NewWidgetId1" }, new EditorInterfaceControl { FieldId = "EditorField2", WidgetId = "NewWidgetId2" } });
            
            return editor;
        }
    }
}
