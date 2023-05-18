// Jyushihaigou.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Jyushihaigou.h"

#include "ElectBalance_JNverDlg.h"

// CJyushihaigou �_�C�A���O

IMPLEMENT_DYNAMIC(CJyushihaigou, CDialog)

CJyushihaigou::CJyushihaigou(CWnd* pParent /*=NULL*/)
	: CDialog(CJyushihaigou::IDD, pParent)
	, LotNo(_T(""))
	, e_MainVolume(_T(""))
	, Cb_J_Kishu(_T(""))
	, Cb_V_J_Sagyousha(_T(""))
	, W_rank_V(_T(""))
	, Cb_V_J_WRank(_T(""))
	, Cb_J_Setubi(_T(""))
{

}

CJyushihaigou::~CJyushihaigou()
{
}

void CJyushihaigou::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT1, LotNo);
	DDX_Control(pDX, IDC_COMBO1, m_kishuCombo);
	DDX_Text(pDX, IDC_EDIT2, e_MainVolume);
	DDX_CBString(pDX, IDC_COMBO1, Cb_J_Kishu);
	DDX_Control(pDX, IDC_COMBO2, Cb_C_J_Sagyousha);
	DDX_CBString(pDX, IDC_COMBO2, Cb_V_J_Sagyousha);
	DDX_Text(pDX, IDC_EDIT3, W_rank_V);
	DDX_CBString(pDX, IDC_COMBO3, Cb_V_J_WRank);
	DDX_Control(pDX, IDC_COMBO3, Cb_C_J_WRank);
	DDX_Control(pDX, IDC_COMBO4, m_setubiCombo);
	DDX_CBString(pDX, IDC_COMBO4, Cb_J_Setubi);
}


BEGIN_MESSAGE_MAP(CJyushihaigou, CDialog)
	ON_EN_CHANGE(IDC_EDIT1, &CJyushihaigou::OnEnChangeEdit1)
	ON_BN_CLICKED(IDC_BUTTON1, &CJyushihaigou::Start)
	ON_EN_CHANGE(IDC_EDIT2, &CJyushihaigou::OnEnChangeEdit2)
	ON_BN_CLICKED(IDC_J_End, &CJyushihaigou::OnBnClickedJEnd)
//	ON_CBN_SELCHANGE(IDC_COMBO1, &CJyushihaigou::OnCbnSelchangeCombo1)
	ON_CBN_EDITCHANGE(IDC_COMBO1, &CJyushihaigou::OnCbnEditchangeCombo1)
	ON_WM_CLOSE()
	ON_EN_CHANGE(IDC_EDIT3, &CJyushihaigou::OnEnChangeEdit3)
	ON_CBN_DROPDOWN(IDC_COMBO_WRank, &CJyushihaigou::OnCbnDropdownComboWrank)
//	ON_CBN_DROPDOWN(IDC_COMBO1, &CJyushihaigou::OnCbnDropdownCombo1)
//ON_CBN_SELCHANGE(IDC_COMBO1, &CJyushihaigou::OnCbnSelchangeCombo1)
END_MESSAGE_MAP()


// CJyushihaigou ���b�Z�[�W �n���h��

void CJyushihaigou::OnEnChangeEdit1()
{
	//LotNo�̓��̓G�f�B�b�g�{�b�N�X
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
	
	//�R�����g�A�E�g20151116
	//UpdateData(TRUE);

	//�X�y�[�X���܂܂�Ă�����
	//�R�����g�A�E�g20151116
	/*
	if(LotNo.Find(_T(" ")) != -1 || LotNo.Find(_T("�@")) != -1){
      keybd_event(VK_BACK, 0, 0, 0 );
      // TAB�L�[�̉������B
      keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);
	  Tabcnt = 0;
	}
	else{
	  //Edit����ϐ���
	  UpdateData(TRUE);
	  Tabcnt = 0;
	}
	*/

}

