using System.Reflection;

namespace Forte.ContentfulSchema.Discovery
{
    public interface IFieldControlConvention
    {
        string GetWidgetId(PropertyInfo property);
    }
}