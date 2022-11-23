using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

/// <summary>
/// 富士情報　新規作成　2022/5
/// 半完成品の最終工程
/// ラベルの照合で完了とする
/// この後製品に投入する
/// </summary>
namespace ArmsWeb.Models
{
    public class LotLabelCompareModel
    {
        public LotLabelCompareModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
        }

        /// <summary>
        /// 設備番号
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 作業者
        /// </summary>
        public string EmpCd { get; set; }

        public MachineInfo Mac { get; set; }

        /// <summary>
        /// マガジンNo
        /// </summary>
        public string MagNo { get; set; }

        /// <summary>
        /// ロットNo
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 判定結果
        /// </summary>
        public bool IsOK { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        /// 照合
        /// </summary>
        /// <returns></returns>
        public void Compare(Magazine mag)
        {
            IsOK = false;

            if (mag == null)
            {
                Msg = "照合NG：マガジンが見つかりません";
                return;
            }

            if (mag.NascaLotNO != LotNo)
            {
                Msg = $"照合NG：マガジンNoとロットNoが一致しません　ロットNoは({mag.NascaLotNO})です";
                return;
            }

            IsOK = true;
            Msg = "照合OK。";
        }

        /// <summary>
        /// 開始前チェックを実施。
        /// 実際の登録は行わない
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckBeforeStart(Magazine mag, out string msg)
        {
            return EndMag(mag, out msg, true);
        }

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool EndMag(Magazine mag, out string msg, bool isBeforeStartCheckOnly)
        {
            msg = "";
            try
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    msg = "ロット情報が存在しません：ロットNo『" + mag.NascaLotNO + "』";
                    return false;
                }

                //最終工程を取得
                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    msg = "工程情報が存在しません";
                    return false;
                }

                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);

                Order order = Order.GetMagazineOrder(mag.NascaLotNO, p.ProcNo);
                if (order == null || dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    order = new Order();
                    order.LotNo = mag.NascaLotNO;
                    order.ProcNo = p.ProcNo;
                    order.InMagazineNo = MagNo;
                }

                order.OutMagazineNo = MagNo;
                order.MacNo = this.Mac.MacNo;
                order.WorkStartDt = DateTime.Now;
                order.WorkEndDt = DateTime.Now;

                order.TranStartEmpCd = this.EmpCd;
                order.TranCompEmpCd = this.EmpCd;
                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }

                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }
                string errMsg;

                bool isStartError = WorkChecker.IsErrorBeforeStartWork(lot, Mac, order, p, out errMsg);

                if (dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    order.LotNo = Order.MagLotToNascaLot(order.LotNo);
                }

                if (isStartError)
                {
                    msg = errMsg;
                    return false;
                }

                if (isBeforeStartCheckOnly)
                    return true;

                bool isError = WorkChecker.IsErrorWorkComplete(order, Mac, lot, out errMsg);

                if (isError)
                {
                    msg = errMsg + " ロットは完了しましたが警告状態になっています。";
                    lot.IsWarning = true;
                    lot.Update();
                    order.Comment += errMsg;
                    order.DeleteInsert(order.LotNo);

                    mag.NowCompProcess = p.ProcNo;
                    mag.NewFg = false;
                    // インラインマガジンロット更新
                    mag = Magazine.ApplyMagazineInOut(order, order.LotNo);
                    Magazine.UpdateNewFgOff(MagNo);

                    return false;
                }
                else
                {
                    order.DeleteInsert(order.LotNo);

                    // インラインマガジンロット更新
                    mag = Magazine.ApplyMagazineInOut(order, order.LotNo);
                    Magazine.UpdateNewFgOff(MagNo);

                    //部材情報登録(バッチのIF処理では製品の投入に間に合わない場合があるのでとりあえず作って後でIFして更新する)
                    updateSQLite(mag, lot);

                    msg = "";

                    //Exporter exp = Exporter.GetInstance();
                    //NASCAResponse res = exp.SendAsmLotAllProc(order.LotNo);
                    //if (res.Status != NASCAStatus.OK)
                    //    msg = "ロットは完了しましたが4M連携は失敗しました。";

                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }

        /// <summary>
        /// 富士情報追加　半完成品ラベル照合対応　
        /// 部材情報に半完成品登録
        /// </summary>
        /// <param name="mat"></param>
        /// <param name="hmgp"></param>
        /// <param name="iswafer"></param>
        /// <param name="isbadmarkframe"></param>
        /// <param name="moldedNm"></param>
        /// <param name="moldedClass"></param>
        /// <param name="moldedDateList"></param>
        /// <param name="waferBlendCd"></param>
        /// <param name="waferWorkCdList"></param>
        /// <param name="waferSupplyId"></param>
        private void updateSQLite(Magazine mag, AsmLot lot)
        {
            using (SqlConnection con = new SqlConnection(ArmsApi.Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {

                con.Open();

                cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar).Value = lot.TypeCd;
                cmd.Parameters.Add("@LOTLOTNO", SqlDbType.NVarChar).Value = lot.NascaLotNo;
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lot.NascaLotNo + "-1-1";
                //4MからのIFが確実に反映されるように1時間前
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-1);
                cmd.Parameters.Add("@LIMITDT", SqlDbType.DateTime).Value = System.DBNull.Value;
                cmd.Parameters.Add("@BINCD", SqlDbType.NVarChar).Value = ArmsApi.Config.Settings.MANU_SGYUK_CD;
                cmd.Parameters.Add("@MATNM", SqlDbType.NVarChar).Value = "";
                cmd.Parameters.Add("@HMGP", SqlDbType.NVarChar).Value = "MATE999";
                cmd.Parameters.Add("@ISFRAME", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@ISWAFER", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@ISBADMARKFRAME", SqlDbType.Int).Value = 0;
                cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = "";
                cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = POPSgyubsh.GetPOPSgyubsh(lot.TypeCd);
                cmd.Parameters.Add("@SUPPLYID", SqlDbType.NVarChar).Value = "";
                cmd.Parameters.Add("@DICEWAFERKB", SqlDbType.NVarChar).Value = 0;
                cmd.Parameters.Add("@FRAMEQTY", SqlDbType.Decimal).Value = mag.FrameQty;
                cmd.Parameters.Add("@USRCD", SqlDbType.Decimal).Value = this.EmpCd.PadRight(8).Substring(0,8).Trim();

                cmd.CommandText = @"
                    SELECT lastupddt FROM TnMaterials
                    WHERE materialcd=@MATCD AND lotno=@LOTNO";

                object objlastupd = cmd.ExecuteScalar();

                if (objlastupd == null)
                {
                    cmd.CommandText = @"
                        INSERT INTO TnMaterials(materialcd, lotno, materialnm, limitdt, bincd, hmgroup, iswafer, isframe, delfg, lastupddt, isbadmarkframe, blendcd, workcd, supplyId, dicewaferkb, stockct,stocklastupddt,updusercd)
                        SELECT @MATCD, @LOTNO, @MATNM, @LIMITDT, @BINCD, @HMGP, @ISWAFER, @ISFRAME,  0, @UPDDT, @ISBADMARKFRAME, @BLENDCD, @WORKCD, @SUPPLYID, @DICEWAFERKB, 
                        @FRAMEQTY * CASE WHEN ISNULL(limitsheartestfg,1)=0 THEN 1 ELSE limitsheartestfg END, @UPDDT, @USRCD
                        FROM TnLot WHERE lotno = @LOTLOTNO;";

                    cmd.ExecuteNonQuery();
                    return;
                }
            }
        }
    }
}