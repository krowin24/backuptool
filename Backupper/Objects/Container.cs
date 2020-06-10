using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Objects {

    /// <summary>
    /// Represents the detailed information for a container stored in an Object Storage Provider.
    /// </summary>
    public class Container {

        public Container(string Name, int Count, long Bytes) {
            this.Name = Name;
            this.Count = Count;
            this.Bytes = Bytes;
        }

        #region Property

        /// <summary>
        /// Gets the name of the container.
        /// </summary>
        public string Name { private set; get; }

        /// <summary>
        /// Gets the number of objects in the container.
        /// </summary>
        public int Count { private set; get; }

        /// <summary>
        /// Gets the total space utilized by the objects in this container.
        /// </summary>
        public long Bytes { private set; get; }

        #endregion

    }
}
