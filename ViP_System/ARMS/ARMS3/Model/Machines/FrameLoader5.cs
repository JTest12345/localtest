using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;
using System.IO;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// NTSV用の搭載機(大隆製)。フレーム搭載時にプロファイルのBOM情報とフレーム(基板DM)を照合。
    /// 1マガジン完成時に、新規アッセンロットを作成。
    /// </summary>
    class FrameLoader5 : MachineBase
    {
        #region PLCアドレス定義

        private const string START_REQ_BIT_ADDR = "B0010";
        private const string START_OK_BIT_ADDR = "B2010";
        private const string START_NG_BIT_ADDR = "B2011";
        private const string COMPLETE_REQ_BIT_ADDR = "B0000";
        private const string COMPLETE_OK_BIT_ADDR = "B2000";
        private const string COMPLETE_NG_BIT_ADDR = "B0021";

        private const string START_MAGNO_WORD_ADDR = "W4000";
        private const string COMPLETE_MAGNO_WORD_ADDR = "W1000";

        private const string DATAMATRIX_WORD_ADDR = "W3000";
        private const string MOUNT_DATAMATRIX_WORD_ADDR_START = "W40C8";
        private const string COMPLETE_DATAMATRIX_WORD_ADDR_START = "W10C8";

        private const int FRAME_LOTNO_LENGTH = 13;

        #endregion

        #region 定数

        private const int MAGNO_WORD_LENGTH = 20;
        private const int DATAMATRIX_WORD_LENGTH = 20;
        private const int DATAMATRIX_MAXCT = 50;
        private const string DUMMY_LOT_START_WORD = "DUMMY";

        #endregion      

        protected override void concreteThreadWork()
        {
            if (IsCompleteRequire())
            {
                this.WorkComplete();
            }

            if (IsStartRequire())
            {
                this.WorkStart();
            }
        }

        private bool IsStartRequire()
        {
            // 収納許可問合わせ送信確認
            string retv = Plc.GetBit(START_REQ_BIT_ADDR);
            if (retv == PLC.Common.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsCompleteRequire()
        {
            // 収納許可問合わせ送信確認
            string retv = Plc.GetBit(COMPLETE_REQ_BIT_ADDR);
            if (retv == PLC.Common.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        private void WorkStart()
        {
            OutputSysLog($"[開始処理] {this.Name} 開始");

            try
            {
                string mountMagno = this.Plc.GetString(START_MAGNO_WORD_ADDR, MAGNO_WORD_LENGTH, true).Replace("\r", "");
                if (string.IsNullOrWhiteSpace(mountMagno) == true)
                {
                    throw new ApplicationException($"装置から収納予定マガジンNoの取得に失敗。取得アドレス：『{START_MAGNO_WORD_ADDR}』,取得アドレス数：『{MAGNO_WORD_LENGTH}』");
                }

                //GetMagazineがBigEndianに対応していないためGetStringで代用しているので、本コード内で抜取。GetMagazineが改修できれば以降の4行は不要。
                string[] tempMag = mountMagno.Split(' ');
                if (tempMag.Length >= 2)
                {
                    mountMagno = tempMag[1];
                }


                OutputSysLog($"[開始処理] {this.Name} 収納予定マガジンNoの取得：『{mountMagno}』");

                if (mountMagno.StartsWith(DUMMY_LOT_START_WORD) == true)
                {
                    Plc.SetBit(START_OK_BIT_ADDR, 1, PLC.Common.BIT_ON);
                    OutputSysLog($"[開始処理] {this.Name} ダミーマガジンの為、開始チェックスキップ。装置にOK信号を送信。収納予定マガジンNo：『{mountMagno}』");
                    return;
                }

                string frameDM = this.Plc.GetString(DATAMATRIX_WORD_ADDR, DATAMATRIX_WORD_LENGTH, true).Replace("\r", "");
                if (string.IsNullOrWhiteSpace(frameDM) == true)
                {
                    throw new ApplicationException($"装置から投入基板DMの取得に失敗。取得アドレス：『{DATAMATRIX_WORD_ADDR}』,取得アドレス数：『{DATAMATRIX_WORD_LENGTH}』");
                }
                OutputSysLog($"[開始処理] {this.Name} 投入基板DMの取得：『{frameDM}』");

                string errMsg;
                bool isInputOK = CheckCanInputFrame(frameDM, out errMsg);
                if (isInputOK == false)
                {
                    throw new ApplicationException($"投入照合エラー。基板DM『{frameDM}』,理由：『{errMsg}』");
                }
                Plc.SetBit(START_OK_BIT_ADDR, 1, PLC.Common.BIT_ON);
                OutputSysLog($"[開始処理] {this.Name} 完了。装置にOK信号を送信");
            }
            catch (ApplicationException ex)
            {
                Plc.SetBit(START_NG_BIT_ADDR, 1, PLC.Common.BIT_ON);
                throw new ApplicationException($"[開始処理異常] {this.Name} 装置にNG信号を送信。異常理由：{ex.Message}");
            }

            catch (Exception ex)
            {
                Plc.SetBit(START_NG_BIT_ADDR, 1, PLC.Common.BIT_ON);
                throw new Exception($"[開始処理異常] {this.Name} 装置にNG信号を送信。異常理由：{ex.ToString()}");
            }
        }

        private void WorkComplete()
        {
            try
            {
                string magno = this.Plc.GetString(COMPLETE_MAGNO_WORD_ADDR, MAGNO_WORD_LENGTH, true).Replace("\r", "");
                if (string.IsNullOrWhiteSpace(magno) == true)
                {
                    throw new ApplicationException($"装置から収納マガジンQRの取得に失敗。取得アドレス：『{COMPLETE_MAGNO_WORD_ADDR}』,取得アドレス数：『{MAGNO_WORD_LENGTH}』");
                }
                OutputSysLog($"[完了処理] {this.Name} 収納マガジンの取得：『{magno}』");

                //GetMagazineがBigEndianに対応していないためGetStringで代用しているので、本コード内で抜取。GetMagazineが改修できれば以降の4行は不要。
                string[] tempMag = magno.Split(' ');
                if (tempMag.Length >= 2)
                {
                    magno = tempMag[1];
                }

                if (magno.StartsWith(DUMMY_LOT_START_WORD) == true)
                {
                    Plc.SetBit(COMPLETE_OK_BIT_ADDR, 1, PLC.Common.BIT_ON);
                    OutputSysLog($"[完了処理] {this.Name} ダミーマガジンの為、完了登録スキップ。装置にOK信号を送信。収納マガジンNo：『{magno}』");
                    return;
                }

                Magazine svrMag = Magazine.GetCurrent(magno);
                if (svrMag != null)
                {
                    throw new ApplicationException($"収納マガジンには稼働中ロットが割り当たっています。ロットNo：『{svrMag.NascaLotNO}』,マガジンNo:『{magno}』");
                }

                // -------------------------------------- 指図発行・実績登録 -----------------------------------------------------
                VirtualMag newMagazine = new VirtualMag();
                newMagazine.LastMagazineNo = magno;
                newMagazine.MagazineNo = magno;
                newMagazine.MacNo = this.MacNo;
                newMagazine.ProcNo = MachineInfo.GetProcNo(this.MacNo);
                //作業開始完了時間取得
                try
                {
                    newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                }
                catch
                {
                    throw new ApplicationException("作業完了時間取得失敗。取得アドレス：『{this.WorkCompleteTimeAddress}』");
                }
                try
                {
                    newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                }
                catch
                {
                    throw new ApplicationException("作業完了時間取得失敗。取得アドレス：『{this.WorkCompleteTimeAddress}』");
                }

                // 収納基板情報取得
                Dictionary<string, string> frameDataList = GetFrameList(COMPLETE_DATAMATRIX_WORD_ADDR_START, DATAMATRIX_WORD_LENGTH, DATAMATRIX_MAXCT);
                if (frameDataList.Count() == 0)
                {
                    throw new ApplicationException($"装置から収納基板情報リストが取得できません。取得アドレス(開始)：『{COMPLETE_DATAMATRIX_WORD_ADDR_START}』");
                }
                List<string> frameDMList = new List<string>(frameDataList.Keys);

                //ロット一覧を取得＆数量チェック
                List<Material> matLotList = CheckFrameStockCt(frameDMList);

                newMagazine.RelatedMaterials = matLotList;

                               
                this.WorkComplete(newMagazine, this, true);

                // -------------------------------------- マッピングファイル移動 & トリガファイル生成 --------------------------------------
                svrMag = Magazine.GetCurrent(magno);
                if (svrMag == null)
                {
                    throw new ApplicationException($"稼働中ロットが存在しません。マガジンNo:『{magno}』");
                }
                AsmLot asmLot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                int procNo = ArmsApi.Model.Process.GetFirstProcess(asmLot.TypeCd).ProcNo;
                string destPath = this.LogOutputDirectoryPath;

                foreach (string frameDM in frameDMList)
                {
                    string trgFileName = frameDM + "_" + asmLot.NascaLotNo + "_" + asmLot.TypeCd + "_" + procNo + "_" + asmLot.NascaLotNo + ".fin";
                    File.Copy(Path.Combine(Config.Settings.MappingDirectoryPath, frameDM + ".mpd"), Path.Combine(destPath, frameDM + ".mpd"), true);
                    File.Create(Path.Combine(destPath, trgFileName)).Close();
                }

                Plc.SetBit(COMPLETE_OK_BIT_ADDR, 1, PLC.Common.BIT_ON);
                OutputSysLog($"[完了処理] {this.Name} 完了。装置にOK信号を送信");

                //TnLotCarrier登録&前データがある場合フラグOFF
                foreach (string frameDM in frameDMList)
                {
                    ArmsApi.Model.LotCarrier oldLotCarrier = ArmsApi.Model.LotCarrier.GetData(frameDM, true, false);
                    if (oldLotCarrier != null && oldLotCarrier.LotNo != asmLot.NascaLotNo)
                    {
                        ArmsApi.Model.LotCarrier.UpdateOperateFg(oldLotCarrier.LotNo, oldLotCarrier.CarrierNo, false);
                    }
                }
                LotCarrier lotcarrier = new LotCarrier(asmLot.NascaLotNo, frameDMList, "660");
                lotcarrier.DeleteInsert();

                //基板段数情報を登録
                RegisterFrameStep(frameDataList, svrMag, procNo);

            }
            catch (ApplicationException ex)
            {
                Plc.SetBit(COMPLETE_NG_BIT_ADDR, 1, PLC.Common.BIT_ON);
                throw new ApplicationException($"[完了処理異常] {this.Name} 装置にNG信号を送信。異常理由：{ex.Message}");
            }
            catch (Exception ex)
            {
                Plc.SetBit(COMPLETE_NG_BIT_ADDR, 1, PLC.Common.BIT_ON);
                throw new Exception($"[完了処理異常] {this.Name} 装置にNG信号を送信。異常理由：{ex.ToString()}");
            }
        }

        /// <summary>
        /// 基板DMを投入予定マガジンに積み込めるかチェック
        /// </summary>
        /// <param name="mountFrameDM"></param>
        /// <returns></returns>
        private bool CheckCanInputFrame(string loadingFrameDM, out string msg)
        {
            msg = string.Empty;

            // 基板情報の取得 (先頭13文字がlotno)
            string matLotNo = GetLotNoFromFrameDm(loadingFrameDM);

            // 判定①： 在庫チェック
            Material[] matInfoList = Material.GetMaterials(matLotNo, false); //在庫チェック機能の有効性が不明(送付データが基本ズレないため)なので一旦除外(2018.1.31)
            if (matInfoList.Length == 0)
            {
                msg = $"基板在庫が存在しません。基板LotNo：『{matLotNo}』";
                return false;
            }
            else if (matInfoList.Length > 1)
            {
                msg = $"基板在庫が複数レコード存在します。基板LotNo：『{matLotNo}』";
                return false;
            }
            Material matInfo = matInfoList[0];

            // 判定②： 割り付きプロファイル情報のBOMチェック
            Profile prof = Profile.GetCurrentDBProfile(this.MacNo);
            if (prof == null)
            {
                msg = $"装置にプロファイルが割り付いていません。MacNo：『{this.MacNo}』";
                return false;
            }
            BOM[] boms = Profile.GetBOM(prof.ProfileId);
            if (!Array.Exists(boms, b => b.MaterialCd == matInfo.MaterialCd))
            {
                msg = $"プロファイルのBOM情報に存在しない基板品目です。プロファイルID：『{prof.ProfileId}』,基板品目：『{matInfo.MaterialCd}』";
                return false;
            }

            // 判定③： 搭載済みの基板と品目が一致しない。(品目混載チェック)
            Dictionary<string, string> mountFrameDataList = GetFrameList(MOUNT_DATAMATRIX_WORD_ADDR_START, DATAMATRIX_WORD_LENGTH, DATAMATRIX_MAXCT);
            // SGA2(三橋様)との相談との結果、混載チェックが不要となった為、規制解除
            //if (mountFrameDataList.Count > 0)
            //{
            //    List<string> mountFrameDmList = new List<string>(mountFrameDataList.Keys);
            //    string mountMatLotNo = GetLotNoFromFrameDm(mountFrameDmList.First());
            //    //リスト取得関数(GetFrameList)で事前に在庫存在チェックはしているので特に気にせずFirstを使う。
            //    Material mountMatInfo = Material.GetMaterials(mountMatLotNo, false).First();
            //    if (matInfo.MaterialCd != mountMatInfo.MaterialCd)
            //    {
            //        msg = $"搭載済み基板DMと投入基板DMとで品目が一致しません。搭載済み基板品目：『{mountMatInfo.MaterialCd}』,投入基板品目：『{matInfo.MaterialCd}』";
            //        return false;
            //    }
            //}

            // 判定④： 基板DMのメーカーマッピングファイルが存在する
            string MappingFilePath = Path.Combine(Config.Settings.MappingDirectoryPath, loadingFrameDM + ".mpd");
            if (File.Exists(MappingFilePath) == false)
            {
                msg = $"投入基板DMのマッピングデータが存在しません。パス:『{MappingFilePath}』";
                return false;
            }

            // 判定⑤： 搭載機に割り付いているプロファイルの発行ロット数より、発行済ロット数が少ない。
            int profileLotCt = Profile.GetOrderCountFromProfileName(prof.ProfileNm);
            AsmLot[] lots = AsmLot.SearchAsmLot(prof.ProfileId);
            if (lots.Length >= profileLotCt)
            {
                msg = $"割り付けプロファイルの発行済みロット数が発行ロット数の上限を超えてしまいます。プロファイル名：『{prof.ProfileNm}』,発行済みロット数：『{lots.Length.ToString()}』";
                return false;
            }

            return true;
        }

        //private List<Material> GetFrameList(string startAddr, int distance, int MaxCt)
        //{
        //    List<Material> retv = new List<Material>();

        //    for (int i = 0; i < MaxCt; i++)
        //    {
        //        string dmAddress = PLC.Common.GetHexAddressAddDecNum(startAddr, "W", distance * i);
        //        string frameDm = Plc.GetString(dmAddress, DATAMATRIX_WORD_LENGTH, true);

        //        if (string.IsNullOrWhiteSpace(frameDm) == false)
        //        {
        //            string matLotNo = frameDm.Substring(0, 10);
        //            Material[] matInfoList = Material.GetMaterials(matLotNo, true);
        //            if (matInfoList.Length == 0)
        //            {
        //                throw new ApplicationException($"収納基板在庫のレコードが存在しません。収納段数{(i+1).ToString("00")},収納基板コード：『{frameDm}』");
        //            }
        //            else if (matInfoList.Length > 1)
        //            {
        //                throw new ApplicationException($"収納基板在庫のレコードが複数存在します。収納段数{(i + 1).ToString("00")},収納基板コード：『{frameDm}』");
        //            }
        //            retv.Add(matInfoList[0]);
        //        }
        //    }

        //    return retv;
        //}

        /// <summary>
        /// 基板DMの先頭13文字から基板LotNoを取得する
        /// </summary>
        /// <param name="frameDM"></param>
        /// <returns></returns>
        private string GetLotNoFromFrameDm(string frameDM)
        {
            return frameDM.Substring(0, FRAME_LOTNO_LENGTH);
        }

        private Dictionary<string, string> GetFrameList(string startAddr, int distance, int MaxCt)
        {
            Dictionary<string, string> retv = new Dictionary<string, string>();

            for (int i = 0; i < MaxCt; i++)
            {
                string dmAddress = PLC.Common.GetHexAddressAddDecNum(startAddr, "W", distance * i);
                string frameDm = Plc.GetString(dmAddress, DATAMATRIX_WORD_LENGTH, true);

                if (string.IsNullOrWhiteSpace(frameDm) == false)
                {
                    string matLotNo = GetLotNoFromFrameDm(frameDm);
                    Material[] matInfoList = Material.GetMaterials(matLotNo, false); //在庫チェック機能の有効性が不明(送付データが基本ズレないため)なので一旦除外(2018.1.31)
                    if (matInfoList.Length == 0)
                    {
                        throw new ApplicationException($"収納基板在庫のレコードが存在しません。収納段数{(i + 1).ToString("00")},収納基板コード：『{frameDm}』");
                    }
                    else if (matInfoList.Length > 1)
                    {
                        throw new ApplicationException($"収納基板在庫のレコードが複数存在します。収納段数{(i + 1).ToString("00")},収納基板コード：『{frameDm}』");
                    }
                    retv.Add(frameDm.Replace("\r", ""), (i + 1).ToString());
                }
            }

            return retv;
        }


        /// <summary>
        /// データマトリクスから資材ロットの一覧を生成
        /// </summary>
        /// <param name="frameDmList"></param>
        /// <returns></returns>
        private List<Material> CheckFrameStockCt(List<String> frameDmList)
        {
            Dictionary<string, int> sumLot = new Dictionary<string, int>();
            List<Material> retv = new List<Material>();

            foreach (string dm in frameDmList)
            {
                if (sumLot.ContainsKey(GetLotNoFromFrameDm(dm)) == true)
                {
                    sumLot[GetLotNoFromFrameDm(dm)] += 1;
                }
                else
                {
                    sumLot.Add(GetLotNoFromFrameDm(dm), 1);
                }
            }

            foreach (KeyValuePair<string, int> frame in sumLot)
            {
                //リスト取得関数で事前に在庫存在チェックはしているので特に気にせずFirstを使う。
                //在庫チェック機能は有効性が不明(送付データが基本ズレない、ズレた時に現場でリカバリが難しい)なので一旦除外(2018.1.31)
                Material matstock = Material.GetMaterials(frame.Key, false).First();
                //if (matstock.StockCt < frame.Value)
                //{
                //    throw new ApplicationException($"フレームの在庫が不足しています。フレームロット:『{frame.Key}』, 搭載数:『{frame.Value}』, 在庫数:『{matstock.StockCt}』");
                //}
                //else
                //{
                    matstock.InputCt = frame.Value;
                    retv.Add(matstock);
                //}
            }

            return retv;
        }

        private void RegisterFrameStep (Dictionary<string, string> frameDataList, Magazine mag, int procno)
        {
            CarrireWorkData regData = new CarrireWorkData();
            regData.LotNo = mag.NascaLotNO;
            regData.ProcNo = procno;
            regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
            foreach(KeyValuePair<string, string> data in frameDataList)
            {
                regData.CarrierNo = data.Key;
                regData.Value = data.Value;
                regData.InsertUpdate();
            }
        }
    }
}
