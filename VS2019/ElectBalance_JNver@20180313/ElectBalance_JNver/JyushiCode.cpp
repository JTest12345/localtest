// JyushiCode.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "JyushiCode.h"

#include "ElectBalance_JNverDlg.h"

#define TIMER_LOOP 9876543


// CJyushiCode �_�C�A���O

IMPLEMENT_DYNAMIC(CJyushiCode, CDialog)

CJyushiCode::CJyushiCode(CWnd* pParent /*=NULL*/)
	: CDialog(CJyushiCode::IDD, pParent)
	, Edit_V_JC(_T(""))
{

}

CJyushiCode::~CJyushiCode()
{
}

void CJyushiCode::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_JyushiCode, Edit_C_JC);
	DDX_Text(pDX, IDC_EDIT_JyushiCode, Edit_V_JC);
}


BEGIN_MESSAGE_MAP(CJyushiCode, CDialog)
	ON_EN_CHANGE(IDC_EDIT_JyushiCode, &CJyushiCode::OnEnChangeEditJyushicode)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_BUTTON_JC_ENTER, &CJyushiCode::OnBnClickedButtonJcEnter)
END_MESSAGE_MAP()


// CJyushiCode ���b�Z�[�W �n���h��

BOOL CJyushiCode::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	//flg_same = FALSE;
	/*int i;
	//JyushiLot�z���������
	for(i=0; i<10; i++){
		JyushiKata[i] = _T("");
		JyushiLot[i] = _T("");
	}*/

	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CJyushiCode::OnEnChangeEditJyushicode()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
	//int i,j;
	/*20151116�R�����g�A�E�g�{�^�����������ɕύX
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	Num = pWnd->Jshg.msr.CheckBoxNum;
	int ms = _ttoi(pWnd->Timer_ms);
	int i,bothsame = 0;

	//Edit����ϐ���
	UpdateData(TRUE);

//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(pWnd->Jshg.msr.Edit_Measure_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(pWnd->Jshg.msr.Edit_Measure_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(pWnd->Jshg.msr.Edit_Measure_Setubi) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}
�����܂�*/


	/*AfxMessageBox(pWnd->Jyushi_List[i][3]);
	AfxMessageBox(Edit_V_JC);*/

    /*20151116�R�����g�A�E�g�{�^�����������ɕύX
	if(Edit_V_JC.Find(pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][1]) != -1){//�����񂪊܂܂�Ă�����
	//if(Edit_V_JC.Find(pWnd->Jyushi_List[i][3]) != -1){//�����񂪊܂܂�Ă�����
		//AfxMessageBox(_T("������܂���."));
		//flg_same = TRUE;
		pWnd->dlflag_JyushiCode = FALSE;//�_�C�A���O�����̂�FALSE
		pWnd->Jshg.msr.Bn_msr_Kend.EnableWindow(TRUE);//�v���I���{�^����L����
		pWnd->Jshg.msr.Bn_msr_KStart.EnableWindow(FALSE);//�v���J�n�{�^���𖳌���
		pWnd->Jshg.msr.m_listc.EnableWindow(FALSE);//���X�g�R���g���[���̃`�F�b�N�{�b�N�X�𖳌��ɂ���
		pWnd->Jshg.msr.SetTimer(TIMER_LOOP,ms,NULL);//�^�C�}�[�̌Ăяo��

		OnClose();
		DestroyWindow();
	}
	�����܂�*/
	/*if(Edit_V_JC.Find(_T("\n")) != -1){//���s�R�[�h�����͂��ꂽ��
	AfxMessageBox(_T("����."));
	pWnd->dlflag_JyushiCode = FALSE;
	OnClose();
	DestroyWindow();
	}*/
}

void CJyushiCode::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_JyushiCode = FALSE;
	DestroyWindow();

	CDialog::OnClose();
}

