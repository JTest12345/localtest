#pragma once
#include "afxwin.h"


// CMnt_Tanto ダイアログ

class CMnt_Tanto : public CDialog
{
	DECLARE_DYNAMIC(CMnt_Tanto)

public:
	CMnt_Tanto(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CMnt_Tanto();

// ダイアログ データ
	enum { IDD = IDD_Mnt_tanto };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedBnMntTToroku();
	afx_msg void OnBnClickedBnMntTSakujo();
	CComboBox Cb_Mnt_T;
	CString Cb_Mnt_T_V;
	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedEnd();
	afx_msg void OnCbnSelchangeCbMntT();
	CString AllName[100];
	int AllNameCnt;

	afx_msg void OnClose();
};
