using System;

namespace Domian.Entities.ActivityForms
{
    public class FormFieldValue
    {
        public Guid Id { get; set; }
        public Guid FormId { get; set; }
        public Guid FormFieldId { get; set; }
        public string Value { get; set; }
        public FormField FormField { get; set; }
        public Guid ApplicantDetailId { get; set; }
    }
}