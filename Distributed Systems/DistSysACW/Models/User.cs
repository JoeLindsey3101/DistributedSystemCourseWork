using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DistSysACW.Models
{
    public class User
    {
        #region Task2
        // TODO: Create a User Class for use with Entity Framework
        // Note that you can use the [key] attribute to set your ApiKey Guid as the primary key 
        #endregion        
        [Key]
        public string ApiKey { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    #region Task13?
    // TODO: You may find it useful to add code here for Logging
    #endregion

    public static class UserDatabaseAccess
    {
        #region Task3 
        // TODO: Make methods which allow us to read from/write to the database 
        #endregion
        static readonly Models.UserContext _context;
        public static string CreateNew(string username)
        {
            using (var ctx = new UserContext())
            {
                string role;
                if (!ctx.Users.Any())
                {
                    role = "Admin";
                }
                else
                {
                    role = "User";
                }
                Guid g = Guid.NewGuid();
                User user = new User { ApiKey = g.ToString(), UserName = username, Role = role };
                ctx.Users.Add(user);
                ctx.SaveChanges();
                return g.ToString();
            }
             
        }
        public static string CheckAPI(string API)
        {
            using (var ctx = new UserContext())
            {
                try 
                {
                   User u = ctx.Users.FirstOrDefault(U => U.ApiKey == API);
                   return "true";
                }
                catch
                {
                    return "false";
                }
            }
        }
        public static string CheckAPIUser(string API, string username)
        {
            using (var ctx = new UserContext())
            {
                try
                {
                    User u = ctx.Users.FirstOrDefault(U => U.ApiKey == API);
                    u = ctx.Users.FirstOrDefault(U => u.UserName == username);
                    return "true";
                }
                catch
                {
                    return "false";
                }
            }
        }
        
        public static string checkAPIReturnUser(string API)
        {
            try
            {
                using (var ctx = new UserContext())
                {
                    User Username = ctx.Users.FirstOrDefault(u => u.ApiKey == API);
                    return Username.UserName;
                }
                
            }
            catch
            {
                return "Something went wrong.";
            }
        }
        public static string checkUsername(string username)
        {
            try
            {
                using (var ctx = new UserContext())
                {
                    string response = "Something went wrong";
                    User u = ctx.Users.FirstOrDefault(U => U.UserName == username);
                    if (u == null)
                    {
                        response = "false";
                    }
                    else if (u != null)
                    {
                        response = "true";
                    }
                    return response;
                }
            }
            catch
            {
                return "false";
            }
        }

        public static string ChangeRole(string username, string role)
        {
            try
            {
                using (var ctx = new UserContext())
                {
                    User u = ctx.Users.FirstOrDefault(U => U.UserName == username);
                    if (u != null)
                    {
                        if (role == "Admin" || role == "User")
                        {
                            u.Role = role;
                            ctx.SaveChanges();
                            return "true";
                        }
                        else
                        {
                            return "User Doesnt Exist";
                        }
                    }
                    else if (u == null)
                    {
                        return "username doesnt match";
                    }
                    return "something went wrong";
                }

            }
            catch
            {
                return "something went wrong";
            }
        }

        public static bool DeleteUser(string API, string username)
        {
            using (var ctx = new UserContext())
            {
                try
                {
                    ctx.Users.Remove(ctx.Users.SingleOrDefault(u => u.ApiKey == API));
                    ctx.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }


}