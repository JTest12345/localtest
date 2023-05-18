using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EICS.Database;

namespace EICS
{
    /// <summary>
    /// パラメータ
    /// </summary>
    public class ParameterInfo
    {
        public const string MANAGE_MAXMIN = "MAX-MIN";
        public const string MANAGE_MAX = "MAX";
        public const string MANAGE_MIN = "MIN";
        public const string MANAGE_OKNG = "OK/NG";

        /// <summary>
        /// パラメータ値を判定
        /// </summary>
        /// <param name="plmInfo">閾値マスタ</param>
        /// <param name="parameterVAL">パラメータ値</param>
        /// <returns>異常内容</returns>
        public static string CheckParameter(Plm plmInfo, string parameterVAL, LSETInfo lsetInfo, string lotNO)
        {

            string errMessage = string.Empty;

            switch (plmInfo.ManageNM)
            {
                case MANAGE_MAXMIN:
                    decimal dVAL = decimal.MinValue;
                    if (!decimal.TryParse(parameterVAL, out dVAL))
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_77, plmInfo.ParameterNM, parameterVAL
							, plmInfo.ParameterMAX + "-" + plmInfo.ParameterMIN, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    dVAL = Math.Round(dVAL, 4, MidpointRounding.AwayFromZero);
                    if (plmInfo.ParameterMIN > dVAL || plmInfo.ParameterMAX < dVAL)
                    {
                        errMessage = string.Format(Constant.MessageInfo.Message_75, plmInfo.ParameterNM, plmInfo.ManageNM, parameterVAL
							, plmInfo.ParameterMAX + "-" + plmInfo.ParameterMIN, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }

                    break;
                case MANAGE_MAX:
                    dVAL = decimal.MinValue;
                    if (!decimal.TryParse(parameterVAL, out dVAL))
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_77, plmInfo.ParameterNM, parameterVAL, plmInfo.ParameterMAX
							, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    dVAL = Math.Round(dVAL, 4, MidpointRounding.AwayFromZero);
                    if (plmInfo.ParameterMAX < dVAL)
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_75, plmInfo.ParameterNM, plmInfo.ManageNM, parameterVAL
							, plmInfo.ParameterMAX, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }

                    break;
                case MANAGE_MIN:
                    dVAL = decimal.MinValue;
                    if (!decimal.TryParse(parameterVAL, out dVAL))
                    {
                        errMessage = string.Format(Constant.MessageInfo.Message_77, plmInfo.ParameterNM, parameterVAL, plmInfo.ParameterMIN
							, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    dVAL = Math.Round(dVAL, 4, MidpointRounding.AwayFromZero);
                    if (plmInfo.ParameterMIN > dVAL)
                    {
                        errMessage = string.Format(Constant.MessageInfo.Message_75, plmInfo.ParameterNM, plmInfo.ManageNM, parameterVAL
							, plmInfo.ParameterMIN, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    break;
                case MANAGE_OKNG:
                    if (plmInfo.ParameterVAL.ToUpper() != parameterVAL.ToString().ToUpper())
                    {
                        errMessage = string.Format(Constant.MessageInfo.Message_76, plmInfo.ParameterNM, parameterVAL, plmInfo.ParameterVAL
							, lotNO, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    break;
            }

            return errMessage;
        }

        /// <summary>
        /// パラメータ値(内規)を判定
        /// </summary>
        /// <param name="plmInfo">閾値マスタ</param>
        /// <param name="parameterVAL">パラメータ値</param>
        /// <returns>異常内容</returns>
        public static string CheckInnerLimit(Plm plmInfo, string parameterVAL, LSETInfo lsetInfo, string lotNo) 
        {
            if (plmInfo.InnerLowerLimit.HasValue == false && plmInfo.InnerUpperLimit.HasValue == false)
            {
                return string.Empty;
            }

            string errMessage = string.Empty;

            switch (plmInfo.ManageNM)
            {
                case MANAGE_MAXMIN:
                    decimal dVAL = decimal.MinValue;
                    if (!decimal.TryParse(parameterVAL, out dVAL))
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_77, plmInfo.ParameterNM, parameterVAL
							, plmInfo.InnerUpperLimit.Value + "-" + plmInfo.InnerLowerLimit.Value, lotNo, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    dVAL = Math.Round(dVAL, 4, MidpointRounding.AwayFromZero);
                    if (plmInfo.InnerLowerLimit.Value > dVAL || plmInfo.InnerUpperLimit.Value < dVAL)
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_155, plmInfo.ParameterNM, plmInfo.ManageNM, parameterVAL
							, plmInfo.InnerUpperLimit.Value + "-" + plmInfo.InnerLowerLimit.Value, lotNo, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }

                    break;
                case MANAGE_MAX:
                    dVAL = decimal.MinValue;
                    if (!decimal.TryParse(parameterVAL, out dVAL))
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_77, plmInfo.ParameterNM, parameterVAL, plmInfo.InnerUpperLimit.Value
							, lotNo, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    dVAL = Math.Round(dVAL, 4, MidpointRounding.AwayFromZero);
                    if (plmInfo.InnerUpperLimit.Value < dVAL)
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_155, plmInfo.ParameterNM, plmInfo.ManageNM, parameterVAL
							, plmInfo.InnerUpperLimit.Value, lotNo, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }

                    break;
                case MANAGE_MIN:
                    dVAL = decimal.MinValue;
                    if (!decimal.TryParse(parameterVAL, out dVAL))
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_77, plmInfo.ParameterNM, parameterVAL, plmInfo.InnerLowerLimit.Value
							, lotNo, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    dVAL = Math.Round(dVAL, 4, MidpointRounding.AwayFromZero);
                    if (plmInfo.InnerLowerLimit.Value > dVAL)
                    {
						errMessage = string.Format(Constant.MessageInfo.Message_155, plmInfo.ParameterNM, plmInfo.ManageNM, parameterVAL
							, plmInfo.InnerLowerLimit.Value, lotNo, lsetInfo.InlineCD, plmInfo.QcParamNO);
                    }
                    break;
                default:
                    //何もしない
                    break;
            }

            return errMessage;
        }
    }
}
