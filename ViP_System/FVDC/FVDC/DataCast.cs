/*************************************************************************************
 * システム名     : 設備マスタシステム
 *  
 * 処理名         : DataCast
 * 
 * 概略           : データを加工する処理を集めたクラス
 * 
 * 作成           : 2006/10/25 SLA.Uchida　無塵ウェアシステムからのコピー
 * 
 * 修正履歴       : 2016/04/01 SLA.Uchida　接続情報を編集する機能追加
 *                  2016/05/09 SLA.Uchida　不要文字削除＆NULL対応
 ************************************************************************************/

using System;
using System. Data;

namespace FVDC
{
	/// <summary>
	/// データを加工する処理を集めたクラス
	/// </summary>
	public class DataCast
	{
		/// <summary>
		/// データを更新し易い文字タイプに成型する
		/// </summary>
		/// <param name="dsRow">入力データ</param>
		/// <param name="stgItem">成型結果</param>
		public void dsRowCast(DataRow dsRow, ref string[] stgItem)
		{
//			string[] stgItem					= new string[dsRow.Table.Columns.Count];
			for(int j = 0; j < dsRow.Table.Columns.Count; j++)
			{
				string dataType					= dsRow[j].GetType().ToString();
				switch(dataType)
				{
					case "System.String":
                        if (dsRow[j].ToString().ToUpper() == "NULL")
                        {
                            stgItem[j]          = "NULL";
                        }
                        else
                        {
                            stgItem[j]          = "'" + dsRow[j].ToString().Replace("'", "''").Replace("\n", "").Replace("\r", "").Trim() + "'";
                        }
						break;
					case "System.DateTime":
						if (dsRow[j].Equals(null))
						{
							stgItem[j]			= "NULL";
						}
						else
						{
							stgItem[j]			= "'" + dsRow[j] + "'";
						}
						break;
					case "System.DBNull":
                        stgItem[j]              = "NULL";
						break;
                    case "System.Boolean":
                        if (dsRow[j].ToString().ToUpper() == "NULL")
                        {
                            stgItem[j]          = "NULL";
                        }
                        else if (Convert.ToBoolean(dsRow[j]))
						{
							stgItem[j]			= "1";
						}
						else
						{
							stgItem[j]			= "0";
						}
						break;
					default:
                        stgItem[j]              = dsRow[j].ToString().Replace("\n", "").Replace("\r", "");
						break;
				}
				if ((j > 2) && (stgItem[j].IndexOf(",", 0) <= 0))
				{
					try
					{
						decimal NumericData		= Convert.ToDecimal(stgItem[j].Replace("'",""));
						stgItem[j]				= NumericData.ToString();
					}
					catch
					{}
				}
			}
		}

