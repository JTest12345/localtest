﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api.ARMS
{
	public class WorkResult
	{
		public string LotNo { get; set; }
		public int ProcNo { get; set; }
		public string WorkCd { get; set; }
		public int MacNo { get; set; }
		public string PlantCd { get; set; }
		public DateTime StartDt { get; set; }
		public DateTime? EndDt { get; set; }
		public bool RevDeployFg { get; set; }
        public int DummyMacNo { get; set; }
		
		public static List<WorkResult> GetDatas(string lotno, int? divideIdentityNo)
		{
			return GetDatas(lotno, null, null, false, divideIdentityNo);
		}
		public static WorkResult GetData(string lotno, int procno) 
		{
			return GetData(lotno, procno, null);
		}
		public static WorkResult GetData(string lotno, int procno, int? divideIdentityNo)
		{
			List<WorkResult> resultList = GetDatas(lotno, procno, null, false, divideIdentityNo);
			if (resultList.Count == 0)
			{
				return null;
			}
			else
			{
				return resultList.Single();
			}
		}
		public static List<WorkResult> GetDatas(string lotno, int? procno, int? macno, bool isTargetOnlyComplete, int? divideIdentityNo)
		{
			List<WorkResult> retv = new List<WorkResult>();

			using (SqlConnection con = new SqlConnection(Config.Settings.ArmsConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

                string sql = @" SELECT TnTran.lotno, TnTran.procno, TnTran.macno, TnTran.startdt, TnTran.enddt, TmMachine.revdeployfg, TmProcess.autoupdmachineno, TmProcess.workcd
								FROM TnTran WITH (nolock) INNER JOIN TmMachine WITH (nolock) ON TnTran.macno = TmMachine.macno
                                INNER JOIN TmProcess WITH (nolock) ON TnTran.procno = TmProcess.procno
								WHERE (TnTran.DelFG = 0) ";

				if (!string.IsNullOrEmpty(lotno))
				{
					if (divideIdentityNo.HasValue)
					{
						sql += " AND (lotno = @LotNO OR lotno = @DivideLotNO)";
						cmd.Parameters.Add("@DivideLotNO", SqlDbType.NVarChar).Value = lotno + "_#" + divideIdentityNo.Value.ToString();
					}
					else
					{
						sql += " AND lotno = @LotNO ";
					}
					cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
				}

				if (procno.HasValue)
				{
                    sql += " AND TnTran.procno = @ProcNO ";
					cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;
				}

				if (macno.HasValue)
				{
					sql += " AND TnTran.macno = @MacNO ";
					cmd.Parameters.Add("@MacNO", SqlDbType.BigInt).Value = macno;
				}

				if (isTargetOnlyComplete)
				{
					sql += " AND enddt IS NOT NULL ";
				}

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					int ordEndDt = rd.GetOrdinal("enddt");
                    int ordDummyMacNo = rd.GetOrdinal("autoupdmachineno");

					while (rd.Read())
					{
						WorkResult r = new WorkResult();
						r.LotNo = rd["lotno"].ToString().Trim();
						r.ProcNo = Convert.ToInt32(rd["procno"]);
						r.WorkCd = rd["workcd"].ToString().Trim();
						r.MacNo = Convert.ToInt32(rd["macno"]);
						r.StartDt = Convert.ToDateTime(rd["startdt"]);
						r.RevDeployFg = Convert.ToBoolean(rd["revdeployfg"]);
						if (!rd.IsDBNull(ordEndDt))
						{
							r.EndDt = rd.GetDateTime(ordEndDt);
						}
                        if (!rd.IsDBNull(ordDummyMacNo))
                        {
                            r.DummyMacNo = Convert.ToInt32(rd[ordDummyMacNo]);
                        }

						retv.Add(r);
					}
				}
			}

			return retv;
		}

//		public static Dictionary<int, bool> GetDatas2(string lotno, int? procno, int? macno, bool isTargetOnlyComplete)
//		{
//			Dictionary<int, bool> retv = new Dictionary<int, bool>();

//			using (SqlConnection con = new SqlConnection(Config.Settings.ArmsConnectionString))
//			using (SqlCommand cmd = con.CreateCommand())
//			{
//				con.Open();

//				string sql = @" SELECT procno, revdeployfg
//								FROM TnTran WITH (nolock) INNER JOIN TmMachine WITH(nolock) ON TmMachine.macno = TnTran.macno
//								WHERE (DelFG = 0) ";

//				if (!string.IsNullOrEmpty(lotno))
//				{
//					sql += " AND lotno = @LotNO ";
//					cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
//				}

//				if (procno.HasValue)
//				{
//					sql += " AND procno = @ProcNO ";
//					cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno;
//				}

//				if (macno.HasValue)
//				{
//					sql += " AND macno = @MacNO ";
//					cmd.Parameters.Add("@MacNO", SqlDbType.BigInt).Value = macno;
//				}

//				if (isTargetOnlyComplete)
//				{
//					sql += " AND enddt IS NOT NULL ";
//				}


//				cmd.CommandText = sql;
//				using (SqlDataReader rd = cmd.ExecuteReader())
//				{
//					int ordEndDt = rd.GetOrdinal("enddt");
//					while (rd.Read())
//					{
//						int procNo = Convert.ToInt32(rd["procno"]);
//						bool revDeployFg = Convert.ToBoolean(rd["revdeployfg"]);

//						retv.Add(procNo, revDeployFg);
//					}
//				}
//			}

//			return retv;
//		}

		/// <summary>
		/// 指定ロットが該当装置種類の作業をしているか確認
		/// </summary>
		/// <param name="lotno"></param>
		/// <param name="macclass"></param>
		/// <returns></returns>
		public static bool HasPassedProcess(string lotno, int procno, int? divideIdentityNo)
		{
			WorkResult result = GetData(lotno, procno, divideIdentityNo);

			//foreach (WorkResult result in results)
			//{
			//	MachineInfo m = MachineInfo.GetData(result.PlantCd);
			//	if (m.ClassCd == macclass.ToString())
			//	{
			//		retv = true;
			//		break;
			//	}
			//}

			if (result != null)
			{
                //検査機の仕様で投入しなかった場合でもダミー実績が記録されるので、ダミー設備(TmProcess.autoupdmachineno参照)の記録があれば作業無し扱いにする
                if (result.MacNo == result.DummyMacNo)
                {
                    return false;
                }
                else
                {
                    return true;
                }
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// フレームの搭載位置をマガジン内で上下に逆転させるかのチェック
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="targetProcNo">自工程と比較対象になる工程の工程CD</param>
		/// <param name="fromProcNo">マップデータを参照しようとしているタイミングの工程CD</param>
		/// <returns></returns>
		public static bool HasNeedReverseFramePlacement(string lotNo, int toProcNo, int currentProcNo, bool isEndTimingRef)
		{
            Log.Info(lotNo, "上下反転チェックパラメタ"
                , string.Format("参照先Proc:{0} 参照元Proc:{1} 参照元の完了時Fg:{2}", toProcNo, currentProcNo, isEndTimingRef));

			List<WorkResult> workResults = WorkResult.GetDatas(lotNo, null, null, false, 1);

			Dictionary<int, int> frameDeployStates = getFrameDeployState(workResults);

            // ログ処理
            string strDeployState = string.Empty;
            foreach (KeyValuePair<int, int> deployState in frameDeployStates)
            {
                strDeployState += string.Format("\r\n【工程:{0} 状態:{1}】", deployState.Key, deployState.Value);
            }

            Log.Info(lotNo, "上下反転チェック(フレーム配置状態)", strDeployState);


			if (frameDeployStates.Count == 0)
			{
				//反転している作業が存在しない
				return false;
			}

			List<WorkResult> sortedResults = workResults.OrderBy(w => w.StartDt).ToList();

            // ログ処理
            string strWorkResult = string.Empty;
            foreach (WorkResult wr in sortedResults)
            {
                strWorkResult += string.Format("\r\n【工程:{0} 開始Dt:{1} 設備:{2}({3}) 反転:{4}】", wr.ProcNo, wr.StartDt.ToString(), wr.MacNo, wr.PlantCd, wr.RevDeployFg);
            }

            Log.Info(lotNo, "上下反転チェック(作業実績)", strWorkResult);

			int currentProcIndex = sortedResults.FindIndex(s => s.ProcNo == currentProcNo);

			//Log.Info(lotNo, "上下反転チェック(作業実績)", string.Format("現作業工程No:{0} ｲﾝﾃﾞｸｽ:{1}", currentProcNo, currentProcIndex));

            int fromProcNo;

            if (isEndTimingRef == true)
            {
				try
				{
					fromProcNo = sortedResults[currentProcIndex].ProcNo;
				}
				catch (Exception err)
				{
					throw;
				}
            }
            else
            {
				try
				{
					fromProcNo = sortedResults[currentProcIndex - 1].ProcNo;
				}
				catch (Exception err)
				{
					throw;
					//throw new ApplicationException("作業実績を");
				}
            }

            Log.Info(lotNo, "上下反転チェック(判定)"
                , string.Format("参照先Proc:{0}(状態:{1}) / 参照元Proc(タイミング加味):{2}(状態:{3})", toProcNo, frameDeployStates[toProcNo], fromProcNo, frameDeployStates[fromProcNo]));

			// 確認先の工程の反転状態(-1or1)と自工程のマップデータ参照時(実際は自工程の一つ手前）での反転状態(-1or1)を比較
			// それぞれの反転状態を足して0になるなら、状態が食い違っているので反転の必要有の判断
			if (frameDeployStates[toProcNo] + frameDeployStates[fromProcNo] == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		//private static Dictionary<int, int> getFrameDeployState(List<WorkResult> workResults)
		//{
		//	Dictionary<int, int> frameDeployStates = new Dictionary<int, int>();

		//	if (workResults.Count == 0) 
		//	{
		//		return frameDeployStates;
		//	}
		//	List<WorkResult> sortedResults = workResults.OrderBy(w => w.StartDt).ToList();

		//	int deployState = 1;

		//	foreach (WorkResult wr in sortedResults)
		//	{
		//		if (WorkFlow.IsReverseDeploy(wr.ProcNo)) 
		//		{
		//			deployState = -deployState;// 上下状態反転
		//		}

		//		frameDeployStates.Add(wr.ProcNo, deployState);
		//	}

		//	return frameDeployStates;
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="workResults"></param>
		/// <returns></returns>
		private static Dictionary<int, int> getFrameDeployState(List<WorkResult> workResults)
		{
			List<WorkResult> sortedResults = workResults.OrderBy(w => w.StartDt).ToList();

			Dictionary<int, int> frameDeployStates = new Dictionary<int, int>();
			//Dictionary<int, int> reverseTrigerList = WorkFlow.GetReverseDeployFlags(null);

			int deployState = 1;

			foreach (WorkResult wr in sortedResults)
			{
				if (wr.RevDeployFg)
				{
					deployState = -deployState;// 上下状態反転
				}

				frameDeployStates.Add(wr.ProcNo, deployState);
			}

			return frameDeployStates;
		}

		/// <summary>
		/// 現在作業中の作業CDを取得
		/// </summary>
		/// <returns></returns>
        public static WorkResult GetCurrentWorkingData(string lotno, int? divideIdentityNo) 
		{
            List<WorkResult> resultList = GetDatas(lotno, divideIdentityNo);
			if (resultList.Count == 0) 
			{
				throw new ApplicationException(string.Format("該当ロットの作業実績が見つかりません。ロットNO：{0}", lotno));
			}

			// 作業完了日時がNULLのデータに絞る
			resultList = resultList.Where(r => r.EndDt.HasValue == false).ToList();
			if (resultList.Count == 0)
			{
				//// 作業完了日時が全てのデータで入っている場合、最後の作業実績を返す(2015.3.30 nyoshimoto resultList.Countが0の場合、下のコードは絶対エラーになるので実績を返す必要が有るなら処理を考え直す必要有)
				//return resultList.OrderByDescending(r => r.StartDt).First().WorkCd;
				throw new ApplicationException(string.Format("作業中の実績（開始済みで未完了）が存在しない為、現在作業中実績が特定できません。ロットNO：{0}", lotno));
			}
			else 
			{
				if (resultList.Count == 1)
				{
					return resultList.Single();
				}
				else 
				{
					throw new ApplicationException(string.Format("作業を完了していない実績が複数存在する為、現在作業中実績が特定できません。ロットNO：{0}", lotno));
				}
			}
		}

        /// <summary>
        /// 現在作業中の作業CDを取得
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentWorkingWorkCd(string lotno, int? divideIdentityNo)
        {
            return GetCurrentWorkingData(lotno, divideIdentityNo).WorkCd;
        }

        /// <summary>
        /// 現在作業中の工程Noを取得
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentWorkingProcNo(string lotno, int? divideIdentityNo)
        {
            return GetCurrentWorkingData(lotno, divideIdentityNo).ProcNo;
        }
	}
}