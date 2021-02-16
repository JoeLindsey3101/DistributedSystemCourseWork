using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DistSysACW.Controllers
{
    public class TalkBackController : BaseController
    {
        /// <summary>
        /// Constructs a TalkBack controller, taking the UserContext through dependency injection
        /// </summary>
        /// <param name="context">DbContext set as a service in Startup.cs and dependency injected</param>
        public TalkBackController(Models.UserContext context) : base(context) { }


        [ActionName("Hello")]
        public string Get()
        {
            // port number : https://localhost:5000
            #region TASK1
            // TODO: add api/talkback/hello response
            #endregion
            string Hello = "Hello World";
            return Hello;
        }

        [ActionName("Sort")]
        public IActionResult Get([FromQuery]string[] integers)
        {
            try
            {
                int[] myInt = Array.ConvertAll(integers, int.Parse);
            }
            catch
            {
                return BadRequest("Please only input integers.");
            }
            
                #region TASK1
                // TODO: 
                // sort the integers into ascending order
                // send the integers back as the api/talkback/sort response
                #endregion
                if (integers == null)
                {
                    return Ok(integers);
                }
                else if (integers != null)
                {
                    Array.Sort(integers);
                    return Ok(integers);
                }

            return null;
        }
    }
}
