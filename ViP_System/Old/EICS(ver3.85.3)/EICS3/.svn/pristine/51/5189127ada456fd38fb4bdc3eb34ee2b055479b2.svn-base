using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

/// <summary>
/// この関数はNAORのオーブン温度管理システムの判定ルーチンを移植。(2018/11/13時点の仕様をコピー)。
/// </summary>

namespace EICS
{
    class OvenTemperatureCheckReporter
    {

        /// <summary>
        /// レポート処理
        /// </summary>
        /// <param name="actionSetting"></param>
        public static string reportAction(LSETInfo lsetInfo, string GroupNM, List<OvenTemperatureInfo> temperatureInfoList, OvenTrgFileInfo trgFile, string sectionCD, string connectStr)
        {

            string errMsg = string.Empty;                       

            List<string> groupNMList = new List<string>() { { GroupNM } };
         
            //監視方法③
            List<CheckStandard3> checkStandardList3 = new List<CheckStandard3>();
            if (groupNMList.Count != 0)
            {
                checkStandardList3 = getCheckStandard3(groupNMList, trgFile.WorkCd, lsetInfo.EquipmentNO, sectionCD, connectStr);
                if (checkStandardList3 != null && checkStandardList3.Count != 0)
                {                     
                    errMsg = checkTemperature3(lsetInfo.EquipmentNO, trgFile, checkStandardList3, temperatureInfoList);
                }
                else
                {
                    errMsg = $"温度プロファイルマスタが存在しません。GroupNM={GroupNM}, WorkCD={trgFile.WorkCd}, PlantCD={lsetInfo.EquipmentNO}";
                }
            }

            return errMsg;

        }
        

