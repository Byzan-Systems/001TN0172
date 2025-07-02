using HDFCMSILWebMVC.Controllers;
using HDFCMSILWebMVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _001TN0172
{
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;

        public SessionValidationMiddleware(RequestDelegate next, IMemoryCache cache)
        {
            _next = next;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var username = context.User.Identity.Name;
                var sessionToken = context.Session.GetString("SessionToken");
                using (var db = new HDFCMSILWebMVC.Entities.DatabaseContext())
                {
                    var validTokenlist = db.Set<MSIL_LoginLogout>().FromSqlRaw("select MSIL_LogoutDatetime,logID,SessionID,IsActive,IPAddress from MSIL_LoginLogout where User_Id='" + UserSession.LoginID + "' order by CONVERT(int, logID) desc").ToList();
                    var validToken = validTokenlist[0].SessionID;
                    if (sessionToken != validToken)
                    {
                        // Session is invalid
                        await context.SignOutAsync();
                        context.Response.Redirect("/Login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

}
