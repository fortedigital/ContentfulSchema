using System;
using System.Linq;
using System.Reflection;
using Contentful.Core.Models.Management;
using Forte.ContentfulSchema.ContentTypes;
using Forte.ContentfulSchema.Conventions;
using Forte.ContentfulSchema.Discovery;
using Moq;
using Xunit;

namespace Forte.ContentfulSchema.Tests.Conventions
{
    public class DefaultFieldControlConventionTests
    {
        private readonly IFieldControlConvention _defaultConvention = DefaultFieldControlConvention.Default;

        [Fact]
        public void ShouldReturnSlugEditorWidgetIdForSlugNamedProperty()
        {
            var propInfo = GetPropertyInfoOfFirstProperty<ClassWithSlugProperty>();
            var widgetId = _defaultConvention.GetWidgetId(propInfo);

            Assert.Equal(SystemWidgetIds.SlugEditor,widgetId);
        }

        [Fact]
        public void ShouldReturnMultipleLineEditorWidgetIdForPropOfILongStringType()
        {
            var propInfo = GetPropertyInfoOfFirstProperty<ClassWithILongProperty>();
            var widgetId = _defaultConvention.GetWidgetId(propInfo);

            Assert.Equal(SystemWidgetIds.MultipleLine,widgetId);
        }

        [Fact]
        public void ShouldReturnMarkdownWidgetIdForPropOfIMarkdownStringType()
        {
            var propInfo = GetPropertyInfoOfFirstProperty<ClassWithIMarkdownString>();
            var widgetId = _defaultConvention.GetWidgetId(propInfo);

            Assert.Equal(SystemWidgetIds.Markdown,widgetId);
        }

        [Fact]
        public void ShouldUseCustomizedDefaultConventions()
        {
            var conventions = new (Func<PropertyInfo, bool> Predicate, string Widget)[]
            {
                (p => p.Name.Equals("CustomProp"), SystemWidgetIds.Checkbox)
            };
            var convention = new DefaultFieldControlConvention(conventions);
            var propInfo = GetPropertyInfoOfFirstProperty<ClassForCustomConvention>();
            var widgetId = convention.GetWidgetId(propInfo);

            Assert.Equal(SystemWidgetIds.Checkbox, widgetId);
        }

        private static PropertyInfo GetPropertyInfoOfFirstProperty<T>()
        {
            return typeof(T).GetProperties().First();
        }

        private class ClassWithSlugProperty
        {
            public string Slug { get; set; }
        }

        private class ClassWithILongProperty
        {
            public ILongString LongText { get; set; }
        }

        private class ClassWithIMarkdownString
        {
            public IMarkdownString MarkDownString{ get; set; }
        }

        private class ClassForCustomConvention
        {
            public string CustomProp { get; set; }
        }
    }
}