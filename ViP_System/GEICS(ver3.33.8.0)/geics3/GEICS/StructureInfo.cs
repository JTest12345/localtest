using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GEICS.Database;

namespace GEICS
{
    /// <summary>
    /// 
    /// </summary>
    public class EmployeeInfo
    {
        public string EmployeeCD { get; set; }
        public string LoginServerNM { get; set; }
        public List<UserFunctionInfo> UserFunctionList { get; set; }
		public string ServerInstance { get; set; }

        public static EmployeeInfo GetLoginEmpInfo(string employeeCD, string loginServerNM)
        {
            try
            {
                EmployeeInfo employeeInfo = new EmployeeInfo();
                employeeInfo.LoginServerNM = loginServerNM;
                employeeInfo.EmployeeCD = employeeCD;
                employeeInfo.UserFunctionList = ConnectQCIL.GetUserFunction(employeeCD);

				using (IConnection conn = NascaConnection.CreateInstance(Constant.StrQCIL, false))
				{
					employeeInfo.ServerInstance = conn.Connection.DataSource;
				}

                return employeeInfo;
            }
            catch(Exception err)
            {
                throw err;
            }
        }
    }

    /// <summary>
    /// TnUserFunction(権限)
    /// </summary>
    public class UserFunctionInfo
    {
        public string EmployeeCD { get; set; }
        public string FunctionCD { get; set; }
        public string FunctionNM { get; set; }
    }

    public class FNMInfo 
    {
        public string AssetsNM { get; set; }
        public int FixedNO { get; set; }
        public string FixedNM { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }
    }
                            
    public class QCNRInfo 
    {
        public int LineCD { get; set; }
        public string EquipmentNO { get; set; }
        public DateTime MeasureDT { get; set; }
        public int InspectionNO { get; set; }
        public int QcnrNO { get; set; }
        public string ConfirmNM { get; set; }
    }

    /// <summary>
    /// TmFILEFMTTYPE(ログファイル紐付け型番情報)
    /// </summary>
    public class FileFmtTypeInfo 
    {
        public bool ChangeFG { get; set; }
        public string OldTypeCD { get; set; }

        public string TypeCD { get; set; }
        public int FileFmtNO { get; set; }
        public long FrameNO { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }
    }

    /// <summary>
    /// TmPRM(管理情報)
    /// </summary>
    public class ParamInfo 
    {
        public int QcParamNO { get; set; }
        public string ModelNM { get; set; }
        public string ChipNM { get; set; }
        public string DieKB { get; set; }
        public string ClassNM { get; set; }
        public string ParameterNM { get; set; }
        public string ManageNM { get; set; }
        public string TimingNM { get; set; }
        public string ChangeUnitVAL { get; set; }
        public string TotalKB { get; set; }
		public int EquipManageFG { get; set; }

        public string Info1NM { get; set; }
        public string Info2NM { get; set; }
        public string Info3NM { get; set; }

        public bool SelectFG { get; set; }

		public bool UnManageTrendFG { get; set; }
		public bool WithoutFileFmtFG { get; set; }
        public int ResinGroupManageFG { get; set; }
    }

	/// <summary>
	/// TmPLMの主キー値
	/// </summary>
	public struct PlmPrimaryKeyValue
	{
		public string modelNM;
		public int qcParamNo;
	}

    /// <summary>
    /// TmPLM(閾値情報)
    /// </summary>
    public class PlmInfo 
    {
        public string ModelNM { get; set; }
        public int QcParamNO { get; set; }
        public string MaterialCD { get; set; }
		public string EquipmentNO { get; set; }
        public string ResinGroupCD { get; set; }
        public decimal? ParameterMAX { get; set; }
        public decimal? ParameterMIN { get; set; }
        public string ParameterVAL { get; set; }
        public int? QcLinePNT { get; set; }
        public decimal? QcLineMAX { get; set; }
        public decimal? QcLineMIN { get; set; }
        public decimal? AimLineVAL { get; set; }
        public decimal? AimRateVAL { get; set; }

