using System;
using System.Collections.Generic;
using System.Text;

namespace EICS
{
    /// <summary>
    /// メッセージ情報
    /// </summary>
    public class MessageJAInfo: IMessage
    {
        private const string MESSAGE_1 = "全装置の処理を開始します。よろしいですか？";
        private const string MESSAGE_2 = "全装置の処理を停止します。よろしいですか？";
        private const string MESSAGE_3 = "管理限界値(MAX)を越えました。";
        private const string MESSAGE_4 = "管理限界値(MIN)を越えました。";
        private const string MESSAGE_5 = "設定値に誤りがあります。";
        private const string MESSAGE_6 = "「Kiss」が起動していません。至急起動と設定を行って下さい。サーバ：{0}";
        private const string MESSAGE_7 = "{0}に修正してもよろしいですか？";
        private const string MESSAGE_8 = "修正完了しました。";
        private const string MESSAGE_9 = "選択されていません。";
        private const string MESSAGE_10 = "ファイルが存在しません。";
        private const string MESSAGE_11 = "ファイルが読み取り専用です。";
        private const string MESSAGE_12 = "暫く経ってから実行して下さい。";
        private const string MESSAGE_13 = "{0}が設定されていません。";
		private const string MESSAGE_14 = "「BlackJumboDog」が起動していません。至急起動して下さい。サーバ：{0}";
        private const string MESSAGE_15 = "[設備番号{0}]例外が発生した為、処理を停止します。";
        private const string MESSAGE_16 = "設備番号：";
        private const string MESSAGE_17 = "既に起動中です。多重起動を防ぐ為、起動出来ません。";
        private const string MESSAGE_18 = "アプリケーションを終了します。よろしいですか？";
        private const string MESSAGE_19 = "処理を終了しています。暫く経ってから再度ボタンを押して下さい。";
        private const string MESSAGE_20 = "型番の設定をして下さい。";
        private const string MESSAGE_21 = "チップの設定をして下さい。";
        //private const string MESSAGE_22 = "[{0}/{1}号機][{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5}";
        //private const string MESSAGE_23 = "[{0}/{1}号機][{2}]の設定値に誤りがあります。取得値={3},閾値={4}";
        private const string MESSAGE_22 = "[{0}/{1}号機][管理番号:{8}/{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5},Lot={6},Linecd={7}";
		private const string MESSAGE_23 = "[{0}/{1}号機][管理番号:{7}/{2}]の設定値に誤りがあります。取得値={3},閾値={4},Lot={5},Linecd={6}";
        private const string MESSAGE_24 = "[設備番号:{0}/実績参照日時:{1}]投入されていないか時刻がずれています。(実績参照日時を開始～完了間に含む実績がありません)";
        private const string MESSAGE_25 = "[{0}/{1}号機]ファイルが20個以上溜まっています。状況を確認下さい。";
        private const string MESSAGE_26 = "[{0}/{1}号機][Pick Level]で前回値との差が{2}と大きく、ｺﾚｯﾄ/ｽﾀﾝﾌﾟﾋﾟﾝ/ｲｼﾞｪｸﾀｰﾋﾟﾝの破損の恐れがあります。破損の有無確認をお願いします";
        private const string MESSAGE_27 = "[型番 {0}/ファイル種類 {1}]は項目紐付けマスタに存在しません。";
        private const string MESSAGE_28 = "[型番 {0}/管理番号 {1}/管理名 {2}]は閾値マスタに存在しません。";
        private const string MESSAGE_29 = "[{0}/{1}号機][Bond Level]で前回値との差が{2}と大きく、ｺﾚｯﾄ/ｽﾀﾝﾌﾟﾋﾟﾝ/ｲｼﾞｪｸﾀｰﾋﾟﾝの破損の恐れがあります。破損の有無確認をお願いします";
        private const string MESSAGE_30 = "リストから削除します。よろしいですか？\r\n[{0}]";
        private const string MESSAGE_31 = "行を選択して下さい。";
        private const string MESSAGE_32 = "入力に誤りがあります。";
        private const string MESSAGE_33 = "登録完了しました。";
        private const string MESSAGE_34 = "ARMSにロット情報がありません。Typeを手入力してください。";
        private const string MESSAGE_35 = "装置リストの取得に失敗しました。マスタを確認して下さい。";
        private const string MESSAGE_36 = "スタートファイルがマスタに設定されていません。";
        private const string MESSAGE_37 = "処理済みスタートファイルのファイル名変更に失敗しました。";
        private const string MESSAGE_38 = "[{0}/{1}号機]ファイル処理に失敗しました。";
        private const string MESSAGE_39 = "監視フォルダが見つかりません。 ファイルパス {0}";
        private const string MESSAGE_40 = "[型番 {0}]チップ搭載数がマスタに存在しません。";
        private const string MESSAGE_41 = "[型番 {0}]フレーム情報がマスタに存在しません。";
        private const string MESSAGE_42 = "[{0}/{1}号機]";
        private const string MESSAGE_43 = "[{0}/{1}]フレーム供給状態の変更に失敗しました。PLCの電源等を確認して下さい。";
        private const string MESSAGE_44 = "ファイルサイズが0KBの為、削除 ファイルパス:{0}";
        private const string MESSAGE_45 = "ファイル内容に問題があります。製造のメンテGに連絡して下さい。ファイルパス:{0} 行:{1}";
        private const string MESSAGE_46 = "登録済みの為、削除 ファイルパス:{0}";
        private const string MESSAGE_47 = "予期しないタイミングです。システム担当者に連絡して下さい。ファイルパス:{0}";
        private const string MESSAGE_48 = "処理が存在しません。システム担当者に連絡して下さい。ファイルパス:{0}";
        private const string MESSAGE_49 = "号機";
        private const string MESSAGE_50 = "ファイル操作の制限時間を超えました。ファイルがロックされている可能性があります。ファイルパス:{0}";
        private const string MESSAGE_51 = "作成途中のファイルが存在します。ファイルパス：{0}";
        private const string MESSAGE_52 = "処理中フォルダに{3}ファイルが揃っていません。マガジン連番:{0} 該当装置に行って、移動元から移動先へファイルを移動して下さい。移動元:{1}　移動先:{2}";
        private const string MESSAGE_53 = @"C:\AD830A\temp";
        private const string MESSAGE_54 = "[ロット番号 {0}]マッピングファイルが見つかりませんでした。フォルダパス:{1} ";
        private const string MESSAGE_55 = "[ロット番号 {0}]ARMSにロット情報が存在しません。システム担当者に連絡して下さい。";
        private const string MESSAGE_56 = "未マッピング時のマッピングフラグOFF ロットが指定されていません。";
        private const string MESSAGE_57 = "[ロット番号 {0}]検査機対象外です。";
        private const string MESSAGE_58 = "結合するマッピングデータの数が一致しません";
        private const string MESSAGE_59 = "マッピングデータの書き込みに失敗しました。";
        private const string MESSAGE_60 = "[型番 {0}]型番の書き込みに失敗しました。";
        private const string MESSAGE_61 = "[処理CD {0}]不正な処理CDが含まれています。";
        private const string MESSAGE_62 = "[ロット番号 {0}]マッピングファイルが存在しない為、全数検査を行います。";
        private const string MESSAGE_63 = "MMファイル内容のアドレスが不足しています。不足アドレスには検査を代入しました。";
        private const string MESSAGE_64 = "[管理項目名 {0}]チップ区分がマスタで設定されていません。";
        private const string MESSAGE_65 = "[ロット番号 {0}]該当する型番が見つかりませんでした。";
        private const string MESSAGE_66 = "[不良アドレス {0}]不良で樹脂少量するアドレスが基マッピングアドレスより大きい所を指しています。";
        private const string MESSAGE_67 = "[変化点フラグ {0}]書き込みに失敗しました。";
        private const string MESSAGE_68 = "外観検査機とEICSでWBマッピング機能(有効/無効)に相違があります。";
        private const string MESSAGE_69 = "設定ファイルの項目が足りません。 設定項目：{0}";
        private const string MESSAGE_70 = "周辺検査列数のマスタ設定に不備があります。製品型番：{0}";
        private const string MESSAGE_71 = "モールド樹脂少箇所が通常モールドされています。ロット番号:{0} アドレス:{1}";
        private const string MESSAGE_72 = "管理されていないシリンジです。シリンジNO:{0} ";
        private const string MESSAGE_73 = "管理されていない動作パターンです。動作パターンNO:{0} ";
        private const string MESSAGE_74 = "管理されていないシリンジです。シリンジ数:{0} ";
        //private const string MESSAGE_75 = "[{0}]が管理限界値({1})を越えました。取得値={2},閾値{1}={3}";
        //private const string MESSAGE_76 = "[{0}]の設定値に誤りがあります。取得値={1},閾値={2}";
        //private const string MESSAGE_77 = "[{0}]の取得値に誤りがあります。取得値={1},閾値={2}";
        private const string MESSAGE_75 = "[{0}/{6}]が管理限界値({1})を越えました。取得値={2},閾値{1}={3},Lot={4},Linecd={5}";
		private const string MESSAGE_76 = "[{0}/{5}]の設定値に誤りがあります。取得値={1},閾値={2},Lot={3},Linecd={4}";
		private const string MESSAGE_77 = "[{0}/{5}]の取得値に誤りがあります。取得値={1},閾値={2},Lot={3},Linecd={4}";
        private const string MESSAGE_78 = "[QcParamNO:{0}]検索位置にパラメータ値が存在しません。紐付けマスタを確認して下さい。";
        private const string MESSAGE_79 = "TCPの受信タイムアウト時間を超過したか、受信に問題があった為、接続を切断しました。";
        private const string MESSAGE_80 = "TCP接続が切断されました。";
        private const string MESSAGE_81 = "装置との通信確立に失敗しました。再度開始を行うか、同アドレスへの通信確立がされていないか確認して下さい。";
        private const string MESSAGE_82 = "装置から切断要求を受信した為、接続を切断しました。装置起動確認後、再度開始して下さい。";
        private const string MESSAGE_83 = "装置から要求タイムアウトを受信しました。再度開始して下さい。";
        private const string MESSAGE_84 = "設定ファイルに存在しない設備です。設備CD:{0}";
        private const string MESSAGE_85 = "[マガジン番号(ロット) {0}]一致するロット情報が存在しません。";
        private const string MESSAGE_86 = "[装置種類 {0}]処理が存在しません。装置種類が正しいか確認して下さい。";
        private const string MESSAGE_87 = "[装置型式 {0}]マッピング順序並び替え情報がマスタに存在しません。";
        private const string MESSAGE_88 = "[フレーム番号 {0}]搭載段数のマスタ設定がされていません。";
        private const string MESSAGE_89 = "[汎用グループCD {0}]汎用情報がマスタに登録されていません。";
		private const string MESSAGE_90 = "開始日時が異常値です。ファイルパス：{0}";
		private const string MESSAGE_91 = "ロット/マガジン番号から特定出来る稼働中のデータが複数存在します。サーバーNO：{0}　ロット番号：{1}　マガジン番号：{2}";
		private const string MESSAGE_92 = "ロット/マガジン番号から特定出来る稼働中のデータが存在しません。サーバーNO：{0}　ロット/マガジン番号：{1}";
		private const string MESSAGE_93 = "複数ラインでロット/マガジン番号が確認され、ラインが特定出来ません。サーバーNO_1：{0}　サーバーNO_2：{1}　ロット/マガジン番号：{2}";
		private const string MESSAGE_94 = "いずれのサーバからもロット/マガジン番号でデータが取得できません。ロット/マガジン番号：{0}";
		private const string MESSAGE_95 = "システムで想定していない設備のModel名です。マスタ設定を確認して下さい。設備番号:{0}　Model名:{1}";
		private const string MESSAGE_96 = "inlineCDを数値変換出来ませんでした。システム担当者に連絡してください。inlineCD:{0}";
		private const string MESSAGE_97 = "システム管理者に連絡して下さい。\r\n装置Noから装置リストのノード取得に失敗しました。装置No：{0}";
		private const string MESSAGE_98 = "設定ファイルのLineTypeが不正です。設定値：{0}";
		private const string MESSAGE_99 = "【外観検査機 出力異常】検査開始/完了アドレスデータが数値ではありません。 取得データ：{0}";
		private const string MESSAGE_100 = "【外観検査機 不正応答】 受信データ：{0}";
		private const string MESSAGE_101 = "【外観検査機 出力異常】検査開始・完了アドレス不正（開始・完了アドレス大小逆）開始アドレス：{0}/完了アドレス：{1}";
		private const string MESSAGE_102 = "【外観検査機 出力異常】検査開始・完了アドレス不正（検査エリア{2}の開始アドレスと検査エリア{3}の完了アドレスの大小逆）開始アドレス：{0}/完了アドレス：{1}";
		private const string MESSAGE_103 = "【外観検査機 出力異常】検査開始・完了アドレス不正（検査開始アドレスもしくは検査完了アドレスが1～{2}の範囲になっていません。）\r\n検査開始アドレス：{0}/検査完了アドレス：{1}";
		private const string MESSAGE_104 = "【外観検査機 検査内容不整合】検査必要箇所数と検査箇所数(MMファイル)不一致 装置：{0}/ロットNO：{1}/マガジンNO：{2}/検査機検査設定数：{3}/MMファイル検査箇所数：{4}";
		private const string MESSAGE_105 = "【外観検査機 検査内容不整合】検査必要箇所と検査箇所(MMファイル)不一致 装置：{0}/ロットNO：{1}/マガジンNO：{2}【外観検査機】検査必要アドレス：{3}";
		private const string MESSAGE_106 = "[{0}/{1}号機]LotNo={2}が指定の桁数と異なります。指定桁数：{3}桁 該当装置のフォルダ内のファイルを『reserve』フォルダに移動してください";
		private const string MESSAGE_107 = "[{0}/{1}号機]MagazineNo={2}が指定の桁数と異なります。{3}指定桁数：{4}桁";
		private const string MESSAGE_108 = "受信タイムアウト（装置からのデータ送信が途切れました。）　タイムアウト時間：{0}秒/受信バイト：{1}byte/送受信長：{2}byte";
        private const string MESSAGE_109 = "デバイス番号異常\r\n応答内容：{0}";
        private const string MESSAGE_110 = "コマンド異常\r\n応答内容：{0}";
        private const string MESSAGE_111 = "書込み禁止アドレスへの書込みが発生\r\n応答内容：{0}";
        private const string MESSAGE_112 = "【出力異常】検査済み段のフラグ情報が数値ではありません。変換対象データ：{0}";
		private const string MESSAGE_113 = "マガジンQRの識別子と一致しません。取得データ{0} / マガジン文字数{1} / マガジン文字列{2}";
		private const string MESSAGE_114 = "[型番 {0}/Prefix_NM {1}]は項目紐付けマスタに存在しません。";
		private const string MESSAGE_115 = "[型番 {0}/Prefix_NM {1}/QcParam_NO {2}]MachinePrefix_NMが設定されていません。";
		private const string MESSAGE_116 = "[ロットNO {0}]の{1}桁目～{2}文字の文字[{3}]から年or月or日が取得出来ませんでした。";
		private const string MESSAGE_117 = "指定された数値が32進数一桁に変換出来ません。(指定値:{0})";
		private const string MESSAGE_118 = "アッセンロットNOの7桁～8桁を32進数に変換出来ませんでした。変換対象：{0}";
        private const string MESSAGE_119 = "ロットマーキング装置がデータ送信完了信号に応答しない為、タイムアウトしました。装置：{0}";
        private const string MESSAGE_120 = "PLC_KeyenceのGetBit()にて予期せぬデータが取得されました。取得データ：{0}";
		private const string MESSAGE_121 = "パラメータデータ[{0}]がdecimal型に変換出来ません。";
        private const string MESSAGE_122 = "ChangeUnitValueのマスタ設定値が不正です。ライン：{0} QCParam：{1} ChangeUnitVa：{2}l";
        private const string MESSAGE_123 = "Int16/Int32以外の対応していない変換が行われました。システム管理者に連絡してください。";
        private const string MESSAGE_124 = "TmTypeCond：レコードが一意に定まりません。レコード件数：{0}/ ラインCD：{1}/ ModelNM：{2}/ 設備CD：{3}/ タイプ：{4}/ CondCD：{5}";
        private const string MESSAGE_125 = "TmTypeCond：CondCDが数値変換出来ません。CondCD：{0}";
		private const string MESSAGE_126 = "【{0}】始業点検データと通常検査データが混在しています。データの確認を行い、ファイルを退避してください";
		private const string MESSAGE_127 = "【{0}】始業点検データの確認が正常に行われませんでした。データの確認を行い、システム管理者に連絡してください。";
		private const string MESSAGE_128 = "同名ファイル存在の為、削除 ファイルパス:{0}";
        private const string MESSAGE_129 = "ロットNoを取得できませんでした。ロット情報取得条件：装置[{0}]/時刻[{1}]";
        private const string MESSAGE_130 = "該当ロットの情報が取得できません。取得条件　ライン:{0} / 設備CD:{1} / マガジンNO:{2} / 開始済み・未完了ロット:{3}";
        private const string MESSAGE_131 = "該当ロットの情報が複数存在します。実績を確認してください。取得条件　ライン:{0} / 設備CD:{1} / マガジンNO:{2} / 開始済み・未完了ロット：{3}";
        private const string MESSAGE_132 = "システム処理がLineType:Outに対応していません。設定かシステム処理に誤りがあります。確認してください。";
		private const string MESSAGE_133 = "TmPRMのTotalKBが設定されていますが、RefQcParamが未設定です。管理番号[ {0} ]";
		private const string MESSAGE_134 = "DC:LMファイルに測定日付がありません。";
		private const string MESSAGE_135 = "{0}を数値変換できません。変換対象:{1}";
		private const string MESSAGE_136 = "【管理番号：[{0}]】Total_KBとRefQcParam_NOの設定に矛盾があります。";
		private const string MESSAGE_137 = "{0}の設定値に誤りがあります。(設定可能な値はtrueかfalse) 設定値：{1} タグ：{2}";
		private const string MESSAGE_138 = "ロットNoが紐付かない為、BM数の登録処理を中止 マガジンNO：{0} / BM数：{1}";
		private const string MESSAGE_139 = "設定ファイルのBadMarkingInfoの設定に誤りがあります。";
		private const string MESSAGE_140 = "装置：{0} ロットが紐付いていない為、{1}File処理スキップ/対象ファイル：{2}";
		private const string MESSAGE_141 = "装置監視開始時の処理において、フォルダが作成出来ません。 対象パス:{0}";
        private const string MESSAGE_142 = "PLCエラー応答：{0}/コマンド：{1}";
        private const string MESSAGE_143 = "【検査内容不整合】MMファイルが存在しない為、検査必要と設定されたアドレスとの照合が行えませんでした。";
		private const string MESSAGE_144 = "ファイル内容がありません。 ファイルパス:{0}";
		private const string MESSAGE_145 = "DateTime型に変換できません。 ファイルパス:{0} / 日付列値:{1} / 時間列値:{2}";
		private const string MESSAGE_146 = "設定ファイルが見つかりません。ファイルパス:{0}";
		private const string MESSAGE_147 = "他ラインのEICSへのエラー通知機能：{0}";
		private const string MESSAGE_148 = "エラーメッセージ通知対象ライン：{0}";
		private const string MESSAGE_149 = "TmPrmのCauseModel_NMの設定が正しくありません。DBサーバラインCD：{0} / QcParamNO：{1} / CauseModel_NM = {2}";
		private const string MESSAGE_150 = "【エラー遠隔通知】送信メッセージが長すぎます。メッセージ長：{0} / 最大メッセージ長：{1}";
		private const string MESSAGE_151 = "バッチモード動作の為の引数に誤りがあります。引数：{0}";
		private const string MESSAGE_152 = "本番DBの設定がありません。TypeGroup = {0} / LineType = {1}";
		private const string MESSAGE_153 = "【外観検査機 出力異常】検査箇所未設定[変化点フラグ=1(抜き取り検査有り)に対して検査機の検査設定箇所が無し]";
		private const string MESSAGE_154 = "同一の装置：{0}、品目：{1}、QcParamNO：{2}で設備CD指定・無指定の閾値マスタが混在しています。";
        //private const string MESSAGE_155 = "[{0}]が工程狙い値({1})を越えました。取得値={2},閾値{1}={3}";
        private const string MESSAGE_155 = "[{0}/{6}]が工程狙い値({1})を越えました。取得値={2},閾値{1}={3},Lot={4},Linecd={5}";
        private const string MESSAGE_156 = "TmFRAMEのWirebonderFrameOutOrderCDの設定で想定外のCDが設定されています。型番：{0}";
		private const string MESSAGE_157 = "{0}の{1}の処理が未定義です。";
        private const string MESSAGE_158 = "設定ファイルの値が整数型ではありません。 設定項目：{0}";
        private const string MESSAGE_159 = "[{0}/{1}号機][管理番号:{8}/{2}]が管理限界値({3})を越えました。取得値={4},閾値{3}={5},プログラム名={6},Linecd={7}";
        private const string MESSAGE_160 = "[{0}/{1}号機][管理番号:{7}/{2}]の設定値に誤りがあります。取得値={3},閾値={4},{6}={5}";
        private const string MESSAGE_161 = "[{0}/{1}号機][管理番号:{5}/{2}]の閾値が設定されていません。プログラム名={3},Linecd={4}";
        private const string MESSAGE_162 = "[{0}/{1}号機]パラメータ取得用PLCアドレスマスタが設定されていません。";

