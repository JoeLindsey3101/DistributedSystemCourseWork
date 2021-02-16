using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using DistSysACW.Models;
using System.Net.Http.Headers;
using System.Threading;

namespace DistSysACW.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        public System.Collections.Specialized.NameValueCollection Headers { get; }

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, Models.UserContext dbContext)
        {
            #region Task5
            // TODO:  Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            //        Then set the correct roles for the User, using claims [FromHeader]string api
            #endregion
            IHeaderDictionary keys = context.Request.Headers;
            if (keys.ContainsKey("x-api-key"))
            {
                string currentHeader = keys["x-api-key"];
                if (currentHeader != null)
                {
                    using (var ctx = dbContext)
                    {
                        Models.User u = ctx.Users.FirstOrDefault(U => U.ApiKey == currentHeader);
                        if (u != null)
                        {
                            string Username = u.UserName;
                            string Role = u.Role;
                            Claim username = new Claim(ClaimTypes.Name, Username);
                            Claim role = new Claim(ClaimTypes.Role, Role);
                            var claims = new List<Claim>();
                            claims.Add(username);
                            claims.Add(role);
                            ClaimsIdentity claimsIdentity = new ClaimsIdentity(currentHeader);
                            claimsIdentity.AddClaims(claims);
                            context.User.AddIdentity(claimsIdentity); 
                        }
                    }
                }
            }

            



            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }

    }
}
