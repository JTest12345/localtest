#pragma once
#include "afxwin.h"



// CJyushiCode ダイアログ

class CJyushiCode : public CDialog
{
	DECLARE_DYNAMIC(CJyushiCode)

public:
	CJyushiCode(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CJyushiCode();

// ダイアログ データ
	enum { IDD = IDD_JyushiCode };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	virtual BOOL OnInitDialog();

	CEdit Edit_C_JC;
	CString Edit_V_JC;
	CString JyushiKata[10];
	CString JyushiLot[10];

	afx_msg void OnEnChangeEditJyushicode();
	int Num;
	//BOOL flg_same;
	afx_msg void OnClose();
	afx_msg void OnBnClickedButtonJcEnter();
};
