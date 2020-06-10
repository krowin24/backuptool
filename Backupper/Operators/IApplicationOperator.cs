
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Operators {

    /// <summary>
    /// Application operating Interface.
    /// </summary>
    public interface IApplicationOperator {

        #region Method

        /// <summary>
        /// Application run method.
        /// </summary>
        void Run();

        #endregion

        #region Property

        /// <summary>
        /// Set or Get the processing interval. (ms)
        /// </summary>
        /// <remarks>
        /// Returns -1 if the timer is not implemented.
        /// </remarks>
        double ExecuteInterval { set; get; }

        /// <summary>
        /// Set or Get the savedata directory path.
        /// </summary>
        string SavedataDirectoryPath { set; get; }

        /// <summary>
        /// Set or Get the date format of the save data directory.
        /// </summary>
        /// <remarks>
        /// (initial value: yyyy-MM-dd HHmmss)
        /// </remarks>
        string SavedataDateTimeFormat { set; get; }

        /// <summary>
        /// Set or get the archive date format.
        /// </summary>
        /// <remarks>
        /// (initial value: yyyy-MM-dd_HHmmss)
        /// </remarks>
        string ArchiveDateTimeFormat { set; get; }

        /// <summary>
        /// Set or Get the Archive container name on object storage.
        /// </summary>
        string ArchiveContainerName { set; get; }

        /// <summary>
        /// Get the logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Get the open stack operating interface.
        /// </summary>
        IOpenStackOperator OpenStack { get; }

        #endregion

    }
}
