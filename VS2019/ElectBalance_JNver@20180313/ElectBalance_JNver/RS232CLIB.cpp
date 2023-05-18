// RS232C���C�u����
#include<stdio.h>

#include "stdafx.h"

#include "RS232CLIB.h"

RS232CLIB::RS232CLIB()
{
	for(int i=0; i<256; i++)
		com_init_en[i] = 0;
}

RS232CLIB::~RS232CLIB()
{
	CloseAll();
}

/*
������
�@�V���A���ʐM(RS-232C)�̃|�[�g���I�[�v������
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@recv_size	�F��M�o�b�t�@�̃T�C�Y�i�P�ʁF�o�C�g�j4096�ȉ��́A4096�ɂȂ�H�H
�@send_size	�F���M�o�b�t�@�̃T�C�Y�i�P�ʁF�o�C�g�j
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ��ꍇ�j
�@2	�F�G���[�i�w�肵��COM�|�[�g�����łɃI�[�v������Ă���j
�@3	�F�G���[�i���̃A�v���P�[�V�������g�p�� or ����COM�|�[�g�͑�������Ă��Ȃ��j
*/
int RS232CLIB::Open(int n, int recv_size, int send_size)
{
	//char comname[11];
	CString comname;

	if(n<1 || n>256) return 1;
	if(com_init_en[n-1]) return 2;

	//sprintf(comname,_T("\\\\.\\COM%d"),n);
	comname.Format(_T("\\\\.\\COM%d"),n);

	handle_com[n-1] = CreateFile(comname,GENERIC_READ|GENERIC_WRITE,0,NULL,OPEN_EXISTING,0,NULL);

	// �I�[�v���Ɏ��s�����Ƃ�
	if(handle_com[n-1]==INVALID_HANDLE_VALUE)	return 3;

	//����M�o�b�t�@�̐ݒ�
	SetupComm(handle_com[n-1],recv_size,send_size);

	//COM�|�[�g���C�l�[�u��
	com_init_en[n-1]=1;

	//�ʐM�̃f�t�H���g�ݒ�
	Config(n,RS232_2400|RS232_1|RS232_EVEN|RS232_7|RS232_X_N|RS232_CTS_N|RS232_DSR_N|RS232_DTR_Y|RS232_RTS_Y);
	//Timeout�̏����ݒ�l(ms)
	RecvTimeOut(n,100);
	SendTimeOut(n,100);
	return 0;
}

/*
������
�@�V���A���ʐM(RS-232C)�̃|�[�g���N���[�Y����
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i���炩�̗��R�ɂ��N���[�Y�Ɏ��s�j
*/
int RS232CLIB::Close(int n)
{
	if(Check(n)) return 1;
	if(!CloseHandle(handle_com[n-1])) return 2;
	com_init_en[n-1]=0;
	return 0;
}

