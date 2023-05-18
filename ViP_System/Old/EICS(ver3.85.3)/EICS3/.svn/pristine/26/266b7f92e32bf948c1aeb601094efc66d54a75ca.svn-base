using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	class GRDMachineInfo : PLCDDGBasedMachine
	{
		private const int GRINDER_PRM_TIMING_NO = 5;

		private const string PLC_AIM_THICKNESS_REQ_ADDR = "ZR01B38E";
		private const string PLC_AIM_THICKNESS_COMP_ADDR = "ZR01B19A";
		private const string PLC_JUDGE_OK_ADDR = "ZR01B198";
		private const string PLC_JUDGE_NG_ADDR = "ZR01B199";
        private const string PLC_JUDGE_NG_REASON = "ZR01B1FC";

        //OK/NG送信後に1を立てて装置に判定結果送信完了を通知するアドレス
        private const string PLC_JUDGE_COMP_NOTIFY_ADDR = "ZR";

		//製品情報読み取り完了通知アドレス（出来栄え側のEICS処理完了を装置へ通知する為の物）
		private const string PLC_PRODUCTINFO_READ_COMP_NOTIFY_ADDR = "ZR01B19C";

		private const string PLC_THICKNESS_RANK_ADDR = "ZR01B1CA";
       
        private List<string> workingDmAddressList
        {
            get { return new List<string> { "ZR0186C8", "ZR0186D2", "ZR0186E8", "ZR0186F2" }; }
        }

        private const int RING_DM_LEN = 10;
		private const int AIM_THICKNESS_REQ_VAL = 1;
		private const int AIM_THICKNESS_WRITE_COMP_VAL = 2;

		public GRDMachineInfo(LSETInfo lsetInfo)
		{
			InitPropAtLoop(lsetInfo);
			InitPLC(lsetInfo);
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				base.machineStatus = Constant.MachineStatus.Runtime;

				InitPropAtLoop(lsetInfo);
				InitPLC(lsetInfo);
				//問題発生時は装置停止

				//狙い厚みデータの要求が有れば、そのデータの送信処理を実施
				if (isRequestAimThickness())
				{
					aimThicknessSendProcess(lsetInfo);
				}

        		CreateFileProcess(lsetInfo, true);
				CreateFileProcess(lsetInfo, false);

				StartingProcess(lsetInfo);

				MachineStopProcess(lsetInfo, true);
				int timingNo = GetTimingNo(lsetInfo.ChipNM);

				EndingProcess(lsetInfo, timingNo);

                ResponseOKFile(true, lsetInfo);
				ResponseOKFile(false, lsetInfo);

                ResponseNGFileWithoutError(false, lsetInfo);
            }
			catch (Exception err)
			{
				throw;
			}
		}

		private bool isRequestAimThickness()
		{
			if (plc.GetDataAsString(PLC_AIM_THICKNESS_REQ_ADDR, 1, PLC.DT_DEC_32BIT) == PLC.BIT_ON)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lsetInfo"></param>
		private void aimThicknessSendProcess(LSETInfo lsetInfo)
		{
            string dm = plc.GetDataAsString(workingDmAddressList.First(), RING_DM_LEN, PLC.DT_STR).Trim('\r','\0');
            if (string.IsNullOrEmpty(dm) == true)
            {
                throw new ApplicationException("読み込んだデータマトリックスが空の為、狙い厚みランクを特定できません。");
            }

            string lotNo = string.Empty;

            bool isSubstrate = hasSubstrate(dm);
            if (isSubstrate == true)
            {
                List<string> lots = getLotsFromDm();
                if (lots.Distinct().Count() != 1)
                {
                    throw new ApplicationException("同一ロットの基板ではない為、狙い厚みランクを特定できません。");    
                }
                lotNo = ArmsApi.Model.LotCarrier.GetLotNo(dm, true).SingleOrDefault();
            }
            else
            {
                lotNo = ArmsApi.Model.LotCarrier.GetLotNoFromRingNo(dm);
            }
			ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(lotNo);

			//湯浅さんのArmApiで作られる関数にリングDMを渡すと、全資材の厚み平均値のリストが得られる
			//List<Plm> plmList = Plm.GetData(lsetInfo.InlineCD, lot.TypeCd, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.ChipNM, GRINDER_PRM_TIMING_NO, ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD));

			List<int> targetParamNoList = Plm.ParamGetTargetNoList(lsetInfo.ModelNM, lsetInfo.ChipNM, lot.TypeCd, GRINDER_PRM_TIMING_NO, ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD));

			List<Plm> plmList = new List<Plm>();
			foreach (int targetParamNo in targetParamNoList)
			{
				Plm plm = ConnectDB.GetPLMData(targetParamNo, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lot.TypeCd, lsetInfo.InlineCD, lsetInfo.ChipNM);

				if (plm.ParamGetLowerCond.HasValue && plm.ParamGetUpperCond.HasValue)
				{
					plmList.Add(plm);
				}
			}

			if (plmList.Count() == 0)
			{
				string errMsg = string.Format("資材の厚み平均値の該当する狙いﾗﾝｸが存在しないか、該当ﾗﾝｸが１つに絞れません。設備:{0}({1})/ﾀｲﾌﾟ:{2}/ChipNm:{3}/ﾀｲﾐﾝｸﾞNo:{4}",
						lsetInfo.ModelNM, lsetInfo.EquipmentNO, lot.TypeCd, lsetInfo.ChipNM, GRINDER_PRM_TIMING_NO);
				throw new ApplicationException(errMsg);
			}

			string allRankStr = string.Join(",", plmList.Select(p => string.Format("[ﾗﾝｸ:{0}({1}～{2})]", p.ParameterVAL, p.ParamGetLowerCond, p.ParamGetUpperCond)));

            //全資材の厚み平均値がどのランクに収まるかをチェック（もし一つでも違うランクのものがあれば、エラーで止める）
            //一つに絞れたら、そのランクを装置に書き込む
            List<float> allMatThicknessAveList = ArmsApi.Model.Material.GetMatThicknessList(lotNo); 
            
			int? thicknessRank = null;

			foreach (float thicknessAve in allMatThicknessAveList)
			{
				List<Plm> correspondingPlmList = plmList.FindAll(p => thicknessAve >= p.ParamGetLowerCond && thicknessAve < p.ParamGetUpperCond);

				if (correspondingPlmList == null || correspondingPlmList != null && correspondingPlmList.Count() != 1)
				{
					string errMsg = string.Format("資材の厚み平均値の該当する狙いﾗﾝｸが存在しないか、該当ﾗﾝｸが１つに絞れません。ﾛｯﾄ:{0}/資材厚み平均値:{1}/全狙いﾗﾝｸ(ｶﾝﾏ区切り):{2}",
						lotNo, thicknessAve, allRankStr);
					throw new ApplicationException(errMsg);
				}

				Plm plm = correspondingPlmList[0];

				int matThicknessRank;
				if(int.TryParse(plm.ParameterVAL, out matThicknessRank) == false)
				{
					string errMsg = string.Format("閾値ﾏｽﾀから取得した狙い厚みﾗﾝｸが数値変換出来ない文字列で設定されています。管理番号:{0}/ﾀｲﾌﾟ:{1}/設備:{2}/設定値:{3}",
						plm.QcParamNO, plm.MaterialCD, plm.EquipmentNO, plm.ParameterVAL);
					throw new ApplicationException(errMsg);
				}
				
				if (thicknessRank.HasValue && thicknessRank.Value != matThicknessRank)
				{
					string errMsg = string.Format("対象ﾛｯﾄの全資材の厚み平均値が1つの狙いﾗﾝｸに収まりませんでした。ﾛｯﾄ:{0}/資材厚み平均値(ｶﾝﾏ区切り):{1}/全狙いﾗﾝｸ(ｶﾝﾏ区切り):{2}",
						lotNo, string.Join(",", allMatThicknessAveList), allRankStr);
					throw new ApplicationException(errMsg);
				}

				thicknessRank = matThicknessRank;
			}

			plc.SetWordAsDecimalData(PLC_THICKNESS_RANK_ADDR, thicknessRank.Value);

			//ランクを書き込んだら、装置の書込み完了信号を2にしてEICS処理が完了した事を装置のPLCメモリを介してARMSに知らせる。
			plc.SetWordAsDecimalData(PLC_AIM_THICKNESS_COMP_ADDR, AIM_THICKNESS_WRITE_COMP_VAL);
		}

		public void SendNG(string memAddr)
		{
			plc.SetWordAsDecimalData(memAddr, 1);
		}

		public void SendOK(string memAddr)
		{
			plc.SetWordAsDecimalData(memAddr, 1);
		}

        public override void MachineStopProcess(LSETInfo lsetInfo, bool isStartUp)
        {
            //バックアップしている実態が関数名から認知できないので、関数名を修正する
            if (CheckForMachineStopFile(isStartUp))
            {
                plc.SetString(PLC_JUDGE_NG_REASON,"EICS Parameter Error", "UTF-16BE"); 
                SendNG(PLC_JUDGE_NG_ADDR);
            }            
        }
        
        protected void ResponseOKFile(bool isStartUp, LSETInfo lset)
		{
			List<string> chkDirList = new List<string>();
			List<string> chkTargetFileList = new List<string>();
			List<KeyValuePair<string, List<string>>> moveTargetFileList = new List<KeyValuePair<string,List<string>>>() ;

			if (isStartUp)
			{
				chkDirList.Add(StartFileDir);
			}
			else
			{
				chkDirList.Add(EndFileDir);
			}

			chkDirList.AddRange(GetSubGrpOutputDir(lset, isStartUp, string.Empty));

			//移動対象を抽出
			foreach (string chkDir in chkDirList)
			{
				//OKファイルがあるかチェック
				chkTargetFileList = Common.GetFiles(chkDir, EICS.Structure.CIFS.EXT_OK_FILE);
				if (chkTargetFileList.Count == 0) return;

                if (isStartUp)
                {
                    SendOK(PLC_JUDGE_OK_ADDR);
                }
                else
                {
                    SendOK(PLC_PRODUCTINFO_READ_COMP_NOTIFY_ADDR);
                }
				
				KeyValuePair<string, List<string>> moveInfo = new KeyValuePair<string,List<string>>(chkDir, chkTargetFileList);

				moveTargetFileList.Add(moveInfo);
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
				"設備:{0}/{1}/{2}号機【OKﾌｧｲﾙ確認の為、ﾃﾞｰﾀ取得要求OFFと出力ﾌｧｲﾙ移動を開始】 isStartUpFg:{3}", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO, isStartUp));
			
			//OKファイルを退避(サブ装置スレッドのファイルも全て移動
			foreach (KeyValuePair<string, List<string>> moveTarget in moveTargetFileList)
			{
				EICS.Structure.CIFS.BackupDoneFiles(moveTarget.Value, moveTarget.Key, string.Empty, DateTime.Now);
			}
		}

        protected override bool IsGetableData(IPlc plc, string plcMemAddr)
        {
            string retv = plc.GetDataAsString(plcMemAddr,1,PLC.DT_DEC_16BIT);
            
            if (retv == "0")
            {
                return false;
            }
            else if (retv == "1")
            {
                return true;
            }
            else
            {
                throw new ApplicationException(string.Format("PLCから予期せぬ応答を受信しました。ﾒﾓﾘｱﾄﾞﾚｽ:{0} 応答:{1} 想定する応答:{2}", plcMemAddr, retv, "0 or 1"));
            }
        }

        protected void ResponseNGFileWithoutError(bool isStartUp, LSETInfo lset)
        {
            List<string> chkDirList = new List<string>();
            List<string> chkTargetFileList = new List<string>();
            List<KeyValuePair<string, List<string>>> moveTargetFileList = new List<KeyValuePair<string, List<string>>>();

            if (isStartUp)
            {
                chkDirList.Add(StartFileDir);
            }
            else
            {
                chkDirList.Add(EndFileDir);
            }

            chkDirList.AddRange(GetSubGrpOutputDir(lset, isStartUp, string.Empty));

            //移動対象を抽出
            foreach (string chkDir in chkDirList)
            {
                //NGファイルがあるかチェック
                chkTargetFileList = Common.GetFiles(chkDir, EICS.Structure.CIFS.EXT_NG_FILE);
                if (chkTargetFileList.Count == 0) return;

                if (isStartUp)
                {
                    SendOK(PLC_JUDGE_OK_ADDR);
                }
                else
                {
                    SendOK(PLC_PRODUCTINFO_READ_COMP_NOTIFY_ADDR);
                }

                KeyValuePair<string, List<string>> moveInfo = new KeyValuePair<string, List<string>>(chkDir, chkTargetFileList);

                moveTargetFileList.Add(moveInfo);
            }

            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
                "設備:{0}/{1}/{2}号機【NGﾌｧｲﾙ確認(ｴﾗｰ無しﾓｰﾄﾞ)の為、ﾃﾞｰﾀ取得要求OFFと出力ﾌｧｲﾙ移動を開始】 isStartUpFg:{3}", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO, isStartUp));

            //OKファイルを退避(サブ装置スレッドのファイルも全て移動
            foreach (KeyValuePair<string, List<string>> moveTarget in moveTargetFileList)
            {
                EICS.Structure.CIFS.BackupDoneFiles(moveTarget.Value, moveTarget.Key, string.Empty, DateTime.Now);
            }
        }

        private bool hasSubstrate(string dm)
        {
            //読込データの要素数で基板かリングかを判断
            string[] dmElement = dm.Split(' ');
            if (dmElement.Count() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 読み込んだDMからロットリストを取得
        /// </summary>
        /// <returns></returns>
        private List<string> getLotsFromDm()
        {
            List<string> retv = new List<string>();

            List<string> dmList = new List<string>();

            foreach(string address in workingDmAddressList)
            {
                string dm = plc.GetDataAsString(address, RING_DM_LEN, PLC.DT_STR).Trim('\r', '\0');
                if (string.IsNullOrEmpty(dm) == true)
                {
                    continue;
                }
                dmList.Add(dm);

                try
                {
                    string lotNo = ArmsApi.Model.LotCarrier.GetLotNo(dm, true).SingleOrDefault();
                    if (string.IsNullOrEmpty(lotNo) == true)
                    {
                        continue;
                    }
                     
                    retv.Add(lotNo);
                }
                catch (Exception)
                {
                    // DMとロットの紐づけが存在しない場合はリストに含めない    
                }
            }

            if (retv.Any() == false)
            {
                throw new ApplicationException($"基板とロットの紐づけがされていない為、狙い厚みランクを特定できません。 読込基板DM:{string.Join(",", dmList)}");
            }

            return retv;
        }
    }
}
