using Domain.Common;
using System;

namespace Domain.Entities
{
    public class ProgrammeSponsor: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
