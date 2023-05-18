// Mnt_Kishu.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Mnt_Kishu.h"

#include "ElectBalance_JNverDlg.h"

// CMnt_Kishu �_�C�A���O

IMPLEMENT_DYNAMIC(CMnt_Kishu, CDialog)

CMnt_Kishu::CMnt_Kishu(CWnd* pParent /*=NULL*/)
	: CDialog(CMnt_Kishu::IDD, pParent)
	, Cb_V_Mnt_K_KishuName(_T(""))
	, Cb_V_Mnt_K_SZ_Name(_T(""))
	, Cb_V_Mnt_K_SZ_Kata(_T(""))
	, Cb_V_Mnt_K_Other1_Name(_T(""))
	, Cb_V_Mnt_K_Other2_Name(_T(""))
	, Cb_V_Mnt_K_Other3_Name(_T(""))
	, Cb_V_Mnt_K_Other4_Name(_T(""))
	, Cb_V_Mnt_K_Other5_Name(_T(""))
	, Cb_V_Mnt_K_Other1_Kata(_T(""))
	, Cb_V_Mnt_K_Other2_Kata(_T(""))
	, Cb_V_Mnt_K_Other3_Kata(_T(""))
	, Cb_V_Mnt_K_Other4_Kata(_T(""))
	, Cb_V_Mnt_K_Other5_Kata(_T(""))
	, EDIT_K_Other1(_T(""))
	, EDIT_K_Other2(_T(""))
	, EDIT_K_Other3(_T(""))
	, EDIT_K_Other4(_T(""))
	, EDIT_K_Other5(_T(""))
	, EDIT_K_WRank_V(_T(""))
{
	
	int i;
	//������
	for(i=0;i<5000;i++)
		AllName[i]=_T("");
	AllNameCnt=0;


}

CMnt_Kishu::~CMnt_Kishu()
{
}

void CMnt_Kishu::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_Cb_Mnt_K_KishuName, Cb_C_Mnt_K_KishuName);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_KishuName, Cb_V_Mnt_K_KishuName);
	DDX_Control(pDX, IDC_Cb_Mnt_K_SZ_Name, Cb_C_Mnt_K_SZ_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_SZ_Name, Cb_V_Mnt_K_SZ_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_SZ_Kata, Cb_C_Mnt_K_SZ_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_SZ_Kata, Cb_V_Mnt_K_SZ_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other1_Name, Cb_C_Mnt_K_Other1_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other1_Name, Cb_V_Mnt_K_Other1_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other2_Name, Cb_C_Mnt_K_Other2_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other2_Name, Cb_V_Mnt_K_Other2_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other3_Name, Cb_C_Mnt_K_Other3_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other3_Name, Cb_V_Mnt_K_Other3_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other4_Name, Cb_C_Mnt_K_Other4_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other4_Name, Cb_V_Mnt_K_Other4_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other5_Name, Cb_C_Mnt_K_Other5_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other5_Name, Cb_V_Mnt_K_Other5_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other1_Kata, Cb_C_Mnt_K_Other1_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other1_Kata, Cb_V_Mnt_K_Other1_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other2_Kata, Cb_C_Mnt_K_Other2_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other2_Kata, Cb_V_Mnt_K_Other2_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other3_Kata, Cb_C_Mnt_K_Other3_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other3_Kata, Cb_V_Mnt_K_Other3_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other4_Kata, Cb_C_Mnt_K_Other4_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other4_Kata, Cb_V_Mnt_K_Other4_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other5_Kata, Cb_C_Mnt_K_Other5_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other5_Kata, Cb_V_Mnt_K_Other5_Kata);
	DDX_Text(pDX, IDC_EDIT_K_Other1, EDIT_K_Other1);
	DDX_Text(pDX, IDC_EDIT_K_Other2, EDIT_K_Other2);
	DDX_Text(pDX, IDC_EDIT_K_Other3, EDIT_K_Other3);
	DDX_Text(pDX, IDC_EDIT_K_Other4, EDIT_K_Other4);
	DDX_Text(pDX, IDC_EDIT_K_Other5, EDIT_K_Other5);
	DDX_Text(pDX, IDC_EDIT_K_WRank, EDIT_K_WRank_V);
}


