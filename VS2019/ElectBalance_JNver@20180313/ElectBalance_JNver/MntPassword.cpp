// MntPassword.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "MntPassword.h"

#include "ElectBalance_JNverDlg.h"

// CMntPassword ダイアログ

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


// CMntPassword メッセージ ハンドラ

void CMntPassword::OnEnChangeEdit1()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	//Editから変数へ
	UpdateData(TRUE);

	if(pWnd->Mnt_K.changepass == 1 && Edit_V_MntPass.Find(_T("regi")) != -1){
		passok = 1;
		pWnd->MntPass.EndDialog(1);
	}


	else if(pWnd->Mnt_K.changepass != 1 && Edit_V_MntPass.Find(_T("jhg")) != -1){//文字列が含まれていたら
		passok = 1;
		pWnd->MntPass.EndDialog(1);
		//pWnd->dlflag_MntPassword = FALSE;//ダイアログを閉じるのでFALSE

		////if(pWnd->dlflag_Maintenance == FALSE){//起動していなければ
		//pWnd->dlflag_Maintenance = TRUE;
		//pWnd->Mnt.Create(IDD_Maintenance,this);
		//pWnd->Mnt.ShowWindow(SW_SHOWNA);//表示
		
	//}
	//else{}//起動中の場合
		
//pWnd->MntPass.EndDialog(1);
		
		//OnClose();
		//DestroyWindow();
	}
}

void CMntPassword::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_MntPassword = FALSE;
	ShowWindow(SW_HIDE);
	//DestroyWindow();

	CDialog::OnClose();
}

BOOL CMntPassword::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	passok = 0;

	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}
