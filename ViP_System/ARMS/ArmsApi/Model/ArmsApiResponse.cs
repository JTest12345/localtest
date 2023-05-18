using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class ArmsApiResponse
    {
         #region プロパティ
        /// <summary>
        /// エラー有無
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 工程ID
        /// </summary>
        public int? ProcNo { get; set; }

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 次投入可能設備
        /// </summary>
        public List<int> NextMachines { get; set; }

        /// <summary>
        /// NASCAロット
        /// </summary>
        public string NascaLotNo { get; set; }

        public string  MagazineNo { get; set; }

        public int? MacNo { get; set; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ArmsApiResponse()
        {
            this.IsError = false;
            NextMachines = new List<int>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (IsError)
            {
                sb.Append("[ERROR]");
            }
            else
            {
                sb.Append("[OK]");
            }
            sb.Append(ProcNo.ToString() + ",");

            if (NextMachines != null)
            {
                bool isFirst = true;
                foreach (int m in NextMachines)
                {
                    if (!isFirst) sb.Append(":");
                    sb.Append(m);
                    isFirst = false;
                }
            }
            sb.Append("," + Message);

            return sb.ToString();
        }

        #region Create
		//public static ArmsApiResponse Create(bool isError, string msg, int? procNo)
		//{
		//	return Create(isError, msg, "", procNo.Value);
		//}

        public static ArmsApiResponse Create(bool isError, string msg, string nascalotNo, int? procNo, string magazineNo, int? macno) //, string lineno を引数から除去 (2015/5/4 nyoshimoto)
        {
            ArmsApiResponse retv = new ArmsApiResponse();
            retv.IsError = isError;
            retv.NascaLotNo = nascalotNo;
            retv.Message = msg;
            if (retv.ProcNo.HasValue)
            {
                retv.ProcNo = procNo.Value;
            }
            retv.MagazineNo = magazineNo;
            if (macno.HasValue)
            {
                retv.MacNo = macno.Value;
            }

            //エラーの場合はロットログ保存
            if (isError && !string.IsNullOrEmpty(nascalotNo))
            {
                //NGログ保存
                //AsmLot.InsertLotLog(nascalotNo, DateTime.Now, msg, procNo.Value, magazineNo, true, lineno);

				//排出理由更新
				VirtualMag.UpdatePurgeReason(macno.Value, magazineNo, msg);
            }

            return retv;
        }
		//public static ArmsApiResponse Create(bool isError, string msg, string nascalotNo, int? procNo) 
		//{
		//	return Create(isError, msg, nascalotNo, procNo, string.Empty, null, ); 
		//}

		public static ArmsApiResponse Create(bool isError, string msg, int procNo, string magazineNo, int macno, string lineno) 
		{
            return Create(isError, msg, string.Empty, procNo, magazineNo, macno); //, linenoを引数から除去 (2015/5/4 nyoshimoto)
		}
        #endregion

    }
}
