using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LENS2;
using System.IO;

//namespace LENS2
//{
//	class MMFileForUnitTest : Inspector.MMFile
//	{
//		public string GetMappingDataAfterInspection(string typeCD, string lotNO, bool isWBMappingInspection)
//		{
//			return base.GetMappingDataAfterInspection(typeCD, lotNO, isWBMappingInspection);
//		}
//	}
//}

namespace UnitTestProject
{


	[TestClass]
	public class InspectorUnitTest
	{
		private string TESTTYPE = "UNIT_TEST";
		private string TESTLOT = "UNIT_TEST";
		private string WB_MAPPING_FOLDER = @"C:\NAMI\data\WB\bind\";
		private string AI_MAPPING_FOLDER = @"C:\NAMI\data\AI\bind\";
		private string MD_MAPPING_FOLDER = @"C:\NAMI\data\MD\bind\";

		private TestContext testContextInstance;
		public TestContext TestContext
		{
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestMethod]
		[TestCase("UNITTEST奇数_全数1_マッピング1_変化点1", "11111111110000000000111111111100000000001111111111", "<奇数段搭載,全数検査1,マッピング検査対象1,変化点1⇒オール1マッピング")]
		[TestCase("UNITTEST奇数_全数1_マッピング1_変化点0", "11111111110000000000111111111100000000001111111111", "<奇数段搭載,全数検査1,マッピング検査対象1,変化点0⇒オール1マッピング")]
		[TestCase("UNITTEST奇数_全数1_マッピング0_変化点1", "11111111110000000000111111111100000000001111111111", "<奇数段搭載,全数検査1,マッピング検査対象0,変化点1⇒オール1マッピング")]
		[TestCase("UNITTEST奇数_全数1_マッピング0_変化点0", "11111111110000000000111111111100000000001111111111", "<奇数段搭載,全数検査1,マッピング検査対象0,変化点0⇒オール1マッピング")]
		//[TestCase("UNITTEST奇数_全数0_マッピング1_変化点1", "01000011111111111111111111111111111111111111111111", "<奇数段搭載,全数検査0,マッピング検査対象1,変化点1⇒wbmファイルから生成")]
		//[TestCase("UNITTEST奇数_全数0_マッピング1_変化点0", "00100001111111111111111111111111111111111111111111", "<奇数段搭載,全数検査0,マッピング検査対象1,変化点0⇒wbmファイルから生成")]
		//[TestCase("UNITTEST奇数_全数0_マッピング0_変化点1", "", "<奇数段搭載,全数検査0,マッピング検査対象0,変化点1⇒string.Empty")]
		//[TestCase("UNITTEST偶数_全数1_マッピング1_変化点1", "00000000001111111111000000000011111111110000000000", "<偶数段搭載,全数検査1,マッピング検査対象1,変化点1⇒オール1マッピング")]
		//[TestCase("UNITTEST偶数_全数1_マッピング1_変化点0", "00000000001111111111000000000011111111110000000000", "<偶数段搭載,全数検査1,マッピング検査対象1,変化点0⇒オール1マッピング")]
		//[TestCase("UNITTEST偶数_全数1_マッピング0_変化点1", "00000000001111111111000000000011111111110000000000", "<偶数段搭載,全数検査1,マッピング検査対象0,変化点1⇒オール1マッピング")]
		//[TestCase("UNITTEST偶数_全数1_マッピング0_変化点0", "00000000001111111111000000000011111111110000000000", "<偶数段搭載,全数検査1,マッピング検査対象0,変化点0⇒オール1マッピング")]
		//[TestCase("UNITTEST偶数_全数0_マッピング1_変化点1", "11111111110100001111111111111111111111111111111111", "<偶数段搭載,全数検査0,マッピング検査対象1,変化点1⇒wbmファイルから生成")]
		//[TestCase("UNITTEST偶数_全数0_マッピング1_変化点0", "11111111110010000111111111111111111111111111111111", "<偶数段搭載,全数検査0,マッピング検査対象1,変化点0⇒wbmファイルから生成")]
		//[TestCase("UNITTEST偶数_全数0_マッピング0_変化点1", "", "<偶数段搭載,全数検査0,マッピング検査対象0,変化点1⇒string.Empty")]
		//[TestCase("UNITTEST全段_全数1_マッピング1_変化点1", "11111111111111111111111111111111111111111111111111", "<全段搭載,全数検査1,マッピング検査対象1,変化点1⇒オール1マッピング")]
		//[TestCase("UNITTEST全段_全数1_マッピング1_変化点0", "11111111111111111111111111111111111111111111111111", "<全段搭載,全数検査1,マッピング検査対象1,変化点0⇒オール1マッピング")]
		//[TestCase("UNITTEST全段_全数1_マッピング0_変化点1", "11111111111111111111111111111111111111111111111111", "<全段搭載,全数検査1,マッピング検査対象0,変化点1⇒オール1マッピング")]
		//[TestCase("UNITTEST全段_全数1_マッピング0_変化点0", "11111111111111111111111111111111111111111111111111", "<全段搭載,全数検査1,マッピング検査対象0,変化点0⇒オール1マッピング")]
		//[TestCase("UNITTEST全段_全数0_マッピング1_変化点1", "11111111111111111111010000111111111111111111111111", "<全段搭載,全数検査0,マッピング検査対象1,変化点1⇒wbmファイルから生成")]
		//[TestCase("UNITTEST全段_全数0_マッピング1_変化点0", "11111111111111111111001000011111111111111111111111", "<全段搭載,全数検査0,マッピング検査対象1,変化点0⇒wbmファイルから生成")]
		//[TestCase("UNITTEST全段_全数0_マッピング0_変化点1", "", "<全段搭載,全数検査0,マッピング検査対象0,変化点1⇒string.Empty")]
		public void 外観検査用マッピングデータ取得テスト()
		{
			//Inspector inspector = new Inspector();
			//TestContext.Run((string ロットNO, string 期待値, string msg) =>
			//	{
			//		inspector.GetMappingDataForInspection(ロットNO).Is(期待値, msg);
			//	});
		}

