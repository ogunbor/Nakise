using Domain.Common;

namespace Domian.Entities.ActivityForms
{
    public class ActivityForm : AuditableEntity
    {
        public ActivityForm()
        {
            FormFields = new HashSet<FormField>();
        }

        public Guid Id { get; set; } 
        public Guid ActivityId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public ICollection<FormField> FormFields { get; set; }
    }
}