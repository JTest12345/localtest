#pragma once


// CRenum ダイアログ

class CRenum : public CDialog
{
	DECLARE_DYNAMIC(CRenum)

public:
	CRenum(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CRenum();

// ダイアログ データ
	enum { IDD = IDD_RENUM };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	CString Edit_V_rn;
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnClose();
	afx_msg void OnBnClickedButtonExe();
};
