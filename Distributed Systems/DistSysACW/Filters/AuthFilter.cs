using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistSysACW.Filters
{
    public class AuthFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                AuthorizeAttribute authAttribute = (AuthorizeAttribute)context.ActionDescriptor.EndpointMetadata.Where(e => e.GetType() == typeof(AuthorizeAttribute)).FirstOrDefault();
                if (authAttribute != null)
                {
                    string[] roles = authAttribute.Roles.Split(',');
                    string role;

                    if (roles.Length == 1)
                    {
                        if (roles.Contains("Admin"))
                        {
                            role = roles[roles.Length - 1];
                            if (context.HttpContext.User.IsInRole(role))
                            {
                                return;
                            }
                            else
                            {
                                throw new UnauthorizedAccessException();
                            }
                        }
                    }
                    foreach (string Role in roles)
                    {
                        string CurrentRole = Role;
                        if (context.HttpContext.User.IsInRole(CurrentRole)) 
                        {
                            return;  
                        }
                    }
                    throw new UnauthorizedAccessException();
                }
            }
            catch
            {
                context.HttpContext.Response.StatusCode = 401;
                context.Result = new JsonResult("Unauthorized. Admin access only.");
            }
        }
    }
}
