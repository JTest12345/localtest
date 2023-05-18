// ElectBalance_JNverDlg.h : ヘッダー ファイル
//

#pragma once

class CElectBalance_JNverDlgAutoProxy;

#include "RS232CLIB.h"
#include "Jyushihaigou.h"
#include "Maintenance.h"
#include "afxwin.h"

#define SIZE 5000

// CElectBalance_JNverDlg ダイアログ
class CElectBalance_JNverDlg : public CDialog
{
	DECLARE_DYNAMIC(CElectBalance_JNverDlg);
	friend class CElectBalance_JNverDlgAutoProxy;

// コンストラクション
public:
	CElectBalance_JNverDlg(CWnd* pParent = NULL);	// 標準コンストラクタ
	virtual ~CElectBalance_JNverDlg();

//----------追加------------
	CToolTipCtrl m_tHint;

  HBRUSH m_BkBrush,m_BtmBrush;

//----------追加------------

// ダイアログ データ
	enum { IDD = IDD_ELECTBALANCE_JNVER_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV サポート


// 実装
protected:
	CElectBalance_JNverDlgAutoProxy* m_pAutoProxy;
	HICON m_hIcon;

	BOOL CanExit();

	// 生成された、メッセージ割り当て関数
	virtual BOOL OnInitDialog();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	afx_msg void OnClose();
	virtual void OnOK();
	virtual void OnCancel();
	DECLARE_MESSAGE_MAP()

	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

	public:
	RS232CLIB rs232c;
	CJyushihaigou Jshg;
	CMaintenance Mnt;
	Cmeasure msr;
	CJyushiCode JC;
	CPassword PW;
	CRenum RN;
	CMnt_Kishu Mnt_K;
	CMnt_Tanto Mnt_T;
	CMntPassword MntPass;

	int cnt;

	BOOL dlflag_Jyushihaigou;
	BOOL dlflag_Maintenance;
	BOOL dlflag_measure;
	BOOL dlflag_JyushiCode;
	BOOL dlflag_password;
	BOOL dlflag_renum;
	BOOL dlflag_Mnt_Kishu;
	BOOL dlflag_Mnt_Tanto;
	BOOL dlflag_MntPassword;
	CString Jyushi_List[SIZE][25];

	afx_msg void OnBnClickedJyushi();

	afx_msg void OnBnClickedMaintenance();
	afx_msg void OnBnClickedEnd();

	//----------追加------------	
	CButton Bn_to_Jshg;
	CButton Bn_to_Mnt;
	//----------追加------------

	//.iniの中身を格納
	CString Jyushidatacsv;
	CString MANcsv;
	CString Resultcsv;
	CString Historycsv;
	CString Timer_ms;
	CString gosa;
	CString COMPORT;

};
