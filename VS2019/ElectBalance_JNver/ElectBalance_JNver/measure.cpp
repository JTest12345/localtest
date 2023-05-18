// measure.cpp : 実装ファイル
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "measure.h"

#include "ElectBalance_JNverDlg.h"

#define TIMER_LOOP 9876543


// Cmeasure ダイアログ

IMPLEMENT_DYNAMIC(Cmeasure, CDialog)

Cmeasure::Cmeasure(CWnd* pParent /*=NULL*/)
	: CDialog(Cmeasure::IDD, pParent)
	, Edit_Measure_Kishu(_T(""))
	, Edit_V_msr_Keiryo(_T(""))
	, Edit_V_msr_Sagyousha(_T(""))
	, Edit_Measure_WRank(_T(""))
{
//LOGFONT
	logfont.lfHeight = 30;
	logfont.lfWidth = 15;
	logfont.lfEscapement = 0;
	logfont.lfOrientation = 0;
	logfont.lfWeight = FW_THIN;
	logfont.lfItalic = 0;
	logfont.lfUnderline = 0;
	logfont.lfStrikeOut = 0;
	logfont.lfCharSet = SHIFTJIS_CHARSET;
	logfont.lfOutPrecision = 1;
	logfont.lfClipPrecision = 2;
	logfont.lfQuality = 2;
	logfont.lfPitchAndFamily = 2;
	CString fontstr; fontstr.Format(_T("ＭＳ ゴシック"));
	memcpy(logfont.lfFaceName, fontstr.GetBuffer(), fontstr.GetLength()*2);//ＭＳ Ｐゴシック
	fontstr.ReleaseBuffer();
	new_font.CreateFontIndirect(&logfont);

	int i,j;
	for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
	//リストコントロール2次元配列を初期化
	for(i=0; i<10; i++)
		for(j=0; j<10; j++)
			ListC[i][j] = _T("");


}

Cmeasure::~Cmeasure()
{
}

void Cmeasure::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_LIST1, m_listc);
	DDX_Text(pDX, IDC_EDIT_Measure_Kishu, Edit_Measure_Kishu);
	DDX_Control(pDX, IDC_EDIT_Measure_Kishu, Edit_C_msr_Kishu);
	DDX_Text(pDX, IDC_EDIT1, Edit_V_msr_Keiryo);
	DDX_Control(pDX, IDC_EDIT1, Edit_C_msr_Keiryo);
	DDX_Control(pDX, IDC_EDIT2, Edit_C_msr_Sagyousha);
	DDX_Text(pDX, IDC_EDIT2, Edit_V_msr_Sagyousha);
	DDX_Control(pDX, IDC_KEnd, Bn_msr_Kend);
	DDX_Control(pDX, IDC_KStart, Bn_msr_KStart);
	DDX_Text(pDX, IDC_EDIT_WRank, Edit_Measure_WRank);
	DDX_Control(pDX, IDC_EDIT_WRank, Edit_C_msr_WRank);
}


BEGIN_MESSAGE_MAP(Cmeasure, CDialog)
	ON_BN_CLICKED(IDC_KStart, &Cmeasure::OnBnClickedKstart)
	ON_BN_CLICKED(IDC_KEnd, &Cmeasure::OnBnClickedKend)
	ON_BN_CLICKED(IDC_End, &Cmeasure::OnBnClickedEnd)
	ON_BN_CLICKED(IDOK, &Cmeasure::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &Cmeasure::OnBnClickedCancel)
	ON_WM_TIMER()
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_REJ, &Cmeasure::OnBnClickedRej)
	ON_NOTIFY(LVN_ITEMCHANGED, IDC_LIST1, &Cmeasure::OnLvnItemchangedList1)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()


// Cmeasure メッセージ ハンドラ

