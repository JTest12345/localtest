// measure.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "measure.h"

#include "ElectBalance_JNverDlg.h"

#define TIMER_LOOP 9876543


// Cmeasure �_�C�A���O

IMPLEMENT_DYNAMIC(Cmeasure, CDialog)

Cmeasure::Cmeasure(CWnd* pParent /*=NULL*/)
	: CDialog(Cmeasure::IDD, pParent)
	, Edit_Measure_Kishu(_T(""))
	, Edit_V_msr_Keiryo(_T(""))
	, Edit_V_msr_Sagyousha(_T(""))
	, Edit_Measure_WRank(_T(""))
	, Edit_Measure_LotNo(_T(""))
	, Edit_Measure_Setubi(_T(""))
	, Edit_V_Target(_T(""))
{
//LOGFONT
	logfont.lfHeight = 30;
	logfont.lfWidth = 15;
	logfont.lfEscapement = 0;
	logfont.lfOrientation = 0;
	logfont.lfWeight = FW_THIN;
	logfont.lfItalic = 0;
	logfont.lfUnderline = 0;
	logfont.lfStrikeOut = 0;
	logfont.lfCharSet = SHIFTJIS_CHARSET;
	logfont.lfOutPrecision = 1;
	logfont.lfClipPrecision = 2;
	logfont.lfQuality = 2;
	logfont.lfPitchAndFamily = 2;
	CString fontstr; fontstr.Format(_T("�l�r �S�V�b�N"));
	memcpy(logfont.lfFaceName, fontstr.GetBuffer(), fontstr.GetLength()*2);//�l�r �o�S�V�b�N
	fontstr.ReleaseBuffer();
	new_font.CreateFontIndirect(&logfont);

	int i,j;
	for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
	//���X�g�R���g���[��2�����z���������
	for(i=0; i<15; i++)
		for(j=0; j<15; j++)
			ListC[i][j] = _T("");


}

Cmeasure::~Cmeasure()
{
}

void Cmeasure::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST1, m_listc);
	DDX_Text(pDX, IDC_EDIT_Measure_Kishu, Edit_Measure_Kishu);
	DDX_Control(pDX, IDC_EDIT_Measure_Kishu, Edit_C_msr_Kishu);
	DDX_Text(pDX, IDC_EDIT1, Edit_V_msr_Keiryo);
	DDX_Control(pDX, IDC_EDIT1, Edit_C_msr_Keiryo);
	DDX_Control(pDX, IDC_EDIT2, Edit_C_msr_Sagyousha);
	DDX_Text(pDX, IDC_EDIT2, Edit_V_msr_Sagyousha);
	DDX_Control(pDX, IDC_KEnd, Bn_msr_Kend);
	DDX_Control(pDX, IDC_KStart, Bn_msr_KStart);
	DDX_Text(pDX, IDC_EDIT_WRank, Edit_Measure_WRank);
	DDX_Control(pDX, IDC_EDIT_WRank, Edit_C_msr_WRank);
	DDX_Control(pDX, IDC_EDIT3, Edit_C_msr_LotNo);
	DDX_Text(pDX, IDC_EDIT3, Edit_Measure_LotNo);
	DDX_Control(pDX, IDC_EDIT4, Edit_C_msr_Setubi);
	DDX_Text(pDX, IDC_EDIT4, Edit_Measure_Setubi);
	DDX_Control(pDX, IDC_EDIT_Target, Edit_C_Target);
	DDX_Text(pDX, IDC_EDIT_Target, Edit_V_Target);
	DDX_Control(pDX, IDC_PICT, m_pict);
}


BEGIN_MESSAGE_MAP(Cmeasure, CDialog)
	ON_BN_CLICKED(IDC_KStart, &Cmeasure::OnBnClickedKstart)
	ON_BN_CLICKED(IDC_KEnd, &Cmeasure::OnBnClickedKend)
	ON_BN_CLICKED(IDC_End, &Cmeasure::OnBnClickedEnd)
	ON_BN_CLICKED(IDOK, &Cmeasure::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &Cmeasure::OnBnClickedCancel)
	ON_WM_TIMER()
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_REJ, &Cmeasure::OnBnClickedRej)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LIST1, &Cmeasure::OnLvnItemchangedList1)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()


// Cmeasure ���b�Z�[�W �n���h��

