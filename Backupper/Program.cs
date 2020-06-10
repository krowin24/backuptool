

using ConoHaNet.Objects;
using ConoHaNet.Objects.Servers;
using Microsoft.Extensions.Configuration;
using MyApplication;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;

class Program {

    private static MyApplication.Operators.IApplicationOperator MyOperator = null;
    private static MyApplication.Config Config = null;

    static void Main(string[] Args){

        Config = new Config("appconfig.json");

        MyOperator = new MyApplication.Operators.ApplicationOperator();

        string UserName = Config.GetValue("Openstack", "UserName");
        string PassWord = Config.GetValue("Openstack", "Password");
        string TenantName = Config.GetValue("Openstack", "TenantName");
        string TenantId = Config.GetValue("Openstack", "TenantId");
        string Region = Config.GetValue("Openstack", "Region");
        string IdentityServiceURL = Config.GetValue("Openstack", "IdentityServiceURL");

        MyOperator.ExecuteInterval = System.Double.Parse(Config.GetValue("Application", "Interval"));
        MyOperator.SavedataDirectoryPath = Config.GetValue("Application", "SavedataPath");
        MyOperator.SavedataDateTimeFormat = Config.GetValue("Application", "SavedataFormat");
        MyOperator.ArchiveDateTimeFormat = Config.GetValue("Application", "ArchiveFormat");
        MyOperator.ArchiveContainerName = Config.GetValue("Application", "ContainerName");

        MyOperator.OpenStack.UserName = UserName;
        MyOperator.OpenStack.Password = PassWord;

        MyOperator.OpenStack.TenantName = TenantName;
        MyOperator.OpenStack.TenantId = TenantId;

        MyOperator.OpenStack.DefaultRegion = Region;

        MyOperator.OpenStack.IdentityServiceURL = IdentityServiceURL;

        MyOperator.Run();

        //ConoHaNet.OpenStackMember StackMember = new ConoHaNet.OpenStackMember(UserName, PassWord, TenantName, TenantId, defaultregion: region, bLazyProviderSetting: false);

        //// 認証サーバーのアドレス変更。
        //StackMember.DefaultPublicEndPoint = IdentityServiceURL;

        //System.Console.WriteLine("Authenticate using your identity provider.");
        //System.Console.WriteLine("USERNAME: " + UserName);
        //System.Console.WriteLine("PASSWORD: " + PassWord);

        //// IDプロバイダーを使って認証する。
        //StackMember.IdentityProvider.Authenticate(StackMember.Identity);

        //System.Console.WriteLine("IDの認証成功");

        //IEnumerable<ConoHaNet.Objects.Container> ContainerList = StackMember.ListContainers();
    }

    /// <summary>
    /// ByteをKB, MB, GB...のような他の形式に変換する。
    /// (KB, MB, GB, TB, PB, EB, ZB or YB)
    /// </summary>
    /// <param name="Amount">変換する数値</param>
    /// <param name="Rounding">小数点第何位まで表示するか。(例: 小数点第一位までなら１を指定します。)</param>
    /// <returns></returns>
    private static System.String FormatSize(long Amount, int Rounding) {

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

}

