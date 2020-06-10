using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication {

    /// <summary>
    /// A class that handles units that represent the amount of data.
    /// </summary>
    class DataCapacityUnit {

        public long Bytes { set; get; }

        /// <summary>
        /// Yetta byes. (YB)
        /// </summary>
        public long Yetta { get { return DataCapacityUnit.ConvertToYetta(this.Bytes); } }

        /// <summary>
        /// Zetta bytes. (ZB)
        /// </summary>
        public long Zetta { get { return DataCapacityUnit.ConvertToZetta(this.Bytes); } }

        /// <summary>
        /// Exa bytes. (EB)
        /// </summary>
        public long Exa { get { return DataCapacityUnit.ConvertToExa(this.Bytes); } }

        /// <summary>
        /// Peta bytes. (PB)
        /// </summary>
        public long Peta { get { return DataCapacityUnit.ConvertToPeta(this.Bytes); } }

        /// <summary>
        /// Tera bytes. (TB)
        /// </summary>
        public long Tera { get { return DataCapacityUnit.ConvertToTera(this.Bytes); } }

        /// <summary>
        /// Giga bytes. (GB)
        /// </summary>
        public long Giga { get { return DataCapacityUnit.ConvertToGB(this.Bytes); } }

        /// <summary>
        /// Mega bytes. (MB)
        /// </summary>
        public long Mega { get { return DataCapacityUnit.ConvertToMB(this.Bytes); } }

        /// <summary>
        /// Kilo bytes. (KB)
        /// </summary>
        public long Kilo { get { return DataCapacityUnit.ConvertToKB(this.Bytes); } }
        
        /// <summary>
        /// The value divided by 1YB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToYetta(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 80), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1ZB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToZetta(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 70), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1EB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToExa(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 60), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1PB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToPeta(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 50), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1TB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToTera(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 40), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1GB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToGB(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 30), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1MB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToMB(long bytes) { return long.Parse(Math.Round(bytes / Math.Pow(2, 20), MidpointRounding.AwayFromZero).ToString()); }

        /// <summary>
        /// The value divided by 1KB is rounded and returned.
        /// </summary>
        /// <param name="bytes">numeric bytes.</param>
        /// <returns></returns>
        static public long ConvertToKB(long bytes) { return byte.Parse(Math.Round(bytes / Math.Pow(2, 10), MidpointRounding.AwayFromZero).ToString()); }

    }

}
