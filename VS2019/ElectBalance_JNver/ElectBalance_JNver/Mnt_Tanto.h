#pragma once
#include "afxwin.h"


// CMnt_Tanto �_�C�A���O

class CMnt_Tanto : public CDialog
{
	DECLARE_DYNAMIC(CMnt_Tanto)

public:
	CMnt_Tanto(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CMnt_Tanto();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_Mnt_tanto };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

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
