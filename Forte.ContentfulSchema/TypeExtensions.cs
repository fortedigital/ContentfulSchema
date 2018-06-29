using System;
using System.Collections.Generic;

namespace Forte.ContentfulSchema
{
    internal static class TypeExtensions
    {
        public static bool IsGenericType(this Type type, Type genericTypeDefinition)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;
        }

        public static bool IsEnumerable(this Type type)
        {
            return type.IsGenericType &&
                   typeof(IEnumerable<>).MakeGenericType(type.GetGenericArguments()[0])
                       .IsAssignableFrom(type);
        }
        
        public static bool IsEnumerableOf<T>(this Type type)
        {
            if (type.IsEnumerable() == false)
                return false;
            
            var elementType = type.GetGenericArguments()[0];
            return type.IsGenericType && 
                   typeof(IEnumerable<>).MakeGenericType(elementType).IsAssignableFrom(type) &&
                   typeof(T).IsAssignableFrom(elementType);
        }

        public static Type GetEnumerableElementType(this Type type)
        {
            if (type.IsEnumerable() == false)
                throw new InvalidOperationException();
            return type.GetGenericArguments()[0];
        }
    }
}