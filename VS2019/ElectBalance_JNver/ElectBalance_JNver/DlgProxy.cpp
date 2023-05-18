// DlgProxy.cpp : 実装ファイル
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
	
	// オートメーション オブジェクトがアクティブである限り、アプリケーションを 
	//	実行状態にしてください、コンストラクタは AfxOleLockApp を呼び出します。
	AfxOleLockApp();

	// アプリケーションのメイン ウィンドウ ポインタをとおしてダイアログ
	//  へアクセスします。プロキシの内部ポインタからダイアログへのポイ
	//  ンタを設定し、ダイアログの戻りポインタをこのプロキシへ設定しま
	//  す。
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
	// すべてのオブジェクトがオートメーションで作成された場合にアプリケーション
	// 	を終了するために、デストラクタが AfxOleUnlockApp を呼び出します。
	//  他の処理の間に、メイン ダイアログを破壊します。
	if (m_pDialog != NULL)
		m_pDialog->m_pAutoProxy = NULL;
	AfxOleUnlockApp();
}

void CElectBalance_JNverDlgAutoProxy::OnFinalRelease()
{
	// オートメーション オブジェクトに対する最後の参照が解放される時に
	// OnFinalRelease が呼び出されます。基本クラスは自動的にオブジェク
	// トを削除します。基本クラスを呼び出す前に、オブジェクトで必要な特
	// 別な後処理を追加してください。

	CCmdTarget::OnFinalRelease();
}

BEGIN_MESSAGE_MAP(CElectBalance_JNverDlgAutoProxy, CCmdTarget)
END_MESSAGE_MAP()

BEGIN_DISPATCH_MAP(CElectBalance_JNverDlgAutoProxy, CCmdTarget)
END_DISPATCH_MAP()

// メモ: VBA からタイプ セーフなバインドをサポートするために、IID_IElectBalance_JNver のサポートを追加します。
//  この IID は、.IDL ファイルのディスパッチ インターフェイスへアタッチされる 
//  GUID と一致しなければなりません。

// {E778F383-4D99-4684-8FCC-DB5EECA0C146}
static const IID IID_IElectBalance_JNver =
{ 0xE778F383, 0x4D99, 0x4684, { 0x8F, 0xCC, 0xDB, 0x5E, 0xEC, 0xA0, 0xC1, 0x46 } };

BEGIN_INTERFACE_MAP(CElectBalance_JNverDlgAutoProxy, CCmdTarget)
	INTERFACE_PART(CElectBalance_JNverDlgAutoProxy, IID_IElectBalance_JNver, Dispatch)
END_INTERFACE_MAP()

// IMPLEMENT_OLECREATE2 マクロが、このプロジェクトの StdAfx.h で定義されます。
// {0EAEC3DF-6495-4E6B-8C8E-B8997E31D9D4}
IMPLEMENT_OLECREATE2(CElectBalance_JNverDlgAutoProxy, "ElectBalance_JNver.Application", 0xeaec3df, 0x6495, 0x4e6b, 0x8c, 0x8e, 0xb8, 0x99, 0x7e, 0x31, 0xd9, 0xd4)


// CElectBalance_JNverDlgAutoProxy メッセージ ハンドラ
