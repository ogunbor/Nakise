using System;
using System.Collections.Generic;
using Domain.Common;

namespace Domian.Entities.ActivityForms
{
    public class FormField : AuditableEntity
    {
        public FormField()
        {
            Options = new HashSet<FieldOption>();
        }

        public Guid Id { get; set; }
        public Guid ActivityFormId { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string FormType { get; set; } // If form field is a custom or default field
        public int Index { get; set; }
        public string Placeholder { get; set; }
        public string Label { get; set; }
        public bool Required { get; set; }
        public int? RatingLevel { get; set; }
        public int? FileNumberLimit { get; set; }
        public int? SingleFileSizeLimit { get; set; }
        public string FileType { get; set; }
        public FormFieldValue FormFieldValue { get; set; }
        public ICollection<FieldOption> Options { get; set; }
    }
}
