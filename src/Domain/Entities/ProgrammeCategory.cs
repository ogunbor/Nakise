using Domain.Common;
using System;

namespace Domain.Entities
{
    public class ProgrammeCategory: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
