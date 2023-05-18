using EICS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
using System.Linq;
using EICS.Machine;
using System.IO;

namespace TestProject
{
    
    
    /// <summary>
	///TEST_SMFile_GetData のテスト クラスです。すべての
	///TEST_SMFile_GetData 単体テストをここに含めます
    ///</summary>
	[TestClass()]
	public class TEST_SMFile_GetData
	{

		string CurrentDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

		private TestContext testContextInstance;

		/// <summary>
		///現在のテストの実行についての情報および機能を
		///提供するテスト コンテキストを取得または設定します。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 追加のテスト属性
		// 
		//テストを作成するときに、次の追加属性を使用することができます:
		//
		//クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//各テストを実行する前にコードを実行するには、TestInitialize を使用
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//各テストを実行した後にコードを実行するには、TestCleanup を使用
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 全数サンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			List<string> fileValue = null; // TODO: 適切な値に初期化してください


			int samplingModeCD = 0; // TODO: 適切な値に初期化してください
			int samplingCT = 0; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = 0; i < 10; i++)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:08";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);

			Assert.AreEqual(expected.MeasureDT, actual.MeasureDT);
			Assert.AreEqual(expected.OpeningCheckFileType, actual.OpeningCheckFileType);
			Assert.AreEqual(expected.TargetVAL, actual.TargetVAL);
			for (int j = 0; j < expected.ValueList.Count; j++)
			{
				Assert.AreEqual(expected.ValueList[j], actual.ValueList[j]);
			}
		}

		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 前5点のサンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			List<string> fileValue = null; // TODO: 適切な値に初期化してください


			int samplingModeCD = 1; // TODO: 適切な値に初期化してください
			int samplingCT = 5; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = 0; i < samplingCT; i++)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:03";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);

			Assert.AreEqual(expected.MeasureDT, actual.MeasureDT);
			Assert.AreEqual(expected.OpeningCheckFileType, actual.OpeningCheckFileType);
			Assert.AreEqual(expected.TargetVAL, actual.TargetVAL);
			for (int j = 0; j < expected.ValueList.Count; j++)
			{
				Assert.AreEqual(expected.ValueList[j], actual.ValueList[j]);
			}
		}

		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 後5点のサンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			int samplingModeCD = 2; // TODO: 適切な値に初期化してください
			int samplingCT = 5; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = 9; i >= samplingCT; i--)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:08";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);

			Assert.AreEqual(expected.MeasureDT, actual.MeasureDT);
			Assert.AreEqual(expected.OpeningCheckFileType, actual.OpeningCheckFileType);
			Assert.AreEqual(expected.TargetVAL, actual.TargetVAL);
			for (int j = 0; j < expected.ValueList.Count; j++)
			{
				Assert.AreEqual(expected.ValueList[j], actual.ValueList[j]);
			}
		}

		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 前11点のサンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			int samplingModeCD = 1; // TODO: 適切な値に初期化してください
			int samplingCT = 11; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = 0; i < 10; i++)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:08";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);

			Assert.AreEqual(expected.MeasureDT, actual.MeasureDT);
			Assert.AreEqual(expected.OpeningCheckFileType, actual.OpeningCheckFileType);
			Assert.AreEqual(expected.TargetVAL, actual.TargetVAL);
			for (int j = 0; j < expected.ValueList.Count; j++)
			{
				Assert.AreEqual(expected.ValueList[j], actual.ValueList[j]);
			}
		}

		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 前方抜き取りでサンプリング数が0以下サンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			List<string> fileValue = null; // TODO: 適切な値に初期化してください


			int samplingModeCD = 1; // TODO: 適切な値に初期化してください
			int samplingCT = 0; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = 0; i < 10; i++)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:08";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);

			Assert.AreEqual(expected.MeasureDT, actual.MeasureDT);
			Assert.AreEqual(expected.OpeningCheckFileType, actual.OpeningCheckFileType);
			Assert.AreEqual(expected.TargetVAL, actual.TargetVAL);
			for (int j = 0; j < expected.ValueList.Count; j++)
			{
				Assert.AreEqual(expected.ValueList[j], actual.ValueList[j]);
			}
		}

		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 後方抜き取りでサンプリング数が0以下サンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			List<string> fileValue = null; // TODO: 適切な値に初期化してください


			int samplingModeCD = 2; // TODO: 適切な値に初期化してください
			int samplingCT = 0; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = 0; i < 10; i++)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:08";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);

			Assert.AreEqual(expected.MeasureDT, actual.MeasureDT);
			Assert.AreEqual(expected.OpeningCheckFileType, actual.OpeningCheckFileType);
			Assert.AreEqual(expected.TargetVAL, actual.TargetVAL);
			for (int j = 0; j < expected.ValueList.Count; j++)
			{
				Assert.AreEqual(expected.ValueList[j], actual.ValueList[j]);
			}
		}

		/// <summary>
		///GetData のテスト
		///</summary>
		[TestMethod()]
		public void 未定義サンプリングモードのサンプリングのテスト()
		{
			MachineFileInfo machineFileInfo = MachineBase.GetFileInfo(CurrentDir + @"/TestProject/AIMachineInfo/TEST_SMFILE.csv", CurrentDir + @"/TestProject/AIMachineInfo/", 1);
			string[] textArray = machineFileInfo.Content.Split('\n');

			int samplingModeCD = 3; // TODO: 適切な値に初期化してください
			int samplingCT = 5; // TODO: 適切な値に初期化してください
			bool wbMappingFG = false; // TODO: 適切な値に初期化してください


			SMFile expected = new SMFile(); // TODO: 適切な値に初期化してください
			for (int i = samplingCT; i < 10; i++)
				expected.ValueList.Add(i);

			expected.MeasureDT = "2014/07/08 23:24:03";
			expected.OpeningCheckFileType = Constant.OpeningCheckFileType.NotOpeningCheckFile;

			SMFile actual;

			try
			{
				actual = SMFile.GetData(textArray.ToList(), 5, samplingModeCD, samplingCT, wbMappingFG, null);
			}
			catch (ApplicationException err)
			{
				return;
			}
			catch (Exception err)
			{
				Assert.Inconclusive("想定外の例外が発生しました。");
			}
			Assert.Inconclusive("例外が発生しませんでした。");
		}
	}
}
