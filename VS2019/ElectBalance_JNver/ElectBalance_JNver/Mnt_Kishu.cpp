// Mnt_Kishu.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Mnt_Kishu.h"

#include "ElectBalance_JNverDlg.h"

// CMnt_Kishu ダイアログ

IMPLEMENT_DYNAMIC(CMnt_Kishu, CDialog)

CMnt_Kishu::CMnt_Kishu(CWnd* pParent /*=NULL*/)
	: CDialog(CMnt_Kishu::IDD, pParent)
	, Cb_V_Mnt_K_KishuName(_T(""))
	, Cb_V_Mnt_K_SZ_Name(_T(""))
	, Cb_V_Mnt_K_SZ_Kata(_T(""))
	, Cb_V_Mnt_K_Other1_Name(_T(""))
	, Cb_V_Mnt_K_Other2_Name(_T(""))
	, Cb_V_Mnt_K_Other3_Name(_T(""))
	, Cb_V_Mnt_K_Other4_Name(_T(""))
	, Cb_V_Mnt_K_Other5_Name(_T(""))
	, Cb_V_Mnt_K_Other1_Kata(_T(""))
	, Cb_V_Mnt_K_Other2_Kata(_T(""))
	, Cb_V_Mnt_K_Other3_Kata(_T(""))
	, Cb_V_Mnt_K_Other4_Kata(_T(""))
	, Cb_V_Mnt_K_Other5_Kata(_T(""))
	, EDIT_K_Other1(_T(""))
	, EDIT_K_Other2(_T(""))
	, EDIT_K_Other3(_T(""))
	, EDIT_K_Other4(_T(""))
	, EDIT_K_Other5(_T(""))
	, EDIT_K_WRank_V(_T(""))
{
	
	int i;
	//初期化
	for(i=0;i<5000;i++)
		AllName[i]=_T("");
	AllNameCnt=0;


}

CMnt_Kishu::~CMnt_Kishu()
{
}

void CMnt_Kishu::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_Cb_Mnt_K_KishuName, Cb_C_Mnt_K_KishuName);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_KishuName, Cb_V_Mnt_K_KishuName);
	DDX_Control(pDX, IDC_Cb_Mnt_K_SZ_Name, Cb_C_Mnt_K_SZ_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_SZ_Name, Cb_V_Mnt_K_SZ_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_SZ_Kata, Cb_C_Mnt_K_SZ_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_SZ_Kata, Cb_V_Mnt_K_SZ_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other1_Name, Cb_C_Mnt_K_Other1_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other1_Name, Cb_V_Mnt_K_Other1_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other2_Name, Cb_C_Mnt_K_Other2_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other2_Name, Cb_V_Mnt_K_Other2_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other3_Name, Cb_C_Mnt_K_Other3_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other3_Name, Cb_V_Mnt_K_Other3_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other4_Name, Cb_C_Mnt_K_Other4_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other4_Name, Cb_V_Mnt_K_Other4_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other5_Name, Cb_C_Mnt_K_Other5_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other5_Name, Cb_V_Mnt_K_Other5_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other1_Kata, Cb_C_Mnt_K_Other1_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other1_Kata, Cb_V_Mnt_K_Other1_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other2_Kata, Cb_C_Mnt_K_Other2_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other2_Kata, Cb_V_Mnt_K_Other2_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other3_Kata, Cb_C_Mnt_K_Other3_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other3_Kata, Cb_V_Mnt_K_Other3_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other4_Kata, Cb_C_Mnt_K_Other4_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other4_Kata, Cb_V_Mnt_K_Other4_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other5_Kata, Cb_C_Mnt_K_Other5_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other5_Kata, Cb_V_Mnt_K_Other5_Kata);
	DDX_Text(pDX, IDC_EDIT_K_Other1, EDIT_K_Other1);
	DDX_Text(pDX, IDC_EDIT_K_Other2, EDIT_K_Other2);
	DDX_Text(pDX, IDC_EDIT_K_Other3, EDIT_K_Other3);
	DDX_Text(pDX, IDC_EDIT_K_Other4, EDIT_K_Other4);
	DDX_Text(pDX, IDC_EDIT_K_Other5, EDIT_K_Other5);
	DDX_Text(pDX, IDC_EDIT_K_WRank, EDIT_K_WRank_V);
}