BOOL Cmeasure::OnInitDialog()
{
	CDialog::OnInitDialog();

	CStdioFile fp;

CString buf;
CString tmp[25];

int i,j;
int first=0,cnt=0,vcnt=0,bothsame=0;
double temp;
int test=0;
CString Stest;
CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	// TODO:  ここに初期化を追加してください
	row_cnt = 1;
	col_cnt = 1;
	FinishCnt = 0;//各剤の終了個数カウント
	//リストビューのカラムを設定(レポート)
	LVCOLUMN lvcol;
	lvcol.mask = LVCF_TEXT | LVCF_WIDTH;//LVCOLUMNのテキスト、幅が有効
	//1のカラム
	/*lvcol.pszText = _T("No");
	lvcol.cx = 40;
	m_listc.InsertColumn(0,&lvcol);*/
	//2のカラム
	lvcol.pszText = _T("使用樹脂");
	lvcol.cx = 80;
	m_listc.InsertColumn(0,&lvcol);
	//3のカラム
	lvcol.pszText = _T("樹脂型番");
	lvcol.cx = 80;
	m_listc.InsertColumn(1,&lvcol);
	//4のカラム
	lvcol.pszText = _T("理論値");
	lvcol.cx = 60;
	m_listc.InsertColumn(2,&lvcol);
	//5のカラム
	lvcol.pszText = _T("計量値");
	lvcol.cx = 60;
	m_listc.InsertColumn(3,&lvcol);

	//m_listc.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_CHECKBOXES);

	CString str = pWnd->Jyushidatacsv;
	
	Keiryo = 0;//適当な数値で初期化
	RValue = 10000;//適当な数値で初期化
	gosa = _tcstod(pWnd->gosa,NULL);//誤差の範囲

//ファイル読み込み
Num = _tcstod(msr_volume,NULL);//doubleへ型変換
Num = Num/100;//比率を計算

for(i=0; i<10; i++) Volume[i] = -1;//初期化

CheckBoxNum = 0;//チェックされている行を格納する変数を初期化

//Editから変数へ
UpdateData(TRUE);
	
UpdateData(FALSE);

//一致しているものを探す
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Edit_Measure_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(Edit_Measure_WRank) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			bothsame = 2;
			break;
		}
	}

	j=2;//剤名から入力
	while(pWnd->Jyushi_List[i][j] != _T("")){
		if(first == 0){
			m_listc.InsertItem(row_cnt,pWnd->Jyushi_List[i][j]);
			first++;
			ListC[row_cnt-1][0]=pWnd->Jyushi_List[i][j];//使用樹脂名を格納
			}
		else{
			ListC[row_cnt-1][col_cnt]=pWnd->Jyushi_List[i][j];//1行2列目以降を格納
			if(j==4 || j == 7 || j == 10 || j == 13 || j == 16 || j == 19){
				temp = _tcstod(ListC[row_cnt-1][col_cnt],NULL);//CStringからDoubleへ
					temp = temp * Num;//比率をかける
					ListC[row_cnt-1][col_cnt].Format(_T("%g"), temp);//DoubleからCStringへ
			}
			m_listc.SetItemText(row_cnt-1, col_cnt, ListC[row_cnt-1][col_cnt]);//2列目へ追加する
			
				col_cnt++;
				if(col_cnt == 3){
					col_cnt = 1;
					row_cnt++;
					first = 0;
				}
		}
		j++;		
	}
					
	//if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

	//	while(fp.ReadString(buf)){
	//		//,区切りのデータをtmpに抽出
	//		for(i=0; i<25; i++) tmp[i] = _T("");
	//		//KishuName=0,SZ=0,Other1=0,Other2=0,Other3=0,Other4=0,Other5=0;

	//		for(i=0; i<25; i++){
	//			a = buf.Find(_T(","));
	//			if(a!=-1){
	//				tmp[i] = buf.Left(a);

	//				if(tmp[i].Compare(_T("endpoint")) == 0){
	//					cnt = 2;
	//					break;
	//				}

	//				//test++;//デバッグ用

	//				
	//				if(Edit_Measure_Kishu.Compare(tmp[i]) == 0)cnt=1;

	//				//一度一致したらブレイクしない
	//				if(Edit_Measure_Kishu.Compare(tmp[i]) != 0 && cnt == 0)break;
	//				
	//				else{
	//				
	//				
	//				if(i == 4 || i == 7 || i == 10 || i == 13 || i == 16 || i == 19){
	//				temp = _tcstod(tmp[i],NULL);//CStringからDoubleへ
	//				temp = temp * Num;//比率をかける
	//				tmp[i].Format(_T("%g"), temp);//DoubleからCStringへ
	//				if(vcnt<10)
	//				Volume[vcnt] = temp;//各行の理論値を格納
	//				vcnt++;
	//				}

	//				if(first < 2){//機種名、波長ランクは表に入れない
	//				first++;
	//				if(first == 2){//波長ランクが一致するか確認する
	//					if(Edit_Measure_WRank.Compare(tmp[i]) != 0){
	//						AfxMessageBox(_T("波長ランクが登録されていません."));
	//						break;
	//					}

	//				}
	//				}
	//				else if(first == 2){
	//				m_listc.InsertItem(row_cnt,tmp[i]);
	//				first++;
	//				ListC[row_cnt-1][col_cnt-1]=tmp[i];//使用樹脂名を格納
	//				}
	//				else{
	//				m_listc.SetItemText(row_cnt-1, col_cnt, tmp[i]);//2列目へ追加する
	//				ListC[row_cnt-1][col_cnt]=tmp[i];//1行2列目以降を格納
	//				
	//				col_cnt++;
	//				if(col_cnt == 3){
	//					col_cnt = 1;
	//					row_cnt++;
	//					first = 2;
	//				}

	//				}
	//				
	//				buf.Delete(0, a+1);

	//				}

	//			}
	//				//break;//機種名のみリストにして終了
	//					
	//		}
	//				if(cnt == 2)break;
	//				//buf.Delete(0, a+1);
	//	}
	//			
	//}

	//	fp.Close();

	Edit_C_msr_Kishu.SetFont(&new_font,TRUE);
	Edit_C_msr_WRank.SetFont(&new_font,TRUE);
	Edit_C_msr_Keiryo.SetFont(&new_font,TRUE);
	Edit_C_msr_Sagyousha.SetFont(&new_font,TRUE);

