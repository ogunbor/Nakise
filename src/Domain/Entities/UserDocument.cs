using Domain.Common;

namespace Domain.Entities
{
    public class UserDocument : AuditableEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentName { get; set; }
        public User User { get; set; }
    }
}
