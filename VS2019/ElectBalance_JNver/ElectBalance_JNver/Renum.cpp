// Renum.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Renum.h"

#include "ElectBalance_JNverDlg.h"

// CRenum ダイアログ

IMPLEMENT_DYNAMIC(CRenum, CDialog)

CRenum::CRenum(CWnd* pParent /*=NULL*/)
	: CDialog(CRenum::IDD, pParent)
	, Edit_V_rn(_T(""))
{

}

CRenum::~CRenum()
{
}

void CRenum::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT1, Edit_V_rn);
}


BEGIN_MESSAGE_MAP(CRenum, CDialog)
	ON_EN_CHANGE(IDC_EDIT1, &CRenum::OnEnChangeEdit1)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_BUTTON_EXE, &CRenum::OnBnClickedButtonExe)
END_MESSAGE_MAP()


// CRenum メッセージ ハンドラ

void CRenum::OnEnChangeEdit1()
{
	// TODO:  これが RICHEDIT コントロールの場合、
	// まず、CDialog::OnInitDialog() 関数をオーバーライドして、OR 状態の ENM_CHANGE
	// フラグをマスクに入れて、CRichEditCtrl().SetEventMask() を呼び出さない限り、
	// コントロールは、この通知を送信しません。

	// TODO:  ここにコントロール通知ハンドラ コードを追加してください。
	
}

void CRenum::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	/*CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_renum = FALSE;
	DestroyWindow();

	pWnd->PW.OnClose();*/

	CDialog::OnClose();
}

void CRenum::OnBnClickedButtonExe()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int Num = pWnd->Jshg.msr.CheckBoxNum,i,bothsame = 0;
	CString Oldg,Newg,JName,JKata;//History作成
	double ReNum;

	//Editから変数へ
	UpdateData(TRUE);


	if(pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][2] == Edit_V_rn){
		AfxMessageBox(_T("修正前と同様の値です."));
	}

	else{
	//一致しているものを探す
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(pWnd->Jshg.Cb_J_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(pWnd->Jshg.W_rank_V) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			break;
		}
	}
 
	ReNum = _tcstod(Edit_V_rn,NULL);//doubleへ型変換;
	ReNum = ReNum / pWnd->Jshg.msr.Num;

	Oldg = pWnd->Jyushi_List[i][(3*Num)+4];//修正前の値
	Newg.Format(_T("%g"), ReNum);//修正後の値
	JName = pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][0];
	JKata = pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][1];


	pWnd->Jyushi_List[i][(3*Num)+4] = Newg;
	//OnClose();
	
	//変える必要があるか？
	//pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][2] = Newg;

	pWnd->RN.EndDialog(1);
		

	//pWnd->msr.OnClose();
	//pWnd->Jshg.Start();
	
		//pWnd->dlflag_measure = TRUE;//起動中にする
		//pWnd->msr.Create(IDD_measure,this);
		//pWnd->msr.ShowWindow(SW_SHOWNA);//表示	

	//pWnd->msr.m_listc.SetItemText(1, 1, _T("aaa"));//4列目へ追加する
	//pWnd->msr.m_listc.SetItemText(Num, 2, Edit_V_rn);//3列目へ追加する
	//pWnd->msr.m_listc.DeleteAllItems();
	/*pWnd->msr.m_listc.RemoveAllGroups();
	pWnd->msr.m_listc;*/

	//ファイルの中身も書き換える

	//ファイル読み込み
CStdioFile fp;
//ファイルの書き込み
CStdioFile ofp;

CString buf;
CString prebuf;
CString tmp[25];
int a,next;
int cnt=0;

CString str = pWnd->Jyushidatacsv;
CString newfile = _T("C:\\EB\\mst\\temp.csv");


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

	next = i;

	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		if(ofp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){

			if(bothsame == 1 && cnt != next){
			
			//,区切りのデータをtmpに抽出
			for(int i=0; i<25; i++) tmp[i] = _T("");

			for(int i=0; i<25; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

						ofp.SeekToEnd();
						ofp.WriteString(buf);//一行全て書き込む
						ofp.WriteString(_T("\n"));//最後に改行を入れる
						break;
					}
			}//buf中の,を見つけるif文
				buf.Delete(0, a+1);
			
			}

			else if(bothsame == 1 && cnt == next){//修正箇所の行は別処理
				//,区切りのデータをtmpに抽出
			for(int i=0; i<25; i++) tmp[i] = _T("");

			for(int i=0; i<25; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					//if(Cb_V_Mnt_K_KishuName.Compare(tmp[i]) == 0){
					////削除したい項目の番号を取得してDeleteString(i)でコンボボックスのプルダウンを更新したい
					//Cb_C_Mnt_K_KishuName.DeleteString(cnt);
					//}
					if(tmp[i].Compare(_T("endpoint")) == 0){
							ofp.WriteString(_T("endpoint,"));//最後に改行を入れる
							ofp.WriteString(_T("\n"));//最後に改行を入れる
							break;
					}
						ofp.SeekToEnd();
						if(i == ((3*Num)+4)){
							ofp.WriteString(Newg);//修正後の数値を書き込む
							ofp.WriteString(_T(","));//途中の場合、カンマ
						}
						else{
							ofp.WriteString(tmp[i]);//部分的に書き込む
							ofp.WriteString(_T(","));//途中の場合、カンマ
						}
							
				}					
			//buf中の,を見つけるif文
				buf.Delete(0, a+1);
				}
			}		
				cnt++;
		}//削除対象の機種があるかどうか
		}
	}
		ofp.Close();
		fp.Close();

		if(bothsame == 1){
			DeleteFile(str);
			MoveFile(newfile,str);
			AfxMessageBox(_T("修正が完了しました"));
		}

//History更新
str = pWnd->Historycsv;
CTime cTime = CTime::GetCurrentTime();           // 現在時刻
CString date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"形式の時刻文字列を取得

if(fp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(date);
				fp.WriteString(_T(","));
				fp.WriteString(pWnd->Jshg.Cb_J_Kishu);
				fp.WriteString(_T(","));
				fp.WriteString(pWnd->Jshg.W_rank_V);
				fp.WriteString(_T(","));
				fp.WriteString(JName);
				fp.WriteString(_T(","));
				fp.WriteString(JKata);
				fp.WriteString(_T(","));
				fp.WriteString(Oldg);
				fp.WriteString(_T(","));
				fp.WriteString(Newg);
				fp.WriteString(_T(","));
				fp.WriteString(_T("\n"));

	fp.Close();
}
}

}
