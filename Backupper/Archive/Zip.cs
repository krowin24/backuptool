
using MyApplication.Archive;
using MyApplication.Operators;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace MyApplication {

    namespace Archive {

        public class Zip : IArchiver {

            private IApplicationOperator MyOperator = null;

            public Zip(IApplicationOperator _operator) { this.MyOperator = _operator; }

            #region Methods

            public bool Compress(string Source, string Destnation) {

                // 非同期動作中
                if (this.IsAsynchronous) return false;

                //パラメータチェック

                // ディレクトリが存在しないならエラー。
                if (!System.IO.Directory.Exists(Source))
                    return false;

                // 既にファイルが存在するならエラー。
                if (System.IO.File.Exists(Destnation))
                    return false;

                this.IsCompressed = false;
                this.IsAsynchronous = true;
                
                // タスク生成
                Task T = AsyncCompress(Source, Destnation);

                try {
                    // 処理がおわるまでまちます。
                    T.Wait();

                } catch(Exception ex) {
                    this.MyOperator.Logger.WriteLine(ex.Message, eLogLevel.ERROR);
                } finally {
                    // 後処理
                    T.Dispose();
                }
                
                return true;
            }

            public bool Discompress(string ArchiveFile, string DestnasionDirectoryPath, bool IsOverwrite = true) {

                // 非同期動作中
                if (this.IsAsynchronous) return false;

                this.IsDiscompressed = false;

                // パスチェック
                if (!System.IO.File.Exists(ArchiveFile)) {
                    //Console.WriteLine("[ERROR] 圧縮ファイルへのパスが正しくありません。");
                    return false;
                }

                if(!System.IO.Directory.Exists(DestnasionDirectoryPath)){
                    //Console.WriteLine("[ERROR] ディレクトリへのパスが正しくありません。");
                    return false;
                }

                // タスク生成
                Task T = AsyncDecompress(ArchiveFile, DestnasionDirectoryPath, IsOverwrite);

                try {
                    // 処理がおわるまでまちます。
                    T.Wait();

                } catch(Exception ex) {
                    this.MyOperator.Logger.WriteLine(ex.Message, eLogLevel.ERROR);
                } finally {
                    // 後処理
                    T.Dispose();
                }

                return true;
            }

            #endregion

            #region Async Tasks

            private async Task AsyncCompress(string Source, string Destnation) {

                // 非同期処理
                await Task.Run(() => {

                    this.MyOperator.Logger.WriteLine(System.String.Format("Archiver thread Id [{0}]", System.Threading.Thread.CurrentThread.ManagedThreadId));

                    try
                    {
                        // ディレクトリの中身を全圧縮。
                        System.IO.Compression.ZipFile.CreateFromDirectory(Source, Destnation);

                    } catch(System.Exception ex) {
                        this.MyOperator.Logger.WriteLine(ex.Message, eLogLevel.ERROR);
                    } finally{
                    }

                    this.IsCompressed = true;
                    this.OnAsyncCompressEnd(new ArchiveEventArgs(Source, Destnation));

                }).ConfigureAwait(false);

                this.IsAsynchronous = false;
            }

            private async Task AsyncDecompress(string Source, string Destnation, bool IsOverwrite) {

                // 非同期処理
                await Task.Run(() => {

                    System.IO.Compression.ZipArchive Archive = null;

                    string FullPath = String.Empty;

                    try {

                        // ZIPアーカイブを取得
                        using (Archive = System.IO.Compression.ZipFile.Open(Source, System.IO.Compression.ZipArchiveMode.Update)) {

                            foreach(var Entry in Archive.Entries) {
                                
                                // 抽出先フルパス作成
                                FullPath = System.IO.Path.Combine(Destnation, Entry.FullName);
                                
                                // ディレクトリかどうか。
                                if(System.String.IsNullOrEmpty(Entry.Name)){

                                    // ディレクトリなら、階層を再現する。
                                    if(!System.IO.Directory.Exists(FullPath)) System.IO.Directory.CreateDirectory(FullPath);

                                } else{

                                    // ファイルなら、そのまま抽出。
                                    if(IsOverwrite)
                                        Entry.ExtractToFile(FullPath, true);
                                    else
                                        if(!System.IO.File.Exists(FullPath))
                                            Entry.ExtractToFile(FullPath, true);
                                }
                            }
                        }

                        //System.IO.Compression.ZipFile.ExtractToDirectory(ArchiveFile, Directory);

                    } catch(System.Exception ex) {
                        this.MyOperator.Logger.WriteLine(ex.Message, eLogLevel.ERROR);
                    } finally{
                        Archive?.Dispose();
                    }

                    this.IsDiscompressed = true;

                }).ConfigureAwait(false);

                this.IsAsynchronous = false;
            }

            public bool IsCompressed { private set; get; }

            public bool IsDiscompressed { private set; get; }

            public bool IsAsynchronous { private set; get; }

            #endregion

            #region Event

            public event System.EventHandler<ArchiveEventArgs> AsyncEnd;

            protected virtual void OnAsyncCompressEnd(ArchiveEventArgs e) {
                var Event = AsyncEnd;
                Event?.Invoke(this, e);
            }

            #endregion

        }

    }
}

