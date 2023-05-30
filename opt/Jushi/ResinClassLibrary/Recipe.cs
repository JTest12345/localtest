using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using ResinPrg;//Junki Nakamura DLL

namespace ResinClassLibrary {

    public class AfterFiltrationRecipe : Recipe {

        /// <summary>
        /// 空のカップ重量
        /// </summary>
        [JsonProperty("beforeCupWeight")]
        public BeforeCupWeighing BeforeCupWeight { get; set; }

        public class BeforeCupWeighing {

            /// <summary>
            /// 実際の計量結果
            /// </summary>
            [JsonProperty("resultAmount")]
            public decimal? ResultAmount { get; set; }

            /// <summary>
            /// 計量開始時間
            /// </summary>
            [JsonProperty("start-time")]
            public DateTime StartTime { get; set; }

            /// <summary>
            /// 計量完了時間
            /// </summary>
            [JsonProperty("end-time")]
            public DateTime EndTime { get; set; }
        }
    }

    public class Recipe {

        public const string AUTO = "AUTO";
        public const string ADD = "ADD";
        public const string CUT_IN = "CUT_IN";
        public const string MANUAL = "MANUAL";


        /// <summary>
        /// 配合手段
        /// <para>AUTO,ADD,CUT_IN,MANUAL</para>
        /// </summary>
        [JsonProperty("flowMode")]
        public string FlowMode { get; set; }

        /// <summary>
        /// 機種名
        /// </summary>
        [JsonProperty("productName")]
        public string ProductName { get; set; }

        /// <summary>
        /// LotNoリスト
        /// </summary>
        [JsonProperty("lotnoList")]
        public List<string> LotNoList { get; set; }

        /// <summary>
        /// 波長ランク
        /// <para>D1,D2,D3,D4</para>
        /// </summary>
        [JsonProperty("wavelengthRank")]
        public string WavelengthRank { get; set; }

        /// <summary>
        /// カップ番号
        /// </summary>
        [JsonProperty("cupno")]
        public string CupNo { get; set; }

        /// <summary>
        /// このカップを使用する基板枚数
        /// </summary>
        [JsonProperty("LF-num")]
        public int LF_Num { get; set; }

        /// <summary>
        /// 配合タイプコード
        /// <para>PMMS連携用</para>
        /// </summary>
        [JsonProperty("mixTypeCode")]
        public string MixTypeCode { get; set; }

        /// <summary>
        /// 強制手配合時の配合タイプコード
        /// <para>差立てした後に自動配合機が壊れて手配合したい時用</para>
        /// <para>PMMS連携用</para>
        /// </summary>
        [JsonProperty("manualMode-mixTypeCode")]
        public string ManualMode_MixTypeCode { get; set; }

        /// <summary>
        /// 樹脂カップの工程番号
        /// <para>PMMS連携用</para>
        /// </summary>
        [JsonIgnore]
        public int ProcessNo { get; set; }

        /// <summary>
        /// 樹脂種類タイプ
        /// <para>MD,SDR,...</para>
        /// </summary>
        [JsonProperty("moldType")]
        public string MoldType { get; set; }

        /// <summary>
        /// 特殊指定
        /// </summary>
        [JsonProperty("specialDesignation")]
        public string SpecialDesignation { get; set; }

        /// <summary>
        /// 変化点
        /// </summary>
        [JsonProperty("henkaten")]
        public string Henkaten { get; set; }

        /// <summary>
        /// 使用部材
        /// </summary>
        [JsonProperty("useBuzai")]
        public List<RecipeBuzai> UseBuzai { get; set; }

        /// <summary>
        /// 手配合部材の結果
        /// </summary>
        [JsonIgnore]
        public Dictionary<int, RecipeBuzai> ManualMixResult { get; set; }

        /// <summary>
        /// レシピファイル名
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        /// 樹脂カップの作業規制時間
        /// <para>PMMS連携用</para>
        /// </summary>
        [JsonProperty("cupWorkRegulations")]
        public List<TmWorkRegulationData> CupWorkRegulations { get; set; }//ResinPrg DLLに置き換え
        //public List<CupWorkRegulation> CupWorkRegulations { get; set; }

        /// <summary>
        /// この樹脂カップ番号を作製した回数
        /// <para>初めて作製 = 0</para>
        /// </summary>
        [JsonIgnore]
        public int TryNum { get; set; }



        /// <summary>
        /// 配合自動機用のレシピ作成（メーカーとの仕様取り交わしあり）
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="path">保存先ファイルパス</param>
        public static void CreateRecipe_for_AutoMachine(Recipe rec, string path, bool change_to_add = false) {

            //配合量を小数点4桁に四捨五入する→3桁に変更(2022.11.21)
            foreach (var rb in rec.UseBuzai) {
                rb.Amount = Math.Round(rb.Amount, 3, MidpointRounding.AwayFromZero);
            }

            string text = "TCSR0001\r\n";//TODO:装置名固定だと複数台対応できない

            text += "START\r\n";

            if (change_to_add) {
                text += $"FLOW,{ADD}\r\n";
            }
            else {
                text += $"FLOW,{rec.FlowMode}\r\n";
            }

            text += $"KISYU,{rec.ProductName}\r\n";
            text += $"SOSHI_RANK,{rec.WavelengthRank}\r\n";
            text += $"LOT_NO,{string.Join("/", rec.LotNoList)}\r\n";
            text += $"CUP_NO,{rec.CupNo}\r\n";


            text += "JUSHI_MACHINE_NO,A\r\n";

            text += $"QTY,{rec.LF_Num.ToString()}\r\n";

            text += "HENKATEN,\r\n";

            //配合タイプ情報
            text += $"REMARKS,{rec.MixTypeCode}:{rec.MoldType}/{rec.SpecialDesignation}\r\n";

            //部材情報追加
            foreach (var rb in rec.UseBuzai) {
                text += $"BUZAI,{rb.Name.Replace("/", "")},{rb.LotNo},{rb.UseAutoMachine.ToString().ToUpper()},{rb.Amount.ToString()},{rb.UpperAllowableErrorGram.ToString()},{rb.LowerAllowableErrorGram.ToString()}\r\n";
            }

            text += "END";

            try {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                    sw.Write(text);
                }
            }
            catch (Exception ex) {
                throw;
            }
        }

