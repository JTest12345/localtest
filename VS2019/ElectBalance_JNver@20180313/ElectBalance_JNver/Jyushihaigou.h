#pragma once

//#include "afxcmn.h"

#include "afxwin.h"
#include "measure.h"

// CJyushihaigou ダイアログ

class CJyushihaigou : public CDialog
{
	DECLARE_DYNAMIC(CJyushihaigou)

public:
	CJyushihaigou(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CJyushihaigou();

// ダイアログ データ
	enum { IDD = IDD_Jushihaigou };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()

	

public:

	Cmeasure msr;
	afx_msg void OnEnChangeEdit1();
	CString LotNo;
	CComboBox m_kishuCombo;
	virtual BOOL OnInitDialog();
	afx_msg void Start();
	afx_msg void OnEnChangeEdit2();
	CString e_MainVolume;
	CString Cb_J_Kishu;
	CComboBox Cb_C_J_Sagyousha;
	CString Cb_V_J_Sagyousha;
	afx_msg void OnBnClickedJEnd();
//	afx_msg void OnCbnSelchangeCombo1();
	afx_msg void OnCbnEditchangeCombo1();
	afx_msg void OnClose();
	CString W_rank_V;
	int name_cnt;
	afx_msg void OnEnChangeEdit3();
	CString Cb_V_J_WRank;
	CComboBox Cb_C_J_WRank;
	CComboBox m_setubiCombo;
	CString Cb_J_Setubi;
	int AlNum(void);
	int Tabcnt;
	int WRankCnt;
	afx_msg void OnCbnDropdownComboWrank();
//	afx_msg void OnCbnDropdownCombo1();
//	afx_msg void OnCbnSelchangeCombo1();
};