/*
������
�@�V���A���ʐM(RS-232C)�̐ݒ��ύX����
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@data		�F�ύX�p�����[�^
		232C_110		�{�[���[�g 110 bps
�@�@�@�@232C_300		�{�[���[�g 300 bps
�@�@�@�@232C_600		�{�[���[�g 600 bps
�@�@�@�@232C_1200		�{�[���[�g 1200 bps
�@�@�@�@232C_2400		�{�[���[�g 2400 bps
�@�@�@�@232C_4800		�{�[���[�g 4800 bps
�@�@�@�@232C_9600		�{�[���[�g 9600 bps�i�����l�j
�@�@�@�@232C_14400		�{�[���[�g 14400 bps
�@�@�@�@232C_19200		�{�[���[�g 19200 bps
�@�@�@�@232C_38400		�{�[���[�g 38400 bps
�@�@�@�@232C_56000		�{�[���[�g 56000 bps
�@�@�@�@232C_57600		�{�[���[�g 57600 bps
�@�@�@�@232C_115200		�{�[���[�g 115200 bps
�@�@�@�@232C_128000		�{�[���[�g 128000 bps
�@�@�@�@232C_256000		�{�[���[�g 256000 bps

�@�@�@�@232C_1			�X�g�b�v�r�b�g 1�r�b�g�i�����l�j
�@�@�@�@232C_15			�X�g�b�v�r�b�g 1.5�r�b�g
�@�@�@�@232C_2			�X�g�b�v�r�b�g 2�r�b�g

�@�@�@�@232C_NO			�p���e�B �Ȃ��i�����l�j
�@�@�@�@232C_ODD		�p���e�B �
�@�@�@�@232C_EVEN		�p���e�B ����
�@�@�@�@232C_MARK		�p���e�B �}�[�N
�@�@�@�@232C_SPACE		�p���e�B �X�y�[�X

�@�@�@�@232C_4			�L�����N�^�� 4bit

�@�@�@�@232C_5			�L�����N�^�� 5bit
�@�@�@�@232C_6			�L�����N�^�� 6bit
�@�@�@�@232C_7			�L�����N�^�� 7bit
�@�@�@�@232C_8			�L�����N�^�� 8bit�i�����l�j

�@�@�@�@232C_X_N		XON/XOFF�͎g��Ȃ��i�����l�j
�@�@�@�@232C_X_Y		XON/XOFF��p���ăt���[������s��

�@�@�@�@232C_DTR_Y		DTR��L���ɂ���i�����l�j
�@�@�@�@232C_DTR_N		DTR�𖳌��ɂ���
�@�@�@�@232C_DTR_H		DTR���n���h�V�F�[�N�ɂ���

�@�@�@�@232C_RTS_Y		RTS��L���ɂ���i�����l�j
�@�@�@�@232C_RTS_N		RTS�𖳌��ɂ���
�@�@�@�@232C_RTS_H		RTS���n���h�V�F�[�N�ɂ���

�@�@�@�@232C_RTS_T		RTS���g�O���ɂ���

�@�@�@�@232C_CTS_Y		CTS��L���ɂ���
�@�@�@�@232C_CTS_N		CTS�𖳌��ɂ���i�����l�j

�@�@�@�@232C_DSR_Y		DSR��L���ɂ���
�@�@�@�@232C_DSR_N		DSR�𖳌��ɂ���i�����l�j
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i���炩�̗��R�ɂ��N���[�Y�Ɏ��s�j
*/
int RS232CLIB::Config(int n, unsigned int data)
{
	DCB dcb;
	int d;
	int baud[16]={0,CBR_110,CBR_300,CBR_600,CBR_1200,CBR_2400,CBR_4800,CBR_9600,CBR_14400,CBR_19200,CBR_38400,CBR_56000,CBR_57600,CBR_115200,CBR_128000,CBR_256000};
	int stopbit[4]={0,ONESTOPBIT,ONE5STOPBITS,TWOSTOPBITS};
	int parity[6]={0,NOPARITY,ODDPARITY,EVENPARITY,MARKPARITY,SPACEPARITY};
	int bytesize[6]={0,4,5,6,7,8};
	int dtr[4]={0,DTR_CONTROL_DISABLE,DTR_CONTROL_ENABLE,DTR_CONTROL_HANDSHAKE};
	int rts[5]={0,RTS_CONTROL_DISABLE,RTS_CONTROL_ENABLE,RTS_CONTROL_HANDSHAKE,RTS_CONTROL_TOGGLE};

	if(Check(n)) return 1;

	GetCommState(handle_com[n-1], &dcb);

	// Baud rate
	d= data & 0xF; if (d) dcb.BaudRate=baud[d];
	
	// Stop bit
	d=(data & 0x30)>>4;	if (d) dcb.StopBits=stopbit[d];

	// Parity
	d=(data & 0x700)>>8;
	if (d) {
		dcb.Parity=parity[d];
		if(d>1){
			dcb.fParity=TRUE;
			dcb.fErrorChar=TRUE;
			dcb.ErrorChar=' ';
		}else{
			dcb.fParity=FALSE;
			dcb.fErrorChar=FALSE;
		}
	}
	
	// Byte size
	d=(data & 0x7000)>>12; if (d) dcb.ByteSize=bytesize[d];
	
	// Dtr control
	d=(data & 0x30000)>>16; if (d) dcb.fDtrControl=dtr[d];

	// Rts control
	d=(data & 0x700000)>>20; if (d) dcb.fRtsControl=rts[d];

	// CTS control
	if(data &  RS232_CTS_Y) dcb.fOutxCtsFlow=TRUE;
	if(data &  RS232_CTS_N) dcb.fOutxCtsFlow=FALSE;
	// DSR control
	if(data &  RS232_DSR_Y) dcb.fOutxDsrFlow=TRUE;
	if(data &  RS232_DSR_N) dcb.fOutxDsrFlow=FALSE;

	// X control
	d=(data & 0x30000000);
	if(d==RS232_X_Y){
		dcb.fTXContinueOnXoff=FALSE;
		dcb.fOutX=TRUE;
		dcb.fInX=TRUE;
	}else if(d==RS232_X_N){
		dcb.fTXContinueOnXoff=FALSE;
		dcb.fOutX=FALSE;
		dcb.fInX=FALSE;
	}

	if (!SetCommState(handle_com[n-1], &dcb)) return 2;
	return 0;
}

