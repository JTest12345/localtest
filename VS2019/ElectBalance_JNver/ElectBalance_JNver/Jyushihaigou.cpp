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
END_MESSAGE_MAP()


// CJyushihaigou ���b�Z�[�W �n���h��

void CJyushihaigou::OnEnChangeEdit1()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
	

	UpdateData(TRUE);

	//�X�y�[�X���܂܂�Ă�����
	if(LotNo.Find(_T(" ")) != -1){
      keybd_event(VK_BACK, 0, 0, 0 );
      // TAB�L�[�̉������B
      keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);
	}
	else{
	  //Edit����ϐ���
	  //UpdateData(TRUE);
	}

}

BOOL CJyushihaigou::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	name_cnt = 0;
	//�o�^�@�탊�X�g�̓ǂݍ���
	CString str = pWnd->Jyushidatacsv;


	int i;

	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
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

	e_MainVolume = "100";//��ܗʂ̏����l��ݒ�

	//�ϐ�����Edit��
	UpdateData(FALSE);


	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CJyushihaigou::Start()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	//Edit����ϐ���
	UpdateData(TRUE);//LotNo,�g�������N��ϐ��֊i�[
	
	m_kishuCombo.GetWindowTextW(Cb_J_Kishu);//�@�햼��ϐ��Ɋi�[
	Cb_C_J_Sagyousha.GetWindowTextW(Cb_V_J_Sagyousha);//��Ǝ҂�ϐ��Ɋi�[

	UpdateData(FALSE);

	if(pWnd->dlflag_measure == FALSE){
	if(LotNo != _T("") && W_rank_V != _T("") && Cb_J_Kishu != _T("") && e_MainVolume != _T("") && Cb_V_J_Sagyousha != _T("")){
		pWnd->dlflag_measure = TRUE;//�N�����ɂ���

		msr.Edit_Measure_Kishu = Cb_J_Kishu;
		msr.msr_volume = e_MainVolume;
		msr.msr_Sagyousha = Cb_V_J_Sagyousha;//����Ȃ��H
		msr.Edit_V_msr_Sagyousha = Cb_V_J_Sagyousha;//�G�f�B�b�gBOX�ɓ���邽�߂����̕ϐ�
		msr.Edit_Measure_WRank = W_rank_V;//�G�f�B�b�gBOX�ɓ���邽�߂����̕ϐ�
		//msr.DoModal();
		msr.Create(IDD_measure,this);
		msr.ShowWindow(SW_SHOWNA);//�\��
	}
	else AfxMessageBox(_T("�����͉ӏ�������܂�."));
	}
	else{}
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

void CJyushihaigou::OnCbnEditchangeCombo1()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
name_cnt++;

	//Edit����ϐ���
	UpdateData(TRUE);

	if(name_cnt == 25){
		//Edit����ϐ���
		UpdateData(TRUE);

		// TAB�L�[
		keybd_event(VK_TAB, 0, 0, 0 );
        // TAB�L�[�̉������B
        keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);

		name_cnt = 0;
	}
	//�X�y�[�X���܂܂�Ă�����
	if(Cb_J_Kishu.Find(_T(" ")) != -1){
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
		}
	else{
      //Edit����ϐ���
	  //UpdateData(TRUE);
	}
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
