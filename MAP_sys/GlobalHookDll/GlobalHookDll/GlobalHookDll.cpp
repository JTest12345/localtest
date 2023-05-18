#include "pch.h"

#include <string.h>

// GlobalHookDll.cpp : DLL �A�v���P�[�V�����p�ɃG�N�X�|�[�g�����֐����`���܂��B

//#include "stdafx.h"
#include <cstdio>
#include <tchar.h>
#include "GlobalHookDll.h"

// ���ׂẴX���b�h�ɃZ�b�g�����t�b�N�ɂȂ�̂�
// �O���[�o���ϐ������L����K�v������
// ���L�Z�O�����g
#pragma data_seg(".shareddata")
HHOOK hKeyHook = 0;
HWND g_hWnd = 0;        // �L�[�R�[�h�̑����̃E�C���h�E�n���h��
#pragma data_seg()
HINSTANCE hInst;



EXPORT_API_ int SetHook(HWND hWnd)
{
    hKeyHook = SetWindowsHookEx(WH_KEYBOARD, KeyHookProc, hInst, 0);
    if (hKeyHook == NULL)
    {
        // �t�b�N���s
    }
    else
    {
        // �t�b�N����
        g_hWnd = hWnd;
    }
    return 0;
}
EXPORT_API_ int ResetHook()
{
    if (UnhookWindowsHookEx(hKeyHook) != 0)
    {
        // �t�b�N��������
    }
    else
    {
        // �t�b�N�������s
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
        //�ړI�̃E�C���h�E�ɃL�[�{�[�h���b�Z�[�W�ƁA�L�[�R�[�h�̓]��

        // �ǂ��ŉ����Ă�HOOK����
        // �{�^���������ꂽ��Ԃ̎�����(�����̓X���[)
        if ((lp & 0x80000000) == 0)
        {
            // �ʏ�L�[
            if ((lp & 0x20000000) == 0)
            {
                // Enter�ȊO
                if (wp != VK_RETURN)
                {

                    now_char[num] = int(wp);
                    num++;

                    PostMessage(g_hWnd, WM_KEYDOWN, wp, 0);
                }
                else
                {//Enter��

                    ////����̂�
                    //if (f_try) {
                    //    memcpy(old_char, now_char, sizeof(now_char));
                    //    f_try = false;
                    //}

                    wchar_t string1[1000] = { 0 };
                    MultiByteToWideChar(0, 0, now_char, strlen(now_char), string1, strlen(now_char));
                    wchar_t string2[1000] = { 0 };
                    MultiByteToWideChar(0, 0, old_char, strlen(old_char), string2, strlen(old_char));

                    //����(EXE)�̃E�B���h�E�n���h���擾
                    HWND hWnd = FindWindow(NULL, _T("KyeChecker"));

                    if (strcmp(now_char, old_char) == 0) {//�����ꍇ
                        //MessageBox(NULL, _T("����"), NULL, MB_OK);
                        status = 0;
                        PostMessage(hWnd, WM_USER_RESULT, status, 0);
                    }
                    else {
                        //MessageBox(NULL, _T("�Ⴄ"), NULL, MB_OK);
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
            // �V�X�e���L�[(Alt(+����)�A��������F10�̎�)
            else
            {
                //MessageBox(NULL, TEXT("�V�X�e���L�[�������ꂽ��I"), NULL, MB_OK);
                PostMessage(g_hWnd, WM_SYSKEYDOWN, wp, 0);
            }
        }
    }
    return CallNextHookEx(hKeyHook, nCode, wp, lp);
}
// �G���g���|�C���g
BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        // �A�^�b�`
        hInst = hModule;
        break;
    case DLL_PROCESS_DETACH:
        // �f�^�b�`
        break;
    }
    return TRUE;
}