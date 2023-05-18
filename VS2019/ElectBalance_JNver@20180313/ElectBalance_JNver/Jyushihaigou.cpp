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
	, Cb_V_J_WRank(_T(""))
	, Cb_J_Setubi(_T(""))
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
	DDX_CBString(pDX, IDC_COMBO3, Cb_V_J_WRank);
	DDX_Control(pDX, IDC_COMBO3, Cb_C_J_WRank);
	DDX_Control(pDX, IDC_COMBO4, m_setubiCombo);
	DDX_CBString(pDX, IDC_COMBO4, Cb_J_Setubi);
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
	ON_CBN_DROPDOWN(IDC_COMBO_WRank, &CJyushihaigou::OnCbnDropdownComboWrank)
//	ON_CBN_DROPDOWN(IDC_COMBO1, &CJyushihaigou::OnCbnDropdownCombo1)
//ON_CBN_SELCHANGE(IDC_COMBO1, &CJyushihaigou::OnCbnSelchangeCombo1)
END_MESSAGE_MAP()


// CJyushihaigou メッセージ ハンドラ

void CJyushihaigou::OnEnChangeEdit1()
{
	//LotNoの入力エディットボックス
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
	
	//コメントアウト20151116
	//UpdateData(TRUE);

	//スペースが含まれていたら
	//コメントアウト20151116
	/*
	if(LotNo.Find(_T(" ")) != -1 || LotNo.Find(_T("　")) != -1){
      keybd_event(VK_BACK, 0, 0, 0 );
      // TABキーの解放する。
      keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);
	  Tabcnt = 0;
	}
	else{
	  //Editから変数へ
	  UpdateData(TRUE);
	  Tabcnt = 0;
	}
	*/

}

BOOL CJyushihaigou::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	name_cnt = 0;
	Tabcnt = 0;
	WRankCnt = 0;
	//登録機種リストの読み込み
	CString str = pWnd->Jyushidatacsv;
	CString Already[SIZE];

	/*Cb_C_J_WRank.InsertString(-1, _T("D1"));
	Cb_C_J_WRank.InsertString(-1, _T("D2"));
	Cb_C_J_WRank.InsertString(-1, _T("D3"));
	Cb_C_J_WRank.InsertString(-1, _T("D4"));
	Cb_C_J_WRank.InsertString(-1, _T("なし"));*/

	m_setubiCombo.InsertString(-1, _T("TDK"));
	m_setubiCombo.InsertString(-1, _T("MUSASHI"));
	m_setubiCombo.InsertString(-1, _T("ショットミニ"));
	m_setubiCombo.InsertString(-1, _T("quspa"));
	m_setubiCombo.InsertString(-1, _T("シート接着剤"));
	m_setubiCombo.InsertString(-1, _T("反射枠樹脂"));
	m_setubiCombo.InsertString(-1, _T("封止樹脂"));
	m_setubiCombo.InsertString(-1, _T("H.D後枠樹脂"));


	m_setubiCombo.InsertString(-1, _T("指定なし"));


	int i,j=0,kishuno = 0,flag = 0;


	//初期化
	for(i=0;i<SIZE;i++){
		Already[i] = _T("");
	}

	//コンボボックスリストに重複する機種名を追加しない
	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
		flag = 0;
		
		for(j=0;j<kishuno;j++){
			if(Already[j] == pWnd->Jyushi_List[i][0]){
				flag = 1;
				break;
			}
		}
		Already[kishuno] = pWnd->Jyushi_List[i][0];
		kishuno++;

		if(flag == 0)
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
	//20180409 100g⇒15g
	e_MainVolume = "15";//主剤量の初期値を設定
	Cb_J_Setubi = _T("指定なし");//設備の指定はデフォルトで指定なし
	

	//変数からEditへ
	UpdateData(FALSE);


	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CJyushihaigou::Start()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	int i,bothsame = 0;//,a,b;

	//機種名にスペースが入っていたら削除
	/*if(Cb_J_Kishu.Find(" ") != -1){
	}*/

	//Cb_J_Kishu.SpanExcluding(_T("D"));
	//変数からEditへ
	//UpdateData(FALSE);//LotNo,波長ランクを変数へ格納

	//Editから変数へ
	UpdateData(TRUE);//LotNo,波長ランクを変数へ格納
	
	//20151116機種名、ロットNo同一化
	CString Kishu_del,Lot_del;
	//20180417車載用にロットNoを分けて対応
	Kishu_del = Cb_J_Kishu;
	//Kishu_del = Cb_J_Kishu.Left(25);
	//Kishu_del = Kishu_del.TrimRight();
	//20180417車載用にロットNoを分けて対応
	//Lot_del = Cb_J_Kishu.Right(Cb_J_Kishu.GetLength()-25);

	Cb_J_Kishu = Kishu_del;//機種
	//LotNo = Lot_del;//ロット
	//m_kishuCombo.GetWindowTextW(Cb_J_Kishu);//機種名を変数に格納
	Cb_C_J_WRank.GetWindowTextW(Cb_V_J_WRank);//波長ランクを変数に格納
	m_setubiCombo.GetWindowTextW(Cb_J_Setubi);//設備の種類を変数に格納
	Cb_C_J_Sagyousha.GetWindowTextW(Cb_V_J_Sagyousha);//作業者を変数に格納

	/*
	//追加 Left
	b = Cb_J_Kishu.Find(_T("　"));//全角スペースを削除
	if(b !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(b);

	a = Cb_J_Kishu.Find(_T(" "));//半角スペースを削除
	if(a !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(a);

		*/
	//変数からEditへ
	UpdateData(FALSE);



	//該当機種、波長ランクがあるかないかを確認する
	//一致しているものを探す
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Cb_J_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(Cb_V_J_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(Cb_J_Setubi) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}
	
	if(bothsame == 1){

	if(pWnd->dlflag_measure == FALSE){
	if(LotNo != _T("") && Cb_V_J_WRank != _T("") && Cb_J_Kishu != _T("") && e_MainVolume != _T("") && Cb_V_J_Sagyousha != _T("") && Cb_J_Setubi != _T("")){
		pWnd->dlflag_measure = TRUE;//起動中にする

		msr.Edit_Measure_Kishu = Cb_J_Kishu;
		msr.Edit_Measure_LotNo = LotNo;
		msr.msr_volume = e_MainVolume;
		msr.msr_Sagyousha = Cb_V_J_Sagyousha;//いらない？
		msr.Edit_V_msr_Sagyousha = Cb_V_J_Sagyousha;//エディットBOXに入れるためだけの変数
		msr.Edit_Measure_WRank = Cb_V_J_WRank;//エディットBOXに入れるためだけの変数

		msr.Edit_Measure_Setubi = Cb_J_Setubi;//エディットBOXに入れるためだけの変数
		//msr.DoModal();
		msr.Create(IDD_measure,this);
		msr.ShowWindow(SW_SHOWNA);//表示
	}
	else AfxMessageBox(_T("未入力箇所があります."));
	}
	else{}
	}
	else{
		AfxMessageBox(_T("該当データがありません.機種名、波長ランク、設備の種類を確認して下さい."));
	}
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

