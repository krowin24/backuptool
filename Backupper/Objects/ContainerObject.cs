using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Objects {


    /// <summary>
    /// Provides the details of an object stored in an Object Storage provider.
    /// </summary>
    public class ContainerObject {

        public ContainerObject(string _name, long _bytes, string _hash, string _content_type, DateTimeOffset _lastmodidied) {
            this.Name = _name;
            this.Hash = _hash;
            this.ContentType = _content_type;
            this.Bytes = _bytes;
            this.LastModified = _lastmodidied;
        }

        public string Name { private set; get; }

        public long Bytes { private set; get; }

        public string Hash { private set; get; }

        public string ContentType { private set; get; }

        public DateTimeOffset LastModified { private set; get; }

    }
}
