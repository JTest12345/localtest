using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using ArmsApi.Model;//ArmsApiを使用する

using KodaClassLibrary;

/*
 * ArmsApiを使用する
 * プロジェクトへ参照追加に最低下記の追加が必要
 * ArmsApi.dll
 * log4net.dll
 * Newtonsoft.Json.dll
 * 以下は必要かもしれない
 * ArmsApi.dll.config
 * ArmsApi.pdb
 * ArmsApi.XmlSerializers.dll
 * log4net.xml
 * Newtonsoft.Json.pdb
 * Newtonsoft.Json.xml
 * 
 * 実際にこのクラスを使用するにはデータベース元がどこかを知る必要があり、
 * それはCドライブ直下にArmsフォルダを丸ごとコピーして使用する
 * C:ARMS/Config/ArmsConfig.xmlの中の"LocalConnString"が接続に使用する文字列
 * 他にも何か所か変える必要あり
*/
namespace KodaWeb.Models {

    public static class UseArmsApi {

        /// <summary>
        /// マガジンコードから18桁LotNo取得
        /// </summary>
        /// <param name="magQR"></param>
        /// <exception cref="Exception">TnMagに対象のマガジン番号が無い時</exception>
        public static string Get_LotNo18(string magQR) {

            Magazine mag;
            //マガジンコードからマガジン情報取得(ここでは18桁LotNoが取得できる)
            //[ARMS].[dbo].[TnMag]データテーブルにアクセスしている
            try {
                mag = Magazine.GetCurrent(magQR);
            }
            catch (Exception ex) {
                throw new ArmsSystemException($"マガジン情報取得に失敗しました。\r\nException Message:{ex.Message}");
            }

            //Magazine情報から18桁ロット番号(NascaLotNO)取得
            if (mag != null) {
                return mag.NascaLotNO;
            }
            else {
                throw new Exception("対象のマガジン情報はありません。");
            }
        }

        /// <summary>
        /// 18桁ロット番号からロット情報取得
        /// </summary>
        /// <param name="lotno18"></param>
        /// <exception cref="Exception"></exception>
        public static AsmLot Get_AsmLot(string lotno18) {

            AsmLot asm_lot;
            //18桁ロット番号からロット情報を読み込む関数(ここでは機種名(typecd)を含むAsmLotクラスを取得)
            //[ARMS].[dbo].[TnLot]データテーブルにアクセスしている
            //[LENS].[dbo].[TnLot]データテーブルにもアクセスしていて、ロット情報が無いとエラーが発生する
            try {
                asm_lot = AsmLot.GetAsmLot(lotno18);
            }
            catch (Exception ex) {
                throw new ArmsSystemException($"ロット情報取得に失敗しました。\r\nException Message:{ex.Message}");
            }

            //ロット情報があれば返す
            if (asm_lot != null) {
                return asm_lot;
            }
            else {
                throw new Exception("対象のロット情報はありません。");
            }
        }

        /// <summary>
        /// 設備番号から設備内の実施中マガジン番号取得
        /// </summary>
        /// <param name="plantcd"></param>
        /// <exception cref="Exception"></exception>
        public static VirtualMag[] Get_MagNo_from_plantcd(string plantcd) {

            //設備番号から設備情報取得（ここではmacnoが取得できる）
            //[ARMS].[dbo].[TmMachine]データテーブルにアクセスしている
            MachineInfo m;
            try {
                m = MachineInfo.GetMachine(plantcd);
            }
            catch (Exception ex) {
                throw new ArmsSystemException($"設備情報取得に失敗しました。\r\nException Message:{ex.Message}");
            }

            //"macno"と"Loaderにある"という条件からマガジン情報取得（ここではVirtualMagを取得できる)
            //"macno"と"Unloaderにある"という条件からもマガジン情報取得
            //[ARMS].[dbo].[TnVirtualMag]データテーブルにアクセスしている(VirtualMagの遷移)
            //[ARMS].[dbo].[TmMachine]データテーブルにもアクセスしている(Machine情報定義している)
            //[ARMS].[dbo].[TmGeneral]データテーブルにもアクセスしている(Loaderとか定義している）

            VirtualMag[] vmags1;
            VirtualMag[] vmags2;
            try {
                vmags1 = VirtualMag.GetVirtualMag(m.MacNo, ((int)Station.Loader));
                vmags2 = VirtualMag.GetVirtualMag(m.MacNo, ((int)Station.Unloader));
            }
            catch (Exception ex) {
                throw new ArmsSystemException($"設備内のマガジン情報取得に失敗しました。\r\nException Message:{ex.Message}");
            }
            var vmag_list = vmags1.ToList();
            vmag_list.AddRange(vmags2.ToList());
            if (vmag_list.Count == 0) {
                throw new Exception("設備内に実施中の投入マガジンがありません。");
            }

            foreach (var magno in vmag_list.Select(x => x.MagazineNo)) {
                int cnt = vmag_list.Count(x => x.MagazineNo == magno);
                if (cnt > 1) {
                    throw new Exception($"設備内に同じ番号のマガジンが2つ以上あります。");
                }
            }

            return vmag_list.ToArray();

        }

        /// <summary>
        /// 最終完了工程を取得
        /// </summary>
        /// <param name="lotno18"></param>
        /// <returns></returns>
        public static Process Get_LastProcess(string lotno18) {
            int pre_procno = Order.GetLastProcNoFromLotNo(lotno18);
            var pre_proc = Process.GetProcess(pre_procno);
            return pre_proc;
        }

        /// <summary>
        /// 次工程を取得
        /// </summary>
        /// <param name="lotno18"></param>
        /// <returns></returns>
        public static Process Get_NextProcess(string lotno18, int pre_procno) {
            AsmLot asmlot = Get_AsmLot(lotno18);
            var next_proc = Process.GetNextProcess(pre_procno, asmlot);
            return next_proc;
        }

        /// <summary>
        /// 次工程を開始済みか取得
        /// </summary>
        /// <param name="lotno18"></param>
        /// <returns></returns>
        public static Process Get_NowProcess(string lotno18) {
            AsmLot asmlot = Get_AsmLot(lotno18);
            Process now_proc;
            try {
                now_proc = Process.GetNowProcess(asmlot);
                return now_proc;
            }
            catch (Exception ex) {
                //開始していない場合、"現在作業中の実績が見つかりません。"の例外が出る
                return null;
            }

        }

        /// <summary>
        /// 18桁ロット番号からマガジン情報を取得する
        /// </summary>
        /// <param name="lotno18"></param>
        /// <returns></returns>
        public static Magazine Get_Magazine(string lotno18) {
            var mag = Magazine.GetMagazine(lotno18);
            return mag;
        }


    }


}