
using Microsoft.Extensions.Options;
using MyApplication.Archive;
using MyApplication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Operators {

    public class ApplicationOperator : IApplicationOperator {

        #region Member

        //private System.Timers.Timer m_Timer = null;

        private IArchiver m_Zipper = null;
        private ILogger m_Logger = null;

        private DataCapacityUnit m_Unit = null;

        #endregion

        public ApplicationOperator(IOpenStackService openstack, IOptions<Models.ApplicationOptions> options) {

            //this.m_Timer = new System.Timers.Timer();
            this.m_Zipper = new MyApplication.Archive.Zip(this);
            this.m_Unit = new DataCapacityUnit() { Bytes = 0 };

            this.Options = options.Value;
            this.OpenStack = openstack;

            // イベント登録
            //this.m_Timer.Elapsed += this.OnTimerTick;
            this.m_Zipper.AsyncEnd += this.OnAsyncCompressed;

        }

        ~ApplicationOperator() {

            this.m_Zipper.AsyncEnd -= this.OnAsyncCompressed;
            //this.m_Timer.Elapsed -= this.OnTimerTick;

            //this.m_Timer?.Close();
            //this.m_Timer?.Dispose();

        }

        #region Methods

        public void Run() {

            this.Logger.WriteLine("Application start.");

            this.Logger.WriteLine("Authenticate the open stack with your identity provider.");

            if (!this.OpenStack.Authenticate())
            {
                this.Logger.WriteLine("Could not authenticate with the entered parameters.");
                return;
            }

            this.Logger.WriteLine("Authentication was successful.");

            this.Execute();

            /*
                        this.m_Timer.Start();
                        while (this.m_Timer.Enabled) {
                            System.Console.Clear();

                            System.Console.WriteLine("=======================================");
                            System.Console.WriteLine("              Backup Tool              ");
                            System.Console.WriteLine("=======================================");
                            System.Console.WriteLine("Using open stack system is [" + OpenStack.Type + "]");
                            System.Console.WriteLine("[Commands]");
                            System.Console.WriteLine("-show: show info of a container or a object.(Example: show container/object name)");
                            System.Console.WriteLine("-delete: delete request on container or object.(Example: delete container/object containername [objectname])");
                            System.Console.WriteLine("-allcontainer: show all container infos.");
                            System.Console.WriteLine("-stop: application end.");
                            System.Console.WriteLine("=======================================");
                            System.Console.Write(">");

                            System.String Commands = System.Console.ReadLine();

                            string[] Input = Commands.Split(' ');

                            System.Console.WriteLine("\r\n");

                            IEnumerable<Objects.Container> ContainerList = null;
                            IEnumerable<Objects.ContainerObject> ObjectList = null;

                            int? limit = null;

                            switch (Input[0]) {
                                case "show":

                                    if ((Input.Length > 5) && !(Input.Length > 2)) break;

                                    if (!string.IsNullOrEmpty(Input[1]) && (Input[1] == "container")) {

                                        if ((Input.Length == 4) && !string.IsNullOrEmpty(Input[3]))
                                            limit = (int.Parse(Input[3]) > 0) ? int.Parse(Input[3]) : 0;

                                        ContainerList = OpenStack.GetContainers();

                                        if (!(ContainerList.Count() > 0)) {
                                            System.Console.WriteLine("No container.");
                                            break;
                                        }

                                        foreach (var entry in ContainerList) {
                                            if (entry.Name == Input[2]) {
                                                System.Console.WriteLine(String.Format("[{0}]   [{1}]   [{2:#,0}]", entry.Name, entry.Count, entry.Bytes));
                                                System.Console.WriteLine("-------------------------");

                                                ObjectList = OpenStack.GetContainerObjects(entry.Name, ((limit > 0) ? limit : null));

                                                if (ObjectList.Count() > 0) {
                                                    foreach (var item in ObjectList) {
                                                        System.Console.WriteLine(String.Format("|-[{0}] [{1} bytes] [{2}] [{3}] [{4}]", item.Name, item.Bytes, item.ContentType, item.LastModified, item.Hash));
                                                    }
                                                } else 
                                                    System.Console.WriteLine("No object.");

                                                System.Console.WriteLine("-------------------------");
                                                break;
                                            }
                                        }
                                    }

                                    System.Console.ReadLine();
                                    break;
                                case "delete":
                                    if ((Input.Length != 3) || string.IsNullOrEmpty(Input[2])) break;

                                    if ((Input[1] == "container") && OpenStack.SearchContainer(Input[2])) {
                                        OpenStack.DeleteContainer(Input[2], true);
                                    }

                                    if ((Input.Length != 4) || string.IsNullOrEmpty(Input[2]) || string.IsNullOrEmpty(Input[3])) break;

                                    if (Input[1] == "object" && OpenStack.SearchObject(Input[2], Input[3])) {
                                        OpenStack.DeleteObject(Input[2], Input[3]);
                                    }
                                    break;
                                case "allcontainer":
                                    this.m_Unit.Bytes = 0;

                                    ContainerList = OpenStack.GetContainers();

                                    if (ContainerList.Count() > 0) {
                                        foreach (var entry in ContainerList) {
                                            this.m_Unit.Bytes += entry.Bytes;
                                            System.Console.WriteLine(String.Format("    [{0}]   [{1}]   [{2:#,0}]", entry.Name, entry.Count, entry.Bytes));
                                        }

                                        System.Console.WriteLine("\r\nObjectstorage Usage [" + FormatDataUnitToString(this.m_Unit.Bytes, 2) + "]\r\n");

                                    } else {　System.Console.WriteLine("No containers."); }

                                    System.Console.ReadLine();
                                    break;
                                case "exit":
                                case "stop":
                                    this.m_Timer.Stop();
                                    break;
                                default:
                                    break;
                            }

                        }
            */

            this.Logger.WriteLine("Application end.");

            this.m_Logger.Dispose();

        }

        public void Execute() {

            // セーブデータディレクトリがあるかどうかチェックする。なければ作る。
            if (!System.IO.Directory.Exists(Options.SavedataPath))
                System.IO.Directory.CreateDirectory(Options.SavedataPath);

            // テンポラリ用ディレクトリのパスをチェック。
            // 中身がなければ、 ./temp で初期化する。
            if (String.IsNullOrEmpty(TemporaryPath))
                TemporaryPath = "./temp";

            // アプリケーションのパスを取得します。
            this.UpdateBasePath();

            // コンテナリスト
            System.Collections.Generic.IEnumerable<MyApplication.Objects.Container> ContainerList = OpenStack.GetContainers();

            // オブジェクトリスト
            //System.Collections.Generic.IEnumerable<MyApplication.Objects.ContainerObject> ObjectList = null;

            // オブジェクトストレージ内の全体使用率をみる

            if (ContainerList.Count() > 0) {

                DataCapacityUnit Unit = new DataCapacityUnit() { Bytes = 0 };

                // 全コンテナのサイズ計算
                foreach (var entry in ContainerList) { Unit.Bytes += entry.Bytes; }

                // 全体で50GB以上使用してるなら、オブジェクトストレージ内を一掃する。
                if (Unit.Giga > 50) {
                    foreach (var entry in ContainerList) { OpenStack.DeleteContainer(entry.Name, true); }

                    this.Logger.WriteLine("Object storage clean up executed.");
                }

            }

            // オブジェクトストレージ内に使用するコンテナ作成準備。
            // 既に同名のコンテナがあれば作りません。

            // 多重防止用汎用フラグ。存在するなら true, しないなら false
            bool IsExists = false;

            if(!OpenStack.SearchContainer(Options.UseContainerName)) OpenStack.CreateContainer(Options.UseContainerName);

            // ローカル内の最新セーブデータ日付
            string LastDateInLocal = null;

            try {
                // セーブデータがあるディレクトリのサブディレクトリを列挙します。
                string[] SubDirectories = System.IO.Directory.GetDirectories(Options.SavedataPath, "*", System.IO.SearchOption.TopDirectoryOnly);

                // サブディレクトリが１個もなければ、なにもしません。
                if (SubDirectories.Length > 0) {

                    // 関数に使用する整形したリストを作ります。
                    List<string> FormatList = new List<string>(SubDirectories.Length);

                    foreach (var entry in SubDirectories) { FormatList.Add(System.IO.Path.GetFileName(entry)); }

                    // ローカル内の最新日付を取得します。
                    LastDateInLocal = this.GetLastedDateTime(FormatList, Options.SavedataFormat);

                }

            } catch (System.Exception ex) {
                this.Logger.WriteLine(ex.Message, eLogLevel.ERROR);
            }

            // ローカルのセーブデータ名の取得に失敗してたならエラーなので、関数を抜けます。
            if (string.IsNullOrEmpty(LastDateInLocal)) {
                this.Logger.WriteLine("Failed to get local save data name.", eLogLevel.ERROR);
                return;
            }

            // アップロード準備
            // 日付の文字列形式をアップロード用に変化させます。
            DateTime offset = DateTime.ParseExact(LastDateInLocal, Options.SavedataFormat, null);
            string ArchiveFileName = offset.ToString(Options.ArchiveFormat) + ".zip";

            IsExists = false;

            ContainerList = OpenStack.GetContainers();

            // コンテナの中身がなければ、確定でアップロードします。
            if (ContainerList.Count() > 0) {

                // 既に同名のアーカイブがオブジェクトストレージ内にあるならアップロードしません。
                if (OpenStack.SearchObject(Options.UseContainerName, ArchiveFileName)) {
                    IsExists = true;

                    this.Logger.WriteLine("The same file is in object storage.");

                }
            }

            // テンポラリディレクトリを新しく作ります。
            if (!System.IO.Directory.Exists(this.TemporaryPath))
                System.IO.Directory.CreateDirectory(this.TemporaryPath);

            // オブジェクトストレージに同一アーカイブがないなら圧縮しません。
            if (!IsExists)

                // ローカル内のセーブデータディレクトリをzip形式に圧縮します。
                this.m_Zipper.Compress(Options.SavedataPath + "/" + LastDateInLocal, this.TemporaryPath + "/" + ArchiveFileName);


        }

        #region Private

        private void OnTimerTick(object Sender, System.EventArgs e) {}

        private void OnAsyncCompressed(object Sender, Archive.ArchiveEventArgs e) {

            this.Logger?.WriteLine("Compress end event.");

            // 圧縮に成功したならアップロードします。
            if (!this.m_Zipper.IsCompressed) return;

            System.IO.FileStream Data = null;

            try {
                // アーカイブをファイルストリームで開きます。
                Data = new System.IO.FileStream(e.Destnation, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                // オブジェクトストレージの指定コンテナにアーカイブをアップロードします。
                OpenStack.CreateObject(Options.UseContainerName, System.IO.Path.GetFileName(e.Destnation), Data, eContentType.CONTENTS_ARCHIVE_ZIP);

                this.Logger?.WriteLine("Archive file uploaded.");

            } catch (System.Exception ex){
                this.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
            } finally{
                // ストリームを必ず閉じます。
                Data?.Close();
                Data?.Dispose();

                // 圧縮したファイルとテンポラリディレクトリを片付けます。
                System.IO.File.Delete(this.TemporaryPath + "/" + e.Destnation);
                System.IO.Directory.Delete(this.TemporaryPath, true);
            }

        }

        private System.String GetLastedDateTime(System.Collections.Generic.IEnumerable<System.String> DirectoryNames, string Format) {

            if (DirectoryNames is null) return null;
            if (string.IsNullOrEmpty(Format)) return null;

            // 最新日付(最小値で初期化)
            System.DateTime First = DateTime.MinValue;

            // コレクションの中身が１個もなければ、なにもしない。
            if (DirectoryNames.Count() > 0) {

                // 比較用の日付リスト
                List<System.DateTime> DateList = new List<DateTime>(DirectoryNames.Count());
                
                // 文字列を日付に変換して格納
                foreach (var entry in DirectoryNames) { DateList.Add(System.DateTime.ParseExact(entry, Format, null)); }

                // 最初は適当に代入。
                First = DateList[0];

                // 日付比較と最新日付の代入。
                foreach (var entry in DateList) { if (First < entry) { First = entry; } }

            }

            return (First == DateTime.MinValue) ? null : First.ToString(Options.SavedataFormat);
        }

        /// <summary>
        /// Looks in the directory for the specified filename.
        /// </summary>
        /// <param name="DirectoryPath">directory path</param>
        /// <param name="Search">search name</param>
        /// <param name="wildcard">search pattern</param>
        /// <returns>True if it exists. False otherwise.</returns>
        private bool CheckFileInDirectory(string DirectoryPath, string Search, string wildcard = "*") {

            bool Result = false;
            
            try {
                // ディレクトリ内のファイルを列挙します。
                IEnumerable<string> FileList = System.IO.Directory.EnumerateFiles(DirectoryPath, wildcard, System.IO.SearchOption.TopDirectoryOnly);

                // ファイルが１個もなければ何もしません。
                if (FileList.Count() > 0) {
                    foreach (var entry in FileList) { if (Search == System.IO.Path.GetFileName(entry)) Result = true; }
                }

            } catch (System.Exception ex){
                this.Logger?.WriteLine(ex.Message, eLogLevel.ERROR);
            }

            return Result;
        }

        /// <summary>
        /// Convert Byte to other formats like KB, MB, GB...
        /// (KB, MB, GB, TB, PB, EB, ZB or YB)
        /// </summary>
        /// <param name="Amount">Number to convert</param>
        /// <param name="Rounding">How many decimal places are displayed? (Example: Specify 1 for the first decimal place.)</param>
        /// <returns></returns>
        private static System.String FormatDataUnitToString(long Amount, int Rounding) {

            if (Amount >= Math.Pow(2, 80))
                return Math.Round(Amount / Math.Pow(2, 70), Rounding).ToString() + " YB"; //yettabyte
            if (Amount >= Math.Pow(2, 70))
                return Math.Round(Amount / Math.Pow(2, 70), Rounding).ToString() + " ZB"; //zettabyte
            if (Amount >= Math.Pow(2, 60))
                return Math.Round(Amount / Math.Pow(2, 60), Rounding).ToString() + " EB"; //exabyte
            if (Amount >= Math.Pow(2, 50))
                return Math.Round(Amount / Math.Pow(2, 50), Rounding).ToString() + " PB"; //petabyte
            if (Amount >= Math.Pow(2, 40))
                return Math.Round(Amount / Math.Pow(2, 40), Rounding).ToString() + " TB"; //terabyte
            if (Amount >= Math.Pow(2, 30))
                return Math.Round(Amount / Math.Pow(2, 30), Rounding).ToString() + " GB"; //gigabyte
            if (Amount >= Math.Pow(2, 20))
                return Math.Round(Amount / Math.Pow(2, 20), Rounding).ToString() + " MB"; //megabyte
            if (Amount >= Math.Pow(2, 10))
                return Math.Round(Amount / Math.Pow(2, 10), Rounding).ToString() + " KB"; //kilobyte

            return Amount.ToString() + " Bytes"; //byte
        }

        private void UpdateBasePath() { this.ApplicationPath = System.IO.Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName; }

        #endregion

        #endregion

        #region Property

        public Models.ApplicationOptions Options { set; get; }

        private string TemporaryPath { set; get; }

        private string ApplicationPath { set; get; }

        private IOpenStackService OpenStack { set; get; }

        public ILogger Logger {
            get {
                if (this.m_Logger == null) { this.m_Logger = new Logger(); }
                return this.m_Logger;
            }
        }

        #endregion

    }
}
