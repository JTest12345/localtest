//<--後工程合理化/エラー集計
using EICS.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


namespace EICS.Machine
{
    class STMachineInfo2 : MachineBase
    {
        /// <summary>選別機(ｴﾗｰ集計</summary>
        //public const string ASSETS_NM = "選別機ｴﾗｰ集計";

        protected virtual string POSTPROCESSEXTENSION() { return "DAT.log$"; }

        /* コードレビューで重欠点:単ロットが取れない
        protected virtual string SQL() { return @"SELECT DISTINCT dbo.NttSSHJ.Lot_NO,dbo.RvtTRANH.start_dt,dbo.RvtTRANH.complt_dt
                                                    FROM dbo.NttSSHJ with(nolock) INNER JOIN
                                                            dbo.RvtTRANH with(nolock) ON dbo.NttSSHJ.MnfctInst_NO = dbo.RvtTRANH.mnfctinst_no INNER JOIN
                                                            dbo.RvtTRANCOMPLT with(nolock) ON dbo.RvtTRANH.mnfctrsl_no = dbo.RvtTRANCOMPLT.mnfctrsl_no
                                                    WHERE (dbo.NttSSHJ.Lot_NO = @lotno) AND
                                                            (dbo.NttSSHJ.Del_FG = 0) AND
                                                            (dbo.RvtTRANH.del_fg = '0') AND
                                                            (dbo.RvtTRANCOMPLT.del_fg = '0')"; }
        */