//リストコントロールにチェックボックスを追加
DWORD dwStyle = m_listc.GetExtendedStyle();
dwStyle |= LVS_EX_CHECKBOXES | LVS_EX_FULLROWSELECT | LVS_EX_GRIDLINES;
m_listc.SetExtendedStyle(dwStyle);

//計測終了ボタンを無効化
Bn_msr_Kend.EnableWindow(FALSE);


	return TRUE;  // return TRUE unless you set the focus to a control
	// 例外 : OCX プロパティ ページは必ず FALSE を返します。
}

void Cmeasure::OnBnClickedKstart()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int i,cnt=0;

	ItemNum = ListView_GetItemCount(m_listc);//行数を取得

	for(i=0;i<ItemNum;i++){
		if(ListView_GetCheckState(m_listc,i)!=0){
			CheckBoxNum = i;
			cnt++;
			if(cnt >=2){
			AfxMessageBox(_T("複数チェックされています."));
			break;
			}
		}
	}
	if(cnt == 0)
		AfxMessageBox(_T("チェックされていません."));
	if(cnt == 1){
		RValue = _tcstod(ListC[CheckBoxNum][2],NULL);

		if(pWnd->dlflag_JyushiCode == FALSE){
			//樹脂の型番が合っているか確認する
			pWnd->dlflag_JyushiCode = TRUE;
			pWnd->JC.Create(IDD_JyushiCode,this);
			pWnd->JC.ShowWindow(SW_SHOWNA);//表示
		
			//if(pWnd->JC.flg_same == TRUE){
			//	Bn_msr_Kend.EnableWindow(TRUE);//計測終了ボタンを有効化
				//タイマーの呼び出し
			//	SetTimer(TIMER_LOOP,200,NULL);
			//}
		}
	}
	
	// 表示更新
    Invalidate();

}