BOOL Cmeasure::OnInitDialog()
{
	CDialog::OnInitDialog();

	CStdioFile fp;

CString buf;
CString tmp[40];//20160715

int i,j;
int first=0,cnt=0,vcnt=0,bothsame=0;
double temp;
int test=0;
CString Stest;
CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	// TODO:  �����ɏ�������ǉ����Ă�������
	row_cnt = 1;
	col_cnt = 1;
	FinishCnt = 0;//�e�܂̏I�����J�E���g
	//���X�g�r���[�̃J������ݒ�(���|�[�g)
	LVCOLUMN lvcol;
	lvcol.mask = LVCF_TEXT | LVCF_WIDTH;//LVCOLUMN�̃e�L�X�g�A�����L��
	//1�̃J����
	/*lvcol.pszText = _T("No");
	lvcol.cx = 40;
	m_listc.InsertColumn(0,&lvcol);*/
	//2�̃J����
	lvcol.pszText = _T("�g�p����");
	lvcol.cx = 120;
	m_listc.InsertColumn(0,&lvcol);
	//3�̃J����
	lvcol.pszText = _T("�����^��");
	lvcol.cx = 120;
	m_listc.InsertColumn(1,&lvcol);
	//4�̃J����
	lvcol.pszText = _T("�ݒ�l");
	lvcol.cx = 60;
	m_listc.InsertColumn(2,&lvcol);
	//5�̃J����
	lvcol.pszText = _T("����l");
	lvcol.cx = 60;
	m_listc.InsertColumn(3,&lvcol);

	//m_listc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_CHECKBOXES);

	CString str = pWnd->Jyushidatacsv;
	
	Keiryo = 0;//�K���Ȑ��l�ŏ�����
	RValue = 10000;//�K���Ȑ��l�ŏ�����
	gosa = _tcstod(pWnd->gosa,NULL);//�덷�͈̔�

//�t�@�C���ǂݍ���
Num = _tcstod(msr_volume,NULL);//double�֌^�ϊ�
//20180409�䗦�v�Z�s�v
Num = Num/15;//�䗦���v�Z

for(i=0; i<15; i++) Volume[i] = -1;//������20160715

CheckBoxNum = 0;//�`�F�b�N����Ă���s���i�[����ϐ���������

//Edit����ϐ���
UpdateData(TRUE);
	
UpdateData(FALSE);

//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Edit_Measure_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(Edit_Measure_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(Edit_Measure_Setubi) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}

	j=3;//�ܖ��������
	while(pWnd->Jyushi_List[i][j] != _T("")){
		if(first == 0){
			m_listc.InsertItem(row_cnt,pWnd->Jyushi_List[i][j]);
			first++;
			ListC[row_cnt-1][0]=pWnd->Jyushi_List[i][j];//�g�p���������i�[
			}
		else{
			ListC[row_cnt-1][col_cnt]=pWnd->Jyushi_List[i][j];//1�s2��ڈȍ~���i�[
			if(j==5 || j == 8 || j == 11 || j == 14 || j == 17 || j == 20 || j == 23 || j == 26 || j == 29 || j == 32 || j == 35){//20160715�ǋL
				temp = _tcstod(ListC[row_cnt-1][col_cnt],NULL);//CString����Double��
					temp = temp * Num;//�䗦��������
					//20180313�m�F
					ListC[row_cnt-1][col_cnt].Format(_T("%.4f"), temp);//Double����CString��
			}
			m_listc.SetItemText(row_cnt-1, col_cnt, ListC[row_cnt-1][col_cnt]);//2��ڂ֒ǉ�����
			
				col_cnt++;
				if(col_cnt == 3){
					col_cnt = 1;
					row_cnt++;
					first = 0;
				}
		}
		j++;		
	}

	Edit_C_msr_Kishu.SetFont(&new_font,TRUE);
	Edit_C_msr_LotNo.SetFont(&new_font,TRUE);
	Edit_C_msr_WRank.SetFont(&new_font,TRUE);
	Edit_C_msr_Setubi.SetFont(&new_font,TRUE);
	Edit_C_msr_Keiryo.SetFont(&new_font,TRUE);
	Edit_C_msr_Sagyousha.SetFont(&new_font,TRUE);
	Edit_C_Target.SetFont(&new_font,TRUE);

//���X�g�R���g���[���Ƀ`�F�b�N�{�b�N�X��ǉ�
DWORD dwStyle = m_listc.GetExtendedStyle();
dwStyle |= LVS_EX_CHECKBOXES | LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES;
m_listc.SetExtendedStyle(dwStyle);

//�v���I���{�^���𖳌���
Bn_msr_Kend.EnableWindow(FALSE);


	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void Cmeasure::OnBnClickedKstart()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int i,cnt=0;

	///////////////////////////////////////////////////
	//�@�킲�Ƃ̌��ʃf�[�^�̏o��
	//HANDLE hFile;

	//CTime cTime;
	//CString date; 
	//cTime = CTime::GetCurrentTime();           // ���ݎ���
 //   date = cTime.Format("%Y%m%d%H%M%S");   // "YYYY/mm/dd HH:MM:SS"�`���̎�����������擾
	//CString str = _T("C:\\EB\\data\\") + pWnd->Jshg.Cb_J_Kishu + _T(".csv");
	//CString strpcs = _T("C:\\EB\\pcs\\") + pWnd->Jshg.Cb_J_Kishu + _T("_") + date + _T(".csv");

	////�t�@�C���̐���
	//hFile = CreateFile(str, 
	//					GENERIC_READ | GENERIC_WRITE,
	//					FILE_SHARE_READ,
	//					NULL,
	//					OPEN_ALWAYS,
	//					FILE_ATTRIBUTE_NORMAL,
	//					NULL);

	//if(hFile == INVALID_HANDLE_VALUE)
	//	AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	////�n���h�������
	//CloseHandle(hFile);

	//hFile = CreateFile(strpcs, 
	//					GENERIC_READ | GENERIC_WRITE,
	//					FILE_SHARE_READ,
	//					NULL,
	//					OPEN_ALWAYS,
	//					FILE_ATTRIBUTE_NORMAL,
	//					NULL);

	//if(hFile == INVALID_HANDLE_VALUE)
	//	AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	////�n���h�������
	//CloseHandle(hFile);

	////�������݃e�X�g
	////�t�@�C���̏�������
	//CStdioFile ofp;
	//CStdioFile ofppcs;
	//int j;

	//date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"�`���̎�����������擾


	//			if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
	//
	//			ofp.SeekToEnd();

	//			
	//			ofp.WriteString(date);//���t
	//			ofp.WriteString(_T(","));
	//			ofp.WriteString(pWnd->Jshg.Cb_J_Kishu);//�@�햼
	//			ofp.WriteString(_T(","));
	//			ofp.WriteString(pWnd->Jshg.Cb_V_J_WRank);//�g�������N
	//			ofp.WriteString(_T(","));
	//			ofp.WriteString(pWnd->Jshg.Cb_J_Setubi);//�ݔ��̎��
	//			ofp.WriteString(_T(","));
	//			ofp.WriteString(pWnd->Jshg.LotNo);//���b�g�i���o�[
	//			ofp.WriteString(_T(","));
	//			ofp.WriteString(pWnd->Jshg.Cb_V_J_Sagyousha);//��ƎҖ�
	//			ofp.WriteString(_T(","));

	//			for(i=0;i<ItemNum;i++){
	//				for(j=0;j<4;j++){
	//					ofp.WriteString(ListC[i][j]);
	//					ofp.WriteString(_T(","));
	//				}
	//			}
	//			ofp.WriteString(_T("\n"));
	//		}
	//		ofp.Close();


	//		//pcs�p�t�@�C���ɂ��������݂��s��
	//		if(ofppcs.Open(strpcs, CFile::modeWrite | CFile::shareDenyNone)){
	//
	//			ofppcs.SeekToEnd();

	//			
	//			ofppcs.WriteString(date);//���t
	//			ofppcs.WriteString(_T(","));
	//			ofppcs.WriteString(pWnd->Jshg.Cb_J_Kishu);//�@�햼
	//			ofppcs.WriteString(_T(","));
	//			ofppcs.WriteString(pWnd->Jshg.Cb_V_J_WRank);//�g�������N
	//			ofppcs.WriteString(_T(","));
	//			ofppcs.WriteString(pWnd->Jshg.Cb_J_Setubi);//�ݔ��̎��
	//			ofppcs.WriteString(_T(","));
	//			ofppcs.WriteString(pWnd->Jshg.LotNo);//���b�g�i���o�[
	//			ofppcs.WriteString(_T(","));
	//			ofppcs.WriteString(pWnd->Jshg.Cb_V_J_Sagyousha);//��ƎҖ�
	//			ofppcs.WriteString(_T(","));

	//			for(i=0;i<ItemNum;i++){
	//				for(j=0;j<4;j++){
	//					ofppcs.WriteString(ListC[i][j]);
	//					ofppcs.WriteString(_T(","));
	//				}
	//			}
	//			ofppcs.WriteString(_T("\n"));
	//		}
	//		ofppcs.Close();




	//////////////////////////////////////////

	ItemNum = ListView_GetItemCount(m_listc);//�s�����擾

	for(i=0;i<ItemNum;i++){
		if(ListView_GetCheckState(m_listc,i)!=0){
			CheckBoxNum = i;
			cnt++;
			if(cnt >=2){
			AfxMessageBox(_T("�����`�F�b�N����Ă��܂�."));
			break;
			}
		}
	}
	if(cnt == 0)
		AfxMessageBox(_T("�`�F�b�N����Ă��܂���."));
	if(cnt == 1){

		//�ڕW�l������
		Edit_V_Target = ListC[pWnd->Jshg.msr.CheckBoxNum][2];	
		//�ϐ�����Edit��
		UpdateData(FALSE);
		RValue = _tcstod(ListC[CheckBoxNum][2],NULL);

		if(pWnd->dlflag_JyushiCode == FALSE){
			//�����̌^�Ԃ������Ă��邩�m�F����
			pWnd->dlflag_JyushiCode = TRUE;
			pWnd->JC.Create(IDD_JyushiCode,this);
			pWnd->JC.ShowWindow(SW_SHOWNA);//�\��
		
			//if(pWnd->JC.flg_same == TRUE){
			//	Bn_msr_Kend.EnableWindow(TRUE);//�v���I���{�^����L����
				//�^�C�}�[�̌Ăяo��
			//	SetTimer(TIMER_LOOP,200,NULL);
			//}
		}
	}
	
	// �\���X�V
    //Invalidate();

}

