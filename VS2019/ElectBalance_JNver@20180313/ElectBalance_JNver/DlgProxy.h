// DlgProxy.h: ヘッダー ファイル
//

#pragma once

class CElectBalance_JNverDlg;


// CElectBalance_JNverDlgAutoProxy コマンド ターゲット

class CElectBalance_JNverDlgAutoProxy : public CCmdTarget
{
	DECLARE_DYNCREATE(CElectBalance_JNverDlgAutoProxy)

	CElectBalance_JNverDlgAutoProxy();           // 動的生成で使用される protected コンストラクタ

// 属性
public:
	CElectBalance_JNverDlg* m_pDialog;

// 操作
public:

// オーバーライド
	public:
	virtual void OnFinalRelease();

// 実装
protected:
	virtual ~CElectBalance_JNverDlgAutoProxy();

	// 生成された、メッセージ割り当て関数

	DECLARE_MESSAGE_MAP()
	DECLARE_OLECREATE(CElectBalance_JNverDlgAutoProxy)

	// 生成された OLE ディスパッチ割り当て関数

	DECLARE_DISPATCH_MAP()
	DECLARE_INTERFACE_MAP()
};

