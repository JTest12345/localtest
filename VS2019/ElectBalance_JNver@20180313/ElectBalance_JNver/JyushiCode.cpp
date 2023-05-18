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
	ON_BN_CLICKED(IDC_BUTTON_JC_ENTER, &CJyushiCode::OnBnClickedButtonJcEnter)
END_MESSAGE_MAP()


// CJyushiCode メッセージ ハンドラ

BOOL CJyushiCode::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	//flg_same = FALSE;
	/*int i;
	//JyushiLot配列を初期化
	for(i=0; i<10; i++){
		JyushiKata[i] = _T("");
		JyushiLot[i] = _T("");
	}*/

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
	/*20151116コメントアウトボタン押下処理に変更
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	Num = pWnd->Jshg.msr.CheckBoxNum;
	int ms = _ttoi(pWnd->Timer_ms);
	int i,bothsame = 0;

	//Editから変数へ
	UpdateData(TRUE);

//一致しているものを探す
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
ここまで*/


	/*AfxMessageBox(pWnd->Jyushi_List[i][3]);
	AfxMessageBox(Edit_V_JC);*/

    /*20151116コメントアウトボタン押下処理に変更
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
	ここまで*/
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

void CJyushiCode::OnBnClickedButtonJcEnter()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。	

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	Num = pWnd->Jshg.msr.CheckBoxNum;
	int ms = _ttoi(pWnd->Timer_ms);
	int i,bothsame = 0;
	CTime cTime;
	CString CurrentDate;
	CString BarcodeDate;
	int CD = 0,BD = 0;
	cTime = CTime::GetCurrentTime();           // 現在時刻
	CurrentDate = cTime.Format("%Y%m%d");   //"YYYYmmdd"形式の時刻文字列を取得

	//Editから変数へ
	UpdateData(TRUE);

//一致しているものを探す
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

	//20180417　車載用、有効期限の取得方法変更
	int a=0,j;
	int cnttest=0;
	CString tmp,JLot;

	tmp = Edit_V_JC;

	//JyushiLot配列を初期化
	/*for(i=0; i<10; i++){
		JyushiKata[i] = _T("");
		JyushiLot[i] = _T("");
	}*/

	//有効期限の場所まで文字列を削除
	for(i=0; i<7; i++){
		a = tmp.Find(_T(","));
		if(a!=-1){
			//樹脂型番を取得
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
			//樹脂ロットを取得
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
	//有効期限先頭文字に対して、6文字を取得する
	tmp = tmp.Left(6);

	BarcodeDate = tmp;
	//BarcodeDate = Edit_V_JC.Right(8);//バーコードの右端から使用期限を取得
	CD = _tstoi(CurrentDate.Right(6));
	BD = _tstoi(BarcodeDate);
	if(Edit_V_JC.Find(pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][1]) != -1){//文字列が含まれていたら
	//if(Edit_V_JC.Find(pWnd->Jyushi_List[i][3]) != -1){//文字列が含まれていたら
		//AfxMessageBox(_T("見つかりました."));
		//flg_same = TRUE;
		if(CD <= BD){//CStringからIntへ バーコード日付の方が未来の場合
		pWnd->dlflag_JyushiCode = FALSE;//ダイアログを閉じるのでFALSE
		pWnd->Jshg.msr.Bn_msr_Kend.EnableWindow(TRUE);//計測終了ボタンを有効化
		pWnd->Jshg.msr.Bn_msr_KStart.EnableWindow(FALSE);//計測開始ボタンを無効化
		pWnd->Jshg.msr.m_listc.EnableWindow(FALSE);//リストコントロールのチェックボックスを無効にする
		pWnd->Jshg.msr.SetTimer(TIMER_LOOP,ms,NULL);//タイマーの呼び出し

		OnClose();
		DestroyWindow();
		}
		else AfxMessageBox(_T("樹脂が有効期限切れです"));
	}
	else AfxMessageBox(_T("樹脂型番が違います"));

	
	/*if(Edit_V_JC.Find(_T("\n")) != -1){//改行コードが入力されたら
	AfxMessageBox(_T("発見."));
	pWnd->dlflag_JyushiCode = FALSE;
	OnClose();
	DestroyWindow();
	}*/
	//CurrentDate
	
}