void Cmeasure::OnBnClickedKend()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	int i,j,k,cnt=0;
	CString Stest;
	//double Keiryo;
	
	char sbuf[128];//��M�p
	memset(sbuf, 0, 128);

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	//CString str = pWnd->Resultcsv;

	//�@�킲�Ƃ̌��ʃf�[�^�̏o��
	HANDLE hFile;

	CTime cTime;
	CString date;



	gosa = _tcstod(pWnd->gosa,NULL);//�덷�͈̔�
	//double RValue;
	int fns_flag = 0;

	//�t�@�C���ǂݍ���
	CStdioFile fp;
	//�t�@�C���̏�������
	CStdioFile ofp;
	CStdioFile ofppcs;

	CString buf;
	CString tmp[16];


	cTime = CTime::GetCurrentTime();           // ���ݎ���
    //date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"�`���̎�����������擾

	for(i=0;i<ItemNum;i++){
		if(ListView_GetCheckState(m_listc,i)!=0){
			CheckBoxNum = i;
			cnt++;
			if(cnt >=2){
			AfxMessageBox(_T("�����`�F�b�N����Ă��܂�."));
			break;
			}
		}
	}
	if(cnt == 0)
		AfxMessageBox(_T("�`�F�b�N����Ă��܂���."));

	if(cnt == 1){
	//Edit����ϐ���
	UpdateData(TRUE);


	//�ϐ�����Edit��
	UpdateData(FALSE);


	RValue = _tcstod(ListC[CheckBoxNum][2],NULL);

	Keiryo = _tcstod(Edit_V_msr_Keiryo,NULL);//CString����Double��
	//20180409
	//if(Keiryo - RValue <= (RValue*gosa/100) && Keiryo - RValue >= (gosa*(-1)*RValue/100)){//�덷�܂߁AOK�ł����
	if(Keiryo - RValue <= gosa && Keiryo - RValue >= (gosa*(-1))){//�덷�܂߁AOK�ł����

		m_listc.SetItemText(CheckBoxNum, 3, Edit_V_msr_Keiryo);//�v�ʒl�����X�g�R���g���[���֒ǉ�����

		if(CBVolumeData[CheckBoxNum] == _T("")){
		FinishCnt++;//�I�����ڂ��J�E���g����
		}

		CBVolumeData[CheckBoxNum]=Edit_V_msr_Keiryo;
		ListC[CheckBoxNum][3] = Edit_V_msr_Keiryo;//�v�ʒl���i�[
		

		//�V���̃��Z�b�g���s��
		//RS232C�̏���
		pWnd->Jshg.msr.er=pWnd->rs232c.Open(pWnd->Jshg.msr.n,4096,4096);
		pWnd->Jshg.msr.er=pWnd->rs232c.Config(pWnd->Jshg.msr.n,RS232_2400|RS232_1|RS232_EVEN|RS232_7|RS232_X_N|RS232_CTS_Y|RS232_DSR_Y|RS232_DTR_Y|RS232_RTS_Y);
		pWnd->Jshg.msr.er=pWnd->rs232c.BaudRate(pWnd->Jshg.msr.n, 2400);

		pWnd->Jshg.msr.sbuf[0] = 0x5a;//Z(0Reset)
		pWnd->Jshg.msr.sbuf[1] = 0x0d;//CR
		pWnd->Jshg.msr.sbuf[2] = 0x0a;//LF
		pWnd->Jshg.msr.er = pWnd->rs232c.Send(pWnd->Jshg.msr.n,pWnd->Jshg.msr.sbuf,3);


		pWnd->rs232c.Close(n);


		if(FinishCnt < ItemNum)//
			KillTimer(TIMER_LOOP);//�^�C�}�[���X�g�b�v

		else if(FinishCnt >= ItemNum){//�S�ďI��������t�@�C���������ݑ�����s��

			//�t�@�C����������
				cTime = CTime::GetCurrentTime();// ���ݎ���
    date = cTime.Format("%Y%m%d%H%M%S");   // �t�@�C�����̐����p�̎�����������w�肷��
	CString str = _T("C:\\EB\\data\\") + pWnd->Jshg.Cb_J_Kishu + _T(".csv");
	CString strpcs = _T("C:\\EB\\pcs\\") + pWnd->Jshg.Cb_J_Kishu + _T("_") + date + _T(".csv");

	//�t�@�C���̐���
	hFile = CreateFile(str, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);

	hFile = CreateFile(strpcs, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);



			//���̑���ɔ����A�K�v�ȕϐ���������
			for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
			FinishCnt = 0;
			fns_flag = 1;
			CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

		    date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"�`���̎�����������擾

			if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
	
				ofp.SeekToEnd();

				
				ofp.WriteString(date);//���t
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_J_Kishu);//�@�햼
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_V_J_WRank);//�g�������N
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_J_Setubi);//�ݔ��̎��
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.LotNo);//���b�g�i���o�[
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_V_J_Sagyousha);//��ƎҖ�
				ofp.WriteString(_T(","));

				for(i=0;i<ItemNum;i++){
					for(j=0;j<4;j++){
						ofp.WriteString(ListC[i][j]);
						ofp.WriteString(_T(","));

						//20180427�������b�gNo�����ʃf�[�^�Ɏc��
						if(j==1){
							for(k=0;k<10;k++){
								if(ListC[i][j] == pWnd->JC.JyushiKata[k]){
									ofp.WriteString(pWnd->JC.JyushiLot[k]);
									ofp.WriteString(_T(","));
									break;
								}
							}

						}
					}
				}
				ofp.WriteString(_T("\n"));
			}
			ofp.Close();


			//pcs�p�t�@�C���ɂ��������݂��s��
			if(ofppcs.Open(strpcs, CFile::modeWrite | CFile::shareDenyNone)){
	
				ofppcs.SeekToEnd();

				
				ofppcs.WriteString(date);//���t
				ofppcs.WriteString(_T(","));
				ofppcs.WriteString(pWnd->Jshg.Cb_J_Kishu);//�@�햼
				ofppcs.WriteString(_T(","));
				ofppcs.WriteString(pWnd->Jshg.Cb_V_J_WRank);//�g�������N
				ofppcs.WriteString(_T(","));
				ofppcs.WriteString(pWnd->Jshg.Cb_J_Setubi);//�ݔ��̎��
				ofppcs.WriteString(_T(","));
				ofppcs.WriteString(pWnd->Jshg.LotNo);//���b�g�i���o�[
				ofppcs.WriteString(_T(","));
				ofppcs.WriteString(pWnd->Jshg.Cb_V_J_Sagyousha);//��ƎҖ�
				ofppcs.WriteString(_T(","));

				for(i=0;i<ItemNum;i++){
					for(j=0;j<4;j++){
						ofppcs.WriteString(ListC[i][j]);
						ofppcs.WriteString(_T(","));

						//20180420�������b�gNo�����ʃf�[�^�Ɏc��
						if(j==1){
							for(k=0;k<10;k++){
								if(ListC[i][j] == pWnd->JC.JyushiKata[k]){
									ofppcs.WriteString(pWnd->JC.JyushiLot[k]);
									ofppcs.WriteString(_T(","));
									break;
								}
							}

						}
					}
				}
				ofppcs.WriteString(_T("\n"));
			}
			ofppcs.Close();



			m_listc.EnableWindow(TRUE);//�`�F�b�N�{�b�N�X��L����
			m_listc.SetCheck(CheckBoxNum,FALSE);//�`�F�b�N�{�b�N�X�̃`�F�b�N�ӏ����O��
			Bn_msr_KStart.EnableWindow(TRUE);//�v���J�n�{�^����L����
			KillTimer(TIMER_LOOP);//�^�C�}�[���X�g�b�v
			
			AfxMessageBox(_T("�f�[�^�̏������݂��������܂���."));

			//Jyushihaigou�_�C�A���O�̊eEdit�̒l��������
			pWnd->Jshg.LotNo = _T("");
			pWnd->Jshg.Cb_J_Kishu = _T("");
			pWnd->Jshg.Cb_V_J_WRank = _T("");
			pWnd->Jshg.Cb_J_Setubi = _T("");
			pWnd->Jshg.e_MainVolume = _T("");
			pWnd->Jshg.Cb_V_J_Sagyousha = _T("");
		
			//measure�_�C�A���O�̃p�����[�^������
			Edit_V_msr_Keiryo = _T("");

			//�ϐ�����Edit��
			UpdateData(FALSE);
			//Edit����ϐ���
			UpdateData(TRUE);

			
		}

		//Stest.Format(_T("%g"), (Keiryo - Volume[CheckBoxNum]));//Double����CString��
		//AfxMessageBox(Edit_V_msr_Keiryo);
		//AfxMessageBox(Stest);

		//measure�_�C�A���O�̃p�����[�^������
		Edit_V_msr_Keiryo = _T("");

		//�ϐ�����Edit��
		UpdateData(FALSE);

		m_listc.EnableWindow(TRUE);//�`�F�b�N�{�b�N�X��L����
		m_listc.SetCheck(CheckBoxNum,FALSE);
		Bn_msr_KStart.EnableWindow(TRUE);//�v���J�n�{�^����L����
		Bn_msr_Kend.EnableWindow(FALSE);//�v���I���{�^����L����

		if(fns_flag == 1){
		//measure�E�B���h�E��j������
			pWnd->Jshg.OnClose();
			
		}

	}

	else{
		AfxMessageBox(_T("�v���ʂ�����������܂���."));
	}

	//for(i=0;i<10;i++){
	//Stest.Format(_T("%g"),Volume[i]);//�f�o�b�O�p
	//AfxMessageBox(Stest);
	//}
	}
}

