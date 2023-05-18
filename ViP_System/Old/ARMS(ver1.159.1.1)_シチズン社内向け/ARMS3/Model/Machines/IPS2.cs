using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// IPS装置(DMC高生産/内製機)
    /// </summary>
    public class IPS2 : IPS
	{
        protected override string Tray2DReadRequestAddress() { return "EM60051"; }
        protected override string Tray2DReadJudgeAddress() { return "EM60021"; }
        protected override string DischargeRequestAddress() { return "EM60052"; }
        protected override string DischargeResponseAddress() { return "EM60022"; }
        protected override string WorkStartDtAddress_yy() { return "EM61511"; }
        protected override string WorkEndDtAddress_yy() { return "EM61517"; }
        protected override string Tray2DAddress() { return "EM61300"; }
        protected override int Tray_WORD_LEN() { return 10; }
        /*
        //トレイ読み込みタイミング
        private const string Tray2DReadRequestAddress   = "EM60051";  //トレイ2D読み込み時ON
        private const string Tray2DReadJudgeAddress     = "EM60021";  //トレイ2D読み込み時の判定結果

        //作業完了タイミング
        private const string DischargeRequestAddress = "EM60052"; //作業完了時ON
        private const string DischargeResponseAddress = "EM60022"; //作業完了時の判定

        //開始時刻
        private const string WorkStartDtAddress_yy = "EM61511"; //開始時間:年 2桁 
        //完了時刻
        private const string WorkEndDtAddress_yy = "EM61517"; //開始時間:年 2桁 

        private const string Tray2DAddress  = "EM61300";//供給ﾄﾚｲ_ﾄﾚｲID情報
        private const int Tray_WORD_LEN = 10;      //DM2700
        */

        //<--PE
        private const string Tray2DFinAddress = "EM61500";  //完成ﾄﾚｲ_ﾄﾚｲID情報

        private const string HOST_READY_JUDGE_ADDR      = "EM60001";      //ﾃﾞｰﾀ受付準備OK（作業許可判定:トレイ2D読み込み時）
        private const string HOST_READY_MAKE_ADDR       = "EM60002";      //ﾃﾞｰﾀ受付準備OK（製品出来栄え）
        //-->PE

        protected override void concreteThreadWork()
        {
            //<--PE
            //Ready信号をON
            Plc.SetBit(HOST_READY_JUDGE_ADDR, 1, Convert.ToString(Keyence.BIT_ON));  //ﾃﾞｰﾀ受付準備OK（作業許可判定）
            Plc.SetBit(HOST_READY_MAKE_ADDR, 1, Convert.ToString(Keyence.BIT_ON));   //ﾃﾞｰﾀ受付準備OK（製品出来栄え）
            //-->PE

            // 作業開始登録
            if (Convert.ToInt32(Plc.GetBit(Tray2DReadRequestAddress())) == 1)
            {
                workStart();
            }

            // 作業完了登録
            if (Convert.ToInt32(Plc.GetBit(DischargeRequestAddress())) == 1)
            {
                workComplete(Tray2DFinAddress);
            }
        }
        public override string getResponseCode(string judgecode)
        {
            string retv;
            if (judgecode == "OK")
            {
                retv = "0";
            }
            else if (judgecode == "NG")
            {
                retv = "1";
            }
            else if (judgecode == "FinOK")
            {
                retv = "0";
            }
            else
            {
                throw new ApplicationException("[getResponseCode]に異常な引数が指定されました。judgecode:" + judgecode);
            }

            return retv;
        }
    }
}
