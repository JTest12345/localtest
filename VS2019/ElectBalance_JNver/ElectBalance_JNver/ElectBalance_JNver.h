// ElectBalance_JNver.h : PROJECT_NAME �A�v���P�[�V�����̃��C�� �w�b�_�[ �t�@�C���ł��B
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH �ɑ΂��Ă��̃t�@�C�����C���N���[�h����O�� 'stdafx.h' ���C���N���[�h���Ă�������"
#endif

#include "resource.h"		// ���C�� �V���{��
#include <locale.h>

// CElectBalance_JNverApp:
// ���̃N���X�̎����ɂ��ẮAElectBalance_JNver.cpp ���Q�Ƃ��Ă��������B
//

class CElectBalance_JNverApp : public CWinApp
{
public:
	CElectBalance_JNverApp();

// �I�[�o�[���C�h
	public:
	virtual BOOL InitInstance();

// ����

	DECLARE_MESSAGE_MAP()
	virtual BOOL PreTranslateMessage(MSG* pMsg);
};

extern CElectBalance_JNverApp theApp;