// ElectBalance_JNver.cpp : アプリケーションのクラス動作を定義します。
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "ElectBalance_JNverDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CElectBalance_JNverApp

BEGIN_MESSAGE_MAP(CElectBalance_JNverApp, CWinApp)
	ON_COMMAND(ID_HELP, &CWinApp::OnHelp)
END_MESSAGE_MAP()


// CElectBalance_JNverApp コンストラクション

CElectBalance_JNverApp::CElectBalance_JNverApp()
{
	// TODO: この位置に構築用コードを追加してください。
	// ここに InitInstance 中の重要な初期化処理をすべて記述してください。
}


// 唯一の CElectBalance_JNverApp オブジェクトです。

CElectBalance_JNverApp theApp;

const GUID CDECL BASED_CODE _tlid =
		{ 0x5AF63B85, 0xBA7A, 0x4D20, { 0x9B, 0x2B, 0x2D, 0x64, 0x3E, 0x43, 0xB6, 0xE9 } };
const WORD _wVerMajor = 1;
const WORD _wVerMinor = 0;


// CElectBalance_JNverApp 初期化

BOOL CElectBalance_JNverApp::InitInstance()
{
	// アプリケーション マニフェストが visual スタイルを有効にするために、
	// ComCtl32.dll Version 6 以降の使用を指定する場合は、
	// Windows XP に InitCommonControlsEx() が必要です。さもなければ、ウィンドウ作成はすべて失敗します。
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// アプリケーションで使用するすべてのコモン コントロール クラスを含めるには、
	// これを設定します。
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	// OLE ライブラリを初期化します。
	if (!AfxOleInit())
	{
		AfxMessageBox(IDP_OLE_INIT_FAILED);
		return FALSE;
	}

	AfxEnableControlContainer();

	// 標準初期化
	// これらの機能を使わずに最終的な実行可能ファイルの
	// サイズを縮小したい場合は、以下から不要な初期化
	// ルーチンを削除してください。
	// 設定が格納されているレジストリ キーを変更します。
	// TODO: 会社名または組織名などの適切な文字列に
	// この文字列を変更してください。
	SetRegistryKey(_T("アプリケーション ウィザードで生成されたローカル アプリケーション"));
	// オートメーションまたは reg/unreg スイッチのコマンド ラインを解析します。
	CCommandLineInfo cmdInfo;
	ParseCommandLine(cmdInfo);

	// アプリケーションが /Embedding または /Automation スイッチで起動されました。
	// アプリケーションをオートメーション サーバーとして実行します。
	if (cmdInfo.m_bRunEmbedded || cmdInfo.m_bRunAutomated)
	{
		// CoRegisterClassObject() 経由でクラス ファクトリを登録します。
		COleTemplateServer::RegisterAll();
	}
	// アプリケーションが /Unregserver または /Unregister スイッチで起動されました。
	// レジストリからエントリを削除します。
	else if (cmdInfo.m_nShellCommand == CCommandLineInfo::AppUnregister)
	{
		COleObjectFactory::UpdateRegistryAll(FALSE);
		AfxOleUnregisterTypeLib(_tlid, _wVerMajor, _wVerMinor);
		return FALSE;
	}
	// アプリケーションがスタンドアロンまたは/Register や /Regserver などのスイッチで起動されました。
	// タイプ ライブラリを含むレジストリ エントリを更新します。
	else
	{
		COleObjectFactory::UpdateRegistryAll();
		AfxOleRegisterTypeLib(AfxGetInstanceHandle(), _tlid);
		if (cmdInfo.m_nShellCommand == CCommandLineInfo::AppRegister)
			return FALSE;
	}
	//日本語対応
	_tsetlocale(LC_ALL,_T(""));

	CElectBalance_JNverDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: ダイアログが <OK> で消された時のコードを
		//  記述してください。
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: ダイアログが <キャンセル> で消された時のコードを
		//  記述してください。
	}

	// ダイアログは閉じられました。アプリケーションのメッセージ ポンプを開始しないで
	//  アプリケーションを終了するために FALSE を返してください。
	return FALSE;
}

BOOL CElectBalance_JNverApp::PreTranslateMessage(MSG* pMsg)
{
	// TODO: ここに特定なコードを追加するか、もしくは基本クラスを呼び出してください。
    // ESCキーの押下を無効にする
    if( pMsg->message == WM_KEYDOWN ){
        if( pMsg->wParam == VK_ESCAPE ) return( TRUE );
    }
	//Enterキーが無効の設定@20151116解除
	/*if( pMsg->message == WM_KEYDOWN ){
        if( pMsg->wParam == VK_RETURN ) return( TRUE );
    }*/
	return CWinApp::PreTranslateMessage(pMsg);
}