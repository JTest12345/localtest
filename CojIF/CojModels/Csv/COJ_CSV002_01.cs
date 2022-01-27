using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CojIF
{
    /// <summary>
    /// {COJ}のCSVファイル適用実験クラス
    /// </summary>
    /// 
    public class COJ_CSV002_01 : COJ
    {
        //
        // ◆基底クラスをCOJクラスとしています
        // 　COJファイル共通のクラスをCOJとしています。
        // 　ヘッダーなど各オブジェクトはインターフェースを使っています

        //
        // ◆入力データの取出し
        // 　COJ毎にプロパティは変わってしまうので個別対応
        // 　ここでもCSV読込のインターフェースを用意して基本的な要素はなるべくあわせていく方向
        //
        public class Fd01
        {
            public FdProps01 formdata { get; set; }
        }

        public class FdProps01 : IFd01_csv
        {
            public string Filepath { get; set; }
            public int Header { get; set; }
            public string Encoding { get; set; }
        }

        public class Fd02
        {
            public List<FdProps02> formdata { get; set; }
        }

        public class FdProps02 : IFd02_csv
        {
            public string Name { get; set; }
            public int Colno { get; set; }
            public string Unit { get; set; }
            public string Type { get; set; }
        }

        public Fd01 _Fd01;
        public Fd02 _Fd02;

        //
        // ◆Constractor
        // 　Jsonの文字列を読み込んで各プロパティに値を代入しています。
        //
        public COJ_CSV002_01(string cojstr)
        {
            // COJをデシリアライズする(Schema/UiSchema/FormDataを除く)
            var coj = JsonConvert.DeserializeObject<COJ>(cojstr);

            // 基底クラスへの適用
            this.header = coj.header;
            this.cejObject = coj.cejObject;

            // 入力データへの適用
            _Fd01 = JsonConvert.DeserializeObject<Fd01>("{\"formdata\":" + coj.cejObject.coList[0].formData.ToString() + "}");
            _Fd02 = JsonConvert.DeserializeObject<Fd02>("{\"formdata\":" + coj.cejObject.coList[1].formData.ToString() + "}");

        }

        //
        // ◆CSV基本情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd01Csv()
        {
            var ret = string.Empty;
            foreach (var prop in this._Fd01.formdata.GetType().GetProperties())
            {
                ret += $"_Fd01.{prop.Name}: " + prop.GetValue(this._Fd01.formdata) + "\r\n";
            }

            return ret;
        }

        //
        // ◆CSV項目情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd02Csv()
        {
            var ret = string.Empty;
            foreach (var props in this._Fd02.formdata)
            {
                ret += "-------------------------------" + "\r\n";
                foreach (var prop in props.GetType().GetProperties())
                {
                    ret += $"_Fd02.{prop.Name}: " + prop.GetValue(props) + "\r\n";
                }
            }

            return ret;
        }

        //
        // ◆CSVをCOJから得た情報で読み込む
        // 　
        public string GetFd_CsvSample()
        {
            var filepath = _Fd01.formdata.Filepath;
            var encoding = _Fd01.formdata.Encoding;
            var contents = new List<string>();
            var ret = string.Empty;

            if (Oskas.CommonFuncs.ReadTextFileLine(filepath, ref contents, encoding))
            {
                for (var itemNO=0; itemNO<_Fd02.formdata.Count; itemNO++) {
                    ret += "-------------------------------" + "\r\n";
                    ret += $"{itemNO + 1}番目の項目抜き出し " + "\r\n";
                    ret += "-------------------------------" + "\r\n";

                    var itemName = _Fd02.formdata[itemNO].Name;
                    var itemUnit = _Fd02.formdata[itemNO].Unit;
                    var itemCol = _Fd02.formdata[itemNO].Colno;

                    ret += $"◇項目：{itemName}" + "\r\n";
                    ret += $"◇単位：{itemUnit}" + "\r\n";
                    ret += "◇生データ" + "\r\n";
                    foreach (var item in contents)
                    {
                        var itemValue = item.Split(',')[itemCol - 1];
                        ret += itemValue + ", ";
                    }
                }

                return ret;
            }
            else
            {
                return "読込失敗";
            }
        }

    }

}
