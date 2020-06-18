
using ConoHaNet.Objects;
using Microsoft.Extensions.Options;
using MyApplication.Objects;
using MyApplication.Operators;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace MyApplication.Services {

    /// <summary>
    /// Operating ConoHaNet.
    /// </summary>
    public class ConoHaService : IOpenStackService {

        #region Member

        private ConoHaNet.OpenStackMember m_Openstack = null;

        #endregion

        public ConoHaService(IOptions<Models.OpenstackOptions> opstions) {
            Options = opstions.Value;
        }

        #region Methods

        public bool Authenticate() {

            ConoHa.UserName = Options.UserName;
            ConoHa.TenantId = Options.TenantId;
            ConoHa.TenantName = Options.TenantName;

            ConoHaNet.Objects.CloudIdentity Identity = null;

            if ((Options.TenantName != null) || (Options.TenantId != null))
                Identity = new ConoHaNet.Objects.Identity.CloudIdentityWithProject() { Username = Options.UserName, Password = Options.Password, ProjectId = (new ConoHaNet.Objects.ProjectId(Options.TenantId ?? Options.TenantName)), ProjectName = Options.TenantName };
            else
                Identity = new CloudIdentity() { Username = Options.UserName, Password = Options.Password };

            ConoHa.Identity = Identity;
            ConoHa.DefaultRegion = Options.Region;

            ConoHa.IdentityProvider.Authenticate(ConoHa.Identity);

            return true;
        }

        public bool CreateContainer(string ContainerName){

            return (ConoHa.CreateContainer(ContainerName) == ObjectStore.ContainerCreated) ? true : false;
        }

        public bool DeleteContainer(string ContainerName, bool IsDeleteInObjects = false) {

            ConoHa.DeleteContainer(ContainerName, IsDeleteInObjects);

            return true;
        }

        public bool CreateObject(string ContainerName, string ObjectName, Stream Data, eContentType Type){

            ConoHa.CreateObject(ContainerName, Data, ObjectName, this.ContentTypeToString(Type));

            return true;
        }

        public bool DeleteObject(string ContainerName, string ObjectName) {

            ConoHa.DeleteObject(ContainerName, ObjectName);

            // update container info.
            ConoHa.CreateContainer(ContainerName);

            return true;
        }

        public System.Collections.Generic.IEnumerable<MyApplication.Objects.Container> GetContainers() {

            IEnumerable<ConoHaNet.Objects.Container> ContainerList = ConoHa.ListContainers();

            List<MyApplication.Objects.Container> Result = null;

            if ((ContainerList != null) && ContainerList.Count() > 0) {
                Result = new List<MyApplication.Objects.Container>(ContainerList.Count());

                foreach (var entry in ContainerList) {
                    Result.Add(new MyApplication.Objects.Container(entry.Name, entry.Count, entry.Bytes));
                }
            }

            return Result;
        }

        public System.Collections.Generic.IEnumerable<MyApplication.Objects.ContainerObject> GetContainerObjects(string ContainerName, int? limit = null) {

            System.Collections.Generic.IEnumerable<ConoHaNet.Objects.ContainerObject> ObjectList = ConoHa.ListObjects(ContainerName, limit);
                
            List<MyApplication.Objects.ContainerObject> Result = null;

            if ((ObjectList != null) && ObjectList.Count() > 0) {

                Result = new List<MyApplication.Objects.ContainerObject>(ObjectList.Count());

                foreach (var entry in ObjectList) {
                    Result.Add(new Objects.ContainerObject(entry.Name, entry.Bytes, entry.Hash, entry.ContentType, entry.LastModified));
                }
            }

            return Result;
        }

        public bool SearchContainer(string ContainerName) {

            if (string.IsNullOrEmpty(ContainerName))
                throw new System.ArgumentException("ContainerName cannot be empty.");

            bool Result = false;

            IEnumerable<ConoHaNet.Objects.Container> ContainerList = ConoHa.ListContainers();

            if ((ContainerList != null) && ContainerList.Count() > 0) {

                foreach (var entry in ContainerList) {
                    if (entry.Name == ContainerName) {
                        Result = true;
                        break;
                    }
                }

            }

            return Result;
        }

        public bool SearchObject(string ContainerName, string ObjectName, string ContentType = null) {

            if (string.IsNullOrEmpty(ContainerName))
                throw new System.ArgumentException("ContainerName cannot be empty.");

            if (string.IsNullOrEmpty(ObjectName))
                throw new System.ArgumentException("ObjectName cannot be empty.");

            bool Result = false;

            IEnumerable<ConoHaNet.Objects.ContainerObject> ObjectList = ConoHa.ListObjects(ContainerName);

            if ((ObjectList != null) && ObjectList.Count() > 0) {
                foreach (var entry in ObjectList) {
                    if (ObjectName == entry.Name) {
                        Result = true;
                        break;
                    }
                }
            }

            return Result;

        }

        private string ContentTypeToString(eContentType Type) {

            switch (Type) {
                case eContentType.CONTENTS_TEXT_PLAIN:
                    return "text/plaie";

                case eContentType.CONTENTS_ARCHIVE_ZIP:
                    return "application/zip";
            }

            return System.String.Empty;
        }

        #endregion

        #region Property

        public Models.OpenstackOptions Options { set; get; }

        //public string UserName { private set; get; }

        //public string Password { private set; get; }

        //public string TenantId { private set; get; }

        //public string TenantName { private set; get; }

        //public string DefaultRegion { private set; get; }

        //public string IdentityServiceURL { private set; get; }

        //public string ObjectStorageServiceURL { private set; get; }
        //    = string.Empty;

        public string Type { get { return "ConoHaNet"; } }

        private ConoHaNet.OpenStackMember ConoHa {
            get {
                if (this.m_Openstack == null)
                    this.m_Openstack = new ConoHaNet.OpenStackMember();

                return this.m_Openstack;

            }
        }

        #endregion

    }

}