        #region プロパティ
        #region Message1～50
        public string Message_1
        {
            get { return MESSAGE_1; }
        }
        public string Message_2
        {
            get { return MESSAGE_2; }
        }
        public string Message_3
        {
            get { return MESSAGE_3; }
        }
        public string Message_4
        {
            get { return MESSAGE_4; }
        }
        public string Message_5
        {
            get { return MESSAGE_5; }
        }
        public string Message_6
        {
            get { return MESSAGE_6; }
        }
        public string Message_7
        {
            get { return MESSAGE_7; }
        }
        public string Message_8
        {
            get { return MESSAGE_8; }
        }
        public string Message_9
        {
            get { return MESSAGE_9; }
        }
        public string Message_10
        {
            get { return MESSAGE_10; }
        }
        public string Message_11
        {
            get { return MESSAGE_11; }
        }
        public string Message_12
        {
            get { return MESSAGE_12; }
        }
        public string Message_13
        {
            get { return MESSAGE_13; }
        }
        public string Message_14
        {
            get { return MESSAGE_14; }
        }
        public string Message_15
        {
            get { return MESSAGE_15; }
        }
        public string Message_16
        {
            get { return MESSAGE_16; }
        }
        public string Message_17
        {
            get { return MESSAGE_17; }
        }
        public string Message_18
        {
            get { return MESSAGE_18; }
        }
        public string Message_19
        {
            get { return MESSAGE_19; }
        }
        public string Message_20
        {
            get { return MESSAGE_20; }
        }
        public string Message_21
        {
            get { return MESSAGE_21; }
        }
        public string Message_22
        {
            get { return MESSAGE_22; }
        }
        public string Message_23
        {
            get { return MESSAGE_23; }
        }
        public string Message_24
        {
            get { return MESSAGE_24; }
        }
        public string Message_25
        {
            get { return MESSAGE_25; }
        }
        public string Message_26
        {
            get { return MESSAGE_26; }
        }
        public string Message_27
        {
            get { return MESSAGE_27; }
        }
        public string Message_28
        {
            get { return MESSAGE_28; }
        }
        public string Message_29
        {
            get { return MESSAGE_29; }
        }
        public string Message_30
        {
            get { return MESSAGE_30; }
        }
        public string Message_31
        {
            get { return MESSAGE_31; }
        }
        public string Message_32
        {
            get { return MESSAGE_32; }
        }
        public string Message_33
        {
            get { return MESSAGE_33; }
        }
        public string Message_34
        {
            get { return MESSAGE_34; }
        }
        public string Message_35
        {
            get { return MESSAGE_35; }
        }
        public string Message_36
        {
            get { return MESSAGE_36; }
        }
        public string Message_37
        {
            get { return MESSAGE_37; }
        }
        public string Message_38
        {
            get { return MESSAGE_38; }
        }
        public string Message_39
        {
            get { return MESSAGE_39; }
        }
        public string Message_40
        {
            get { return MESSAGE_40; }
        }
        public string Message_41
        {
            get { return MESSAGE_41; }
        }
        public string Message_42
        {
            get { return MESSAGE_42; }
        }
        public string Message_43
        {
            get { return MESSAGE_43; }
        }
        public string Message_44
        {
            get { return MESSAGE_44; }
        }
        public string Message_45
        {
            get { return MESSAGE_45; }
        }
        public string Message_46
        {
            get { return MESSAGE_46; }
        }
        public string Message_47
        {
            get { return MESSAGE_47; }
        }
        public string Message_48
        {
            get { return MESSAGE_48; }
        }
        public string Message_49
        {
            get { return MESSAGE_49; }
        }
        public string Message_50
        {
            get { return MESSAGE_50; }
		}
		#endregion
		#region Message51～100
		public string Message_51
        {
            get { return MESSAGE_51; }
        }
        public string Message_52
        {
            get { return MESSAGE_52; }
        }
        public string Message_53
        {
            get { return MESSAGE_53; }
        }
        public string Message_54
        {
            get { return MESSAGE_54; }
        }
        public string Message_55
        {
            get { return MESSAGE_55; }
        }
        public string Message_56
        {
            get { return MESSAGE_56; }
        }
        public string Message_57
        {
            get { return MESSAGE_57; }
        }
        public string Message_58
        {
            get { return MESSAGE_58; }
        }
        public string Message_59
        {
            get { return MESSAGE_59; }
        }
        public string Message_60
        {
            get { return MESSAGE_60; }
        }
        public string Message_61
        {
            get { return MESSAGE_61; }
        }
        public string Message_62
        {
            get { return MESSAGE_62; }
        }
        public string Message_63
        {
            get { return MESSAGE_63; }
        }
        public string Message_64
        {
            get { return MESSAGE_64; }
        }
        public string Message_65
        {
            get { return MESSAGE_65; }
        }
        public string Message_66
        {
            get { return MESSAGE_66; }
        }
        public string Message_67
        {
            get { return MESSAGE_67; }
        }
        public string Message_68
        {
            get { return MESSAGE_68; }
        }
        public string Message_69
        {
            get { return MESSAGE_69; }
        }
        public string Message_70
        {
            get { return MESSAGE_70; }
        }
        public string Message_71
        {
            get { return MESSAGE_71; }
        }
        public string Message_72
        {
            get { return MESSAGE_72; }
        }
        public string Message_73
        {
            get { return MESSAGE_73; }
        }
        public string Message_74
        {
            get { return MESSAGE_74; }
        }
        public string Message_75
        {
            get { return MESSAGE_75; }
        }
        public string Message_76
        {
            get { return MESSAGE_76; }
        }
        public string Message_77
        {
            get { return MESSAGE_77; }
        }
        public string Message_78
        {
            get { return MESSAGE_78; }
        }
        public string Message_79
        {
            get { return MESSAGE_79; }
        }
        public string Message_80
        {
            get { return MESSAGE_80; }
        }
        public string Message_81
        {
            get { return MESSAGE_81; }
        }
        public string Message_82
        {
            get { return MESSAGE_82; }
        }
        public string Message_83
        {
            get { return MESSAGE_83; }
        }
        public string Message_84
        {
            get { return MESSAGE_84; }
        }
        public string Message_85
        {
            get { return MESSAGE_85; }
        }
        public string Message_86
        {
            get { return MESSAGE_86; }
        }
        public string Message_87
        {
            get { return MESSAGE_87; }
        }
        public string Message_88
        {
            get { return MESSAGE_88; }
        }
        public string Message_89
        {
            get { return MESSAGE_89; }
        }
		public string Message_90
		{
			get { return MESSAGE_90; }
		}
		public string Message_91
		{
			get { return MESSAGE_91; }
		}
		public string Message_92
		{
			get { return MESSAGE_92; }
		}
		public string Message_93
		{
			get { return MESSAGE_93; }
		}
		public string Message_94
		{
			get { return MESSAGE_94; }
		}
		public string Message_95
		{
			get { return MESSAGE_95; }
		}
		public string Message_96
		{
			get { return MESSAGE_96; }
		}
		public string Message_97
		{
			get { return MESSAGE_97; }
		}
		public string Message_98
		{
			get { return MESSAGE_98; }
		}
		public string Message_99
		{
			get { return MESSAGE_99; }
		}
		public string Message_100
		{
			get { return MESSAGE_100; }
		}
		#endregion
		#region Message101～150
		public string Message_101
		{
			get { return MESSAGE_101; }
		}
		public string Message_102
		{
			get { return MESSAGE_102; }
		}
		public string Message_103
		{
			get { return MESSAGE_103; }
		}
		public string Message_104
		{
			get { return MESSAGE_104; }
		}
		public string Message_105
		{
			get { return MESSAGE_105; }
		}
		public string Message_106
		{
			get { return MESSAGE_106; }
		}
		public string Message_107
		{
			get { return MESSAGE_107; }
		}
		public string Message_108
		{
			get { return MESSAGE_108; }
		}
        public string Message_109
        {
            get { return MESSAGE_109; }
        }
        public string Message_110
        {
            get { return MESSAGE_110; }
        }
        public string Message_111
        {
            get { return MESSAGE_111; }
        }
        public string Message_112
        {
            get { return MESSAGE_112; }
        }
		public string Message_113
		{
			get { return MESSAGE_113; }
		}
		public string Message_114
		{
			get { return MESSAGE_114; }
		}
		public string Message_115
		{
			get { return MESSAGE_115; }
		}
		public string Message_116
		{
			get { return MESSAGE_116; }
		}
		public string Message_117
		{
			get { return MESSAGE_117; }
		}
		public string Message_118
		{
			get { return MESSAGE_118; }
		}
        public string Message_119
        {
            get { return MESSAGE_119; }
        }
        public string Message_120
        {
            get { return MESSAGE_120; }
        }
		public string Message_121
		{
			get { return MESSAGE_121; }
		}
		public string Message_122
        {
            get { return MESSAGE_122; }
        }
        public string Message_123
        {
            get { return MESSAGE_123; }
        }
        public string Message_124
        {
            get { return MESSAGE_124; }
        }
        public string Message_125
        {
            get { return MESSAGE_125; }
        }
		public string Message_126
		{
			get { return MESSAGE_126; }
		}
		public string Message_127
		{
			get { return MESSAGE_127; }
		}
		public string Message_128
		{
			get { return MESSAGE_128; }
		}
        public string Message_129
        {
            get { return MESSAGE_129; }
        }
        public string Message_130
        {
            get { return MESSAGE_130; }
        }
        public string Message_131
        {
            get { return MESSAGE_131; }
        }
        public string Message_132
        {
            get { return MESSAGE_132; }
        }
		public string Message_133
		{
			get { return MESSAGE_133; }
		}
		public string Message_134
		{
			get { return MESSAGE_134; }
		}
		public string Message_135
		{
			get { return MESSAGE_135; }
		}
		public string Message_136
		{
			get { return MESSAGE_136; }
		}
		public string Message_137
		{
			get { return MESSAGE_137; }
		}
		public string Message_138
		{
			get { return MESSAGE_138; }
		}
		public string Message_139
		{
			get { return MESSAGE_139; }
		}
		public string Message_140
		{
			get { return MESSAGE_140; }
		}
		public string Message_141
		{
			get { return MESSAGE_141; }
		}
		public string Message_142
		{
			get { return MESSAGE_142; }
		}
		public string Message_143
		{
			get { return MESSAGE_143; }
		}
		public string Message_144
		{
			get { return MESSAGE_144; }
		}
		public string Message_145
		{
			get { return MESSAGE_145; }
		}
		public string Message_146
		{
			get { return MESSAGE_146; }
		}
		public string Message_147
		{
			get { return MESSAGE_147; }
		}
		public string Message_148
		{
			get { return MESSAGE_148; }
		}
		public string Message_149
		{
			get { return MESSAGE_149; }
		}
		public string Message_150
		{
			get { return MESSAGE_150; }
		}
		#endregion
		#region Message151～200
		public string Message_151
		{
			get { return MESSAGE_151; }
		}
		public string Message_152
		{
			get { return MESSAGE_152; }
		}
		public string Message_153
		{
			get { return MESSAGE_153; }
		}
		public string Message_154
		{
			get { return MESSAGE_154; }
		}
        public string Message_155
        {
            get { return MESSAGE_155; }
        }
        public string Message_156
        {
            get { return MESSAGE_156; }
        }
		public string Message_157
		{
			get { return MESSAGE_157; }
		}
        public string Message_158
        {
            get { return MESSAGE_158; }
        }
        public string Message_159
        {
            get { return MESSAGE_159; }
        }
        public string Message_160
        {
            get { return MESSAGE_160; }
        }
        public string Message_161
        {
            get { return MESSAGE_161; }
        }
        public string Message_162
        {
            get { return MESSAGE_162; }
        }

