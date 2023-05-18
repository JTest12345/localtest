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
	for(i=0; i<10; i++)
		for(j=0; j<10; j++)
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
CString tmp[25];

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
	lvcol.cx = 80;
	m_listc.InsertColumn(0,&lvcol);
	//3�̃J����
	lvcol.pszText = _T("�����^��");
	lvcol.cx = 80;
	m_listc.InsertColumn(1,&lvcol);
	//4�̃J����
	lvcol.pszText = _T("���_�l");
	lvcol.cx = 60;
	m_listc.InsertColumn(2,&lvcol);
	//5�̃J����
	lvcol.pszText = _T("�v�ʒl");
	lvcol.cx = 60;
	m_listc.InsertColumn(3,&lvcol);

	//m_listc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_CHECKBOXES);

	CString str = pWnd->Jyushidatacsv;
	
	Keiryo = 0;//�K���Ȑ��l�ŏ�����
	RValue = 10000;//�K���Ȑ��l�ŏ�����
	gosa = _tcstod(pWnd->gosa,NULL);//�덷�͈̔�

//�t�@�C���ǂݍ���
Num = _tcstod(msr_volume,NULL);//double�֌^�ϊ�
Num = Num/100;//�䗦���v�Z

for(i=0; i<10; i++) Volume[i] = -1;//������

CheckBoxNum = 0;//�`�F�b�N����Ă���s���i�[����ϐ���������

//Edit����ϐ���
UpdateData(TRUE);
	
UpdateData(FALSE);

