#pragma once


// CMntPassword �_�C�A���O

class CMntPassword : public CDialog
{
	DECLARE_DYNAMIC(CMntPassword)

public:
	CMntPassword(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CMntPassword();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_Mnt_PASSWORD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()
public:
	afx_msg void OnEnChangeEdit1();
	CString Edit_V_MntPass;
	afx_msg void OnClose();
	int passok;
	virtual BOOL OnInitDialog();
};
