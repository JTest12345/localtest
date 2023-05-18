using ArmsWorkTransparency.Model.PLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsWorkTransparency.Model.PLC
{
	public enum Maker
	{
		Mitsubishi,
		Omron,
		Keyence,
	}

	public enum Socket
	{
		Tcp,
		Udp,
	}

	public enum CommandType
	{
		RD,
		RDS,
		WR,
		WRS,
	}

	partial class Common
	{
		public static IPLC GetInstance(string makernm, string ipAddress, int port, string host, string socket)
		{
			if (makernm == Maker.Mitsubishi.ToString())
			{
				if (socket == Socket.Tcp.ToString())
				{
					return new MitsubishiTcp(ipAddress, port);
				}
				else 
				{
					return new Mitsubishi(ipAddress, port);
				}
			}
			else if (makernm == Maker.Keyence.ToString())
			{
				return new RelayMachinePLC(ipAddress, port);
			}
			else if (makernm == Maker.Omron.ToString())
			{
				return new Omron(ipAddress, port, host);
			}
			else 
			{
				throw new Exception(string.Format("想定外のPLCメーカーが設定されています。 設定名:{0}", makernm));
			}
		}

		public static IPLC GetInstance(string makernm, string ipAddress, int port, string host) 
		{
			return GetInstance(makernm, ipAddress, port, host, Socket.Udp.ToString());
		}

		/// <summary>
		/// PLCのBIT型ONの場合の値
		/// </summary>
		public const string BIT_ON = "1";

		/// <summary>
		/// PLCのBIT型OFFの場合の値
		/// </summary>
		public const string BIT_OFF = "0";

        public const string BIT_ERROR = "ERROR";
        
	}

    public interface IPLC
    {
        string IPAddress { get; set; }
        int Port { get; set; }

        string GetBit(string hexAddressWithDeviceNM);
        string GetBit(string hexAddressWithDeviceNM, int length);
        string[] GetBitArray(string hexAddressWithDeviceNM, int length);

        void SetBit(string hexAddressWithDeviceNM, int points, string data);
        
        string GetMagazineNo(string hexAddressWithDeviceNm);
        string GetMagazineNo(string hexAddressWithDeviceNm, int wordLength);
        string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided);
        string GetMagazineNo(string[] plcResponseBitArray, bool notDevided);

		string GetMachineNo(string hexAddressWithDeviceNm);
		string GetMachineNo(string hexAddressWithDeviceNm, int wordLength);

        string GetString(string hexAddressWithDeviceNm, int wordLength);
		bool SetString(string hexAddressWithDeviceNm, string data);
		string GetWord(string hexAddressWithDeviceNm, int length);

        int GetWordAsDecimalData(string hexAddressWithDeviceNM);
		int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength);
        void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data);
        
        DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM);
        DateTime? GetWordsAsDateTime(string[] res);

		bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue);
    }
}