		/// <summary>
		/// 漢数字等コンバート出来ない値も含めて全て数字に変換する
		/// </summary>
		/// <param name="Char">変換対象</param>
		/// <returns>結果</returns>
		public int NumericChange(string Char)
		{
			string Numeric	= Char.Trim();
			Numeric			= Numeric.Replace("〇","0");
			Numeric			= Numeric.Replace("零","0");
			Numeric			= Numeric.Replace("一","1");
			Numeric			= Numeric.Replace("壱","1");
			Numeric			= Numeric.Replace("弐","2");
			Numeric			= Numeric.Replace("二","2");
			Numeric			= Numeric.Replace("参","3");
			Numeric			= Numeric.Replace("三","3");
			Numeric			= Numeric.Replace("四","4");
			Numeric			= Numeric.Replace("伍","5");
			Numeric			= Numeric.Replace("五","5");
			Numeric			= Numeric.Replace("六","6");
			Numeric			= Numeric.Replace("七","7");
			Numeric			= Numeric.Replace("八","8");
			Numeric			= Numeric.Replace("九","9");
			Numeric			= Numeric.Replace("十","0");
			Numeric			= Numeric.Replace("百","00");
			Numeric			= Numeric.Replace("千","000");
			Numeric			= Numeric.Replace("万","0000");

			Numeric			= Numeric.Replace("Ⅰ","1");
			Numeric			= Numeric.Replace("Ⅱ","2");
			Numeric			= Numeric.Replace("Ⅲ","3");
			Numeric			= Numeric.Replace("Ⅳ","4");
			Numeric			= Numeric.Replace("Ⅴ","5");
			Numeric			= Numeric.Replace("Ⅵ","6");
			Numeric			= Numeric.Replace("Ⅶ","7");
			Numeric			= Numeric.Replace("Ⅷ","8");
			Numeric			= Numeric.Replace("Ⅸ","9");
			Numeric			= Numeric.Replace("Ⅹ","10");

			Numeric			= Numeric.Replace("ⅰ","1");
			Numeric			= Numeric.Replace("ⅱ","2");
			Numeric			= Numeric.Replace("ⅲ","3");
			Numeric			= Numeric.Replace("ⅳ","4");
			Numeric			= Numeric.Replace("ⅴ","5");
			Numeric			= Numeric.Replace("ⅵ","6");
			Numeric			= Numeric.Replace("ⅶ","7");
			Numeric			= Numeric.Replace("ⅷ","8");
			Numeric			= Numeric.Replace("ⅸ","9");
			Numeric			= Numeric.Replace("ⅹ","10");

			Numeric			= Numeric.Replace("①","1");
			Numeric			= Numeric.Replace("②","2");
			Numeric			= Numeric.Replace("③","3");
			Numeric			= Numeric.Replace("④","4");
			Numeric			= Numeric.Replace("⑤","5");
			Numeric			= Numeric.Replace("⑥","6");
			Numeric			= Numeric.Replace("⑦","7");
			Numeric			= Numeric.Replace("⑧","8");
			Numeric			= Numeric.Replace("⑨","9");
			Numeric			= Numeric.Replace("⑩","10");

			Numeric			= Numeric.Replace("０","0");
			Numeric			= Numeric.Replace("１","1");
			Numeric			= Numeric.Replace("２","2");
			Numeric			= Numeric.Replace("３","3");
			Numeric			= Numeric.Replace("４","4");
			Numeric			= Numeric.Replace("５","5");
			Numeric			= Numeric.Replace("６","6");
			Numeric			= Numeric.Replace("７","7");
			Numeric			= Numeric.Replace("８","8");
			Numeric			= Numeric.Replace("９","9");

			try
			{
				return Convert.ToInt32(Numeric);
			}
			catch
			{
				return 0;
			}
		}
        
		/// <summary>
		/// 接続情報を編集する
		/// </summary>
		/// <param name="TitleNM"></param>
		/// <param name="ConnectionNM"></param>
		/// <param name="LoginNM"></param>
		/// <returns></returns>
        public string ConnectServer(string TitleNM, ref string TableName)
        {
            /// タイトルより対象テーブル情報を特定する
            string DbName           = "";
            if (TitleNM.Contains("："))
            {
                string[] sptTitle   = TitleNM.Split('：');
                string[] sptTable   = sptTitle[1].Split('【');
                DbName              = sptTable[0];
                TableName           = sptTable[1].Replace("】", "");
            }
            else
            {
                string[] sptTable   = TitleNM.Split('【');
                DbName              = sptTable[0];
                try
                {
                    TableName       = sptTable[1].Replace("】", "");
                }
                catch { }
            }

            /// 接続情報の編集
            string ConnectServer    = "";

            switch (DbName)
            {
                case "ARMS":
                    ConnectServer   = Constant.StrARMSDB;
                    break;
                case "LENS":
                    ConnectServer   = Constant.StrLENSDB;
                    break;
                case "QCIL":
                    ConnectServer   = Constant.StrQCILDB;
                    break;
            }
            return ConnectServer;
        }
	}
}

