using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allyn.Cms.Rest.Handlers
{
    /// <summary>
    /// Custom rolicy authorization requirement.
    /// </summary>
    public class PolicyRequirement : IAuthorizationRequirement { }
}
