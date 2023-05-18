using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS
{
    /// <summary>
    /// メッセージ情報
    /// </summary>
    public class MessageJAInfo : IMessage
    {
        private const string MESSAGE_1 = "ユーザ名またはパスワードが入力されていません。";
        private const string MESSAGE_2 = "ユーザ認証に失敗しました。";
        private const string MESSAGE_3 = "ユーザ名またはパスワードに誤りがあります。";
        private const string MESSAGE_4 = "必須入力項目が入力されていません。";
        private const string MESSAGE_5 = "データを上書きします。よろしいですか？";
        private const string MESSAGE_6 = "同じデータが既にデータベースに登録されています。";
        private const string MESSAGE_7 = "登録に失敗しました。";
        private const string MESSAGE_8 = "登録完了しました。";
        private const string MESSAGE_9 = "対象データがありません。";
        private const string MESSAGE_10 = "複数選択されています。1つに絞ってください。";
        private const string MESSAGE_11 = "選択行を削除します。よろしいですか？";
        private const string MESSAGE_12 = "削除に失敗しました。";
        private const string MESSAGE_13 = "削除完了しました。";
        private const string MESSAGE_14 = "条件に一致するデータはありません。";
        private const string MESSAGE_15 = "選択行を承認登録します。よろしいですか？";
        private const string MESSAGE_16 = "承認登録に失敗しました。";
        private const string MESSAGE_17 = "承認登録完了しました。";
        private const string MESSAGE_18 = "が見つかりませんでした。";
        private const string MESSAGE_19 = "データの出力に失敗しました。";
        private const string MESSAGE_20 = "所属工程が見つかりませんでした。";
        private const string MESSAGE_21 = "作業者を選択してください。";
        private const string MESSAGE_22 = "合格日を全て入力してください。";
        private const string MESSAGE_23 = "既に様式番号は登録されています。変更しますか？";
        private const string MESSAGE_24 = "文書種類を選択してください。";
        private const string MESSAGE_25 = "表示されている内容を保存します。よろしいですか？";
        private const string MESSAGE_26 = "係がマスタに登録されていない名称です。";
        private const string MESSAGE_27 = "品種がマスタに登録されていない名称です。";
        private const string MESSAGE_28 = "工程がマスタに登録されていない名称です。";
        private const string MESSAGE_29 = "見直し周期がマスタに登録されていない名称です。";
        private const string MESSAGE_30 = "仮登録が完了しました。";
        private const string MESSAGE_31 = "仮登録に失敗しました。";
        private const string MESSAGE_32 = "NASCA作業名がマスタに登録されていない名称です。";
        private const string MESSAGE_33 = "係を選択してください。";
        private const string MESSAGE_34 = "様式番号を選択してください。";
        private const string MESSAGE_35 = "データベースの内容を上書きします。よろしいですか？";
        private const string MESSAGE_36 = "選択行の仮登録を取り消します。よろしいですか？";
        private const string MESSAGE_37 = "承認回覧中のスキルマップのみ取り消し可能です。";
        private const string MESSAGE_38 = "初版のスキルマップは取り消し不可能です。";
        private const string MESSAGE_39 = "取り消しに失敗しました。";
        private const string MESSAGE_40 = "取り消し完了しました。";
        private const string MESSAGE_41 = "教育者と作業者に同一人物の選択はできません。";
        private const string MESSAGE_42 = "選択行を登録します。よろしいですか？";
        private const string MESSAGE_43 = "既に登録済みです：";
        private const string MESSAGE_44 = "製品型番が選択されていません。";
        private const string MESSAGE_45 = "失敗しました。もう一度取得して下さい。\r\n\r\n [詳細]=";
        private const string MESSAGE_46 = "インライン番号からインライン文字列を取得出来ません。\r\n 「QCIL.xml」と「マスタTmLINE(接続先：{0})」を確認して下さい。";
        private const string MESSAGE_47 = "エラーリストの取得に失敗しました。もう一度取得して下さい。\r\n\r\n [詳細]=";
        private const string MESSAGE_48 = "抽出が完了しました。";
        private const string MESSAGE_49 = "ライン/設備/設備番号/Typeの何れかが未選択です。";
        private const string MESSAGE_50 = "「C:\\QCIL\\SettingFiles\\QCIL.xml」にライン構成装置Noが正しく設定出来ていません。\r\nシステム担当者へ連絡して下さい。";
        private const string MESSAGE_51 = "対応内容と対応者を入力して下さい。";
        private const string MESSAGE_52 = "対応内容は半角400文字/全角200文字以下にして下さい。";
        private const string MESSAGE_53 = "対応者は半角32文字/全角16文字以下にして下さい。";
        private const string MESSAGE_54 = "[装置No][期間]の何れかが正しく入力されていません。\n正しく入力し直してから、再度実行して下さい。";
        private const string MESSAGE_55 = "変更行に誤りがある為、データを登録出来ません。";
        private const string MESSAGE_56 = "登録するデータがありません。登録を中止します。";
        private const string MESSAGE_57 = "TmQsubから[QcParam_NO]={0}でInspection_NOを取得出来ませんでした。";
        private const string MESSAGE_58 = "TmQsubから[QcParamNo]の取得に失敗しました。\r\n\r\n [詳細]=";
        private const string MESSAGE_59 = "QcParamNoの取得に失敗しました。\r\n\r\n [詳細]=";
        private const string MESSAGE_60 = "Model_NMの取得に失敗しました。\r\n\r\n [詳細]=";
        private const string MESSAGE_61 = "実行中です。暫く経ってから再度行って下さい。";
        private const string MESSAGE_62 = "この処理は時間が掛かります。処理を続けてよろしいですか？";
        private const string MESSAGE_63 = "処理が完了しました。";
        private const string MESSAGE_64 = "31日を越えるデータは出力出来ません。";
        private const string MESSAGE_65 = "チップ数量に正しい数値を入力して下さい。";
        private const string MESSAGE_66 = "社員番号または社員名を選択して下さい。";
        private const string MESSAGE_67 = "指定した条件の社員は存在しません。";
        private const string MESSAGE_68 = "指定バッチが見つかりません。";
        private const string MESSAGE_69 = "選択中の権限を追加します。よろしいですか？";
        private const string MESSAGE_70 = "選択中の保持権限を削除します。よろしいですか？";
        private const string MESSAGE_71 = "装置分類の取得に失敗しました。";
        private const string MESSAGE_72 = "検索条件に一致する定型文が存在しませんでした。";
        private const string MESSAGE_73 = "必要なデータが欠けています。";
        private const string MESSAGE_74 = "機能の取得に失敗しました。";
        private const string MESSAGE_75 = "保存が完了しました。";
        private const string MESSAGE_76 = "(書き込み権限)";
        private const string MESSAGE_77 = "(読み取り権限)";
        private const string MESSAGE_78 = "(クライアント)";
        private const string MESSAGE_79 = "";
        private const string MESSAGE_80 = "未設定";
        private const string MESSAGE_81 = "対象フォルダが見つかりませんでした。フォルダパス:{0}";
        private const string MESSAGE_82 = "対象ファイルが見つかりませんでした。フォルダパス:{0}";
        private const string MESSAGE_83 = "対象ファイルが複数存在します。フォルダパス:{0}";
        private const string MESSAGE_84 = "指定されたロット番号の型番が見つかりませんでした。ロット番号:{0}";
        private const string MESSAGE_85 = "設定済";
        private const string MESSAGE_86 = "管理NOには数値を入力して下さい。";
        private const string MESSAGE_87 = "選択した項目はすでにリストに存在します。";
        private const string MESSAGE_88 = "更新処理をするにはキーの情報が不適切です。型式:{0} 管理NO:{1} 製品型番:{2} ";
        private const string MESSAGE_89 = "更新処理をするにはキーの情報が不適切です。製品型番:{0} 管理グループNO:{1} 管理種類NO:{2} ";
        private const string MESSAGE_90 = "関連付く監視項目が登録されていません。管理NO：{0} ";
        private const string MESSAGE_91 = "変更理由が入力されていません。管理NO：{0} ";
        private const string MESSAGE_92 = "更新処理をするにはキーの情報が不適切です。製品型番:{0} ";
        private const string MESSAGE_93 = "不良項目名がマスタに登録されていません。不良CD：{0}";
        private const string MESSAGE_94 = "ラインが選択されていません。";
        private const string MESSAGE_95 = "転送対象が選択されていません。";
        private const string MESSAGE_96 = "登録対象が選択されていません。";
		private const string MESSAGE_97 = "参照元データが選択されていません。";
		private const string MESSAGE_98 = "設定ファイルのLineTypeが不正です。設定値：{0}";
		private const string MESSAGE_99 = "設定ファイルの設定値が数値に変換出来ません。設定値：{0}";
		private const string MESSAGE_100 = "コピー内容の形式に誤りがあります。";
		private const string MESSAGE_101 = "TmPRM/TmPRMInfo/TmTIM いずれかにレコードが存在しません。パラメータ名：{0} / QcParamNO：{1}";
        private const string MESSAGE_102 = "[汎用グループ番号 {0}]汎用情報がマスタに登録されていません。";
        private const string MESSAGE_103 = "同型番で「チップ搭載数/装置」の値が違うレコードがあります。";
        private const string MESSAGE_104 = "異なる型番が選択されているため参照登録できません。";
        private const string MESSAGE_105 = "(内規値編集権限)";
        private const string MESSAGE_106 = "更新処理をするにはキーの情報が不適切です。製品型番:{0} 管理NO:{1} ";
        private const string MESSAGE_107 = "(蛍光体シート閾値読み取り専用)";
        private const string MESSAGE_108 = "(蛍光体シート閾値編集)";

        #region プロパティ

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

        #endregion
    }

    public class MessageENInfo : IMessage
    {
        private const string MESSAGE_1 = "User name or Password is not input.";
        private const string MESSAGE_2 = "It failed to certify as user.";
        private const string MESSAGE_3 = "User name or Password is wrong. ";
        private const string MESSAGE_4 = "The essential content is not input.";
        private const string MESSAGE_5 = "Update the data. OK?";
        private const string MESSAGE_6 = "Same data has already registered in database.";
        private const string MESSAGE_7 = "It failed registration. ";
        private const string MESSAGE_8 = "Registration was completed.";
        private const string MESSAGE_9 = "There is not object data.";
        private const string MESSAGE_10 = "Two or more contents are selected. Select one content. ";
        private const string MESSAGE_11 = "Delete selected line. OK?";
        private const string MESSAGE_12 = "It failed to delete the data. ";
        private const string MESSAGE_13 = "Deletion of the data is completed.";
        private const string MESSAGE_14 = "There is not the data that is not matched with condition.";
        private const string MESSAGE_15 = "Selected line is applied and then it is registered. OK?";
        private const string MESSAGE_16 = "It failed to apply and register the data.";
        private const string MESSAGE_17 = "Applying and registering of data is completed.";
        private const string MESSAGE_18 = @"""A"" could not be found.";
        private const string MESSAGE_19 = "It failed to output the data.";
        private const string MESSAGE_20 = "Process could not be found.";
        private const string MESSAGE_21 = "Select operator.";
        private const string MESSAGE_22 = @"Input all ""Passed date""";
        private const string MESSAGE_23 = @"""Check sheet No"" has already registered. Do you want to change it?";
        private const string MESSAGE_24 = "Select document type.";
        private const string MESSAGE_25 = "Displayed content is saved. OK?";
        private const string MESSAGE_26 = @"The name of ""Section"" is not registered in the master.";
        private const string MESSAGE_27 = @"The name of ""Product type"" is not registered in the master.";
        private const string MESSAGE_28 = @"The name of ""Process"" is not registered in the master.";
        private const string MESSAGE_29 = @"The name of ""Review cycle (Month)""  is not registered in the master.";
        private const string MESSAGE_30 = "Pre-registration was completed.";
        private const string MESSAGE_31 = "It failed Pre-registration.";
        private const string MESSAGE_32 = @"The name of ""Operation name in NASCA"" is not registered in the master.";
        private const string MESSAGE_33 = "Select section.";
        private const string MESSAGE_34 = @"Select ""Check sheet No."".";
        private const string MESSAGE_35 = "Update content of database. OK?";
        private const string MESSAGE_36 = "Pre-registration in selected line is canceled. OK?";
        private const string MESSAGE_37 = "It is possible to cancel only Slill Map that is applying.";
        private const string MESSAGE_38 = "It is impossible to cancel Skill Map on 1st edition.";
        private const string MESSAGE_39 = "It failed to cancel the data.";
        private const string MESSAGE_40 = "Cancel  was completed.";
        private const string MESSAGE_41 = @"Same person is not able to select with ""Educator"" and ""Operator"".";
        private const string MESSAGE_42 = "Selected line is registered. OK?";
        private const string MESSAGE_43 = "It has already registered.";
        private const string MESSAGE_44 = "Product type is not selected.";
        private const string MESSAGE_45 = "It failed to execute operation. Acquire the data or information again.    [Detail] = ";
        private const string MESSAGE_46 = @"Inline_ character strings can not be acquired form Inline_No. Check ""QCIL.xml"" and ""Master TmLINE""";
        private const string MESSAGE_47 = "It failed to get erorr list. Acquire the data or information again.    [Detail] = ";
        private const string MESSAGE_48 = "The extraction of data is completed.";
        private const string MESSAGE_49 = @"""Line"", ""Machine (Equipment)"", ""Machine / Equipment No."", Type, either is not selected.";
        private const string MESSAGE_50 = @"Composition machine No at line is wrong setting in ""C:\\QCIL\\SettingFiles\\QCIL.xml"". Contact PIC of the system.";
        private const string MESSAGE_51 = "Input content of correspondence and person in charge.";
        private const string MESSAGE_52 = "Input content of correspondence within 200 characters.";
        private const string MESSAGE_53 = "Input person in charge within 32 characters.";
        private const string MESSAGE_54 = @"""Machine (Equipment) No."" or ""Period"" is wrong. Execute it again after input ""Machine (Equipment) No."" or ""Period"" again.";
        private const string MESSAGE_55 = "Data can not be registered. The line that the content was changed is wrong.";
        private const string MESSAGE_56 = "There is not data to register. Registration is discontinued.";
        private const string MESSAGE_57 = "Inspection_NO can not be acquired with [QcParam_NO]={0} from TmQsub";
        private const string MESSAGE_58 = "It failed to acquire [QcParamNo] from TmQsub  [Detail] = ";
        private const string MESSAGE_59 = "It failed to acquire [QcParamNo]   [Detail] = ";
        private const string MESSAGE_60 = "It failed to acquire [Model_NM]   [Detail] = ";
        private const string MESSAGE_61 = "Operation is executing. Execute it again after it passes for a while.";
        private const string MESSAGE_62 = "Completion of operation needs long time (half hour or more). Execute operation?";
        private const string MESSAGE_63 = "Operation is completed.";
        private const string MESSAGE_64 = "The data that the date is over 31 days can not be output.";
        private const string MESSAGE_65 = "Please input a correct value to the text of the die.";
        private const string MESSAGE_66 = "Please select the employee number or the employee name.";
        private const string MESSAGE_67 = "Can not found the employee";
        private const string MESSAGE_68 = "Can not found the batch";
        private const string MESSAGE_69 = "The authority under the selection is added？Ok?";
        private const string MESSAGE_70 = "The authority under the selection is deleted？Ok?";
        private const string MESSAGE_71 = "Failed in the acquisition of machine category";
        private const string MESSAGE_72 = "There was not a fixed form sentence to accord in search condition.";
        private const string MESSAGE_73 = "Necessary data doesn't suffice.";
        private const string MESSAGE_74 = "Failed in the acquisition of function";
        private const string MESSAGE_75 = "Preservation finished";
        private const string MESSAGE_76 = "(Mode write)";
        private const string MESSAGE_77 = "(Mode readonly)";
        private const string MESSAGE_78 = "(Mode Cliant)";
        private const string MESSAGE_79 = "";
        private const string MESSAGE_80 = "Unsetting";
        private const string MESSAGE_81 = "対象フォルダが見つかりませんでした。フォルダパス:{0}";
        private const string MESSAGE_82 = "対象ファイルが見つかりませんでした。フォルダパス:{0}";
        private const string MESSAGE_83 = "対象ファイルが複数存在します。フォルダパス:{0}";
        private const string MESSAGE_84 = "[ロット番号 {0}]指定されたロット番号の型番が見つかりませんでした。";
		private const string MESSAGE_85 = "It has set it.";
		private const string MESSAGE_86 = "Please input the numerical value to management NO.";
		private const string MESSAGE_87 = "Selected items already exist in the list.";
		private const string MESSAGE_88 = "Information on the key is improper to do the update processing.  the model: {0} management NO:{1} product serial number: {2}.";
		private const string MESSAGE_89 = "Information on the key is improper to do the update processing.  the product serial number: {0} management group NO:{1} management kind NO:{2}.";
		private const string MESSAGE_90 = "The adhering watch item of the relation is not registered.  Management NO:{0}.";
		private const string MESSAGE_91 = "The reason for the change is not input.  Management NO:{0}.";
		private const string MESSAGE_92 = "Information on the key is improper to do the update processing.  the product serial number: {0}.";
		private const string MESSAGE_93 = "The defective item name is not being registered by the master.  Defective CD:{0}.";
		private const string MESSAGE_94 = "The line has not been selected.";
		private const string MESSAGE_95 = "The forwarding object has not been selected.";
		private const string MESSAGE_96 = "The registration object has not been selected.";
		private const string MESSAGE_97 = "";
		private const string MESSAGE_98 = "";
		private const string MESSAGE_99 = "";
		private const string MESSAGE_100 = "";
		private const string MESSAGE_101 = "";
        private const string MESSAGE_102 = "[汎用グループ番号 {0}]汎用情報がマスタに登録されていません。";
        private const string MESSAGE_103 = "同型番で「チップ搭載数/装置」の値が違うレコードがあります。";
        private const string MESSAGE_104 = "異なる型番が選択されているため参照登録できません。";
        private const string MESSAGE_105 = "(内規値編集権限)";
        private const string MESSAGE_106 = "更新処理をするにはキーの情報が不適切です。製品型番:{0} 管理NO：{0} ";
        private const string MESSAGE_107 = "(蛍光体シート閾値読み取り専用)";
        private const string MESSAGE_108 = "(蛍光体シート閾値編集)";

        #region プロパティ

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

        #endregion
    }
}
