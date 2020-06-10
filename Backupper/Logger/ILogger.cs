using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication {

    public enum eLogLevel {
        APPLICATION = 0x00,
        ERROR,
    }

    /// <summary>
    /// Log File operation Interface.
    /// </summary>
    public interface ILogger{

        #region Method

        /// <summary>
        /// Write to the log file.
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="Level">Log message level</param>
        void Write(System.String Message, eLogLevel Level = eLogLevel.APPLICATION);

        /// <summary>
        /// Write line to the log file.
        /// </summary>
        /// <param name="Message">Log message</param>
        /// <param name="Level">Log message level</param>
        void WriteLine(System.String Message, eLogLevel Level = eLogLevel.APPLICATION);

        #endregion

        #region Property



        #endregion

    }

}