        #endregion
        #endregion
    }

    public class MessageENInfo : IMessage
    {
        private const string MESSAGE_1 = "Operation will be started for all machines. OK?";
        private const string MESSAGE_2 = "Operation will be stopped for all machines. OK?";
        private const string MESSAGE_3 = "test2";
        private const string MESSAGE_4 = "test2";
        private const string MESSAGE_5 = "test2";
        private const string MESSAGE_6 = "Kiss is not worked. Boot Kiss and set condition immediately.";
        private const string MESSAGE_7 = "There is not {0} in threshold master.";
        private const string MESSAGE_8 = "It completed to correct it,";
        private const string MESSAGE_9 = "It has not selected yet.";
        private const string MESSAGE_10 = "There is not file.";
        private const string MESSAGE_11 = "The file can be read only.";
        private const string MESSAGE_12 = "Execute it after passes for a while.";
        private const string MESSAGE_13 = "{0} has not set yet.";
        private const string MESSAGE_14 = "Black Jumbo Dog is not worked. Boot BlackJumboDog and set immediately.";
        private const string MESSAGE_15 = "[PlantCD{0}]Operation is stopped, because the exception was occurred.";
        private const string MESSAGE_16 = "PlantCD:";
        private const string MESSAGE_17 = "It has already operated. Therefore, it can not operate to prevent a multiple operation,";
        private const string MESSAGE_18 = "The application is finished. OK?";
        private const string MESSAGE_19 = "Operation is completed. Push the button after passes for a while.";
        private const string MESSAGE_20 = "Set the product type.";
        private const string MESSAGE_21 = "Set the chip's (dice's) type.";
        //private const string MESSAGE_22 = "[{0}/{1} machine No][{2}] is over management limit {3}.  \r\n Acquisition value={4} Threshold{3}={5}.";
        //private const string MESSAGE_23 = "[{0}/{1} machine No][{2}] is Setting value wrong. Acquisition value={3}  Threshold={4}.";
        private const string MESSAGE_22 = "[{0}/{1} machine No][{2}] is over management limit {3}. Acquisition value={4},Threshold{3}={5},Lot={6},Linecd={7}";
        private const string MESSAGE_23 = "[{0}/{1} machine No][{2}] is Setting value wrong. Acquisition value={3},Threshold={4},Lot={5},Linecd={6}";
        private const string MESSAGE_24 = "[Equipment no{0}/{1}]Investment is not executed or time is difference.";
        private const string MESSAGE_25 = "[{0}/{1} machine No]There are 20 or more files, check the condition.";
        private const string MESSAGE_26 = @"[{0}/{1} machine No][Pick Level] Difference is big, refer to {2}. There is a possibility that ""Collet"" or ""Stamp pin"" or ""Eject pin"" has damage. Check these parts condition.";
        private const string MESSAGE_27 = @"[Type {0}/File {1}]There is not in content conditional master.";
        private const string MESSAGE_28 = "[Type {0}/qcparam No {1}/parameter Name {2}]There is not in threshold master.";
        private const string MESSAGE_29 = @"[{0}/{1} machine No][Bond Level] Difference is big, refer to {2}. There is a possibility that ""Collet"" or ""Stamp pin"" or ""Eject pin"" has damage. Check these parts condition.";
        private const string MESSAGE_30 = "Delete it from the list. OK? \r\n{0}";
        private const string MESSAGE_31 = "Select the line.";
        private const string MESSAGE_32 = "There is a mistake at input content.";
        private const string MESSAGE_33 = "Registration is completed";
        private const string MESSAGE_34 = "There is not lot information in ARMS. Input product type by keyboard.";
        private const string MESSAGE_35 = "failed in the acquisition of the machine list.please confirm the master.";
        private const string MESSAGE_36 = "The start file is not being set to the master.";
        private const string MESSAGE_37 = "Failed in the change of the start file name that had been processed.";
        private const string MESSAGE_38 = "[{0}/{1}machine No.]failed in the file processing.";
        private const string MESSAGE_39 = "The observation folder is not found. File path {0}";
        private const string MESSAGE_40 = "[{0}/{1}machine No./Type{2}]The value of mounted die doesn't exist in master.。";
        private const string MESSAGE_41 = "";
        private const string MESSAGE_42 = "[{0}/{1} machine No]";
        private const string MESSAGE_43 = "[{0}/{1}]Failed in the change of the flame supply state.please confirm the power of PLC.";
        private const string MESSAGE_44 = "File is deleted, because the file size is 0KB. path:{0}";
        private const string MESSAGE_45 = "There is a problem in the content. Please contact to maintenance group of the production。File path:{0} 行:{1}";
        private const string MESSAGE_46 = "file is deleted, because it had been  registered already. File path:{0}";
        private const string MESSAGE_47 = "It is timing that doesn't exist in the schedule. Please contact to manager of systems. File path:{0}";
        private const string MESSAGE_48 = "Processing doesn't exist. please contact to manager of systems.File path:{0}";
        private const string MESSAGE_49 = "Machine";
        private const string MESSAGE_50 = "It was over a time limit of the file operation. There is a possibility of acting as Locked of the file. File path:{0}";
        private const string MESSAGE_51 = "作成途中のファイルが存在します。ファイルパス：{0}";
        private const string MESSAGE_52 = "{3} file is not equal to a folder during processing. Magazine serial number: {0} You go to the applicable machine, and please move a file from Original place to the place that moves. Original place:{1} ,The place that moves:{2}";
        private const string MESSAGE_53 = @"C:\AD830A\temp";
        private const string MESSAGE_54 = "[ロット番号 {0}]マッピングファイルが見つかりませんでした。フォルダパス:{1} ";
        private const string MESSAGE_55 = "[ロット番号 {0}]ARMSにロット情報が存在しません。システム担当者に連絡して下さい。";
        private const string MESSAGE_56 = "未マッピング時のマッピングフラグOFF ロットが指定されていません。";
        private const string MESSAGE_57 = "[ロット番号 {0}]検査機対象外です。";
        private const string MESSAGE_58 = "結合するマッピングデータの数が一致しません";
        private const string MESSAGE_59 = "マッピングデータの書き込みに失敗しました。";
        private const string MESSAGE_60 = "[型番 {0}]型番の書き込みに失敗しました。";
        private const string MESSAGE_61 = "[処理CD {0}]不正な処理CDが含まれています。";
        private const string MESSAGE_62 = "[ロット番号 {0}]マッピングファイルが存在しない為、全数検査を行います。";
        private const string MESSAGE_63 = "MMファイル内容のアドレスが不足しています。不足アドレスには検査を代入しました。";
        private const string MESSAGE_64 = "[管理番号 {0}/管理項目名 {1}]チップ区分が不適切です。";
        private const string MESSAGE_65 = "";
        private const string MESSAGE_66 = "";
        private const string MESSAGE_67 = "";
        private const string MESSAGE_68 = "";
        private const string MESSAGE_69 = "";
        private const string MESSAGE_70 = "";
        private const string MESSAGE_71 = "";
        private const string MESSAGE_72 = "";
        private const string MESSAGE_73 = "";
        private const string MESSAGE_74 = "";
        private const string MESSAGE_75 = "";
        private const string MESSAGE_76 = "";
        private const string MESSAGE_77 = "";
        private const string MESSAGE_78 = "";
        private const string MESSAGE_79 = "";
        private const string MESSAGE_80 = "";
        private const string MESSAGE_81 = "";
        private const string MESSAGE_82 = "";
        private const string MESSAGE_83 = "";
        private const string MESSAGE_84 = "";
        private const string MESSAGE_85 = "";
        private const string MESSAGE_86 = "";
        private const string MESSAGE_87 = "";
        private const string MESSAGE_88 = "";
        private const string MESSAGE_89 = "";
		private const string MESSAGE_90 = "";
		private const string MESSAGE_91 = "";
		private const string MESSAGE_92 = "";
		private const string MESSAGE_93 = "";
		private const string MESSAGE_94 = "";
		private const string MESSAGE_95 = "";
		private const string MESSAGE_96 = "";
		private const string MESSAGE_97 = "";
		private const string MESSAGE_98 = "";
		private const string MESSAGE_99 = "";
		private const string MESSAGE_100 = "";
		private const string MESSAGE_101 = "";
		private const string MESSAGE_102 = "";
		private const string MESSAGE_103 = "";
		private const string MESSAGE_104 = "";
		private const string MESSAGE_105 = "";
		private const string MESSAGE_106 = "";
		private const string MESSAGE_107 = "";
		private const string MESSAGE_108 = "";
        private const string MESSAGE_109 = "";
        private const string MESSAGE_110 = "";
        private const string MESSAGE_111 = "";
        private const string MESSAGE_112 = "";
		private const string MESSAGE_113 = "";
		private const string MESSAGE_114 = "";
		private const string MESSAGE_115 = "";
		private const string MESSAGE_116 = "";
		private const string MESSAGE_117 = "";
		private const string MESSAGE_118 = "";
        private const string MESSAGE_119 = "";
        private const string MESSAGE_120 = "";
		private const string MESSAGE_121 = "";
        private const string MESSAGE_122 = "";
        private const string MESSAGE_123 = "";
        private const string MESSAGE_124 = "";
        private const string MESSAGE_125 = "";
		private const string MESSAGE_126 = "";
		private const string MESSAGE_127 = "";
		private const string MESSAGE_128 = "";
        private const string MESSAGE_129 = "";
        private const string MESSAGE_130 = "";
        private const string MESSAGE_131 = "";
        private const string MESSAGE_132 = "";
        private const string MESSAGE_133 = "";
        private const string MESSAGE_134 = "";
        private const string MESSAGE_135 = "";
        private const string MESSAGE_136 = "";
		private const string MESSAGE_137 = "";
		private const string MESSAGE_138 = "";
		private const string MESSAGE_139 = "";
		private const string MESSAGE_140 = "";
		private const string MESSAGE_141 = "";
		private const string MESSAGE_142 = "";
		private const string MESSAGE_143 = "";
		private const string MESSAGE_144 = "";
		private const string MESSAGE_145 = "";
		private const string MESSAGE_146 = "";
		private const string MESSAGE_147 = "";
		private const string MESSAGE_148 = "";
		private const string MESSAGE_149 = "";
		private const string MESSAGE_150 = "";
		private const string MESSAGE_151 = "";
		private const string MESSAGE_152 = "";
		private const string MESSAGE_153 = "";
		private const string MESSAGE_154 = "";
        private const string MESSAGE_155 = "[{0}]が工程狙い値({1})を越えました。取得値={2},閾値{1}={3},Lot={4},Linecd={5}";
        private const string MESSAGE_156 = "TmFRAMEのWirebonderFrameOutOrderCDの設定で想定外のCDが設定されています。型番：{0}";
        private const string MESSAGE_157 = "{0}の{1}の処理が未定義です。";
        private const string MESSAGE_158 = "";
        private const string MESSAGE_159 = "";
        private const string MESSAGE_160 = "";
        private const string MESSAGE_161 = "";
        private const string MESSAGE_162 = "";


