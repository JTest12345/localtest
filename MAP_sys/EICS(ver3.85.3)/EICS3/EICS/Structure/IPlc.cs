using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Structure
{
	public interface IPlc
	{
		void Dispose();

		bool GetBool(string hexAddressWithDeviceNM);

		string GetBit(string hexAddressWithDeviceNM);
		string GetBit(string hexAddressWithDeviceNM, int length);

		void SetBit(string hexAddressWithDeviceNM, int points, string data);
		string GetDataAsString(string hexAddressWithDeviceNM, int points, string dataType);

		string GetWord(string hexAddressWithDeviceNM);
		string GetWord(string hexAddressWithDeviceNM, int length);

		string GetString(string hexAddressWithDeviceNM, int wordLength, bool doSwapFg, bool isBigEndian);
		bool SetString(string hexAddressWithDeviceNM, string data);
		bool SetString(string hexAddressWithDeviceNM, string data, string encStr);

		int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength);
		void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data);

        string GetMagazineNo(string hexAddressWithDeviceNM, int wordLength);

        DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM);

    }
}