void Cmeasure::OnBnClickedKend()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	int i,j,cnt=0;
	CString Stest;
	//double Keiryo;
	
	char sbuf[128];//受信用
	memset(sbuf, 0, 128);

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	CString str = pWnd->Resultcsv;
	gosa = _tcstod(pWnd->gosa,NULL);//誤差の範囲
	//double RValue;
	int fns_flag = 0;

	//ファイル読み込み
	CStdioFile fp;
	//ファイルの書き込み
	CStdioFile ofp;

	CString buf;
	CString tmp[16];

	CTime cTime;
	CString date; 

	cTime = CTime::GetCurrentTime();           // 現在時刻
    date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"形式の時刻文字列を取得

	for(i=0;i<ItemNum;i++){
		if(ListView_GetCheckState(m_listc,i)!=0){
			CheckBoxNum = i;
			cnt++;
			if(cnt >=2){
			AfxMessageBox(_T("複数チェックされています."));
			break;
			}
		}
	}
	if(cnt == 0)
		AfxMessageBox(_T("チェックされていません."));

	if(cnt == 1){
	//Editから変数へ
	UpdateData(TRUE);


	//変数からEditへ
	UpdateData(FALSE);

	RValue = _tcstod(ListC[CheckBoxNum][2],NULL);
	
	Keiryo = _tcstod(Edit_V_msr_Keiryo,NULL);//CStringからDoubleへ
	if(Keiryo - RValue <= gosa && Keiryo - RValue >= (gosa*(-1))){//誤差含め、OKであれば

		m_listc.SetItemText(CheckBoxNum, 3, Edit_V_msr_Keiryo);//計量値をリストコントロールへ追加する

		if(CBVolumeData[CheckBoxNum] == _T("")){
		FinishCnt++;//終了項目をカウントする
		}

		CBVolumeData[CheckBoxNum]=Edit_V_msr_Keiryo;
		ListC[CheckBoxNum][3] = Edit_V_msr_Keiryo;//計量値を格納
		

		//天秤のリセットを行う
		//RS232Cの処理
		pWnd->Jshg.msr.er=pWnd->rs232c.Open(pWnd->Jshg.msr.n,4096,4096);
		pWnd->Jshg.msr.er=pWnd->rs232c.Config(pWnd->Jshg.msr.n,RS232_2400|RS232_1|RS232_EVEN|RS232_7|RS232_X_N|RS232_CTS_Y|RS232_DSR_Y|RS232_DTR_Y|RS232_RTS_Y);
		pWnd->Jshg.msr.er=pWnd->rs232c.BaudRate(pWnd->Jshg.msr.n, 2400);

		pWnd->Jshg.msr.sbuf[0] = 0x5a;//Z(0Reset)
		pWnd->Jshg.msr.sbuf[1] = 0x0d;//CR
		pWnd->Jshg.msr.sbuf[2] = 0x0a;//LF
		pWnd->Jshg.msr.er = pWnd->rs232c.Send(pWnd->Jshg.msr.n,pWnd->Jshg.msr.sbuf,3);


		pWnd->rs232c.Close(n);


		if(FinishCnt < ItemNum)//
			KillTimer(TIMER_LOOP);//タイマーをストップ

		else if(FinishCnt >= ItemNum){//全て終了したらファイル書き込み操作を行う
			//次の測定に備え、必要な変数を初期化
			for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
			FinishCnt = 0;
			fns_flag = 1;
			CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
			
			if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
	
				ofp.SeekToEnd();

				
				ofp.WriteString(date);//日付
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_J_Kishu);//機種名
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.W_rank_V);//波長ランク
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.LotNo);//ロットナンバー
				ofp.WriteString(_T(","));
				ofp.WriteString(pWnd->Jshg.Cb_V_J_Sagyousha);//作業者名
				ofp.WriteString(_T(","));

				for(i=0;i<ItemNum;i++){
					for(j=0;j<4;j++){
						ofp.WriteString(ListC[i][j]);
						ofp.WriteString(_T(","));
					}
				}
				ofp.WriteString(_T("\n"));
			}
			ofp.Close();

			m_listc.EnableWindow(TRUE);//チェックボックスを有効化
			m_listc.SetCheck(CheckBoxNum,FALSE);//チェックボックスのチェック箇所を外す
			Bn_msr_KStart.EnableWindow(TRUE);//計測開始ボタンを有効化
			KillTimer(TIMER_LOOP);//タイマーをストップ
			
			AfxMessageBox(_T("データの書き込みが完了しました."));

			//Jyushihaigouダイアログの各Editの値を初期化
			pWnd->Jshg.LotNo = _T("");
			pWnd->Jshg.Cb_J_Kishu = _T("");
			pWnd->Jshg.W_rank_V = _T("");
			pWnd->Jshg.e_MainVolume = _T("");
			pWnd->Jshg.Cb_V_J_Sagyousha = _T("");
		
			//measureダイアログのパラメータ初期化
			Edit_V_msr_Keiryo = _T("");

			//変数からEditへ
			UpdateData(FALSE);
			//Editから変数へ
			UpdateData(TRUE);

			
		}

		//Stest.Format(_T("%g"), (Keiryo - Volume[CheckBoxNum]));//DoubleからCStringへ
		//AfxMessageBox(Edit_V_msr_Keiryo);
		//AfxMessageBox(Stest);

		//measureダイアログのパラメータ初期化
		Edit_V_msr_Keiryo = _T("");

		//変数からEditへ
		UpdateData(FALSE);

		m_listc.EnableWindow(TRUE);//チェックボックスを有効化
		m_listc.SetCheck(CheckBoxNum,FALSE);
		Bn_msr_KStart.EnableWindow(TRUE);//計測開始ボタンを有効化
		Bn_msr_Kend.EnableWindow(FALSE);//計測終了ボタンを有効化

		if(fns_flag == 1){
		//measureウィンドウを破棄する
			pWnd->Jshg.OnClose();
		}

	}

	else{
		AfxMessageBox(_T("計測量が正しくありません."));
	}

	//for(i=0;i<10;i++){
	//Stest.Format(_T("%g"),Volume[i]);//デバッグ用
	//AfxMessageBox(Stest);
	//}
	}
}

