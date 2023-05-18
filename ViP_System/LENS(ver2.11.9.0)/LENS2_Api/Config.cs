using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LENS2_Api
{
	public class Config
	{
		/// <summary>
		/// StaticInstance
		/// </summary>
		public static Config Settings;

		private const string SETTING_FILE_NM = "LensConfig.xml";

		/// <summary>
		/// ワイヤーボンド工程NO
		/// </summary>
        public int WirebondProcNo { get; set; }

        /// <summary>
        /// モールド工程NO
        /// </summary>
        public int MoldProcNo { get; set; }

		/// <summary>
		/// 検査工程NO
		/// </summary>
		public int InspectProcNo { get; set; }

		/// <summary>
		/// ARMS データベース接続文字列
		/// </summary>
		public string ArmsConnectionString { get; set; }

		/// <summary>
		/// LENS データベース接続文字列
		/// </summary>
		public string LensConnectionString { get; set; }

		/// <summary>
		/// ファイル操作リトライ回数
		/// </summary>
		public int FileAccessRetryCount { get; set; }

		public string EncodingString { get; set; }

		/// <summary>
		/// 外観検査用マッピングファイル保管場所
		/// </summary>
		public string ForAIMappingFileDirPath { get; set; }

		/// <summary>
		/// モールド用マッピングファイル保管場所
		/// </summary>
		public string ForMDMappingFileDirPath { get; set; }

		public string ForWBCompareMachineLogDirPath { get; set; }

		public string ForAICompareMachineLogDirPath { get; set; }

		public int LRMappingAddressFrontSideSize { get; set; }

		public int LRMappingAddressBackSideSize { get; set; }

		/// <summary>
		/// 外観検査機用・モールド用のマッピングファイルをバックアップフォルダへ移動するまでの日数
		/// </summary>
		public int MoveMappingFileIntervalDay { get; set; }

        public string MachineGroupCd { get; set; }

        public string PreMoldWorkCd { get; set; }

        /// <summary>
        /// モールド工程NO (マッピング無し)
        /// </summary>
        public List<int> MoldProcNoList { get; set; }

        /// <summary>
        /// モールド工程NOリスト (マッピング無し)
        /// </summary>
        public List<int> PreMoldProcNoList { get; set; }

		/// <summary>
		/// 検査機でマッピング照合OK, NGを返すモード
		/// </summary>
		public bool IsMappingResultInterlockMode { get; set; }

		public bool DebugLogOutFg { get; set; }

		/// <summary>
		/// データベース操作リトライ回数
		/// </summary>
		public int DatabaseAccessRetryCount { get; set; }

        /// <summary>
        /// EICSS データベース接続文字列
        /// </summary>
        public string EicsConnectionString { get; set; }

        /// <summary>
        /// MDマッピングNG時の流動規制工程
        /// </summary>
        public List<KeyValuePair<int, string>> MdMappingNgRestrictWorkCd { get; set; }
        
        static Config()
		{
			try
			{
				string raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, SETTING_FILE_NM), Encoding.UTF8);
				Settings = JsonConvert.DeserializeObject<Config>(raw);

			}
			catch (JsonReaderException err)
			{
				throw err;
			}
		}

		//public void Save()
		//{
		//	string raw = JsonConvert.SerializeObject(this, Formatting.Indented);
		//	try
		//	{
		//		File.WriteAllText(SETTING_FILE_NM, raw);
		//	}
		//	catch (Exception err)
		//	{
		//		//Log.SysLog.Error("Config保存失敗 " + ex.ToString());
		//		//throw new ApplicationException("Config保存失敗");
		//	}
		//}
	}
}
