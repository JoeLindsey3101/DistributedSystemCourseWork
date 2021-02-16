using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.IO;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DistSysACW.Controllers
{

    public class UserController : BaseController
    {
        /// <param name = "context">DbContext set as a service in Startup.cs and dependency injected</param>
        public UserController(Models.UserContext context) : base(context) { }

        [ActionName("New")]
        [HttpGet]
        public IActionResult Get([FromQuery]string username)
        {
            string response = UserDatabaseAccess.checkUsername(username);
            string Return = "Something went wrong";
            if (response == "true")
            {
                Return = "True - User Does Exist! Did you mean to do a POST to create a new user?";

            }
            else if (response == "false")
            {
                Return = "False - User Does Not Exist! Did you mean to do a POST to create a new user?";

            }
            return Ok(Return);
        }
        [ActionName("New")]
        [HttpPost]
        public IActionResult Post([FromBody]string username)
        {

            string response = "Something went wrong";
            if (username == null)
            {
                return BadRequest("Oops. Make sure your body contains a string with your username and your Content - Type is Content - Type:application / json");
            }
            string answer = UserDatabaseAccess.checkUsername(username);
            if (answer == "false")
            {
                string GUID = UserDatabaseAccess.CreateNew(username);
                response = GUID;
                return Ok(response);
            }
            else if (answer == "true")
            {
                response = "Oops. This username is already in use. Please try again with a new username.";
                return StatusCode(403, response);
            }

            return BadRequest(response);
        }
        [ActionName("RemoveUser")]
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public IActionResult Delete([FromHeader(Name = "x-api-key")]string Header, [FromQuery]string username)
        {
            if (Header == null)
            {
                return BadRequest("Please input a API key in the header.");
            }
            if (username == null)
            {
                return BadRequest("Please send a username in the URI");
            }
            string APIAnswer = "";
            if (Header != "")
            {
                APIAnswer = UserDatabaseAccess.checkAPIReturnUser(Header);
                if (APIAnswer == username)
                {
                    bool response = UserDatabaseAccess.DeleteUser(Header, username);
                    return Ok(response);
                }
                else if (APIAnswer != username)
                {
                    return BadRequest("Please make sure that the username sent in the query matches the username for the API key");
                }
            }


            return BadRequest("Something went wrong.");
        }
        [ActionName("ChangeRole")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeRole([FromBody]string jsonObject)
        {
            try
            {
                //split the json string to get the username and role out of the string object.
                Stream stream = Request.Body;
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                string json = new StreamReader(stream).ReadToEnd();
                string[] vs = new string[4];
                vs = json.Split(':', ',');
                string Username = vs[1];
                string Role = vs[3];
                string Response = UserDatabaseAccess.ChangeRole(Username, Role);
                if (Response == "true")
                {
                    return Ok("Done");
                }
                else if (Response == "User Doesnt Exist")
                {
                    return BadRequest("NOT DONE: Role does not exist");
                }
                else if (Response == "username doesnt match")
                {
                    return BadRequest("NOT DONE: Username does not exist");
                }
                else if (Response == "something went wrong")
                {
                    return BadRequest("NOT DONE: An error occured");
                }
                return BadRequest("NOT DONE: An error occured");
            }
            catch
            {
                return BadRequest("NOT DONE: An error occured");
            }
        }
    }
}
