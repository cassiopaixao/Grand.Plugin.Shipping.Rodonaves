using System;
using System.Collections.Generic;
using System.Text;

namespace Grand.Plugin.Shipping.Rodonaves.Domain
{
    /// <summary>
    /// Represents an Auth response
    /// </summary>
    public class AuthModel
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public int Expires_in { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public string UserCompanyId { get; set; }
        public string IsMaster { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expires { get; set; }
    }

}
