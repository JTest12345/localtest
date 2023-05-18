// ElectBalance_JNverDlg.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "ElectBalance_JNverDlg.h"
#include "DlgProxy.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// アプリケーションのバージョン情報に使われる CAboutDlg ダイアログ

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// ダイアログ データ
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

// 実装
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CElectBalance_JNverDlg ダイアログ




IMPLEMENT_DYNAMIC(CElectBalance_JNverDlg, CDialog);

CElectBalance_JNverDlg::CElectBalance_JNverDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CElectBalance_JNverDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_pAutoProxy = NULL;
}

CElectBalance_JNverDlg::~CElectBalance_JNverDlg()
{
	// このダイアログ用のオートメーション プロキシがある場合は、このダイアログ
	//  へのポインタを NULL に戻します、それによってダイアログが削除されたこと
	//  がわかります。
	if (m_pAutoProxy != NULL)
		m_pAutoProxy->m_pDialog = NULL;
}

void CElectBalance_JNverDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CElectBalance_JNverDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_CLOSE()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_BUTTON2, &CElectBalance_JNverDlg::OnBnClickedJyushi)
	ON_BN_CLICKED(IDC_BUTTON1, &CElectBalance_JNverDlg::OnBnClickedMaintenance)
	ON_BN_CLICKED(IDC_End, &CElectBalance_JNverDlg::OnBnClickedEnd)
END_MESSAGE_MAP()


// CElectBalance_JNverDlg メッセージ ハンドラ

