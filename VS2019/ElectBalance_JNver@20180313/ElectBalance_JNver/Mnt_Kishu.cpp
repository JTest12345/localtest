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
	, Cb_V_Mnt_K_WRank(_T(""))
	, Cb_V_Mnt_K_Setubi(_T(""))
	, Cb_V_Mnt_K_Other6_Name(_T(""))
	, Cb_V_Mnt_K_Other7_Name(_T(""))
	, Cb_V_Mnt_K_Other8_Name(_T(""))
	, Cb_V_Mnt_K_Other9_Name(_T(""))
	, Cb_V_Mnt_K_Other10_Name(_T(""))
	, Cb_V_Mnt_K_Other6_Kata(_T(""))
	, Cb_V_Mnt_K_Other7_Kata(_T(""))
	, Cb_V_Mnt_K_Other8_Kata(_T(""))
	, Cb_V_Mnt_K_Other9_Kata(_T(""))
	, Cb_V_Mnt_K_Other10_Kata(_T(""))
	, EDIT_K_Other6(_T(""))
	, EDIT_K_Other7(_T(""))
	, EDIT_K_Other8(_T(""))
	, EDIT_K_Other9(_T(""))
	, EDIT_K_Other10(_T(""))
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
	DDX_Control(pDX, IDC_COMBO1, Cb_C_Mnt_K_WRank);
	DDX_CBString(pDX, IDC_COMBO1, Cb_V_Mnt_K_WRank);
	DDX_Control(pDX, IDC_COMBO2, Cb_C_Mnt_K_Setubi);
	DDX_CBString(pDX, IDC_COMBO2, Cb_V_Mnt_K_Setubi);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other6_Name, Cb_C_Mnt_K_Other6_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other6_Name, Cb_V_Mnt_K_Other6_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other7_Name, Cb_C_Mnt_K_Other7_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other7_Name, Cb_V_Mnt_K_Other7_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other8_Name, Cb_C_Mnt_K_Other8_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other8_Name, Cb_V_Mnt_K_Other8_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other9_Name, Cb_C_Mnt_K_Other9_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other9_Name, Cb_V_Mnt_K_Other9_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other10_Name, Cb_C_Mnt_K_Other10_Name);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other10_Name, Cb_V_Mnt_K_Other10_Name);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other6_Kata, Cb_C_Mnt_K_Other6_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other6_Kata, Cb_V_Mnt_K_Other6_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other7_Kata, Cb_C_Mnt_K_Other7_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other7_Kata, Cb_V_Mnt_K_Other7_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other8_Kata, Cb_C_Mnt_K_Other8_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other8_Kata, Cb_V_Mnt_K_Other8_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other9_Kata, Cb_C_Mnt_K_Other9_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other9_Kata, Cb_V_Mnt_K_Other9_Kata);
	DDX_Control(pDX, IDC_Cb_Mnt_K_Other10_Kata, Cb_C_Mnt_K_Other10_Kata);
	DDX_CBString(pDX, IDC_Cb_Mnt_K_Other10_Kata, Cb_V_Mnt_K_Other10_Kata);
	DDX_Text(pDX, IDC_EDIT_K_Other6, EDIT_K_Other6);
	DDX_Text(pDX, IDC_EDIT_K_Other7, EDIT_K_Other7);
	DDX_Text(pDX, IDC_EDIT_K_Other8, EDIT_K_Other8);
	DDX_Text(pDX, IDC_EDIT_K_Other9, EDIT_K_Other9);
	DDX_Text(pDX, IDC_EDIT_K_Other10, EDIT_K_Other10);
}


BEGIN_MESSAGE_MAP(CMnt_Kishu, CDialog)
	ON_BN_CLICKED(IDC_Bn_Mnt_K_Toroku, &CMnt_Kishu::OnBnClickedBnMntKToroku)
	ON_BN_CLICKED(IDC_Bn_Mnt_K_Sakujo, &CMnt_Kishu::OnBnClickedBnMntKSakujo)
	ON_BN_CLICKED(IDC_End, &CMnt_Kishu::OnBnClickedEnd)
	ON_CBN_SELCHANGE(IDC_Cb_Mnt_K_KishuName, &CMnt_Kishu::OnCbnSelchangeCbMntKKishuname)
	ON_WM_CLOSE()
	ON_CBN_EDITCHANGE(IDC_Cb_Mnt_K_KishuName, &CMnt_Kishu::OnCbnEditchangeCbMntKKishuname)
	ON_CBN_SELCHANGE(IDC_Cb_Mnt_K_Other6_Name, &CMnt_Kishu::OnCbnSelchangeCbMntKOther6Name)
