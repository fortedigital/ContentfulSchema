using System.Collections.Generic;
using Contentful.Core.Models;

namespace Forte.ContentfulSchema.Core
{
    public class FieldComparer : IEqualityComparer<Field>
    {
        public bool Equals(Field first, Field second)
        {
            if (first.Id != second.Id)
                return false;

            if (first.Name != second.Name)
                return false;

            if (first.Type != second.Type)
                return false;

            if (first.Disabled != second.Disabled)
                return false;

            if (first.Omitted != second.Omitted)
                return false;

            if (first.Localized != second.Localized)
                return false;

            if (first.Required != second.Required)
                return false;

            if (first.LinkType != second.LinkType)
                return false;

            if (first.Items == null && second.Items != null)
                return false;

            if (first.Items != null)
            {
                if (second.Items == null)
                    return false;

                if (first.Items.LinkType != second.Items.LinkType)
                    return false;

                if (first.Items.Type != second.Items.Type)
                    return false;

                if (first.Items.Validations == null && second.Items.Validations != null)
                    return false;

                if (first.Items.Validations != null && second.Items.Validations == null)
                    return false;

                // It's almost impossible to check if there are any differences in validators
                // It's not possible to check that through IFieldValidator interface
                if (first.Items.Validations != null || second.Items.Validations != null)
                    return false;
            }

            return true;
        }

        public int GetHashCode(Field obj)
        {
            return obj.GetHashCode();
        }
    }
}