// DlgProxy.h: �w�b�_�[ �t�@�C��
//

#pragma once

class CElectBalance_JNverDlg;


// CElectBalance_JNverDlgAutoProxy �R�}���h �^�[�Q�b�g

class CElectBalance_JNverDlgAutoProxy : public CCmdTarget
{
	DECLARE_DYNCREATE(CElectBalance_JNverDlgAutoProxy)

	CElectBalance_JNverDlgAutoProxy();           // ���I�����Ŏg�p����� protected �R���X�g���N�^

// ����
public:
	CElectBalance_JNverDlg* m_pDialog;

// ����
public:

// �I�[�o�[���C�h
	public:
	virtual void OnFinalRelease();

// ����
protected:
	virtual ~CElectBalance_JNverDlgAutoProxy();

	// �������ꂽ�A���b�Z�[�W���蓖�Ċ֐�

	DECLARE_MESSAGE_MAP()
	DECLARE_OLECREATE(CElectBalance_JNverDlgAutoProxy)

	// �������ꂽ OLE �f�B�X�p�b�`���蓖�Ċ֐�

	DECLARE_DISPATCH_MAP()
	DECLARE_INTERFACE_MAP()
};

