using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using DistSysACW.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace DistSysACW.Controllers
{
    public class ProtectedController : BaseController
    {
        /// <param name = "context">DbContext set as a service in Startup.cs and dependency injected</param>
        public ProtectedController(Models.UserContext context) : base(context) { }

        [ActionName("Hello")]
        [HttpGet]
        [Authorize(Roles ="Admin, User")]
        public IActionResult Hello([FromHeader(Name = "x-api-key")]string Header)
        {
            if (Header == null)
            {
                return BadRequest("Please input a API key in the header.");
            }
            string username = UserDatabaseAccess.checkAPIReturnUser(Header);
            return Ok("Hello " + username);
        }
        [ActionName("sha1")]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult SHA1([FromQuery]string message)
        {
            if (message == null)
            {
                return BadRequest("Bad Request");
            }
            try
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);
                byte[] sha1ByteMessage;
                SHA1 sha1Provider = new SHA1CryptoServiceProvider();
                sha1ByteMessage = sha1Provider.ComputeHash(asciiByteMessage);
                string hex = ByteArrayToString(sha1ByteMessage);
                hex.ToUpper();
                return Ok(hex);
            }
            catch
            {
                return BadRequest();
            }
        }
        [ActionName("sha256")]
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public IActionResult sha256([FromQuery]string message)
        {
            if (message == null)
            {
                return BadRequest("Bad Request");
            }
            try
            {
                byte[] asciiByteMessage = System.Text.Encoding.ASCII.GetBytes(message);
                byte[] sha256ByteMessage;
                SHA256 sha256Provider = new SHA256CryptoServiceProvider();
                sha256ByteMessage = sha256Provider.ComputeHash(asciiByteMessage);
                string hex = ByteArrayToString(sha256ByteMessage);
                hex.ToUpper();
                return Ok(hex);
            }
            catch
            {
                return BadRequest();
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

    }
}