using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Models{

    public class OpenstackOptions {

        public OpenstackOptions() { }

        /// <summary>
        /// Set or Get user name.
        /// </summary>
        public string UserName { set; get; }

        /// <summary>
        /// Set or Get password.
        /// </summary>
        public string Password { set; get; }

        /// <summary>
        /// Set or Get tenant name.
        /// </summary>
        public string TenantName { set; get; }

        /// <summary>
        /// Set or Get tenant identity.
        /// </summary>
        public string TenantId { set; get; }

        /// <summary>
        /// Set or Get default region
        /// </summary>
        public string Region { set; get; }

        /// <summary>
        /// Set the serivce url for identity.
        /// </summary>
        public string IdentityServiceURL { set; get; }
    }

}
