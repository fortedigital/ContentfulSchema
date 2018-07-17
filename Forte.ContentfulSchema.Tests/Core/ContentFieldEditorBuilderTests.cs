//using Contentful.Core.Models;
//using Contentful.Core.Models.Management;
//using Forte.ContentfulSchema.Core;
//using Moq;
//using System.Reflection;
//using Xunit;

//namespace Forte.ContentfulSchema.Tests.Core
//{
//    public class ContentFieldEditorBuilderTests
//    {
//        [Fact]
//        public void ShouldSetWidgetIdForTheFieldIfProviderContainsSuchRule()
//        {
//            ContentFieldEditorBuilder builder = CreateBuilder(SystemWidgetIds.Checkbox);
//            var result = builder.GetEditorControl(Mock.Of<PropertyInfo>(), new Field { Id = "1" });

//            Assert.Equal("1", result.FieldId);
//            Assert.Equal(SystemWidgetIds.Checkbox, result.WidgetId);
//        }

//        [Fact]
//        public void ShouldNotSetDefaultWidgetIdIfProviderDoesNotContainValidRule()
//        {
//            ContentFieldEditorBuilder builder = CreateBuilder(null);
//            var result = builder.GetEditorControl(Mock.Of<PropertyInfo>(), new Field { Id = "1" });

//            Assert.Equal("1", result.FieldId);
//            Assert.Null(result.WidgetId);
//        }

//        private static ContentFieldEditorBuilder CreateBuilder(string providerResponse)
//        {
//            var providerMock = new Mock<IContentEditorControlProvider>();
//            providerMock.Setup(m => m.GetWidgetIdForField(It.IsAny<PropertyInfo>(), It.IsAny<Field>()))
//                        .Returns(providerResponse);

//            var builder = new ContentFieldEditorBuilder(providerMock.Object);
//            return builder;
//        }
//    }
//}
