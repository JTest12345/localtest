#pragma once
#include "afxcmn.h"
#include "afxwin.h"

#include "JyushiCode.h"
#include "Password.h"
#include "Renum.h"

// Cmeasure ダイアログ

class Cmeasure : public CDialog
{
	DECLARE_DYNAMIC(Cmeasure)

public:
	Cmeasure(CWnd* pParent = NULL);   // 標準コンストラクタ
	virtual ~Cmeasure();

// ダイアログ データ
	enum { IDD = IDD_measure };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV サポート

//結果Disp用フォント
	CFont new_font;
	LOGFONT logfont;


	DECLARE_MESSAGE_MAP()


public:
	virtual BOOL OnInitDialog();

	CJyushiCode JyushiCode;

	int row_cnt;
	int col_cnt;
	CListCtrl m_listc;
	CString Edit_Measure_Kishu;
	CString msr_volume;
	CString msr_Sagyousha;

	CEdit Edit_C_msr_Kishu;
	CString Edit_V_msr_Keiryo;
	CEdit Edit_C_msr_Keiryo;
	afx_msg void OnBnClickedKstart();
	afx_msg void OnBnClickedKend();
	afx_msg void OnBnClickedEnd();
	CEdit Edit_C_msr_Sagyousha;
	CString Edit_V_msr_Sagyousha;
	int CheckBoxNum;
	int ItemNum;
	int FinishCnt;
	CButton Bn_msr_Kend;
	double Volume[10];//各剤の計測量を格納する配列
	double NowVolume;
	double Num;//主剤100gに対して、入力されたgの比率を格納
	CString ListC[10][10];//リストコントロール2次元配列
	CString CBVolumeData[10];

	afx_msg void OnBnClickedOk();
	afx_msg void OnBnClickedCancel();
	afx_msg void OnTimer(UINT_PTR nIDEvent);
	int AsciiNumReturn(int);
	CButton Bn_msr_KStart;
	int n;//COMポート
	char sbuf[128];//受信用

	unsigned char er;//データ受信確認用
	afx_msg void OnClose();
	CString Edit_Measure_WRank;
	CEdit Edit_C_msr_WRank;
	afx_msg void OnBnClickedRej();
	afx_msg void OnLvnItemchangedList1(NMHDR *pNMHDR, LRESULT *pResult);
	afx_msg HBRUSH OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor);

	double Keiryo;
	double RValue;
	double gosa;

};