        protected virtual string SQL() { return @"SELECT DISTINCT dbo.NttSSHJ.Lot_NO, dbo.RvtTRANH.start_dt, dbo.RvtTRANH.complt_dt, dbo.RvtORDH.material_cd
                                                    FROM dbo.NttSSHJ WITH (nolock) INNER JOIN
                                                            dbo.RvtTRANH WITH (nolock) ON dbo.NttSSHJ.MnfctInst_NO = dbo.RvtTRANH.mnfctinst_no INNER JOIN
                                                            dbo.RvtTRANCOMPLT WITH (nolock) ON dbo.RvtTRANH.mnfctrsl_no = dbo.RvtTRANCOMPLT.mnfctrsl_no INNER JOIN
                                                            dbo.RvtORDH ON dbo.NttSSHJ.MnfctInst_NO = dbo.RvtORDH.mnfctinst_no
                                                    WHERE (dbo.NttSSHJ.Lot_NO = @lotno) AND (dbo.NttSSHJ.Del_FG = 0) AND (dbo.RvtTRANH.del_fg = '0') AND (dbo.RvtTRANCOMPLT.del_fg = '0') AND 
                                                            (dbo.RvtORDH.del_fg = '0') AND (dbo.RvtORDH.material_cd LIKE '%.ST%')"; }
        //指図を選別工程に限っていない為、SQL修正必要。
        //選別のみの問題。
        //WhereにRvtORDH.Materialcd like '%.ST%'の条件が必要。→
        //WhereにRvtORDH.Processcdを指定。この場合、部署によってcdが違う為、QCIL.xmlに持つなど必要。

        public const string INPUT = "input";    //処理フォルダ
        public const string DONE = "done";      //処理済フォルダ
        public const string NONE = "none";      //未処理フォルダ(NASCAに情報ないロット、14日以内のファイルでない)
        public const int TARGETBEFOREDAY = -14; //過去2週間分：鈴木さんOK
        public const int ERRCOLUMN = 3;         //0始まりの4列目(固定)に装置エラー番号が入るのでそこを集計対象とする。：鈴木さんOK


        public STMachineInfo2(LSETInfo lsetInfo)
        {
        }

        ~STMachineInfo2()
        {

        }

        protected virtual NascaTranInfo GetNascaTranInfo(int linecd, string sql, string lotno)
        {
            var nascatraninfo = new NascaTranInfo();
            nascatraninfo = ConnectDB.GetNascaTran(this.LineCD, SQL(), lotno);
            return nascatraninfo;
        }

        /// <summary>
        /// ファイルチェック
        /// </summary>
        /// <param name="lsetInfo">装置情報</param>
        /// <returns>装置処理状態ステータス</returns>
        public override void CheckFile(LSETInfo lsetInfo)
        {
            SettingInfo SettingInfo = SettingInfo.GetSingleton();

            //エラーファイルをinputフォルダにコピーする
            ErrFileCopy(lsetInfo);

            //※ロットと判断済みのファイルのみがinputフォルダに入っている。未完ロットファイル(SQLで判定)は入らない。
            //input(処理対象)フォルダが監視フォルダなので、ここを監視
            List<FileInfo> ListFileInfo = GetListFileInfo(lsetInfo.InputFolderNM + "\\" + INPUT);

            //ファイル更新日時を見て14日以内のファイルを処理対象とする。
            List<FileInfo> ListFileInfoTarget = new List<FileInfo>();
            foreach (FileInfo fileinfo in ListFileInfo)
            {
                if (fileinfo.LastWriteTime >= DateTime.Now.AddDays(TARGETBEFOREDAY))
                {
                    ListFileInfoTarget.Add(fileinfo);
                }
                else
                {//14日以内のファイルでなければ、noneフォルダへ移動
                    File.Move(fileinfo.FullName, lsetInfo.InputFolderNM + "\\" + NONE + "\\" + fileinfo.Name);
                }
            }

            //エラー集計するターゲットファイルリスト
            foreach (FileInfo fileinfo in ListFileInfoTarget)
            {
                string lotno = fileinfo.Name.Split('.')[0];

                var nascatraninfo = new NascaTranInfo();
                nascatraninfo = GetNascaTranInfo(this.LineCD, SQL(), lotno);

                //一旦inputフォルダに入った後(入ったときはロットが1つだけHitした)
                //*時間にEICSを再動作させてロットが2つ以上Hitした場合 → noneフォルダへファイル移動
                if (string.IsNullOrEmpty(nascatraninfo.lotno) == true)
                {
                    File.Move(fileinfo.FullName, lsetInfo.InputFolderNM + "\\" + NONE + "\\" + fileinfo.Name);
                    continue;
                }

                //ファイル内のNGを集計して、TnLogにDel/Insして、処理済フォルダ[done]へ移動。
                //ファイル読み込み
                IEnumerable<string> lines = File.ReadLines(fileinfo.FullName);

                //ファイル内の装置エラーNoのリスト作成
                List<int> errlist = new List<int>();
                foreach (string line in lines)
                {
                    var values = line.Split(',');
                    int result;
                    if (int.TryParse(values[ERRCOLUMN].Trim(), out result) == true)
                    {
                        errlist.Add(result);
                    }
                }

                //管理項目(TmPLM)毎にエラー個数登録
                List<Plm> plmList = Plm.GetData(this.LineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, lsetInfo.ChipNM);
                foreach (Plm plm in plmList)
                {
                    int errcnt = 0;
                    int errno = ConnectDB.GetFileErrCntInfo(this.LineCD, lsetInfo.ModelNM, plm.QcParamNO);
                    errcnt=errlist.Where(x => x == errno).Count();

                    //TnLogへ記録
                    List<ErrMessageInfo> errMessageList = new List<ErrMessageInfo>();
                    MagInfo magInfo = new MagInfo();
                    magInfo.sNascaLotNO = nascatraninfo.lotno;
                    magInfo.sMagazineNO = "";
                    magInfo.sMaterialCD = lsetInfo.TypeCD;

                    //2.エラー集計(EICSエラー出力なし)
                    ConnectDB.InsertTnLOG_NotOutputErr(lsetInfo, plm, magInfo, Convert.ToString(errcnt), Convert.ToString(nascatraninfo.compltdt));

                    //高負荷対策
                    Thread.Sleep(250);  //処理時間：1ファイル最大4.5秒程度(plm200程度のループ。22ms毎に250msのsleepを入れる。1ループ約300ms)
                                        //高負荷対策：1ファイル1分程度(250msスリープあり/管理項目毎) ⇔ 4.5秒(スリープなし)
                                        //            [全項目エラーの場合]約1分で処理完了/ファイル
                                        //                                  Tmplm 1ループ当たり約300ms(22ms+250msのSleep)×管理項目約200回(plm)=60000(=1分)。
                                        //            [10項目エラーの場合]約7秒で処理完了/ファイル
                                        //                                  Tmplm 1ループ当たり約22ms×管理項目約190回(plm)=4180(=4秒) + 
                                        //                                  Tmplm 1ループ当たり約300ms(22ms+250msのSleep)×管理項目約10回(plm)=3000(=3秒)。
                }

                //GEICS表示用→マスタのMAXは0 → 9999に変更。
                //CheckQC(lsetInfo, 200, lsetInfo.TypeCD);//200:後工程 約8秒

                //処理したので、doneファイルへ移動
                if (System.IO.File.Exists(lsetInfo.InputFolderNM + "\\" + DONE + "\\" + fileinfo.Name))
                { 
                    File.Delete(lsetInfo.InputFolderNM + "\\" + DONE + "\\" + fileinfo.Name);//doneフォルダに既にある場合は削除してから、move。
                }
                File.Move(fileinfo.FullName, lsetInfo.InputFolderNM + "\\" + DONE + "\\" + fileinfo.Name);

            }

            //監視周期用(処理フォルダを一旦空にしたら、一定時間(現状では30分)スリープ。)
            Thread.Sleep(SettingInfo.PostProcessMonitoringCycleSec * 1000);
        }

        private void ErrFileCopy(LSETInfo lsetInfo)
        {
            //コピー元ファイルリスト=TmLSET.InputFolderにある所定ファイルのリスト作成
            List<FileInfo> ListFileInfoFrom = GetListFileInfo(lsetInfo.InputFolderNM);

            //コピー先フォルダの確認。なければフォルダ作成
            if (System.IO.Directory.Exists(lsetInfo.InputFolderNM + "\\" + INPUT) == false)
            {
                System.IO.Directory.CreateDirectory(lsetInfo.InputFolderNM + "\\" + INPUT);
            }
            if (System.IO.Directory.Exists(lsetInfo.InputFolderNM + "\\" + DONE) == false)
            {
                System.IO.Directory.CreateDirectory(lsetInfo.InputFolderNM + "\\" + DONE);
            }
            if (System.IO.Directory.Exists(lsetInfo.InputFolderNM + "\\" + NONE) == false)
            {
                System.IO.Directory.CreateDirectory(lsetInfo.InputFolderNM + "\\" + NONE);
            }

            //コピー先ファイルリスト=input(処理対象)/done(処理済)/none(対象外=ロットファイルでは無い)全てのファイルリスト
            List<FileInfo> ListFileInfoTo = GetListFileInfo(lsetInfo.InputFolderNM + "\\" + INPUT);
            ListFileInfoTo.AddRange(GetListFileInfo(lsetInfo.InputFolderNM + "\\" + DONE));
            ListFileInfoTo.AddRange(GetListFileInfo(lsetInfo.InputFolderNM + "\\" + NONE));

            //コピー先にあるか確認して、無ければコピー対象のリストに入れる
            List<FileInfo> ListFileInfoCopy = new List<FileInfo>();

            foreach (FileInfo fileinfo in ListFileInfoFrom)
            {
                //コピー先(input/done/none)に無ければ、inputフォルダへコピーする対象とする。
                if (ListFileInfoTo.Exists(f => f.Name == fileinfo.Name) == false)
                {
                    string lotno = Path.GetFileNameWithoutExtension(fileinfo.FullName).Split('.')[0];
                    var nascatraninfo = new NascaTranInfo();
                    if (string.IsNullOrWhiteSpace(lotno) == true)
                    {
                        continue;
                    }

                    //NASCAに情報ない又は、ロットが2つ以上Hitした場合 → noneフォルダへファイル移動
                    //nascatraninfo = ConnectDB.GetNascaTran(this.LineCD, SQL(), lotno);
                    nascatraninfo = GetNascaTranInfo(this.LineCD, SQL(), lotno);
                    if (string.IsNullOrWhiteSpace(nascatraninfo.lotno) == true)
                    {
                        //[ロットファイルでは無いとみなしたファイル]と
                        //[いずれロットファイルになるが、まだSQLに引っかからないファイル]はinputフォルダに移動しない。
                        // →ファイルが完成する(7時間程度書き続けるファイルあり)前にinputフォルダへ移動してしまった不具合対応
                        continue;
                    }

                    ListFileInfoCopy.Add(fileinfo);
                }
            }

            //対象をinput(処理対象)へコピー
            foreach (FileInfo fileinfo in ListFileInfoCopy)
            {
                File.Copy(fileinfo.FullName, lsetInfo.InputFolderNM + "\\" + INPUT + "\\" + fileinfo.Name);
            }
        }

        private List<FileInfo> GetListFileInfo(string dir)
        {
            List<FileInfo> ListFileInfo = new List<FileInfo>();

            //現状：ファイル名のどこにあっても○→ファイル名の終端に来る場合のみに修正。
            foreach (string swfname in Common.GetFiles(dir, POSTPROCESSEXTENSION()))
            {
                FileInfo fileInfo = new FileInfo(swfname);
                ListFileInfo.Add(fileInfo);
            }
            return ListFileInfo;
        }


    }
}
//-->後工程合理化/エラー集計