BEGIN_MESSAGE_MAP(CMnt_Kishu, CDialog)
	ON_BN_CLICKED(IDC_Bn_Mnt_K_Toroku, &CMnt_Kishu::OnBnClickedBnMntKToroku)
	ON_BN_CLICKED(IDC_Bn_Mnt_K_Sakujo, &CMnt_Kishu::OnBnClickedBnMntKSakujo)
	ON_BN_CLICKED(IDC_End, &CMnt_Kishu::OnBnClickedEnd)
	ON_CBN_SELCHANGE(IDC_Cb_Mnt_K_KishuName, &CMnt_Kishu::OnCbnSelchangeCbMntKKishuname)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CMnt_Kishu メッセージ ハンドラ

void CMnt_Kishu::OnBnClickedBnMntKToroku()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	CString str = pWnd->Jyushidatacsv;

	int KishuName=0,WRank=0,SZ=0,Other1=0,Other2=0,Other3=0,Other4=0,Other5=0;
	int flag = 0,i,j;
	int bothsame=0;
	

	//Editから変数へ
	UpdateData(TRUE);//波長ランク、剤量

	
	for(i=0;i<AllNameCnt;i++){
	if(AllName[i].Compare(Cb_V_Mnt_K_KishuName) == 0){
		flag = 2;//変えた
		break;
		}
	}


	//コンボボックスに入力されている値をValue変数に格納
	Cb_C_Mnt_K_KishuName.GetWindowTextW(Cb_V_Mnt_K_KishuName);//機種名

	Cb_C_Mnt_K_SZ_Name.GetWindowTextW(Cb_V_Mnt_K_SZ_Name);//主剤の樹脂名
	Cb_C_Mnt_K_SZ_Kata.GetWindowTextW(Cb_V_Mnt_K_SZ_Kata);//主剤の樹脂型番

	Cb_C_Mnt_K_Other1_Name.GetWindowTextW(Cb_V_Mnt_K_Other1_Name);//主剤以外の樹脂名①
	Cb_C_Mnt_K_Other1_Kata.GetWindowTextW(Cb_V_Mnt_K_Other1_Kata);//主剤以外の樹脂型番①

	Cb_C_Mnt_K_Other2_Name.GetWindowTextW(Cb_V_Mnt_K_Other2_Name);//主剤以外の樹脂名②
	Cb_C_Mnt_K_Other2_Kata.GetWindowTextW(Cb_V_Mnt_K_Other2_Kata);//主剤以外の樹脂型番②

	Cb_C_Mnt_K_Other3_Name.GetWindowTextW(Cb_V_Mnt_K_Other3_Name);//主剤以外の樹脂名③
	Cb_C_Mnt_K_Other3_Kata.GetWindowTextW(Cb_V_Mnt_K_Other3_Kata);//主剤以外の樹脂型番③

	Cb_C_Mnt_K_Other4_Name.GetWindowTextW(Cb_V_Mnt_K_Other4_Name);//主剤以外の樹脂名④
	Cb_C_Mnt_K_Other4_Kata.GetWindowTextW(Cb_V_Mnt_K_Other4_Kata);//主剤以外の樹脂型番④

	Cb_C_Mnt_K_Other5_Name.GetWindowTextW(Cb_V_Mnt_K_Other5_Name);//主剤以外の樹脂名⑤
	Cb_C_Mnt_K_Other5_Kata.GetWindowTextW(Cb_V_Mnt_K_Other5_Kata);//主剤以外の樹脂型番⑤

	if(Cb_V_Mnt_K_KishuName != "")KishuName++;
	if(EDIT_K_WRank_V != "")WRank++;
	if(Cb_V_Mnt_K_SZ_Name != "")SZ++;
	if(Cb_V_Mnt_K_SZ_Kata != "")SZ++;
	if(Cb_V_Mnt_K_Other1_Name != "")Other1++;
	if(Cb_V_Mnt_K_Other1_Kata != "")Other1++;
	if(EDIT_K_Other1 != "")Other1++;
	if(Cb_V_Mnt_K_Other2_Name != "")Other2++;
	if(Cb_V_Mnt_K_Other2_Kata != "")Other2++;
	if(EDIT_K_Other2 != "")Other2++;
	if(Cb_V_Mnt_K_Other3_Name != "")Other3++;
	if(Cb_V_Mnt_K_Other3_Kata != "")Other3++;
	if(EDIT_K_Other3 != "")Other3++;
	if(Cb_V_Mnt_K_Other4_Name != "")Other4++;
	if(Cb_V_Mnt_K_Other4_Kata != "")Other4++;
	if(EDIT_K_Other4 != "")Other4++;
	if(Cb_V_Mnt_K_Other5_Name != "")Other5++;
	if(Cb_V_Mnt_K_Other5_Kata != "")Other5++;
	if(EDIT_K_Other5 != "")Other5++;


