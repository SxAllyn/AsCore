using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Allyn.Cms.Rest.Areas.Mgr.Controllers
{
    /// <summary>
    /// User manager
    /// </summary>
    [Route("Mgr/[controller]")]
    [ApiController]
    [Authorize("AsMgrPolicy")]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Get a user entity
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUser")]
        public async Task<dynamic> GetUserAsync() {
            return await Task.Run(() => new { Key = Guid.NewGuid(), Name = "Allyn" });
        }
    }
}