        #region プロパティ

        #region Message1～100
        public string Message_1
        {
            get { return MESSAGE_1; }
        }

        public string Message_2
        {
            get { return MESSAGE_2; }
        }

        public string Message_3
        {
            get { return MESSAGE_3; }
        }

        public string Message_4
        {
            get { return MESSAGE_4; }
        }

        public string Message_5
        {
            get { return MESSAGE_5; }
        }
        public string Message_6
        {
            get { return MESSAGE_6; }
        }
        public string Message_7
        {
            get { return MESSAGE_7; }
        }
        public string Message_8
        {
            get { return MESSAGE_8; }
        }
        public string Message_9
        {
            get { return MESSAGE_9; }
        }
        public string Message_10
        {
            get { return MESSAGE_10; }
        }
        public string Message_11
        {
            get { return MESSAGE_11; }
        }
        public string Message_12
        {
            get { return MESSAGE_12; }
        }
        public string Message_13
        {
            get { return MESSAGE_13; }
        }
        public string Message_14
        {
            get { return MESSAGE_14; }
        }
        public string Message_15
        {
            get { return MESSAGE_15; }
        }
        public string Message_16
        {
            get { return MESSAGE_16; }
        }
        public string Message_17
        {
            get { return MESSAGE_17; }
        }
        public string Message_18
        {
            get { return MESSAGE_18; }
        }
        public string Message_19
        {
            get { return MESSAGE_19; }
        }
        public string Message_20
        {
            get { return MESSAGE_20; }
        }
        public string Message_21
        {
            get { return MESSAGE_21; }
        }
        public string Message_22
        {
            get { return MESSAGE_22; }
        }
        public string Message_23
        {
            get { return MESSAGE_23; }
        }
        public string Message_24
        {
            get { return MESSAGE_24; }
        }
        public string Message_25
        {
            get { return MESSAGE_25; }
        }
        public string Message_26
        {
            get { return MESSAGE_26; }
        }
        public string Message_27
        {
            get { return MESSAGE_27; }
        }
        public string Message_28
        {
            get { return MESSAGE_28; }
        }
        public string Message_29
        {
            get { return MESSAGE_29; }
        }
        public string Message_30
        {
            get { return MESSAGE_30; }
        }
        public string Message_31
        {
            get { return MESSAGE_31; }
        }
        public string Message_32
        {
            get { return MESSAGE_32; }
        }
        public string Message_33
        {
            get { return MESSAGE_33; }
        }
        public string Message_34
        {
            get { return MESSAGE_34; }
        }
        public string Message_35
        {
            get { return MESSAGE_35; }
        }
        public string Message_36
        {
            get { return MESSAGE_36; }
        }
        public string Message_37
        {
            get { return MESSAGE_37; }
        }
        public string Message_38
        {
            get { return MESSAGE_38; }
        }
        public string Message_39
        {
            get { return MESSAGE_39; }
        }
        public string Message_40
        {
            get { return MESSAGE_40; }
        }
        public string Message_41
        {
            get { return MESSAGE_41; }
        }
        public string Message_42
        {
            get { return MESSAGE_42; }
        }
        public string Message_43
        {
            get { return MESSAGE_43; }
        }
        public string Message_44
        {
            get { return MESSAGE_44; }
        }
        public string Message_45
        {
            get { return MESSAGE_45; }
        }
        public string Message_46
        {
            get { return MESSAGE_46; }
        }
        public string Message_47
        {
            get { return MESSAGE_47; }
        }
        public string Message_48
        {
            get { return MESSAGE_48; }
        }
        public string Message_49
        {
            get { return MESSAGE_49; }
        }
        public string Message_50
        {
            get { return MESSAGE_50; }
        }
        public string Message_51
        {
            get { return MESSAGE_51; }
        }
        public string Message_52
        {
            get { return MESSAGE_52; }
        }
        public string Message_53
        {
            get { return MESSAGE_53; }
        }
        public string Message_54
        {
            get { return MESSAGE_54; }
        }
        public string Message_55
        {
            get { return MESSAGE_55; }
        }
        public string Message_56
        {
            get { return MESSAGE_56; }
        }
        public string Message_57
        {
            get { return MESSAGE_57; }
        }
        public string Message_58
        {
            get { return MESSAGE_58; }
        }
        public string Message_59
        {
            get { return MESSAGE_59; }
        }
        public string Message_60
        {
            get { return MESSAGE_60; }
        }
        public string Message_61
        {
            get { return MESSAGE_61; }
        }
        public string Message_62
        {
            get { return MESSAGE_62; }
        }
        public string Message_63
        {
            get { return MESSAGE_63; }
        }
        public string Message_64
        {
            get { return MESSAGE_64; }
        }
        public string Message_65
        {
            get { return MESSAGE_65; }
        }
        public string Message_66
        {
            get { return MESSAGE_66; }
        }
        public string Message_67
        {
            get { return MESSAGE_67; }
        }
        public string Message_68
        {
            get { return MESSAGE_68; }
        }
        public string Message_69
        {
            get { return MESSAGE_69; }
        }
        public string Message_70
        {
            get { return MESSAGE_70; }
        }
        public string Message_71
        {
            get { return MESSAGE_71; }
        }
        public string Message_72
        {
            get { return MESSAGE_72; }
        }
        public string Message_73
        {
            get { return MESSAGE_73; }
        }
        public string Message_74
        {
            get { return MESSAGE_74; }
        }
        public string Message_75
        {
            get { return MESSAGE_75; }
        }
        public string Message_76
        {
            get { return MESSAGE_76; }
        }
        public string Message_77
        {
            get { return MESSAGE_77; }
        }
        public string Message_78
        {
            get { return MESSAGE_78; }
        }
        public string Message_79
        {
            get { return MESSAGE_79; }
        }
        public string Message_80
        {
            get { return MESSAGE_80; }
        }
        public string Message_81
        {
            get { return MESSAGE_81; }
        }
        public string Message_82
        {
            get { return MESSAGE_82; }
        }
        public string Message_83
        {
            get { return MESSAGE_83; }
        }
        public string Message_84
        {
            get { return MESSAGE_84; }
        }
        public string Message_85
        {
            get { return MESSAGE_85; }
        }
        public string Message_86
        {
            get { return MESSAGE_86; }
        }
        public string Message_87
        {
            get { return MESSAGE_87; }
        }
        public string Message_88
        {
            get { return MESSAGE_88; }
        }
        public string Message_89
        {
            get { return MESSAGE_89; }
        }
		public string Message_90
		{
			get { return MESSAGE_90; }
		}
		public string Message_91
		{
			get { return MESSAGE_91; }
		}
		public string Message_92
		{
			get { return MESSAGE_92; }
		}
		public string Message_93
		{
			get { return MESSAGE_93; }
		}
		public string Message_94
		{
			get { return MESSAGE_94; }
		}
		public string Message_95
		{
			get { return MESSAGE_95; }
		}
		public string Message_96
		{
			get { return MESSAGE_96; }
		}
		public string Message_97
		{
			get { return MESSAGE_97; }
		}
		public string Message_98
		{
			get { return MESSAGE_98; }
		}
		public string Message_99
		{
			get { return MESSAGE_99; }
		}
		public string Message_100
		{
			get { return MESSAGE_100; }
		}
		#endregion
		#region Message101～200
		public string Message_101
		{
			get { return MESSAGE_101; }
		}
		public string Message_102
		{
			get { return MESSAGE_102; }
		}
		public string Message_103
		{
			get { return MESSAGE_103; }
		}
		public string Message_104
		{
			get { return MESSAGE_104; }
		}
		public string Message_105
		{
			get { return MESSAGE_105; }
		}
		public string Message_106
		{
			get { return MESSAGE_106; }
		}
		public string Message_107
		{
			get { return MESSAGE_107; }
		}
		public string Message_108
		{
			get { return MESSAGE_108; }
		}
        public string Message_109
        {
            get { return MESSAGE_109; }
        }
        public string Message_110
        {
            get { return MESSAGE_110; }
        }
        public string Message_111
        {
            get { return MESSAGE_111; }
        }
        public string Message_112
        {
            get { return MESSAGE_112; }
        }
		public string Message_113
		{
			get { return MESSAGE_113; }
		}
		public string Message_114
		{
			get { return MESSAGE_114; }
		}
		public string Message_115
		{
			get { return MESSAGE_115; }
		}
		public string Message_116
		{
			get { return MESSAGE_116; }
		}
		public string Message_117
		{
			get { return MESSAGE_117; }
		}
		public string Message_118
		{
			get { return MESSAGE_118; }
		}
        public string Message_119
        {
            get { return MESSAGE_119; }
        }
        public string Message_120
        {
            get { return MESSAGE_120; }
        }
		public string Message_121
		{
			get { return MESSAGE_121; }
		}
        public string Message_122
        {
            get { return MESSAGE_122; }
        }
        public string Message_123
        {
            get { return MESSAGE_123; }
        }
        public string Message_124
        {
            get { return MESSAGE_124; }
        }
        public string Message_125
        {
            get { return MESSAGE_125; }
        }
		public string Message_126
		{
			get { return MESSAGE_126; }
		}
		public string Message_127
		{
			get { return MESSAGE_127; }
		}
		public string Message_128
		{
			get { return MESSAGE_128; }
		}
        public string Message_129
        {
            get { return MESSAGE_129; }
        }
        public string Message_130
        {
            get { return MESSAGE_130; }
        }
        public string Message_131
        {
            get { return MESSAGE_131; }
        }
        public string Message_132
        {
            get { return MESSAGE_132; }
        }
        public string Message_133
        {
            get { return MESSAGE_133; }
        }
        public string Message_134
        {
            get { return MESSAGE_134; }
        }
        public string Message_135
        {
            get { return MESSAGE_135; }
        }
        public string Message_136
        {
            get { return MESSAGE_136; }
        }
		public string Message_137
		{
			get { return MESSAGE_137; }
		}
		public string Message_138
		{
			get { return MESSAGE_138; }
		}
		public string Message_139
		{
			get { return MESSAGE_139; }
		}
		public string Message_140
		{
			get { return MESSAGE_140; }
		}
		public string Message_141
		{
			get { return MESSAGE_141; }
		}
		public string Message_142
		{
			get { return MESSAGE_142; }
		}
		public string Message_143
		{
			get { return MESSAGE_143; }
		}
		public string Message_144
		{
			get { return MESSAGE_144; }
		}
		public string Message_145
		{
			get { return MESSAGE_145; }
		}
		public string Message_146
		{
			get { return MESSAGE_146; }
		}
		public string Message_147
		{
			get { return MESSAGE_147; }
		}
		public string Message_148
		{
			get { return MESSAGE_148; }
		}
		public string Message_149
		{
			get { return MESSAGE_149; }
		}
		public string Message_150
		{
			get { return MESSAGE_150; }
		}
		public string Message_151
		{
			get { return MESSAGE_151; }
		}
		public string Message_152
		{
			get { return MESSAGE_152; }
		}
		public string Message_153
		{
			get { return MESSAGE_153; }
		}
		public string Message_154
		{
			get { return MESSAGE_154; }
		}
        public string Message_155
        {
            get { return MESSAGE_155; }
        }
        public string Message_156
        {
            get { return MESSAGE_156; }
        }
		public string Message_157
		{
			get { return MESSAGE_157; }
		}
        public string Message_158
        {
            get { return MESSAGE_158; }
        }
        public string Message_159
        {
            get { return MESSAGE_159; }
        }
        public string Message_160
        {
            get { return MESSAGE_160; }
        }
        public string Message_161
        {
            get { return MESSAGE_161; }
        }
        public string Message_162
        {
            get { return MESSAGE_162; }
        }

        #endregion

        #endregion
    }
}
