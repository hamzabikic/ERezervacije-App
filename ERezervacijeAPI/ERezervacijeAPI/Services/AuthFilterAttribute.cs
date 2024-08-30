using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERezervacijeAPI.Services
{
    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute():base(typeof(AuthActionFilter))
        {

        }
    }
    public class AuthActionFilter : IAsyncActionFilter
    {
        private readonly MyAuthService auth;
        public  AuthActionFilter (MyAuthService _auth)
        {
            auth = _auth;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var prijava = await auth.isLogiran();
            if (!prijava.JePrijavljen) throw new UnauthorizedAccessException();
            await next();
        }
    }
}
