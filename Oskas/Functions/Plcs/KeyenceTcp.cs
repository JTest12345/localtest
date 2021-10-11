using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oskas.Functions.Plcs;

namespace Oskas.Functions.Plcs
{
    public class KeyenceTcp : IPLC
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }

        public _KeyenceTcp Plc { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KeyenceTcp(string address, int port) 
        {
            this.IPAddress = address;
            this.Port = port;

            Plc = _KeyenceTcp.GetInstance();
        }

        public string GetBit(string hexAddressWithDeviceNM)
        {
            return Plc.GetBit(hexAddressWithDeviceNM, this.IPAddress, this.Port); 
        }

        public string GetBit(string hexAddressWithDeviceNM, int length)
        {
            return string.Join("", Plc.GetBit(hexAddressWithDeviceNM, length, this.IPAddress, this.Port)).Trim();
        }

        public string[] GetBitArray(string hexAddressWithDeviceNM, int length)
        {
            return Plc.GetBit(hexAddressWithDeviceNM, length, this.IPAddress, this.Port);
        }

        public void SetBit(string hexAddressWithDeviceNM, int points, string data)
        {
            Plc.SetBit(hexAddressWithDeviceNM, data, this.IPAddress, this.Port);
        }

        public string GetMagazineNo(string hexAddressWithDeviceNm)
        {
            return this.GetMagazineNo(hexAddressWithDeviceNm, true);
        }

        public string GetMagazineNo(string hexAddressWithDeviceNm, int wordLength)
        {
            return Plc.GetMagazineNo(hexAddressWithDeviceNm, true, this.IPAddress, this.Port, wordLength);
        }

        public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided)
        {
            return Plc.GetMagazineNo(hexAddressWithDeviceNm, notDevided, this.IPAddress, this.Port, _KeyenceTcp.MAGAZINE_NO_WORD_LENGTH);
        }

        public string GetMagazineNo(string[] plcResponseBitArray, bool notDevided)
        {
            return Plc.GetMagazineNo(plcResponseBitArray, notDevided);
        }

		public string GetMachineNo(string hexAddressWithDeviceNm)
		{
			return GetMachineNo(hexAddressWithDeviceNm, _KeyenceTcp.MACHINE_NO_WORD_LENGTH);
		}

		public string GetMachineNo(string hexAddressWithDeviceNm, int wordLength)
		{
			return Plc.GetMachineNo(hexAddressWithDeviceNm, this.IPAddress, this.Port, wordLength);
		}

        public string GetString(string hexAddressWithDeviceNm, int wordLength)
        {
            return GetString(hexAddressWithDeviceNm, wordLength, false);
        }

        public string GetString(string hexAddressWithDeviceNm, int wordLength, bool isBigEndian)
        {
            return Plc.GetString(hexAddressWithDeviceNm, wordLength, this.IPAddress, this.Port, isBigEndian);
        }


		public bool SetString(string hexAddressWithDeviceNm, string data)
		{
			Plc.SetString(hexAddressWithDeviceNm, data, this.IPAddress, this.Port);

			return true;
		}

        public int GetWordAsDecimalData(string hexAddressWithDeviceNM)
        {
			return GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
        }

		public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength)
		{
            return Plc.GetWordAsDecimalData(hexAddressWithDeviceNM, this.IPAddress, this.Port);
        }

        public void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
        {
            Plc.SetWordAsDecimalData(hexAddressWithDeviceNM, data, this.IPAddress, this.Port);
        }

		public string GetWord(string hexAddressWithDeviceNM, int length) 
		{
			return Plc.GetWord(hexAddressWithDeviceNM, this.IPAddress, this.Port);
		}

        public DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM)
        {
            DateTime? retv = Plc.GetWordsAsDateTime(hexAddressWithDeviceNM, this.IPAddress, this.Port).Value;
            if(retv.HasValue == false)
            {
                //異常終了
                throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
            }

            return retv.Value;
        }
        
        public DateTime? GetWordsAsDateTime(string[] res)
        {
            return Plc.GetWordsAsDateTime(res);
        }

        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue)
        {
            return Plc.WatchBit(hexAddressWithDeviceNM, timeout, exitValue, this.IPAddress, this.Port);
        }

        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue, Action act)
        {
            return Plc.WatchBit(hexAddressWithDeviceNM, timeout, exitValue, this.IPAddress, this.Port, act);
        }

        public bool Ping(string host, int timeout, int retryTimes)
        {
            return Plc.Ping(host, timeout, retryTimes);
        }
    }
}