        public int RevNO { get; set; }
        public string ReasonVAL { get; set; }
        public string UpdUserCD { get; set; }
		public bool DSFG { get; set; }
        public bool DelFG { get; set; }
        public DateTime? LastUpdDT { get; set; }

        public string ChipNM { get; set; }
        public string ClassNM { get; set; }
        public string ParameterNM { get; set; }
        public string ManageNM { get; set; }

        public string TimingNM { get; set; }
        public string Info1NM { get; set; }
        public string Info2NM { get; set; }
        public string Info3NM { get; set; }

        public int InspectionNO { get; set; }
        public int QCnumNO { get; set; }

        public bool ChangeFG { get; set; }

        public string DiceCT { get; set; }

        public decimal? InnerUpperLimit { get; set; }
        public decimal? InnerLowerLimit { get; set; }

        public float? ParamGetUpperCond { get; set; }
        public float? ParamGetLowerCond { get; set; }

        public int EquipManageFG { get; set; }
		public bool UnManageTrendFG { get; set; }
		public bool WithoutFileFmtFG { get; set; }
        public int ResinGroupManageFG { get; set; }
        public bool ProgramMaterialCdFG { get; set; }

        /// <summary>
        /// Plmの内容に問題が無いかチェックし問題が有ればTrueを返す
        /// </summary>
        /// <param name="plmList"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool HasProblem(List<PlmInfo> plmList, out string errMsg)
		{
			try
			{
				errMsg = string.Empty;

				CheckEquipManageParam(plmList, out errMsg);

				string equipProblemErrStr = string.Empty;
				string limitProblemErrStr = string.Empty;

				foreach (PlmInfo plm in plmList)
				{
					equipProblemErrStr += ExistEquipOnParam(plm);

					limitProblemErrStr += CheckLimitValueProblem(plm);
				}

				if (string.IsNullOrEmpty(equipProblemErrStr) == false)
				{
					errMsg += "設備マスタに未追加の装置が設定されています。設備マスタ担当者へ確認して下さい。\r\n" + equipProblemErrStr;
				}

				if (string.IsNullOrEmpty(limitProblemErrStr) == false)
				{
					errMsg += "閾値マスタ担当者へ確認して下さい。\r\n" + limitProblemErrStr;
				}

				if (string.IsNullOrEmpty(errMsg))
				{
					//Prmは問題を抱えて無いのでfalse
					return false;
				}
				else
				{
					//問題がある為、true
					return true;
				}
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		public static void CheckEquipManageParam(List<PlmInfo> plmList, out string errMsg)
		{
			string localErrMsg = string.Empty;

			//設備CD別管理の仕様上許可されない問題が発生していないかをチェック

			#region 設備CDを管理する状況で空白レコードがあるかどうかのチェック
			List<PlmInfo> equipManagePlmList = plmList.Where(p => p.EquipManageFG != 0).ToList();
			List<string> typeList = equipManagePlmList.Select(e => e.MaterialCD).Distinct().ToList();
			List<int> paramNoList = equipManagePlmList.Select(e => e.QcParamNO).Distinct().ToList();
			
			//設備CD管理のパラメタは空白を一切許可しない。空白レコードがあったらエラー
			foreach (int qcParamNo in paramNoList)
			{
				string paramNm = equipManagePlmList.Where(e => e.QcParamNO == qcParamNo).Select(e => e.ParameterNM).Distinct().Single();
				//品種、パラメタNo毎の全装置分のパラメタリスト
				List<PlmInfo> equipManagePlmPerParam = equipManagePlmList.Where(e => e.QcParamNO == qcParamNo).ToList();

				foreach (string type in typeList)
				{
					int equipEmptyRecordCt = equipManagePlmPerParam.Where(e => e.MaterialCD == type && string.IsNullOrEmpty(e.EquipmentNO)).Count();

					if (equipEmptyRecordCt > 0)
					{
						if (string.IsNullOrEmpty(localErrMsg))
						{
							localErrMsg += "設備CD別管理が【必要】なパラメータで【設備CD指定の無い】規格が存在します。\r\n";
						}
						localErrMsg += string.Format("\tタイプ:{0} / パラメータ番号:{1} / パラメータ名:{2}\r\n", type, qcParamNo, paramNm);
					}
				}
			}
			#endregion

			#region 設備CDを管理しない状況で設備CD指定レコードがあるかどうかのチェック
			List<PlmInfo> nonEquipManagePlmList = plmList.Where(p => p.EquipManageFG == 0).ToList();
			typeList = nonEquipManagePlmList.Select(e => e.MaterialCD).Distinct().ToList();
			paramNoList = nonEquipManagePlmList.Select(e => e.QcParamNO).Distinct().ToList();

			bool addedErrMsg = false;

			//設備CD管理をしないパラメタは空白でなければならない。設備CD指定のレコードがあったらエラー
			foreach (int qcParamNo in paramNoList)
			{
				string paramNm = nonEquipManagePlmList.Where(e => e.QcParamNO == qcParamNo).Select(e => e.ParameterNM).Distinct().Single();

				List<PlmInfo> nonEquipManagePlmPerParam = nonEquipManagePlmList.Where(e => e.QcParamNO == qcParamNo).ToList();

				foreach (string type in typeList)
				{
					int equipManageRecordCt = nonEquipManagePlmPerParam.Where(e => e.MaterialCD == type &&
						string.IsNullOrEmpty(e.EquipmentNO) == false).Count();

					if (equipManageRecordCt > 0)
					{
						if (addedErrMsg == false)
						{
							localErrMsg += "設備CD別管理が【不要】なパラメータで【設備CD指定されている】規格が存在します。\r\n";
							addedErrMsg = true;
						}
						localErrMsg += string.Format("\tタイプ:{0} / パラメータ番号:{1} / パラメータ名:{2}\r\n", type, qcParamNo, paramNm);
					}
				}
			}
			#endregion

			errMsg = localErrMsg;
		}

		/// <summary>
		/// TmPLMの設備CDの値がTmEquiに存在するものか確認する関数。存在しない場合メッセージだけで処理の停止はしない
		/// </summary>
		/// <param name="plmList"></param>
		/// <param name="errMsg"></param>
		public static string ExistEquipOnParam(PlmInfo plmInfo)
		{
			string localErrMsg = string.Empty;

			if (string.IsNullOrEmpty(plmInfo.EquipmentNO.Trim()))
			{
				return string.Empty;
			}

			List<Equi> equiList = Equi.GetEquipmentInfo(string.Empty, Constant.StrQCIL, plmInfo.EquipmentNO, plmInfo.ModelNM, false);

			if (equiList.Count == 0)
			{
				localErrMsg = string.Format("\tタイプ：{0}　管理No：{1}　パラメタ名：{2}　設備CD：{3}\r\n", plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO);
			}

			return localErrMsg;
		}

		public static string CheckLimitValueProblem(PlmInfo plmInfo)
		{
			string localErrMsg = string.Empty;

			if (plmInfo.ManageNM == Constant.sMax)
			{
				if (plmInfo.ParameterMAX.HasValue == false)
				{
					localErrMsg += string.Format("\t上限値が未設定 タイプ：{0}　管理No：{1}　パラメタ名：{2}　設備CD：{3}\r\n",
						plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO);
				}
			}

			if(plmInfo.ManageNM == Constant.sMaxMin)
			{
				if (plmInfo.ParameterMAX < plmInfo.ParameterMIN)
				{
					localErrMsg += string.Format("\t上限値よりも下限値が大きい タイプ：{0}　管理No：{1}　パラメタ名：{2}　設備CD：{3} Max:{4} Min:{5}\r\n",
						plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO, plmInfo.ParameterMAX, plmInfo.ParameterMIN);
				}

				if (plmInfo.ParameterMAX.HasValue == false)
				{
					localErrMsg += string.Format("\t上限値が未設定 タイプ：{0}　管理No：{1}　パラメタ名：{2}　設備CD：{3}\r\n",
						plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO);
				}

				if (plmInfo.ParameterMIN.HasValue == false)
				{
					localErrMsg += string.Format("\t下限値が未設定 タイプ：{0}　管理No：{1}　パラメタ名：{2}　設備CD：{3}\r\n",
						plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO);
				}
			}

			if (plmInfo.ManageNM == Constant.sOKNG)
			{
				if (plmInfo.ParameterVAL == null)
				{
					localErrMsg += string.Format("\t照合値が未設定 タイプ：{0}　管理No：{1}　パラメタ名：{2}　設備CD：{3}\r\n",
						plmInfo.MaterialCD, plmInfo.QcParamNO, plmInfo.ParameterNM, plmInfo.EquipmentNO);
				}
			}

			return localErrMsg;
		}

    }

    /// <summary>
    /// TnDEFECT(不良情報)
    /// </summary>
    public class DefectInfo 
    {
        public bool ChangeFG { get; set; }
        public int LineCD { get; set; }
        public string PlantCD { get; set; }
        public string LotNO { get; set; }
        public string DefAddressNO { get; set; }
		public string UpdateDefAddressNO { get; set; }
        public string DefUnitNO { get; set; }
        public DateTime TargetDT { get; set; }
        public string WorkCD { get; set; }
        public string DefItemCD { get; set; }
        public string DefItemNM { get; set; }
        public string DefCauseCD { get; set; }
        public string DefCauseNM { get; set; }
        public string DefClassCD { get; set; }
        public string DefClassNM { get; set; }
        public string TranCD { get; set; }
        public string UpdateTranCD { get; set; }
        public string UpdUserCD { get; set; }
        public bool DelFG { get; set; }
        public DateTime LastUpdDT { get; set; }
        public bool AddressCompareFG { get; set; }
    }

    /// <summary>
    /// TnDEFECT(不良情報)検索条件
    /// </summary>
    public struct DefectSearchInfo 
    {
        public int LineCD { get; set; }
        public string PlantCD { get; set; }
        public string LotNO { get; set; }
        public DateTime? TargetFromDT { get; set; }
        public DateTime? TargetToDT { get; set; }
        public string DefItemNM { get; set; }
		public bool IsTargetDelRecord { get; set; }
    }

    /// <summary>
    /// TmGENERAL(汎用情報)
    /// </summary>
    public class GeneralInfo
    {
        public string GeneralCD { get; set; }
        public string GeneralNM { get; set; }
    }

    /// <summary>
    /// TmServ(中間PC情報)
    /// </summary>
    public class ServInfo 
    {
        public bool SelectFG { get; set; }
        public string ServerCD { get; set; }
        public string ServerNM { get; set; }
        public string DatabaseNM { get; set; }
        public bool MainServerFG { get; set; }
        public bool SubServerFG { get; set; }
    }

    /// <summary>
    /// TmFRAME(フレーム情報)
    /// </summary>
    public class FRAMEInfo
    {
        public long FrameNO { get; set; }
        public string TypeCD { get; set; }
        public int XPackageCT { get; set; }
        public int YPackageCT { get; set; }
        public int MagazineStepCT { get; set; }
        public int MagazineStepMAXCT { get; set; }

        public int FramePackageCT { get; set; }
        public int MagazinPackageCT { get; set; }
        public int MagazinPackageMAXCT { get; set; }
    }

    /// <summary>
    /// マッピング内容(WB)
    /// </summary>
    public class MappingDataInfo
    {
        public MappingDataInfo(int addressNO, string inspectionNO)
        {
            AddressNO = addressNO;
            InspectionNO = inspectionNO;
        }

        public int AddressNO { get; set; }
        public string InspectionNO { get; set; }
    }

    /// <summary>
    /// マッピング基ファイル内容(WB)
    /// </summary>
    public class MappingBaseInfo
    {
        public MappingBaseInfo(int unitNO, int addressNO, string tranNO, string errorCD)
        {
            UnitNO = unitNO;
            AddressNO = addressNO;
            TranNO = tranNO;
            ErrorCD = errorCD;
        }

        public int UnitNO { get; set; }
        public int AddressNO { get; set; }
        public string TranNO { get; set; }
        public string ErrorCD { get; set; }
    }
}