        /// <summary>
        /// ～_rcp.txtファイルを読み込んでレシピを取得
        /// <para>CreateRecipe_for_AutoMachine メソッドで作成したファイルを読み込む</para>
        /// </summary>
        public static Recipe Get_Recipe_from_txtFile(string path) {

            var recipe = new Recipe {
                UseBuzai = new List<RecipeBuzai>(),
                FileName = Path.GetFileName(path)
            };

            int order = 1;//カップが0なので1から
            string line;

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

                    while (true) {

                        line = sr.ReadLine();

                        if (string.IsNullOrEmpty(line)) {
                            break;
                        }

                        var array = line.Split(',');

                        if (array[0] == "FLOW") {
                            recipe.FlowMode = array[1];
                        }
                        else if (array[0] == "KISYU") {
                            recipe.ProductName = array[1];
                        }
                        else if (array[0] == "SOSHI_RANK") {
                            recipe.WavelengthRank = array[1];
                        }
                        else if (array[0] == "LOT_NO") {
                            var lots = array[1].Split('/');
                            recipe.LotNoList = lots.ToList();
                        }
                        else if (array[0] == "CUP_NO") {
                            recipe.CupNo = array[1];
                        }
                        else if (array[0] == "QTY") {
                            recipe.LF_Num = int.Parse(array[1]);
                        }
                        else if (array[0] == "HENKATEN") {
                            var list = new List<string>();
                            for (int i = 1; i < array.Length; i++) {
                                var s = array[i].Replace(" ", "");
                                if (string.IsNullOrEmpty(s) == false) {
                                    list.Add(s);
                                }
                            }

                            if (list.Count == 0) {
                                recipe.Henkaten = "";
                            }
                            else {
                                recipe.Henkaten = string.Join(",", list);
                            }
                        }
                        else if (array[0] == "REMARKS") {
                            try {
                                var sp = array[1].Split(':');
                                recipe.MixTypeCode = sp[0];

                                //REMARKSの記述が無い場合(一旦仮で入れておく)
                                if (string.IsNullOrEmpty(recipe.MixTypeCode)) {
                                    recipe.MixTypeCode = "101";
                                }

                                //一旦入れておく
                                if (sp[1].Contains("SDR")) {
                                    recipe.MoldType = "SDR";
                                }
                                else {
                                    recipe.MoldType = "MD";
                                }
                            }
                            catch {
                                recipe.MoldType = "MD";
                            }
                        }
                        else if (array[0] == "BUZAI") {
                            var use_buzai = new RecipeBuzai() {
                                Name = array[1],
                                LotNo = array[2],
                                Amount = decimal.Parse(array[4]),
                                UpperAllowableErrorGram = decimal.Parse(array[5]),
                                LowerAllowableErrorGram = decimal.Parse(array[6])
                            };

                            //Manualフォルダに入っているレシピファイルは
                            //手配合ならTRUEになっている
                            //Autoフォルダのレシピファイルとは違う
                            if (bool.Parse(array[3])) {
                                use_buzai.UseAutoMachine = false;
                            }
                            else {
                                use_buzai.UseAutoMachine = true;
                            }

                            //配合量を小数点以下3桁にする 1.2345→1.235（四捨五入） MidpointRounding.AwayFromZeroでないとだめ
                            use_buzai.Amount = Math.Round(use_buzai.Amount, 3, MidpointRounding.AwayFromZero);


                            use_buzai.MixOrder = order;
                            order += 1;

                            recipe.UseBuzai.Add(use_buzai);
                        }

                    }

                }

                return recipe;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// 配合ソフト(内製)用レシピ作成
        /// <para>JSON形式で保存して、読み込むときは丸ごとクラス変換する</para>
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="path">保存先ファイルパス</param>
        /// <returns>レシピファイルの中身の文字列</returns>
        public static string CreateRecipe_for_HaigoSoft<T>(T recipe, string path) where T : Recipe {

            //配合量を小数点3桁に四捨五入する
            foreach (var rb in recipe.UseBuzai) {
                rb.Amount = Math.Round(rb.Amount, 3, MidpointRounding.AwayFromZero);
            }

            //レシピファイルは読みやすいようにインデントあり
            string text = JsonConvert.SerializeObject(recipe, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

            try {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                    sw.Write(text);
                }
                //返す文字列は最小限にインデントなし
                return JsonConvert.SerializeObject(recipe, Formatting.None, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex) {
                throw;
            }
        }

        /// <summary>
        /// ～_rcp.jsonファイルを読み込んでレシピを取得
        /// <para>CreateRecipe_for_HaigoSoft メソッドで作成したファイルを読み込む</para>
        /// </summary>
        public static T Get_Recipe_from_jsonFile<T>(string path) where T : Recipe {

            string json;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {
                json = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

    }
}
