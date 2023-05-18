// Renum.cpp : �����t�@�C��
//

#include "stdafx.h"
#include "ElectBalance_JNver.h"
#include "Renum.h"

#include "ElectBalance_JNverDlg.h"

// CRenum �_�C�A���O

IMPLEMENT_DYNAMIC(CRenum, CDialog)

CRenum::CRenum(CWnd* pParent /*=NULL*/)
	: CDialog(CRenum::IDD, pParent)
	, Edit_V_rn(_T(""))
{

}

CRenum::~CRenum()
{
}

void CRenum::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT1, Edit_V_rn);
}


BEGIN_MESSAGE_MAP(CRenum, CDialog)
	ON_EN_CHANGE(IDC_EDIT1, &CRenum::OnEnChangeEdit1)
	ON_WM_CLOSE()
	ON_BN_CLICKED(IDC_BUTTON_EXE, &CRenum::OnBnClickedButtonExe)
END_MESSAGE_MAP()


// CRenum ���b�Z�[�W �n���h��

void CRenum::OnEnChangeEdit1()
{
	// TODO:  ���ꂪ RICHEDIT �R���g���[���̏ꍇ�A
	// �܂��ACDialog::OnInitDialog() �֐����I�[�o�[���C�h���āAOR ��Ԃ� ENM_CHANGE
	// �t���O���}�X�N�ɓ���āACRichEditCtrl().SetEventMask() ���Ăяo���Ȃ�����A
	// �R���g���[���́A���̒ʒm�𑗐M���܂���B

	// TODO:  �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����Ă��������B
	
}

void CRenum::OnClose()
{
	// TODO: �����Ƀ��b�Z�[�W �n���h�� �R�[�h��ǉ����邩�A����̏������Ăяo���܂��B
	/*CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	pWnd->dlflag_renum = FALSE;
	DestroyWindow();

	pWnd->PW.OnClose();*/

	CDialog::OnClose();
}

void CRenum::OnBnClickedButtonExe()
{
	// TODO: �����ɃR���g���[���ʒm�n���h�� �R�[�h��ǉ����܂��B
	CElectBalance_JNverDlg* pWnd = (CElectBalance_JNverDlg*)AfxGetApp()->m_pMainWnd;
	int Num = pWnd->Jshg.msr.CheckBoxNum,i,bothsame = 0;
	CString Oldg,Newg,JName,JKata;//History�쐬
	double ReNum;

	//Edit����ϐ���
	UpdateData(TRUE);


	if(pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][2] == Edit_V_rn){
		AfxMessageBox(_T("�C���O�Ɠ��l�̒l�ł�."));
	}

	else if(Edit_V_rn == _T("")){
		AfxMessageBox(_T("�l����͂��ĉ�����."));
	}

	else{
	//��v���Ă�����̂�T��
	for(i=0;i<SIZE;i++){
		if(pWnd->Jyushi_List[i][0].Compare(pWnd->Jshg.Cb_J_Kishu) == 0 && pWnd->Jyushi_List[i][1].Compare(pWnd->Jshg.Cb_V_J_WRank) == 0 && pWnd->Jyushi_List[i][2].Compare(pWnd->Jshg.Cb_J_Setubi) == 0){
				bothsame = 1;
				break;
				}
		else if(pWnd->Jyushi_List[i][0] == _T("")){
			break;
		}
	}
	
	//20180313�m�F
	ReNum = _tcstod(Edit_V_rn,NULL);//double�֌^�ϊ�
	Newg.Format(_T("%.4f"), ReNum);//�C����̒l(�䗦�ς���O)
	pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][2] = Newg;//���X�g�R���g���[���͔䗦��ς���O�̒l����
	ReNum = ReNum / pWnd->Jshg.msr.Num;//�䗦��ς���

	//pWnd->Jshg.msr.formdata = _tcstod(Edit_V_rn,NULL);//���͒l��ۑ����Ă���

	Oldg = pWnd->Jyushi_List[i][(3*Num)+5];//�C���O�̒l
	Newg.Format(_T("%.4f"), ReNum);//�C����̒l
	JName = pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][0];
	JKata = pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][1];

	//�C����̒l����
	//pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][2] = Newg;
	pWnd->Jyushi_List[i][(3*Num)+5] = Newg;
	//OnClose();
	
	//�ς���K�v�����邩�H
	//pWnd->Jshg.msr.ListC[pWnd->Jshg.msr.CheckBoxNum][2] = Newg;

	pWnd->RN.EndDialog(1);
		

	//pWnd->msr.OnClose();
	//pWnd->Jshg.Start();
	
		//pWnd->dlflag_measure = TRUE;//�N�����ɂ���
		//pWnd->msr.Create(IDD_measure,this);
		//pWnd->msr.ShowWindow(SW_SHOWNA);//�\��	

	//pWnd->msr.m_listc.SetItemText(1, 1, _T("aaa"));//4��ڂ֒ǉ�����
	//pWnd->msr.m_listc.SetItemText(Num, 2, Edit_V_rn);//3��ڂ֒ǉ�����
	//pWnd->msr.m_listc.DeleteAllItems();
	/*pWnd->msr.m_listc.RemoveAllGroups();
	pWnd->msr.m_listc;*/

	//�t�@�C���̒��g������������

	//�t�@�C���ǂݍ���
