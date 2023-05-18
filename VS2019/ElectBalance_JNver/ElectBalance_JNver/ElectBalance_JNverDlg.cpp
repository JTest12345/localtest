// ElectBalance_JNverDlg.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "ElectBalance_JNverDlg.h"
#include "DlgProxy.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// �A�v���P�[�V�����̃o�[�W�������Ɏg���� CAboutDlg �_�C�A���O

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// �_�C�A���O �f�[�^
	enum { IDD = IDD_ABOUTBOX };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV �T�|�[�g

// ����
protected:
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
END_MESSAGE_MAP()


// CElectBalance_JNverDlg �_�C�A���O




IMPLEMENT_DYNAMIC(CElectBalance_JNverDlg, CDialog);

CElectBalance_JNverDlg::CElectBalance_JNverDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CElectBalance_JNverDlg::IDD, pParent)
{
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
	m_pAutoProxy = NULL;
}

CElectBalance_JNverDlg::~CElectBalance_JNverDlg()
{
	// ���̃_�C�A���O�p�̃I�[�g���[�V���� �v���L�V������ꍇ�́A���̃_�C�A���O
	//  �ւ̃|�C���^�� NULL �ɖ߂��܂��A����ɂ���ă_�C�A���O���폜���ꂽ����
	//  ���킩��܂��B
	if (m_pAutoProxy != NULL)
		m_pAutoProxy->m_pDialog = NULL;
}

void CElectBalance_JNverDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
}

BEGIN_MESSAGE_MAP(CElectBalance_JNverDlg, CDialog)
	ON_WM_SYSCOMMAND()
	ON_WM_CLOSE()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	//}}AFX_MSG_MAP
	ON_BN_CLICKED(IDC_BUTTON2, &CElectBalance_JNverDlg::OnBnClickedJyushi)
	ON_BN_CLICKED(IDC_BUTTON1, &CElectBalance_JNverDlg::OnBnClickedMaintenance)
	ON_BN_CLICKED(IDC_End, &CElectBalance_JNverDlg::OnBnClickedEnd)
END_MESSAGE_MAP()


// CElectBalance_JNverDlg ���b�Z�[�W �n���h��

