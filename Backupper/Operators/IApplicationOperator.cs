
using MyApplication.Services;
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
        /// Set or get the application setting.
        /// </summary>
        Models.ApplicationOptions Options { set; get; }

        /// <summary>
        /// Get the logger.
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Get the open stack operating interface.
        /// </summary>
        //IOpenStackService OpenStack { get; }

        #endregion

    }
}
