using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.Machines;
using ARMS3.Model.PLC;
using ArmsApi.Model;
using ArmsApi;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace ARMS3.Model.Carriers
{
    public abstract class CarrierBase : ARMSThreadObject, ICarrier
    {
        private const string LASTMAGAZINESETTIME_XML = @"C:\ARMS\Config\LastMagazineSetTime_{0}.xml";

        /// <summary>
        /// PLC
        /// </summary>
        public IPLC Plc { get; set; }

        /// <summary>
        /// ライン番号
        /// </summary>
        public string LineNo { get; set; }

        /// <summary>
        /// キャリア番号
        /// </summary>
        public int CarNo { get; set; }
        /// <summary>
        /// アーム操作用のMutex
        /// </summary>
        public Mutex carrierMutex { get; set; }

        public Location LastMoveTo { get; set; }

        /// <summary>
        /// QRリーダー部の装置番号
        /// </summary>
        public int QRReaderMacNo { get; set; }

        /// <summary>
        /// 保持しているマガジン
        /// </summary>
        public virtual List<VirtualMag> HoldingMagazines { get; set; }
        
        /// <summary>
        /// 搬送時にインターロックチェックする対象のアドレス (キー：搬送対象装置のMacPoint)
        /// </summary>
        public SortedList<string, string> SemaphoreSlimList { get; set; }

        /// <summary>
        /// 電源ON
        /// </summary>
        public bool IsPowerON { get; set; }

        /// <summary>
        /// 自身が届く装置リスト
        /// </summary>
        protected List<IMachine> reachMachines { get; set; }

        public virtual Location MoveFromTo(Location moveFrom, Location moveTo, bool dequeueMoveFrom, bool isEmptyMagazine, bool resetNextProcessIdToCurrentProfileFirstProcNo, bool isCheckQR)
        {
            return moveTo;
        }

		public void setPlanTo(string planToAddress, string planToPoint)
		{
			try
			{
				if (string.IsNullOrEmpty(planToAddress)) 
				{
					return;
				}

				this.Plc.SetWordAsDecimalData(planToAddress, int.Parse(planToPoint));
			}
			catch (InvalidCastException ex)
			{
				Log.RBLog.Info("搬送予定先のポイントに数値以外が設定されています", ex);
				throw ex;
			}
		}

        /// <summary>
        /// PLCコマンド受付状態
        /// </summary>
        /// <returns></returns>
        public bool IsPLCReadyToCommand(string address)
        {
            string retv = this.Plc.GetBit(address);

            if (retv == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
            }
            else
            {
                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

		/// <summary>
		/// キャリア全体で保持しているマガジンを全て取得
		/// </summary>
		/// <returns></returns>
		public static List<VirtualMag> GetHoldingMagazines()
		{
			List<VirtualMag> retv = new List<VirtualMag>();

			IEnumerable<List<VirtualMag>> carMagList = LineKeeper.Carriers.Select(c => c.HoldingMagazines)
				.Where(m => m.Count != 0);

			foreach (List<VirtualMag> magList in carMagList)
			{
				retv.AddRange(magList);
			}

			return retv;
		}

        /// <summary>
        /// 最終のモールドオーブンプロファイル予約
        /// この値と違うものが常温CV先頭に来た場合は、
        /// モールド全オーブンの予約を実施
        /// </summary>
        public int LastMDOvenProfileReserve;

        /// <summary>
        /// 最後のダイボンドプロファイル予約
        /// この値と違うものが搭載で完成した場合は、
        /// 全オーブンの予約を実施
        /// </summary>
        public int LastDBOvenProfileReserve;

        /// <summary>
        /// QR照合異常発生
        /// </summary>
        public class QRMissMatchException : RobotException
        {
            public string VirtualMag { get; set; }
            public string RealMag { get; set; }
            public Location From { get; set; }

            public QRMissMatchException(string message, string virtualMag, string realMag, Location from, ICarrier robot)
                : base(message)
            {
                VirtualMag = virtualMag;
                RealMag = realMag;
                From = from;

                FrmErrHandle frm = new FrmErrHandle(robot, this);
                frm.ShowDialog();

                this.Method = frm.Method;
            }
        }

        /// <summary>
        /// 異常発生
        /// </summary>
        public class RobotException : ApplicationException
        {
            public ErrorHandleMethod Method { get; set; }

            public RobotException(string message)
                : base(message)
            {

            }
        }

        public class RobotStatusException : RobotException
        {
            public RobotStatusException(string message, ICarrier robot)
                : base(message)
            {
                FrmErrHandle frm = new FrmErrHandle(robot, message);
                frm.ShowDialog();

                this.Method = frm.Method;
            }
        }

        /// <summary>
        /// タイムアウト発生
        /// </summary>
        public class RobotTimeoutException : RobotException
        {
            public RobotTimeoutException(string message, ICarrier robot)
                : base(message)
            {
                FrmErrHandle frm = new FrmErrHandle(robot, message);
                frm.ShowDialog();

                this.Method = frm.Method;
            }
        }

        /// <summary>
        /// チャックミス発生
        /// </summary>
        public class RobotChuckMissException : RobotException
        {
            public RobotChuckMissException(string message, ICarrier robot)
                : base(message)
            {
                FrmErrHandle frm = new FrmErrHandle(robot, message);
                frm.ShowDialog();

                this.Method = frm.Method;
            }
        }


        /// <summary>
        /// マガジン排出
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public virtual void PurgeHandlingMagazine(VirtualMag mag, string reason)
        {

        }

        #region XML関係

        /// <summary>
        /// XMLファイル作成
        /// </summary>
        public void CreateLastMagazineSetTimeXMLFile()
        {
            string filepath = string.Format(LASTMAGAZINESETTIME_XML, this.CarNo);

            if (System.IO.File.Exists(filepath)) return;

            var doc = new XDocument();
            var root = new XElement("settings");
            doc.Add(root);
            doc.Save(filepath);
        }

        /// <summary>
        /// XMLファイル読み込み
        /// </summary>
        public void LoadLastMagazineSetTimeXMLFile()
        {
            this.CreateLastMagazineSetTimeXMLFile();

            string filepath = string.Format(LASTMAGAZINESETTIME_XML, this.CarNo);
            var doc = XDocument.Load(filepath);            

            IEnumerable<XElement> settings = doc.Elements("settings");
            if (settings == null) return;

            var skipMacNoList = new List<int>();
            IEnumerable<XElement> macnoElms = settings.Elements("macno");
            foreach (var elm in macnoElms)
            {
                XAttribute attr = elm.Attribute("key");
                if (attr == null) continue;

                int macNo;
                if (int.TryParse(attr.Value, out macNo) == false)
                {
                    continue;
                }

                DateTime dt;
                if (DateTime.TryParse(elm.Value, out dt) == false)
                {
                    continue;
                }
                IMachine imac = this.reachMachines.FirstOrDefault(l => l.MacNo == macNo);
                if (imac == null)
                {
                    skipMacNoList.Add(macNo);
                    continue;
                }

                if (imac.LastMagazineSetTime >= dt) continue;

                // 最終搬送時刻の設定
                imac.LastMagazineSetTime = dt;
            }
            if (skipMacNoList.Any() == true)
            {
                string msg = "lineconfigに存在しない または 設備マスタが有効でない為、次の装置はファイルからの最終搬送時刻の読み込みをスキップ\r\n";
                msg += $"   ファイル={filepath}";
                foreach(var macNo in skipMacNoList)
                {
                    msg += $"\r\n   macno={macNo}";
                }
                Log.RBLog.Info(msg);
            }
        }

        /// <summary>
        /// XMLファイル書き込み
        /// </summary>
        /// <param name="macno">最終搬送時刻を更新する装置番号</param>
        public void SaveLastMagazineSetTimeXMLFile(int macno)
        {
            // 最初に最終搬送時刻を更新する
            LineKeeper.GetMachine(macno).LastMagazineSetTime = DateTime.Now;

            // ファイルの有無チェック (なければ、ファイル作成)
            this.CreateLastMagazineSetTimeXMLFile();

            string filepath = string.Format(LASTMAGAZINESETTIME_XML, this.CarNo);
            var doc = XDocument.Load(filepath);
            
            // 一度、ファイル内のデータを全てクリアする
            doc.RemoveNodes();

            XElement root = new XElement("settings");
            doc.Add(root);
            
            // 手の届く装置の内、最終搬送時刻を一回以上更新した装置の装置番号 + 最終搬送時刻を全てファイルに書き出す
            foreach (var iMachine in this.reachMachines.OrderBy(l => l.MacNo))
            {
                // 一度も搬送していない装置は無視する
                if (iMachine.LastMagazineSetTime == DateTime.MinValue) continue;

                XElement elm = new XElement("macno");
                elm.Value = iMachine.LastMagazineSetTime.ToString("yyyy/MM/dd HH:mm:ss");
                XAttribute attr = new XAttribute("key", iMachine.MacNo.ToString());
                elm.Add(attr);
                root.Add(elm);
            }

            doc.Save(filepath);
        }

        //private void saveTime(DateTime dt, string node)
        //{
        //    XDocument doc;
        //if (System.IO.File.Exists(LAST_ACTION_RECORD_XML))
        //    {
        //        doc = XDocument.Load(LAST_ACTION_RECORD_XML);
        //    }
        //    else
        //    {
        //        doc = new XDocument();
        //    }

        //    XElement root = doc.Element("settings");
        //    if (root == null)
        //    {
        //        root = new XElement("settings");
        //        doc.Add(root);
        //    }

        //    XElement elm = root.Element(node);
        //    if (elm == null)
        //    {
        //        elm = new XElement(node);
        //        root.Add(elm);
        //    }

        //    elm.Value = dt.ToString();
        //    doc.Save(LAST_ACTION_RECORD_XML);
        //}


        //protected DateTime loadRelayPlcData(string carrierNo)
        //{
        //    string xmlfilename = carrierNo.ToString() + ".xml";
        //    XDocument doc;
        //    if (System.IO.File.Exists(xmlfilename))
        //    {
        //        doc = XDocument.Load(xmlfilename);
        //    }
        //    else
        //    {
        //        return DateTime.MinValue;
        //    }

        //    XElement setting = doc.Element("settings").Element(node);

        //    if (setting == null)
        //    {
        //        return DateTime.MinValue;
        //    }

        //    DateTime dt;
        //    if (DateTime.TryParse(setting.Value, out dt))
        //    {
        //        return dt;
        //    }
        //    else
        //    {
        //        return DateTime.MinValue;
        //    }
        //}

        //protected void deleteRelayPlcData(string carrierNo)
        //{
        //    string xmlfilename = carrierNo.ToString() + ".xml";
        //    XDocument doc;
        //    if (System.IO.File.Exists(xmlfilename))
        //    {
        //        System.IO.File.Delete(xmlfilename);
        //    }
        //}
        #endregion

    }
}
