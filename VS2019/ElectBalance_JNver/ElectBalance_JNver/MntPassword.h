#pragma once


// CMntPassword ダイアログ

class CMntPassword : public CDialog
{
	DECLARE_DYNAMIC(CMntPassword)

public:
	CMntPassword(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CMntPassword();

// ダイアログ データ
	enum { IDD = IDD_Mnt_PASSWORD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnEnChangeEdit1();
	CString Edit_V_MntPass;
	afx_msg void OnClose();
	int passok;
	virtual BOOL OnInitDialog();
};