void Cmeasure::OnBnClickedEnd()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	if(MessageBox(_T("終了します.よろしいですか？"),NULL,MB_OKCANCEL) == IDOK){
		OnClose();
	}
}

void Cmeasure::OnBnClickedOk()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	OnOK();
}

void Cmeasure::OnBnClickedCancel()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	OnCancel();
}

void Cmeasure::OnTimer(UINT_PTR nIDEvent)
{
	CString trace;

	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	//int bufsize = 128;
	char rbuf[128];//受信用
	memset(rbuf, 0, 128);
	memset(sbuf, 0, 128);
	CString str,CStVolume;

	int i;
	int temp,Num,keta=0,secondp=0,allcheck=0;
	int startpoint=-1,dotpoint=-1;
	NowVolume=0;
	er = 0;

	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	n=_ttoi(pWnd->COMPORT);//COMポートナンバー
	//pWnd->COMPORT.ReleaseBuffer();


//	n = _tcstoi(pWnd->COMPORT,NULL);

	switch(nIDEvent){
		case TIMER_LOOP:
			//RS232Cの処理
			er=pWnd->rs232c.Open(n,4096,4096);

			er=pWnd->rs232c.Config(n,RS232_2400|RS232_1|RS232_EVEN|RS232_7|RS232_X_N|RS232_CTS_Y|RS232_DSR_Y|RS232_DTR_Y|RS232_RTS_Y);
			er=pWnd->rs232c.BaudRate(n, 2400);


			//sbuf[0] = 0x5a;//Z(0Reset)
			//sbuf[0] = 0x43;//C(Stop)
			//sbuf[0] = 0x51;//Q(Measure)
			//sbuf[1] = 0x0d;//CR
			//sbuf[2] = 0x0a;//LF
			//er = pWnd->rs232c.Send(n,sbuf,3);


			//【データ受信】レーザマーカ->コントローラ
			//必ず18byteは受信する
			while(1){
				er += pWnd->rs232c.Recv(n, &rbuf[er], 128);
				if(er >= 18)
					break;
			}

			//前から検索
			for(i=0;i<er;i++){
				temp = (int)rbuf[i];//アスキーコードを取得

				if(temp == 43){//+が見つかったら(2回目)
					startpoint = i;
				}
				else if(temp == 46){//.が見つかったら
					dotpoint = 1;
					break;
				}

				//else if(temp == 43 && secondp == 0)//+が見つかったら(1回目)
				//	secondp = 1;

				//else if(rbuf[0] == 0)break;//何も入っていないときはループを抜ける
			}


			//trace.Format(_T("%c%c%c%c%c%c%c%c%c\n"), rbuf[startpoint], rbuf[startpoint+1], rbuf[startpoint+2], rbuf[startpoint+3], rbuf[startpoint+4], rbuf[startpoint+5], rbuf[startpoint+6], rbuf[startpoint+7], rbuf[startpoint+8]);
			//TRACE(trace);
			//事前に全ての位に数字が入っているか確認
			for(i=startpoint+1;i<startpoint+9;i++){
			temp = (int)rbuf[i];//アスキーコードを取得
			Num = AsciiNumReturn(temp);
			if(Num != -1){
				allcheck++;
			}
			}

			if(allcheck == 7){//7桁全てに数字が入っていたら
			for(i=startpoint+1;i<startpoint+9;i++){
				temp = (int)rbuf[i];//アスキーコードを取得
				if(temp != 48){//0以外の数字のとき
					Num = AsciiNumReturn(temp);
					if(Num != -1){
						if(keta == 0)NowVolume += Num*10000;
						else if(keta == 1)NowVolume += Num*1000;
						else if(keta == 2)NowVolume += Num*100;
						else if(keta == 3)NowVolume += Num*10;
						else if(keta == 4)NowVolume += Num*1;
						else if(keta == 6)NowVolume += Num*0.1;
						else if(keta == 7)NowVolume += Num*0.01;
																								

					}
				}
				if(keta == 7)break;//小数点第二位まで値を確認したらループを抜ける
				keta++;
			}
			Keiryo = NowVolume;
			}
			//temp = _tcstod(tmp[i],NULL);//CStringからDoubleへ
			//if(NowVolume != 0){
			//	CStVolume.Format(_T("%g"), NowVolume);//DoubleからCStringへ
			//	AfxMessageBox(CStVolume);
			//}

		

			//trace.Format(_T("%g\n"), NowVolume);
			//TRACE(trace);
			if(startpoint != -1 && dotpoint != -1){
			Edit_V_msr_Keiryo.Format(_T("%g"), NowVolume);
			//変数からEditへ
			UpdateData(FALSE);
			}

			//正しく受信できていないとき
		//	if(er != 128){
		//		er = AfxMessageBox(_T("Recv Timeout"),MB_RETRYCANCEL);
		//		if(er == IDCANCEL){
		//			pWnd->rs232c.ClearRecv(n);
		//			//return 3;
		//	}
		//}	

		pWnd->rs232c.Close(n);

			break;
			
		default:
			break;
	}

	CDialog::OnTimer(nIDEvent);

		
}


