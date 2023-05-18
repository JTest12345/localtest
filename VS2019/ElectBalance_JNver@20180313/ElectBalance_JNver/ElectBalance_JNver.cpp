// ElectBalance_JNver.cpp : �A�v���P�[�V�����̃N���X������`���܂��B
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


// CElectBalance_JNverApp �R���X�g���N�V����

CElectBalance_JNverApp::CElectBalance_JNverApp()
{
	// TODO: ���̈ʒu�ɍ\�z�p�R�[�h��ǉ����Ă��������B
	// ������ InitInstance ���̏d�v�ȏ��������������ׂċL�q���Ă��������B
}


// �B��� CElectBalance_JNverApp �I�u�W�F�N�g�ł��B

CElectBalance_JNverApp theApp;

const GUID CDECL BASED_CODE _tlid =
		{ 0x5AF63B85, 0xBA7A, 0x4D20, { 0x9B, 0x2B, 0x2D, 0x64, 0x3E, 0x43, 0xB6, 0xE9 } };
const WORD _wVerMajor = 1;
const WORD _wVerMinor = 0;


// CElectBalance_JNverApp ������

BOOL CElectBalance_JNverApp::InitInstance()
{
	// �A�v���P�[�V���� �}�j�t�F�X�g�� visual �X�^�C����L���ɂ��邽�߂ɁA
	// ComCtl32.dll Version 6 �ȍ~�̎g�p���w�肷��ꍇ�́A
	// Windows XP �� InitCommonControlsEx() ���K�v�ł��B�����Ȃ���΁A�E�B���h�E�쐬�͂��ׂĎ��s���܂��B
	INITCOMMONCONTROLSEX InitCtrls;
	InitCtrls.dwSize = sizeof(InitCtrls);
	// �A�v���P�[�V�����Ŏg�p���邷�ׂẴR���� �R���g���[�� �N���X���܂߂�ɂ́A
	// �����ݒ肵�܂��B
	InitCtrls.dwICC = ICC_WIN95_CLASSES;
	InitCommonControlsEx(&InitCtrls);

	CWinApp::InitInstance();

	// OLE ���C�u���������������܂��B
	if (!AfxOleInit())
	{
		AfxMessageBox(IDP_OLE_INIT_FAILED);
		return FALSE;
	}

	AfxEnableControlContainer();

	// �W��������
	// �����̋@�\���g�킸�ɍŏI�I�Ȏ��s�\�t�@�C����
	// �T�C�Y���k���������ꍇ�́A�ȉ�����s�v�ȏ�����
	// ���[�`�����폜���Ă��������B
	// �ݒ肪�i�[����Ă��郌�W�X�g�� �L�[��ύX���܂��B
	// TODO: ��Ж��܂��͑g�D���Ȃǂ̓K�؂ȕ������
	// ���̕������ύX���Ă��������B
	SetRegistryKey(_T("�A�v���P�[�V���� �E�B�U�[�h�Ő������ꂽ���[�J�� �A�v���P�[�V����"));
	// �I�[�g���[�V�����܂��� reg/unreg �X�C�b�`�̃R�}���h ���C������͂��܂��B
	CCommandLineInfo cmdInfo;
	ParseCommandLine(cmdInfo);

	// �A�v���P�[�V������ /Embedding �܂��� /Automation �X�C�b�`�ŋN������܂����B
	// �A�v���P�[�V�������I�[�g���[�V���� �T�[�o�[�Ƃ��Ď��s���܂��B
	if (cmdInfo.m_bRunEmbedded || cmdInfo.m_bRunAutomated)
	{
		// CoRegisterClassObject() �o�R�ŃN���X �t�@�N�g����o�^���܂��B
		COleTemplateServer::RegisterAll();
	}
	// �A�v���P�[�V������ /Unregserver �܂��� /Unregister �X�C�b�`�ŋN������܂����B
	// ���W�X�g������G���g�����폜���܂��B
	else if (cmdInfo.m_nShellCommand == CCommandLineInfo::AppUnregister)
	{
		COleObjectFactory::UpdateRegistryAll(FALSE);
		AfxOleUnregisterTypeLib(_tlid, _wVerMajor, _wVerMinor);
		return FALSE;
	}
	// �A�v���P�[�V�������X�^���h�A�����܂���/Register �� /Regserver �Ȃǂ̃X�C�b�`�ŋN������܂����B
	// �^�C�v ���C�u�������܂ރ��W�X�g�� �G���g�����X�V���܂��B
	else
	{
		COleObjectFactory::UpdateRegistryAll();
		AfxOleRegisterTypeLib(AfxGetInstanceHandle(), _tlid);
		if (cmdInfo.m_nShellCommand == CCommandLineInfo::AppRegister)
			return FALSE;
	}
	//���{��Ή�
	_tsetlocale(LC_ALL,_T(""));

	CElectBalance_JNverDlg dlg;
	m_pMainWnd = &dlg;
	INT_PTR nResponse = dlg.DoModal();
	if (nResponse == IDOK)
	{
		// TODO: �_�C�A���O�� <OK> �ŏ����ꂽ���̃R�[�h��
		//  �L�q���Ă��������B
	}
	else if (nResponse == IDCANCEL)
	{
		// TODO: �_�C�A���O�� <�L�����Z��> �ŏ����ꂽ���̃R�[�h��
		//  �L�q���Ă��������B
	}

	// �_�C�A���O�͕����܂����B�A�v���P�[�V�����̃��b�Z�[�W �|���v���J�n���Ȃ���
	//  �A�v���P�[�V�������I�����邽�߂� FALSE ��Ԃ��Ă��������B
	return FALSE;
}

BOOL CElectBalance_JNverApp::PreTranslateMessage(MSG* pMsg)
{
	// TODO: �����ɓ���ȃR�[�h��ǉ����邩�A�������͊�{�N���X���Ăяo���Ă��������B
    // ESC�L�[�̉����𖳌��ɂ���
    if( pMsg->message == WM_KEYDOWN ){
        if( pMsg->wParam == VK_ESCAPE ) return( TRUE );
    }
	//Enter�L�[�������̐ݒ�@20151116����
	/*if( pMsg->message == WM_KEYDOWN ){
        if( pMsg->wParam == VK_RETURN ) return( TRUE );
    }*/
	return CWinApp::PreTranslateMessage(pMsg);
}