CStdioFile fp;
//�t�@�C���̏�������
CStdioFile ofp;

CString buf;
CString prebuf;
CString tmp[40];
int a,next;
int cnt=0;

CString str = pWnd->Jyushidatacsv;
CString newfile = _T("C:\\EB\\mst\\temp.csv");


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

	next = i;//�s�����݈ʒu������

	int j=0;
	while(pWnd->Jyushi_List[i][j] != _T("")){
	j++;
	}

	if(fp.Open(str,CFile::modeRead | CFile::shareDenyNone)){

		if(ofp.Open(newfile, CFile::modeWrite | CFile::shareDenyNone)){

		while(fp.ReadString(buf)){

			if(bothsame == 1 && cnt != next){
			
			//,��؂�̃f�[�^��tmp�ɒ��o
			for(int i=0; i<40; i++) tmp[i] = _T("");

			for(int i=0; i<40; i++){
				a = buf.Find(_T(","));
				if(a!=-1){
					tmp[i] = buf.Left(a);

						ofp.SeekToEnd();
						ofp.WriteString(buf);//��s�S�ď�������
						ofp.WriteString(_T("\n"));//�Ō�ɉ��s������
						break;
					}
			}//buf����,��������if��
				buf.Delete(0, a+1);
			
			}

			else if(bothsame == 1 && cnt == next){//�C���ӏ��̍s�͕ʏ���
				//,��؂�̃f�[�^��tmp�ɒ��o
			for(int i=0; i<40; i++) tmp[i] = _T("");

			for(int i=0; i<40; i++){
				a = buf.Find(_T(","));

				if(i==j){
							ofp.WriteString(_T("endpoint,"));//�Ō�ɉ��s������
							ofp.WriteString(_T("\n"));//�Ō�ɉ��s������
							break;
					}
				if(a!=-1){
					tmp[i] = buf.Left(a);

					
					//if(Cb_V_Mnt_K_KishuName.Compare(tmp[i]) == 0){
					////�폜���������ڂ̔ԍ����擾����DeleteString(i)�ŃR���{�{�b�N�X�̃v���_�E�����X�V������
					//Cb_C_Mnt_K_KishuName.DeleteString(cnt);
					//}
					//if(tmp[i].Find(_T("endpoint")) != -1){
					
						ofp.SeekToEnd();
						if(i == ((3*Num)+5)){
							ofp.WriteString(Newg);//�C����̐��l����������
							ofp.WriteString(_T(","));//�r���̏ꍇ�A�J���}
						}
						else{
							ofp.WriteString(tmp[i]);//�����I�ɏ�������
							ofp.WriteString(_T(","));//�r���̏ꍇ�A�J���}
						}
							
				}					
			//buf����,��������if��
				buf.Delete(0, a+1);
				}
			}		
				cnt++;
		}//�폜�Ώۂ̋@�킪���邩�ǂ���
		}
	}
		ofp.Close();
		fp.Close();

		if(bothsame == 1){
			DeleteFile(str);
			MoveFile(newfile,str);
			AfxMessageBox(_T("�C�����������܂���"));
		}

//History�X�V
str = pWnd->Historycsv;
CTime cTime = CTime::GetCurrentTime();           // ���ݎ���
CString date = cTime.Format("%Y/%m/%d %H:%M:%S");   // "YYYY/mm/dd HH:MM:SS"�`���̎�����������擾

if(fp.Open(str, CFile::modeWrite | CFile::shareDenyNone)){

				fp.SeekToEnd();
				
				fp.WriteString(date);
				fp.WriteString(_T(","));
				fp.WriteString(pWnd->Jshg.Cb_J_Kishu);
				fp.WriteString(_T(","));
				fp.WriteString(pWnd->Jshg.Cb_V_J_WRank);
				fp.WriteString(_T(","));
				fp.WriteString(pWnd->Jshg.Cb_J_Setubi);
				fp.WriteString(_T(","));
				fp.WriteString(pWnd->Jshg.LotNo);
				fp.WriteString(_T(","));
				fp.WriteString(JName);
				fp.WriteString(_T(","));
				fp.WriteString(JKata);
				fp.WriteString(_T(","));
				fp.WriteString(Oldg);
				fp.WriteString(_T(","));
				fp.WriteString(Newg);
				fp.WriteString(_T(","));
				fp.WriteString(_T("\n"));

	fp.Close();
}
}

}

void CRenum::OnOK()
{
	// TODO: �����ɓ���ȃR�[�h��ǉ����邩�A�������͊�{�N���X���Ăяo���Ă��������B

	//CDialog::OnOK();
}