void Cmeasure::OnBnClickedEnd()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	if(MessageBox(_T("�I�����܂�.��낵���ł����H"),NULL,MB_OKCANCEL) == IDOK){
		OnClose();
	}
}

void Cmeasure::OnBnClickedOk()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	OnOK();
}

void Cmeasure::OnBnClickedCancel()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	OnCancel();
}

void Cmeasure::OnTimer(UINT_PTR nIDEvent)
{
	CString trace;

	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	//int bufsize = 128;
	char rbuf[128];//��M�p
	memset(rbuf, 0, 128);
	memset(sbuf, 0, 128);
	CString str,CStVolume;

	int i;
	int temp=0,keta=0,secondp=0,allcheck=0;//20160715 Num�폜
	int startpoint=-1,dotpoint=-1,dotnum=0,cnt=0;
	//int getmoji[12];//20160715���g�p�ɂ��R�����g�A�E�g
	NowVolume=0;
	er = 0;

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	//�ϐ�����Edit��
	//UpdateData(FALSE);

	n=_ttoi(pWnd->COMPORT);//COM�|�[�g�i���o�[
	//pWnd->COMPORT.ReleaseBuffer();


//	n = _tcstoi(pWnd->COMPORT,NULL);

	switch(nIDEvent){
		case TIMER_LOOP:
			//RS232C�̏���
			er=pWnd->rs232c.Open(n,4096,4096);

			er=pWnd->rs232c.Config(n,RS232_2400|RS232_1|RS232_EVEN|RS232_7|RS232_X_N|RS232_CTS_Y|RS232_DSR_Y|RS232_DTR_Y|RS232_RTS_Y);
			er=pWnd->rs232c.BaudRate(n, 2400);


			//sbuf[0] = 0x5a;//Z(0Reset)
			//sbuf[0] = 0x43;//C(Stop)
			//sbuf[0] = 0x51;//Q(Measure)
			//sbuf[1] = 0x0d;//CR
			//sbuf[2] = 0x0a;//LF
			//er = pWnd->rs232c.Send(n,sbuf,3);


			//�y�f�[�^��M�z���[�U�}�[�J->�R���g���[��
			//�K��18byte�͎�M����
			while(1){
				er += pWnd->rs232c.Recv(n, &rbuf[er], 128);
				if(er >= 18)
					break;
			}

			//�V�T�����@@20151116
			for(i=0;i<er;i++){
				if((int)rbuf[i] == 83 && (int)rbuf[i+1] == 84 && (int)rbuf[i+2] == 44 && (int)rbuf[i+3] == 43 &&
					((int)rbuf[i+4] >= 48 && (int)rbuf[i+4] <= 57) && ((int)rbuf[i+5] >= 48 && (int)rbuf[i+5] <= 57) &&
					((int)rbuf[i+6] >= 48 && (int)rbuf[i+6] <= 57) && ((int)rbuf[i+7] >= 48 && (int)rbuf[i+7] <= 57) &&
					((int)rbuf[i+8] >= 48 && (int)rbuf[i+8] <= 57) && ((int)rbuf[i+9] >= 48 && (int)rbuf[i+9] <= 57) &&
					(int)rbuf[i+10] == 46 && ((int)rbuf[i+11] >= 48 && (int)rbuf[i+11] <= 57)){
						NowVolume += AsciiNumReturn((int)rbuf[i+4])*100000;
						NowVolume += AsciiNumReturn((int)rbuf[i+5])*10000;
						NowVolume += AsciiNumReturn((int)rbuf[i+6])*1000;
						NowVolume += AsciiNumReturn((int)rbuf[i+7])*100;
						NowVolume += AsciiNumReturn((int)rbuf[i+8])*10;
						NowVolume += AsciiNumReturn((int)rbuf[i+9])*1;
						NowVolume += AsciiNumReturn((int)rbuf[i+11])*0.1;
						Keiryo = NowVolume;
						Edit_V_msr_Keiryo.Format(_T("%.1f"), NowVolume);
						UpdateData(FALSE);
						break;
				}

				else if((int)rbuf[i] == 83 && (int)rbuf[i+1] == 84 && (int)rbuf[i+2] == 44 && (int)rbuf[i+3] == 43 &&
					((int)rbuf[i+4] >= 48 && (int)rbuf[i+4] <= 57) && ((int)rbuf[i+5] >= 48 && (int)rbuf[i+5] <= 57) &&
					((int)rbuf[i+6] >= 48 && (int)rbuf[i+6] <= 57) && ((int)rbuf[i+7] >= 48 && (int)rbuf[i+7] <= 57) &&
					((int)rbuf[i+8] >= 48 && (int)rbuf[i+8] <= 57) && ((int)rbuf[i+9] == 46) &&
					((int)rbuf[i+10] >= 48 && (int)rbuf[i+10] <= 57) && ((int)rbuf[i+11] >= 48 && (int)rbuf[i+11] <= 57)){
						NowVolume += AsciiNumReturn((int)rbuf[i+4])*10000;
						NowVolume += AsciiNumReturn((int)rbuf[i+5])*1000;
						NowVolume += AsciiNumReturn((int)rbuf[i+6])*100;
						NowVolume += AsciiNumReturn((int)rbuf[i+7])*10;
						NowVolume += AsciiNumReturn((int)rbuf[i+8])*1;
						NowVolume += AsciiNumReturn((int)rbuf[i+10])*0.1;
						NowVolume += AsciiNumReturn((int)rbuf[i+11])*0.01;
						Keiryo = NowVolume;
						Edit_V_msr_Keiryo.Format(_T("%.2f"), NowVolume);
						UpdateData(FALSE);
						break;
				}

				else if((int)rbuf[i] == 83 && (int)rbuf[i+1] == 84 && (int)rbuf[i+2] == 44 && (int)rbuf[i+3] == 43 &&
					((int)rbuf[i+4] >= 48 && (int)rbuf[i+4] <= 57) && ((int)rbuf[i+5] >= 48 && (int)rbuf[i+5] <= 57) &&
					((int)rbuf[i+6] >= 48 && (int)rbuf[i+6] <= 57) && ((int)rbuf[i+7] >= 48 && (int)rbuf[i+7] <= 57) &&
					(int)rbuf[i+8] >= 46 && ((int)rbuf[i+9] >= 48 && (int)rbuf[i+9] <= 57) &&
					((int)rbuf[i+10] >= 48 && (int)rbuf[i+10] <= 57) && ((int)rbuf[i+11] >= 48 && (int)rbuf[i+11] <= 57)){
						NowVolume += AsciiNumReturn((int)rbuf[i+4])*1000;
						NowVolume += AsciiNumReturn((int)rbuf[i+5])*100;
						NowVolume += AsciiNumReturn((int)rbuf[i+6])*10;
						NowVolume += AsciiNumReturn((int)rbuf[i+7])*1;
						NowVolume += AsciiNumReturn((int)rbuf[i+9])*0.1;
						NowVolume += AsciiNumReturn((int)rbuf[i+10])*0.01;
						NowVolume += AsciiNumReturn((int)rbuf[i+11])*0.001;
						Keiryo = NowVolume;
						Edit_V_msr_Keiryo.Format(_T("%.3f"), NowVolume);
						UpdateData(FALSE);
						break;
				}
				//20180313�ǋL�@0.0001���\���p
				else if((int)rbuf[i] == 83 && (int)rbuf[i+1] == 84 && (int)rbuf[i+2] == 44 && (int)rbuf[i+3] == 43 &&
					((int)rbuf[i+4] >= 48 && (int)rbuf[i+4] <= 57) && ((int)rbuf[i+5] >= 48 && (int)rbuf[i+5] <= 57) &&
					((int)rbuf[i+6] >= 48 && (int)rbuf[i+6] <= 57) && (int)rbuf[i+7] >= 46 &&
					 ((int)rbuf[i+8] >= 48 && (int)rbuf[i+8] <= 57) && ((int)rbuf[i+9] >= 48 && (int)rbuf[i+9] <= 57) &&
					((int)rbuf[i+10] >= 48 && (int)rbuf[i+10] <= 57) && ((int)rbuf[i+11] >= 48 && (int)rbuf[i+11] <= 57)){
						NowVolume += AsciiNumReturn((int)rbuf[i+4])*100;
						NowVolume += AsciiNumReturn((int)rbuf[i+5])*10;
						NowVolume += AsciiNumReturn((int)rbuf[i+6])*1;
						NowVolume += AsciiNumReturn((int)rbuf[i+8])*0.1;
						NowVolume += AsciiNumReturn((int)rbuf[i+9])*0.01;
						NowVolume += AsciiNumReturn((int)rbuf[i+10])*0.001;
						NowVolume += AsciiNumReturn((int)rbuf[i+11])*0.0001;
						Keiryo = NowVolume;
						Edit_V_msr_Keiryo.Format(_T("%.4f"), NowVolume);
						UpdateData(FALSE);
						break;
				}
				//20180409�ǋL�@XXXX.YYYY�\���Ή�
				else if((int)rbuf[i] == 83 && (int)rbuf[i+1] == 84 && (int)rbuf[i+2] == 44 && (int)rbuf[i+3] == 43 &&
					((int)rbuf[i+4] >= 48 && (int)rbuf[i+4] <= 57) && ((int)rbuf[i+5] >= 48 && (int)rbuf[i+5] <= 57) &&
					((int)rbuf[i+6] >= 48 && (int)rbuf[i+6] <= 57) && ((int)rbuf[i+7] >= 48 && (int)rbuf[i+7] <= 57) && (int)rbuf[i+8] >= 46 &&
					 ((int)rbuf[i+9] >= 48 && (int)rbuf[i+9] <= 57) && ((int)rbuf[i+10] >= 48 && (int)rbuf[i+10] <= 57) &&
					((int)rbuf[i+11] >= 48 && (int)rbuf[i+11] <= 57) && ((int)rbuf[i+12] >= 48 && (int)rbuf[i+12] <= 57)){
						NowVolume += AsciiNumReturn((int)rbuf[i+4])*1000;
						NowVolume += AsciiNumReturn((int)rbuf[i+5])*100;
						NowVolume += AsciiNumReturn((int)rbuf[i+6])*10;
						NowVolume += AsciiNumReturn((int)rbuf[i+7])*1;
						NowVolume += AsciiNumReturn((int)rbuf[i+9])*0.1;
						NowVolume += AsciiNumReturn((int)rbuf[i+10])*0.01;
						NowVolume += AsciiNumReturn((int)rbuf[i+11])*0.001;
						NowVolume += AsciiNumReturn((int)rbuf[i+12])*0.0001;
						Keiryo = NowVolume;
						Edit_V_msr_Keiryo.Format(_T("%.4f"), NowVolume);
						UpdateData(FALSE);
						break;
				}
			}

			//�O���猟��
			/*�R�����g�A�E�g20151116
			for(i=0;i<er;i++){
				temp = (int)rbuf[i];//�A�X�L�[�R�[�h���擾

				if(temp == 43){//+������������(2���)
					startpoint = i;
				}
				else if(temp == 46){//.������������
					dotpoint = 1;
					break;
				}

				//else if(temp == 43 && secondp == 0)//+������������(1���)
				//	secondp = 1;

				//else if(rbuf[0] == 0)break;//���������Ă��Ȃ��Ƃ��̓��[�v�𔲂���
			}

			//trace.Format(_T("%c%c%c%c%c%c%c%c%c\n"), rbuf[startpoint], rbuf[startpoint+1], rbuf[startpoint+2], rbuf[startpoint+3], rbuf[startpoint+4], rbuf[startpoint+5], rbuf[startpoint+6], rbuf[startpoint+7], rbuf[startpoint+8]);
			//TRACE(trace);
			//���O�ɑS�Ă̈ʂɐ����������Ă��邩�m�F
			for(i=startpoint+1;i<startpoint+9;i++){
			temp = (int)rbuf[i];//�A�X�L�[�R�[�h���擾
			Num = AsciiNumReturn(temp);

			//�ǉ�//
			cnt++;
			if(Num == -1)
				dotnum = cnt;
			//�ǉ�//

			if(Num != -1){
				allcheck++;
			}
			}

			if(allcheck == 7){//7���S�Ăɐ����������Ă�����
			for(i=startpoint+1;i<startpoint+9;i++){
				temp = (int)rbuf[i];//�A�X�L�[�R�[�h���擾
				if(temp != 48){//0�ȊO�̐����̂Ƃ�
					Num = AsciiNumReturn(temp);
					if(Num != -1 && dotnum == 5){
						if(keta == 0)NowVolume += Num*1000;
						else if(keta == 1)NowVolume += Num*100;
						else if(keta == 2)NowVolume += Num*10;
						else if(keta == 3)NowVolume += Num*1;
						else if(keta == 5)NowVolume += Num*0.1;
						else if(keta == 6)NowVolume += Num*0.01;
						else if(keta == 7)NowVolume += Num*0.001;
					}

					else if(Num != -1 && dotnum == 6){
						if(keta == 0)NowVolume += Num*10000;
						else if(keta == 1)NowVolume += Num*1000;
						else if(keta == 2)NowVolume += Num*100;
						else if(keta == 3)NowVolume += Num*10;
						else if(keta == 4)NowVolume += Num*1;
						else if(keta == 6)NowVolume += Num*0.1;
						else if(keta == 7)NowVolume += Num*0.01;
					}
				}
				if(keta == 7)break;//�����_���ʂ܂Œl���m�F�����烋�[�v�𔲂���
				keta++;
			}
			Keiryo = NowVolume;
			}
			//temp = _tcstod(tmp[i],NULL);//CString����Double��
			//if(NowVolume != 0){
			//	CStVolume.Format(_T("%g"), NowVolume);//Double����CString��
			//	AfxMessageBox(CStVolume);
			//}

		

			//trace.Format(_T("%g\n"), NowVolume);
			//TRACE(trace);
			if(startpoint != -1 && dotpoint != -1){
			Edit_V_msr_Keiryo.Format(_T("%.2f"), NowVolume);
			//�ϐ�����Edit��
			UpdateData(FALSE);
			}
�R�����g�A�E�g*/

			//��������M�ł��Ă��Ȃ��Ƃ�
		//	if(er != 128){
		//		er = AfxMessageBox(_T("Recv Timeout"),MB_RETRYCANCEL);
		//		if(er == IDCANCEL){
		//			pWnd->rs232c.ClearRecv(n);
		//			//return 3;
		//	}
		//}	

		pWnd->rs232c.Close(n);

			break;
			
		default:
			break;
	}

	CDialog::OnTimer(nIDEvent);

}

