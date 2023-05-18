using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.NASCA
{
    public enum NASCAStatus : int
    {
        OK = 0,
        NG = 1,
        Retry = 2,
        TimeOut = 3,
    }

    public class NASCAResponse
    {

        private const string FRAME_LOADER_LOTNO_PREFIX = "ロットNO:";

        /// <summary>
        /// NASCA応答データのデータ長
        /// </summary>
        private const int DATA_LENGTH = 6;

        /// <summary>
        /// 次作業装置の区切り文字
        /// </summary>
        private const char DATA_SPLITTER = ':';

        /// <summary>
        /// 回答ステータス
        /// </summary>
        public NASCAStatus Status { get; set; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// メッセージ内に含まれているロット番号
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// SPIDERから戻ってくる生の文字列
        /// </summary>
        public string OriginalMessage { get; set; }

        /// <summary>
        /// private コンストラクタ
        /// </summary>
        public NASCAResponse() { }


        /// <summary>
        /// 応答ファイル解析用
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static NASCAResponse ParseResponseFile(string path, int retry)
        {
            if (File.Exists(path) != true)
            {
                throw new ArmsException("NASCA応答ファイル行方不明");
            }

            #region ファイル長監視　5ms

            long length = 0;
            while (length == 0)
            {
                FileInfo fi = new FileInfo(path);
                length = fi.Length;
                System.Threading.Thread.Sleep(5);
                fi = new FileInfo(path);
                long length2 = fi.Length;
                if (length != length2)
                {
                    length = 0;
                }
            }
            #endregion

            NASCAResponse retv = new NASCAResponse();

            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.GetEncoding("shift-jis")))
                {
                    if (sr.Peek() < 0)
                    {
                        throw new ArmsException("NASCA応答ファイルに中身がありません");
                    }

                    retv.OriginalMessage = sr.ReadLine();
                }
            }
            catch (IOException ex)
            {
                //別プロセスがファイルをロックしている
                //3回までリトライ
                if (retry <= 3)
                {
                    //2秒待機して再帰
                    System.Threading.Thread.Sleep(2000);
                    return ParseResponseFile(path, ++retry);
                }
                else
                {
                    throw ex;
                }
            }

            string[] data = retv.OriginalMessage.Split(Spider.SPL);
            if (data.Length != DATA_LENGTH)
            {
                retv.Status = NASCAStatus.NG;
                retv.Message = "NASCA応答ファイルの項目数が不正です";
                return retv;
            }

            retv.Status = (NASCAStatus)Enum.Parse(typeof(NASCAStatus), data[1].Trim());
            string[] nextProcessList = data[3].Split(DATA_SPLITTER);

            retv.Message = data[5];

            if (retv.Message.Contains(FRAME_LOADER_LOTNO_PREFIX))
            {
                retv.LotNo = retv.Message.Remove(0, retv.Message.IndexOf(FRAME_LOADER_LOTNO_PREFIX) + FRAME_LOADER_LOTNO_PREFIX.Length);
            }
            else
            {
                retv.LotNo = null;
            }

            return retv;
        }


        public static NASCAResponse GetNGResponse(string msg)
        {
            NASCAResponse res = new NASCAResponse();
            res.Status = NASCAStatus.NG;
            res.Message = msg;
            if (res.OriginalMessage == null) res.OriginalMessage = msg;
            return res;
        }

        public static NASCAResponse GetOKResponse()
        {
            NASCAResponse res = new NASCAResponse();
            res.Status = NASCAStatus.OK;
            if (res.OriginalMessage == null) res.OriginalMessage = "OK";
            if (res.Message == null) res.Message = "OK";
            return res;
        }
    }
}
