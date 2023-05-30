using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ResinClassLibrary;

namespace ResinClassLibrary {

    public class MixInfo {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MixInfo() {
        }

        private const string RESIN = "RESIN";

        /// <summary>
        /// 指示日時
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 条件
        /// </summary>
        public string Conditions { get; set; }

        /// <summary>
        /// 結果/投入指示
        /// </summary>
        public string ResultOrInput { get; set; }

        /// <summary>
        /// 評価方法
        /// </summary>
        public string EvaluationMethod { get; set; }

        /// <summary>
        /// 先行評価ログデータ取得行
        /// </summary>
        public int SenkoLogDataRow { get; set; }

        /// <summary>
        /// 使用する部材リスト
        /// </summary>
        public List<MixBuzai> UseBuzaiList {
            get {
                var list = new List<MixBuzai>();

                //MixBuzai型のプロパティ一覧を取得
                var props = this.GetType().GetProperties().Where(x => x.PropertyType == typeof(MixBuzai));

                foreach (var p in props) {
                    //ここで取得したmix_buzaiはこのクラスのインスタンスのプロパティ
                    MixBuzai mix_buzai = (MixBuzai)this.GetType().GetProperty(p.Name).GetValue(this);

                    if (mix_buzai != null && (mix_buzai.BaseAmount > 0 || mix_buzai.PercentOfTotalWeight > 0)) {
                        list.Add(mix_buzai);
                    }
                }

                return list;
            }
        }


        /// <summary>
        /// 実際の配合量を計算する時に使用する基準配合量[g]
        /// <para>照明HWは100g</para>
        /// </summary>
        public decimal BaseAmountForCalculation { get; set; }

        /// <summary>
        /// 主剤(樹脂A剤)
        /// </summary>
        public MixBuzai ResinA { get; set; } = new MixBuzai() { Type = Buzai.RESIN_A };

        /// <summary>
        /// 硬化剤(樹脂B剤)
        /// </summary>
        public MixBuzai ResinB { get; set; } = new MixBuzai() { Type = Buzai.RESIN_B };

        /// <summary>
        /// フィラー
        /// </summary>
        public MixBuzai Filler { get; set; } = new MixBuzai() { Type = Buzai.FILLER };

        /// <summary>
        /// 酸化チタン
        /// </summary>
        public MixBuzai TiO2 { get; set; } = new MixBuzai() { Type = Buzai.TIO2 };

        /// <summary>
        /// トロ
        /// </summary>
        public MixBuzai Toro { get; set; } = new MixBuzai() { Type = Buzai.TORO };

        /// <summary>
        /// Y1蛍光体
        /// </summary>
        public MixBuzai Y1 { get; set; } = new MixBuzai() { Type = Buzai.YELLOW };

        /// <summary>
        /// Y2蛍光体
        /// </summary>
        public MixBuzai Y2 { get; set; } = new MixBuzai() { Type = Buzai.YELLOW };

        /// <summary>
        /// Y3蛍光体
        /// </summary>
        public MixBuzai Y3 { get; set; } = new MixBuzai() { Type = Buzai.YELLOW };

        /// <summary>
        /// R1蛍光体
        /// </summary>
        public MixBuzai R1 { get; set; } = new MixBuzai() { Type = Buzai.RED };

        /// <summary>
        /// R2蛍光体
        /// </summary>
        public MixBuzai R2 { get; set; } = new MixBuzai() { Type = Buzai.RED };

        /// <summary>
        /// R3蛍光体
        /// </summary>
        public MixBuzai R3 { get; set; } = new MixBuzai() { Type = Buzai.RED };

        /// <summary>
        /// 使用しない部材をNullにする
        /// </summary>
        public void NotUseBuzai_to_Null() {

            //MixBuzai型のプロパティ一覧を取得
            var props = this.GetType().GetProperties().Where(x => x.PropertyType == typeof(MixBuzai));

            foreach (var p in props) {
                //ここで取得したmix_buzaiはこのクラスのインスタンスのプロパティ
                MixBuzai mix_buzai = (MixBuzai)this.GetType().GetProperty(p.Name).GetValue(this);

                if (mix_buzai.PercentOfTotalWeight == null && (mix_buzai.BaseAmount == null || mix_buzai.BaseAmount <= 0)) {
                    this.GetType().GetProperty(p.Name).SetValue(this, null);
                }
            }
        }