        /// <summary>
        /// 作業開始～完了までの時間取得
        /// </summary>
        /// <param name="typeCD"></param>
        /// <param name="workCD"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public static int GetCompleteMinutes(string typeCD, string workCD, string connectStr)
        {
            #region

            using (SqlConnection con = new SqlConnection(connectStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = " SELECT CompleteMinutes "
                                   + " FROM dbo.TM_MINUTE "
                                   + " WHERE (TypeCD = '" + typeCD + "') AND "
                                   + " (WorkCD = '" + workCD + "') ";


                con.Open();
                object retv = cmd.ExecuteScalar();

                if (retv == null)
                {
                    throw new ApplicationException($"硬化時間マスタが取得できません。タイプ：{typeCD}, 作業：{workCD}");
                }
                else
                {
                    return Convert.ToInt32(retv);
                }

            }


            #endregion
        }

        /// <summary>
        /// 監視基準取得③
        /// </summary>
        /// <param name="groupNMList"></param>
        /// <param name="workCD"></param>
        /// <param name="plantCD"></param>
        /// <param name="sectionCD"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        private static List<CheckStandard3> getCheckStandard3(List<string> groupNMList, string workCD, string plantCD, string sectionCD, string connectStr)
        {
            #region
            List<CheckStandard3> retv = new List<CheckStandard3>();
            string groupNM = "";

            string whereConditonGroup = "";
            for (int i = 0; i < groupNMList.Count; i++)
            {
                if (whereConditonGroup != "")
                {
                    whereConditonGroup += ",";
                }
                whereConditonGroup += "'" + groupNMList[i] + "'";
            }

            using (SqlConnection con = new SqlConnection(connectStr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                cmd.CommandText = " SELECT GroupNM, WorkLineNo, DivisionID, "
                               + " StartTemperature_Obliqueness, MarginTemperatureUpper_Obliqueness, MarginTemperatureLower_Obliqueness, "
                               + " PassageStepTime_Obliqueness, PassageStepTemperature_Obliqueness, MarginStepTemperatureUpper_Obliqueness, "
                               + " MarginStepTemperatureLower_Obliqueness, CompltTemperature_Obliqueness, MarginCompltTemperatureUpper_Obliqueness, "
                               + " MarginCompltTemperatureLower_Obliqueness, PassageCompltTime_Obliqueness, StartTemperature_Horizon, "
                               + " MarginStartTemperatureUpper_Horizon, MarginStartTemperatureLower_Horizon, SclerosisTime_Horizon, "
                               + " MarginTemperatureUpper_Horizon, MarginTemperatureLower_Horizon "
                               + " FROM dbo.TM_CHECK_STD3 "
                               + " WHERE (GroupNM IN(" + whereConditonGroup + ")) AND "
                               + " (WorkCD = '" + workCD + "') AND "
                               + " (PlantCD = '" + plantCD + "') AND "
                               + " (SectionCD = " + sectionCD + ") AND "
                               + " (DelFG = 0) "
                               + " ORDER BY WorkLineNo ";

                con.Open();
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        if (groupNM == "")
                        {
                            groupNM = rd["GroupNM"].ToString().Trim();
                        }
                        else
                        {
                            if (groupNM != rd["GroupNM"].ToString().Trim())
                            {
                                return null;
                            }
                        }

                        CheckStandard3 checkStandard3 = new CheckStandard3();

                        checkStandard3.WorkLineNo = rd["WorkLineNo"].ToString().Trim();
                        checkStandard3.DivisionID = rd["DivisionID"].ToString().Trim();
                        if (checkStandard3.DivisionID == "1")
                        {
                            checkStandard3.StartTemperature_Obliqueness = Convert.ToInt32(rd["StartTemperature_Obliqueness"]);
                            checkStandard3.MarginTemperatureUpper_Obliqueness = Convert.ToInt32(rd["MarginTemperatureUpper_Obliqueness"]);
                            checkStandard3.MarginTemperatureLower_Obliqueness = Convert.ToInt32(rd["MarginTemperatureLower_Obliqueness"]);
                            checkStandard3.PassageStepTime_Obliqueness = Convert.ToInt32(rd["PassageStepTime_Obliqueness"]);
                            checkStandard3.PassageStepTemperature_Obliqueness = Convert.ToDecimal(rd["PassageStepTemperature_Obliqueness"]);
                            checkStandard3.MarginStepTemperatureUpper_Obliqueness = Convert.ToInt32(rd["MarginStepTemperatureUpper_Obliqueness"]);
                            checkStandard3.MarginStepTemperatureLower_Obliqueness = Convert.ToInt32(rd["MarginStepTemperatureLower_Obliqueness"]);
                            checkStandard3.CompltTemperature_Obliqueness = Convert.ToInt32(rd["CompltTemperature_Obliqueness"]);
                            checkStandard3.MarginCompltTemperatureUpper_Obliqueness = Convert.ToInt32(rd["MarginCompltTemperatureUpper_Obliqueness"]);
                            checkStandard3.MarginCompltTemperatureLower_Obliqueness = Convert.ToInt32(rd["MarginCompltTemperatureLower_Obliqueness"]);
                            checkStandard3.PassageCompltTime_Obliqueness = Convert.ToInt32(rd["PassageCompltTime_Obliqueness"]);
                        }
                        else
                        {
                            checkStandard3.StartTemperature_Horizon = Convert.ToInt32(rd["StartTemperature_Horizon"]);
                            checkStandard3.MarginStartTemperatureUpper_Horizon = Convert.ToInt32(rd["MarginStartTemperatureUpper_Horizon"]);
                            checkStandard3.MarginStartTemperatureLower_Horizon = Convert.ToInt32(rd["MarginStartTemperatureLower_Horizon"]);
                            checkStandard3.SclerosisTime_Horizon = Convert.ToInt32(rd["SclerosisTime_Horizon"]);
                            checkStandard3.MarginTemperatureUpper_Horizon = Convert.ToInt32(rd["MarginTemperatureUpper_Horizon"]);
                            checkStandard3.MarginTemperatureLower_Horizon = Convert.ToInt32(rd["MarginTemperatureLower_Horizon"]);
                        }
                        retv.Add(checkStandard3);
                    }
                }

            }

            return retv;
            #endregion
        }