BOOL CElectBalance_JNverDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// "バージョン情報..." メニューをシステム メニューに追加します。

	// IDM_ABOUTBOX は、システム コマンドの範囲内になければなりません。
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// このダイアログのアイコンを設定します。アプリケーションのメイン ウィンドウがダイアログでない場合、
	//  Framework は、この設定を自動的に行います。
	SetIcon(m_hIcon, TRUE);			// 大きいアイコンの設定
	SetIcon(m_hIcon, FALSE);		// 小さいアイコンの設定


	// TODO: 初期化をここに追加します。
	if( CreateDirectory(_T("C:\\EB"), NULL )){}

	if( CreateDirectory(_T("C:\\EB\\data"), NULL )){}

	if( CreateDirectory(_T("C:\\EB\\exe"), NULL )){}

	if( CreateDirectory(_T("C:\\EB\\mst"), NULL )){}

	int ini_check = 0;
	int str_check = 0;
	int hst_check = 0;
	int cnt = 0;
	CFileFind find;
	CString ini = _T("C:\\EB\\mst\\EB.ini");
	CStdioFile fp;

	CString buf;
	CString tmp[25];
	CString str = _T("C:\\EB\\mst\\Jyushidata.csv");
	CString hst = _T("C:\\EB\\mst\\History.csv");
	HANDLE hFile;

	if (find.FindFile(str)){//jyushidataがすでに存在するか調べる
	str_check = 1;
	}

	if (find.FindFile(hst)){//.iniがすでに存在するか調べる
	hst_check = 1;
	}

	if (find.FindFile(ini)){//.iniがすでに存在するか調べる
	ini_check = 1;
	}



	if(ini_check == 0){
		hFile = CreateFile(_T("C:\\EB\\mst\\EB.ini"), 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile関数が失敗しました"));

	//ハンドルを閉じる
	CloseHandle(hFile);

	//初期生成の場合、EB.iniファイルの中身を更新
	
	if(fp.Open(ini, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(_T("C:\\EB\\mst\\Jyushidata.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("C:\\EB\\mst\\MAN.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("C:\\EB\\data\\Result.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("C:\\EB\\mst\\History.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("100"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("0.5"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("1"));
				fp.WriteString(_T("\n\n\n\n\n\n"));
				fp.WriteString(_T("①機種、波長ごとのデータファイルのパス"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("②作業者名のデータファイルのパス"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("③結果出力ファイルのパス"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("④樹脂の理論値修正履歴ファイルのパス"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("⑤計量時の更新頻度(ms)"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("⑥誤差範囲(g)"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("⑦COMポートのナンバー"));
		fp.Close();
		}
	}

	//.iniファイルの中身を変数に格納
if(fp.Open(ini,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			if(cnt == 0)
				Jyushidatacsv = buf;
			else if(cnt == 1)
				MANcsv = buf;
			else if(cnt == 2)
				Resultcsv = buf;
			else if(cnt == 3)
				Historycsv = buf;
			else if(cnt == 4)
				Timer_ms = buf;
			else if(cnt == 5)
				gosa = buf;
			else if(cnt == 6)
				COMPORT = buf;
			else if(cnt >= 7)
				break;
		cnt++;
		}	
}									
	fp.Close();

str = Jyushidatacsv;

	//ファイルの生成

	hFile = CreateFile(MANcsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile関数が失敗しました"));

	//ハンドルを閉じる
	CloseHandle(hFile);

hFile = CreateFile(Jyushidatacsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile関数が失敗しました"));

	//ハンドルを閉じる
	CloseHandle(hFile);

hFile = CreateFile(Resultcsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile関数が失敗しました"));

	//ハンドルを閉じる
	CloseHandle(hFile);

hFile = CreateFile(Historycsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile関数が失敗しました"));

	//ハンドルを閉じる
	CloseHandle(hFile);



	//ダイアログのフラグを初期化
	dlflag_Jyushihaigou = FALSE;
	dlflag_Maintenance = FALSE;
	dlflag_measure = FALSE;
	dlflag_JyushiCode = FALSE;
	dlflag_password = FALSE;
	dlflag_renum = FALSE;
	dlflag_Mnt_Kishu = FALSE;
	dlflag_Mnt_Tanto = FALSE;
	dlflag_MntPassword = FALSE;

	int i,j,a;
	//樹脂リストを初期化
	for(i=0;i<SIZE;i++)
		for(j=0;j<25;j++)
			Jyushi_List[i][j] = _T("");

	//樹脂リストを配列に格納



i=0;
if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,区切りのデータをtmpに抽出
			for(j=0; j<25; j++) tmp[j] = _T("");
			i++;
			for(j=0; j<25; j++){
				a = buf.Find(_T(","));
				if(buf.Compare(_T("endpoint")) == 0)
					break;
				if(a!=-1){
					tmp[j] = buf.Left(a);

					if(tmp[j].Compare(_T("endpoint")) == 0){
						buf.Delete(0, a+1);
						break;
					}
				
					Jyushi_List[i-1][j]=tmp[j];//使用樹脂名を格納
					}
					buf.Delete(0, a+1);
					
					}

		}
}									

		fp.Close();

		if(str_check == 0){
		if(fp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(_T("機種名,"));
				fp.WriteString(_T("波長ランク,"));
				fp.WriteString(_T("樹脂名①,"));
				fp.WriteString(_T("樹脂型番①,"));
				fp.WriteString(_T("樹脂量①,"));
				fp.WriteString(_T("樹脂名②,"));
				fp.WriteString(_T("樹脂型番②,"));
				fp.WriteString(_T("樹脂量②,"));
				fp.WriteString(_T("樹脂名③,"));
				fp.WriteString(_T("樹脂型番③,"));
				fp.WriteString(_T("樹脂量③,"));
				fp.WriteString(_T("樹脂名④,"));
				fp.WriteString(_T("樹脂型番④,"));
				fp.WriteString(_T("樹脂量④,"));
				fp.WriteString(_T("樹脂名⑤,"));
				fp.WriteString(_T("樹脂型番⑤,"));
				fp.WriteString(_T("樹脂量⑤,"));
				fp.WriteString(_T("endpoint,"));
				fp.WriteString(_T("\n"));
				
			fp.Close();
			}
		}

		if(hst_check == 0){
		if(fp.Open(hst, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(_T("日付,"));
				fp.WriteString(_T("機種名,"));
				fp.WriteString(_T("波長ランク,"));
				fp.WriteString(_T("樹脂名,"));
				fp.WriteString(_T("樹脂型番,"));
				fp.WriteString(_T("変更前樹脂量,"));
				fp.WriteString(_T("変更後樹脂量,"));
				fp.WriteString(_T("\n"));
				
			fp.Close();
			}
		}

		/*AfxMessageBox(Jyushidatacsv);
		AfxMessageBox(MANcsv);
		AfxMessageBox(Resultcsv);
		AfxMessageBox(Timer_ms);
		AfxMessageBox(gosa);*/

	return TRUE;  // フォーカスをコントロールに設定した場合を除き、TRUE を返します。
}

void CElectBalance_JNverDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// ダイアログに最小化ボタンを追加する場合、アイコンを描画するための
//  下のコードが必要です。ドキュメント/ビュー モデルを使う MFC アプリケーションの場合、
//  これは、Framework によって自動的に設定されます。

void CElectBalance_JNverDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // 描画のデバイス コンテキスト

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// クライアントの四角形領域内の中央
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// アイコンの描画
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// ユーザーが最小化したウィンドウをドラッグしているときに表示するカーソルを取得するために、
//  システムがこの関数を呼び出します。
HCURSOR CElectBalance_JNverDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

// コントローラがオブジェクトの 1 つをまだ保持している場合、
//  オートメーションサーバーはユーザーが UI を閉じる際に終了で
//  きません。これらのメッセージ ハンドラはプロキシがまだ使用中
//  かどうかを確認し、それから UI が非表示になりますがダイアロ
//  グはそれが消された場合その場所に残ります。

void CElectBalance_JNverDlg::OnClose()
{
	if (CanExit())
		CDialog::OnClose();
}

void CElectBalance_JNverDlg::OnOK()
{
	if (CanExit())
		CDialog::OnOK();
}

void CElectBalance_JNverDlg::OnCancel()
{
	if (CanExit())
		CDialog::OnCancel();
}

BOOL CElectBalance_JNverDlg::CanExit()
{
	// プロキシ オブジェクトがまだ残っている場合、オートメーション
	//  コントローラはこのアプリケーションをまだ保持しています。
	//  ダイアログの周囲は残しますが UI は非表示になります。
	if (m_pAutoProxy != NULL)
	{
		ShowWindow(SW_HIDE);
		return FALSE;
	}

	return TRUE;
}


void CElectBalance_JNverDlg::OnBnClickedJyushi()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。

	//m_flg_rankbin_search = TRUE;//Destroy時にfalseに戻す
	//		dlg_rankbin_search.Create(IDD_DIALOG_GRAPHXBARR,GetDesktopWindow());
			//Jshg.Create(IDD_Jyushihaigou,this);
	//Jshg.DoModal();
	if(dlflag_Jyushihaigou == FALSE){//起動していなければ
		dlflag_Jyushihaigou = TRUE;
		Jshg.Create(IDD_Jushihaigou,this);
		Jshg.ShowWindow(SW_SHOWNA);//表示
	}
	else{}//起動中の場合

}

void CElectBalance_JNverDlg::OnBnClickedMaintenance()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	if(pWnd->dlflag_Maintenance == FALSE){//起動していなければ
	MntPass.DoModal();

	if(pWnd->MntPass.passok == 1){
		//pWnd->dlflag_MntPassword = FALSE;//ダイアログを閉じるのでFALSE

		//if(pWnd->dlflag_Maintenance == FALSE){//起動していなければ
		pWnd->dlflag_Maintenance = TRUE;
		pWnd->Mnt.Create(IDD_Maintenance,this);
		pWnd->Mnt.ShowWindow(SW_SHOWNA);//表示


		}
	}
	//if(dlflag_MntPassword == FALSE){//起動していなければ
	//dlflag_MntPassword = TRUE;	
	//MntPass.Create(IDD_Mnt_PASSWORD,this);
	//MntPass.ShowWindow(SW_SHOWNA);//表示	
	//}

	//else{}//起動中の場合

}

void CElectBalance_JNverDlg::OnBnClickedEnd()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	OnCancel();
}

HBRUSH CElectBalance_JNverDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
 
  // TODO: デフォルトのブラシが望みのものでない場合には、違うブラシを返してください
  HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
  

  //TRACE(&quot;OnControlColor\n&quot;);
  switch( nCtlColor )
  {
  case CTLCOLOR_DLG : // ダイアログの背景色
  case CTLCOLOR_STATIC:
    //pDC-&gt;SetBkColor( RGB(100,152,193) );
    return m_BkBrush;
  case CTLCOLOR_BTN :
  case CTLCOLOR_EDIT:   
    return m_BtmBrush;
  default :
    return CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
  }
  
  return hbr;
}
