#pragma once
#include "afxwin.h"



// CJyushiCode �_�C�A���O

class CJyushiCode : public CDialog
{
	DECLARE_DYNAMIC(CJyushiCode)

public:
	CJyushiCode(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CJyushiCode();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_JyushiCode };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

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
