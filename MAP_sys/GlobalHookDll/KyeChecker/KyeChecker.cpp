// GlobalHookTest.cpp : アプリケーションのエントリ ポイントを定義します。
//

#include "pch.h"
//#include "stdafx.h"
#include "KyeChecker.h"
#include "GlobalHookDll.h" 

#include <tchar.h>

#define MAX_LOADSTRING 100
// グローバル変数:
HINSTANCE hInst;                                // 現在のインターフェイス
TCHAR szTitle[MAX_LOADSTRING];                  // タイトル バーのテキスト
TCHAR szWindowClass[MAX_LOADSTRING];            // メイン ウィンドウ クラス名
HWND hWnd; 
const int ClientWidth = 260;
const int ClientHeight = 40;
HFONT hFont1; 
HFONT hOldFont;
// このコード モジュールに含まれる関数の宣言を転送します:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

int status = 99;
LPCTSTR lpszText = _T("キー入力監視中");

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
    _In_opt_ HINSTANCE hPrevInstance,
    _In_ LPTSTR    lpCmdLine,
    _In_ int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);
    // TODO: ここにコードを挿入してください。
    MSG msg;
    HACCEL hAccelTable;
    // グローバル文字列を初期化しています。
    LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadString(hInstance, IDC_KYECHECKER, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);
    // アプリケーションの初期化を実行します:
    if (!InitInstance(hInstance, nCmdShow))
    {
        return FALSE;
    }
    hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_KYECHECKER));
    SetHook(hWnd);   // Add
    // メイン メッセージ ループ:
    while (GetMessage(&msg, NULL, 0, 0))
    {
        //DLLよりステータス取得
        //status = GetStatus();
        //InvalidateRect(0,0,false);

        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }
    ResetHook();   // Add
    return (int)msg.wParam;
}
//
//  関数: MyRegisterClass()
//
//  目的: ウィンドウ クラスを登録します。
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEX wcex;
    wcex.cbSize = sizeof(WNDCLASSEX);
    wcex.style = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc = WndProc;
    wcex.cbClsExtra = 0;
    wcex.cbWndExtra = 0;
    wcex.hInstance = hInstance;
    wcex.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_KYECHECKER));
    wcex.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
    wcex.lpszMenuName = MAKEINTRESOURCE(IDC_KYECHECKER);
    wcex.lpszClassName = szWindowClass;
    wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));
    return RegisterClassEx(&wcex);
}
//
//   関数: InitInstance(HINSTANCE, int)
//
//   目的: インスタンス ハンドルを保存して、メイン ウィンドウを作成します。
//
//   コメント:
//
//        この関数で、グローバル変数でインスタンス ハンドルを保存し、
//        メイン プログラム ウィンドウを作成および表示します。
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
    HWND hWnd;
    hInst = hInstance; // グローバル変数にインスタンス処理を格納します。
    // Mod Start
    hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);
    DWORD dwStyle = WS_OVERLAPPEDWINDOW ^ WS_THICKFRAME; // ウィンドウサイズ変更不可
    RECT rc = { 0, 0, ClientWidth, ClientHeight };
    AdjustWindowRectEx(&rc, dwStyle, FALSE, 0);
    int nWidth = rc.right - rc.left;
    int nHeight = rc.bottom - rc.top;
    hWnd = CreateWindow(
        szWindowClass,
        szTitle,
        dwStyle,
        CW_USEDEFAULT, CW_USEDEFAULT, nWidth, nHeight,
        NULL, NULL,
        hInstance, NULL
    );
    // Mod End
    if (!hWnd)
    {
        return FALSE;
    }
    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);


    //常に手前に表示
    SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, (SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW));

    return TRUE;
}


//
//  関数: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  目的:    メイン ウィンドウのメッセージを処理します。
//
//  WM_COMMAND  - アプリケーション メニューの処理
//  WM_PAINT    - メイン ウィンドウの描画
//  WM_DESTROY  - 中止メッセージを表示して戻る
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    int wmId, wmEvent;
    PAINTSTRUCT ps;
    HDC hdc;
    switch (message)
    {
    case WM_COMMAND:
        wmId = LOWORD(wParam);
        wmEvent = HIWORD(wParam);
        // 選択されたメニューの解析:
        switch (wmId)
        {
        case IDM_ABOUT:
            DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
            break;
        case IDM_EXIT:
            DestroyWindow(hWnd);
            break;
        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
        }
        break;

    case WM_PAINT:
        hdc = BeginPaint(hWnd, &ps);
        // TODO: 描画コードをここに追加してください...
        hOldFont = static_cast<HFONT>(SelectObject(hdc, hFont1));  // Add

        if(status==0){//同じ
            SetBkColor(hdc, RGB(0xFF, 0, 0));
            lpszText = _T("同じ               ");
        }
        else if(status==1) {
            SetBkColor(hdc, RGB(0, 0, 0xFF));
            lpszText = _T("違う               ");
        }

        TextOut(hdc, 0, 0, lpszText, _tcslen(lpszText));  // Add

        EndPaint(hWnd, &ps);
        break;

    case WM_DESTROY:
        PostQuitMessage(0);
        break;
    
    case WM_USER_RESULT:
        if(wParam==0){//同じ
            status = 0;
        }
        else {
            status = 1;
        }
        InvalidateRect(hWnd, NULL, FALSE);
        
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}
// バージョン情報ボックスのメッセージ ハンドラーです。
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;
    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}
// Add Start
HFONT MyCreateFont(int nHeight, DWORD dwCharSet, LPCTSTR lpName)
{
    return CreateFont(
        nHeight, 0, 0, 0,
        FW_DONTCARE,
        FALSE, FALSE, FALSE,
        dwCharSet,
        OUT_DEFAULT_PRECIS,
        CLIP_DEFAULT_PRECIS,
        ANTIALIASED_QUALITY,    // アンチエイリアス
        DEFAULT_PITCH | FF_DONTCARE,
        lpName
    );
}
// Add End