void CJyushihaigou::OnCbnEditchangeCombo1()//機種名変更したら
{
	// 機種名のコンボボックス
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	//コメントアウト20151116
	/*
name_cnt++;

	//Editから変数へ
	UpdateData(TRUE);

	if(name_cnt >= 25){
		//Editから変数へ
		UpdateData(TRUE);

		if(Tabcnt == 0){
		// TABキー
		keybd_event(VK_TAB, 0, 0, 0 );
        // TABキーの解放する。
        keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
		Tabcnt = 1;
		}
		name_cnt = 0;


	}

	if(Cb_J_Kishu.Find(_T(" ")) != -1 || Cb_J_Kishu.Find(_T("　")) != -1){
	//if(AlNum() == -1){
		if(Tabcnt == 0){
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
		Tabcnt = 1;
		}
		}

	else{
      //Editから変数へ
	  UpdateData(TRUE);
	}
	*/
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

int CJyushihaigou::AlNum(void)
{
	int Num=-1;
	if(Cb_J_Kishu.Find(_T("A")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("B")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("C")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("D")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("E")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("F")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("G")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("H")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("I")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("J")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("K")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("L")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("M")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("N")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("O")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("P")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("Q")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("R")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("S")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("T")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("U")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("V")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("W")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("X")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("Y")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("Z")) == -1)Num=0;

	if(Cb_J_Kishu.Find(_T("a")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("b")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("c")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("d")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("e")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("f")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("g")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("h")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("i")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("j")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("k")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("l")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("m")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("n")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("o")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("p")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("q")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("r")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("s")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("t")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("u")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("v")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("w")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("x")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("y")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("z")) == -1)Num=0;

	if(Cb_J_Kishu.Find(_T("1")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("2")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("3")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("4")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("5")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("6")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("7")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("8")) == -1)Num=0;
	if(Cb_J_Kishu.Find(_T("9")) == -1)Num=0;



	return Num;
}

void CJyushihaigou::OnCbnDropdownComboWrank()//波長ランク
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int i;//,a,b;

	//Editから変数へ
	UpdateData(TRUE);

	CString Kishu_del;
	Kishu_del = Cb_J_Kishu.Left(25);
	Kishu_del = Kishu_del.TrimRight();

	//追加 Left
	/*
	b = Cb_J_Kishu.Find(_T("　"));//全角スペースを削除
	if(b !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(b);

	a = Cb_J_Kishu.Find(_T(" "));//半角スペースを削除
	if(a !=-1)
		Cb_J_Kishu = Cb_J_Kishu.Left(a);
*/
	UpdateData(FALSE);

	//前選択機種の波長ランクを削除する
	Cb_C_J_WRank.ResetContent();

	//Cb_C_J_WRank.DeleteString(0);
				
	//一致しているものを探す
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Kishu_del) == 0 && pWnd->Jyushi_List[i][0] != _T("")){
				Cb_C_J_WRank.InsertString(-1, pWnd->Jyushi_List[i][1]);
				WRankCnt++;
				}
		else if(pWnd->Jyushi_List[i][0] == _T(""))break;
	}

	//入力された機種名の波長ランクのみ表示させる
	//Cb_C_J_WRank.InsertString(-1, _T("test"));
}
