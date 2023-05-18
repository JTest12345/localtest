// MntPassword.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "MntPassword.h"

#include "ElectBalance_JNverDlg.h"

// CMntPassword �_�C�A���O

IMPLEMENT_DYNAMIC(CMntPassword, CDialog)

CMntPassword::CMntPassword(CWnd* pParent /*=NULL*/)
	: CDialog(CMntPassword::IDD, pParent)
	, Edit_V_MntPass(_T(""))
{

}

CMntPassword::~CMntPassword()
{
}

void CMntPassword::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT1, Edit_V_MntPass);
	DDV_MaxChars(pDX, Edit_V_MntPass, 10);
}


BEGIN_MESSAGE_MAP(CMntPassword, CDialog)
	ON_EN_CHANGE(IDC_EDIT1, &CMntPassword::OnEnChangeEdit1)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CMntPassword ���b�Z�[�W �n���h��

void CMntPassword::OnEnChangeEdit1()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	//Edit����ϐ���
	UpdateData(TRUE);

	if(pWnd->Mnt_K.changepass == 1 && Edit_V_MntPass.Find(_T("regi")) != -1){
		passok = 1;
		pWnd->MntPass.EndDialog(1);
	}


	else if(pWnd->Mnt_K.changepass != 1 && Edit_V_MntPass.Find(_T("jhg")) != -1){//�����񂪊܂܂�Ă�����
		passok = 1;
		pWnd->MntPass.EndDialog(1);
		//pWnd->dlflag_MntPassword = FALSE;//�_�C�A���O�����̂�FALSE

		////if(pWnd->dlflag_Maintenance == FALSE){//�N�����Ă��Ȃ����
		//pWnd->dlflag_Maintenance = TRUE;
		//pWnd->Mnt.Create(IDD_Maintenance,this);
		//pWnd->Mnt.ShowWindow(SW_SHOWNA);//�\��
		
	//}
	//else{}//�N�����̏ꍇ
		
//pWnd->MntPass.EndDialog(1);
		
		//OnClose();
		//DestroyWindow();
	}
}

void CMntPassword::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_MntPassword = FALSE;
	ShowWindow(SW_HIDE);
	//DestroyWindow();

	CDialog::OnClose();
}

BOOL CMntPassword::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	passok = 0;

	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}
