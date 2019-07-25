using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Allyn.Cms.Rest.Models
{
    /// <summary>
    /// User login system usage entity
    /// </summary>
    public class Logoner
    {
        /// <summary>
        /// User account string
        /// </summary>
        [Required(ErrorMessage = "账号不能为空!")]
        [RegularExpression("^[a-zA-Z0-9]{5,32}$", ErrorMessage = "账号规则为5~10位的字母或数字!")]
        public string Account { get; set; }

        /// <summary>
        /// User password string
        /// </summary>
        [Required(ErrorMessage = "密码不能为空!")]
        [RegularExpression("^[\u0021-\u007e]+$", ErrorMessage = "密码规则为字母,数字,和半角符号!")]
        public string Password { get; set; }

        /// <summary>
        /// Is remember the login status
        /// </summary>
        public bool RememberMe { get; set; }
    }
}