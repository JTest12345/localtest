// Mnt_Tanto.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Mnt_Tanto.h"

#include "ElectBalance_JNverDlg.h"

// CMnt_Tanto ダイアログ

IMPLEMENT_DYNAMIC(CMnt_Tanto, CDialog)

CMnt_Tanto::CMnt_Tanto(CWnd* pParent /*=NULL*/)
	: CDialog(CMnt_Tanto::IDD, pParent)
	, Cb_Mnt_T_V(_T(""))
{
	int i;
	//初期化
	for(i=0;i<100;i++)
		AllName[i]=_T("");
	AllNameCnt=0;


}

CMnt_Tanto::~CMnt_Tanto()
{
}

void CMnt_Tanto::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_Cb_Mnt_T, Cb_Mnt_T);
	DDX_CBString(pDX, IDC_Cb_Mnt_T, Cb_Mnt_T_V);
}


BEGIN_MESSAGE_MAP(CMnt_Tanto, CDialog)
	ON_BN_CLICKED(IDC_Bn_Mnt_T_Toroku, &CMnt_Tanto::OnBnClickedBnMntTToroku)
	ON_BN_CLICKED(IDC_Bn_Mnt_T_Sakujo, &CMnt_Tanto::OnBnClickedBnMntTSakujo)
	ON_BN_CLICKED(IDC_End, &CMnt_Tanto::OnBnClickedEnd)
	ON_CBN_SELCHANGE(IDC_Cb_Mnt_T, &CMnt_Tanto::OnCbnSelchangeCbMntT)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CMnt_Tanto メッセージ ハンドラ


void CMnt_Tanto::OnBnClickedBnMntTToroku()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
CString str = pWnd->MANcsv;

Cb_Mnt_T.GetWindowTextW(Cb_Mnt_T_V);

//ファイル読み込み
CStdioFile fp;
//ファイルの書き込み
CStdioFile ofp;

CString buf;
CString tmp[16];
int i,flag=0;

for(i=0;i<AllNameCnt;i++){
	if(AllName[i].Compare(Cb_Mnt_T_V) == 0){
		flag = 1;
		break;
	}
}
	if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){

		if(Cb_Mnt_T_V == "")
			AfxMessageBox(_T("未入力です."));
		else if(flag == 1){
			AfxMessageBox(_T("すでに登録済みです."));
		}
		//else if(MessageBox(_T("登録してよろしいですか？"),NULL,MB_OKCANCEL) == IDOK){
		else{
				ofp.SeekToEnd();
				
				ofp.WriteString(Cb_Mnt_T_V);
				ofp.WriteString(_T(","));
				ofp.WriteString(_T("\n"));
				Cb_Mnt_T.InsertString(-1, Cb_Mnt_T_V);//追加した項目をリストに追加

				AllName[i] = Cb_Mnt_T_V;//リストへ追加する
				AllNameCnt++;

				AfxMessageBox(_T("登録が完了しました."));					
		}
	}
	ofp.Close();
}

void CMnt_Tanto::OnBnClickedBnMntTSakujo()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
//CString str = _T("C:\\EB\\mst\\MAN.csv");
	CString str = pWnd->MANcsv;
CString newfile = _T("C:\\EB\\mst\\temp.csv");

Cb_Mnt_T.GetWindowTextW(Cb_Mnt_T_V);//機種名

//ファイル読み込み
CStdioFile fp;
//ファイルの書き込み
CStdioFile ofp;

CString buf;
CString tmp[4];
int a,i,j=0;
int NameCnt=0,cnt=0;


//ファイルの生成
HANDLE hFile;

	hFile = CreateFile(newfile, 
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


	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		if(ofp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,区切りのデータをtmpに抽出	
			for(i=0; i<4; i++) tmp[i] = _T("");
			for(i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					if(Cb_Mnt_T_V.Compare(tmp[i]) == 0){
					//削除したい項目の番号を取得してDeleteString(i)でコンボボックスのプルダウンを更新したい
					Cb_Mnt_T.DeleteString(cnt);
					for(i=cnt;i<AllNameCnt;i++){
						
						AllName[i] = AllName[i+1];//該当箇所を初期化する
					
					}
					AllNameCnt--;
					}

					else if(Cb_Mnt_T_V.Compare(tmp[i]) != 0){ //|| NameCnt == 1){//一回のみ書き込みを飛ばす

					ofp.SeekToEnd();
					ofp.WriteString(tmp[i]);
					ofp.WriteString(_T(","));
					ofp.WriteString(_T("\n"));
					cnt++;
					}
					//一回のみ書き込みを飛ばす
					//if(Cb_Mnt_T_V.Compare(tmp[i]) == 0)NameCnt = 1;

					//cnt++;
					break;

				}
				buf.Delete(0, a+1);
			}
		}
	}
	}
		ofp.Close();
		fp.Close();

		DeleteFile(str);
		MoveFile(newfile,str);
		//CFile::Rename(newfile,str);
		//DeleteFile(str);

		AfxMessageBox(_T("削除が完了しました."));
	
}

BOOL CMnt_Tanto::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
CString str = _T("C:\\EB\\mst\\MAN.csv");

//ファイル読み込み
CStdioFile fp;

CString buf;
CString tmp[4];
int a,i;

	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,区切りのデータをtmpに抽出	
			for(i=0; i<4; i++) tmp[i] = _T("");
			for(i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					Cb_Mnt_T.InsertString(-1, tmp[i]);//リストへ追加		
					AllName[AllNameCnt] = tmp[i];//リストに担当者名を格納
					AllNameCnt++;
				}
					break;
				}
				buf.Delete(0, a+1);
			}
	}
		fp.Close();

	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CMnt_Tanto::OnBnClickedEnd()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	OnClose();
}

void CMnt_Tanto::OnCbnSelchangeCbMntT()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
}

void CMnt_Tanto::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_Mnt_Tanto = FALSE;
	DestroyWindow();

	CDialog::OnClose();
}
