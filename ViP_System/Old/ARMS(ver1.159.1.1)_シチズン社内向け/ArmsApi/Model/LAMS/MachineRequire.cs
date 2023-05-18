using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LAMS
{
    public class MachineRequire
    {
        public string PlantCd { get; set; }

        public int RequireInputFg { get; set; }

        public int InputForbiddenFg { get; set; }

        public DateTime LastUpdDt { get; set; }

        private const int BIT_ON = 1;

        public bool IsRequireInput
        {
            get
            {
                if (this.RequireInputFg == BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool IsInputForbidden
        {
            get
            {
                if (this.InputForbiddenFg == BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static List<MachineRequire> GetMachineRequire()
        {
            return GetMachineRequire(new List<string>());
        }

        public static List<MachineRequire> GetMachineRequire(string plantcd)
        {
            List<string> plantcdList = new List<string> { plantcd };
            return GetMachineRequire(plantcdList);
        }

        public static List<MachineRequire> GetMachineRequire(List<string> plantcdList)
        {
            List<MachineRequire> retV = new List<MachineRequire>();
            
            if (string.IsNullOrWhiteSpace(Config.Settings.LAMSConSTR) == true)
            {
                // LAMS接続文字列の設定が無い場合は、空データを返す
                return retV;
            }

            using (var db = new ArmsApi.Model.DataContext.LAMSDataContext(Config.Settings.LAMSConSTR))
            {
                var MachineRequireList = db.TnMachineRequire.ToList();

                // 供給要求ONで吸い上げた直後にLAMSCrawlerが停止した場合、装置が停止 + テーブル上は供給要求ONのケースが生まれるので
                // テーブル上の更新日時が直近10分以内のレコードのみを抽出する
                MachineRequireList = MachineRequireList.Where(m => m.lastupddt >= DateTime.Now.AddMinutes(-10)).ToList();

                // 引数に設備CD指定がある場合は、フィルタリングをする
                if (plantcdList != null && plantcdList.Count > 0)
                {
                    MachineRequireList = MachineRequireList.Where(m => plantcdList.Contains(m.plantcd)).ToList();
                }

                foreach (ArmsApi.Model.DataContext.TnMachineRequire tnMachineRequire in MachineRequireList)
                {
                    MachineRequire mr = new MachineRequire();
                    mr.PlantCd = tnMachineRequire.plantcd;
                    mr.RequireInputFg = tnMachineRequire.requireinputfg;
                    mr.InputForbiddenFg = tnMachineRequire.inputforbiddenfg;
                    mr.LastUpdDt = tnMachineRequire.lastupddt;

                    // 抽出したタイプを返値に追加
                    retV.Add(mr);
                }
            }
            return retV;
        }
    }
}
