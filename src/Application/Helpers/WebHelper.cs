using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public interface IWebHelper
    {
        UserHelperDTO User();
    } 

    public class WebHelper : IWebHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext HttpContext
        {
            get { return _httpContextAccessor.HttpContext; }
        }        

        private static UserHelperDTO UserHelper
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == ClaimTypeHelper.UserId).FirstOrDefault()?.Value ?? "";
                Guid.TryParse(userId, out Guid id);
                var organizationId = _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.OrganizationId).FirstOrDefault()?.Value ?? Guid.Empty.ToString();
                var fullName = _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.FullName).FirstOrDefault()?.Value ?? "";
                var email = _httpContextAccessor?.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypeHelper.Email).FirstOrDefault()?.Value ?? "";
                
                var result = new UserHelperDTO
                {
                    OrganizationId = Guid.Parse(organizationId),
                    UserId = id,
                    FullName = fullName,
                    Email = email
                };
                return result;
            }
        }

        public UserHelperDTO User()
        {
            return UserHelper;
        }
    }

    public class UserHelperDTO
    {
        public Guid OrganizationId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