BEGIN_MESSAGE_MAP(CMnt_Kishu, CDialog)
	ON_BN_CLICKED(IDC_Bn_Mnt_K_Toroku, &CMnt_Kishu::OnBnClickedBnMntKToroku)
	ON_BN_CLICKED(IDC_Bn_Mnt_K_Sakujo, &CMnt_Kishu::OnBnClickedBnMntKSakujo)
	ON_BN_CLICKED(IDC_End, &CMnt_Kishu::OnBnClickedEnd)
	ON_CBN_SELCHANGE(IDC_Cb_Mnt_K_KishuName, &CMnt_Kishu::OnCbnSelchangeCbMntKKishuname)
	ON_WM_CLOSE()
END_MESSAGE_MAP()


// CMnt_Kishu ���b�Z�[�W �n���h��

void CMnt_Kishu::OnBnClickedBnMntKToroku()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	CString str = pWnd->Jyushidatacsv;

	int KishuName=0,WRank=0,SZ=0,Other1=0,Other2=0,Other3=0,Other4=0,Other5=0;
	int flag = 0,i,j;
	int bothsame=0;
	

	//Edit����ϐ���
	UpdateData(TRUE);//�g�������N�A�ܗ�

	
	for(i=0;i<AllNameCnt;i++){
	if(AllName[i].Compare(Cb_V_Mnt_K_KishuName) == 0){
		flag = 2;//�ς���
		break;
		}
	}


	//�R���{�{�b�N�X�ɓ��͂���Ă���l��Value�ϐ��Ɋi�[
	Cb_C_Mnt_K_KishuName.GetWindowTextW(Cb_V_Mnt_K_KishuName);//�@�햼

	Cb_C_Mnt_K_SZ_Name.GetWindowTextW(Cb_V_Mnt_K_SZ_Name);//��܂̎�����
	Cb_C_Mnt_K_SZ_Kata.GetWindowTextW(Cb_V_Mnt_K_SZ_Kata);//��܂̎����^��

	Cb_C_Mnt_K_Other1_Name.GetWindowTextW(Cb_V_Mnt_K_Other1_Name);//��܈ȊO�̎������@
	Cb_C_Mnt_K_Other1_Kata.GetWindowTextW(Cb_V_Mnt_K_Other1_Kata);//��܈ȊO�̎����^�ԇ@

	Cb_C_Mnt_K_Other2_Name.GetWindowTextW(Cb_V_Mnt_K_Other2_Name);//��܈ȊO�̎������A
	Cb_C_Mnt_K_Other2_Kata.GetWindowTextW(Cb_V_Mnt_K_Other2_Kata);//��܈ȊO�̎����^�ԇA

	Cb_C_Mnt_K_Other3_Name.GetWindowTextW(Cb_V_Mnt_K_Other3_Name);//��܈ȊO�̎������B
	Cb_C_Mnt_K_Other3_Kata.GetWindowTextW(Cb_V_Mnt_K_Other3_Kata);//��܈ȊO�̎����^�ԇB

	Cb_C_Mnt_K_Other4_Name.GetWindowTextW(Cb_V_Mnt_K_Other4_Name);//��܈ȊO�̎������C
	Cb_C_Mnt_K_Other4_Kata.GetWindowTextW(Cb_V_Mnt_K_Other4_Kata);//��܈ȊO�̎����^�ԇC

	Cb_C_Mnt_K_Other5_Name.GetWindowTextW(Cb_V_Mnt_K_Other5_Name);//��܈ȊO�̎������D
	Cb_C_Mnt_K_Other5_Kata.GetWindowTextW(Cb_V_Mnt_K_Other5_Kata);//��܈ȊO�̎����^�ԇD

	if(Cb_V_Mnt_K_KishuName != "")KishuName++;
	if(EDIT_K_WRank_V != "")WRank++;
	if(Cb_V_Mnt_K_SZ_Name != "")SZ++;
	if(Cb_V_Mnt_K_SZ_Kata != "")SZ++;
	if(Cb_V_Mnt_K_Other1_Name != "")Other1++;
	if(Cb_V_Mnt_K_Other1_Kata != "")Other1++;
	if(EDIT_K_Other1 != "")Other1++;
	if(Cb_V_Mnt_K_Other2_Name != "")Other2++;
	if(Cb_V_Mnt_K_Other2_Kata != "")Other2++;
	if(EDIT_K_Other2 != "")Other2++;
	if(Cb_V_Mnt_K_Other3_Name != "")Other3++;
	if(Cb_V_Mnt_K_Other3_Kata != "")Other3++;
	if(EDIT_K_Other3 != "")Other3++;
	if(Cb_V_Mnt_K_Other4_Name != "")Other4++;
	if(Cb_V_Mnt_K_Other4_Kata != "")Other4++;
	if(EDIT_K_Other4 != "")Other4++;
	if(Cb_V_Mnt_K_Other5_Name != "")Other5++;
	if(Cb_V_Mnt_K_Other5_Kata != "")Other5++;
	if(EDIT_K_Other5 != "")Other5++;


