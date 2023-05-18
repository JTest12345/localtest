#pragma once
#include "afxwin.h"


// CMnt_Kishu ダイアログ

class CMnt_Kishu : public CDialog
{
	DECLARE_DYNAMIC(CMnt_Kishu)

public:
	CMnt_Kishu(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~CMnt_Kishu();

// ダイアログ データ
	enum { IDD = IDD_Mnt_Kishu };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnBnClickedBnMntKToroku();
	afx_msg void OnBnClickedBnMntKSakujo();
	CComboBox Cb_C_Mnt_K_KishuName;
	CString Cb_V_Mnt_K_KishuName;
	CComboBox Cb_C_Mnt_K_SZ_Name;
	CString Cb_V_Mnt_K_SZ_Name;
	CComboBox Cb_C_Mnt_K_SZ_Kata;
	CString Cb_V_Mnt_K_SZ_Kata;
	CComboBox Cb_C_Mnt_K_Other1_Name;
	CString Cb_V_Mnt_K_Other1_Name;
	CComboBox Cb_C_Mnt_K_Other2_Name;
	CString Cb_V_Mnt_K_Other2_Name;
	CComboBox Cb_C_Mnt_K_Other3_Name;
	CString Cb_V_Mnt_K_Other3_Name;
	CComboBox Cb_C_Mnt_K_Other4_Name;
	CString Cb_V_Mnt_K_Other4_Name;
	CComboBox Cb_C_Mnt_K_Other5_Name;
	CString Cb_V_Mnt_K_Other5_Name;
	CComboBox Cb_C_Mnt_K_Other1_Kata;
	CString Cb_V_Mnt_K_Other1_Kata;
	CComboBox Cb_C_Mnt_K_Other2_Kata;
	CString Cb_V_Mnt_K_Other2_Kata;
	CComboBox Cb_C_Mnt_K_Other3_Kata;
	CString Cb_V_Mnt_K_Other3_Kata;
	CComboBox Cb_C_Mnt_K_Other4_Kata;
	CString Cb_V_Mnt_K_Other4_Kata;
	CComboBox Cb_C_Mnt_K_Other5_Kata;
	CString Cb_V_Mnt_K_Other5_Kata;
	CString EDIT_K_Other1;
	CString EDIT_K_Other2;
	CString EDIT_K_Other3;
	CString EDIT_K_Other4;
	CString EDIT_K_Other5;
	virtual BOOL OnInitDialog();
	afx_msg void OnBnClickedEnd();
	afx_msg void OnCbnSelchangeCbMntKKishuname();
	CString AllName[5000];//機種名
	CString AllNameWRank[5000];//波長ランク
	int AllNameCnt;
	int AllNameWRankCnt;
	afx_msg void OnClose();
	CString EDIT_K_WRank_V;
	CComboBox Cb_C_Mnt_K_WRank;
	CString Cb_V_Mnt_K_WRank;
	CComboBox Cb_C_Mnt_K_Setubi;
	CString Cb_V_Mnt_K_Setubi;
	afx_msg void OnCbnEditchangeCbMntKKishuname();
	int changepass;
	CComboBox Cb_C_Mnt_K_Other6_Name;
	CString Cb_V_Mnt_K_Other6_Name;
	afx_msg void OnCbnSelchangeCbMntKOther6Name();
	CComboBox Cb_C_Mnt_K_Other7_Name;
	CString Cb_V_Mnt_K_Other7_Name;
	CComboBox Cb_C_Mnt_K_Other8_Name;
	CString Cb_V_Mnt_K_Other8_Name;
	CComboBox Cb_C_Mnt_K_Other9_Name;
	CString Cb_V_Mnt_K_Other9_Name;
	CComboBox Cb_C_Mnt_K_Other10_Name;
	CString Cb_V_Mnt_K_Other10_Name;
	CComboBox Cb_C_Mnt_K_Other6_Kata;
	CString Cb_V_Mnt_K_Other6_Kata;
	CComboBox Cb_C_Mnt_K_Other7_Kata;
	CString Cb_V_Mnt_K_Other7_Kata;
	CComboBox Cb_C_Mnt_K_Other8_Kata;
	CString Cb_V_Mnt_K_Other8_Kata;
	CComboBox Cb_C_Mnt_K_Other9_Kata;
	CString Cb_V_Mnt_K_Other9_Kata;
	CComboBox Cb_C_Mnt_K_Other10_Kata;
	CString Cb_V_Mnt_K_Other10_Kata;
	CString EDIT_K_Other6;
	CString EDIT_K_Other7;
	CString EDIT_K_Other8;
	CString EDIT_K_Other9;
	CString EDIT_K_Other10;
};
