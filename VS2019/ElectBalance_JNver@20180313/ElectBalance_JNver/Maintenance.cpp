// Maintenance.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Maintenance.h"

#include "ElectBalance_JNverDlg.h"

// CMaintenance �_�C�A���O

IMPLEMENT_DYNAMIC(CMaintenance, CDialog)

CMaintenance::CMaintenance(CWnd* pParent /*=NULL*/)
	: CDialog(CMaintenance::IDD, pParent)
{

}

CMaintenance::~CMaintenance()
{
}

void CMaintenance::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}


BEGIN_MESSAGE_MAP(CMaintenance, CDialog)
	ON_BN_CLICKED(IDC_Mnt_Kishu, &CMaintenance::OnBnClickedMntKishu)
	ON_BN_CLICKED(IDC_Mnt_Tanto, &CMaintenance::OnBnClickedMntTanto)
	ON_BN_CLICKED(IDC_End, &CMaintenance::OnBnClickedEnd)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CMaintenance ���b�Z�[�W �n���h��

void CMaintenance::OnBnClickedMntKishu()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	//Mnt_K.DoModal();
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	if(pWnd->dlflag_Mnt_Kishu == FALSE){//�N�����Ă��Ȃ����
		pWnd->dlflag_Mnt_Kishu = TRUE;
		Mnt_K.Create(IDD_Mnt_Kishu,this);
		Mnt_K.ShowWindow(SW_SHOWNA);//�\��
	}
	else{}//�N�����̏ꍇ
}

void CMaintenance::OnBnClickedMntTanto()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	//Mnt_T.DoModal();
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	if(pWnd->dlflag_Mnt_Tanto == FALSE){//�N�����Ă��Ȃ����
		pWnd->dlflag_Mnt_Tanto = TRUE;
		Mnt_T.Create(IDD_Mnt_tanto,this);
		Mnt_T.ShowWindow(SW_SHOWNA);//�\��
	}
	else{}//�N�����̏ꍇ
}

void CMaintenance::OnBnClickedEnd()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B

	OnClose();
}

void CMaintenance::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;


	if(pWnd->dlflag_Mnt_Kishu == TRUE)
		pWnd->dlflag_Mnt_Kishu = FALSE;
	if(pWnd->dlflag_Mnt_Tanto == TRUE)
		pWnd->dlflag_Mnt_Tanto = FALSE;

	pWnd->dlflag_Maintenance = FALSE;
	
	DestroyWindow();

	//pWnd->MntPass.EndDialog(1);

	CDialog::OnClose();
}
