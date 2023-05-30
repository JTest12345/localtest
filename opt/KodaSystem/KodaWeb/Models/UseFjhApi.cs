using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CejDataAccessCom;

/*
 *富士情報APIを使用する
 *
 * プロジェクトへ参照追加に最低下記の追加が必要
 * CejDataAccessCom.dll
 * また実行ファイルと同じ場所に下記も必要
 * CejDataAccessCom.dll.config ←データベース接続に修正必要（富士情報に確認）
 * CejDataAccessCom.pdb
 */

namespace KodaWeb.Models {

    public class UseFjhApi {

        /// <summary>
        /// 実績情報(機種名、10桁LotNo)からロット情報を取得する
        /// </summary>
        /// <param name="lot_list"></param>
        public static LotNoDll.LotInfo Get_LotInfo_from_JSK(string productname, string lotno10) {

            var list = new List<LotNoDll.JitLotInfo>() { new LotNoDll.JitLotInfo { TypeCd = productname, JitLotNo = lotno10 } };

            try {
                //ロット情報取得
                LotNoDll lotno = LotNoDll.GetLotNoFromJ(list, true);
                return lotno.LotInfoLst[0];
            }
            catch (MyException myex) {
                //ロット取得時に把握できた例外（MessageはFJH記載)
                throw;
            }
            catch (Exception ex) {
                //その他の例外
                throw;
            }
        }

        /// <summary>
        /// 実績情報(機種名、10桁LotNo)からロット情報を取得する
        /// </summary>
        /// <param name="lot_list"></param>
        public static List<LotNoDll.LotInfo> Get_LotInfo_from_JSK(List<LotNoDll.JitLotInfo> lot_list) {

            try {
                //ロット情報取得
                LotNoDll lotno = LotNoDll.GetLotNoFromJ(lot_list, true);
                return lotno.LotInfoLst;
            }
            catch (MyException myex) {
                //ロット取得時に把握できた例外（MessageはFJH記載)
                throw;
            }
            catch (Exception ex) {
                //その他の例外
                throw;
            }
        }

        /// <summary>
        /// 4Mロット番号(18桁LotNo)からロット情報を取得する
        /// </summary>
        /// <param name="list"></param>
        public static List<LotNoDll.LotInfo> Get_LotInfo_from_LotNo18(List<string> list) {

            try {
                //ロット情報取得
                LotNoDll lotno = LotNoDll.GetLotNoFrom4M(list, true);
                return lotno.LotInfoLst;
            }
            catch (MyException myex) {
                //ロット取得時に把握できた例外（MessageはFJH記載)
                throw;
            }
            catch (Exception ex) {
                //その他の例外
                throw;
            }
        }


    }
}