int Cmeasure::AsciiNumReturn(int Ascii)
{
	int Num;
	if(Ascii == 48)Num=0;
	else if(Ascii == 49)Num=1;
	else if(Ascii == 50)Num=2;
	else if(Ascii == 51)Num=3;
	else if(Ascii == 52)Num=4;
	else if(Ascii == 53)Num=5;
	else if(Ascii == 54)Num=6;
	else if(Ascii == 55)Num=7;
	else if(Ascii == 56)Num=8;
	else if(Ascii == 57)Num=9;

	else Num = -1;

	return Num;
}

void Cmeasure::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	int i,j;
	for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
	//���X�g�R���g���[��2�����z���������
	for(i=0; i<15; i++)
		for(j=0; j<15; j++)
			ListC[i][j] = _T("");

	Edit_V_msr_Keiryo = _T("");

	//�ϐ�����Edit��
	UpdateData(FALSE);


	////if(pWnd->dlflag_JyushiCode == TRUE)
	//	pWnd->dlflag_JyushiCode == FALSE;
	////if(pWnd->dlflag_password == TRUE)
	//	pWnd->dlflag_password == FALSE;
	////if(pWnd->dlflag_renum == TRUE)
	//	pWnd->dlflag_renum == FALSE;


	pWnd->dlflag_measure = FALSE;

	

	DestroyWindow();

	CDialog::OnClose();
}

