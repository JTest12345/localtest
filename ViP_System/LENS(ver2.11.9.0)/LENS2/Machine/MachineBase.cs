﻿using LENS2_Api;
using Newtonsoft.Json;
using PLC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2.Machine
{
	public class MachineBase
	{
		/// <summary>
		/// 装置種類
		/// </summary>
		public string ClassNM { get; set; }

		///// <summary>
		///// 装置番号
		///// </summary>
		//public int MachineNO { get; set; }

		/// <summary>
		/// 設備番号
		/// </summary>
		public string NascaPlantCD { get; set; }

		/// <summary>
		/// 号機名
		/// </summary>
		public string MachineNM { get; set; }

		public string WorkStatus { get; set; }
				
		public IPlc Plc { get; set; }

		public string WatchingDirectoryPath { get; set; }

		public class MainteTargetMachineAddress
		{
			public string AddressNm { get; set; }
			public string Address { get; set; }
			public bool NumRestrictFg { get; set; }
			public bool BitRestrictFg { get; set; }
			public string Comment { get; set; }
		}

		public static IMachine GetMachine(string plantcd)
		{
			MachineInfo machine = MachineInfo.GetData(plantcd);

			IMachine mac;
			switch (machine.ClassCd)
			{
				case "Wirebonder":
					string raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Wirebonder.xml"), Encoding.UTF8);
					mac = JsonConvert.DeserializeObject<Wirebonder>(raw);
					break;

				case "Inspector":
					raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Inspector.xml"), Encoding.UTF8);
					mac = JsonConvert.DeserializeObject<Inspector>(raw);
					break;

                case "Inspector_AISIS":
                    raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Inspector_AISIS.xml"), Encoding.UTF8);
                    mac = JsonConvert.DeserializeObject<Inspector_AISIS>(raw);
                    break;

                case "Mold":
					raw = File.ReadAllText(Path.Combine(Properties.Settings.Default.ConfigFilePath, "Mold.xml"), Encoding.UTF8);
					mac = JsonConvert.DeserializeObject<Mold>(raw);

					break;

				default:
					throw new ApplicationException(string.Format("想定外の装置種類が存在します。装置種類:{0}", machine.ClassNm));
			}
			mac.NascaPlantCD = machine.NascaPlantCd;
			mac.ClassNM = machine.ClassNm;
			mac.MachineNM = machine.MachineNm;
			mac.WatchingDirectoryPath = machine.WatchingDirectoryPath;

			mac.Plc = new Keyence(machine.PlcIpAddress, machine.PlcPort);

			return mac;
		}


		/// <summary>
		/// 装置から取得したマガジン番号(ロット番号)をロット番号にして返す
		/// </summary>
		/// <returns></returns>
		public string GetHoldMagazineLotNo(string address)
		{
#if DEBUG
            //string qrData = "30 M001447";

            string qrData = this.Plc.GetData(address, 10, Keyence.Suffix_H, true);
            if (string.IsNullOrWhiteSpace(qrData))
            {
                throw new ApplicationException("QR読取データが不正です。");
            }
#else
			string qrData = this.Plc.GetData(address, 10, Keyence.Suffix_H, true);
			if (string.IsNullOrWhiteSpace(qrData))
			{
				throw new ApplicationException("QR読取データが不正です。");
			}
#endif
			InfoLog(string.Format("[QR読み取り文字] {0}", qrData)); 
			return LENS2_Api.ARMS.WorkMagazine.GetDataFromLabel(qrData).LotNo;
		}

		/// <summary>
		/// 16bitのビット列(文字列)の配列を数値の配列へ変換
		/// </summary>
		/// <param name="originalData">2進値(文字列)の配列（例："0","0","0","0","0","0","0","0","0","0","0","1","0","0","0","0"）</param>
		/// <returns>16要素を2進⇒16進数値変換した結果（例："0000000000010000"⇒16）</returns>
		public static int[] Convert2ByteBinaryArrayToDecimalArray(string[] originalData)
		{
			int bitNum = 16;
			int elementCt = (int)Math.Ceiling((double)originalData.Length / (double)bitNum);

			int[] convertedData = new int[elementCt];

			for (int i = 0; i < convertedData.Length; i++)
			{
                int remainder = originalData.Length - bitNum * i;

				if (remainder >= bitNum)
				{
                    string bitJoinStr = string.Join("", originalData.Skip(bitNum * i).Take(bitNum).Reverse().ToArray());
					convertedData[i] = Convert.ToInt32(bitJoinStr, 2);
				}
				else
				{
                    string bitJoinStr = string.Join("", originalData.Skip(bitNum * i).Take(remainder).Reverse().ToArray());
					convertedData[i] = Convert.ToInt32(bitJoinStr, 2);
				}
			}

			return convertedData;
		}

        public void HideLog(string message)
        {
            Log.Info(this.ClassNM, this.MachineNM, message, false);
        }

		public static void CommonHideLog(string message)
		{
			Log.Info("共通処理", "共通処理", message, false);
		}

		public void InfoLog(string message)
		{
			Log.Info(this.ClassNM, this.MachineNM, message, true);
		}
		public void InfoLog(string message, string lotNo)
		{
			message = string.Format("ロットNo:{0} / {1}", lotNo, message);
			Log.Info(this.ClassNM, this.MachineNM, message, true);
		}
		public void InfoLog(string message, string lotNo, string magazineNo)
		{
			message = string.Format("ロットNo:{0} / マガジンNo:{1} / {2}", lotNo, magazineNo, message);
		}
		public void ExclamationLog(string message)
		{
			Log.Exclamation(this.ClassNM, this.MachineNM, message, true);
		}
		public void ExclamationLog(string message, string lotNo)
		{
			message = string.Format("ロットNo:{0} / {1}", lotNo, message);
			Log.Exclamation(this.ClassNM, this.MachineNM, message, true);
		}
		public void ExclamationLog(string message, string lotNo, string magazineNo)
		{
			message = string.Format("ロットNo:{0} / マガジンNo:{1} / {2}", lotNo, magazineNo, message);
			Log.Exclamation(this.ClassNM, this.MachineNM, message, true);
		}
		public void ErrorLog(string message)
		{
			Log.Error(this.ClassNM, this.MachineNM, message, true);
		}
		public void ErrorLog(string message, string lotNo)
		{
			message = string.Format("ロットNo:{0} / {1}", lotNo, message);
			Log.Error(this.ClassNM, this.MachineNM, message, true);
		}
		public void ErrorLog(string message, string lotNo, string magazineNo)
		{
			message = string.Format("ロットNo:{0} / マガジンNo:{1} / {2}", lotNo, magazineNo, message);
			Log.Error(this.ClassNM, this.MachineNM, message, true);
		}
	}
}