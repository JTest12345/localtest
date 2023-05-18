using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Carriers
{
    public interface ICarrier
    {
        /// <summary>
        /// ライン番号
        /// </summary>
        string LineNo { get; set; }

        /// <summary>
        /// キャリア番号
        /// </summary>
        int CarNo { get; set; }

        /// <summary>
        /// キャリア名
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// PLC
        /// </summary>
        IPLC Plc { get; set; }

		/// <summary>
		/// 保持しているマガジン
		/// </summary>
		List<VirtualMag> HoldingMagazines { get; set; }

        /// <summary>
        /// 搬送時にインターロックチェックする対象のアドレス (キー：搬送対象装置のMacPoint)
        /// </summary>
        SortedList<string, string> SemaphoreSlimList { get; set; }
        
        /// <summary>
        /// QRリーダー部の装置番号
        /// </summary>
        int QRReaderMacNo { get; set; }

        /// <summary>
        /// 電源ON
        /// </summary>
        bool IsPowerON { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moveFrom"></param>
        /// <param name="moveTo"></param>
        /// <param name="dequeueMoveFrom"></param>
        /// <param name="isEmptyMagazine"></param>
        /// <param name="resetNextProcessIdToCurrentProfileFirstProcNo"></param>
        /// <param name="isCheckQR"></param>
        /// <returns></returns>
        Location MoveFromTo(Location moveFrom, Location moveTo, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo, bool isCheckQR);

        /// <summary>
        /// スレッド処理開始
        /// 開始済みの場合は無視
        /// </summary>
        /// <returns></returns>
        void RunWork();

        /// <summary>
        /// スレッド処理通常停止要求
        /// </summary>
        bool StopRequested { get; set; }

        /// <summary>
        /// スレッド動作状態取得
        /// </summary>
        /// <returns></returns>
        WorkStatus GetWorkStatus();

        void PurgeHandlingMagazine(VirtualMag mag, string reason);

        void LoadLastMagazineSetTimeXMLFile();
    }
}
