using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class WorkCndEditModel
    {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plantcd"></param>
        public WorkCndEditModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(this.PlantCd);

            WorkCondition[] conds = WorkCondition.SearchCondition(null, null, null, false);
            this.WorkCondList = new Dictionary<int, WorkCondition>();
            for (int i = 0; i < conds.Length; i++)
            {
                this.WorkCondList.Add(i, conds[i]);
            }

            conds = this.Mac.GetWorkConditions(DateTime.Now, DateTime.Now);
            this.Conditions = new Dictionary<int, WorkCondition>();
            for (int i = 0; i < conds.Length; i++)
            {
                this.Conditions.Add(i, conds[i]);
            }
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        public WorkCondition EditTarget { get; set; }

        public MachineInfo Mac { get; set; }

        public Dictionary<int, WorkCondition> WorkCondList { get; set; }

        /// <summary>
        /// 装置に割り付け済みの条件一覧
        /// </summary>
        public Dictionary<int, WorkCondition> Conditions;

        public void RemoveWorkCnd(WorkCondition cnd, DateTime removedt)
        {
            cnd.EndDt = removedt.AddSeconds(-1);
            Mac.DeleteInsertWorkCond(cnd);
        }

        public void InsertNew()
        {
            this.EditTarget.StartDt = DateTime.Now;
            this.Mac.DeleteInsertWorkCond(this.EditTarget);
        }

        public WorkCondition ParseWorkCond(string barcode, int condindex)
        {
            if (string.IsNullOrEmpty(barcode)) throw new ApplicationException("バーコード内容が不正です");

            string[] inputval = barcode.Split(' ');
            if (inputval.Length != 2) throw new ApplicationException("バーコード内容が不正です");

            //string header = inputval[0];
            //WorkCondition cond = WorkCondition.GetConditoinFromHeader(header);
            //if (cond == null)
            //{
            //    throw new ApplicationException("製造条件マスタが存在しません");
            //}
            WorkCondition cond = this.WorkCondList[condindex];

            cond.CondVal = inputval[1];
            cond.StartDt = DateTime.Now;
            return cond;
        }
    }
}