// Mnt_Tanto.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Mnt_Tanto.h"

#include "ElectBalance_JNverDlg.h"

// CMnt_Tanto �_�C�A���O

IMPLEMENT_DYNAMIC(CMnt_Tanto, CDialog)

CMnt_Tanto::CMnt_Tanto(CWnd* pParent /*=NULL*/)
	: CDialog(CMnt_Tanto::IDD, pParent)
	, Cb_Mnt_T_V(_T(""))
{
	int i;
	//������
	for(i=0;i<100;i++)
		AllName[i]=_T("");
	AllNameCnt=0;


}

CMnt_Tanto::~CMnt_Tanto()
{
}

void CMnt_Tanto::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_Cb_Mnt_T, Cb_Mnt_T);
	DDX_CBString(pDX, IDC_Cb_Mnt_T, Cb_Mnt_T_V);
}


BEGIN_MESSAGE_MAP(CMnt_Tanto, CDialog)
	ON_BN_CLICKED(IDC_Bn_Mnt_T_Toroku, &CMnt_Tanto::OnBnClickedBnMntTToroku)
	ON_BN_CLICKED(IDC_Bn_Mnt_T_Sakujo, &CMnt_Tanto::OnBnClickedBnMntTSakujo)
	ON_BN_CLICKED(IDC_End, &CMnt_Tanto::OnBnClickedEnd)
	ON_CBN_SELCHANGE(IDC_Cb_Mnt_T, &CMnt_Tanto::OnCbnSelchangeCbMntT)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CMnt_Tanto ���b�Z�[�W �n���h��


void CMnt_Tanto::OnBnClickedBnMntTToroku()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
CString str = pWnd->MANcsv;

Cb_Mnt_T.GetWindowTextW(Cb_Mnt_T_V);

//�t�@�C���ǂݍ���
CStdioFile fp;
//�t�@�C���̏�������
CStdioFile ofp;

CString buf;
CString tmp[16];
int i,flag=0;

for(i=0;i<AllNameCnt;i++){
	if(AllName[i].Compare(Cb_Mnt_T_V) == 0){
		flag = 1;
		break;
	}
}
	if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){

		if(Cb_Mnt_T_V == "")
			AfxMessageBox(_T("�����͂ł�."));
		else if(flag == 1){
			AfxMessageBox(_T("���łɓo�^�ς݂ł�."));
		}
		//else if(MessageBox(_T("�o�^���Ă�낵���ł����H"),NULL,MB_OKCANCEL) == IDOK){
		else{
				ofp.SeekToEnd();
				
				ofp.WriteString(Cb_Mnt_T_V);
				ofp.WriteString(_T(","));
				ofp.WriteString(_T("\n"));
				Cb_Mnt_T.InsertString(-1, Cb_Mnt_T_V);//�ǉ��������ڂ����X�g�ɒǉ�

				AllName[i] = Cb_Mnt_T_V;//���X�g�֒ǉ�����
				AllNameCnt++;

				AfxMessageBox(_T("�o�^���������܂���."));					
		}
	}
	ofp.Close();
}

void CMnt_Tanto::OnBnClickedBnMntTSakujo()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
//CString str = _T("C:\\EB\\mst\\MAN.csv");
	CString str = pWnd->MANcsv;
CString newfile = _T("C:\\EB\\mst\\temp.csv");

Cb_Mnt_T.GetWindowTextW(Cb_Mnt_T_V);//�@�햼

//�t�@�C���ǂݍ���
CStdioFile fp;
//�t�@�C���̏�������
CStdioFile ofp;

CString buf;
CString tmp[4];
int a,i,j=0;
int NameCnt=0,cnt=0;


//�t�@�C���̐���
HANDLE hFile;

	hFile = CreateFile(newfile, 
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


	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		if(ofp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,��؂�̃f�[�^��tmp�ɒ��o	
			for(i=0; i<4; i++) tmp[i] = _T("");
			for(i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					if(Cb_Mnt_T_V.Compare(tmp[i]) == 0){
					//�폜���������ڂ̔ԍ����擾����DeleteString(i)�ŃR���{�{�b�N�X�̃v���_�E�����X�V������
					Cb_Mnt_T.DeleteString(cnt);
					for(i=cnt;i<AllNameCnt;i++){
						
						AllName[i] = AllName[i+1];//�Y���ӏ�������������
					
					}
					AllNameCnt--;
					}

					else if(Cb_Mnt_T_V.Compare(tmp[i]) != 0){ //|| NameCnt == 1){//���̂ݏ������݂��΂�

					ofp.SeekToEnd();
					ofp.WriteString(tmp[i]);
					ofp.WriteString(_T(","));
					ofp.WriteString(_T("\n"));
					cnt++;
					}
					//���̂ݏ������݂��΂�
					//if(Cb_Mnt_T_V.Compare(tmp[i]) == 0)NameCnt = 1;

					//cnt++;
					break;

				}
				buf.Delete(0, a+1);
			}
		}
	}
	}
		ofp.Close();
		fp.Close();

		DeleteFile(str);
		MoveFile(newfile,str);
		//CFile::Rename(newfile,str);
		//DeleteFile(str);

		AfxMessageBox(_T("�폜���������܂���."));
	
}

BOOL CMnt_Tanto::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
CString str = _T("C:\\EB\\mst\\MAN.csv");

//�t�@�C���ǂݍ���
CStdioFile fp;

CString buf;
CString tmp[4];
int a,i;

	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){
			//,��؂�̃f�[�^��tmp�ɒ��o	
			for(i=0; i<4; i++) tmp[i] = _T("");
			for(i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					Cb_Mnt_T.InsertString(-1, tmp[i]);//���X�g�֒ǉ�		
					AllName[AllNameCnt] = tmp[i];//���X�g�ɒS���Җ����i�[
					AllNameCnt++;
				}
					break;
				}
				buf.Delete(0, a+1);
			}
	}
		fp.Close();

	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CMnt_Tanto::OnBnClickedEnd()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	OnClose();
}

void CMnt_Tanto::OnCbnSelchangeCbMntT()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
}

void CMnt_Tanto::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_Mnt_Tanto = FALSE;
	DestroyWindow();

	CDialog::OnClose();
}
