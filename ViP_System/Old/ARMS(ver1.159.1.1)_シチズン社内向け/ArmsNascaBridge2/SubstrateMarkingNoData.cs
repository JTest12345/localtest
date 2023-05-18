using ArmsApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class SubstrateMarkingNoData
    {
        public string DataMatrix { get; set; }
        public string MarkingNo { get; set; }

        public static void Import()
        {
            try
            {
                if (String.IsNullOrWhiteSpace(Config.Settings.MarkingNoDirectoryPath) == true) return;

                List<string> files = MappingData.GetFiles(Config.Settings.MarkingNoDirectoryPath, "lmd", 0);
                string destPath = Path.Combine(Config.Settings.MarkingNoDirectoryPath, "Done", DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString());

                if (Directory.Exists(destPath) == false)
                {
                    Directory.CreateDirectory(destPath);
                }

                foreach (string file in files)
                {
                    List<SubstrateMarkingNoData> dataList = getData(file);

                    //既に退避先にファイルがある場合はスルー。(処理せずファイルを削除）
                    //※元ファイルのコピーバッチが10日間？は同じファイルを送り続けてくるのでその対応。
                    //月が変わったら退避先が変わるので同ファイルを余分に無意味なチェックをしてしまうが、
                    //月一回だしそれほど時間はかからないので特に問題ないとする。
                    string destfilePath = Path.Combine(destPath, Path.GetFileName(file));
                    if (File.Exists(destfilePath) == true)
                    {
                        File.Delete(file);
                        continue;
                    }
                    
                    foreach (SubstrateMarkingNoData data in dataList)
                    {
                        ArmsApi.Model.SubstrateInfo svrData = ArmsApi.Model.SubstrateInfo.GetData(data.DataMatrix);

                        if(string.IsNullOrWhiteSpace(svrData.DataMatrix) == true)
                        {
                            ArmsApi.Model.SubstrateInfo regData = new ArmsApi.Model.SubstrateInfo
                            {
                                DataMatrix = data.DataMatrix,
                                thicknessRank = null,
                                MarkingNo = data.MarkingNo,
                                MappingData = null
                            };
                            regData.InsertUpdate();
                        }
                        else
                        {
                            svrData.MarkingNo = data.MarkingNo;
                            svrData.InsertUpdate();
                        }
                    }

                    File.Move(file, destfilePath);
                }
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] SubstrateMarkingNoData Error:" + err.ToString());
            }
        }

        private static List<SubstrateMarkingNoData> getData(string file)
        {
            List<SubstrateMarkingNoData> retv = new List<SubstrateMarkingNoData> ();

            string[] content = File.ReadAllLines(file);

            // ファイルの中身を1行ずつ確認
            foreach (string s in content)
            {
                string[] fileData = s.Split(',');
                if (fileData.Length <= 2)
                {
                    continue;
                }

                SubstrateMarkingNoData data = new SubstrateMarkingNoData();
                data.DataMatrix = fileData[1];
                data.MarkingNo = fileData[2];
                retv.Add(data);
            }
            return retv;
        }
        
    }
}
