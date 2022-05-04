using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Organization : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OrganizationRef { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