//�t�@�C���̏�������
CStdioFile ofp;

CString buf;
CString tmp[16];

i=0;j=0;
	//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Cb_V_Mnt_K_KishuName) == 0 && pWnd->Jyushi_List[i][1].Compare(EDIT_K_WRank_V) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T(""))break;
	}

	if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
				ofp.SeekToEnd();
				//��s�̒��Ŕ��[�ȓ��͂��󂯕t���Ȃ�
				if((KishuName == 0 || KishuName == 1) && (WRank == 0 || WRank == 1) && (SZ == 2 || SZ == 0) && (Other1 == 3 || Other1 == 0) && (Other2 == 3 || Other2 == 0) && (Other3 == 3 || Other3 == 0) && (Other4 == 3 || Other4 == 0) && (Other5 == 3 || Other5 == 0)){
					
					if(KishuName == 0 && WRank == 0 && SZ == 0 && Other1 == 0 && Other2 == 0 && Other3 == 0 && Other4 == 0 && Other5 == 0)
					AfxMessageBox(_T("�����͂ł�."));

					else if(KishuName == 0)
					AfxMessageBox(_T("�@�햼�������͂ł�."));

					else if(bothsame == 1){
					AfxMessageBox(_T("���łɓo�^�ς݂ł�."));
					}

					else{//�����͂łȂ����
						

					if(KishuName == 1){
					ofp.WriteString(Cb_V_Mnt_K_KishuName);
					if(WRank == 0)ofp.WriteString(_T(",endpoint,\n"));//���̃{�b�N�X�ɉ��������Ă��Ȃ����
					else ofp.WriteString(_T(","));
					
					Cb_C_Mnt_K_KishuName.InsertString(-1, Cb_V_Mnt_K_KishuName);//CB���X�g�֒ǉ�
					AllName[i] = Cb_V_Mnt_K_KishuName;//���X�g�֒ǉ�����
					AllNameCnt++;//�z��𑝂₷

					pWnd->Jyushi_List[i][0] = Cb_V_Mnt_K_KishuName;

					}


					if(WRank == 1){//�{�b�N�X�ɒl�������Ă����Ƃ�
				ofp.WriteString(EDIT_K_WRank_V);
				AllNameWRank[i] = EDIT_K_WRank_V;//���X�g�֒ǉ�����
				AllNameWRankCnt++;//�z��𑝂₷
				pWnd->Jyushi_List[i][1] = EDIT_K_WRank_V;

				if(SZ == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}


					if(SZ == 2){//�S�Ẵ{�b�N�X�ɒl�������Ă����Ƃ�
				ofp.WriteString(Cb_V_Mnt_K_SZ_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_SZ_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(_T("100"));
				//�z��Ɋi�[
				pWnd->Jyushi_List[i][2] = Cb_V_Mnt_K_SZ_Name;
				pWnd->Jyushi_List[i][3] = Cb_V_Mnt_K_SZ_Kata;
				pWnd->Jyushi_List[i][4] = _T("100");

				if(Other1 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other1 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other1_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other1_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other1);

				pWnd->Jyushi_List[i][5] = Cb_V_Mnt_K_Other1_Name;
				pWnd->Jyushi_List[i][6] = Cb_V_Mnt_K_Other1_Kata;
				pWnd->Jyushi_List[i][7] = EDIT_K_Other1;

				if(Other2 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
				
					}

					if(Other2 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other2_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other2_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other2);

				pWnd->Jyushi_List[i][8] = Cb_V_Mnt_K_Other2_Name;
				pWnd->Jyushi_List[i][9] = Cb_V_Mnt_K_Other2_Kata;
				pWnd->Jyushi_List[i][10] = EDIT_K_Other2;

				if(Other3 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other3 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other3_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other3_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other3);

				pWnd->Jyushi_List[i][11] = Cb_V_Mnt_K_Other3_Name;
				pWnd->Jyushi_List[i][12] = Cb_V_Mnt_K_Other3_Kata;
				pWnd->Jyushi_List[i][13] = EDIT_K_Other3;


				if(Other4 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other4 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other4_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other4_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other4);

				pWnd->Jyushi_List[i][14] = Cb_V_Mnt_K_Other4_Name;
				pWnd->Jyushi_List[i][15] = Cb_V_Mnt_K_Other4_Kata;
				pWnd->Jyushi_List[i][16] = EDIT_K_Other4;

				if(Other5 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other5 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other5_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other5_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other5);

				pWnd->Jyushi_List[i][17] = Cb_V_Mnt_K_Other5_Name;
				pWnd->Jyushi_List[i][18] = Cb_V_Mnt_K_Other5_Kata;
				pWnd->Jyushi_List[i][19] = EDIT_K_Other5;

				ofp.WriteString(_T(",endpoint,\n"));

				//ofp.WriteString(_T("\n"));
					}
				AfxMessageBox(_T("�o�^���������܂���."));
					}
				}
				else
					AfxMessageBox(_T("�����͕���������܂�."));
	}
	ofp.Close();

	UpdateData(FALSE);

}

