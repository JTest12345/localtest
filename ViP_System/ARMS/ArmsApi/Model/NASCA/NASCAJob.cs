using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.NASCA
{


    public enum NASCAJobCategory : int
    {
        WorkStart = 1,
        DBComplete = 12,
        OtherComplete = 14,
        WorkStartAndComplete = 21,
    }

    public interface INASCAJob
    {
        string GetCommandText();
        //NASCAJobQueueに入れる場合はEqualsの実装が必要。
    }


    public class NASCAJob : INASCAJob
    {
        /// <summary>
        /// Spider区切り文字
        /// </summary>
        public const char DATA_SPLITTER = ',';

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            NASCAJob job = (NASCAJob)obj;
            if (this.Mag.MagazineNo == job.Mag.MagazineNo)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public NASCAJobCategory JobCategory { get; set; }
        public int? ProcessID { get; set; }
        public string MachineNo { get; set; }
        public string StartMagazineNo { get; set; }
        public string CompleteMagazineNo { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? CompleteDT { get; set; }
        public string Stocker1 { get; set; }
        public string Stocker2 { get; set; }
        public string WorkStartEmpCD { get; set; }
        public string WorkCompleteEmpCD { get; set; }
        public string StartWafer { get; set; }
        public string EndWafer { get; set; }
        public string WaferChangerChange { get; set; }
        public List<Material> Materials { get; set; }

        public VirtualMag Mag { get; set; }

        protected NASCAJob() { }


        public static NASCAJob GetDBCompleteJob(VirtualMag mag)
        {
            NASCAJob job = new NASCAJob();
            job.Mag = mag;
            job.JobCategory = NASCAJobCategory.DBComplete;
            job.MachineNo = mag.CurrentLocation.MacNo.ToString();
            job.StartDT = mag.WorkStart;
            job.CompleteDT = mag.WorkComplete;
            job.CompleteMagazineNo = mag.MagazineNo;
            job.StartMagazineNo = mag.LastMagazineNo;
            job.ProcessID = mag.ProcNo;
            job.Stocker1 = mag.Stocker1.ToString();
            job.Stocker2 = mag.Stocker2.ToString();
            job.StartWafer = mag.StartWafer.ToString();
            job.EndWafer = mag.EndWafer.ToString();
            if (mag.WaferChangerChangeCount.HasValue == true)
            {
                job.WaferChangerChange = mag.WaferChangerChangeCount.ToString();
            }
            else
            {
                job.WaferChangerChange = "0";
            }

            return job;
        }

        public static NASCAJob GetMAPDBCompleteJob(VirtualMag mag)
        {

            NASCAJob job = new NASCAJob();
            job.Mag = mag;
            job.JobCategory = NASCAJobCategory.DBComplete;
            job.MachineNo = mag.CurrentLocation.MacNo.ToString();
            job.StartDT = mag.WorkStart;
            job.CompleteDT = mag.WorkComplete;
            job.CompleteMagazineNo = mag.MagazineNo;
            job.StartMagazineNo = mag.LastMagazineNo;
            job.ProcessID = mag.ProcNo;
            job.Stocker1 = mag.Stocker1.ToString();
            job.Stocker2 = mag.Stocker2.ToString();
            job.StartWafer = mag.StartWafer.ToString();
            job.EndWafer = mag.EndWafer.ToString();
            if (mag.WaferChangerChangeCount.HasValue == true)
            {
                job.WaferChangerChange = mag.WaferChangerChangeCount.ToString();
            }
            else
            {
                job.WaferChangerChange = "0";
            }

            job.Materials = mag.RelatedMaterials;
            return job;
        }



        public static NASCAJob GetWorkCompleteJob(VirtualMag mag)
        {
            NASCAJob job = new NASCAJob();
            job.Mag = mag;
            job.JobCategory = NASCAJobCategory.OtherComplete;
            job.MachineNo = mag.CurrentLocation.ToString();
            job.StartDT = mag.WorkStart;
            job.CompleteDT = mag.WorkComplete;
            job.CompleteMagazineNo = mag.MagazineNo;
            job.StartMagazineNo = mag.LastMagazineNo;
            job.ProcessID = mag.ProcNo;
            job.Stocker1 = mag.Stocker1.ToString();
            job.Stocker2 = mag.Stocker2.ToString();
            job.StartWafer = mag.StartWafer.ToString();
            job.EndWafer = mag.EndWafer.ToString();

            return job;
        }

        /// <summary>
        /// NASCA送信用文字列作成
        /// </summary>
        /// <returns></returns>
        public string GetCommandText()
        {
            string cmd;
            if (string.IsNullOrEmpty(this.StartWafer) == false)
            {

                cmd =
                  //NASCA.InlineNo + DATA_SPLITTER
                  //+ NASCA.InlineCategory + DATA_SPLITTER
                  ((int)this.JobCategory).ToString().PadLeft(3, '0') + DATA_SPLITTER
                  + this.ProcessID + DATA_SPLITTER
                  + this.MachineNo + DATA_SPLITTER
                  + this.StartMagazineNo + DATA_SPLITTER
                  + this.CompleteMagazineNo + DATA_SPLITTER
                  + this.StartDT.ToString() + DATA_SPLITTER
                  + this.CompleteDT.ToString() + DATA_SPLITTER
                  + "0-" + this.StartWafer + DATA_SPLITTER
                  + this.WaferChangerChange + "-" + this.EndWafer + DATA_SPLITTER
                  + "660" + DATA_SPLITTER
                  + "660"
                  + ",,"; //未使用項目MEL専用
            }
            else
            {
                cmd =
                  //NASCA.InlineNo + DATA_SPLITTER
                  //+ NASCA.InlineCategory + DATA_SPLITTER
                  ((int)this.JobCategory).ToString().PadLeft(3, '0') + DATA_SPLITTER
                  + this.ProcessID + DATA_SPLITTER
                  + this.MachineNo + DATA_SPLITTER
                  + this.StartMagazineNo + DATA_SPLITTER
                  + this.CompleteMagazineNo + DATA_SPLITTER
                  + this.StartDT.ToString() + DATA_SPLITTER
                  + this.CompleteDT.ToString() + DATA_SPLITTER
                  + this.Stocker1 + DATA_SPLITTER
                  + this.Stocker2 + DATA_SPLITTER
                  + "660" + DATA_SPLITTER
                  + "660"
                  + ",,"; //未使用項目MEL専用            
            }

            if (this.Materials != null)
            {
                cmd =
                  //NASCA.InlineNo + DATA_SPLITTER
                  "MAPDB" + DATA_SPLITTER
                  + ((int)this.JobCategory).ToString().PadLeft(3, '0') + DATA_SPLITTER
                  + this.ProcessID + DATA_SPLITTER
                  + this.MachineNo + DATA_SPLITTER
                  + this.StartMagazineNo + DATA_SPLITTER
                  + this.CompleteMagazineNo + DATA_SPLITTER
                  + this.StartDT.ToString() + DATA_SPLITTER
                  + this.CompleteDT.ToString() + DATA_SPLITTER
                  + "0-" + this.StartWafer + DATA_SPLITTER
                  + this.WaferChangerChange + "-" + this.EndWafer + DATA_SPLITTER
                  + "660" + DATA_SPLITTER
                  + "660";

                cmd += DATA_SPLITTER;

                bool isfirst = true;
                foreach (Material s in this.Materials)
                {
                    if (!isfirst) cmd += ":";
                    cmd += s.MaterialCd;
                    isfirst = false;
                }
                cmd += DATA_SPLITTER;
            }

            return cmd;
        }
    }
}
