// DlgProxy.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "DlgProxy.h"
#include "ElectBalance_JNverDlg.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CElectBalance_JNverDlgAutoProxy

IMPLEMENT_DYNCREATE(CElectBalance_JNverDlgAutoProxy, CCmdTarget)

CElectBalance_JNverDlgAutoProxy::CElectBalance_JNverDlgAutoProxy()
{
	EnableAutomation();
	
	// �I�[�g���[�V���� �I�u�W�F�N�g���A�N�e�B�u�ł������A�A�v���P�[�V������ 
	//	���s��Ԃɂ��Ă��������A�R���X�g���N�^�� AfxOleLockApp ���Ăяo���܂��B
	AfxOleLockApp();

	// �A�v���P�[�V�����̃��C�� �E�B���h�E �|�C���^���Ƃ����ă_�C�A���O
	//  �փA�N�Z�X���܂��B�v���L�V�̓����|�C���^����_�C�A���O�ւ̃|�C
	//  ���^��ݒ肵�A�_�C�A���O�̖߂�|�C���^�����̃v���L�V�֐ݒ肵��
	//  ���B
	ASSERT_VALID(AfxGetApp()->m_pMainWnd);
	if (AfxGetApp()->m_pMainWnd)
	{
		ASSERT_KINDOF(CElectBalance_JNverDlg, AfxGetApp()->m_pMainWnd);
		if (AfxGetApp()->m_pMainWnd->IsKindOf(RUNTIME_CLASS(CElectBalance_JNverDlg)))
		{
			m_pDialog = reinterpret_cast<CElectBalance_JNverDlg*>(AfxGetApp()->m_pMainWnd);
			m_pDialog->m_pAutoProxy = this;
		}
	}
}

CElectBalance_JNverDlgAutoProxy::~CElectBalance_JNverDlgAutoProxy()
{
	// ���ׂẴI�u�W�F�N�g���I�[�g���[�V�����ō쐬���ꂽ�ꍇ�ɃA�v���P�[�V����
	// 	���I�����邽�߂ɁA�f�X�g���N�^�� AfxOleUnlockApp ���Ăяo���܂��B
	//  ���̏����̊ԂɁA���C�� �_�C�A���O��j�󂵂܂��B
	if (m_pDialog != NULL)
		m_pDialog->m_pAutoProxy = NULL;
	AfxOleUnlockApp();
}

void CElectBalance_JNverDlgAutoProxy::OnFinalRelease()
{
	// �I�[�g���[�V���� �I�u�W�F�N�g�ɑ΂���Ō�̎Q�Ƃ��������鎞��
	// OnFinalRelease ���Ăяo����܂��B��{�N���X�͎����I�ɃI�u�W�F�N
	// �g���폜���܂��B��{�N���X���Ăяo���O�ɁA�I�u�W�F�N�g�ŕK�v�ȓ�
	// �ʂȌ㏈����ǉ����Ă��������B

	CCmdTarget::OnFinalRelease();
}

BEGIN_MESSAGE_MAP(CElectBalance_JNverDlgAutoProxy, CCmdTarget)
END_MESSAGE_MAP()

BEGIN_DISPATCH_MAP(CElectBalance_JNverDlgAutoProxy, CCmdTarget)
END_DISPATCH_MAP()

// ����: VBA ����^�C�v �Z�[�t�ȃo�C���h���T�|�[�g���邽�߂ɁAIID_IElectBalance_JNver �̃T�|�[�g��ǉ����܂��B
//  ���� IID �́A.IDL �t�@�C���̃f�B�X�p�b�` �C���^�[�t�F�C�X�փA�^�b�`����� 
//  GUID �ƈ�v���Ȃ���΂Ȃ�܂���B

// {E778F383-4D99-4684-8FCC-DB5EECA0C146}
static const IID IID_IElectBalance_JNver =
{ 0xE778F383, 0x4D99, 0x4684, { 0x8F, 0xCC, 0xDB, 0x5E, 0xEC, 0xA0, 0xC1, 0x46 } };

BEGIN_INTERFACE_MAP(CElectBalance_JNverDlgAutoProxy, CCmdTarget)
	INTERFACE_PART(CElectBalance_JNverDlgAutoProxy, IID_IElectBalance_JNver, Dispatch)
END_INTERFACE_MAP()

// IMPLEMENT_OLECREATE2 �}�N�����A���̃v���W�F�N�g�� StdAfx.h �Œ�`����܂��B
// {0EAEC3DF-6495-4E6B-8C8E-B8997E31D9D4}
IMPLEMENT_OLECREATE2(CElectBalance_JNverDlgAutoProxy, "ElectBalance_JNver.Application", 0xeaec3df, 0x6495, 0x4e6b, 0x8c, 0x8e, 0xb8, 0x99, 0x7e, 0x31, 0xd9, 0xd4)


// CElectBalance_JNverDlgAutoProxy ���b�Z�[�W �n���h��
