using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

using TcpIp;
using static Oven_Control.OperateFile;
using static Oven_Control.Program;
using Ftp;

/*コマンド送信後一定時間待つ必要があります。
 *モニターコマンドは0.3秒以上。但しプログラム関連は0.5秒以上。
 *設定コマンドは0.5秒以上。但しプログラム関連は1秒以上。
 *
 *【設定コマンドの場合の応答データ】
 *      正常時＝OK:01,設定コマンド,デリミタ
 *      異常時＝NA,エラーメッセージ,デリミタ
 *【応答コマンドの場合の応答データ】
 *      モニターデータ,デリミタ
 */

namespace OvenCommand {

    class Oven {

        private const int MonitorWaitTime = 450;
        private const int MonitorProgramWaitTime = 750;
        private const int SettingWaitTime = 750;
        private const int SettingProgramWaitTime = 1500;

        /// <summary>
        /// オンラインコンバーターとの通信で使うデリミタ(CrLf)
        /// </summary>
        public static readonly string delimiter = "\r\n";

        /// <summary>
        /// キュア炉のIPEndPoint
        /// </summary>
        public IPEndPoint Local_endpoint { get; private set; }

        /// <summary>
        /// キュア炉の号機番号
        /// </summary>
        public int MyNo { get; private set; }

        /// <summary>
        /// キュア炉の開始時間
        /// </summary>
        public DateTime startTime { get; private set; }

        /// <summary>
        /// キュア炉の終了時間
        /// </summary>
        public DateTime endTime { get; private set; }

        /// <summary>
        /// 発生したアラーム回数
        /// </summary>
        public int alarm_count { get; private set; } = 0;

        /// <summary>
        /// キュア炉ごとのcomlogfileのファイルパス（このソフトがあるフォルダ下のファイル）
        /// </summary>
        private string comlog_file;

        /// <summary>
        /// キュア炉運転ごとのlogfileのフォルダパス(このソフトがあるフォルダ下のフォルダ）
        /// </summary>
        public string templog_folder { get; private set; }

        /// <summary>
        /// 投入製品情報の2次元配列(機種名,LotNo)
        /// </summary>
        public string[,] Input_product { get; private set; }

        /// <summary>
        /// キュア条件ファイル名(.csv)
        /// </summary>
        public string Recipe { get; private set; }

        /// <summary>
        /// キュア条件ファイルの1行目～2行目の説明分
        /// </summary>
        public string explanation { get; private set; }

        /// <summary>
        /// 温度過昇防止器設定温度
        /// </summary>
        public string overheating_temp { get; private set; }

        /// <summary>
        /// 作業者コード
        /// </summary>
        public string operator_id { get; private set; }

        /// <summary>
        /// 温度監視対象のステップ:温度情報
        /// <para>例：ステップ2を150℃で監視の場合(key=2 value=150)</para>
        /// </summary>
        private Dictionary<int, int> check_step;

        /// <summary>
        /// 温度監視を始めたかの情報
        /// <para>温度が監視温度±2℃以内になるまで監視は始めない</para>
        /// <para>例：ステップ2が監視する状態になっている場合(key=2 value=true)</para>
        /// </summary>
        private Dictionary<int, bool> check_step_started;

        /// <summary>
        /// 温度情報記録メソッド用のロックオブジェクト
        /// </summary>
        private object write_temp_lock = new object();

        /// <summary>
        /// 温度記録を書き込むファイルパス（このソフトがあるフォルダ下のファイル）
        /// </summary>
        public string templog_path { get; private set; }

        /// <summary>
        /// 運転終了した時のイベント
        /// </summary>
        public event EventHandler RunFinish;

        /// <summary>
        /// Chartにデータをプロットするようの自作のイベントハンドラー
        /// </summary>
        public delegate void ChartPlotEventHandler(object sender, ChartPlotEventArgs e);

        /// <summary>
        /// Chartにデータをプロットする用のイベント
        /// </summary>
        public event ChartPlotEventHandler ChartPlot;

        /// <summary>
        /// 温度が上下限を超えた時のイベント
        /// </summary>
        public event EventHandler TempOver;

        /// <summary>
        /// FALCON専用　温度が上下限を超えた時のイベント
        /// </summary>
        public event EventHandler TempOver_falcon;

        /// <summary>
        /// 安定するまでの時間を監視
        /// </summary>
        public double stable_time;

        /// <summary>
        /// FALCON機種の55℃総監視時間
        /// </summary>
        public double falcon_monitortime;

        /// <summary>
        /// FALCON機種の55℃監視内NG回数
        /// </summary>
        public double falcon_temp_NG_Count;