void Cmeasure::OnBnClickedRej()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int cnt=0;

	ItemNum = ListView_GetItemCount(m_listc);//�s�����擾

	for(int i=0;i<ItemNum;i++){
		if(ListView_GetCheckState(m_listc,i)!=0){
			CheckBoxNum = i;
			cnt++;
			if(cnt >=2){
			AfxMessageBox(_T("�����`�F�b�N����Ă��܂�."));
			break;
			}
		}
	}

	if(cnt == 0)AfxMessageBox(_T("�`�F�b�N����Ă��܂���."));

	else if(/*pWnd->dlflag_password == FALSE && */cnt == 1){
			//�����̌^�Ԃ������Ă��邩�m�F����
			pWnd->dlflag_password = TRUE;
			//pWnd->PW.Create(IDD_PASSWORD,this);
			//pWnd->PW.ShowWindow(SW_SHOWNA);//�\��
			pWnd->PW.DoModal();

			//m_listc.SetItemText(1, 1, _T("aaa"));//4��ڂ֒ǉ�����
			if(pWnd->RN.Edit_V_rn != _T(""))
			m_listc.SetItemText(pWnd->Jshg.msr.CheckBoxNum, 2, pWnd->RN.Edit_V_rn);//3��ڂ֒ǉ�����

			pWnd->RN.Edit_V_rn = _T("");
	}
}

