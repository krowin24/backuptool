using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Models {

    public class ApplicationOptions {

        public ApplicationOptions() {}

        /// <summary>
        /// Set or Get the processing interval. (ms)
        /// </summary>
        /// <remarks>
        /// Returns -1 if the timer is not implemented.
        /// </remarks>
        public int Interval { set; get; } = (30 * 1000);

        /// <summary>
        /// Set or Get the savedata directory path.
        /// </summary>
        public string SavedataPath { set; get; } = "./savedata";

        /// <summary>
        /// Set or Get the date format of the save data directory.
        /// </summary>
        /// <remarks>
        /// (initial value: yyyy-MM-dd HHmmss)
        /// </remarks>
        public string SavedataFormat { set; get; } = "yyyy-MM-dd HHmmss";

        /// <summary>
        /// Set or get the archive date format.
        /// </summary>
        /// <remarks>
        /// (initial value: yyyy-MM-dd_HHmmss)
        /// </remarks>
        public string ArchiveFormat { set; get; } = "yyyy-MM-dd_HHmmss";

        /// <summary>
        /// Set or Get the Archive container name on object storage.
        /// </summary>
        public string UseContainerName { set; get; } = "backup";

    }
}