BOOL CElectBalance_JNverDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// "�o�[�W�������..." ���j���[���V�X�e�� ���j���[�ɒǉ����܂��B

	// IDM_ABOUTBOX �́A�V�X�e�� �R�}���h�͈͓̔��ɂȂ���΂Ȃ�܂���B
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// ���̃_�C�A���O�̃A�C�R����ݒ肵�܂��B�A�v���P�[�V�����̃��C�� �E�B���h�E���_�C�A���O�łȂ��ꍇ�A
	//  Framework �́A���̐ݒ�������I�ɍs���܂��B
	SetIcon(m_hIcon, TRUE);			// �傫���A�C�R���̐ݒ�
	SetIcon(m_hIcon, FALSE);		// �������A�C�R���̐ݒ�


	// TODO: �������������ɒǉ����܂��B
	if( CreateDirectory(_T("C:\\EB"), NULL )){}

	if( CreateDirectory(_T("C:\\EB\\data"), NULL )){}

	if( CreateDirectory(_T("C:\\EB\\exe"), NULL )){}

	if( CreateDirectory(_T("C:\\EB\\mst"), NULL )){}

	int ini_check = 0;
	int str_check = 0;
	int hst_check = 0;
	int cnt = 0;
	CFileFind find;
	CString ini = _T("C:\\EB\\mst\\EB.ini");
	CStdioFile fp;

	CString buf;
	CString tmp[25];
	CString str = _T("C:\\EB\\mst\\Jyushidata.csv");
	CString hst = _T("C:\\EB\\mst\\History.csv");
	HANDLE hFile;

	if (find.FindFile(str)){//jyushidata�����łɑ��݂��邩���ׂ�
	str_check = 1;
	}

	if (find.FindFile(hst)){//.ini�����łɑ��݂��邩���ׂ�
	hst_check = 1;
	}

	if (find.FindFile(ini)){//.ini�����łɑ��݂��邩���ׂ�
	ini_check = 1;
	}



	if(ini_check == 0){
		hFile = CreateFile(_T("C:\\EB\\mst\\EB.ini"), 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);

	//���������̏ꍇ�AEB.ini�t�@�C���̒��g���X�V
	
	if(fp.Open(ini, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(_T("C:\\EB\\mst\\Jyushidata.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("C:\\EB\\mst\\MAN.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("C:\\EB\\data\\Result.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("C:\\EB\\mst\\History.csv"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("100"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("0.5"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("1"));
				fp.WriteString(_T("\n\n\n\n\n\n"));
				fp.WriteString(_T("�@�@��A�g�����Ƃ̃f�[�^�t�@�C���̃p�X"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("�A��ƎҖ��̃f�[�^�t�@�C���̃p�X"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("�B���ʏo�̓t�@�C���̃p�X"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("�C�����̗��_�l�C�������t�@�C���̃p�X"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("�D�v�ʎ��̍X�V�p�x(ms)"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("�E�덷�͈�(g)"));
				fp.WriteString(_T("\n"));
				fp.WriteString(_T("�FCOM�|�[�g�̃i���o�["));
		fp.Close();
		}
	}

	//.ini�t�@�C���̒��g��ϐ��Ɋi�[
if(fp.Open(ini,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			if(cnt == 0)
				Jyushidatacsv = buf;
			else if(cnt == 1)
				MANcsv = buf;
			else if(cnt == 2)
				Resultcsv = buf;
			else if(cnt == 3)
				Historycsv = buf;
			else if(cnt == 4)
				Timer_ms = buf;
			else if(cnt == 5)
				gosa = buf;
			else if(cnt == 6)
				COMPORT = buf;
			else if(cnt >= 7)
				break;
		cnt++;
		}	
}									
	fp.Close();

str = Jyushidatacsv;

	//�t�@�C���̐���

	hFile = CreateFile(MANcsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);

hFile = CreateFile(Jyushidatacsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);

hFile = CreateFile(Resultcsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);

hFile = CreateFile(Historycsv, 
						GENERIC_READ | GENERIC_WRITE,
						FILE_SHARE_READ,
						NULL,
						OPEN_ALWAYS,
						FILE_ATTRIBUTE_NORMAL,
						NULL);

	if(hFile == INVALID_HANDLE_VALUE)
		AfxMessageBox(_T("CreateFile�֐������s���܂���"));

	//�n���h�������
	CloseHandle(hFile);



	//�_�C�A���O�̃t���O��������
	dlflag_Jyushihaigou = FALSE;
	dlflag_Maintenance = FALSE;
	dlflag_measure = FALSE;
	dlflag_JyushiCode = FALSE;
	dlflag_password = FALSE;
	dlflag_renum = FALSE;
	dlflag_Mnt_Kishu = FALSE;
	dlflag_Mnt_Tanto = FALSE;
	dlflag_MntPassword = FALSE;

	int i,j,a;
	//�������X�g��������
	for(i=0;i<SIZE;i++)
		for(j=0;j<25;j++)
			Jyushi_List[i][j] = _T("");

	//�������X�g��z��Ɋi�[



i=0;
if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,��؂�̃f�[�^��tmp�ɒ��o
			for(j=0; j<25; j++) tmp[j] = _T("");
			i++;
			for(j=0; j<25; j++){
				a = buf.Find(_T(","));
				if(buf.Compare(_T("endpoint")) == 0)
					break;
				if(a!=-1){
					tmp[j] = buf.Left(a);

					if(tmp[j].Compare(_T("endpoint")) == 0){
						buf.Delete(0, a+1);
						break;
					}
				
					Jyushi_List[i-1][j]=tmp[j];//�g�p���������i�[
					}
					buf.Delete(0, a+1);
					
					}

		}
}									

		fp.Close();

		if(str_check == 0){
		if(fp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(_T("�@�햼,"));
				fp.WriteString(_T("�g�������N,"));
				fp.WriteString(_T("�������@,"));
				fp.WriteString(_T("�����^�ԇ@,"));
				fp.WriteString(_T("�����ʇ@,"));
				fp.WriteString(_T("�������A,"));
				fp.WriteString(_T("�����^�ԇA,"));
				fp.WriteString(_T("�����ʇA,"));
				fp.WriteString(_T("�������B,"));
				fp.WriteString(_T("�����^�ԇB,"));
				fp.WriteString(_T("�����ʇB,"));
				fp.WriteString(_T("�������C,"));
				fp.WriteString(_T("�����^�ԇC,"));
				fp.WriteString(_T("�����ʇC,"));
				fp.WriteString(_T("�������D,"));
				fp.WriteString(_T("�����^�ԇD,"));
				fp.WriteString(_T("�����ʇD,"));
				fp.WriteString(_T("endpoint,"));
				fp.WriteString(_T("\n"));
				
			fp.Close();
			}
		}

		if(hst_check == 0){
		if(fp.Open(hst, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(_T("���t,"));
				fp.WriteString(_T("�@�햼,"));
				fp.WriteString(_T("�g�������N,"));
				fp.WriteString(_T("������,"));
				fp.WriteString(_T("�����^��,"));
				fp.WriteString(_T("�ύX�O������,"));
				fp.WriteString(_T("�ύX�������,"));
				fp.WriteString(_T("\n"));
				
			fp.Close();
			}
		}

		/*AfxMessageBox(Jyushidatacsv);
		AfxMessageBox(MANcsv);
		AfxMessageBox(Resultcsv);
		AfxMessageBox(Timer_ms);
		AfxMessageBox(gosa);*/

	return TRUE;  // �t�H�[�J�X���R���g���[���ɐݒ肵���ꍇ�������ATRUE ��Ԃ��܂��B
}

void CElectBalance_JNverDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// �_�C�A���O�ɍŏ����{�^����ǉ�����ꍇ�A�A�C�R����`�悷�邽�߂�
//  ���̃R�[�h���K�v�ł��B�h�L�������g/�r���[ ���f�����g�� MFC �A�v���P�[�V�����̏ꍇ�A
//  ����́AFramework �ɂ���Ď����I�ɐݒ肳��܂��B

void CElectBalance_JNverDlg::OnPaint()
{
	if (IsIconic())
	{
		CPaintDC dc(this); // �`��̃f�o�C�X �R���e�L�X�g

		SendMessage(WM_ICONERASEBKGND, reinterpret_cast<WPARAM>(dc.GetSafeHdc()), 0);

		// �N���C�A���g�̎l�p�`�̈���̒���
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// �A�C�R���̕`��
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// ���[�U�[���ŏ��������E�B���h�E���h���b�O���Ă���Ƃ��ɕ\������J�[�\�����擾���邽�߂ɁA
//  �V�X�e�������̊֐����Ăяo���܂��B
HCURSOR CElectBalance_JNverDlg::OnQueryDragIcon()
{
	return static_cast<HCURSOR>(m_hIcon);
}

// �R���g���[�����I�u�W�F�N�g�� 1 ���܂��ێ����Ă���ꍇ�A
//  �I�[�g���[�V�����T�[�o�[�̓��[�U�[�� UI �����ۂɏI����
//  ���܂���B�����̃��b�Z�[�W �n���h���̓v���L�V���܂��g�p��
//  ���ǂ������m�F���A���ꂩ�� UI ����\���ɂȂ�܂����_�C�A��
//  �O�͂��ꂪ�����ꂽ�ꍇ���̏ꏊ�Ɏc��܂��B

void CElectBalance_JNverDlg::OnClose()
{
	if (CanExit())
		CDialog::OnClose();
}

void CElectBalance_JNverDlg::OnOK()
{
	if (CanExit())
		CDialog::OnOK();
}

void CElectBalance_JNverDlg::OnCancel()
{
	if (CanExit())
		CDialog::OnCancel();
}

BOOL CElectBalance_JNverDlg::CanExit()
{
	// �v���L�V �I�u�W�F�N�g���܂��c���Ă���ꍇ�A�I�[�g���[�V����
	//  �R���g���[���͂��̃A�v���P�[�V�������܂��ێ����Ă��܂��B
	//  �_�C�A���O�̎��͎͂c���܂��� UI �͔�\���ɂȂ�܂��B
	if (m_pAutoProxy != NULL)
	{
		ShowWindow(SW_HIDE);
		return FALSE;
	}

	return TRUE;
}


void CElectBalance_JNverDlg::OnBnClickedJyushi()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B

	//m_flg_rankbin_search = TRUE;//Destroy����false�ɖ߂�
	//		dlg_rankbin_search.Create(IDD_DIALOG_GRAPHXBARR,GetDesktopWindow());
			//Jshg.Create(IDD_Jyushihaigou,this);
	//Jshg.DoModal();
	if(dlflag_Jyushihaigou == FALSE){//�N�����Ă��Ȃ����
		dlflag_Jyushihaigou = TRUE;
		Jshg.Create(IDD_Jushihaigou,this);
		Jshg.ShowWindow(SW_SHOWNA);//�\��
	}
	else{}//�N�����̏ꍇ

}

void CElectBalance_JNverDlg::OnBnClickedMaintenance()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	if(pWnd->dlflag_Maintenance == FALSE){//�N�����Ă��Ȃ����
	MntPass.DoModal();

	if(pWnd->MntPass.passok == 1){
		//pWnd->dlflag_MntPassword = FALSE;//�_�C�A���O�����̂�FALSE

		//if(pWnd->dlflag_Maintenance == FALSE){//�N�����Ă��Ȃ����
		pWnd->dlflag_Maintenance = TRUE;
		pWnd->Mnt.Create(IDD_Maintenance,this);
		pWnd->Mnt.ShowWindow(SW_SHOWNA);//�\��


		}
	}
	//if(dlflag_MntPassword == FALSE){//�N�����Ă��Ȃ����
	//dlflag_MntPassword = TRUE;	
	//MntPass.Create(IDD_Mnt_PASSWORD,this);
	//MntPass.ShowWindow(SW_SHOWNA);//�\��	
	//}

	//else{}//�N�����̏ꍇ

}

void CElectBalance_JNverDlg::OnBnClickedEnd()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	OnCancel();
}

HBRUSH CElectBalance_JNverDlg::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor) 
{
 
  // TODO: �f�t�H���g�̃u���V���]�݂̂��̂łȂ��ꍇ�ɂ́A�Ⴄ�u���V��Ԃ��Ă�������
  HBRUSH hbr = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
  

  //TRACE(&quot;OnControlColor\n&quot;);
  switch( nCtlColor )
  {
  case CTLCOLOR_DLG : // �_�C�A���O�̔w�i�F
  case CTLCOLOR_STATIC:
    //pDC-&gt;SetBkColor( RGB(100,152,193) );
    return m_BkBrush;
  case CTLCOLOR_BTN :
  case CTLCOLOR_EDIT:   
    return m_BtmBrush;
  default :
    return CDialog::OnCtlColor(pDC, pWnd, nCtlColor);
  }
  
  return hbr;
}
