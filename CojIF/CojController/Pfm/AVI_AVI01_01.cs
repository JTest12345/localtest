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
    /// {COJ}
    /// </summary>
    /// 
    public class AVI_AVI01_01 : AVI01_01
    {

        public Fd01 fdheader;
        public Fd02 jisseki;
        public Fd03 furyomeisai;

        //
        // ◆Constractor
        // 　Jsonの文字列を読み込んで各プロパティに値を代入しています。
        //
        public AVI_AVI01_01(string cojstr)
        {
            // COJをデシリアライズする(Schema/UiSchema/FormDataを除く)
            var coj = JsonConvert.DeserializeObject<COJ>(cojstr);

            // 基底クラスへの適用
            this.header = coj.header;
            this.cejObject = coj.cejObject;

            // 入力データへの適用
            fdheader = JsonConvert.DeserializeObject<Fd01>("{\"formdata\":" + coj.cejObject.coList[0].formData.ToString() + "}");
            jisseki = JsonConvert.DeserializeObject<Fd02>("{\"formdata\":" + coj.cejObject.coList[1].formData.ToString() + "}");
            furyomeisai = JsonConvert.DeserializeObject<Fd03>("{\"formdata\":" + coj.cejObject.coList[2].formData.ToString() + "}");
        }

        //
        // ◆基本情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd01Csv()
        {
            var ret = string.Empty;
            foreach (var prop in this.fdheader.formdata.GetType().GetProperties())
            {
                ret += $"header.{prop.Name}: " + prop.GetValue(this.fdheader.formdata) + "\r\n";
            }

            return ret;
        }

        //
        // ◆基本情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd02Csv()
        {
            var ret = string.Empty;
            foreach (var prop in this.jisseki.formdata.GetType().GetProperties())
            {
                ret += $"jisseki.{prop.Name}: " + prop.GetValue(this.jisseki.formdata) + "\r\n";
            }

            return ret;
        }

        //
        // ◆CSV項目情報のデータ格納状態を返すメソッド
        // 　
        public string GetFd03Csv()
        {
            var ret = string.Empty;
            foreach (var props in this.furyomeisai.formdata)
            {
                ret += "-------------------------------" + "\r\n";
                foreach (var prop in props.GetType().GetProperties())
                {
                    ret += $"furyomeisai.{prop.Name}: " + prop.GetValue(props) + "\r\n";
                }
            }

            return ret;
        }

    }

}