void Cmeasure::OnLvnItemchangedList1(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	*pResult = 0;
}

HBRUSH Cmeasure::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	// TODO:  ������ DC �̑�����ύX���Ă��������B

	// �G�f�B�b�g�{�b�N�X�̐F�ύX
    //if (nCtlColor == CTLCOLOR_EDIT)
	//if (nCtlColor == IDD_measure)
	//if(Keiryo - RValue <= gosa && Keiryo - RValue >= (gosa*(-1))){//�덷�܂߁AOK�ł����
	//20180409���Z
//	if(Keiryo - RValue <= (RValue*gosa/100) && Keiryo - RValue >= (gosa*(-1)*RValue/100)){//�덷�܂߁AOK�ł����
	if(Keiryo - RValue <= gosa && Keiryo - RValue >= (gosa*(-1))){//�덷�܂߁AOK�ł����
        if (*pWnd == Edit_C_msr_Keiryo)
        {
            // �����F
            pDC->SetTextColor(RGB(255, 0, 0));
            // �w�i�F
            //hbr = static_cast<HBRUSH>(GetStockObject(BLACK_BRUSH));
            //pDC->SetBkColor(RGB(255, 0, 0));
        }
	}

	// TODO:  ����l���g�p�������Ȃ��ꍇ�͕ʂ̃u���V��Ԃ��܂��B
	return hbr;
}
