using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResinClassLibrary {

    public class AutoMachine {

        /// <summary>
        /// 自動配合機にセットされている部材情報
        /// </summary>
        public List<SetBuzai> SetBuzaiList { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dic"></param>
        public AutoMachine(List<SetBuzai> list) {
            SetBuzaiList = list;
        }

        /// <summary>
        /// 自動配合機にセットされている部材情報を取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AutoMachine Get_SetBuzai(string path) {

            var list = new List<SetBuzai>();

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

                //1行目を読み込む（使わない）
                sr.ReadLine();

                //2行目以降
                string str;

                while (true) {
                    str = sr.ReadLine();

                    if (str == "" | str == null) {
                        break;
                    }

                    string[] array = str.Split(',');
                    try {
                        list.Add(new SetBuzai {
                            PumpNo = int.Parse(array[0]),
                            DispenserType = array[1],
                            Name = array[2],
                            LotNo = array[3],
                            RemainingWeight = double.Parse(array[4])
                        });
                    }
                    catch (Exception ex) {
                        throw new Exception("自動配合機にセットされている部材情報の取得に失敗しました。");
                    }
                }

                return new AutoMachine(list);
            }

        }

        public class SetBuzai : Buzai {

            /// <summary>
            /// ポンプ番号
            /// </summary>
            public int PumpNo { get; set; }

            /// <summary>
            /// 配合自動機での部材種類
            /// </summary>
            public string DispenserType { get; set; }

            /// <summary>
            /// 残量
            /// </summary>
            public double RemainingWeight { get; set; }

            /// <summary>
            /// 残量が不足している場合はtrueにする
            /// <para>残量 ＜ 配合量×3 ⇒ true</para>
            /// </summary>
            public bool Shortage { get; set; }

        }

    }

}



