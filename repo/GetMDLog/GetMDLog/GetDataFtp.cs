using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Authentication;

namespace GetMDLog
{
    class GetDataFtp
    {
        public static void DownloadFile()
        {
            string KVSDPass;
            string VTCFPass;
            List<string> MDMachineInfo = new List<string>();
            List<string> MDKVIP = new List<string>();
            List<string> MDVTIP = new List<string>();
            List<KeyValuePair<string, List<string>>> MachineList = new List<KeyValuePair<string, List<string>>>();

            GetMDLog.Config.LoadSetting();
            KVSDPass = Config.Settings.KVSDPass;
            VTCFPass = Config.Settings.VTCFPass;            
            MachineList = Config.Settings.MachineList.ToList();

            foreach (KeyValuePair<string, List<string>> MD in MachineList)
            {
                //VTアラーム履歴を取得
                try
                {
                    using (var conn = new FtpClient(MD.Value[1], "VT", "VT"))
                    {
                        conn.ConnectTimeout = 5000;
                        conn.Connect();
                        //デフォルトにすると日本語を認識してくれるようになる
                        conn.Encoding = Encoding.Default;

                        DateTime setdate = DateTime.Now.AddDays(-1);
                        DateTime filedate;

                        //conn.DeleteFile("/MMC/log7/test.csv");
                        //conn.UploadFile(@"D:\test\MD\log000_樹脂量2207250612.csv", "/MMC/log7/test.csv");
                        //conn.UploadFile("/MMC/log7/test.csv", @"D:\test\MD\test.csv");

                        //// download a file and ensure the local directory is created
                        //conn.DownloadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto_ftp.csv", @"tmp\00002_bto.csv");

                        //OPL取得の場合は"/CF/VTOPL/00000_00999"
                        foreach (var item in conn.GetListing(VTCFPass, FtpListOption.Recursive))
                        {
                            switch (item.Type)
                            {

                                case FtpObjectType.Directory:
                                    break;

                                case FtpObjectType.File:
                                    filedate = item.Modified;//conn.GetModifiedTime(item.FullName);
                                    if (filedate >= setdate)
                                    {
                                        string localname = @"\\svfile7\境川\工程データ\装置出力LOG\MD\" + MD.Key + @"\00000_00999\" + item.Name;
                                        //string localname = @"\\vautom1\装置出力LOG\MD\" + MD.Key + @"\OPL\" + item.Name;
                                        conn.DownloadFile(localname, item.FullName);
                                        //本番削除処理をいれる
                                        conn.DeleteFile(item.FullName);
                                    }
                                    break;

                                case FtpObjectType.Link:
                                    break;
                            }
                        }

                        // 切断
                        conn.Disconnect();
                        // 解放
                        conn.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                //KV樹脂量データを取得
                try
                {
                    using (var conn = new FtpClient(MD.Value[0], "KV", "KV"))
                    {
                        conn.ConnectTimeout = 5000;
                        conn.Connect();
                        //デフォルトにすると日本語を認識してくれるようになる
                        conn.Encoding = Encoding.Default;

                        DateTime setdate = DateTime.Now.AddDays(-1);
                        DateTime filedate;

                        //conn.DeleteFile("/MMC/log7/test.csv");
                        //conn.UploadFile(@"D:\test\MD\log000_樹脂量2207250612.csv", "/MMC/log7/test.csv");
                        //conn.UploadFile("/MMC/log7/test.csv", @"D:\test\MD\test.csv");

                        //// download a file and ensure the local directory is created
                        //conn.DownloadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto_ftp.csv", @"tmp\00002_bto.csv");

                        foreach (var item in conn.GetListing(KVSDPass, FtpListOption.Recursive))
                        {
                            switch (item.Type)
                            {

                                case FtpObjectType.Directory:
                                    break;

                                case FtpObjectType.File:                                    
                                    filedate = item.Modified;//conn.GetModifiedTime(item.FullName);
                                    if (filedate >= setdate)
                                    {
                                        string localname = @"\\svfile7\境川\工程データ\装置出力LOG\MD\" + MD.Key + @"\log7\" + item.Name;
                                        //string localname = @"D:\test\MD\" + "test2.csv";
                                        conn.DownloadFile(localname, item.FullName);
                                        //本番削除処理をいれる
                                        conn.DeleteFile(item.FullName);
                                    }

                                    break;

                                case FtpObjectType.Link:
                                    break;
                            }
                        }

                        // 切断
                        conn.Disconnect();
                        // 解放
                        conn.Dispose();
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            //foreach終了
            }
        }

        //違う接続方法
        public static void DownloadFile2()
        {
            FtpClient client = new FtpClient();

            client.Host = "10.129.97.182";
            client.Port = 21;
            // 資格情報の設定
            client.Credentials = new NetworkCredential("KV", "KV");
            // 要求の完了後に接続を閉じる
            client.SocketKeepAlive = false;
            // Explicit設定
            //client.EncryptionMode = FtpEncryptionMode.Explicit;
            // プロトコルはTls
            client.SslProtocols = SslProtocols.Tls;
            // 接続タイムアウトを5秒に設定
            client.ConnectTimeout = 5000;
            // 証明書の内容を確認しない
            //client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);

            try
            {
                // 接続
                client.Connect();
                client.Encoding = Encoding.Default;
                //client.Encoding("euc_jp");

                client.DownloadFile(@"C:\test\MD\log001_樹脂量2203281215.csv", "/MMC/log7/log001_樹脂量2203281215.csv");
                // ファイルのアップロード
                //client.UploadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto.csv", "00002_bto.csv");

                //client.DeleteFile("/MMC/log7/test.csv");
                //client.UploadFile(@"D:\test\MD\log000_樹脂量2207250612.csv", "/MMC/log7/test.csv");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // 切断
                client.Disconnect();
                // 解放
                client.Dispose();
            }

        }
        private void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            // 証明書の内容を確認しない
            e.Accept = true;
        }
    }
}