        /// <summary>
        /// コンストラクター(キュア炉番号,IP EndPoint,キュア条件,投入製品情報)
        /// </summary>
        /// <param name="myno">キュア炉の号機番号</param>
        /// <param name="local_ep">OnlineConverterのIPEndpoint(IP,Port)</param>
        /// <param name="recipe">キュア条件</param>
        /// <param name="input_product">投入製品情報の2次元配列(機種名,ロット番号)</param>
        public Oven(int myno, IPEndPoint local_ep, string recipe, string ope_id, string[,] input_product = null) {
            MyNo = myno;
            Local_endpoint = local_ep;
            Recipe = recipe + ".csv";
            operator_id = ope_id;
            Input_product = input_product;
            comlog_file = AppFolder + "/Log/oven" + MyNo + "/" + $"oven{MyNo}_com.txt";
        }


        /// <summary>
        /// 全てのコマンドで共通で使うベースメソッド
        /// <para>キュア炉からの応答がNAの場合は例外スローします</para>
        /// <para>通信エラー時も例外スローします</para>
        /// </summary>
        private string Base_command(string command) {
            
            //送信メッセージログ書き込み
            string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            WriteAppend(comlog_file, dt + "\t" + command);


            //TCP/IPクライアントインスタンス作成
            Client clt = new Client(Local_endpoint);

            string r_msg;
            try {
                //応答からデリミタ除いた文字列取得
                r_msg = clt.Send_receive(command + delimiter).Replace(delimiter, "");

                //受信メッセージログ書き込み
                dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(comlog_file, dt + "\t" + r_msg);

                //Alarm取得の場合応答が1文字の場合有り
                if (r_msg.Length > 1) {
                    //受信メッセージが異常処理だった場合
                    if (r_msg.Substring(0, 2) == "NA") {
                        throw new OvenCommandException(r_msg);
                        //r_msg = r_msg.Replace("NA:", "");
                    }
                }
            }
            catch (SocketException) {
                //TCPIP通信の接続タイムアウトの時
                throw;
            }
            catch (OvenCommandException) {
                //キュア炉からの応答がNAの時
                throw;
            }
            catch {
                //他エラー時
                throw;
            }

            return r_msg;
        }


        /// <summary>
        /// モニターコマンド：現在の装置情報を取得する
        /// <para>戻り値：測定温度(整数),(空白),運転状態,発生中の警報数</para>
        /// </summary>
        public string[] Get_Mon() {
            string r_msg = Base_command("MON?,DETAIL");
            string[] array;

            array = r_msg.Split(',');

            Thread.Sleep(MonitorWaitTime);
            return array;
        }

        /// <summary>
        /// モニターコマンド：現在のプログラム運転状態を取得する
        /// <para>通常時：実行中のステップNo,温度制御設定値,残時間,0(未使用),0(未使用)</para>
        /// <para>異常時：エラー内容</para>
        /// </summary>
        public string[] Get_PrgmMon() {
            string r_msg = Base_command("PRGM MON?");
            string[] array;

            array = r_msg.Split(',');

            Thread.Sleep(MonitorProgramWaitTime);
            return array;
        }

        /// <summary>
        /// モニターコマンド：発生中の警報とその番号を取得する
        /// <para>通常時：現在の警報発生数[,警報番号] [,警報番号]</para>
        /// <para>異常時：エラー内容</para>
        /// </summary>
        public string Get_Alarm() {
            string r_msg = Base_command("ALARM?,DETAIL");
            Thread.Sleep(MonitorWaitTime);
            return r_msg;
        }

        /// <summary>
        /// モニターコマンド：操作制限情報(キープロテクト)を取得する
        /// <para>戻り値："ON"または"OFF"</para>
        /// </summary>
        public string Get_KeyProtect() {
            string r_msg = Base_command("KEY PROTECT?");
            Thread.Sleep(MonitorWaitTime);
            return r_msg;
        }


        /// <summary>
        /// 設定コマンド：待機状態にする(パネル電源OFFの場合はONにする)
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        public void Set_Mode_Standby() {
            string r_msg = Base_command("MODE,STANDBY");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_Standby r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);
        }