		//[TestCase("UNITTEST奇数全数0マッピング0変化点0", "00001000011111111111111111111111111111111111111111", "<奇数段搭載,全数検査0,マッピング検査対象0,変化点0⇒Exception")]
		//[TestCase("UNITTEST偶数全数0マッピング0変化点0", "11111111110000100001111111111111111111111111111111", "<偶数段搭載,全数検査0,マッピング検査対象0,変化点0⇒Exception")]
		//[TestCase("UNITTEST全段全数0マッピング0変化点0", "11111111111111111111000010000111111111111111111111", "<全段搭載,全数検査0,マッピング検査対象0,変化点0⇒Exception")]





		//[TestMethod]
		//[TestCase(true, true, true, 8, "11111111", "<全数検査対象true, マッピング検査対象true, 変化点フラグtrue, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//[TestCase(true, false, true, 8, "11111111", "<全数検査対象true, マッピング検査対象false, 変化点フラグtrue, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//[TestCase(true, true, false, 8, "11111111", "<全数検査対象true, マッピング検査対象true, 変化点フラグfalse, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//[TestCase(true, false, false, 8, "11111111", "<全数検査対象true, マッピング検査対象false, 変化点フラグfalse, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//[TestCase(false, true, true, 8, "01000011", "<全数検査対象false, マッピング検査対象true, 変化点フラグtrue, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//[TestCase(false, false, true, 8, null, "<全数検査対象false, マッピング検査対象false, 変化点フラグtrue, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//[TestCase(false, true, false, 8, "01000011", "<全数検査対象false, マッピング検査対象true, 変化点フラグfalse, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//public void 外観検査用マッピングデータ取得の正常系()
		//{
		//	Inspector inspector = new Inspector();

		//	//ファイルを置いたり初期化処理
		//	Initialize外観検査用マッピングデータ取得();

		//	//テスト実行
		//	TestContext.Run((bool 全数検査対象, bool マッピング検査対象, bool 変化点フラグ, int パッケージ数perロット, string 期待値, string msg) =>
		//	{
		//		inspector.GetSequentialMappingData(TESTLOT, 全数検査対象, マッピング検査対象, 変化点フラグ, パッケージ数perロット).Is(期待値, msg);
		//	});

		//	//置いたファイルの削除処理
		//}

		//[TestMethod]
		//[TestCase(false, false, false, 8, "00000000", "<全数検査対象false, マッピング検査対象true, 変化点フラグtrue, パッケージ数perロット8⇒マッピングデータ1が8個の文字列>")]
		//public void 外観検査用マッピングデータ取得の異常系()
		//{
		//	Inspector inspector = new Inspector();

		//	//ファイルを置いたり初期化処理
		//	Initialize外観検査用マッピングデータ取得();

		//	//テスト実行
		//	TestContext.Run((bool 全数検査対象, bool マッピング検査対象, bool 変化点フラグ, int パッケージ数perロット, string 期待値, string msg) =>
		//	{
		//		AssertEx.Throws<ApplicationException>(() => inspector.GetSequentialMappingData(TESTLOT, 全数検査対象, マッピング検査対象, 変化点フラグ, パッケージ数perロット));
		//	});
		//}

		public void Initialize外観検査用マッピングデータ取得()
		{
			//FileInfo.Copy(string.Format("./TestData/{0}.wbm", TESTLOT), string.Format("{0}\\{1}\\{1}.wbm", WB_MAPPING_FOLDER, TESTLOT));
		}

		public void Initialize外観検査後マッピングデータ取得()
		{
		}

		//[TestMethod]
		//[TestCase(true, )]
		//public void 外観検査後マッピングデータ取得の正常系()
		//{

		//	MMFileForUnitTest mmFile = new MMFileForUnitTest();

		//	//ファイルを置いたり初期化処理
		//	Initialize外観検査後マッピングデータ取得();

		//	//テスト実行
		//	TestContext.Run((bool WBマッピング検査対象, string 期待値, string msg) =>
		//	{
		//		mmFile.GetMappingDataAfterInspection(TESTTYPE, TESTLOT, WBマッピング検査対象).Is(期待値, msg);
		//	});
		//}

		//[TestMethod]
		//public void 外観検査後マッピングデータ取得の異常系()
		//{

		//}
	}
}