//ファイルの書き込み
CStdioFile ofp;

CString buf;
CString tmp[16];

i=0;j=0;
	//一致しているものを探す
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Cb_V_Mnt_K_KishuName) == 0 && pWnd->Jyushi_List[i][1].Compare(EDIT_K_WRank_V) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T(""))break;
	}

	if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
				ofp.SeekToEnd();
				//一行の中で半端な入力を受け付けない
				if((KishuName == 0 || KishuName == 1) && (WRank == 0 || WRank == 1) && (SZ == 2 || SZ == 0) && (Other1 == 3 || Other1 == 0) && (Other2 == 3 || Other2 == 0) && (Other3 == 3 || Other3 == 0) && (Other4 == 3 || Other4 == 0) && (Other5 == 3 || Other5 == 0)){
					
					if(KishuName == 0 && WRank == 0 && SZ == 0 && Other1 == 0 && Other2 == 0 && Other3 == 0 && Other4 == 0 && Other5 == 0)
					AfxMessageBox(_T("未入力です."));

					else if(KishuName == 0)
					AfxMessageBox(_T("機種名が未入力です."));

					else if(bothsame == 1){
					AfxMessageBox(_T("すでに登録済みです."));
					}

					else{//未入力でなければ
						

					if(KishuName == 1){
					ofp.WriteString(Cb_V_Mnt_K_KishuName);
					if(WRank == 0)ofp.WriteString(_T(",endpoint,\n"));//次のボックスに何も入っていなければ
					else ofp.WriteString(_T(","));
					
					Cb_C_Mnt_K_KishuName.InsertString(-1, Cb_V_Mnt_K_KishuName);//CBリストへ追加
					AllName[i] = Cb_V_Mnt_K_KishuName;//リストへ追加する
					AllNameCnt++;//配列を増やす

					pWnd->Jyushi_List[i][0] = Cb_V_Mnt_K_KishuName;

					}


					if(WRank == 1){//ボックスに値が入っていたとき
				ofp.WriteString(EDIT_K_WRank_V);
				AllNameWRank[i] = EDIT_K_WRank_V;//リストへ追加する
				AllNameWRankCnt++;//配列を増やす
				pWnd->Jyushi_List[i][1] = EDIT_K_WRank_V;

				if(SZ == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}


					if(SZ == 2){//全てのボックスに値が入っていたとき
				ofp.WriteString(Cb_V_Mnt_K_SZ_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_SZ_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(_T("100"));
				//配列に格納
				pWnd->Jyushi_List[i][2] = Cb_V_Mnt_K_SZ_Name;
				pWnd->Jyushi_List[i][3] = Cb_V_Mnt_K_SZ_Kata;
				pWnd->Jyushi_List[i][4] = _T("100");

				if(Other1 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other1 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other1_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other1_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other1);

				pWnd->Jyushi_List[i][5] = Cb_V_Mnt_K_Other1_Name;
				pWnd->Jyushi_List[i][6] = Cb_V_Mnt_K_Other1_Kata;
				pWnd->Jyushi_List[i][7] = EDIT_K_Other1;

				if(Other2 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
				
					}

					if(Other2 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other2_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other2_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other2);

				pWnd->Jyushi_List[i][8] = Cb_V_Mnt_K_Other2_Name;
				pWnd->Jyushi_List[i][9] = Cb_V_Mnt_K_Other2_Kata;
				pWnd->Jyushi_List[i][10] = EDIT_K_Other2;

				if(Other3 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other3 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other3_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other3_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other3);

				pWnd->Jyushi_List[i][11] = Cb_V_Mnt_K_Other3_Name;
				pWnd->Jyushi_List[i][12] = Cb_V_Mnt_K_Other3_Kata;
				pWnd->Jyushi_List[i][13] = EDIT_K_Other3;


				if(Other4 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other4 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other4_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other4_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other4);

				pWnd->Jyushi_List[i][14] = Cb_V_Mnt_K_Other4_Name;
				pWnd->Jyushi_List[i][15] = Cb_V_Mnt_K_Other4_Kata;
				pWnd->Jyushi_List[i][16] = EDIT_K_Other4;

				if(Other5 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other5 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other5_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other5_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other5);

				pWnd->Jyushi_List[i][17] = Cb_V_Mnt_K_Other5_Name;
				pWnd->Jyushi_List[i][18] = Cb_V_Mnt_K_Other5_Kata;
				pWnd->Jyushi_List[i][19] = EDIT_K_Other5;

				ofp.WriteString(_T(",endpoint,\n"));

				//ofp.WriteString(_T("\n"));
					}
				AfxMessageBox(_T("登録が完了しました."));
					}
				}
				else
					AfxMessageBox(_T("未入力部分があります."));
	}
	ofp.Close();

	UpdateData(FALSE);

}