BOOL CJyushihaigou::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	name_cnt = 0;
	Tabcnt = 0;
	WRankCnt = 0;
	//�o�^�@�탊�X�g�̓ǂݍ���
	CString str = pWnd->Jyushidatacsv;
	CString Already[SIZE];

	/*Cb_C_J_WRank.InsertString(-1, _T("D1"));
	Cb_C_J_WRank.InsertString(-1, _T("D2"));
	Cb_C_J_WRank.InsertString(-1, _T("D3"));
	Cb_C_J_WRank.InsertString(-1, _T("D4"));
	Cb_C_J_WRank.InsertString(-1, _T("�Ȃ�"));*/

	m_setubiCombo.InsertString(-1, _T("TDK"));
	m_setubiCombo.InsertString(-1, _T("MUSASHI"));
	m_setubiCombo.InsertString(-1, _T("�V���b�g�~�j"));
	m_setubiCombo.InsertString(-1, _T("quspa"));
	m_setubiCombo.InsertString(-1, _T("�V�[�g�ڒ���"));
	m_setubiCombo.InsertString(-1, _T("���˘g����"));
	m_setubiCombo.InsertString(-1, _T("���~����"));
	m_setubiCombo.InsertString(-1, _T("H.D��g����"));


	m_setubiCombo.InsertString(-1, _T("�w��Ȃ�"));


	int i,j=0,kishuno = 0,flag = 0;


	//������
	for(i=0;i<SIZE;i++){
		Already[i] = _T("");
	}

	//�R���{�{�b�N�X���X�g�ɏd������@�햼��ǉ����Ȃ�
	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
		flag = 0;
		
		for(j=0;j<kishuno;j++){
			if(Already[j] == pWnd->Jyushi_List[i][0]){
				flag = 1;
				break;
			}
		}
		Already[kishuno] = pWnd->Jyushi_List[i][0];
		kishuno++;

		if(flag == 0)
		m_kishuCombo.InsertString(-1, pWnd->Jyushi_List[i][0]);
	}
//�t�@�C���ǂݍ���
CStdioFile fp;

CString buf;
CString tmp[4];
int a;
//	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){
//
//		while(fp.ReadString(buf)){
//			//,��؂�̃f�[�^��tmp�ɒ��o	
//			for(int i=0; i<4; i++) tmp[i] = _T("");
//			for(int i=0; i<4; i++){
//				a = buf.Find(_T(","));
//				if(a!=-1){
//					tmp[i] = buf.Left(a);
//
//					m_kishuCombo.InsertString(-1, tmp[i]);			
//					
//				}
//					break;
//				}
//				buf.Delete(0, a+1);
//			}
//	}
//		fp.Close();

//��Ǝ҃��X�g�̓ǂݍ���
str = pWnd->MANcsv;

//�t�@�C���ǂݍ���
	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,��؂�̃f�[�^��tmp�ɒ��o	
			for(int i=0; i<4; i++) tmp[i] = _T("");
			for(int i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					Cb_C_J_Sagyousha.InsertString(-1, tmp[i]);			
					
				}
					break;
				}
				buf.Delete(0, a+1);
		}
	}
		fp.Close();


	//���͂��ꂽ��������擾�H