/*
������
�@�f�[�^�̑��M������
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@buf		�F���M�f�[�^���i�[����Ă���̈�̐擪�A�h���X
  size		�F���M�f�[�^�̃T�C�Y�i�P�ʁF�o�C�g�j
���Ԓl
�@���M�f�[�^�̃o�C�g��
�@�I�[�v������Ă��Ȃ�or�͈͊O�̃|�[�g�I������0���Ԃ�
*/
int RS232CLIB::Send(int n, void *buf, int size)
{
	DWORD m;
	if(Check(n)) return 0;
	WriteFile(handle_com[n-1],buf,size,&m,NULL);
	return m;
}

/*
������
�@�f�[�^�̎�M������
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@buf		�F��M�f�[�^���i�[����̈�̐擪�A�h���X
  size		�F��M�f�[�^�̃T�C�Y�i�P�ʁF�o�C�g�j
		size�����̃f�[�^�������Ȃ��ƁAtimeout�܂őҋ@����
���Ԓl
�@��M�ł����f�[�^�̃o�C�g��
�@�I�[�v������Ă��Ȃ�or�͈͊O�̃|�[�g�I������0���Ԃ�
*/
int RS232CLIB::Recv(int n, void *buf, int size)
{
	DWORD m;
	if(Check(n)) return 0;
	ReadFile(handle_com[n-1],buf,size,&m,NULL);
	return m;
}

/*
������
�@�G���[�`�F�b�N
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@n ���͈͊O�ł�������COMn�����������̏ꍇ�P
*/
int RS232CLIB::Check(int n)
{
	if(n<1 || n>MAXPORT) return 1;
	if(!com_init_en[n-1]) return 1;
	return 0;
}

/*
������
�@���M�o�b�t�@�ɂ��܂��Ă���o�C�g����Ԃ�
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@���M�o�b�t�@���̃o�C�g��
�@�I�[�v������Ă��Ȃ�or�͈͊O�̃|�[�g�I������0���Ԃ�
*/
int RS232CLIB::CheckSend(int n)
{
	COMSTAT cs;
	DWORD err;
	if(Check(n)) return 0;
	ClearCommError(handle_com[n-1],&err,&cs);
	return cs.cbOutQue;
}

/*
������
�@��M�o�b�t�@�ɂ��܂��Ă���o�C�g����Ԃ�
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@��M�o�b�t�@���̃o�C�g��
�@�I�[�v������Ă��Ȃ�or�͈͊O�̃|�[�g�I������0���Ԃ�
*/
int RS232CLIB::CheckRecv(int n)
{
	COMSTAT cs;
	DWORD err;
	if(Check(n)) return 0;
	ClearCommError(handle_com[n-1],&err,&cs);
	return cs.cbInQue;
}

/*
������
�@���M�o�b�t�@�N���A
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
*/
int RS232CLIB::ClearSend(int n)
{
	if(Check(n)) return 1;
	PurgeComm(handle_com[n-1],PURGE_TXCLEAR);		
	return 0;
}

/*
������
�@��M�o�b�t�@�N���A
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
*/
int RS232CLIB::ClearRecv(int n)
{
	if(Check(n)) return 1;
	PurgeComm(handle_com[n-1],PURGE_RXCLEAR);		
	return 0;
}