void CMnt_Kishu::OnBnClickedBnMntKSakujo()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	CString str = pWnd->Jyushidatacsv;

CString newfile = _T("C:\\EB\\mst\\temp.csv");

Cb_C_Mnt_K_KishuName.GetWindowTextW(Cb_V_Mnt_K_KishuName);


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



//ファイル読み込み
CStdioFile fp;
//ファイルの書き込み
CStdioFile ofp;

CString buf;
CString prebuf;
CString tmp[4];
int a,next,i=0,j,point;
int NameCnt=0,cnt=0,deletecnt=0,KishuSame=0,bothsame = 0;


//波長ランクを変数に格納
UpdateData(TRUE);

j=0;
	//一致しているものを探す
	for(next=0;next<SIZE;next++){
		if(pWnd->Jyushi_List[next][0].Compare(Cb_V_Mnt_K_KishuName) == 0 && pWnd->Jyushi_List[next][1].Compare(EDIT_K_WRank_V) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[next][0] == _T(""))break;
	}

	point = next;
	//removeする箇所に次の配列の番号を代入.すると(next+1)番目の機種が配列上には2つ存在することになる.
	if(bothsame == 1){
		//while(pWnd->Jyushi_List[next+1][0] != _T("")){
		while(pWnd->Jyushi_List[next][0] != _T("")){
			for(i=0;i<20;i++)
				pWnd->Jyushi_List[next][i] = pWnd->Jyushi_List[next+1][i];
			next++;
		}
		//next++;	
		//}
	}

	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		if(ofp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){

			if(bothsame == 1 && cnt != point){
			
			//,区切りのデータをtmpに抽出
			for(int i=0; i<4; i++) tmp[i] = _T("");

			for(int i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					//if(Cb_V_Mnt_K_KishuName.Compare(tmp[i]) == 0){
					////削除したい項目の番号を取得してDeleteString(i)でコンボボックスのプルダウンを更新したい
					////Cb_C_Mnt_K_KishuName.DeleteString(cnt);
					//Cb_C_Mnt_K_KishuName.DeleteString(point-1);

					//}		
						ofp.SeekToEnd();		
						ofp.WriteString(buf);//一行全て書き込む
						ofp.WriteString(_T("\n"));//最後に改行を入れる
						break;
					}
			}//buf中の,を見つけるif文
				buf.Delete(0, a+1);
			
			}//For文中
			else if(bothsame == 1 && cnt == point){
				Cb_C_Mnt_K_KishuName.DeleteString(point-1);
			}

				cnt++;
		}//削除対象の機種があるかどうか

			//else if(cnt == 0){ 
			//	//AfxMessageBox(_T("未入力です."));
			//	//cnt++;
			//}
		}
	}
	
		ofp.Close();
		fp.Close();

		//if(Cb_V_Mnt_K_KishuName != _T("") && NameCnt == 1){//該当機種が見つかったら
		if(bothsame >0){//該当機種が見つかったら
		DeleteFile(str);
		MoveFile(newfile,str);
		AfxMessageBox(_T("削除が完了しました."));
		}
		//else if(NameCnt == 0 && cnt == 0){//該当機種が見つからなかったら
		else if(bothsame == 0){//該当機種が見つからなかったら
		/*DeleteFile(str);
		MoveFile(newfile,str);*/
		DeleteFile(newfile);
		AfxMessageBox(_T("該当機種が見つかりません."));
		}
}

