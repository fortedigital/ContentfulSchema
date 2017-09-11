using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class ContentTypeComparer : IEqualityComparer<ContentType>
    {
        private readonly IEqualityComparer<Field> _fieldComparer;

        public ContentTypeComparer(IEqualityComparer<Field> fieldComparer)
        {
            _fieldComparer = fieldComparer;
        }
        
        public bool Equals(ContentType first, ContentType second)
        {
            if (first.SystemProperties.Id != second.SystemProperties.Id)
                return false;

            if (first.Name != second.Name)
                return false;

            if (first.DisplayField != second.DisplayField)
                return false;

            if (first.Description != second.Description)
                return false;

            if (first.Fields.Count != second.Fields.Count)
                return false;

            var matchedFields = first.Fields
                .GroupJoin(second.Fields, ctf => ctf.Id, ictf => ictf.Id,
                    (cf, icf) => new {Field = cf, InferedField = icf.SingleOrDefault()});

            foreach (var fieldMatch in matchedFields)
            {
                // Field was deleted
                if (fieldMatch.InferedField == null)
                    return false;

                if (_fieldComparer.Equals(fieldMatch.Field, fieldMatch.InferedField) == false)
                    return false;
            }

            return true;
        }

        public int GetHashCode(ContentType obj)
        {
            return obj.GetHashCode();
        }
    }
}