using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Services {

    /// <summary>
    /// File type identifier.
    /// </summary>
    public enum eContentType {

        /// <summary>
        /// Content-Type: text/plain
        /// </summary>
        CONTENTS_TEXT_PLAIN = 0x00,

        /// <summary>
        /// Content-Type: application/zip
        /// </summary>
        CONTENTS_ARCHIVE_ZIP,

    }

    /// <summary>
    /// OpenStack operating Interface.
    /// </summary>
    public interface IOpenStackService {

        #region Methods

        #region ObjectStorage

        bool CreateContainer(string ContainerName);

        bool DeleteContainer(string ContainerName, bool IsDeleteInObjects = false);

        bool CreateObject(string ContainerName, string ObjectName, System.IO.Stream Data, eContentType Type);

        bool DeleteObject(string ContainerName, string ObjectName);

        bool SearchContainer(string ContainerName);

        bool SearchObject(string ContainerName, string ObjectName, string ContentType = null);

        System.Collections.Generic.IEnumerable<MyApplication.Objects.Container> GetContainers();

        System.Collections.Generic.IEnumerable<MyApplication.Objects.ContainerObject> GetContainerObjects(string ContainerName, int? limit = null);

        #endregion

        /// <summary>
        /// Authenticate using your identity provider.
        /// </summary>
        /// <returns>True if authentication is successful. False otherwise.</returns>
        bool Authenticate();

        #endregion

        #region Property

        /// <summary>
        /// Get or set the open stack settings.
        /// </summary>
        Models.OpenstackOptions Options { set; get; }

        ///// <summary>
        ///// Get the user name.
        ///// </summary>
        //string UserName { get; }

        ///// <summary>
        ///// Get the password.
        ///// </summary>
        //string Password { get; }

        ///// <summary>
        ///// Get the tenant identity.
        ///// </summary>
        //string TenantId { get; }

        ///// <summary>
        ///// Get the tenant name.
        ///// </summary>
        //string TenantName { get; }

        ///// <summary>
        ///// Get the default region
        ///// </summary>
        //string DefaultRegion { get; }

        ///// <summary>
        ///// Get the serivce url for identity.
        ///// </summary>
        //string IdentityServiceURL { get; }

        ///// <summary>
        ///// Get the service url for object storage.
        ///// </summary>
        //string ObjectStorageServiceURL { get; }

        /// <summary>
        /// Get the openstack service type.
        /// </summary>
        string Type { get; }

        #endregion

    }
}