//	m_kishuCombo.GetWindowTextW(str);
	//20180409 100g��15g
	e_MainVolume = "15";//��ܗʂ̏����l��ݒ�
	Cb_J_Setubi = _T("�w��Ȃ�");//�ݔ��̎w��̓f�t�H���g�Ŏw��Ȃ�
	

	//�ϐ�����Edit��
	UpdateData(FALSE);


	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CJyushihaigou::Start()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	int i,bothsame = 0;//,a,b;

	//�@�햼�ɃX�y�[�X�������Ă�����폜
	/*if(Cb_J_Kishu.Find(" ") != -1){
	}*/

	//Cb_J_Kishu.SpanExcluding(_T("D"));
	//�ϐ�����Edit��
	//UpdateData(FALSE);//LotNo,�g�������N��ϐ��֊i�[

	//Edit����ϐ���
	UpdateData(TRUE);//LotNo,�g�������N��ϐ��֊i�[
	
	//20151116�@�햼�A���b�gNo���ꉻ
	CString Kishu_del,Lot_del;
	//20180417�ԍڗp�Ƀ��b�gNo�𕪂��đΉ�
	Kishu_del = Cb_J_Kishu;
	//Kishu_del = Cb_J_Kishu.Left(25);
	//Kishu_del = Kishu_del.TrimRight();
	//20180417�ԍڗp�Ƀ��b�gNo�𕪂��đΉ�
	//Lot_del = Cb_J_Kishu.Right(Cb_J_Kishu.GetLength()-25);

	Cb_J_Kishu = Kishu_del;//�@��
	//LotNo = Lot_del;//���b�g
	//m_kishuCombo.GetWindowTextW(Cb_J_Kishu);//�@�햼��ϐ��Ɋi�[
	Cb_C_J_WRank.GetWindowTextW(Cb_V_J_WRank);//�g�������N��ϐ��Ɋi�[
	m_setubiCombo.GetWindowTextW(Cb_J_Setubi);//�ݔ��̎�ނ�ϐ��Ɋi�[
	Cb_C_J_Sagyousha.GetWindowTextW(Cb_V_J_Sagyousha);//��Ǝ҂�ϐ��Ɋi�[

	/*
	//�ǉ� Left
	b = Cb_J_Kishu.Find(_T("�@"));//�S�p�X�y�[�X���폜
	if(b !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(b);

	a = Cb_J_Kishu.Find(_T(" "));//���p�X�y�[�X���폜
	if(a !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(a);

		*/
	//�ϐ�����Edit��
	UpdateData(FALSE);



	//�Y���@��A�g�������N�����邩�Ȃ������m�F����
	//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Cb_J_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(Cb_V_J_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(Cb_J_Setubi) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}
	
	if(bothsame == 1){

	if(pWnd->dlflag_measure == FALSE){
	if(LotNo != _T("") && Cb_V_J_WRank != _T("") && Cb_J_Kishu != _T("") && e_MainVolume != _T("") && Cb_V_J_Sagyousha != _T("") && Cb_J_Setubi != _T("")){
		pWnd->dlflag_measure = TRUE;//�N�����ɂ���

		msr.Edit_Measure_Kishu = Cb_J_Kishu;
		msr.Edit_Measure_LotNo = LotNo;
		msr.msr_volume = e_MainVolume;
		msr.msr_Sagyousha = Cb_V_J_Sagyousha;//����Ȃ��H
		msr.Edit_V_msr_Sagyousha = Cb_V_J_Sagyousha;//�G�f�B�b�gBOX�ɓ���邽�߂����̕ϐ�
		msr.Edit_Measure_WRank = Cb_V_J_WRank;//�G�f�B�b�gBOX�ɓ���邽�߂����̕ϐ�

		msr.Edit_Measure_Setubi = Cb_J_Setubi;//�G�f�B�b�gBOX�ɓ���邽�߂����̕ϐ�
		//msr.DoModal();
		msr.Create(IDD_measure,this);
		msr.ShowWindow(SW_SHOWNA);//�\��
	}
	else AfxMessageBox(_T("�����͉ӏ�������܂�."));
	}
	else{}
	}
	else{
		AfxMessageBox(_T("�Y���f�[�^������܂���.�@�햼�A�g�������N�A�ݔ��̎�ނ��m�F���ĉ�����."));
	}
}

void CJyushihaigou::OnEnChangeEdit2()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
}

void CJyushihaigou::OnBnClickedJEnd()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B

	OnClose();
}

