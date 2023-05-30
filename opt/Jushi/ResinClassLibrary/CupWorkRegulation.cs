using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ResinVBClassLibrary;
using ResinPrg;

namespace ResinClassLibrary {

    public static class CupWorkRegulation {

        ///// <summary>
        ///// 樹脂カップ作業制限時間リストを取得する
        ///// </summary>
        ///// <param name="path">制限時間マスタファイルパス</param>
        ///// <param name="p_name">機種名</param>
        ///// <param name="mixtype_code">樹脂カップ作業工程コード</param>
        ///// <param name="cupno">カップ番号</param>
        ///// <returns></returns>
        //public static List<CupWorkRegulation> Get_CupWorkRegulation(string path, string p_name, string mixtype_code, string cupno) {

        //    var list = new List<CupWorkRegulation>();

        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

        //        //1行目を読み込む（使わない）
        //        sr.ReadLine();

        //        //2行目以降
        //        string str;

        //        while (true) {
        //            str = sr.ReadLine();

        //            if (str == "" | str == null) {
        //                break;
        //            }

        //            string[] array = str.Split(',');

        //            var name_ok = VBFuncs.CheckProductName(p_name, array[0]);

        //            if (name_ok) {
        //                if (mixtype_code == array[1]) {

        //                    int procfrom = int.Parse(array[2]);
        //                    int procto = int.Parse(array[3]);

        //                    //同じ基準工程～次工程の情報が既にあるか数える
        //                    int cnt = list.Count(x => x.ProcessFrom == procfrom && x.ProcessTo == procto);

        //                    if (cnt == 0) {
        //                        list.Add(new CupWorkRegulation {
        //                            CupNo = cupno,
        //                            ProcessFrom = procfrom,
        //                            ProcessTo = procto,
        //                            ForbiddenMinute = int.Parse(array[4]),
        //                            LimitMinute = int.Parse(array[5]),
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //    }

        //    return list;

        //}

        ///// <summary>
        ///// 樹脂カップ作業制限時間リストを取得する
        ///// <para>"PMMSTimeSetting.txt"から取得</para>
        ///// </summary>
        ///// <param name="path">制限時間マスタファイルパス</param>
        ///// <param name="p_name">機種名</param>
        ///// <param name="mixtype_code">樹脂カップ作業工程コード</param>
        ///// <param name="cupno">カップ番号</param>
        ///// <returns></returns>
        //public static List<TmWorkRegulationData> Get_CupWorkRegulation_fromFile(string path, string p_name, string mixtype_code, string cupno) {

        //    var list = new List<TmWorkRegulationData>();

        //    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        //    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

        //        //1行目を読み込む（使わない）
        //        sr.ReadLine();

        //        //2行目以降
        //        string str;

        //        while (true) {
        //            str = sr.ReadLine();

        //            if (str == "" | str == null) {
        //                break;
        //            }

        //            string[] array = str.Split(',');

        //            var name_ok = VBFuncs.CheckProductName(p_name, array[0]);

        //            if (name_ok) {
        //                if (mixtype_code == array[1]) {

        //                    int procfrom = int.Parse(array[2]);
        //                    int procto = int.Parse(array[3]);

        //                    //同じ基準工程～次工程の情報が既にあるか数える
        //                    int cnt = list.Count(x => x.procfrom == procfrom && x.procto == procto);

        //                    if (cnt == 0) {
        //                        list.Add(new TmWorkRegulationData {
        //                            cupno = cupno,
        //                            procfrom = procfrom,                 //基準工程
        //                            procto = procto,                     //次工程
        //                            fromwaittime = int.Parse(array[4]),  //基準工程実施してから次工程開始が出来るようになるまでの時間(分)
        //                            fromtoendtime = int.Parse(array[5]), //基準工程実施してから次工程完了までの時間(分)
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //    }

        //    return list;
        //}
    }
}