END_MESSAGE_MAP()


// CMnt_Kishu ���b�Z�[�W �n���h��

void CMnt_Kishu::OnBnClickedBnMntKToroku()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	
	pWnd->Mnt_K.changepass = 1;
	pWnd->MntPass.DoModal();
	pWnd->Mnt_K.changepass = 0;

	if(pWnd->MntPass.passok == 1){

	CString str = pWnd->Jyushidatacsv;
	CString newfile = _T("C:\\EB\\mst\\temp.csv");

	int KishuName=0,WRank=0,Setubi=0,SZ=0,Other1=0,Other2=0,Other3=0,Other4=0,Other5=0,Other6=0,Other7=0,Other8=0,Other9=0,Other10=0;
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
	Cb_C_Mnt_K_WRank.GetWindowTextW(Cb_V_Mnt_K_WRank);//�g�������N
	Cb_C_Mnt_K_Setubi.GetWindowTextW(Cb_V_Mnt_K_Setubi);//�ݔ��̎��

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

	Cb_C_Mnt_K_Other6_Name.GetWindowTextW(Cb_V_Mnt_K_Other6_Name);//��܈ȊO�̎������E
	Cb_C_Mnt_K_Other6_Kata.GetWindowTextW(Cb_V_Mnt_K_Other6_Kata);//��܈ȊO�̎����^�ԇE

	Cb_C_Mnt_K_Other7_Name.GetWindowTextW(Cb_V_Mnt_K_Other7_Name);//��܈ȊO�̎������F
	Cb_C_Mnt_K_Other7_Kata.GetWindowTextW(Cb_V_Mnt_K_Other7_Kata);//��܈ȊO�̎����^�ԇF

	Cb_C_Mnt_K_Other8_Name.GetWindowTextW(Cb_V_Mnt_K_Other8_Name);//��܈ȊO�̎������G
	Cb_C_Mnt_K_Other8_Kata.GetWindowTextW(Cb_V_Mnt_K_Other8_Kata);//��܈ȊO�̎����^�ԇG

	Cb_C_Mnt_K_Other9_Name.GetWindowTextW(Cb_V_Mnt_K_Other9_Name);//��܈ȊO�̎������H
	Cb_C_Mnt_K_Other9_Kata.GetWindowTextW(Cb_V_Mnt_K_Other9_Kata);//��܈ȊO�̎����^�ԇH

	Cb_C_Mnt_K_Other10_Name.GetWindowTextW(Cb_V_Mnt_K_Other10_Name);//��܈ȊO�̎������I
	Cb_C_Mnt_K_Other10_Kata.GetWindowTextW(Cb_V_Mnt_K_Other10_Kata);//��܈ȊO�̎����^�ԇI

	if(Cb_V_Mnt_K_KishuName != "")KishuName++;
	if(Cb_V_Mnt_K_WRank != "")WRank++;
	if(Cb_V_Mnt_K_Setubi != "")Setubi++;
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

	if(Cb_V_Mnt_K_Other6_Name != "")Other6++;
	if(Cb_V_Mnt_K_Other6_Kata != "")Other6++;
	if(EDIT_K_Other6 != "")Other6++;

	if(Cb_V_Mnt_K_Other7_Name != "")Other7++;
	if(Cb_V_Mnt_K_Other7_Kata != "")Other7++;
	if(EDIT_K_Other7 != "")Other7++;

	if(Cb_V_Mnt_K_Other8_Name != "")Other8++;
	if(Cb_V_Mnt_K_Other8_Kata != "")Other8++;
	if(EDIT_K_Other8 != "")Other8++;

	if(Cb_V_Mnt_K_Other9_Name != "")Other9++;
	if(Cb_V_Mnt_K_Other9_Kata != "")Other9++;
	if(EDIT_K_Other9 != "")Other9++;

	if(Cb_V_Mnt_K_Other10_Name != "")Other10++;
	if(Cb_V_Mnt_K_Other10_Kata != "")Other10++;
	if(EDIT_K_Other10 != "")Other10++;
	


//�t�@�C���̏�������
CStdioFile fp;
CStdioFile ofp;
CStdioFile fptemp;


CString buf;
CString tmp[40];//6��ގ����̏ꍇ�A23���11��ނɕύX37�񂠂邽�߁A�z��40�ɕύX

int next;
i=0;j=0;
	//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(Cb_V_Mnt_K_KishuName) == 0 && pWnd->Jyushi_List[i][1].Compare(Cb_V_Mnt_K_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(Cb_V_Mnt_K_Setubi) == 0){
				bothsame = 1;
				next = i;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T(""))break;
	}

	if(ofp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){
				ofp.SeekToEnd();
				//��s�̒��Ŕ��[�ȓ��͂��󂯕t���Ȃ�
				if((KishuName == 0 || KishuName == 1) && (WRank == 0 || WRank == 1) && (SZ == 2 || SZ == 0) && (Other1 == 3 || Other1 == 0) && (Other2 == 3 || Other2 == 0) && (Other3 == 3 || Other3 == 0) && (Other4 == 3 || Other4 == 0) && (Other5 == 3 || Other5 == 0)
					&& (Other6 == 3 || Other6 == 0) && (Other7 == 3 || Other7 == 0) && (Other8 == 3 || Other8 == 0) && (Other9 == 3 || Other9 == 0) && (Other10 == 3 || Other10 == 0)){
					
					if(KishuName == 0 && WRank == 0 && SZ == 0 && Other1 == 0 && Other2 == 0 && Other3 == 0 && Other4 == 0 && Other5 == 0 && Other6 == 0 && Other7 == 0 && Other8 == 0 && Other9 == 0 && Other10 == 0)
					AfxMessageBox(_T("�����͂ł�."));

					else if(KishuName == 0)
					AfxMessageBox(_T("�@�햼�������͂ł�."));

					else if(bothsame == 1){//�@�햼�A�g�������N�A�ݔ��̎�ނ������f�[�^�����łɂ���ꍇ
					////////////////////�C�����鏈��������////////////////////
					//��v���Ă�����̂�T��
					int a;
					int cnt=0;

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

		if(fptemp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){

			if(bothsame == 1 && cnt != next){
			
			//,��؂�̃f�[�^��tmp�ɒ��o
			for(i=0; i<40; i++) tmp[i] = _T("");

			for(i=0; i<40; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

						fptemp.SeekToEnd();
						fptemp.WriteString(buf);//��s�S�ď�������
						fptemp.WriteString(_T("\n"));//�Ō�ɉ��s������
						break;
					}
			}//buf����,��������if��
				buf.Delete(0, a+1);
			
			}

			else if(bothsame == 1 && cnt == next){//�C���ӏ��̍s�͕ʏ���
			//���̓t�H�[���̓��e��S�ď�������
			if(KishuName == 1){
					fptemp.WriteString(Cb_V_Mnt_K_KishuName);
					if(WRank == 0)fptemp.WriteString(_T(",endpoint,\n"));//���̃{�b�N�X�ɉ��������Ă��Ȃ����
					else fptemp.WriteString(_T(","));
					
					//Cb_C_Mnt_K_KishuName.InsertString(-1, Cb_V_Mnt_K_KishuName);//CB���X�g�֒ǉ�

					pWnd->Jyushi_List[next][0] = Cb_V_Mnt_K_KishuName;

					}

					if(WRank == 1){//�{�b�N�X�ɒl�������Ă����Ƃ�
				fptemp.WriteString(Cb_V_Mnt_K_WRank);
				AllNameWRank[i] = Cb_V_Mnt_K_WRank;//���X�g�֒ǉ�����
				AllNameWRankCnt++;//�z��𑝂₷
				pWnd->Jyushi_List[next][1] = Cb_V_Mnt_K_WRank;

				if(Setubi == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

					if(Setubi == 1){//�{�b�N�X�ɒl�������Ă����Ƃ�
				fptemp.WriteString(Cb_V_Mnt_K_Setubi);
				pWnd->Jyushi_List[next][2] = Cb_V_Mnt_K_Setubi;

				if(SZ == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}


					if(SZ == 2){//�S�Ẵ{�b�N�X�ɒl�������Ă����Ƃ�
				fptemp.WriteString(Cb_V_Mnt_K_SZ_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_SZ_Kata);
				fptemp.WriteString(_T(","));
				//20180409�@100��15
				fptemp.WriteString(_T("15"));
				//�z��Ɋi�[
				pWnd->Jyushi_List[next][3] = Cb_V_Mnt_K_SZ_Name;
				pWnd->Jyushi_List[next][4] = Cb_V_Mnt_K_SZ_Kata;
				pWnd->Jyushi_List[next][5] = _T("15");

				if(Other1 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

					if(Other1 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other1_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other1_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other1);

				pWnd->Jyushi_List[next][6] = Cb_V_Mnt_K_Other1_Name;
				pWnd->Jyushi_List[next][7] = Cb_V_Mnt_K_Other1_Kata;
				pWnd->Jyushi_List[next][8] = EDIT_K_Other1;

				if(Other2 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
				
					}

					if(Other2 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other2_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other2_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other2);

				pWnd->Jyushi_List[next][9] = Cb_V_Mnt_K_Other2_Name;
				pWnd->Jyushi_List[next][10] = Cb_V_Mnt_K_Other2_Kata;
				pWnd->Jyushi_List[next][11] = EDIT_K_Other2;

				if(Other3 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

					if(Other3 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other3_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other3_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other3);

				pWnd->Jyushi_List[next][12] = Cb_V_Mnt_K_Other3_Name;
				pWnd->Jyushi_List[next][13] = Cb_V_Mnt_K_Other3_Kata;
				pWnd->Jyushi_List[next][14] = EDIT_K_Other3;


				if(Other4 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

					if(Other4 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other4_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other4_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other4);

				pWnd->Jyushi_List[next][15] = Cb_V_Mnt_K_Other4_Name;
				pWnd->Jyushi_List[next][16] = Cb_V_Mnt_K_Other4_Kata;
				pWnd->Jyushi_List[next][17] = EDIT_K_Other4;

				if(Other5 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

					if(Other5 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other5_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other5_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other5);

				pWnd->Jyushi_List[next][18] = Cb_V_Mnt_K_Other5_Name;
				pWnd->Jyushi_List[next][19] = Cb_V_Mnt_K_Other5_Kata;
				pWnd->Jyushi_List[next][20] = EDIT_K_Other5;

				if(Other6 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

					if(Other6 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other6_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other6_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other6);

				pWnd->Jyushi_List[next][21] = Cb_V_Mnt_K_Other6_Name;
				pWnd->Jyushi_List[next][22] = Cb_V_Mnt_K_Other6_Kata;
				pWnd->Jyushi_List[next][23] = EDIT_K_Other6;

				if(Other7 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

				if(Other7 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other7_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other7_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other7);

				pWnd->Jyushi_List[next][24] = Cb_V_Mnt_K_Other7_Name;
				pWnd->Jyushi_List[next][25] = Cb_V_Mnt_K_Other7_Kata;
				pWnd->Jyushi_List[next][26] = EDIT_K_Other7;

				if(Other8 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

				if(Other8 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other8_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other8_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other8);

				pWnd->Jyushi_List[next][27] = Cb_V_Mnt_K_Other8_Name;
				pWnd->Jyushi_List[next][28] = Cb_V_Mnt_K_Other8_Kata;
				pWnd->Jyushi_List[next][29] = EDIT_K_Other8;

				if(Other9 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

				if(Other9 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other9_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other9_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other9);

				pWnd->Jyushi_List[next][30] = Cb_V_Mnt_K_Other9_Name;
				pWnd->Jyushi_List[next][31] = Cb_V_Mnt_K_Other9_Kata;
				pWnd->Jyushi_List[next][32] = EDIT_K_Other9;

				if(Other10 == 0)fptemp.WriteString(_T(",endpoint,\n"));
				else fptemp.WriteString(_T(","));
					}

				if(Other10 == 3){
				fptemp.WriteString(Cb_V_Mnt_K_Other10_Name);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(Cb_V_Mnt_K_Other10_Kata);
				fptemp.WriteString(_T(","));
				fptemp.WriteString(EDIT_K_Other10);

				pWnd->Jyushi_List[next][33] = Cb_V_Mnt_K_Other10_Name;
				pWnd->Jyushi_List[next][34] = Cb_V_Mnt_K_Other10_Kata;
				pWnd->Jyushi_List[next][35] = EDIT_K_Other10;


				fptemp.WriteString(_T(",endpoint,\n"));
				}

			}
				cnt++;
		}//�폜�Ώۂ̋@�킪���邩�ǂ���
		}
	}
		fp.Close();
		fptemp.Close();		

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
				ofp.WriteString(Cb_V_Mnt_K_WRank);
				AllNameWRank[i] = Cb_V_Mnt_K_WRank;//���X�g�֒ǉ�����
				AllNameWRankCnt++;//�z��𑝂₷
				pWnd->Jyushi_List[i][1] = Cb_V_Mnt_K_WRank;

				if(Setubi == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Setubi == 1){//�{�b�N�X�ɒl�������Ă����Ƃ�
				ofp.WriteString(Cb_V_Mnt_K_Setubi);
				pWnd->Jyushi_List[i][2] = Cb_V_Mnt_K_Setubi;

				if(SZ == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(SZ == 2){//�S�Ẵ{�b�N�X�ɒl�������Ă����Ƃ�
				ofp.WriteString(Cb_V_Mnt_K_SZ_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_SZ_Kata);
				ofp.WriteString(_T(","));
				//20180409 100 -> 15
				ofp.WriteString(_T("15"));
				//�z��Ɋi�[
				pWnd->Jyushi_List[i][3] = Cb_V_Mnt_K_SZ_Name;
				pWnd->Jyushi_List[i][4] = Cb_V_Mnt_K_SZ_Kata;
				pWnd->Jyushi_List[i][5] = _T("15");

				if(Other1 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other1 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other1_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other1_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other1);

				pWnd->Jyushi_List[i][6] = Cb_V_Mnt_K_Other1_Name;
				pWnd->Jyushi_List[i][7] = Cb_V_Mnt_K_Other1_Kata;
				pWnd->Jyushi_List[i][8] = EDIT_K_Other1;

				if(Other2 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
				
					}

					if(Other2 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other2_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other2_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other2);

				pWnd->Jyushi_List[i][9] = Cb_V_Mnt_K_Other2_Name;
				pWnd->Jyushi_List[i][10] = Cb_V_Mnt_K_Other2_Kata;
				pWnd->Jyushi_List[i][11] = EDIT_K_Other2;

				if(Other3 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other3 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other3_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other3_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other3);

				pWnd->Jyushi_List[i][12] = Cb_V_Mnt_K_Other3_Name;
				pWnd->Jyushi_List[i][13] = Cb_V_Mnt_K_Other3_Kata;
				pWnd->Jyushi_List[i][14] = EDIT_K_Other3;


				if(Other4 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other4 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other4_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other4_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other4);

				pWnd->Jyushi_List[i][15] = Cb_V_Mnt_K_Other4_Name;
				pWnd->Jyushi_List[i][16] = Cb_V_Mnt_K_Other4_Kata;
				pWnd->Jyushi_List[i][17] = EDIT_K_Other4;

				if(Other5 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other5 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other5_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other5_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other5);

				pWnd->Jyushi_List[i][18] = Cb_V_Mnt_K_Other5_Name;
				pWnd->Jyushi_List[i][19] = Cb_V_Mnt_K_Other5_Kata;
				pWnd->Jyushi_List[i][20] = EDIT_K_Other5;
				
				//�ǉ�20160715
				if(Other6 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other6 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other6_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other6_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other6);

				pWnd->Jyushi_List[i][21] = Cb_V_Mnt_K_Other6_Name;
				pWnd->Jyushi_List[i][22] = Cb_V_Mnt_K_Other6_Kata;
				pWnd->Jyushi_List[i][23] = EDIT_K_Other6;

				if(Other7 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other7 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other7_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other7_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other7);

				pWnd->Jyushi_List[i][24] = Cb_V_Mnt_K_Other7_Name;
				pWnd->Jyushi_List[i][25] = Cb_V_Mnt_K_Other7_Kata;
				pWnd->Jyushi_List[i][26] = EDIT_K_Other7;

				if(Other8 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other8 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other8_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other8_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other8);

				pWnd->Jyushi_List[i][27] = Cb_V_Mnt_K_Other8_Name;
				pWnd->Jyushi_List[i][28] = Cb_V_Mnt_K_Other8_Kata;
				pWnd->Jyushi_List[i][29] = EDIT_K_Other8;

				if(Other9 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other9 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other9_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other9_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other9);

				pWnd->Jyushi_List[i][30] = Cb_V_Mnt_K_Other9_Name;
				pWnd->Jyushi_List[i][31] = Cb_V_Mnt_K_Other9_Kata;
				pWnd->Jyushi_List[i][32] = EDIT_K_Other9;

				if(Other10 == 0)ofp.WriteString(_T(",endpoint,\n"));
				else ofp.WriteString(_T(","));
					}

					if(Other10 == 3){
				ofp.WriteString(Cb_V_Mnt_K_Other10_Name);
				ofp.WriteString(_T(","));
				ofp.WriteString(Cb_V_Mnt_K_Other10_Kata);
				ofp.WriteString(_T(","));
				ofp.WriteString(EDIT_K_Other10);

				pWnd->Jyushi_List[i][33] = Cb_V_Mnt_K_Other10_Name;
				pWnd->Jyushi_List[i][34] = Cb_V_Mnt_K_Other10_Kata;
				pWnd->Jyushi_List[i][35] = EDIT_K_Other10;

				ofp.WriteString(_T(",endpoint,\n"));

				}
				AfxMessageBox(_T("�o�^���������܂���."));
				}
			}
				else
					AfxMessageBox(_T("�����͕���������܂�."));
	}
	ofp.Close();

	if(bothsame == 1){
			DeleteFile(str);
			MoveFile(newfile,str);
			AfxMessageBox(_T("�C�����������܂���"));
		}

	UpdateData(FALSE);
	}

}

void CMnt_Kishu::OnBnClickedBnMntKSakujo()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;

	pWnd->Mnt_K.changepass = 1;
	pWnd->MntPass.DoModal();
	pWnd->Mnt_K.changepass = 0;

	if(pWnd->MntPass.passok == 1){

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
		if(pWnd->Jyushi_List[next][0].Compare(Cb_V_Mnt_K_KishuName) == 0 && pWnd->Jyushi_List[next][1].Compare(Cb_V_Mnt_K_WRank) == 0 && pWnd->Jyushi_List[next][2].Compare(Cb_V_Mnt_K_Setubi) == 0){
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
}

BOOL CMnt_Kishu::OnInitDialog()
{
	CDialog::OnInitDialog();

	// TODO:  �����ɏ�������ǉ����Ă�������
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("A"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("B"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("C"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("D"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("D1"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("D2"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("D3"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("D4"));
	Cb_C_Mnt_K_WRank.InsertString(-1, _T("�Ȃ�"));

	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("TDK"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("MUSASHI"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("�V���b�g�~�j"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("quspa"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("�V�[�g�ڒ���"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("���˘g����"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("���~����"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("H.D��g����"));
	Cb_C_Mnt_K_Setubi.InsertString(-1, _T("�w��Ȃ�"));


	int i;

	for(i=1;pWnd->Jyushi_List[i][0]!=_T("");i++){
		Cb_C_Mnt_K_KishuName.InsertString(-1, pWnd->Jyushi_List[i][0]);
	}

Cb_V_Mnt_K_SZ_Name = "B��";
Cb_V_Mnt_K_Other1_Name = "A��";
Cb_V_Mnt_K_Other2_Name = "�t�B���[";
Cb_V_Mnt_K_Other3_Name = "�u����";

Cb_V_Mnt_K_Setubi = _T("�w��Ȃ�");

//�ϐ�����Edit��
UpdateData(FALSE);

changepass = 0;

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
	Cb_V_Mnt_K_WRank = pWnd->Jyushi_List[BoxNum+1][1];
	Cb_V_Mnt_K_Setubi = pWnd->Jyushi_List[BoxNum+1][2];

	Cb_V_Mnt_K_SZ_Name = pWnd->Jyushi_List[BoxNum+1][3];
	Cb_V_Mnt_K_SZ_Kata = pWnd->Jyushi_List[BoxNum+1][4];

	Cb_V_Mnt_K_Other1_Name = pWnd->Jyushi_List[BoxNum+1][6];
	Cb_V_Mnt_K_Other1_Kata = pWnd->Jyushi_List[BoxNum+1][7];
	EDIT_K_Other1 = pWnd->Jyushi_List[BoxNum+1][8];

	Cb_V_Mnt_K_Other2_Name = pWnd->Jyushi_List[BoxNum+1][9];
	Cb_V_Mnt_K_Other2_Kata = pWnd->Jyushi_List[BoxNum+1][10];
	EDIT_K_Other2 = pWnd->Jyushi_List[BoxNum+1][11];

	Cb_V_Mnt_K_Other3_Name = pWnd->Jyushi_List[BoxNum+1][12];
	Cb_V_Mnt_K_Other3_Kata = pWnd->Jyushi_List[BoxNum+1][13];
	EDIT_K_Other3 = pWnd->Jyushi_List[BoxNum+1][14];

	Cb_V_Mnt_K_Other4_Name = pWnd->Jyushi_List[BoxNum+1][15];
	Cb_V_Mnt_K_Other4_Kata = pWnd->Jyushi_List[BoxNum+1][16];
	EDIT_K_Other4 = pWnd->Jyushi_List[BoxNum+1][17];

	Cb_V_Mnt_K_Other5_Name = pWnd->Jyushi_List[BoxNum+1][18];
	Cb_V_Mnt_K_Other5_Kata = pWnd->Jyushi_List[BoxNum+1][19];
	EDIT_K_Other5 = pWnd->Jyushi_List[BoxNum+1][20];

	Cb_V_Mnt_K_Other6_Name = pWnd->Jyushi_List[BoxNum+1][21];
	Cb_V_Mnt_K_Other6_Kata = pWnd->Jyushi_List[BoxNum+1][22];
	EDIT_K_Other6 = pWnd->Jyushi_List[BoxNum+1][23];

	Cb_V_Mnt_K_Other7_Name = pWnd->Jyushi_List[BoxNum+1][24];
	Cb_V_Mnt_K_Other7_Kata = pWnd->Jyushi_List[BoxNum+1][25];
	EDIT_K_Other7 = pWnd->Jyushi_List[BoxNum+1][26];

	Cb_V_Mnt_K_Other8_Name = pWnd->Jyushi_List[BoxNum+1][27];
	Cb_V_Mnt_K_Other8_Kata = pWnd->Jyushi_List[BoxNum+1][28];
	EDIT_K_Other8 = pWnd->Jyushi_List[BoxNum+1][29];

	Cb_V_Mnt_K_Other9_Name = pWnd->Jyushi_List[BoxNum+1][30];
	Cb_V_Mnt_K_Other9_Kata = pWnd->Jyushi_List[BoxNum+1][31];
	EDIT_K_Other9 = pWnd->Jyushi_List[BoxNum+1][32];

	Cb_V_Mnt_K_Other10_Name = pWnd->Jyushi_List[BoxNum+1][33];
	Cb_V_Mnt_K_Other10_Kata = pWnd->Jyushi_List[BoxNum+1][34];
	EDIT_K_Other10 = pWnd->Jyushi_List[BoxNum+1][35];

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

void CMnt_Kishu::OnCbnEditchangeCbMntKKishuname()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	//�o�[�R�[�h�Ή����\�ɂ���
	//int name_cnt = 0;
	//name_cnt++;

	////Edit����ϐ���
	//UpdateData(TRUE);

	//if(name_cnt >= 25){
	//	//Edit����ϐ���
	//	UpdateData(TRUE);

	//	// TAB�L�[
	//	keybd_event(VK_TAB, 0, 0, 0 );
 //       // TAB�L�[�̉������B
 //       keybd_event(VK_TAB, 0, KEYEVENTF_KEYUP, 0);
	//	name_cnt = 0;
	//}

	//if(Cb_J_Kishu.Find(_T(" ")) != -1 || Cb_J_Kishu.Find(_T("�@")) != -1){
	//	// BackSpace�L�[
	//	keybd_event(VK_BACK, 0, 0, 0 );
	//	// BackSpace�L�[�̉������B
	//	keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, 0);

	//	//Edit����ϐ���
	//	UpdateData(TRUE);

	//	//flag
	//	}
	//	}

	//else{
 //     //Edit����ϐ���
	//  UpdateData(TRUE);
	//}
}

void CMnt_Kishu::OnCbnSelchangeCbMntKOther6Name()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
}
