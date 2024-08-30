using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERezervacijeAPI.Services
{
    public class ManagerAttribute : TypeFilterAttribute
    {
        public ManagerAttribute() : base(typeof(ManagerActionFilter))
        {
        }
    }
    public class ManagerActionFilter : IAsyncActionFilter
    {
        private readonly MyAuthService auth;
        public ManagerActionFilter(MyAuthService _auth)
        {
            auth = _auth;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.JePrijavljen|| !prijava.IsManager)
            {
                throw new UnauthorizedAccessException();
            }
            await next();
        }
    }
}