        /// <summary>
        /// 100gベースの配合量から実際に配合する量を計算してAmountプロパティにセットする
        /// </summary>
        /// <param name="calc_base_amount">配合比を計算する際の基準樹脂量(1gあたりにするために使用)</param>
        /// <param name="make_amount">樹脂カップを作成する時の樹脂量</param>
        public void Calculate_MixAmount(decimal calc_base_amount, decimal make_amount) {

            foreach (var mb in UseBuzaiList) {
                mb.Amount = (decimal)mb.BaseAmount / calc_base_amount * make_amount;
            }
        }

        /// <summary>
        /// 各部材が±何gの誤差で配合するのか計算して、AllowableErrorGramプロパティにセットする
        /// </summary>
        public void Calculate_AllowableErrorGram() {

            foreach (var mb in UseBuzaiList) {
                //今はg か % かどっちかのみ入ってくるので
                if (mb.AllowableErrorGram == null) {
                    mb.AllowableErrorGram = mb.Amount * mb.AllowableErrorPercent / 100; //%なので100で割る

                    //もし0.001g未満だった場合は0.001gで設定する
                    if (mb.AllowableErrorGram < 0.001m) {
                        mb.AllowableErrorGram = 0.001m;
                    }
                }

                //mb.Amountが桁数が多い小数の場合があり、mb.AllowableErrorGramも桁数が多くなる場合があるので
                //小数点3桁になるように切り捨て処理する
                //0.01238459 ⇒ 12.38459 ⇒ 12 ⇒ 0.012
                decimal d = (decimal)mb.AllowableErrorGram;
                decimal dd = Math.Floor(d * 1000) / 1000;
                mb.AllowableErrorGram = dd;
                mb.UpperAllowableErrorGram = dd;
                mb.LowerAllowableErrorGram = dd;
            }
        }

        /// <summary>
        /// 配合手段を取得する
        /// </summary>
        public string Get_FlowMode() {

            //KSF蛍光体使用の場合
            //int ksf_cnt = UseBuzaiList.Count(x => x.NeedGroveBox == true);
            //if (ksf_cnt > 0) {
            //    return Recipe.MANUAL;
            //}

            //グローブボックスが必要ない使用部材
            var normal_usebuzai_list = UseBuzaiList.Where(x => x.NeedGroveBox == false).ToList();

            //グローブボックスが必要ない使用部材で、自動配合機で出来るもの
            var machine_normal_usebuzai_list = UseBuzaiList.Where(x => x.UseAutoMachine == true && x.NeedGroveBox == false).ToList();


            //全ての部材が自動配合機を使う場合
            if (normal_usebuzai_list.Count == machine_normal_usebuzai_list.Count) {
                return Recipe.AUTO;
            }

            //自動配合機を使う部材が1個もない場合
            if (machine_normal_usebuzai_list.Count == 0) {
                return Recipe.MANUAL;
            }

            //樹脂が自動配合機で出来ない時は手配合
            foreach (var mb in normal_usebuzai_list.Where(x => x.Type.Contains(RESIN) == true)) {
                if (mb.UseAutoMachine == false) {
                    return Recipe.MANUAL;
                }
            }

            //トロが自動配合機で出来ない時は手配合
            foreach (var mb in normal_usebuzai_list.Where(x => x.Type.Contains(Buzai.TORO) == true)) {
                if (mb.UseAutoMachine == false) {
                    return Recipe.MANUAL;
                }
            }

            //自動配合する蛍光体の数取得
            int p_cnt = machine_normal_usebuzai_list.Count(x => x.Type == Buzai.RED || x.Type == Buzai.YELLOW);

            if (p_cnt == 0) {
                return Recipe.CUT_IN;
            }
            else {
                return Recipe.ADD;
            }
        }

