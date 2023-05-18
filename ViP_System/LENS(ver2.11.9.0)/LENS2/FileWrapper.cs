using LENS2_Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LENS2
{
	class FileWrapper
	{
		public static string[] ReadAllLines(string filePath)
		{
			return ReadAllLines(filePath, DateTime.Now);
		}
		
		public static string[] ReadAllLines(string filePath, DateTime fileAccessStartDT)
		{
			try
			{
				if (fileAccessStartDT.AddSeconds(10) <= System.DateTime.Now)
				{
					//10秒の制限時間を超えた場合、エラー
					throw new ApplicationException(string.Format("ファイル操作の制限時間を超えました。ファイルがロックされている可能性があります。ファイルパス:{0}", filePath));
				}

                // MMファイル中のマガジン番号列のデータ末に改行文字が入ってるようで、行データが途中で配列の別要素として格納されてしまった為、下記はコメントアウト
                //return System.IO.File. .ReadAllLines(filePath, GetEncoding()).ToArray();

                string text = System.IO.File.ReadAllText(filePath, GetEncoding());

                return text.Split('\n');
			}
			catch (IOException)
			{
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", filePath));
				Thread.Sleep(500);
				return ReadAllLines(filePath, fileAccessStartDT);
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		public static Encoding GetEncoding()
		{
			return Encoding.GetEncoding(Config.Settings.EncodingString);
		}
	}
}