void CMnt_Kishu::OnBnClickedBnMntKSakujo()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	CString str = pWnd->Jyushidatacsv;

CString newfile = _T("C:\\EB\\mst\\temp.csv");

Cb_C_Mnt_K_KishuName.GetWindowTextW(Cb_V_Mnt_K_KishuName);


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



//�t�@�C���ǂݍ���
CStdioFile fp;
//�t�@�C���̏�������
CStdioFile ofp;

CString buf;
CString prebuf;
CString tmp[4];
int a,next,i=0,j,point;
int NameCnt=0,cnt=0,deletecnt=0,KishuSame=0,bothsame = 0;


//�g�������N��ϐ��Ɋi�[
UpdateData(TRUE);

j=0;
	//��v���Ă�����̂�T��
	for(next=0;next<SIZE;next++){
		if(pWnd->Jyushi_List[next][0].Compare(Cb_V_Mnt_K_KishuName) == 0 && pWnd->Jyushi_List[next][1].Compare(EDIT_K_WRank_V) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[next][0] == _T(""))break;
	}

	point = next;
	//remove����ӏ��Ɏ��̔z��̔ԍ�����.�����(next+1)�Ԗڂ̋@�킪�z���ɂ�2���݂��邱�ƂɂȂ�.
	if(bothsame == 1){
		//while(pWnd->Jyushi_List[next+1][0] != _T("")){
		while(pWnd->Jyushi_List[next][0] != _T("")){
			for(i=0;i<20;i++)
				pWnd->Jyushi_List[next][i] = pWnd->Jyushi_List[next+1][i];
			next++;
		}
		//next++;	
		//}
	}

	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		if(ofp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){

			if(bothsame == 1 && cnt != point){
			
			//,��؂�̃f�[�^��tmp�ɒ��o
			for(int i=0; i<4; i++) tmp[i] = _T("");

			for(int i=0; i<4; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

					//if(Cb_V_Mnt_K_KishuName.Compare(tmp[i]) == 0){
					////�폜���������ڂ̔ԍ����擾����DeleteString(i)�ŃR���{�{�b�N�X�̃v���_�E�����X�V������
					////Cb_C_Mnt_K_KishuName.DeleteString(cnt);
					//Cb_C_Mnt_K_KishuName.DeleteString(point-1);

					//}		
						ofp.SeekToEnd();		
						ofp.WriteString(buf);//��s�S�ď�������
						ofp.WriteString(_T("\n"));//�Ō�ɉ��s������
						break;
					}
			}//buf����,��������if��
				buf.Delete(0, a+1);
			
			}//For����
			else if(bothsame == 1 && cnt == point){
				Cb_C_Mnt_K_KishuName.DeleteString(point-1);
			}

				cnt++;
		}//�폜�Ώۂ̋@�킪���邩�ǂ���

			//else if(cnt == 0){ 
			//	//AfxMessageBox(_T("�����͂ł�."));
			//	//cnt++;
			//}
		}
	}
	
		ofp.Close();
		fp.Close();

		//if(Cb_V_Mnt_K_KishuName != _T("") && NameCnt == 1){//�Y���@�킪����������
		if(bothsame >0){//�Y���@�킪����������
		DeleteFile(str);
		MoveFile(newfile,str);
		AfxMessageBox(_T("�폜���������܂���."));
		}
		//else if(NameCnt == 0 && cnt == 0){//�Y���@�킪������Ȃ�������
		else if(bothsame == 0){//�Y���@�킪������Ȃ�������
		/*DeleteFile(str);
		MoveFile(newfile,str);*/
		DeleteFile(newfile);
		AfxMessageBox(_T("�Y���@�킪������܂���."));
		}
}

