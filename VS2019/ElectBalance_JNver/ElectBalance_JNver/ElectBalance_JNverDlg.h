// ElectBalance_JNverDlg.h : �w�b�_�[ �t�@�C��
//

#pragma once

class CElectBalance_JNverDlgAutoProxy;

#include "RS232CLIB.h"
#include "Jyushihaigou.h"
#include "Maintenance.h"
#include "afxwin.h"

#define SIZE 5000

// CElectBalance_JNverDlg �_�C�A���O
class CElectBalance_JNverDlg : public CDialog
{
	DECLARE_DYNAMIC(CElectBalance_JNverDlg);
	friend class CElectBalance_JNverDlgAutoProxy;

// �R���X�g���N�V����
public:
	CElectBalance_JNverDlg(CWnd* pParent = NULL);	// �W���R���X�g���N�^
	virtual ~CElectBalance_JNverDlg();

//----------�ǉ�------------
	CToolTipCtrl m_tHint;

  HBRUSH m_BkBrush,m_BtmBrush;

//----------�ǉ�------------

// �_�C�A���O �f�[�^
	enum { IDD = IDD_ELECTBALANCE_JNVER_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// DDX/DDV �T�|�[�g


// ����
protected:
	CElectBalance_JNverDlgAutoProxy* m_pAutoProxy;
	HICON m_hIcon;

	BOOL CanExit();

	// �������ꂽ�A���b�Z�[�W���蓖�Ċ֐�
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

	//----------�ǉ�------------	
	CButton Bn_to_Jshg;
	CButton Bn_to_Mnt;
	//----------�ǉ�------------

	//.ini�̒��g���i�[
	CString Jyushidatacsv;
	CString MANcsv;
	CString Resultcsv;
	CString Historycsv;
	CString Timer_ms;
	CString gosa;
	CString COMPORT;

};
