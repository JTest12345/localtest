// Password.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Password.h"

#include "ElectBalance_JNverDlg.h"

// CPassword ダイアログ

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


// CPassword メッセージ ハンドラ

void CPassword::OnEnChangeEdit1()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int Num = pWnd->Jshg.msr.CheckBoxNum;

	//Editから変数へ
	UpdateData(TRUE);

	if(Edit_V_Pass.Find(_T("jhg")) != -1){//文字列が含まれていたら
		pWnd->dlflag_password = FALSE;//ダイアログを閉じるのでFALSE
		//pWnd->Jshg.msr.Bn_msr_Kend.EnableWindow(TRUE);//計測終了ボタンを有効化
		//pWnd->Jshg.msr.Bn_msr_KStart.EnableWindow(FALSE);//計測開始ボタンを無効化
		//pWnd->Jshg.msr.SetTimer(TIMER_LOOP,5,NULL);//タイマーの呼び出し

		if(pWnd->dlflag_renum == FALSE){
			//樹脂の型番が合っているか確認する
			pWnd->dlflag_renum = TRUE;
			//pWnd->RN.Create(IDD_RENUM,this);
			//pWnd->RN.ShowWindow(SW_SHOWNA);//表示
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
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	/*CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_password = FALSE;
	DestroyWindow();*/

	CDialog::OnClose();
}
