#pragma once


// CRenum �_�C�A���O

class CRenum : public CDialog
{
	DECLARE_DYNAMIC(CRenum)

public:
	CRenum(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CRenum();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_RENUM };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()
public:
	CString Edit_V_rn;
	afx_msg void OnEnChangeEdit1();
	afx_msg void OnClose();
	afx_msg void OnBnClickedButtonExe();
};
