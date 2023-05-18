using EICS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace TestProject
{
    
    
    /// <summary>
    ///WBMachineInfoWBMachineInfoTest のテスト クラスです。すべての
    ///WBMachineInfoWBMachineInfoTest 単体テストをここに含めます
    ///</summary>
	[TestClass()]
	public class TEST_GetAroundCount
	{

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

		#region テスト前初期化
		private List<MappingBaseInfo> GetInitializedMappingBaseList(int inspectionAddr, int magazinePkgMaxCt)
		{
			List<MappingBaseInfo> mappingBaseList = new List<MappingBaseInfo>();


			for (int i = 1; i <= magazinePkgMaxCt; i++)
			{
				int unitNO = 0;
				string errorCD = "0";
				string tranCD = "0";


				if (i == inspectionAddr)
				{
					tranCD = "1";
				}

				mappingBaseList.Add(new MappingBaseInfo(unitNO, i, tranCD, errorCD));
			}

			return mappingBaseList;
		}
		#endregion

		private void 変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.Before;
			maxPkgCt = 9000;
			frameXPkgCt = 50;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.After;
			maxPkgCt = 9000;
			frameXPkgCt = 50;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.Before;
			maxPkgCt = 9000;
			frameXPkgCt = 50;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.After;
			maxPkgCt = 9000;
			frameXPkgCt = 50;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.Before;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.After;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.Before;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			int aroundCol = 2;
			frameDirect = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.After;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = frameYPkgCt * aroundCol;
		}

		private void 変数初期化_順方向・手前・Max9000pcs・X45・Y4・半列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			double aroundCol = 0.5;
			frameDirect = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.Before;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 4;
			isMap = false;

			aroundColPkgCt = (int)(frameYPkgCt * aroundCol + 0.5);
		}

		private void 変数初期化_順方向・手前・Max9000pcs・X45・Y5・半列・非MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			double aroundCol = 0.5;
			frameDirect = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.Before;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 5;
			isMap = false;

			aroundColPkgCt = (int)(frameYPkgCt * aroundCol + 0.5);
		}

		private void 変数初期化_Max8100pcs・X45・Y4・MAP品種(ref int frameDirect, ref WBMachineInfo.Direction direct, ref int maxPkgCt, ref int frameXPkgCt, ref int frameYPkgCt, ref int aroundColPkgCt, ref bool isMap)
		{
			frameDirect = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.After;
			maxPkgCt = 8100;
			frameXPkgCt = 45;
			frameYPkgCt = 4;
			isMap = true;
		}

		#region フレームのX方向パッケージ最大列数が偶数のケース
		#region 順方向ボンディングのテスト
		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの手前のテスト
		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の49列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8996の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8996;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8995の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8995;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8994の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8994;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8993の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8993;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の50列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス9000の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 9000;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8999の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8999;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8998の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8998;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8997の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8997;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion


		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの後のテスト
		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の49列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8996の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8996;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8995の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8995;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8994の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8994;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8993の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8993;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の50列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス9000の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 9000;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8999の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8999;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8998の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8998;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8997の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8997;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion
		#endregion

		#region 逆方向ボンディングのテスト
		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの手前のテスト
		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の49列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8996の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8996;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8995の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8995;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8994の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8994;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8993の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8993;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の50列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス9000の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 9000;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8999の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8999;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8998の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8998;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8997の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8997;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの後のテスト
		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の49列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8996の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8996;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8995の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8995;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8994の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8994;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8993の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8993;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の50列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス9000の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 9000;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8999の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8999;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8998の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8998;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8997の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8997;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max9000pcs・X50・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion
		#endregion
		#endregion


		#region フレームのX方向パッケージ最大列数が奇数のケース
		#region 順方向ボンディングのテスト
		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの手前のテスト
		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2024列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8096の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8096;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8095の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8095;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8094の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8094;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8093の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8093;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2025列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8100の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8100;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8099の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8099;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8098の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8098;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8097の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8097;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion


		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの後のテスト
		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2024列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8096の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8096;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8095の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8095;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8094の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8094;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8093の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8093;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 順方向ボンディングで周辺検査列2でY方向Pkg数4の2025列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8100の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8100;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8099の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8099;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8098の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8098;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数順方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8097の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8097;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion
		#endregion

		#region 逆方向ボンディングのテスト
		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの手前のテスト
		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2024列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8096の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8096;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8095の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8095;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8094の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8094;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8093の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8093;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の50列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8100の手前の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8100;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8099の手前の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8099;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8098の手前の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8098;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8097の手前の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8097;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・手前・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の起点アドレスの後のテスト
		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の1列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス1の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス2の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス3の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス4の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス5の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス6の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 6;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス7の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 7;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2024列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8096の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8096;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8095の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8095;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8094の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8094;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8093の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8093;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion

		#region 逆方向ボンディングで周辺検査列2でY方向Pkg数4の2025列目のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8100の後の周辺検査数が11かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8100;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 11;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8099の後の周辺検査数が10かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8099;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 10;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8098の後の周辺検査数が9かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8098;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数奇数逆方向ボンディングで周辺検査列2でY方向Pkg数4で起点アドレス8097の後の周辺検査数が8かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 8097;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_逆方向・後・Max8100pcs・X45・Y4・2列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 8; // TODO: 適切な値に初期化してください

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#endregion
		#endregion
		#endregion

		#region 半列のテスト
		#region Y方向Pkg数4のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数4で起点アドレス1の手前の周辺検査数が2かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y4・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 2;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数4で起点アドレス2の手前の周辺検査数が3かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y4・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 3;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数4で起点アドレス3の手前の周辺検査数が2かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y4・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 2;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数4で起点アドレス4の手前の周辺検査数が3かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y4・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 3;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}
		#endregion
		#region Y方向Pkg数5のテスト
		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数5で起点アドレス1の手前の周辺検査数が3かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y5・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 3;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数5で起点アドレス2の手前の周辺検査数が4かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 2;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y5・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 4;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数5で起点アドレス3の手前の周辺検査数が5かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 3;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y5・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 9999;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数5で起点アドレス4の手前の周辺検査数が3かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 4;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y5・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 3;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void X列数偶数順方向ボンディングで周辺検査列半列でY方向Pkg数5で起点アドレス5の手前の周辺検査数が4かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 5;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_順方向・手前・Max9000pcs・X45・Y5・半列・非MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 4;

			int actual;
			actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
			Assert.AreEqual(expected, actual);
		}

		#endregion
		#endregion

		/// <summary>
		///GetAroundCount のテスト
		///</summary>
		[TestMethod()]
		public void MAP品種がどんなケースでも周辺検査数5かテスト()
		{
			//起点アドレス
			int replaceStartAddr = 1;

			#region 変数宣言
			int frameYPackageCt = 0;
			int frameXPackageCt = 0;
			int aroundColumnPkgCt = 0;
			int maxPkgCt = 0;
			bool isMapType = false;
			int frameDirection = 0;
			WBMachineInfo.Direction direct = new WBMachineInfo.Direction();
			#endregion

			変数初期化_Max8100pcs・X45・Y4・MAP品種(ref frameDirection, ref direct,
				ref maxPkgCt, ref frameXPackageCt, ref frameYPackageCt, ref aroundColumnPkgCt, ref isMapType);

			int framePackageCt = frameXPackageCt * frameYPackageCt;

			List<MappingBaseInfo> mappingBaseList = GetInitializedMappingBaseList(replaceStartAddr, maxPkgCt);

			//期待値
			int expected = 5;

			double aroundCol;
			frameDirection = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.After;

			for (replaceStartAddr = 1; replaceStartAddr <= maxPkgCt; replaceStartAddr++)
			{
				for (aroundCol = 0; aroundCol <= 3; aroundCol += 0.5)
				{
					int aroundColPkgCt = (int)(frameYPackageCt * aroundCol + 0.5);
					int actual;
					actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
					Assert.AreEqual(expected, actual);
				}
			}


			frameDirection = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.After;

			for (replaceStartAddr = 1; replaceStartAddr <= maxPkgCt; replaceStartAddr++)
			{
				for (aroundCol = 0; aroundCol <= 3; aroundCol += 0.5)
				{
					int aroundColPkgCt = (int)(frameYPackageCt * aroundCol + 0.5);
					int actual;
					actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
					Assert.AreEqual(expected, actual);
				}
			}

			frameDirection = (int)Constant.FrameDirection.Normal;
			direct = WBMachineInfo.Direction.Before;

			for (replaceStartAddr = 1; replaceStartAddr <= maxPkgCt; replaceStartAddr++)
			{
				for (aroundCol = 0; aroundCol <= 3; aroundCol += 0.5)
				{
					int aroundColPkgCt = (int)(frameYPackageCt * aroundCol + 0.5);
					int actual;
					actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
					Assert.AreEqual(expected, actual);
				}
			}


			frameDirection = (int)Constant.FrameDirection.Reverse;
			direct = WBMachineInfo.Direction.Before;

			for (replaceStartAddr = 1; replaceStartAddr <= maxPkgCt; replaceStartAddr++)
			{
				for (aroundCol = 0; aroundCol <= 3; aroundCol += 0.5)
				{
					int aroundColPkgCt = (int)(frameYPackageCt * aroundCol + 0.5);
					int actual;
					actual = WBMachineInfo.GetAroundCount(framePackageCt, frameYPackageCt, aroundColumnPkgCt, frameDirection, replaceStartAddr, mappingBaseList, direct, isMapType);
					Assert.AreEqual(expected, actual);
				}
			}
		}
	}
}