BOOL CMnt_Kishu::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  ここに初期化を追加してください
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	
	int i;

	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
		Cb_C_Mnt_K_KishuName.InsertString(-1, pWnd->Jyushi_List[i][0]);
	}

Cb_V_Mnt_K_SZ_Name = "B剤";
Cb_V_Mnt_K_Other1_Name = "A剤";
Cb_V_Mnt_K_Other2_Name = "フィラー";
Cb_V_Mnt_K_Other3_Name = "蛍光体";

//変数からEditへ
UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void CMnt_Kishu::OnBnClickedEnd()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	OnClose();
}

void CMnt_Kishu::OnCbnSelchangeCbMntKKishuname()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	//機種を選択した時に、各パラメータをエディットボックスへ表示させる処理を記述
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int BoxNum=0;

	BoxNum = Cb_C_Mnt_K_KishuName.GetCurSel();

	Cb_V_Mnt_K_KishuName = pWnd->Jyushi_List[BoxNum+1][0];
	EDIT_K_WRank_V = pWnd->Jyushi_List[BoxNum+1][1];

	Cb_V_Mnt_K_SZ_Name = pWnd->Jyushi_List[BoxNum+1][2];
	Cb_V_Mnt_K_SZ_Kata = pWnd->Jyushi_List[BoxNum+1][3];

	Cb_V_Mnt_K_Other1_Name = pWnd->Jyushi_List[BoxNum+1][5];
	Cb_V_Mnt_K_Other1_Kata = pWnd->Jyushi_List[BoxNum+1][6];
	EDIT_K_Other1 = pWnd->Jyushi_List[BoxNum+1][7];

	Cb_V_Mnt_K_Other2_Name = pWnd->Jyushi_List[BoxNum+1][8];
	Cb_V_Mnt_K_Other2_Kata = pWnd->Jyushi_List[BoxNum+1][9];
	EDIT_K_Other2 = pWnd->Jyushi_List[BoxNum+1][10];

	Cb_V_Mnt_K_Other3_Name = pWnd->Jyushi_List[BoxNum+1][11];
	Cb_V_Mnt_K_Other3_Kata = pWnd->Jyushi_List[BoxNum+1][12];
	EDIT_K_Other3 = pWnd->Jyushi_List[BoxNum+1][13];

	Cb_V_Mnt_K_Other4_Name = pWnd->Jyushi_List[BoxNum+1][14];
	Cb_V_Mnt_K_Other4_Kata = pWnd->Jyushi_List[BoxNum+1][15];
	EDIT_K_Other4 = pWnd->Jyushi_List[BoxNum+1][16];

	Cb_V_Mnt_K_Other5_Name = pWnd->Jyushi_List[BoxNum+1][17];
	Cb_V_Mnt_K_Other5_Kata = pWnd->Jyushi_List[BoxNum+1][18];
	EDIT_K_Other5 = pWnd->Jyushi_List[BoxNum+1][19];

	UpdateData(FALSE);


}

void CMnt_Kishu::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_Mnt_Kishu = FALSE;
	DestroyWindow();

	CDialog::OnClose();
}
