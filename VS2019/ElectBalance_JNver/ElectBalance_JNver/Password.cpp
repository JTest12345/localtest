// Password.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Password.h"

#include "ElectBalance_JNverDlg.h"

// CPassword �_�C�A���O

IMPLEMENT_DYNAMIC(CPassword, CDialog)

CPassword::CPassword(CWnd* pParent /*=NULL*/)
	: CDialog(CPassword::IDD, pParent)
	, Edit_V_Pass(_T(""))
{

}

CPassword::~CPassword()
{
}

void CPassword::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT1, Edit_C_Pass);
	DDX_Text(pDX, IDC_EDIT1, Edit_V_Pass);
	DDV_MaxChars(pDX, Edit_V_Pass, 10);
}


BEGIN_MESSAGE_MAP(CPassword, CDialog)
	ON_EN_CHANGE(IDC_EDIT1, &CPassword::OnEnChangeEdit1)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CPassword ���b�Z�[�W �n���h��

void CPassword::OnEnChangeEdit1()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int Num = pWnd->Jshg.msr.CheckBoxNum;

	//Edit����ϐ���
	UpdateData(TRUE);

	if(Edit_V_Pass.Find(_T("jhg")) != -1){//�����񂪊܂܂�Ă�����
		pWnd->dlflag_password = FALSE;//�_�C�A���O�����̂�FALSE
		//pWnd->Jshg.msr.Bn_msr_Kend.EnableWindow(TRUE);//�v���I���{�^����L����
		//pWnd->Jshg.msr.Bn_msr_KStart.EnableWindow(FALSE);//�v���J�n�{�^���𖳌���
		//pWnd->Jshg.msr.SetTimer(TIMER_LOOP,5,NULL);//�^�C�}�[�̌Ăяo��

		if(pWnd->dlflag_renum == FALSE){
			//�����̌^�Ԃ������Ă��邩�m�F����
			pWnd->dlflag_renum = TRUE;
			//pWnd->RN.Create(IDD_RENUM,this);
			//pWnd->RN.ShowWindow(SW_SHOWNA);//�\��
			//pWnd->RN.DoModal();
		}
pWnd->RN.DoModal();
pWnd->PW.EndDialog(1);
		
		//OnClose();
		//DestroyWindow();
	}


}

void CPassword::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	/*CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_password = FALSE;
	DestroyWindow();*/

	CDialog::OnClose();
}
