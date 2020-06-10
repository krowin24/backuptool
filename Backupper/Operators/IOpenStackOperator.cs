using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Operators {

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
    public interface IOpenStackOperator {

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
        /// Set or Get user name.
        /// </summary>
        string UserName { set; get; }

        /// <summary>
        /// Set or Get password.
        /// </summary>
        string Password { set; get; }

        /// <summary>
        /// Set or Get tenant identity.
        /// </summary>
        string TenantId { set; get; }

        /// <summary>
        /// Set or Get tenant name.
        /// </summary>
        string TenantName { set; get; }

        /// <summary>
        /// Set or Get default region
        /// </summary>
        string DefaultRegion { set; get; }

        /// <summary>
        /// Set the serivce url for identity.
        /// </summary>
        string IdentityServiceURL { set; }

        /// <summary>
        /// Set the service url for object storage.
        /// </summary>
        string ObjectStorageServiceURL { set; }

        /// <summary>
        /// Get the openstack operator type.
        /// </summary>
        string Type { get; }

        #endregion

    }
}
