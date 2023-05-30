using ArmsApi.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using KodaClassLibrary;
using CejDataAccessCom;

namespace KodaWeb.Models.WebApi {

    public class ApiLotInfo : LotInfo {

        /// <summary>
        /// 18桁ロット番号からロット情報を取得する
        /// <para>ArmsApi、富士情報Apiを使う</para>
        /// </summary>
        /// <param name="lotno18">新システムで使用している18桁ロット番号</param>
        /// <returns></returns>
        /// <exception cref="FjhSystemException">富士情報DLLエラー</exception>
        /// <exception cref="ArmsSystemException">ArmsApiDLLエラー</exception>
        public static ApiLotInfo Get_LotInfo_from_LotNo18(string lotno18) {

            var lotinfo = new ApiLotInfo() { LotnNo18 = lotno18 };

            //最初に実績ロット確認
            LotNoDll.LotInfo scm_lot;
            try {
                scm_lot = UseFjhApi.Get_LotInfo_from_LotNo18(new List<string> { lotinfo.LotnNo18 })[0];
            }
            catch (Exception ex) {
                throw new FjhSystemException(ex.Message);
            }

            if (scm_lot.IsOK) {
                lotinfo.ProductName = scm_lot.TypeCd;
                lotinfo.LotnNo10 = scm_lot.JitLotNo;
                lotinfo.VLotNo = scm_lot.VLotNo;
            }
            else {
                //新システムに対象ロットが無ければ終了(18桁のロット番号で問い合わせているのでロット番号が違うと思われる)
                throw new FjhSystemException("対象のロットはシステムに存在しません。ロット番号を確認して下さい。");
            }


            try {
                //ARMSで最終完了工程を確認
                var pre_proc = UseArmsApi.Get_LastProcess(lotinfo.LotnNo18);
                lotinfo.PreProcess = new Process { ProcessName = pre_proc.InlineProNM, ProcessNo = pre_proc.ProcNo, ProcessCode = pre_proc.WorkCd };

                //ARMSで次工程を確認
                var next_proc = UseArmsApi.Get_NextProcess(lotinfo.LotnNo18, pre_proc.ProcNo);
                if (next_proc == null) {
                    lotinfo.NextProcess = null;
                }
                else {
                    lotinfo.NextProcess = new Process { ProcessName = next_proc.InlineProNM, ProcessNo = next_proc.ProcNo, ProcessCode = next_proc.WorkCd };
                }
                //ARMSで次工程を開始済みか確認
                var now_proc = UseArmsApi.Get_NowProcess(lotinfo.LotnNo18);
                if (now_proc == null) {
                    lotinfo.NowProcess = null;
                }
                else {
                    lotinfo.NowProcess = new Process { ProcessName = now_proc.InlineProNM, ProcessNo = now_proc.ProcNo, ProcessCode = now_proc.WorkCd };
                }

                //マガジン番号取得
                var mag = UseArmsApi.Get_Magazine(lotinfo.LotnNo18);
                if (mag == null) {
                    lotinfo.MagazineNo = null;
                }
                else {
                    lotinfo.MagazineNo = mag.MagazineNo;
                }
            }
            catch (Exception ex) {
                throw new ArmsSystemException(ex.Message);
            }

            return lotinfo;

        }

        /// <summary>
        /// 機種名、10桁ロット番号からロット情報を取得する
        /// <para>ArmsApi、富士情報Apiを使う</para>
        /// </summary>
        /// <param name="productname">機種名</param>
        /// <param name="lotno10">実績収集システムで使用している10桁ロット番号</param>
        /// <returns></returns>
        /// <exception cref="FjhSystemException">富士情報DLLエラー</exception>
        /// <exception cref="ArmsSystemException">ArmsApiDLLエラー</exception>
        public static ApiLotInfo Get_LotInfo_from_LotNo10(string productname, string lotno10) {

            var lotinfo = new ApiLotInfo() { ProductName = productname, LotnNo10 = lotno10 };

            //最初に実績ロット確認
            LotNoDll.LotInfo scm_lot;
            try {
                scm_lot = UseFjhApi.Get_LotInfo_from_JSK(productname, lotno10);
            }
            catch (Exception ex) {
                throw new FjhSystemException(ex.Message);
            }

            if (scm_lot.IsOK) {
                lotinfo.LotnNo18 = scm_lot.SeiLotNo;
                lotinfo.VLotNo = scm_lot.VLotNo;
            }
            else {
                //新システムに対象ロットが無ければ終了
                throw new NotNewSystemException();
            }

            try {
                //ARMSで最終完了工程を確認
                var pre_proc = UseArmsApi.Get_LastProcess(lotinfo.LotnNo18);
                lotinfo.PreProcess = new Process { ProcessName = pre_proc.InlineProNM, ProcessNo = pre_proc.ProcNo, ProcessCode = pre_proc.WorkCd };

                //ARMSで次工程を確認
                var next_proc = UseArmsApi.Get_NextProcess(lotinfo.LotnNo18, pre_proc.ProcNo);
                if (next_proc == null) {
                    lotinfo.NextProcess = null;
                }
                else {
                    lotinfo.NextProcess = new Process { ProcessName = next_proc.InlineProNM, ProcessNo = next_proc.ProcNo, ProcessCode = next_proc.WorkCd };
                }

                //ARMSで次工程を開始済みか確認
                var now_proc = UseArmsApi.Get_NowProcess(lotinfo.LotnNo18);

                if (now_proc == null) {
                    lotinfo.NowProcess = null;
                }
                else {
                    lotinfo.NowProcess = new Process { ProcessName = now_proc.InlineProNM, ProcessNo = now_proc.ProcNo, ProcessCode = now_proc.WorkCd };
                }

                //マガジン番号取得
                var mag = UseArmsApi.Get_Magazine(lotinfo.LotnNo18);
                if (mag == null) {
                    lotinfo.MagazineNo = null;
                }
                else {
                    lotinfo.MagazineNo = mag.MagazineNo;
                }
            }
            catch (Exception ex) {
                throw new ArmsSystemException(ex.Message);
            }

            return lotinfo;

        }

    }

}