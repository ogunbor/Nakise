using Domain.Common;

namespace Shared.DataTransferObjects
{
    public class OrganizationResponse : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OrganizationRef { get; set; }
        public bool IsActive { get; set; }
    }
}