/*
������
�@���M�^�C���A�E�g���ԕύXor�ݒ����
�@0�w��̓^�C���A�E�g���Ȃ�
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@sto		�F�^�C���A�E�g����(ms�j
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i���炩�̗��R�ɂ��ݒ�Ɏ��s�j
*/
int RS232CLIB::SendTimeOut(int n, int sto)
{
	COMMTIMEOUTS ct;
	
	if(Check(n)) return 1;
	GetCommTimeouts(handle_com[n-1],&ct);

	ct.WriteTotalTimeoutMultiplier=0;
	ct.WriteTotalTimeoutConstant=sto;
	
	if(!SetCommTimeouts(handle_com[n-1],&ct)) return 2;
	return 0;
}

/*
������
�@��M�^�C���A�E�g���ԕύXor�ݒ����
�@0�w��̓^�C���A�E�g���Ȃ�
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@sto		�F�^�C���A�E�g����(ms�j
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i���炩�̗��R�ɂ��ݒ�Ɏ��s�j
*/
int RS232CLIB::RecvTimeOut(int n, int rto)
{
	COMMTIMEOUTS ct;

	if(Check(n)) return 1;
	GetCommTimeouts(handle_com[n-1],&ct);

	ct.ReadIntervalTimeout=rto;
	ct.ReadTotalTimeoutMultiplier=0;
	ct.ReadTotalTimeoutConstant=rto;
	
	if(!SetCommTimeouts(handle_com[n-1],&ct)) return 2;
	return 0;
}

/*
������
�@�V���A���ʐM(RS-232C)�̃|�[�g��S�ăN���[�Y����
������
�@�Ȃ�
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i���炩�̗��R�ɂ��N���[�Y�Ɏ��s�j
*/
void RS232CLIB::CloseAll(void)
{
	int n;
	for(n=1;n<=MAXPORT;n++){
		if (com_init_en[n-1]) Close(n);
	}
}

/*
������
�@�{�[���[�g�̒l��ύX����
�@����ȃ{�[���[�g���ݒ�\�H
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@baundrate	�F�{�[���[�g�l(bps)
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i�w��̃{�[���[�g���ݒ�ł��Ȃ��B�{�[���[�g�ύX�͂���Ă��Ȃ�)
*/
int RS232CLIB::BaudRate(int n, int baudrate)
{
	DCB dcb;

	if(Check(n)) return 1;
	GetCommState(handle_com[n-1],&dcb);
	dcb.BaudRate=baudrate;
	if (!SetCommState(handle_com[n-1], &dcb)) return 2;
	return 0;
}

/*
������
�@XON/XOFF�t���[����ɂ����邵�����l��ݒ肷��
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
�@xoff		�F�c��T�C�Y(byte)
�@xon		�F�o�b�t�@���̃f�[�^��(byte)
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i���炩�̗��R�ɂ��N���[�Y�Ɏ��s�j
*/
int RS232CLIB::XoffXon(int n, int xoff, int xon)
{
	DCB dcb;

	if(Check(n)) return 1;

	GetCommState(handle_com[n-1], &dcb);
	dcb.XoffLim=xoff;
	dcb.XonLim=xon;
	if (!SetCommState(handle_com[n-1], &dcb)) return 2;
	return 0;
}

/*
������
�@�V���A���ʐM(RS-232C)�̐ݒ��ύX����_�C�A���O���J��
������
�@n			�F�|�[�g�ԍ� [1�`256]�@(COM1�`COM256�ɑΉ����܂�)
���Ԓl
�@0	�F����I���i�I�[�v���ł����j
�@1	�F�G���[�i�|�[�g�ԍ���1�`256�łȂ�or�I�[�v������Ă��Ȃ��ꍇ�j
�@2	�F�G���[�i�p�����[�^���͈͊O�Őݒ�o���Ȃ��j
*/
int RS232CLIB::ConfigDialog(int n)
{
	COMMCONFIG cc;
	DWORD size;
	//char comname[7];
	CString comname;

	if(Check(n)) return 1;

	//sprintf(comname,_T("COM%d"),n);
	comname.Format(_T("COM%d"),n);

	GetCommConfig(handle_com[n-1],&cc,&size);
	CommConfigDialog(comname,HWND_DESKTOP,&cc);
	if (!SetCommConfig(handle_com[n-1],&cc,sizeof(cc)))return 2;
	return 0;
}