        /// <summary>
        /// 配合の順番を設定する
        /// </summary>
        public void Set_MD_MixOrder() {

            //配合する部材を取得する(GroveBox必要ない)
            var normal_mix_list = UseBuzaiList.Where(x => x.NeedGroveBox == false).ToList();
            //配合する部材を取得する(GroveBox必要)
            var ksf_mix_list = UseBuzaiList.Where(x => x.NeedGroveBox == true).ToList();


            //配合順番を決めていく
            int order = 1;//カップが0なので1から

            //①赤蛍光体の配合量が少ない方から配合する
            var red_list = normal_mix_list.Where(x => x.Type == Buzai.RED).OrderBy(x => x.Amount).ToList();
            if (red_list.Count > 0) {
                for (int i = 0; i < red_list.Count; i++) {
                    red_list[i].MixOrder = order;
                    order += 1;
                }
            }

            //②黄色蛍光体の配合量が少ない方から配合する
            var yellow_list = normal_mix_list.Where(x => x.Type == Buzai.YELLOW).OrderBy(x => x.Amount).ToList();
            if (yellow_list.Count > 0) {
                for (int i = 0; i < yellow_list.Count; i++) {
                    yellow_list[i].MixOrder = order;
                    order += 1;
                }
            }

            //③フィラーの配合量が少ない方から配合する(現在1種類しかない)
            var filler_list = normal_mix_list.Where(x => x.Type == Buzai.FILLER).OrderBy(x => x.Amount).ToList();
            if (filler_list.Count > 0) {
                for (int i = 0; i < filler_list.Count; i++) {
                    filler_list[i].MixOrder = order;
                    order += 1;
                }
            }

            //④樹脂　2種類しかない　種類増えたら書き換える
            var resin_list = normal_mix_list.Where(x => x.Type.Contains(RESIN) == true).ToList();
            if (resin_list.Count == 2) {
                //A,B剤なので2でないとおかしい
                ResinA.MixOrder = order;//Aが先
                order += 1;
                ResinB.MixOrder = order;
                order += 1;
            }

            //⑤トロの配合量が少ない方から配合する(現在1種類しかない)
            var toro_list = normal_mix_list.Where(x => x.Type == Buzai.TORO).OrderBy(x => x.Amount).ToList();
            if (toro_list.Count > 0) {
                for (int i = 0; i < toro_list.Count; i++) {
                    toro_list[i].MixOrder = order;
                    order += 1;
                }
            }

            //⑥KSF(GroveBox必要)の配合量が少ない方から配合する(現在1種類しかない)
            var ksf_list = ksf_mix_list.OrderBy(x => x.Amount).ToList();
            if (ksf_list.Count > 0) {
                for (int i = 0; i < ksf_list.Count; i++) {
                    ksf_list[i].MixOrder = order;
                    order += 1;
                }
            }
        }

        /// <summary>
        /// 配合の順番を設定する
        /// </summary>
        public void Set_SDR_MixOrder() {

            //配合する部材を取得する(GroveBox必要ない)
            var normal_mix_list = UseBuzaiList.Where(x => x.NeedGroveBox == false).ToList();

            //配合順番を決めていく
            int order = 1;//カップが0なので1から

            //①樹脂A剤
            ResinA.MixOrder = order;
            order += 1;

            //②フィラーの配合量が少ない方から配合する(現在1種類しかない)
            var filler_list = normal_mix_list.Where(x => x.Type == Buzai.FILLER).OrderBy(x => x.Amount).ToList();
            if (filler_list.Count > 0) {
                for (int i = 0; i < filler_list.Count; i++) {
                    filler_list[i].MixOrder = order;
                    order += 1;
                }
            }

            //③酸化チタンの配合量が少ない方から配合する(現在1種類しかない)
            var tio2_list = normal_mix_list.Where(x => x.Type == Buzai.TIO2).OrderBy(x => x.Amount).ToList();
            if (tio2_list.Count > 0) {
                for (int i = 0; i < tio2_list.Count; i++) {
                    tio2_list[i].MixOrder = order;
                    order += 1;
                }
            }

            //④樹脂B剤
            ResinB.MixOrder = order;
        }

        /// <summary>
        /// ろ過後配合の順番を設定する
        /// </summary>
        public void Set_AfterFilt_SDR_MixOrder() {

            //配合する部材を取得する(GroveBox必要ない)
            var normal_mix_list = UseBuzaiList.Where(x => x.NeedGroveBox == false).ToList();

            //配合順番を決めていく
            int order = 1;//カップが0なので1から

            /*現状トロしか配合するものがない*/

            //①トロの配合量が少ない方から配合する(現在1種類しかない)
            var toro_list = normal_mix_list.Where(x => x.Type == Buzai.TORO).OrderBy(x => x.Amount).ToList();
            if (toro_list.Count > 0) {
                for (int i = 0; i < toro_list.Count; i++) {
                    toro_list[i].MixOrder = order;
                    order += 1;
                }
            }
        }
    }
}