//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Edit_Measure_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(Edit_Measure_WRank) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}

	j=2;//�ܖ��������
	while(pWnd->Jyushi_List[i][j] != _T("")){
		if(first == 0){
			m_listc.InsertItem(row_cnt,pWnd->Jyushi_List[i][j]);
			first++;
			ListC[row_cnt-1][0]=pWnd->Jyushi_List[i][j];//�g�p���������i�[
			}
		else{
			ListC[row_cnt-1][col_cnt]=pWnd->Jyushi_List[i][j];//1�s2��ڈȍ~���i�[
			if(j==4 || j == 7 || j == 10 || j == 13 || j == 16 || j == 19){
				temp = _tcstod(ListC[row_cnt-1][col_cnt],NULL);//CString����Double��
					temp = temp * Num;//�䗦��������
					ListC[row_cnt-1][col_cnt].Format(_T("%g"), temp);//Double����CString��
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
					
	//if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

	//	while(fp.ReadString(buf)){
	//		//,��؂�̃f�[�^��tmp�ɒ��o
	//		for(i=0; i<25; i++) tmp[i] = _T("");
	//		//KishuName=0,SZ=0,Other1=0,Other2=0,Other3=0,Other4=0,Other5=0;

	//		for(i=0; i<25; i++){
	//			a = buf.Find(_T(","));
	//			if(a!=-1){
	//				tmp[i] = buf.Left(a);

	//				if(tmp[i].Compare(_T("endpoint")) == 0){
	//					cnt = 2;
	//					break;
	//				}

	//				//test++;//�f�o�b�O�p

	//				
	//				if(Edit_Measure_Kishu.Compare(tmp[i]) == 0)cnt=1;

	//				//��x��v������u���C�N���Ȃ�
	//				if(Edit_Measure_Kishu.Compare(tmp[i]) != 0 && cnt == 0)break;
	//				
	//				else{
	//				
	//				
	//				if(i == 4 || i == 7 || i == 10 || i == 13 || i == 16 || i == 19){
	//				temp = _tcstod(tmp[i],NULL);//CString����Double��
	//				temp = temp * Num;//�䗦��������
	//				tmp[i].Format(_T("%g"), temp);//Double����CString��
	//				if(vcnt<10)
	//				Volume[vcnt] = temp;//�e�s�̗��_�l���i�[
	//				vcnt++;
	//				}

	//				if(first < 2){//�@�햼�A�g�������N�͕\�ɓ���Ȃ�
	//				first++;
	//				if(first == 2){//�g�������N����v���邩�m�F����
	//					if(Edit_Measure_WRank.Compare(tmp[i]) != 0){
	//						AfxMessageBox(_T("�g�������N���o�^����Ă��܂���."));
	//						break;
	//					}

	//				}
	//				}
	//				else if(first == 2){
	//				m_listc.InsertItem(row_cnt,tmp[i]);
	//				first++;
	//				ListC[row_cnt-1][col_cnt-1]=tmp[i];//�g�p���������i�[
	//				}
	//				else{
	//				m_listc.SetItemText(row_cnt-1, col_cnt, tmp[i]);//2��ڂ֒ǉ�����
	//				ListC[row_cnt-1][col_cnt]=tmp[i];//1�s2��ڈȍ~���i�[
	//				
	//				col_cnt++;
	//				if(col_cnt == 3){
	//					col_cnt = 1;
	//					row_cnt++;
	//					first = 2;
	//				}

	//				}
	//				
	//				buf.Delete(0, a+1);

	//				}

	//			}
	//				//break;//�@�햼�̂݃��X�g�ɂ��ďI��
	//					
	//		}
	//				if(cnt == 2)break;
	//				//buf.Delete(0, a+1);
	//	}
	//			
	//}

	//	fp.Close();

	Edit_C_msr_Kishu.SetFont(&new_font,TRUE);
	Edit_C_msr_WRank.SetFont(&new_font,TRUE);
	Edit_C_msr_Keiryo.SetFont(&new_font,TRUE);
	Edit_C_msr_Sagyousha.SetFont(&new_font,TRUE);

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
    Invalidate();

}

void Cmeasure::OnBnClickedKend()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	int i,j,cnt=0;
	CString Stest;
	//double Keiryo;
	
	char sbuf[128];//��M�p
	memset(sbuf, 0, 128);

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	CString str = pWnd->Resultcsv;
	gosa = _tcstod(pWnd->gosa,NULL);//�덷�͈̔�
	//double RValue;
	int fns_flag = 0;

	//�t�@�C���ǂݍ���
	CStdioFile fp;
	//�t�@�C���̏�������
	CStdioFile ofp;

	CString buf;
	CString tmp[16];

	CTime cTime;
	CString date; 

	cTime = CTime::GetCurrentTime();           // ���ݎ���
    date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"�`���̎�����������擾

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
			//���̑���ɔ����A�K�v�ȕϐ���������
			for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
			FinishCnt = 0;
			fns_flag = 1;
			CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
			
			if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
	
				ofp.SeekToEnd();

				
				ofp.WriteString(date);//���t
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_J_Kishu);//�@�햼
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.W_rank_V);//�g�������N
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.LotNo);//���b�g�i���o�[
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_V_J_Sagyousha);//��ƎҖ�
				ofp.WriteString(_T(","));

				for(i=0;i<ItemNum;i++){
					for(j=0;j<4;j++){
						ofp.WriteString(ListC[i][j]);
						ofp.WriteString(_T(","));
					}
				}
				ofp.WriteString(_T("\n"));
			}
			ofp.Close();

			m_listc.EnableWindow(TRUE);//�`�F�b�N�{�b�N�X��L����
			m_listc.SetCheck(CheckBoxNum,FALSE);//�`�F�b�N�{�b�N�X�̃`�F�b�N�ӏ����O��
			Bn_msr_KStart.EnableWindow(TRUE);//�v���J�n�{�^����L����
			KillTimer(TIMER_LOOP);//�^�C�}�[���X�g�b�v
			
			AfxMessageBox(_T("�f�[�^�̏������݂��������܂���."));

			//Jyushihaigou�_�C�A���O�̊eEdit�̒l��������
			pWnd->Jshg.LotNo = _T("");
			pWnd->Jshg.Cb_J_Kishu = _T("");
			pWnd->Jshg.W_rank_V = _T("");
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
	int temp,Num,keta=0,secondp=0,allcheck=0;
	int startpoint=-1,dotpoint=-1;
	NowVolume=0;
	er = 0;

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

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

			//�O���猟��
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
			if(Num != -1){
				allcheck++;
			}
			}

			if(allcheck == 7){//7���S�Ăɐ����������Ă�����
			for(i=startpoint+1;i<startpoint+9;i++){
				temp = (int)rbuf[i];//�A�X�L�[�R�[�h���擾
				if(temp != 48){//0�ȊO�̐����̂Ƃ�
					Num = AsciiNumReturn(temp);
					if(Num != -1){
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
			Edit_V_msr_Keiryo.Format(_T("%g"), NowVolume);
			//�ϐ�����Edit��
			UpdateData(FALSE);
			}

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
	for(i=0; i<10; i++)
		for(j=0; j<10; j++)
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