    /*
         
    // ログファイル
    private static Logger Logger = new Logger();

    // 処理を周期的に実行させるタイマー
    private static System.Timers.Timer ApplicationExecuteTimer;

    // 監視対象パス(ディレクトリ)
    private static string ObserveDirectoryPath = "./savedata";

        //// ConoHaNet コアクラス
        //ConoHaNet.OpenStackMember StackMember = null;

        System.IO.FileStream PutFile = null;

        System.Text.StringBuilder StringBuffer = new System.Text.StringBuilder(System.String.Empty);

        try {

            // アプリケーション開始。
            //Logger.WriteLine("Application is started.");

            //ApplicationExecuteTimer = new System.Timers.Timer(30 * 1000);
            //ApplicationExecuteTimer.Elapsed += OnTimerTick;

            //ApplicationExecuteTimer.Start();

            //// タイマーが有効ならずっと回る。
            //while (ApplicationExecuteTimer.Enabled) {
            //    string Command = System.Console.ReadLine();
            //    if (Command == "stop") ApplicationExecuteTimer.Stop();
            //}

            Logger.WriteLine("Timer tick start.");

            // パスがディレクトリかチェック。
            if (!System.IO.Directory.Exists(ObserveDirectoryPath)) {
                System.Console.WriteLine("[ERROR] Path is not directory.");
                return;
            }

            // 最新日付
            System.DateTime NewTime;

            // 圧縮するディレクトリへの相対パス
            string CompressDirectoryPath = "./savedata/";

            // 圧縮したファイルを置く場所。
            string ArchivePath = "./archive/";

            // 変換する日付書式を指定する。
            string ParseFormat = "yyyy-MM-dd HHmmss";

            // サブディレクトリ一覧取得
            string[] SubDirectories = System.IO.Directory.GetDirectories(ObserveDirectoryPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            // サブディレクトリが１個もなければ処理しません。
            if(SubDirectories.Length > 0){

                List<System.DateTime> DateList = new List<DateTime>(SubDirectories.Length);

                foreach (string entry in SubDirectories) {
                    string temp = System.IO.Path.GetFileName(entry);
                    DateList.Add(System.DateTime.ParseExact(temp, ParseFormat, null));
                }

                NewTime = DateList[0];

                foreach (var entry in DateList) {
                    if (NewTime < entry) NewTime = entry;
                }

                // 最新時刻を文字列に変換して、圧縮パスへと結合します。
                CompressDirectoryPath += NewTime.ToString(ParseFormat);
                ArchivePath += NewTime.ToString(ParseFormat) + ".zip";

                Logger.WriteLine("Lastest time directory is [" + CompressDirectoryPath + "]");

            }

            //try {

            //} catch (System.Exception ex) {
            //    System.Console.WriteLine("[ERROR]\r\n" + ex.Message);
            //    Logger.WriteLine(ex.Message, eLogLevel.ERROR);
            //}

            MyApplication.Archive.Zip Archiver = new MyApplication.Archive.Zip();

            // 最新時刻のディレクトリを圧縮する。
            Archiver.Compress(CompressDirectoryPath, ArchivePath);

            Logger.WriteLine("Timer tick end.");

            /*
            // ConoHaNet のコアクラスを作成。
            StackMember = new ConoHaNet.OpenStackMember(UserName, PassWord, TenantName, TenantId, defaultregion: region, bLazyProviderSetting: false);

            // 認証サーバーのアドレス変更。
            StackMember.DefaultPublicEndPoint = "https://identity.tyo2.conoha.io/v2.0";

            Log.WriteLine("Authenticate using your identity provider.");
            Log.WriteLine("USERNAME: " + UserName);
            Log.WriteLine("PASSWORD: " + PassWord);

            // IDプロバイダーを使って認証する。
            StackMember.IdentityProvider.Authenticate(StackMember.Identity);

            System.Console.Clear();
            System.Console.WriteLine("IDの認証成功");

            Log.WriteLine("Authenticate is Success.");

            ConoHaNet.Providers.IObjectStorageProvider ObjectStorageProvider = StackMember.FilesProvider;


        } catch (System.Exception ex) {
            Console.WriteLine("[ERROR]\r\n" + ex.Message);
            Logger.WriteLine("\r\n" + ex.Message, MyApplication.eLogLevel.ERROR);
        } finally{

            PutFile?.Close();
            PutFile?.Dispose();

            ApplicationExecuteTimer?.Close();
            ApplicationExecuteTimer?.Dispose();

            Logger.WriteLine("Application is end.");
        }

        // 作業ディレクトリを取得します。
        System.Reflection.Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
        string MyApplicationPath = System.IO.Path.GetDirectoryName(MyAssembly.Location);

        // 作業ディレクトリをアプリケーションディレクトリにします。
        // 絶対パス 「C:\」 指定はエラー回避用の値です。
        System.Environment.CurrentDirectory = MyApplicationPath ?? @"C:\";

        // コンテナ名
        string ContainerName = "se-test";

        // ファイルパスとオブジェクト名
        // ファイルパスは相対パスです。
        string PutFilePath =  "./put/example.txt";
        string ObjectName = System.IO.Path.GetFileName(PutFilePath);

        // ファイルを読み取り専用ストリームで開きます。
        PutFile = new System.IO.FileStream(PutFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            
#if DEBUG
        Log.WriteLine("Application Directory Path [" + MyApplicationPath + "]", MyApplication.eLogLevel.DEBUG);
#endif

        // オブジェクトストレージに接続し、コンテナを作成します。
        System.Console.WriteLine("コンテナを作成します。");
        Log.WriteLine("Create the [" + ContainerName + "] Container.");

        switch(StackMember.CreateContainer("se-test")){
            case ObjectStore.ContainerCreated:
                System.Console.WriteLine("コンテナの作成に成功しました。");
                Log.WriteLine("Created is Success.");
                break;
            case ObjectStore.ContainerExists:
                System.Console.WriteLine("既に同じ名前のコンテナが存在します。コンテナの作成に失敗しました。");
                Log.WriteLine("A container with the same name already exists. Failed to create the container.", MyApplication.eLogLevel.ERROR);
                break;
        }

        // 作成したコンテナに、ローカルからファイルをストリームで開き、それを複製して格納します。識別の為にオブジェクト名も指定します。
        Log.WriteLine("Put file is [" + PutFilePath + "], Object name is [" + ObjectName + "]");
        StackMember.CreateObject(ContainerName, PutFile, ObjectName);

        // データを渡したらストリームを閉じます。
        PutFile?.Close();
        PutFile?.Dispose();
        PutFile = null;

        // オブジェクトを更新します。
        System.Console.WriteLine("オブジェクトメタデータを更新します。");
        Log.WriteLine("Update the metadata.");
        StackMember.UpdateObjectMetadata(ContainerName, ObjectName, StackMember.GetObjectMetaData(ContainerName, ObjectName));

        // オブジェクトストレージの中身を列挙する。
        System.Console.WriteLine("コンテナを列挙します。");

        System.Collections.Generic.IEnumerable<ConoHaNet.Objects.Container> Containers = StackMember.ListContainers();
        int Count = 0;
            
        if (Containers.Count() > 0) {
            foreach (ConoHaNet.Objects.Container entry in Containers) {
                StringBuffer.Append(System.String.Format("Container Name [{0}], StoredCount [{1}], Size [{2}]", entry.Name, entry.Count, FormatSize(entry.Bytes, 2)));
                System.Console.WriteLine(StringBuffer.ToString());
#if DEBUG
                Log.WriteLine(StringBuffer.ToString());
#endif
                StringBuffer.Clear();

                // １０個以上の項目は表示しません。
                if (entry.Count > 0) {
                    Count = 0;
                    foreach(ConoHaNet.Objects.ContainerObject item in StackMember.ListObjects(entry.Name)) {
                        StringBuffer.Append(System.String.Format("    - {0}, {1}, {2}, {3}, {4}", item.Name, FormatSize(item.Bytes, 2), item.ContentType, item.Hash, item.LastModified));
                        System.Console.WriteLine(StringBuffer.ToString());
#if DEBUG
                        Log.WriteLine(StringBuffer.ToString());
#endif
                        StringBuffer.Clear();

                        if (++Count > 9) break;
                    }
                }

            }
        } else{
            System.Console.WriteLine("コンテナがありません。");
        }

        // 複製して格納したファイルを削除します。
        System.Console.WriteLine("コンテナに格納してあるオブジェクトを削除します。");
        Log.WriteLine("Delete the [" + ObjectName + "] object.");

        StackMember.DeleteObject(ContainerName, ObjectName);

        // 作成したコンテナを削除します。
        System.Console.WriteLine("コンテナを削除します。");
        Log.WriteLine("Delete the [" + ContainerName + "] Container.");

        StackMember.DeleteContainer(ContainerName);

        */

    /*
    // 圧縮するディレクトリへのパス
    string CompressTarget = "./extract";

    // 圧縮先ファイル名
    string CompressedFile = "./temp.zip";

    // 抽出するディレクトリへのパス
    string ExtractDirectoryPath = "./extract";

    MyApplication.File.Zip Archiver = new MyApplication.File.Zip();

    Archiver.Compress(CompressTarget, CompressedFile);

    // 圧縮処理の結果
    Console.Write("Compressing...");
    if(Archiver.IsCompressed){
        Console.WriteLine("successed.");
    } else{
        Console.WriteLine("failed.");
    }

    Archiver.Decompress(CompressedFile, ExtractDirectoryPath);

    // 抽出処理の結果。
    Console.Write("Decompressing...");
    if(Archiver.IsDecompressed){
        Console.WriteLine("successed.");
    } else{
        Console.WriteLine("failed.");
    }
    */


