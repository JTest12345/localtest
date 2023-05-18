using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LENS2;
using Newtonsoft.Json;
using System.Text;
using LENS2_Api;
using System.Collections.Generic;

namespace UnitTestProject.Mold
{
	[TestClass]
	public class getMappingDataForMoldUnitTest
	{
		private TestContext testContextInstance;
		public TestContext TestContext
		{
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}
		[TestMethod]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 1)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 2)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 3)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 4)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 5)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 6)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 7)]
		[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 8)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 9)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 10)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 11)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 12)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 13)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 14)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 15)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "FrontSide", "All", 60, 6, 40, 16)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 17)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 18)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 19)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 20)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 21)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 22)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 23)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 24)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 25)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 26)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 27)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 28)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 29)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 30)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 31)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "FrontSide", "All", 60, 6, 40, 32)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 33)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 34)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 35)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 36)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 37)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 38)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 39)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 40)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 41)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 42)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 43)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 44)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 45)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 46)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 47)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "BackSide", "All", 60, 6, 40, 48)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 49)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 50)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 51)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 52)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 53)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 54)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 55)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 56)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 57)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 58)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 59)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 60)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 61)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 62)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 63)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "BackSide", "All", 60, 6, 40, 64)]
		////
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 65)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 66)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 67)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 68)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 69)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 70)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 71)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 72)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 73)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 74)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 75)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 76)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 77)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 78)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 79)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "FrontSide", "All", 85, 8, 40, 80)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 81)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 82)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 83)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 84)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 85)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 86)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 87)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 88)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 89)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 90)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 91)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 92)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 93)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 94)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 95)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "FrontSide", "All", 85, 8, 40, 96)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 97)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 98)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 99)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 100)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 101)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 102)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 103)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 104)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 105)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 106)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 107)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 108)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 109)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 110)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 111)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "BackSide", "All", 85, 8, 40, 112)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 113)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 114)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 115)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 116)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 117)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 118)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 119)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 120)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 121)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 122)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 123)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 124)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 125)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 126)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 127)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "BackSide", "All", 85, 8, 40, 128)]
		////
		////
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 129)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 130)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 131)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 132)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 133)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 134)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 135)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 136)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 137)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 138)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 139)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 140)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 141)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 142)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 143)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "FrontSide", "Even", 60, 6, 40, 144)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 145)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 146)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 147)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 148)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 149)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 150)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 151)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 152)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 153)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 154)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 155)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 156)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 157)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 158)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 159)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "FrontSide", "Even", 60, 6, 40, 160)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 161)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 162)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 163)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 164)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 165)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 166)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 167)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 168)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 169)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 170)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 171)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 172)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 173)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 174)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 175)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "BackSide", "Even", 60, 6, 40, 176)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 177)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 178)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 179)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 180)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 181)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 182)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 183)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 184)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 185)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 186)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 187)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 188)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 189)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 190)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 191)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "BackSide", "Even", 60, 6, 40, 192)]
		////
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 193)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 194)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 195)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 196)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 197)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 198)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 199)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 200)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 201)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 202)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 203)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 204)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 205)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 206)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 207)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "FrontSide", "Even", 85, 8, 40, 208)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 209)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 210)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 211)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 212)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 213)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 214)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 215)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 216)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 217)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 218)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 219)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 220)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 221)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 222)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 223)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "FrontSide", "Even", 85, 8, 40, 224)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 225)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 226)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 227)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 228)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 229)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 230)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 231)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 232)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 233)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 234)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 235)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 236)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 237)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 238)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 239)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "BackSide", "Even", 85, 8, 40, 240)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 241)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 242)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 243)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 244)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 245)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 246)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 247)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 248)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 249)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 250)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 251)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 252)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 253)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 254)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 255)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "BackSide", "Even", 85, 8, 40, 256)]
		////
		////
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 257)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 258)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 259)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 260)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 261)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 262)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 263)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 264)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 265)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 266)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 267)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 268)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 269)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 270)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 271)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 60, 6, 40, 272)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 273)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 274)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 275)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 276)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 277)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 278)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 279)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 280)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 281)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 282)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 283)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 284)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 285)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 286)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 287)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 60, 6, 40, 288)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 289)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 290)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 291)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 292)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 293)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 294)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 295)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 296)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 297)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 298)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 299)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 300)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 301)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 302)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 303)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "BackSide", "Odd", 60, 6, 40, 304)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 305)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 306)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 307)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 308)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 309)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 310)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 311)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 312)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 313)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 314)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 315)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 316)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 317)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 318)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 319)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "BackSide", "Odd", 60, 6, 40, 320)]
		////
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 321)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 322)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 323)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 324)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 325)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 326)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 327)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 328)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 329)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 330)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 331)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 332)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 333)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 334)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 335)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "FrontSide", "Odd", 85, 8, 40, 336)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 337)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 338)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 339)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 340)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 341)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 342)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 343)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 344)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 345)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 346)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 347)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 348)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 349)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 350)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 351)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "FrontSide", "Odd", 85, 8, 40, 352)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 353)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 354)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 355)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 356)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 357)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 358)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 359)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 360)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 361)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 362)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 363)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 364)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 365)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 366)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 367)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 0.5, 0, "BackSide", "Odd", 85, 8, 40, 368)]
		////
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 369)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, false, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 370)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 371)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, false, true, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 372)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 373)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, false, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 374)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 375)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", false, true, true, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 376)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 377)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, false, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 378)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 379)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, false, true, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 380)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 381)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, false, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 382)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, false, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 383)]
		//[TestCase("UnitTestLot1", "UnitTestMag1", "UnitTestType1", true, true, true, true, "Column", 1.0, 0, "BackSide", "Odd", 85, 8, 40, 384)]
		public void MDに渡すマッピングデータの取得テスト()
		{
			string raw = File.ReadAllText(@"C:\Config\Nami\UNITTEST_Mold.xml", Encoding.UTF8);
			NAMI.Machine.Mold mold = JsonConvert.DeserializeObject<NAMI.Machine.Mold>(raw);

			TestContext.Run((string ロットNo, string マガジンNo, string タイプ
				, bool 変化点フラグ, bool 全数検査フラグ, bool マッピングフラグ, bool 検査機通過フラグ
				, string 周辺検査タイプ, double 周辺検査列, int 周辺検査パッケージ数
				, string ワイヤー開始位置, string 格納方法
				, int Xパッケージ数, int Yパッケージ数, int フレーム段数, int テスト番号) =>
			{
				Lot lot = GetForUnitTestLot(ロットNo, タイプ, 変化点フラグ, 全数検査フラグ, マッピングフラグ, 検査機通過フラグ);

				Magazine.Config magConfig = GetForUnitTestMagConfig(周辺検査タイプ, Convert.ToDecimal(周辺検査列), 周辺検査パッケージ数
					, ワイヤー開始位置, 格納方法, マガジンNo, Xパッケージ数, Yパッケージ数, フレーム段数);


				Dictionary<int, string> 取得データ = mold.getMappingDataForMold(lot, magConfig);


				Assert.IsTrue(IsEqualList(取得データ, 期待値取得(テスト番号)), "テスト番号 = " + テスト番号.ToString());
				
			});
		}

		public Dictionary<int, string> 期待値取得(int テスト番号)
		{
			Dictionary<int, string> 期待値 = new Dictionary<int, string>();

			if (テスト番号 == 1)
			{
				期待値 = 全0マッピングデータ();
			}

			return 期待値;
		}

		public Dictionary<int, string> 全0マッピングデータ()
		{
			Dictionary<int, string> 期待値 = new Dictionary<int, string>();

			for (int i = 1; i <= 14400; i++)
			{
				期待値.Add(i, "0");
			}

			return 期待値;
		}

		public bool IsEqualList(Dictionary<int, string> target1, Dictionary<int, string> target2)
		{
			if (target1.Count != target2.Count)
			{
				return false;
			}

			foreach (KeyValuePair<int, string> value in target1)
			{
				if (value.Value != target2[value.Key])
				{
					return false;
				}
			}

			return true;
		}

		public Lot GetForUnitTestLot(string ロットNo, string タイプ, bool 変化点フラグ, bool 全数検査フラグ, bool マッピングフラグ, bool 検査機通過フラグ)
		{
			Lot lot = new Lot();
			lot.LotNo = ロットNo;
			lot.TypeCd = タイプ;
			lot.IsChangePoint = 変化点フラグ;
			lot.IsFullSizeInspection = 全数検査フラグ;
			lot.IsMappingInspection = マッピングフラグ;
			lot.HasPassedInspector = 検査機通過フラグ;

			return lot;
		}

		public Magazine.Config GetForUnitTestMagConfig(string 周辺検査タイプ, decimal 周辺検査列, int 周辺検査パッケージ数, string ワイヤー開始位置, string 格納方法, string マガジンNo, int Xパッケージ数, int Yパッケージ数, int フレーム段数)
		{
			Magazine.Config magConfig = new Magazine.Config();

			if (LENS2_Api.Magazine.Config.AroundInspectionType.Column.ToString() == 周辺検査タイプ)
			{
				magConfig.AroundInspectType = LENS2_Api.Magazine.Config.AroundInspectionType.Column;
			}
			else if (LENS2_Api.Magazine.Config.AroundInspectionType.Package.ToString() == 周辺検査タイプ)
			{
				magConfig.AroundInspectType = LENS2_Api.Magazine.Config.AroundInspectionType.Package;
			}
			else
			{
				//強制でテストNGにする時にAssert.Fail()で良かったか確認
				Assert.Fail();
			}

			magConfig.FrameAroundInspectColumn = 周辺検査列;
			magConfig.FrameAroundInspectPackage = 周辺検査パッケージ数;

			if (LENS2_Api.Magazine.Config.FrameWirebondStartPosition.BackSide.ToString() == ワイヤー開始位置)
			{
				magConfig.FrameWirebondStartPositionCD = LENS2_Api.Magazine.Config.FrameWirebondStartPosition.BackSide;
			}
			else if (LENS2_Api.Magazine.Config.FrameWirebondStartPosition.FrontSide.ToString() == ワイヤー開始位置)
			{
				magConfig.FrameWirebondStartPositionCD = LENS2_Api.Magazine.Config.FrameWirebondStartPosition.FrontSide;
			}
			else
			{
				//強制でテストNGにする時にAssert.Fail()で良かったか確認
				Assert.Fail();

			}

			if (LENS2_Api.Magazine.Config.LoadStep.All.ToString() == 格納方法)
			{
				magConfig.LoadStepCD = LENS2_Api.Magazine.Config.LoadStep.All;
			}
			else if (LENS2_Api.Magazine.Config.LoadStep.Even.ToString() == 格納方法)
			{
				magConfig.LoadStepCD = LENS2_Api.Magazine.Config.LoadStep.Even;
			}
			else if (LENS2_Api.Magazine.Config.LoadStep.Odd.ToString() == 格納方法)
			{
				magConfig.LoadStepCD = LENS2_Api.Magazine.Config.LoadStep.Odd;
			}
			else
			{
				//強制でテストNGにする時にAssert.Fail()で良かったか確認
				Assert.Fail();
			}

			magConfig.MagazineNo = マガジンNo;
			magConfig.PackageQtyX = Xパッケージ数;
			magConfig.PackageQtyY = Yパッケージ数;
			magConfig.StepNum = フレーム段数;

			return magConfig;
		}

	}
}