        /// <summary>
        /// 設定コマンド：パネル電源をOFFにする
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        public void Set_Mode_OFF() {
            string r_msg = Base_command("MODE,OFF");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_OFF r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);
        }

        /// <summary>
        /// 設定コマンド：定値運転を開始する
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        public void Set_Mode_Constant(int set_temp, TextBox tb = null) {

           

            //前準備
            PreSetup();
            if (tb != null) { tb.Text = $"Oven{MyNo} 前準備完了\r\n"; tb.Update(); }


            string r_msg;
            //始めに上限をMaxに設定する(これをしないと温度設定順番によってはNA:DATA OUT OF RANGEが出る為）
            r_msg = Base_command("CONSTANTSET,HTEMP,210");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_Constant set_HTEMP r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);

            //温度設定
            r_msg = Base_command($"CONSTANTSET,TEMP,{set_temp.ToString()}");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_Constant set_TEMP r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);

            //温度上限絶対警報値設定
            int htemp = set_temp + 10;
            r_msg = Base_command($"CONSTANTSET,HTEMP,{htemp.ToString()}");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_Constant set_HTEMP r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);

            if (tb != null) { tb.Text += $"Oven{MyNo} 定値運転設定完了\r\n"; tb.Update(); }


            //定値運転開始
            r_msg = Base_command("MODE,CONSTANT");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_Constant start r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);

            if (tb != null) { tb.Text += $"Oven{MyNo} 定値運転開始:{set_temp.ToString()}℃\r\n"; tb.Update(); }
        }


        /// <summary>
        /// 設定コマンド：キープロテクトONにする
        /// <para>設定変更プロテクトON＋運転操作プロテクトON</para>
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        public void Set_KeyProtect_ON() {
            string r_msg = Base_command("KEYPROTECT,ON");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_KeyProtect_ON r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);
        }

        /// <summary>
        /// 設定コマンド：キープロテクトOFFにする
        /// <para>設定変更プロテクトOFF＋運転操作プロテクトOFF</para>
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        public void Set_KeyProtect_OFF() {
            string r_msg = Base_command("KEYPROTECT,OFF");

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_KeyProtect_OFF r_msg:" + r_msg);
            }

            Thread.Sleep(SettingWaitTime);
        }


        /// <summary>
        /// 装置を待機状態＋キープロテクトOFFにする
        /// <para>待機状態にならない場合は例外をスローします</para>
        /// </summary>
        private void PreSetup() {
            string[] mon_array;
            string status;

            //現在の装置状態取得
            mon_array = Get_Mon();
            status = mon_array[2];

            //電源OFFではない　かつ　待機状態ではない　か判断
            if (status != "OFF" && status != "STANDBY") {
                throw new OvenCommandException("Method:PreSetup state:oven is running!");
            }
            else {
                //キープロテクトがONならOFFにする
                if (Get_KeyProtect() == "ON") {
                    Set_KeyProtect_OFF();
                }
            }

            //電源OFF状態か判断
            if (status == "OFF") {

                //待機状態にする
                Set_Mode_Standby();

                //再度確認する
                mon_array = Get_Mon();
                status = mon_array[2];
            }

            //待機状態か判断
            if (status != "STANDBY") {
                throw new OvenCommandException("Method:PreSetup state:oven is not STANDBY");
            }

        }

        /// <summary>
        /// 指定のプログラム番号が登録されているか調べる
        /// <para>引数：1～10の整数</para>
        /// <para>登録されている場合はTrueを返す</para>
        /// </summary>
        private bool Check_PrgmUse(int num) {
            string[] r_msg = Base_command("PRGM USE?,RAM").Split(',');
            Thread.Sleep(MonitorProgramWaitTime);

            bool check = false;

            if (r_msg.Length != 1) {
                for (int i = 1; i < r_msg.Length; i++) {
                    if (int.Parse(r_msg[i]) == num) {
                        check = true;
                    }
                }
            }

            return check;
        }


        /// <summary>
        /// 設定コマンド：指定したプログラムパターンを消去する
        /// <para>固定で1番のプログラムを消すようになっています</para>
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        private void Set_PrgmErase() {

            //プログラムが登録されているか調べる
            if (Check_PrgmUse(1) == false) {
                //登録されていない場合はプログラムパターンを消す必要はない
                return;
            }

            //プログラムパターン消去
            string r_msg = Base_command("PRGM ERASE,RAM:1");
            Thread.Sleep(SettingProgramWaitTime);

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_PrgmErase r_msg:" + r_msg);
            }

            return;
        }


        /// <summary>
        /// キュア条件ファイルを読み込み、条件コマンドを返します
        /// <para>ファイルは文字コードUTF-8のtxtファイルとして下さい</para>
        /// <para>読み込み失敗時は空の配列を返します</para>
        /// <para>読み込んだ行が7行未満の場合は空の配列を返します</para>
        /// <para>3行目には監視するステップ情報が書いてある(STEP3とか) or None です</para>
        /// <para>引数：読み込むファイルのフルパス</para>
        /// </summary>
        private string[] Read_ProfileCsv(string path) {

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {
                    string[] str = sr.ReadToEnd().Replace("\r\n", "\n").Split('\n');

                    //7行未満は条件ファイルの記載がおかしい
                    if (str.Length < 7) {
                        return new string[] { };
                    }

                    //条件の説明分を取得
                    explanation = $"{str[0]}\r\n{str[1]}";

                    //過昇温防止器設定温度取得
                    overheating_temp = str[1].Split(',')[1].Replace("Overheating:", "");

                    //条件の監視するステップ番号、温度を取得
                    string[] step = str[2].Split(',');

                    check_step = new Dictionary<int, int>();
                    check_step_started = new Dictionary<int, bool>();

                    foreach (string s in step) {
                        if (s == "None") {
                            check_step = null;
                            check_step_started = null;
                            break;
                        }
                        else {
                            string[] d = s.Split(':');
                            check_step.Add(int.Parse(d[0].Replace("STEP", "")), int.Parse(d[1]));
                            check_step_started.Add(int.Parse(d[0].Replace("STEP", "")), false);
                        }
                    }

                    //プログラム作成コマンドを取得
                    string[] ret_array = new string[str.Length - 4];

                    for (int i = 4; i < str.Length; i++) {
                        ret_array[i - 4] = str[i];
                    }
                    return ret_array;
                }
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                return new string[] { };
            }

        }


        /// <summary>
        /// 設定コマンド：プログラム新規作成コマンド
        /// <para>固定で1番のプログラムを作成するようにしてください(条件csvファイルで指定)</para>
        /// </summary>
        private void Set_PrgmDataWrite() {

            //プログラムコマンドリスト作成
            string[] com = Read_ProfileCsv(RecipeFolder + "/" + Recipe);

            if (com.Length == 0) {
                throw new OvenCommandException("Method:Set_PrgmDataWrite Loading profile data failed");
            }

            try {
                string r_msg;

                //プログラムデータを順番にキュア炉に送信
                for (int i = 0; i < com.Length; i++) {
                    r_msg = Base_command(com[i]);
                    Thread.Sleep(SettingProgramWaitTime);

                    //受信メッセージがおかしい場合
                    if (r_msg.Substring(0, 2) != "OK") {
                        throw new OvenCommandException("Method:Set_PrgmDataWrite r_msg:" + r_msg);
                    }
                }
            }
            catch (Exception e) {

                //例外発生時はプログラム作成を中止する
                try {
                    Thread.Sleep(SettingProgramWaitTime);
                    Base_command("PRGM DATA WRITE, PGM1, EDIT CANCEL");
                    Thread.Sleep(SettingProgramWaitTime);
                }
                catch {
                    throw new OvenCommandException("Method:EDIT CANCEL FAILED", e);
                }
                throw;
            }
        }

        /// <summary>
        /// 設定コマンド：プログラム運転を開始する
        /// <para>固定で1番のプログラムを開始するようになっています</para>
        /// <para>受信メッセージがおかしい場合は例外をスローします</para>
        /// </summary>
        private void Set_Mode_Run() {

            //プログラム運転開始
            string r_msg = Base_command("MODE,RUN1");
            Thread.Sleep(SettingProgramWaitTime);

            //受信メッセージがおかしい場合
            if (r_msg.Substring(0, 2) != "OK") {
                throw new OvenCommandException("Method:Set_Mode_Run r_msg:" + r_msg);
            }
        }

        /// <summary>
        /// プログラム1をスタートさせる
        /// <para>この一連の作業を行います：削除⇒作成⇒開始⇒KeyProtect ON</para>
        /// <para>textBoxを渡せば、そこに進捗表示する</para>
        /// <para>どこかで例外が発生すれば例外がスローされてきます</para>
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartPrgm1(TextBox tb = null) {

            //デバッグ用 20230214
            //CLU028H0604C4-00A          test
            //string typecd = Input_product[0, 0];
            //bool judge = typecd.Contains("CS700F");
            //bool judge = typecd.Contains("CLU028");

            //メール送信テスト
            //Oven_Control.MainForm M = new Oven_Control.MainForm();
            //M.mail_send_NG1persent("1.52");

            //20230215 FALCON用変数初期化
            falcon_monitortime = 0;
            falcon_temp_NG_Count = 0;
            stable_time = 0;
            
            //前準備
            PreSetup();
            if (tb != null) { tb.Text += $"Oven{MyNo} 前準備完了\r\n"; tb.Update(); }

            //プログラム1削除
            Set_PrgmErase();
            if (tb != null) { tb.Text += $"Oven{MyNo} プログラム1削除完了\r\n"; tb.Update(); }

            //プログラム1新規作成
            Set_PrgmDataWrite();
            if (tb != null) { tb.Text += $"Oven{MyNo} プログラム1新規作成完了\r\n"; tb.Update(); }

            MessageBox.Show($"過昇温防止器設定温度が合っているか確認して下さい。\n\n設定値：{overheating_temp}", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //キープロテクトONにする
            Set_KeyProtect_ON();
            if (tb != null) { tb.Text += $"Oven{MyNo} キープロテクトON完了\r\n"; tb.Update(); }

            Thread.Sleep(1000);

            //プログラム1運転開始
            Set_Mode_Run();
            if (tb != null) { tb.Text += $"Oven{MyNo} プログラム1運転開始完了\r\n"; tb.Update(); }

            //開始時間記録
            startTime = DateTime.Now;
        }

        /// <summary>
        /// ファイル記録関連初期準備
        /// <para>失敗時は例外がスローされます</para>
        /// </summary>
        public void WriteStartInfo() {//boolからvoidに変更

            //最初に情報をファイル書き込み
            string now = $"oven{MyNo}_{startTime:yyyyMMdd-HHmmss}";

            //1回の運転ごとのログフォルダパス
            templog_folder = AppFolder + "/Log/oven" + MyNo + "/" + now;

            //温度書き込むファイルパス
            templog_path = templog_folder + "/" + $"{now}.csv";

            string text = "File," + Path.GetFileName(templog_path) + "\n";
            text += "Recipe," + Recipe + "\n";
            text += "Operator," + operator_id + "\n";

            int len = Input_product.GetLength(0);
            int n_len = 37 - len;

            //製品情報
            for (int i = 0; i < len; i++) {
                text += "Input Product," + Input_product[i, 0] + "," + Input_product[i, 1] + "\n";
            }
            //温度履歴書き込み位置調整の為の改行
            for (int i = 0; i < n_len; i++) {
                text += "\n";
            }

            text += "Time,Temperature,Status,Alarm,Alarm Detail";

            //初期情報をファイルに書き込み（ファイルは新規作成）
            WriteAppend(templog_path, text, throw_ex: true);

            //ファイルが作られたか確認
            if (File.Exists(templog_path) == false) {
                throw new OvenCommandException("初期ファイル作成に失敗しました。");
                //return false;
            }

            //条件ファイルをコピー
            try {
                File.Copy(RecipeFolder + "/" + Recipe, templog_folder + "/" + Recipe, true);
            }
            catch (Exception) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + "WriiteStartInfo Error() : Recipe file copy failed");
                throw new OvenCommandException("条件ファイルコピーに失敗しました。");
                //return false;
            }

            //return true;
        }

        public void LogTemp() {

            //タイマー使用して定期的に別スレッドにて温度取得メソッド実行
            var timerState = new TimerState { status = "RUN", filepath = templog_path };
            int period = 10000; //10秒ごとに温度チェック
            TimerCallback timerDelegate = new TimerCallback(check_temp);
            System.Threading.Timer timer = new System.Threading.Timer(timerDelegate, timerState, 0, period);

            //RUN以外になるまで無限ループ
            while (timerState.status == "RUN") { Thread.Sleep(period); }

            //タイマー無限期間にしてから破棄
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            timer.Dispose();

            //終了時間記録
            endTime = DateTime.Now;

            //FALCON機種だった場合
            if (Recipe.Contains("JUS3"))
            {
                double NG_persent = 0;
                if (falcon_temp_NG_Count > 0)
                {
                    //10秒ごとに呼び出しているので
                    NG_persent = falcon_temp_NG_Count / (falcon_monitortime / 10) * 100;
                }
                //分表示
                //falcon_monitortime = falcon_monitortime / 60;
                //20230215　FALCON用 フォームラベルへ反映
                Oven_Control.MainForm.FormInstance.LabelText = "FALCON55℃ステップ専用" + "\r\n" + "NG率:" + NG_persent.ToString("0.00") + "%\r\nNG回数:" + falcon_temp_NG_Count.ToString() + "回";


                //NG_persentが1以上ならメール 20230315
                //CSV等でデータ残すか⇒不要CET確認
                if (NG_persent >= 1)
                {
                    Oven_Control.MainForm M = new Oven_Control.MainForm();
                    M.mail_send_NG1persent(NG_persent.ToString("0.00"));
                }

                //最後に初期化
                falcon_monitortime = 0;
                falcon_temp_NG_Count = 0;
                stable_time = 0;
            }

            //運転終了イベント発生（MainFormの表示を停止中にする）
            if (RunFinish != null) {
                RunFinish(this, EventArgs.Empty);
            }

            /*
             * ここから運転終了後処理
             */
            //通信ログファイルを移動
            while (true) {
                try {
                    File.Move(comlog_file, templog_folder + "/" + $"oven{MyNo}_com.txt");
                    break;
                }
                catch (Exception e) {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"運転終了後処理：通信ログファイルを移動\tSystem Message：{e.Message}");

                    string msg = "運転終了後処理に失敗しました。\n失敗動作：通信ログファイルを移動\n管理者に連絡して下さい。\n\nリトライしますか？";
                    DialogResult result = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.No) {
                        break;
                    }
                }
            }

            //温度記録に関するzipファイル作成
            string dir = Path.GetFileName(templog_folder);
            string zip_path = FtpFolder + $"/{dir}.zip";
            while (true) {
                try {
                    ZipFile.CreateFromDirectory(templog_folder, zip_path, CompressionLevel.Optimal, false, enc);
                    break;
                }
                catch (Exception e) {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"運転終了後処理：温度記録に関するzipファイル作成\tSystem Message：{e.Message}");

                    string msg = "運転終了後処理に失敗しました。\n失敗動作：温度記録に関するzipファイル作成\n管理者に連絡して下さい。\n\nリトライしますか？";
                    DialogResult result = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.No) {
                        break;
                    }
                }
            }

            //zipファイルを共有フォルダにコピー
            string copy_dir = $"{Directory.GetParent(RecipeFolder).ToString()}\\Temperature_Log\\No.{MyNo}";
            string copy_path = Path.GetFileName(zip_path);
            copy_path = copy_dir + "\\" + copy_path;
            while (true) {
                try {
                    if (Directory.Exists(copy_dir) == false) { Directory.CreateDirectory(copy_dir); }
                    File.Copy(zip_path, copy_path);
                    break;
                }
                catch (Exception e) {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"運転終了後処理：zipファイルを共有フォルダにコピー\tSystem Message：{e.Message}");

                    string msg = "運転終了後処理に失敗しました。\n失敗動作：zipファイルを共有フォルダにコピー\nネットワーク接続を確認して下さい。\n\nリトライしますか？";
                    DialogResult result = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.No) {
                        break;
                    }
                }
            }

            //zipファイルの対応Lot一覧を作成
            string filelist_path = $"{Directory.GetParent(copy_dir).ToString()}\\{startTime:yyyy}_file_list.csv";
            while (true) {
                try {
                    if (File.Exists(filelist_path) == false) {
                        WriteAppend(filelist_path, "機種,LotNo,記録ファイル名,作業者,開始時間,終了時間,Alarm回数,キュア条件,条件説明,過昇防止器設定温度", throw_ex: true);
                    }
                    string list_text = "";
                    for (int i = 0; i < Input_product.GetLength(0); i++) {
                        list_text = $"{Input_product[i, 0]},{Input_product[i, 1]},{Path.GetFileName(zip_path)},{operator_id},{startTime:yyyy/MM/dd HH:mm:ss},{endTime:yyyy/MM/dd HH:mm:ss},{alarm_count.ToString()},{Recipe},{explanation.Replace("\r\n", ":")}";
                        WriteAppend(filelist_path, list_text, throw_ex: true);
                        WriteAppend($"{AppFolder}\\Log\\backup_file_list.csv", list_text);
                    }
                    break;
                }
                catch (Exception e) {
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend(file_errorlog, dt + "\t" + $"運転終了後処理：zipファイルの対応Lot一覧を作成\tSystem Message：{e.Message}");

                    string msg = "運転終了後処理に失敗しました。\n失敗動作：zipファイルの対応Lot一覧を作成\nネットワーク接続を確認して下さい。\n\nリトライしますか？";
                    DialogResult result = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (result == DialogResult.No) {

                        string temp_path = AppFolder + "/Temp/temp_filelist.csv";

                        if (File.Exists(temp_path) == false) {
                            WriteAppend(temp_path, "機種,LotNo,記録ファイル名,作業者,開始時間,終了時間,Alarm回数,キュア条件,条件説明,過昇防止器設定温度");
                        }
                        string list_text = "";
                        for (int i = 0; i < Input_product.GetLength(0); i++) {
                            list_text = $"{Input_product[i, 0]},{Input_product[i, 1]},{Path.GetFileName(zip_path)},{operator_id},{startTime:yyyy/MM/dd HH:mm:ss},{endTime:yyyy/MM/dd HH:mm:ss},{alarm_count.ToString()},{Recipe},{explanation.Replace("\r\n", ":")}";
                            WriteAppend(temp_path, list_text);
                        }

                        MessageBox.Show($"{temp_path}\nに一時的に情報を書き込みました。\n\n管理者に連絡して下さい", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                }
            }

            //FTPを使用して上位システムへファイル転送するなら
            if (use_ftp) {
                while (true) {
                    try {
                        //FTPアップロード先の情報読み取り
                        string[] ftp_info = ReadSeraverIP();
                        if (ftp_info != null) {
                            //FTPアップロード
                            FTPclient.UploadFile(ftp_info[0], ftp_info[1], ftp_info[2], zip_path);
                        }
                        break;
                    }
                    catch (Exception e) {
                        string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        WriteAppend(file_errorlog, dt + "\t" + $"運転終了後処理：FTPを使用して上位システムへファイル転送\tSystem Message：{e.Message}");

                        string msg = "運転終了後処理に失敗しました。\n失敗動作：FTPを使用して上位システムへファイル転送\nFTP転送先の情報が記載されているか？または、ネットワーク接続を確認して下さい。\n\nリトライしますか？";
                        DialogResult result = MessageBox.Show(msg, "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                        if (result == DialogResult.No) {
                            break;
                        }
                    }
                }
            }

            //zipファイル削除
            try {
                File.Delete(zip_path);
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + $"運転終了後処理：zipファイル削除\tSystem Message：{e.Message}");

                MessageBox.Show("運転終了後処理に失敗しました。\n\n失敗動作：zipファイル削除", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        /// <summary>
        /// 温度監視処理
        /// </summary>
        private void check_temp(object timerState) {

            string[] mon_array, prgmmon_array;
            string now_time, status, alarm;
            int temp;
            DateTime now;

            var state = timerState as TimerState;
            string text;

            //ロックして他のスレッドから同時に出来ないようにする
            lock (write_temp_lock) {

                try {
                    //温度情報取得
                    mon_array = Get_Mon();
                    now = DateTime.Now;
                    now_time = now.ToString("yyyy/MM/dd HH:mm:ss");
                    temp = int.Parse(mon_array[0]);
                    status = mon_array[2];
                    alarm = mon_array[3];
                    text = $"{now_time},{temp},{status},{alarm}";

                    if (alarm != "0") {
                        alarm_count += 1;
                        try {
                            string[] alarm_detail = Get_Alarm().Split(',');
                            for (int i = 1; i < alarm_detail.Length; i++) {
                                text += $",{alarm_detail[i]}";
                            }
                        }
                        catch {
                            text += ",Get Alarm failed";
                        }
                    }

                    //ログファイルに書き込み
                    WriteAppend(state.filepath, text);

                    //プログラム運転状態でない and プログラム中断状態でないなら終了
                    //プログラム中断は警告、異常が発生した時に起こる
                    if (status != "RUN" && status != "RUN PAUSE") {
                        state.status = status;
                        return;
                    }

                    //ステップ情報取得
                    prgmmon_array = Get_PrgmMon();

                    //MainFormのChartPlotメソッドに渡すデータの設定
                    ChartPlotEventArgs cpea = new ChartPlotEventArgs();
                    cpea.date_time = now;
                    cpea.temperature = temp;

                    //監視ステップなら温度チェック（±5℃）
                    foreach (int i in check_step.Keys) {

                        //現在は監視ステップかどうか
                        if (i == int.Parse(prgmmon_array[0])) {

                            //FALCON向け監視を追加 20230214追加
                            //レシピ名JUS3がFALCON専用ならtrue
                            bool judge = Recipe.Contains("JUS3");

                            //FALCON55度監視専用　監視ステップに対して監視する状態になっているかどうか
                            if (check_step_started[i] == false && prgmmon_array[0] == "2" && judge == true)
                            {
                                //±1度ではなくて、設定温度で監視開始しないとはずれ判定となるため
                                int check_start_upper = check_step[i];
                                int check_start_lower = check_step[i];

                                //※オーバーシュートするようなら、〇〇回設定温度にならないと監視を始めないようにするかなど工夫が必要
                                //FALCON専用 温度が設定温度範囲内なら
                                if (check_start_lower <= temp && temp <= check_start_upper)
                                {
                                    //安定時間監視用 ※複数ステップで監視がある場合、次のステップに行ったときに初期化を追加
                                    stable_time += 10;
                                    //10回設定温度で安定したらオーバーシュート領域を抜けたと想定
                                    if(stable_time >= 100)
                                    {
                                        //このステップの監視スタート
                                        check_step_started[i] = true;

                                        //ここで次以降のステップで監視がある場合、変数を初期化しても良い
                                        //stable_time = 0;
                                    }

                                }
                            }
                            //監視ステップに対して監視する状態になっているかどうか
                            else if (check_step_started[i] == false) {
                                int check_start_upper = check_step[i] + 2;
                                int check_start_lower = check_step[i] - 2;

                                //温度が設定温度±2℃の範囲内なら
                                if (check_start_lower <= temp && temp <= check_start_upper) {
                                    //このステップの監視スタート
                                    check_step_started[i] = true;
                                }
                            }

                            

                            //FALCON用追加　読み込んだ機種コードの先頭を取得 品種で判定するのは× レシピ名で判定
                            //string typecd = Input_product[0, 0];
                            //bool judge = typecd.Contains("CS700F");

                            int upper;
                            int lower;

                            //狙い値に対して、1度以上2度以下で外れている回数をカウントする専用
                            int upper2 = check_step[i] + 2; ;
                            int lower2 = check_step[i] - 2; ;

                            //FALCON 55度ステップのみを±1度で監視したい場合
                            if (prgmmon_array[0] == "2" && judge == true)//typecdがFALCONなら
                            {
                                upper = check_step[i] + 1;
                                lower = check_step[i] - 1;                                                                           
                            }
                            //FALCON55度監視以外の場合は±5度で監視
                            else
                            {
                                upper = check_step[i] + 5;
                                lower = check_step[i] - 5;
                            }

                            //現在のステップが監視する状態なら
                            if (check_step_started[i] == true) {
                                //MainFormのChartPlotメソッドに渡すデータの設定
                                cpea.upper = upper;
                                cpea.lower = lower;

                                //ステップ監視中に入って、FALCONかつ対象ステップなら
                                if(prgmmon_array[0] == "2" && judge == true)
                                {
                                    //10秒おきにタイマーで呼び出しているため 総合計時間を加算
                                    falcon_monitortime += 10;
                                }

                                //FALCON 狙い値に対して、1度より大きく2度以下で外れている回数をカウントする専用
                                //1度以上外れている仕様に変えるなら
                                //この条件は2度オーバーのみ抽出　temp <= lower
                                //if (((temp < lower && temp >= lower2) || (temp > upper && temp <= upper2)) && (prgmmon_array[0] == "2" && judge == true))
                                if (((temp <= lower && temp >= lower2) || (temp >= upper && temp <= upper2)) && (prgmmon_array[0] == "2" && judge == true))
                                {
                                    falcon_temp_NG_Count += 1;
                                }

                                //FALCON 55度専用　温度が±1℃以内か確認
                                if ((temp < lower || temp > upper) && (prgmmon_array[0] == "2" && judge == true))
                                {
                                    // 温度上下限超えイベント発生
                                    if (TempOver_falcon != null)
                                    {
                                        TempOver_falcon(this, EventArgs.Empty);
                                    }

                                }
                                //上記以外
                                else if (temp < lower || temp > upper)
                                {
                                    // 温度上下限超えイベント発生
                                    if (TempOver != null)
                                    {
                                        TempOver(this, EventArgs.Empty);
                                    }
                                }

                            }
                        }
                    }

                    //プロットイベントの発生
                    if (ChartPlot != null) {
                        ChartPlot(this, cpea);
                    }

                }
                catch (Exception e) {
                    //このメソッド専用のログファイルに書き込む
                    string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                    WriteAppend($"{templog_folder}\\check_temp_errorlog.txt", dt + "\t" + e.Message);

                }
            }
        }



    }

    /// <summary>
    /// スレッドタイマー使うためのクラス
    /// <para>引数オブジェクトを渡したいので使う</para>
    /// </summary>
    class TimerState {
        public string status;
        public string filepath;

    }
    /// <summary>
    /// チャートプロットイベント用クラス　引数オブジェクト渡したいので使う
    /// </summary>
    public class ChartPlotEventArgs : EventArgs {
        public DateTime date_time;
        public int temperature;
        public int? upper = null;
        public int? lower = null;
    }


    public class OvenCommandException : Exception {

        /// <summary>
        /// 引数無しのコンストラクタ
        /// </summary>
        public OvenCommandException() {
            //
        }

        /// <summary>
        /// メッセージ文字列を渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        public OvenCommandException(string message) : base(message) {
            // メッセージ文字列を渡すコンストラクタ
        }

        /// <summary>
        /// メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        /// </summary>
        /// <param name="message">メッセージ文字列</param>
        /// <param name="innerException">発生済み例外オブジェクト</param>
        public OvenCommandException(string message, Exception innerException) : base(message, innerException) {
            //メッセージ文字列と発生済み例外オブジェクトを渡すコンストラクタ
        }

    }
}
