using System.Reflection;

namespace Forte.ContentfulSchema.Discovery
{
    internal interface IFieldControlConvention
    {
        string GetWidgetId(PropertyInfo property);
    }
}