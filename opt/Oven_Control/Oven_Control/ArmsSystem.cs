using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ArmsApi.Model;//ArmsApiを使用する

/*
 * ArmsApiを使用する
 * 自分のPCの下記に元ファイルなどあり、
 * D:\VARMS\ArmsApi\bin\Release\ArmsApi.dll
 * プロジェクトへ参照追加でArmsApi.dllを追加する
 * ビルドするとArmsApi以外にも色々と付属のものが生成される
 * 全部で9個必要
 * ArmsApi.dll
 * ArmsApi.dll.config
 * ArmsApi.pdb
 * ArmsApi.XmlSerializers.dll
 * log4net.dll
 * log4net.xml
 * Newtonsoft.Json.dll
 * Newtonsoft.Json.pdb
 * Newtonsoft.Json.xml
 * 
 * 実際にこのクラスを使用するにはデータベース元がどこかを知る必要があり、
 * それはCドライブ直下にArmsフォルダを丸ごとコピーして使用する
 * C:ARMS/Config/ArmsConfig.xmlの中の"LocalConnString"が接続に使用する文字列
*/

namespace Oven_Control {

    public static class ArmsSystem {

        /// <summary>
        /// マガジンQRコードから機種名、LotNoを取得する
        /// <para>戻り値={"product":A160xxxxxx,"lotno":345DFRxxx}</para>
        /// <para>情報取得失敗時はkeyに"Error"を入れて返す</para>
        /// </summary>
        /// <param name="magQR"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetProductInfo(string magQR) {

            var dic = new Dictionary<string, string>();

            //マガジンQRコードからマガジン番号取得
            //マガジンQR = "A 10000"
            //Aは生産区　A:照明合理化
            //10000はマガジン番号
            string magNo;
            try {
                magNo = magQR.Split(' ')[1];
            }
            catch (Exception e) {
                dic.Add("Error",
                    $"マガジン番号の取得に失敗しました。\nマガジンQRコードのデータ形式は正しいですか？\r\nException Message:{e.Message}");
                return dic;
            }


            Magazine mag;
            //マガジンコードからマガジン情報取得
            try {
                mag = Magazine.GetCurrent(magNo);
            }
            catch (Exception e) {
                dic.Add("Error", $"マガジン情報取得に失敗しました。\r\nException Message:{e.Message}");
                return dic;
            }

            //Magazine情報からロットNo(NascaLotNO)取得
            if (mag != null) {
                dic.Add("lotno", mag.NascaLotNO);
            }
            else {
                dic.Add("Error", "対象のマガジン情報はありません。");
                return dic;
            }

            AsmLot lot;
            //ロットNoからロット情報を読み込む関数
            try {
                lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            }
            catch (Exception e) {
                dic.Add("Error", $"ロット情報取得に失敗しました。\r\nException Message:{e.Message}");
                return dic;
            }

            //ロット情報から機種名取得
            if (lot != null) {
                dic.Add("product", lot.TypeCd);
            }
            else {
                dic.Add("Error", "対象のロット情報はありません。");
                return dic;
            }

            return dic;
        }


    }
}
