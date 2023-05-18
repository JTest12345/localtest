#include "pch.h"

#include <string.h>

// GlobalHookDll.cpp : DLL アプリケーション用にエクスポートされる関数を定義します。

//#include "stdafx.h"
#include <cstdio>
#include <tchar.h>
#include "GlobalHookDll.h"

// すべてのスレッドにセットされるフックになるので
// グローバル変数を共有する必要がある
// 共有セグメント
#pragma data_seg(".shareddata")
HHOOK hKeyHook = 0;
HWND g_hWnd = 0;        // キーコードの送り先のウインドウハンドル
#pragma data_seg()
HINSTANCE hInst;



EXPORT_API_ int SetHook(HWND hWnd)
{
    hKeyHook = SetWindowsHookEx(WH_KEYBOARD, KeyHookProc, hInst, 0);
    if (hKeyHook == NULL)
    {
        // フック失敗
    }
    else
    {
        // フック成功
        g_hWnd = hWnd;
    }
    return 0;
}
EXPORT_API_ int ResetHook()
{
    if (UnhookWindowsHookEx(hKeyHook) != 0)
    {
        // フック解除成功
    }
    else
    {
        // フック解除失敗
    }
    return 0;
}


int status = 0;

EXPORT_API_ int GetStatus()
{
    return status;
}


char now_char[1000];
char old_char[1000];
int num = 0;
bool f_try = true;


EXPORT_API_ LRESULT CALLBACK KeyHookProc(int nCode, WPARAM wp, LPARAM lp)
{
    TCHAR msg[64] = { 0 };


    if (nCode < 0)
        return CallNextHookEx(hKeyHook, nCode, wp, lp);
    if (nCode == HC_ACTION)
    {
        //目的のウインドウにキーボードメッセージと、キーコードの転送

        // どこで押してもHOOKする
        // ボタンが押された状態の時限定(離しはスルー)
        if ((lp & 0x80000000) == 0)
        {
            // 通常キー
            if ((lp & 0x20000000) == 0)
            {
                // Enter以外
                if (wp != VK_RETURN)
                {

                    now_char[num] = int(wp);
                    num++;

                    PostMessage(g_hWnd, WM_KEYDOWN, wp, 0);
                }
                else
                {//Enter時

                    ////初回のみ
                    //if (f_try) {
                    //    memcpy(old_char, now_char, sizeof(now_char));
                    //    f_try = false;
                    //}

                    wchar_t string1[1000] = { 0 };
                    MultiByteToWideChar(0, 0, now_char, strlen(now_char), string1, strlen(now_char));
                    wchar_t string2[1000] = { 0 };
                    MultiByteToWideChar(0, 0, old_char, strlen(old_char), string2, strlen(old_char));

                    //相手(EXE)のウィンドウハンドル取得
                    HWND hWnd = FindWindow(NULL, _T("KyeChecker"));

                    if (strcmp(now_char, old_char) == 0) {//同じ場合
                        //MessageBox(NULL, _T("同じ"), NULL, MB_OK);
                        status = 0;
                        PostMessage(hWnd, WM_USER_RESULT, status, 0);
                    }
                    else {
                        //MessageBox(NULL, _T("違う"), NULL, MB_OK);
                        status = 1;
                        PostMessage(hWnd, WM_USER_RESULT, status, 0);
                    }
                   

                    //MessageBox(NULL, string1, NULL, MB_OK);
                    //MessageBox(NULL, string2, NULL, MB_OK);


                    memcpy(old_char, now_char, sizeof(now_char));
                    memset(now_char, '\0', sizeof(now_char));            
                    num = 0;


                    PostMessage(g_hWnd, WM_KEYDOWN, wp, 0);
                }
            }
            // システムキー(Alt(+何か)、もしくはF10の時)
            else
            {
                //MessageBox(NULL, TEXT("システムキーが押されたよ！"), NULL, MB_OK);
                PostMessage(g_hWnd, WM_SYSKEYDOWN, wp, 0);
            }
        }
    }
    return CallNextHookEx(hKeyHook, nCode, wp, lp);
}
// エントリポイント
BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        // アタッチ
        hInst = hModule;
        break;
    case DLL_PROCESS_DETACH:
        // デタッチ
        break;
    }
    return TRUE;
}