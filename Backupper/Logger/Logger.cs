
using System;
using System.Net.Http.Headers;

namespace MyApplication {

    /// <summary>
    /// ログ操作クラス
    /// </summary>
    public class Logger : ILogger {

        #region Member

        /// <summary>
        /// ファイルストリーム
        /// </summary>
        private System.IO.FileStream m_Stream = null;

        /// <summary>
        /// ファイル書き込み用ストリーム
        /// </summary>
        private System.IO.StreamWriter m_Writer = null;

        private bool m_IsDisposed = false;

        #endregion

        /// <summary>
        /// 初期ファイル位置と現在時刻を使って初期化します。
        /// </summary>
        public Logger() {
            DateTime Now = DateTime.Now;

            this.Initialize("./logs/" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".txt");
        }

        /// <summary>
        /// パラメータを指定して初期化します。
        /// </summary>
        /// <param name="Path">ログファイルのパス</param>
        public Logger(string Path) { this.Initialize(Path); }

        ~Logger() {}

        #region Methods

        public void Write(System.String Message, eLogLevel Type = eLogLevel.APPLICATION) {

            this.m_Writer?.Write("[" + this.TypeToString(Type) + "] " + Message);
            //this.m_Writer?.Flush();

        }

        public void WriteLine(System.String Message, eLogLevel Type = eLogLevel.APPLICATION) { this.Write(Message + "\r\n", Type); }

        public void Dispose()
            => this.Dispose(true);

        private void Initialize(string Path) {

            // 既にファイルがあれば例外をスローします。
            if (System.IO.File.Exists(Path))
                throw new  System.IO.IOException("The log file already exists.");

            // ログ用のディレクトリがなければ作る。
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));

            this.m_Stream = new System.IO.FileStream(Path, System.IO.FileMode.CreateNew, System.IO.FileAccess.Write);
            this.m_Writer = new System.IO.StreamWriter(this.m_Stream);

            this.m_Writer.AutoFlush = true;

        }

        private string TypeToString(eLogLevel Level) {

            switch (Level) {
                case eLogLevel.APPLICATION:
                    return "APP";
                case eLogLevel.ERROR:
                    return "ERROR";
            }

            return String.Empty;
        }

        protected virtual void Dispose(bool diposing) {
            if (this.m_IsDisposed) return;

            if (diposing) {

                //this.m_Writer?.Close();
                this.m_Writer?.Dispose();

                //this.m_Stream?.Close();
                //this.m_Stream?.Dispose();
            }

            this.m_IsDisposed = true;
        }

        #endregion

    }

}
