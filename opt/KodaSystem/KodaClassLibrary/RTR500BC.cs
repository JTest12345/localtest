using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;


namespace KodaClassLibrary {

    public class RTR500BC {

        public class XMLObject {

            public Root Root { get; set; }

            public string XmlText { get; set; }
        }

        /// <summary>
        /// 指定したxmlファイルを読み込んで、デシリアライズしたオブジェクトを取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XMLObject GetObj_from_xmlFile(string path) {

            XMLObject obj = new XMLObject();
            var mySerializer = new XmlSerializer(typeof(Root));

            //xmlテキスト取得
            using (var sr = new StreamReader(path, Encoding.UTF8)) {
                obj.XmlText = sr.ReadToEnd();
            }

            //分けないと上手くいかないので再度StreamReaderで読み込む
            //デシリアライズオブジェクト取得
            using (var streamReader = new StreamReader(path, Encoding.UTF8)) {
                obj.Root = (Root)mySerializer.Deserialize(streamReader);
            }

            //XMLに不正な文字が入っている場合は下記のコードを参考にする
            //var xmlSettings = new System.Xml.XmlReaderSettings() {
            //    //XMLとして不正な文字をチェックさせるかどうか
            //    //falseはチェックしない
            //    CheckCharacters = false,
            //};
            ////XmlSerializerでデシリアライズするときはXmlReaderを使って不正な文字を許容させる
            //using (var streamReader = new StreamReader(path, Encoding.UTF8))
            //using (var xmlReader = System.Xml.XmlReader.Create(streamReader, xmlSettings)) {
            //    root = (Root)mySerializer.Deserialize(xmlReader);
            //}

            return obj;
        }

        /// <summary>
        /// xmlテキストから、デシリアライズしたオブジェクトを取得する
        /// <para>データベースに保存しておいたxmlテキストからオブジェクト作る用メソッド</para>
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Root GetObj_from_xmlText(string text) {

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);

            var mySerializer = new XmlSerializer(typeof(Root));
            Root root;

            using (MemoryStream ms = new MemoryStream()) {

                doc.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                root = (Root)mySerializer.Deserialize(ms);
            }

            return root;
        }

    }

    [XmlRoot("file")]
    public class Root {

        [XmlElement("base")]
        public Base Base { get; set; }

        [XmlElement("group")]
        public List<Group> Group { get; set; }

        [XmlAttribute("sample")]
        public string Sample { get; set; }
    }

    public class Base {

        [XmlElement("serial")]
        public string Serial { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("time_diff")]
        public int TimeDiff { get; set; }

        [XmlElement("std_bias")]
        public int StdBias { get; set; }

        [XmlElement("dst_bias")]
        public int DstBias { get; set; }

        [XmlElement("time_zone")]
        public string TimeZone { get; set; }

    }

    public class Group {

        [XmlElement("num")]
        public int Num { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("remote")]
        public List<Remote> Remote { get; set; }
    }

    public class Remote {

        [XmlElement("serial")]
        public string Serial { get; set; }

        [XmlElement("model")]
        public string Model { get; set; }

        [XmlElement("num")]
        public int Num { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("rssi")]
        public int Rssi { get; set; }
        [XmlElement("ch")]
        public List<CH> CH { get; set; }

    }

    public class CH {

        [XmlElement("num")]
        public int Num { get; set; }

        [XmlElement("scale_expr")]
        public string ScaleExpr { get; set; }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("current")]
        public Current Current { get; set; }

        [XmlElement("record")]
        public Record Record { get; set; }
    }

    public class Current {
        //値が入ってこない時は""となりstring型以外はエラーになるため、string型にしている
        //絶対に何かが入ってくる場合はint型でOK

        /// <summary>
        /// 現在値の時刻（世界協定時刻(UTC) 1970年1月1日からの経過秒数）
        /// </summary>
        [XmlElement("unix_time")]
        public long UnixTime { get; set; }

        /// <summary>
        /// 現在値の時刻の文字列（親機情報の時差情報を使って変換したローカルタイム）
        /// </summary>
        [XmlElement("time_str")]
        public string TimeStr { get; set; }

        [XmlElement("value")]
        public Value Value { get; set; }

        [XmlElement("unit")]
        public string Unit { get; set; }

        [XmlElement("batt")]
        public string Batt { get; set; }
    }

    public class Record {
        //値が入ってこない時は""となりstring型以外はエラーになるため、string型にしている
        //絶対に何かが入ってくる場合はint型でOK

        [XmlElement("type")]
        public string Type { get; set; }

        [XmlElement("unix_time")]
        public string UnixTime { get; set; }

        [XmlElement("data_id")]
        public string DataId { get; set; }

        [XmlElement("interval")]
        public string Interval { get; set; }

        [XmlElement("count")]
        public int Count { get; set; }

        [XmlElement("data")]
        public string Data { get; set; }
    }

    public class Value {

        [XmlAttribute("valid")]
        public bool Valid { get; set; }

        [XmlText()]
        public string ValueStr { get; set; }

    }
}