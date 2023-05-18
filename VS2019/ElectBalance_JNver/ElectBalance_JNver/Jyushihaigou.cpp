// Jyushihaigou.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Jyushihaigou.h"

#include "ElectBalance_JNverDlg.h"

// CJyushihaigou ダイアログ

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


// CJyushihaigou メッセージ ハンドラ

void CJyushihaigou::OnEnChangeEdit1()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
	

	UpdateData(TRUE);

	//スペースが含まれていたら
	if(LotNo.Find(_T(" ")) != -1){
      keybd_event(VK_BACK, 0, 0, 0 );
      // TABキーの解放する。
      keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);
	}
	else{
	  //Editから変数へ
	  //UpdateData(TRUE);
	}

}

BOOL CJyushihaigou::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	name_cnt = 0;
	//登録機種リストの読み込み
	CString str = pWnd->Jyushidatacsv;


	int i;

	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
		m_kishuCombo.InsertString(-1, pWnd->Jyushi_List[i][0]);
	}
//ファイル読み込み
CStdioFile fp;

CString buf;
CString tmp[4];
int a;
//	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){
//
//		while(fp.ReadString(buf)){
//			//,区切りのデータをtmpに抽出	
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

//作業者リストの読み込み
str = pWnd->MANcsv;

//ファイル読み込み
	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,区切りのデータをtmpに抽出	
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


	//入力された文字列を取得？
//	m_kishuCombo.GetWindowTextW(str);

	e_MainVolume = "100";//主剤量の初期値を設定

	//変数からEditへ
	UpdateData(FALSE);


	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CJyushihaigou::Start()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	//Editから変数へ
	UpdateData(TRUE);//LotNo,波長ランクを変数へ格納
	
	m_kishuCombo.GetWindowTextW(Cb_J_Kishu);//機種名を変数に格納
	Cb_C_J_Sagyousha.GetWindowTextW(Cb_V_J_Sagyousha);//作業者を変数に格納

	UpdateData(FALSE);

	if(pWnd->dlflag_measure == FALSE){
	if(LotNo != _T("") && W_rank_V != _T("") && Cb_J_Kishu != _T("") && e_MainVolume != _T("") && Cb_V_J_Sagyousha != _T("")){
		pWnd->dlflag_measure = TRUE;//起動中にする

		msr.Edit_Measure_Kishu = Cb_J_Kishu;
		msr.msr_volume = e_MainVolume;
		msr.msr_Sagyousha = Cb_V_J_Sagyousha;//いらない？
		msr.Edit_V_msr_Sagyousha = Cb_V_J_Sagyousha;//エディットBOXに入れるためだけの変数
		msr.Edit_Measure_WRank = W_rank_V;//エディットBOXに入れるためだけの変数
		//msr.DoModal();
		msr.Create(IDD_measure,this);
		msr.ShowWindow(SW_SHOWNA);//表示
	}
	else AfxMessageBox(_T("未入力箇所があります."));
	}
	else{}
}

void CJyushihaigou::OnEnChangeEdit2()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
}

void CJyushihaigou::OnBnClickedJEnd()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。

	OnClose();
}

void CJyushihaigou::OnCbnEditchangeCombo1()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
name_cnt++;

	//Editから変数へ
	UpdateData(TRUE);

	if(name_cnt == 25){
		//Editから変数へ
		UpdateData(TRUE);

		// TABキー
		keybd_event(VK_TAB, 0, 0, 0 );
        // TABキーの解放する。
        keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);

		name_cnt = 0;
	}
	//スペースが含まれていたら
	if(Cb_J_Kishu.Find(_T(" ")) != -1){
		// BackSpaceキー
		keybd_event(VK_BACK, 0, 0, 0 );
		// BackSpaceキーの解放する。
		keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);

		//Editから変数へ
		UpdateData(TRUE);

		// TABキー
		keybd_event(VK_TAB, 0, 0, 0 );
        // TABキーの解放する。
        keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
		}
	else{
      //Editから変数へ
	  //UpdateData(TRUE);
	}
}

void CJyushihaigou::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
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
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
}