int Cmeasure::AsciiNumReturn(int Ascii)
{
	int Num;
	if(Ascii == 48)Num=0;
	else if(Ascii == 49)Num=1;
	else if(Ascii == 50)Num=2;
	else if(Ascii == 51)Num=3;
	else if(Ascii == 52)Num=4;
	else if(Ascii == 53)Num=5;
	else if(Ascii == 54)Num=6;
	else if(Ascii == 55)Num=7;
	else if(Ascii == 56)Num=8;
	else if(Ascii == 57)Num=9;

	else Num = -1;

	return Num;
}

void Cmeasure::OnClose()
{
	// TODO: ここにメッセージ ハンドラ コードを追加するか、既定の処理を呼び出します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	int i,j;
	for(i=0;i<ItemNum;i++)CBVolumeData[i]=_T("");
	//リストコントロール2次元配列を初期化
	for(i=0; i<10; i++)
		for(j=0; j<10; j++)
			ListC[i][j] = _T("");

	Edit_V_msr_Keiryo = _T("");

	//変数からEditへ
	UpdateData(FALSE);


	////if(pWnd->dlflag_JyushiCode == TRUE)
	//	pWnd->dlflag_JyushiCode == FALSE;
	////if(pWnd->dlflag_password == TRUE)
	//	pWnd->dlflag_password == FALSE;
	////if(pWnd->dlflag_renum == TRUE)
	//	pWnd->dlflag_renum == FALSE;


	pWnd->dlflag_measure = FALSE;

	

	DestroyWindow();

	CDialog::OnClose();
}

