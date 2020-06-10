using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Archive{

    public enum eArchiveFileType {

        /// <summary>
        /// zip Archive
        /// </summary>
        ARCHIVE_TYPE_ZIP = 0x00,
    }

    /// <summary>
    /// Interface for archive operations.
    /// </summary>
    public interface IArchiver {

        #region Methods

        /// <summary>
        /// Specify a directory to compress to archive file.
        /// </summary>
        /// <param name="SourceDirectoryPath">Path to a directory to compress.</param>
        /// <param name="ArchiveFilePath">Path to the archive file.</param>
        /// <returns>Success is true. Failed is false.</returns>
        bool Compress(string SourceDirectoryPath, string ArchiveFile);

        /// <summary>
        /// Specify an archive file and extract it to a directory.
        /// </summary>
        /// <remarks>
        /// If overwrite mode is enabled, all files will be overwritten.
        /// </remarks>
        /// <param name="ArchiveFile"></param>
        /// <param name="DestnasionDirectoryPath"></param>
        /// <param name="IsOverwrite"></param>
        /// <returns>Success is true. Failed is false.</returns>
        bool Discompress(string ArchiveFile, string DestnasionDirectoryPath, bool IsOverwrite = true);

        #endregion

        #region Property

        bool IsCompressed { get; }

        bool IsDiscompressed { get; }

        bool IsAsynchronous { get; }

        #endregion

        #region Event

        event System.EventHandler<ArchiveEventArgs> AsyncEnd;

        #endregion

    }

}
