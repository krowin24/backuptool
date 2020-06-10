
using ConoHaNet.Objects;
using MyApplication.Objects;
using MyApplication.Operators;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace MyApplication {

    namespace Operators {

        /// <summary>
        /// Operating ConoHaNet.
        /// </summary>
        public class ConoHaOperator : IOpenStackOperator {

            #region Member

            private ConoHaNet.OpenStackMember m_Openstack = null;

            private IApplicationOperator m_Core = null;

            #endregion

            public ConoHaOperator(IApplicationOperator Core) { this.m_Core = Core; }

            #region Property

            public string UserName { set; get; }

            public string Password { set; get; }

            public string TenantId { set; get; }

            public string TenantName { set; get; }

            public string DefaultRegion { set; get; }

            public string IdentityServiceURL { set { ConoHa.DefaultPublicEndPoint = value; } }

            public string ObjectStorageServiceURL { set { } }

            public string Type { get { return "ConoHaNet"; } }

            private ConoHaNet.OpenStackMember ConoHa {
                get {
                    if (this.m_Openstack == null) {
                        this.m_Openstack = new ConoHaNet.OpenStackMember();

                        this.m_Core.Logger?.WriteLine("ConoHa core created.");
                    }

                    return this.m_Openstack;

                }
            }

            #endregion

            #region Methods

            public bool Authenticate() {

                this.m_Core.Logger?.WriteLine("Identity authenticate request.");
                //this.m_Core.Logger?.WriteLine(System.String.Format("[NAME] {0} / [PASS] {1}", this.UserName, this.Password));

                ConoHa.UserName = this.UserName;
                ConoHa.TenantId = this.TenantId;
                ConoHa.TenantName = this.TenantName;

                ConoHaNet.Objects.CloudIdentity Identity = null;

                if ((this.TenantName != null) || (this.TenantId != null))
                    Identity = new ConoHaNet.Objects.Identity.CloudIdentityWithProject() { Username = this.UserName, Password = this.Password, ProjectId = (new ConoHaNet.Objects.ProjectId(this.TenantId ?? this.TenantName)), ProjectName = this.TenantName };
                else
                    Identity = new CloudIdentity() { Username = this.UserName, Password = this.Password };

                ConoHa.Identity = Identity;
                ConoHa.DefaultRegion = this.DefaultRegion;

                try {
                    ConoHa.IdentityProvider.Authenticate(ConoHa.Identity);
                } catch (System.Exception ex){
                    this.m_Core.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
                    return false;
                }

                return true;
            }

            public bool CreateContainer(string ContainerName){

                try {

                    this.m_Core.Logger?.WriteLine("Request the create Container [" + ContainerName + "]");

                    ConoHaNet.Objects.ObjectStore Result = ConoHa.CreateContainer(ContainerName);

                    switch (Result) {
                        case ObjectStore.ContainerCreated:
                            this.m_Core.Logger?.WriteLine("Created is Success.");
                            break;
                        case ObjectStore.ContainerExists:
                            this.m_Core.Logger?.WriteLine("A container with the same name already exists. Failed to create the container.", MyApplication.eLogLevel.ERROR);
                            break;
                    }

                } catch (System.Exception ex){
                    this.m_Core.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
                    return false;
                }

                return true;
            }

            public bool DeleteContainer(string ContainerName, bool IsDeleteInObjects = false) {

                try {
                    this.m_Core.Logger?.WriteLine("Request the delete container [" + ContainerName + "]");

                    ConoHa.DeleteContainer(ContainerName, IsDeleteInObjects);

                } catch (System.Exception ex){
                    this.m_Core.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
                    return false;
                }

                return true;
            }

            public bool CreateObject(string ContainerName, string ObjectName, Stream Data, eContentType Type){

                try {
                    this.m_Core.Logger?.WriteLine("Request the create object [" + ObjectName + "] in [" + ContainerName + "]");

                    ConoHa.CreateObject(ContainerName, Data, ObjectName, this.ContentTypeToString(Type));

                    // update container info.
                    ConoHa.CreateContainer(ContainerName);
                } catch (System.Exception ex){
                    this.m_Core.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
                    return false;
                }

                return true;
            }

            public bool DeleteObject(string ContainerName, string ObjectName) {

                try {
                    this.m_Core.Logger?.WriteLine("Request the delete object [" + ObjectName + "] In [" + ContainerName + "]");

                    ConoHa.DeleteObject(ContainerName, ObjectName);

                    // update container info.
                    ConoHa.CreateContainer(ContainerName);
                } catch (System.Exception ex){
                    this.m_Core.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
                    return false;
                }

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

        }

    }
    
}
