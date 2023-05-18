#pragma once

#include "Mnt_Kishu.h"
#include "Mnt_Tanto.h"
#include "MntPassword.h"

// CMaintenance ダイアログ

class CMaintenance : public CDialog
{
	DECLARE_DYNAMIC(CMaintenance)

public:
	CMaintenance(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CMaintenance();

// ダイアログ データ
	enum { IDD = IDD_Maintenance };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()

	CMnt_Kishu Mnt_K;
	CMnt_Tanto Mnt_T;

public:
	afx_msg void OnBnClickedMntKishu();
	afx_msg void OnBnClickedMntTanto();
	afx_msg void OnBnClickedEnd();
	afx_msg void OnClose();
};
