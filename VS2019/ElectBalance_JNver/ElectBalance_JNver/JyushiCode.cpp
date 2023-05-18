// JyushiCode.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "JyushiCode.h"

#include "ElectBalance_JNverDlg.h"

#define TIMER_LOOP 9876543


// CJyushiCode ダイアログ

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
END_MESSAGE_MAP()


// CJyushiCode メッセージ ハンドラ

BOOL CJyushiCode::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	//flg_same = FALSE;

	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CJyushiCode::OnEnChangeEditJyushicode()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
	//int i,j;
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	Num = pWnd->Jshg.msr.CheckBoxNum;
	int ms = _ttoi(pWnd->Timer_ms);
	int i,bothsame = 0;

	//Editから変数へ
	UpdateData(TRUE);

//一致しているものを探す
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(pWnd->Jshg.msr.Edit_Measure_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(pWnd->Jshg.msr.Edit_Measure_WRank) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}



	/*AfxMessageBox(pWnd->Jyushi_List[i][3]);
	AfxMessageBox(Edit_V_JC);*/


	if(Edit_V_JC.Find(pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][1]) != -1){//文字列が含まれていたら
	//if(Edit_V_JC.Find(pWnd->Jyushi_List[i][3]) != -1){//文字列が含まれていたら
		//AfxMessageBox(_T("見つかりました."));
		//flg_same = TRUE;
		pWnd->dlflag_JyushiCode = FALSE;//ダイアログを閉じるのでFALSE
		pWnd->Jshg.msr.Bn_msr_Kend.EnableWindow(TRUE);//計測終了ボタンを有効化
		pWnd->Jshg.msr.Bn_msr_KStart.EnableWindow(FALSE);//計測開始ボタンを無効化
		pWnd->Jshg.msr.m_listc.EnableWindow(FALSE);//リストコントロールのチェックボックスを無効にする
		pWnd->Jshg.msr.SetTimer(TIMER_LOOP,ms,NULL);//タイマーの呼び出し


		OnClose();
		DestroyWindow();
	}
	/*if(Edit_V_JC.Find(_T("\n")) != -1){//改行コードが入力されたら
	AfxMessageBox(_T("発見."));
	pWnd->dlflag_JyushiCode = FALSE;
	OnClose();
	DestroyWindow();
	}*/
}

void CJyushiCode::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_JyushiCode = FALSE;
	DestroyWindow();

	CDialog::OnClose();
}
