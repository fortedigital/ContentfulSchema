using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Forte.ContentfulSchema.Core
{
    public class ContentTypeDefinition
    {
        public ContentType ContentType { get; }
        public EditorInterface EditorInterface { get; }

        public ContentTypeDefinition(ContentType contentType, EditorInterface editorInterface)
        {
            this.ContentType = contentType;
            this.EditorInterface = editorInterface;
        }

        public bool Update(ContentType contentType)
        {
            var modified = false;

            if (this.ContentType.Name != contentType.Name)
            {
                contentType.Name = this.ContentType.Name;
                modified = true;
            }

            if (this.ContentType.Description != contentType.Description)
            {
                contentType.Description = this.ContentType.Description;
                modified = true;
            }

            if (this.ContentType.DisplayField != contentType.DisplayField)
            {
                contentType.DisplayField = this.ContentType.DisplayField;
                modified = true;
            }

            var matchedExistingFields = contentType.Fields.GroupJoin(
                this.ContentType.Fields,
                field => field.Id,
                field => field.Id,
                (field, fields) => new {Existing = field, Updated = fields.SingleOrDefault()});

            var matchedNewFields = this.ContentType.Fields.GroupJoin(
                contentType.Fields,
                field => field.Id,
                field => field.Id,
                (field, fields) => new {Existing = fields.SingleOrDefault(), Updated = field});

            foreach (var match in matchedExistingFields.Union(matchedNewFields).ToList())
            {
                if (match.Updated == null)
                {
                    if (match.Existing.Disabled == false)
                    {
                        match.Existing.Disabled = true;
                        modified = true;
                    }
                }
                else if (match.Existing == null)
                {
                    contentType.Fields.Add(match.Updated);
                    modified = true;
                }
                else
                {
                    this.UpdateField(match.Existing, match.Updated, ref modified);
                }
            }
            
            return modified;
        }

        public bool Update(EditorInterface editorInterface)
        {
            //
            // Cannot add or remove controls (they always have to match content type fields) - use Join not GroupJoin
            //          
            var matchedControls = editorInterface.Controls.Join(this.EditorInterface.Controls,
                infered => infered.FieldId,
                existing => existing.FieldId,
                (i, e) => new { Existing = i, Updated = e});

            var modified = false;
            foreach (var match in matchedControls)
            {
                if (string.IsNullOrEmpty(match.Updated.WidgetId) == false && match.Existing.WidgetId != match.Updated.WidgetId)
                {
                    match.Existing.WidgetId = match.Updated.WidgetId;
                    modified = true;
                }
            }

            return modified;
        }

        private void UpdateField(Field existing, Field updated, ref bool modified)
        {
            if (existing.Name != updated.Name)
            {
                existing.Name = updated.Name;
                modified = true;
            }

            if (existing.Type != updated.Type)
            {
                existing.Type = updated.Type;
                modified = true;
            }

            if (existing.Disabled != updated.Disabled)
            {
                existing.Type = updated.Type;
                modified = true;
            }

            if (existing.Omitted != updated.Omitted)
            {
                existing.Omitted = updated.Omitted;
                modified = true;
            }

            if (existing.Localized != updated.Localized)
            {
                existing.Localized = updated.Localized;
                modified = true;
            }

            if (existing.Required != updated.Required)
            {
                existing.Required = updated.Required;
                modified = true;
            }

            if (existing.LinkType != updated.LinkType)
            {
                existing.LinkType = updated.LinkType;
                modified = true;
            }

            if (Equals(existing.Validations, updated.Validations) == false)
            {
                existing.Validations = updated.Validations;
                modified = true;
            }

            if (existing.Items == null && updated.Items != null)
            {
                existing.Items = updated.Items;
                modified = true;
            } 
            else if (existing.Items != null && updated.Items == null)
            {
                existing.Items = null;
                modified = true;
            }
            else if (existing.Items != null && updated.Items != null)
            {
                if (existing.Items.LinkType != updated.Items.LinkType)
                {
                    existing.Items.LinkType = updated.Items.LinkType;
                    modified = true;
                }

                if (existing.Items.Type != updated.Items.Type)
                {
                    existing.Items.Type = updated.Items.Type;
                    modified = true;
                }

                if (Equals(existing.Items.Validations, updated.Items.Validations) == false)
                {
                    existing.Items.Validations = updated.Items.Validations;
                    modified = true;
                }
            }
        }

        private static bool Equals(IEnumerable<IFieldValidator> first, IEnumerable<IFieldValidator> second)
        {
            if (first == null && second == null)
                return true;            
            if (first == null || second == null)
                return false;
            if (first.Count() != second.Count())
                return false;
            
            foreach (var validator in first)
            {
                var match = second.FirstOrDefault(v => v.GetType() == validator.GetType());
                if (match == null)
                    return false;

                switch (validator)
                {
                    case LinkContentTypeValidator linkTypeValidator:
                        if (Equals(linkTypeValidator, (LinkContentTypeValidator) match) == false) 
                            return false;
                        break;
                    
                    default:
                        return false;
                }
            }

            return true;
        }

        private static bool Equals(LinkContentTypeValidator first, LinkContentTypeValidator second)
        {
            if (first.Message != second.Message)
                return false;
            if (first.ContentTypeIds.SequenceEqual(second.ContentTypeIds) == false)
                return false;
            
            return true;
        }
    }
}
