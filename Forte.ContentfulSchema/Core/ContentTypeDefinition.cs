using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;

namespace Forte.ContentfulSchema.Core
{
    public class ContentTypeDefinition
    {
        public ContentType InferedContentType { get; }
        public EditorInterface InferedEditorInterface { get; }

        public ContentTypeDefinition(ContentType inferedContentType, EditorInterface inferedEditorInterface)
        {
            this.InferedContentType = inferedContentType;
            this.InferedEditorInterface = inferedEditorInterface;
        }

        public bool Update(ContentType contentType)
        {
            var modified = false;

            if (this.InferedContentType.SystemProperties.Id != contentType.SystemProperties.Id)
            {
                contentType.SystemProperties.Id = this.InferedContentType.SystemProperties.Id;
                modified = true;
            }

            if (this.InferedContentType.Name != contentType.Name)
            {
                contentType.Name = this.InferedContentType.Name;
                modified = true;
            }

            if (AreEmptyOrEqual(this.InferedContentType.Description, contentType.Description) == false)
            {
                contentType.Description = this.InferedContentType.Description;
                modified = true;
            }

            if (AreEmptyOrEqual(this.InferedContentType.DisplayField, contentType.DisplayField) == false)
            {
                contentType.DisplayField = this.InferedContentType.DisplayField;
                modified = true;
            }

            var matchedExistingFields = contentType.Fields.GroupJoin(
                this.InferedContentType.Fields,                             
                field => field.Id,
                field => field.Id,
                (field, fields) => new {Existing = field, Updated = fields.SingleOrDefault()});

            var matchedNewFields = this.InferedContentType.Fields.GroupJoin(
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
            var matchedControls = editorInterface.Controls.Join(this.InferedEditorInterface.Controls,
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
                existing.Disabled = updated.Disabled;
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
            if (AreEmptyOrEqual(first.Message, second.Message) == false)
                return false;
            if (first.ContentTypeIds.SequenceEqual(second.ContentTypeIds) == false)
                return false;
            
            return true;
        }

        private static bool AreEmptyOrEqual(string a, string b)
        {
            if (String.IsNullOrEmpty(a) && String.IsNullOrEmpty(b))
                return true;
            
            return a == b;
        }
    }
}
