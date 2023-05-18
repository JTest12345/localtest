/*************************************************************************************
 * �V�X�e����     : �����v��捞�V�X�e��
 *  
 * ������         : ExcelIO�@�G�N�Z�����o��
 * 
 * �T��           : �G�N�Z�����o�͊֘A�̏������s��
 * 
 * �쐬           : 2009/03/09 SAC.Uchida
 * 
 * �C������       : 2018/11/08 SLA2.Uchida �}�b�s���O��͋@�\�ǉ�
 ************************************************************************************/

using System;
using System.IO;
using System.Data;
using System.Reflection;
using Excel;

namespace FVDC
{
	/// <summary>
	/// Excel��p���钠�[�̊��N���X
	/// </summary>
	///
	public class ExcelIO : IDisposable
	{
		/// �ϐ��錾
		protected	ExcelAdapter	excel;

		public ExcelIO()
		{
		}

		~ExcelIO()
		{
			this.Dispose();
		}
		public void Dispose()
		{
			if(this.excel != null)
			{
				excel.Dispose();
			}			
		}
		
		/// <summary>
		/// �w�肳�ꂽ�G�N�Z�����|�[�g���J��
		/// </summary>
		/// <param name="prmfileName"></param>
		/// <returns>������0</returns>
		public bool OpenReport(string prmfileName)
		{
			try
			{
				if ( excel == null )
				{
					excel				= new ExcelAdapter();
				}

				object objMissing		= System.Reflection.Missing.Value;
				excel.Book				= excel.App.Workbooks.Open(
					prmfileName, objMissing, objMissing, objMissing, objMissing,
					objMissing, objMissing, objMissing, objMissing, objMissing,
					objMissing, objMissing, objMissing);

				excel.App.Visible		= true;
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// �^����ꂽDataTable�����̂܂�Excel�ɓf���o��
		/// </summary>
		/// <param name="dt">Excel�ɏo�͂���DataTable</param>
		/// <returns>������0</returns>
		public int MakePlaneReport(System.Data.DataTable dt)
		{
			if(excel == null)
			{
				excel = new ExcelAdapter();
			}

		
			excel.Book = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet = (Worksheet)excel.Book.Worksheets[1];

			object missing = System.Reflection.Missing.Value;

			if(dt == null)
			{
				throw new ApplicationException("Data object not found.", null);
			}

			for(int i = 0; i <= dt.Columns.Count - 1; i++)
			{
				excel.Cell(1, i + 1).Value2 = dt.Columns[i].ColumnName;
			}

			for(int i = 0; i <= dt.Rows.Count - 1; i++)
			{
				for(int j = 0; j <= dt.Columns.Count - 1; j++)
				{
					excel.Cell(i + 2 , j + 1).Value2 = dt.Rows[i][j].ToString();
				}
			}

			excel.App.Visible = true;
			return 0;
		}

		/// <summary>
		/// �^����ꂽDataTable�R���N�V�����̃L���v�V�����������ɐݒ肵�Ă����̂�Excel�ɓf���o��
		/// </summary>
		/// <param name="dt">Excel�ɏo�͂���DataTable�R���N�V����</param>
		/// <returns>������0</returns>
		public int MakeCaptionReport(System.Data.DataTable dt)
		{
			if(dt.Rows.Count == 0)
			{
				return 0;
			}
			if(excel == null)
			{
				excel = new ExcelAdapter();
			}
		
			excel.Book = excel.App.Workbooks.Add(System.Reflection.Missing.Value);
			excel.Sheet = (Worksheet)excel.Book.Worksheets[1];

			object missing = System.Reflection.Missing.Value;
            
            /// �̈�m��
			object[,] objItem						= new object[dt.Rows.Count + 1, dt.Columns.Count];
            
            /// ���o���ݒ�
			int		SetIx							= 0;
			for(int i = 0; i <= dt.Columns.Count - 1; i++)
			{
				if ((dt.Columns[i].ColumnName != dt.Columns[i].Caption)
					&& (dt.Columns[i].Caption.Trim() != ""))
				{
					objItem[0, SetIx]				= dt.Columns[i].Caption;
					SetIx++;
				}
			}
            
            /// �ڍ׃f�[�^�ݒ�
			long ix			= 1;
			for(int i = 0; i <= dt.Rows.Count - 1; i++)
			{
				SetIx								= 0;
				for(int j = 1; j < dt.Columns.Count; j++)
				{
					if ( ( dt.Columns [j - 1].ColumnName != dt.Columns [j - 1].Caption )
						&& ( dt.Columns [j - 1].Caption.Trim() != "" ) )
					{
						try
						{
							switch ( dt.Columns [j - 1].DataType.Name )
							{
								case "Boolean":
									if ( Convert.ToBoolean(dt.Rows [i] [j - 1]) )
									{
										objItem [ix, SetIx] = "��";
									}
									break;
								case "DateTime":
									objItem [ix, SetIx] = Convert.ToDateTime(dt.Rows [i] [j - 1]).ToString("yyyy/MM/dd HH:mm:ss");
									break;
								default:
									objItem [ix, SetIx] = dt.Rows [i] [j - 1].ToString();
									break;
							}
						}
						catch
						{
						}
						SetIx++;
					}
				}
				ix++;
			}
			Range			range;
			range									= excel.Sheet.get_Range("A1", Missing.Value);
			range									= range.get_Resize(ix, SetIx);
			range.Value2							= objItem;

			/// �����T�C�Y����
			range.Columns.EntireColumn.AutoFit();
			/// �t�B���^�[�ݒ�
			range.AutoFilter(1, Missing.Value, XlAutoFilterOperator.xlAnd, Missing.Value, Missing.Value);
			/// �E�C���h�E�g�̌Œ�
			range									= excel.Sheet.get_Range("A2", Missing.Value);
			range.Activate();
			excel.App.ActiveWindow.FreezePanes		= true;

			excel.App.Visible = true;
			return 0;
		}

        /// <summary>
        /// CSV�t�@�C���f�[�^���擾���f�[�^�Z�b�g�ɏ�������
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="KeyChar"></param>
        /// <param name="dsCSV"></param>
        /// <returns></returns>
        public bool CSV_Read(string FileName, string DataList, ref int SiftCT, ref DsFree dsCSV, NotifyStatusHandler notifyHander)�@/// 2018/11/08 SLA2.Uchida �}�b�s���O��͋@�\�ǉ�
        {

            try
            {
                string[] sptTitle           = DataList.Split(',');
                string KeyChar              = sptTitle[0];
                object objValue;

                if (excel == null)
                {
                    excel                   = new ExcelAdapter();
                }
                
                /// Excel�����ݒ�
                excel.App.DisplayAlerts     = false;

                object objMissing           = System.Reflection.Missing.Value;
                excel.Book                  = excel.App.Workbooks.Open(
                                            FileName, objMissing, objMissing, objMissing, objMissing,
                                            objMissing, objMissing, objMissing, objMissing, objMissing,
                                            objMissing, objMissing, objMissing);

                excel.Sheet                 = (Excel.Worksheet)excel.Book.Worksheets[1];

                /// �Z���̓��e���A���C���X�g�Ɉꊇ�ŃR�s�[����
                Excel.Range range           = excel.Sheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, objMissing);
                int MaxRow                  = range.Row;
                int MaxCol                  = range.Column;

                Range FindRange�@           = range.Find(KeyChar);

                range                       = excel.Sheet.get_Range("A" + FindRange.Row, objMissing);
                range                       = range.get_Resize(MaxRow - FindRange.Row + 1, MaxCol);
                objValue                    = range.Value;
                MaxRow                      = MaxRow - FindRange.Row + 1;
                object[,] objItem           = new object[MaxRow, MaxCol];
                Array.Copy((System.Array)objValue, objItem, objItem.Length);
                int CntRow                  = MaxRow / 100;
                int OutTimeCT               = 0;
                int TotalCT                 = 0;
                int SumCT                   = 0;

                /// �^�C�g���f�[�^���f�[�^�Z�b�g�ɒǉ�����

                /// �f�[�^�J��������
                dsCSV.List.Columns.Clear();

                /// �ʒu��񂪊܂܂�邩����
                bool LogingFG                                           = false;
                if (DataList.Contains("�ʒu"))
                {
                    for (int k = 0; k < MaxCol; k++)
                    {
                        if (objItem[0, k].ToString().Contains("�ʒu"))
                        {
                            LogingFG                                    = true;
                            break;
                        }
                    }
                    /// �ʒu��񂪗L��Ƃ�
                    if (LogingFG)
                    {
                        DataList                                        = sptTitle[0] + "," + sptTitle[1] + "," + sptTitle[2];
                    }
                    /// �ʒu��񂪖����Ƃ�
                    else
                    {
                        /// ���݂���^�C�g�������ɂ���
                        DataList                                        = sptTitle[0];
                        for (int k = 1; k < MaxCol; k++)
                        {
                            for (int j = 1; j < sptTitle.Length; j++)
                            {
                                try
                                {
                                    if (objItem[0, k].ToString().Contains(sptTitle[j]))
                                    {
                                        DataList                        = DataList + "," + sptTitle[j];
                                        if (sptTitle[j].Contains("�f�o����"))
                                        {
                                            OutTimeCT++;
                                        }
                                        break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    sptTitle                                            = DataList.Split(',');
                }
                else
                {
                    LogingFG                                            = true;
                }

                string CheckTitle                                       = "";
                for (int i = 0; i < sptTitle.Length; i++)
                {
                    if ((sptTitle[i].Contains("۷�ݸޱ��ڽ"))
                        || (sptTitle[i].Contains("�ʒu")))
                    {
                        CheckTitle                                      = sptTitle[i];
                        dsCSV.List.Columns.Add(sptTitle[i], Type.GetType("System.Int32"));
                        dsCSV.List.Columns[sptTitle[i]].DefaultValue    = 0;
                    }
                    else
                    {
                        dsCSV.List.Columns.Add(sptTitle[i], Type.GetType("System.String"));
                        dsCSV.List.Columns[sptTitle[i]].DefaultValue    = "";
                    }
                    dsCSV.List.Columns[sptTitle[i]].Caption             = sptTitle[i];
                }
                DsFree dsSort                                           = (DsFree)dsCSV.Clone();
                /// �f�[�^�擾
                int IX                                                  = -1;
                for (int i = 1; i < MaxRow; i++)
                {
                    if (objItem[i, 0].ToString() != "")
                    {
                        IX++;
                        dsCSV.List.Rows.Add(new object[] { "0" });

                        for (int k = 0; k < MaxCol; k++)
                        {
                            for (int j = 0; j < sptTitle.Length; j++)
                            {
                                try
                                {
                                    ///�������������@2019/02/25 SLA2.Uchida �A�h���X���O�̃f�[�^��ǂݔ�΂��@������������
                                    if ((objItem[0, k].ToString().Contains(CheckTitle))
                                        && (objItem[i, k].ToString() == "0"))
                                    {
                                        for (i = i + 1; i < MaxRow; i++)
                                        {
                                            if (objItem[i, k].ToString() != "0")
                                            {
                                                break;
                                            }
                                        }
                                        if (i == MaxRow)
                                        {
                                            dsCSV.List.Rows.RemoveAt(dsCSV.List.Rows.Count - 1);
                                            k                           = MaxCol;
                                            break;
                                        }
                                    }
                                    ///�������������@2019/02/25 SLA2.Uchida �A�h���X���O�̃f�[�^��ǂݔ�΂��@������������
                                    if (objItem[0, k].ToString().Contains(sptTitle[j]))
                                    {
                                        try
                                        {
                                            dsCSV.List[IX][sptTitle[j]] = objItem[i, k].ToString();
                                        }
                                        catch
                                        {
                                            dsCSV.List[IX][sptTitle[j]] = Convert.ToInt32(objItem[i, k]);
                                        }
                                        /// �r�o���Ԃ��Q�O���������v����
                                        if ((SumCT < 20)
                                            && (sptTitle[j].Contains("�f�o����")))
                                        {
                                            try
                                            {
                                                TotalCT                 += Convert.ToInt32(objItem[i, k]);
                                                SumCT++;
                                            }
                                            catch { }
                                        }

                                        break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    /// �X�e�[�^�X�o�[�̍X�V    
                    if ((CntRow > 0) && (i % CntRow == 0))
                    {
                        notifyHander(i / CntRow, null);
                    }
                }

                if (LogingFG)
                {
                    /// ���בւ�
                    DataView dtView             = new DataView(dsCSV.List);
                    dtView.Sort                 = sptTitle[1];
                    for (int i = 0; i < dsCSV.List.Rows.Count; i++)
                    {
                        if (dtView[i].Row.ItemArray[0].ToString() != "")
                        {
                            dsSort.List.Rows.Add(dtView[i].Row.ItemArray);
                        }
                    }
                    dsCSV                       = (DsFree)dsSort.Copy();
                }
                /// �r�o���Ԃ��L��Ƃ�
                if (OutTimeCT > 0)
                {
                    /// ���v�̕��ς����߂�
                    int AvgCT                   = TotalCT / SumCT / 2;
                    int SubCT                   = 0;

                    /// �����r�o���ԗ�̌��o���s�v�J�����̍폜
                    for (int i = dsCSV.List.Columns.Count - 1; i > 0; i--)
                    {
                        if (dsCSV.List.Columns[i].ColumnName.Contains("�f�o����"))
                        {
                            /// ���ς̂T�O���ȉ��������͐����Ŗ����Ƃ�������Ƃ��ăJ�E���g����
                            try
                            {
                                if (Convert.ToInt32(dsCSV.List[1][i].ToString()) < AvgCT)
                                {
                                    SubCT++;
                                }
                            }
                            catch
                            {
                                SubCT++;
                            }
                            /// ����폜����
                            dsCSV.List.Columns.Remove(dsCSV.List.Columns[i].ColumnName);
                        }
                    }
                    SiftCT                      = OutTimeCT - SubCT;
                }

                return true;
            }
            catch (Exception ex)
            {
                ///TODO: �G���[����
                excel.App.DisplayAlerts			= true;
                return false;
            }
            finally
            {
                /// �u�b�N�����
                excel.Book.Close(false, FileName, System.Reflection.Missing.Value);
                excel.App.DisplayAlerts			= true;
            }
        }
    }
}
