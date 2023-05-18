using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace GEICS
{
    public class MachineFileInfo
    {


        /// <summary>
        /// ファイル内容を取得(行配列)
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>内容配列</returns>
        public static string[] GetMachineFileLineValue(string filePath)
        {
            return GetMachineFileLineValue(filePath, System.DateTime.Now);
        }

        public static string[] GetMachineFileLineValue(string filePath, DateTime startDT)
        {
            try
            {
                if (startDT.AddSeconds(10) <= System.DateTime.Now)
                {
                    //10秒の制限時間を超えた場合、エラー
                    throw new Exception(string.Format(Constant.MessageInfo.Message_50, filePath));
                }

                return File.ReadAllLines(filePath, System.Text.Encoding.Default);
            }
            catch (IOException)
            {
                //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("Filelock:{0}", filePath));
                Thread.Sleep(500);
                return GetMachineFileLineValue(filePath, startDT);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        /// <summary>
        /// WBマッピングファイルの作成
        /// </summary>
        /// <param name="targetDirPath">作成場所ディレクトリ</param>
        /// <param name="fileName">作成ファイル名(ロット番号)</param>
        /// <param name="mappingList">作成ファイル内容</param>
        public static void CreateMappingFile(string targetDirPath, string fileName, List<MappingDataInfo> mappingList)
        {
            try
            {
                if (!Directory.Exists(targetDirPath))
                {
                    Directory.CreateDirectory(targetDirPath);
                }

                using (StreamWriter sw = new StreamWriter(Path.Combine(targetDirPath, fileName + ".wbm"), false, Encoding.Default))
                {
                    foreach (MappingDataInfo mappingInfo in mappingList)
                    {
                        //#if TEST
                        //                        sw.Write("[" + mappingInfo.AddressNO + "]" + mappingInfo.InspectionNO + ",");
                        //#else
                        sw.Write(mappingInfo.InspectionNO + ",");
                        //#endif
                    }
                }
            }
            catch (Exception err)
            {
                throw err;
            }
        }

    }
}
