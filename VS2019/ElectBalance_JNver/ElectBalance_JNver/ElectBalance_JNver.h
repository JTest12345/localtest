// ElectBalance_JNver.h : PROJECT_NAME アプリケーションのメイン ヘッダー ファイルです。
//

#pragma once

#ifndef __AFXWIN_H__
	#error "PCH に対してこのファイルをインクルードする前に 'stdafx.h' をインクルードしてください"
#endif

#include "resource.h"		// メイン シンボル
#include <locale.h>

// CElectBalance_JNverApp:
// このクラスの実装については、ElectBalance_JNver.cpp を参照してください。
//

class CElectBalance_JNverApp : public CWinApp
{
public:
	CElectBalance_JNverApp();

// オーバーライド
	public:
	virtual BOOL InitInstance();

// 実装

	DECLARE_MESSAGE_MAP()
	virtual BOOL PreTranslateMessage(MSG* pMsg);
};

extern CElectBalance_JNverApp theApp;