BOOL CMnt_Kishu::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	
	int i;

	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
		Cb_C_Mnt_K_KishuName.InsertString(-1, pWnd->Jyushi_List[i][0]);
	}

Cb_V_Mnt_K_SZ_Name = "B��";
Cb_V_Mnt_K_Other1_Name = "A��";
Cb_V_Mnt_K_Other2_Name = "�t�B���[";
Cb_V_Mnt_K_Other3_Name = "�u����";

//�ϐ�����Edit��
UpdateData(FALSE);

	return TRUE;  // return TRUE unless you set the focus to a control
	// ��O : OCX �v���p�e�B �y�[�W�͕K�� FALSE ��Ԃ��܂��B
}

void CMnt_Kishu::OnBnClickedEnd()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	OnClose();
}

void CMnt_Kishu::OnCbnSelchangeCbMntKKishuname()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	//�@���I���������ɁA�e�p�����[�^���G�f�B�b�g�{�b�N�X�֕\�������鏈�����L�q
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int BoxNum=0;

	BoxNum = Cb_C_Mnt_K_KishuName.GetCurSel();

	Cb_V_Mnt_K_KishuName = pWnd->Jyushi_List[BoxNum+1][0];
	EDIT_K_WRank_V = pWnd->Jyushi_List[BoxNum+1][1];

	Cb_V_Mnt_K_SZ_Name = pWnd->Jyushi_List[BoxNum+1][2];
	Cb_V_Mnt_K_SZ_Kata = pWnd->Jyushi_List[BoxNum+1][3];

	Cb_V_Mnt_K_Other1_Name = pWnd->Jyushi_List[BoxNum+1][5];
	Cb_V_Mnt_K_Other1_Kata = pWnd->Jyushi_List[BoxNum+1][6];
	EDIT_K_Other1 = pWnd->Jyushi_List[BoxNum+1][7];

	Cb_V_Mnt_K_Other2_Name = pWnd->Jyushi_List[BoxNum+1][8];
	Cb_V_Mnt_K_Other2_Kata = pWnd->Jyushi_List[BoxNum+1][9];
	EDIT_K_Other2 = pWnd->Jyushi_List[BoxNum+1][10];

	Cb_V_Mnt_K_Other3_Name = pWnd->Jyushi_List[BoxNum+1][11];
	Cb_V_Mnt_K_Other3_Kata = pWnd->Jyushi_List[BoxNum+1][12];
	EDIT_K_Other3 = pWnd->Jyushi_List[BoxNum+1][13];

	Cb_V_Mnt_K_Other4_Name = pWnd->Jyushi_List[BoxNum+1][14];
	Cb_V_Mnt_K_Other4_Kata = pWnd->Jyushi_List[BoxNum+1][15];
	EDIT_K_Other4 = pWnd->Jyushi_List[BoxNum+1][16];

	Cb_V_Mnt_K_Other5_Name = pWnd->Jyushi_List[BoxNum+1][17];
	Cb_V_Mnt_K_Other5_Kata = pWnd->Jyushi_List[BoxNum+1][18];
	EDIT_K_Other5 = pWnd->Jyushi_List[BoxNum+1][19];

	UpdateData(FALSE);


}

void CMnt_Kishu::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_Mnt_Kishu = FALSE;
	DestroyWindow();

	CDialog::OnClose();
}