void CJyushihaigou::OnCbnEditchangeCombo1()//�@�햼�ύX������
{
	// �@�햼�̃R���{�{�b�N�X
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	//�R�����g�A�E�g20151116
	/*
name_cnt++;

	//Edit����ϐ���
	UpdateData(TRUE);

	if(name_cnt >= 25){
		//Edit����ϐ���
		UpdateData(TRUE);

		if(Tabcnt == 0){
		// TAB�L�[
		keybd_event(VK_TAB, 0, 0, 0 );
        // TAB�L�[�̉������B
        keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
		Tabcnt = 1;
		}
		name_cnt = 0;


	}

	if(Cb_J_Kishu.Find(_T(" ")) != -1 || Cb_J_Kishu.Find(_T("�@")) != -1){
	//if(AlNum() == -1){
		if(Tabcnt == 0){
		// BackSpace�L�[
		keybd_event(VK_BACK, 0, 0, 0 );
		// BackSpace�L�[�̉������B
		keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);

		//Edit����ϐ���
		UpdateData(TRUE);

		// TAB�L�[
		keybd_event(VK_TAB, 0, 0, 0 );
        // TAB�L�[�̉������B
        keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
		Tabcnt = 1;
		}
		}

	else{
      //Edit����ϐ���
	  UpdateData(TRUE);
	}
	*/
}

void CJyushihaigou::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_Jyushihaigou = FALSE;

	if(pWnd->dlflag_measure == TRUE)
		pWnd->dlflag_measure = FALSE;

	if(pWnd->dlflag_JyushiCode == TRUE)
		pWnd->dlflag_JyushiCode = FALSE;
	if(pWnd->dlflag_password == TRUE)
		pWnd->dlflag_password = FALSE;
	if(pWnd->dlflag_renum == TRUE)
		pWnd->dlflag_renum = FALSE;

	DestroyWindow();

	CDialog::OnClose();
}

void CJyushihaigou::OnEnChangeEdit3()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
}

int CJyushihaigou::AlNum(void)
{
	int Num=-1;
	if(Cb_J_Kishu.Find(_T("A")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("B")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("C")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("D")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("E")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("F")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("G")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("H")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("I")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("J")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("K")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("L")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("M")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("N")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("O")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("P")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("Q")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("R")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("S")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("T")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("U")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("V")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("W")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("X")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("Y")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("Z")) == -1)Num=0;

	if(Cb_J_Kishu.Find(_T("a")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("b")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("c")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("d")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("e")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("f")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("g")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("h")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("i")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("j")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("k")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("l")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("m")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("n")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("o")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("p")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("q")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("r")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("s")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("t")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("u")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("v")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("w")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("x")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("y")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("z")) == -1)Num=0;

	if(Cb_J_Kishu.Find(_T("1")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("2")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("3")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("4")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("5")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("6")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("7")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("8")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("9")) == -1)Num=0;



	return Num;
}

void CJyushihaigou::OnCbnDropdownComboWrank()//�g�������N
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int i;//,a,b;

	//Edit����ϐ���
	UpdateData(TRUE);

	CString Kishu_del;
	Kishu_del = Cb_J_Kishu.Left(25);
	Kishu_del = Kishu_del.TrimRight();

	//�ǉ� Left
	/*
	b = Cb_J_Kishu.Find(_T("�@"));//�S�p�X�y�[�X���폜
	if(b !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(b);

	a = Cb_J_Kishu.Find(_T(" "));//���p�X�y�[�X���폜
	if(a !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(a);
*/
	UpdateData(FALSE);

	//�O�I���@��̔g�������N���폜����
	Cb_C_J_WRank.ResetContent();

	//Cb_C_J_WRank.DeleteString(0);
				
	//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Kishu_del) == 0 && pWnd->Jyushi_List[i][0] != _T("")){
				Cb_C_J_WRank.InsertString(-1, pWnd->Jyushi_List[i][1]);
				WRankCnt++;
				}
		else if(pWnd->Jyushi_List[i][0] == _T(""))break;
	}

	//���͂��ꂽ�@�햼�̔g�������N�̂ݕ\��������
	//Cb_C_J_WRank.InsertString(-1, _T("test"));
}
