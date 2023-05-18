#pragma once
#include "afxwin.h"

#include "Renum.h"

// CPassword ダイアログ

class CPassword : public CDialog
{
	DECLARE_DYNAMIC(CPassword)

public:
	CPassword(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CPassword();

// ダイアログ データ
	enum { IDD = IDD_PASSWORD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	CEdit Edit_C_Pass;
	CString Edit_V_Pass;
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnClose();
};
