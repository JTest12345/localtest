#pragma once
#include "afxwin.h"

#include "Renum.h"

// CPassword �_�C�A���O

class CPassword : public CDialog
{
	DECLARE_DYNAMIC(CPassword)

public:
	CPassword(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CPassword();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_PASSWORD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()
public:
	CEdit Edit_C_Pass;
	CString Edit_V_Pass;
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnClose();
};