void CJyushiCode::OnBnClickedButtonJcEnter()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B	

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	Num = pWnd->Jshg.msr.CheckBoxNum;
	int ms = _ttoi(pWnd->Timer_ms);
	int i,bothsame = 0;
	CTime cTime;
	CString CurrentDate;
	CString BarcodeDate;
	int CD = 0,BD = 0;
	cTime = CTime::GetCurrentTime();           // ���ݎ���
	CurrentDate = cTime.Format("%Y%m%d");   //"YYYYmmdd"�`���̎�����������擾

	//Edit����ϐ���
	UpdateData(TRUE);

//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(pWnd->Jshg.msr.Edit_Measure_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(pWnd->Jshg.msr.Edit_Measure_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(pWnd->Jshg.msr.Edit_Measure_Setubi) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}

	//20180417�@�ԍڗp�A�L�������̎擾���@�ύX
	int a=0,j;
	int cnttest=0;
	CString tmp,JLot;

	tmp = Edit_V_JC;

	//JyushiLot�z���������
	/*for(i=0; i<10; i++){
		JyushiKata[i] = _T("");
		JyushiLot[i] = _T("");
	}*/

	//�L�������̏ꏊ�܂ŕ�������폜
	for(i=0; i<7; i++){
		a = tmp.Find(_T(","));
		if(a!=-1){
			//�����^�Ԃ��擾
			if(i==0){
				JLot = tmp;
				a = JLot.Find(_T(","));
				for(j=0; j<10;j++){
					if(JyushiKata[j]==_T("")){
					JyushiKata[j]=JLot.Left(a);
					break;
					}
				}
			}
			tmp.Delete(0, a+1);
			//�������b�g���擾
			if(i==2){
				JLot = tmp;
				a = JLot.Find(_T(","));
				for(j=0; j<10;j++){
					if(JyushiLot[j]==_T("")){
					JyushiLot[j]=JLot.Left(a);
					break;
					}
				}
			}
		}
	}
	//�L�������擪�����ɑ΂��āA6�������擾����
	tmp = tmp.Left(6);

	BarcodeDate = tmp;
	//BarcodeDate = Edit_V_JC.Right(8);//�o�[�R�[�h�̉E�[����g�p�������擾
	CD = _tstoi(CurrentDate.Right(6));
	BD = _tstoi(BarcodeDate);
	if(Edit_V_JC.Find(pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][1]) != -1){//�����񂪊܂܂�Ă�����
	//if(Edit_V_JC.Find(pWnd->Jyushi_List[i][3]) != -1){//�����񂪊܂܂�Ă�����
		//AfxMessageBox(_T("������܂���."));
		//flg_same = TRUE;
		if(CD <= BD){//CString����Int�� �o�[�R�[�h���t�̕��������̏ꍇ
		pWnd->dlflag_JyushiCode = FALSE;//�_�C�A���O�����̂�FALSE
		pWnd->Jshg.msr.Bn_msr_Kend.EnableWindow(TRUE);//�v���I���{�^����L����
		pWnd->Jshg.msr.Bn_msr_KStart.EnableWindow(FALSE);//�v���J�n�{�^���𖳌���
		pWnd->Jshg.msr.m_listc.EnableWindow(FALSE);//���X�g�R���g���[���̃`�F�b�N�{�b�N�X�𖳌��ɂ���
		pWnd->Jshg.msr.SetTimer(TIMER_LOOP,ms,NULL);//�^�C�}�[�̌Ăяo��

		OnClose();
		DestroyWindow();
		}
		else AfxMessageBox(_T("�������L�������؂�ł�"));
	}
	else AfxMessageBox(_T("�����^�Ԃ��Ⴂ�܂�"));

	
	/*if(Edit_V_JC.Find(_T("\n")) != -1){//���s�R�[�h�����͂��ꂽ��
	AfxMessageBox(_T("����."));
	pWnd->dlflag_JyushiCode = FALSE;
	OnClose();
	DestroyWindow();
	}*/
	//CurrentDate
	
}
