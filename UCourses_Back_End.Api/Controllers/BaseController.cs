using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCourses_Back_End.Core.Interfaces.IRepositories;
using UCourses_Back_End.Core.Interfaces.IServices;

namespace UCourses_Back_End.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IUnitOfWork work;
        protected readonly IAuditLogService? auditLog;

        public BaseController(IUnitOfWork work, IAuditLogService? auditLog = null)
        {
            this.work = work;
            this.auditLog = auditLog;
        }

        protected string? GetCurrentUserId()
        {
            return User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
