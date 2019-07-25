using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allyn.Cms.Mgr.AppStrat
{
    public class JwtSetting
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Signing { get; set; }
    }
}
