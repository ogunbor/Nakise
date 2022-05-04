using System;

namespace Domian.Entities.ActivityForms
{
    public class FieldOption
    {
        public Guid Id { get; set; }
        public Guid FormFieldId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public FormField FormField { get; set; }
    }
}