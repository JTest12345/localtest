// Maintenance.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Maintenance.h"

#include "ElectBalance_JNverDlg.h"

// CMaintenance ダイアログ

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


// CMaintenance メッセージ ハンドラ

void CMaintenance::OnBnClickedMntKishu()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	//Mnt_K.DoModal();
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	if(pWnd->dlflag_Mnt_Kishu == FALSE){//起動していなければ
		pWnd->dlflag_Mnt_Kishu = TRUE;
		Mnt_K.Create(IDD_Mnt_Kishu,this);
		Mnt_K.ShowWindow(SW_SHOWNA);//表示
	}
	else{}//起動中の場合
}

void CMaintenance::OnBnClickedMntTanto()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	//Mnt_T.DoModal();
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	if(pWnd->dlflag_Mnt_Tanto == FALSE){//起動していなければ
		pWnd->dlflag_Mnt_Tanto = TRUE;
		Mnt_T.Create(IDD_Mnt_tanto,this);
		Mnt_T.ShowWindow(SW_SHOWNA);//表示
	}
	else{}//起動中の場合
}

void CMaintenance::OnBnClickedEnd()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。

	OnClose();
}

void CMaintenance::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
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