void Cmeasure::OnBnClickedRej()
{
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int cnt=0;

	ItemNum = ListView_GetItemCount(m_listc);//行数を取得

	for(int i=0;i<ItemNum;i++){
		if(ListView_GetCheckState(m_listc,i)!=0){
			CheckBoxNum = i;
			cnt++;
			if(cnt >=2){
			AfxMessageBox(_T("複数チェックされています."));
			break;
			}
		}
	}

	if(cnt == 0)AfxMessageBox(_T("チェックされていません."));

	else if(/*pWnd->dlflag_password == FALSE && */cnt == 1){
			//樹脂の型番が合っているか確認する
			pWnd->dlflag_password = TRUE;
			//pWnd->PW.Create(IDD_PASSWORD,this);
			//pWnd->PW.ShowWindow(SW_SHOWNA);//表示
			pWnd->PW.DoModal();

			//m_listc.SetItemText(1, 1, _T("aaa"));//4列目へ追加する
			if(pWnd->RN.Edit_V_rn != _T(""))
			m_listc.SetItemText(pWnd->Jshg.msr.CheckBoxNum, 2, pWnd->RN.Edit_V_rn);//3列目へ追加する

			pWnd->RN.Edit_V_rn = _T("");
	}
}

void Cmeasure::OnLvnItemchangedList1(NMHDR *pNMHDR, LRESULT *pResult)
{
	LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
	// TODO: ここにコントロール通知ハンドラ コードを追加します。
	*pResult = 0;
}

HBRUSH Cmeasure::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	// TODO:  ここで DC の属性を変更してください。

	// エディットボックスの色変更
    //if (nCtlColor == CTLCOLOR_EDIT)
	//if (nCtlColor == IDD_measure)
	if(Keiryo - RValue <= gosa && Keiryo - RValue >= (gosa*(-1))){//誤差含め、OKであれば
        if (*pWnd == Edit_C_msr_Keiryo)
        {
            // 文字色
            pDC->SetTextColor(RGB(255, 0, 0));
            // 背景色
            //hbr = static_cast<HBRUSH>(GetStockObject(BLACK_BRUSH));
            //pDC->SetBkColor(RGB(255, 0, 0));
        }
	}

	// TODO:  既定値を使用したくない場合は別のブラシを返します。
	return hbr;
}
