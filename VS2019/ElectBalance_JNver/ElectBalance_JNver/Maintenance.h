#pragma once

#include "Mnt_Kishu.h"
#include "Mnt_Tanto.h"
#include "MntPassword.h"

// CMaintenance �_�C�A���O

class CMaintenance : public CDialog
{
	DECLARE_DYNAMIC(CMaintenance)

public:
	CMaintenance(CWnd* pParent = NULL);   // �W���R���X�g���N�^
	virtual ~CMaintenance();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_Maintenance };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

	DECLARE_MESSAGE_MAP()

	CMnt_Kishu Mnt_K;
	CMnt_Tanto Mnt_T;

public:
	afx_msg void OnBnClickedMntKishu();
	afx_msg void OnBnClickedMntTanto();
	afx_msg void OnBnClickedEnd();
	afx_msg void OnClose();
};
