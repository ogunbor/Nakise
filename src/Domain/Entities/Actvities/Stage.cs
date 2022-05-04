using Domain.Common;
using System;

namespace Domain.Entities.Activities
{
    public class Stage : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid CallForApplicationId { get; set; }
        public string? Name { get; set; }
        public int Index { get; set; }
    }
}
