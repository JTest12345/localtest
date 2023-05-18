using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLC
{
	public interface IPlc
	{
		bool GetBitForAI_MD(string memoryAddressNO);
		bool GetBit(string memoryAddressNO);
		void SetBit(string memoryAddressNO, bool statusFG);
		void SendMultiValue(string memoryAddress, int[] valueArray, string suffix);
		void SendMultiValue(string memoryAddress, string[] valueArray, string suffix);
		string SendString(string memoryAddressNO, string sendData);
		string Send1ByteData(string memoryAddressNO, int sendByteData);

		string GetData(string Addr, int num);
		string GetData(string memoryAddressNO, int length, string suffix, bool isConvB2A);
		string GetData(string memoryAddressNO, int length, string suffix);
	}
}