        /// <summary>
        /// 温度推移判定処理③
        /// </summary>
        /// <param name="plantCD"></param>
        /// <param name="target"></param>
        /// <param name="checkStandardList"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        private static string checkTemperature3(string plantCD, OvenTrgFileInfo trgFile, List<CheckStandard3> checkStandardList, List<OvenTemperatureInfo> temperatureInfoList)
        {
            #region
            string message = "";

            //監視基準チェック
            DateTime startTime = trgFile.StartDt;
            DateTime endTime = trgFile.EndDt;
            foreach (CheckStandard3 checkStandard in checkStandardList)
            {

                bool checkOkFg = false;

                if (checkStandard.DivisionID == "1")
                {
                    //斜め
                    DateTime? stepTime = null;
                    DateTime? periodStartTime = null;
                    DateTime? periodEndTime = null;
                    decimal nowStandardLowerTemp = checkStandard.StartTemperature_Obliqueness - checkStandard.MarginTemperatureLower_Obliqueness;
                    decimal nowStandardUpperTemp = checkStandard.StartTemperature_Obliqueness + checkStandard.MarginTemperatureUpper_Obliqueness;

                    decimal lastPresentVal = 0;
                    foreach (OvenTemperatureInfo temperatureInfo in temperatureInfoList)
                    {
                        if (temperatureInfo.PresentVal >= nowStandardLowerTemp &&
                            temperatureInfo.PresentVal <= nowStandardUpperTemp)
                        {
                            if (periodStartTime == null)
                            {
                                periodStartTime = temperatureInfo.LogDT;
                            }
                        }
                        periodEndTime = temperatureInfo.LogDT;
                        lastPresentVal = temperatureInfo.PresentVal;
                        if (periodStartTime != null && periodEndTime != null)
                        {
                            if (temperatureInfo.PresentVal >= checkStandard.CompltTemperature_Obliqueness - checkStandard.MarginCompltTemperatureLower_Obliqueness &&
                                temperatureInfo.PresentVal <= checkStandard.CompltTemperature_Obliqueness + checkStandard.MarginCompltTemperatureUpper_Obliqueness)
                            {
                                //監視完了温度を満たした場合
                                //監視完了までの時間チェック
                                TimeSpan ts = Convert.ToDateTime(periodEndTime) - Convert.ToDateTime(periodStartTime);

                                if (ts.TotalMinutes > checkStandard.PassageCompltTime_Obliqueness)
                                {
                                    checkOkFg = false;
                                }
                                else
                                {
                                    startTime = periodEndTime.Value;

                                    checkOkFg = true;
                                }

                                periodStartTime = null;
                                periodEndTime = null;
                                break;
                            }
                        }

                        bool checkFG = false;
                        if (stepTime == null)
                        {
                            checkFG = true;
                        }
                        else
                        {
                            TimeSpan ts = stepTime.Value - temperatureInfo.LogDT.Value;
                            if (ts.TotalMinutes > -0.1 &&
                                ts.TotalMinutes < 0.1)
                            {
                                checkFG = true;
                            }
                        }

                        if (checkFG)
                        {
                            if (temperatureInfo.PresentVal >= nowStandardLowerTemp &&
                                temperatureInfo.PresentVal <= nowStandardUpperTemp)
                            {
                                stepTime = temperatureInfo.LogDT.Value.AddMinutes(checkStandard.PassageStepTime_Obliqueness);
                                nowStandardLowerTemp = temperatureInfo.PresentVal + checkStandard.PassageStepTemperature_Obliqueness - checkStandard.MarginStepTemperatureLower_Obliqueness;
                                nowStandardUpperTemp = temperatureInfo.PresentVal + checkStandard.PassageStepTemperature_Obliqueness + checkStandard.MarginStepTemperatureUpper_Obliqueness;
                            }
                            else
                            {
                                if (periodStartTime != null && periodEndTime != null)
                                {
                                    checkOkFg = false;
                                    stepTime = null;
                                    periodStartTime = null;
                                    periodEndTime = null;
                                    nowStandardLowerTemp = checkStandard.StartTemperature_Obliqueness - checkStandard.MarginTemperatureLower_Obliqueness;
                                    nowStandardUpperTemp = checkStandard.StartTemperature_Obliqueness + checkStandard.MarginTemperatureUpper_Obliqueness;
                                    //break;
                                }
                            }
                        }
                    }

                    //監視完了までの時間チェック
                    if (periodStartTime != null && periodEndTime != null)
                    {
                        if (lastPresentVal >= checkStandard.CompltTemperature_Obliqueness - checkStandard.MarginCompltTemperatureLower_Obliqueness &&
                            lastPresentVal <= checkStandard.CompltTemperature_Obliqueness + checkStandard.MarginCompltTemperatureUpper_Obliqueness)
                        {
                            //監視完了温度を満たした場合
                            TimeSpan ts = periodEndTime.Value - periodStartTime.Value;

                            if (ts.TotalMinutes > checkStandard.PassageCompltTime_Obliqueness)
                            {
                                checkOkFg = false;
                            }
                            else
                            {
                                startTime = periodEndTime.Value;

                                checkOkFg = true;
                            }
                        }
                        else
                        {
                            checkOkFg = false;
                        }
                    }
                }
                else if (checkStandard.DivisionID == "2")
                {
                    //水平
                    DateTime? periodStartTime = null;
                    DateTime? periodEndTime = null;

                    foreach (OvenTemperatureInfo temperatureInfo in temperatureInfoList)
                    {
                        bool temperatureOKFG = false;
                        if (periodStartTime == null)
                        {
                            if (temperatureInfo.PresentVal >= checkStandard.StartTemperature_Horizon - checkStandard.MarginStartTemperatureLower_Horizon &&
                                temperatureInfo.PresentVal <= checkStandard.StartTemperature_Horizon + checkStandard.MarginStartTemperatureUpper_Horizon)
                            {
                                temperatureOKFG = true;
                            }
                        }
                        else
                        {
                            if (temperatureInfo.PresentVal >= checkStandard.StartTemperature_Horizon - checkStandard.MarginTemperatureLower_Horizon &&
                                temperatureInfo.PresentVal <= checkStandard.StartTemperature_Horizon + checkStandard.MarginTemperatureUpper_Horizon)
                            {
                                temperatureOKFG = true;
                            }
                        }


                        if (temperatureOKFG)
                        {
                            if (periodStartTime == null)
                            {
                                periodStartTime = temperatureInfo.LogDT;
                            }
                            periodEndTime = temperatureInfo.LogDT;

                            //硬化時間を超えたら次の波形チェックへ
                            TimeSpan ts = Convert.ToDateTime(periodEndTime) - Convert.ToDateTime(periodStartTime);
                            if (ts.TotalMinutes >= checkStandard.SclerosisTime_Horizon)
                            {
                                checkOkFg = true;
                                startTime = periodEndTime.Value;

                                periodStartTime = null;
                                periodEndTime = null;
                                break;
                            }
                        }
                        else
                        {
                            if (periodStartTime != null && periodEndTime != null)
                            {
                                TimeSpan ts = periodEndTime.Value - periodStartTime.Value;
                                if (ts.TotalMinutes >= checkStandard.SclerosisTime_Horizon)
                                {
                                    checkOkFg = true;
                                    startTime = periodEndTime.Value;
                                }
                                else
                                {
                                    checkOkFg = false;
                                }

                                periodStartTime = null;
                                periodEndTime = null;
                                break;
                            }
                        }
                    }

                    if (periodStartTime != null && periodEndTime != null)
                    {
                        TimeSpan ts = periodEndTime.Value - periodStartTime.Value;

                        if (ts.TotalMinutes >= checkStandard.SclerosisTime_Horizon)
                        {
                            checkOkFg = true;
                            startTime = periodEndTime.Value;
                        }
                        else
                        {
                            checkOkFg = false;
                        }
                    }
                }

                if (checkOkFg == false)
                {
                    if (message != "")
                    {
                        message += "<br>";
                    }

                    if (checkStandard.DivisionID == "1")
                    {
                        //斜め
                        message += "STEP " + checkStandard.PassageStepTime_Obliqueness.ToString() + "min 温度"
                                + checkStandard.PassageStepTemperature_Obliqueness.ToString() + "℃ の波形が異常です。";
                    }
                    else if (checkStandard.DivisionID == "2")
                    {
                        //水平
                        message += checkStandard.StartTemperature_Horizon.ToString() + "℃×"
                                + checkStandard.SclerosisTime_Horizon.ToString() + "min　がありません。";
                    }

                    break;
                }
            }

            return message;
            #endregion
        }
    }
}
