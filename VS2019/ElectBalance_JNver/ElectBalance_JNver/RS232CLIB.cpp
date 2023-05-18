// RS232Cライブラリ
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
■説明
　シリアル通信(RS-232C)のポートをオープンする
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　recv_size	：受信バッファのサイズ（単位：バイト）4096以下は、4096になる？？
　send_size	：送信バッファのサイズ（単位：バイト）
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でない場合）
　2	：エラー（指定したCOMポートがすでにオープンされている）
　3	：エラー（他のアプリケーションが使用中 or そのCOMポートは装備されていない）
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

	// オープンに失敗したとき
	if(handle_com[n-1]==INVALID_HANDLE_VALUE)	return 3;

	//送受信バッファの設定
	SetupComm(handle_com[n-1],recv_size,send_size);

	//COMポートをイネーブル
	com_init_en[n-1]=1;

	//通信のデフォルト設定
	Config(n,RS232_2400|RS232_1|RS232_EVEN|RS232_7|RS232_X_N|RS232_CTS_N|RS232_DSR_N|RS232_DTR_Y|RS232_RTS_Y);
	//Timeoutの初期設定値(ms)
	RecvTimeOut(n,100);
	SendTimeOut(n,100);
	return 0;
}

/*
■説明
　シリアル通信(RS-232C)のポートをクローズする
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（何らかの理由によりクローズに失敗）
*/
int RS232CLIB::Close(int n)
{
	if(Check(n)) return 1;
	if(!CloseHandle(handle_com[n-1])) return 2;
	com_init_en[n-1]=0;
	return 0;
}

/*
■説明
　シリアル通信(RS-232C)の設定を変更する
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　data		：変更パラメータ
		232C_110		ボーレート 110 bps
　　　　232C_300		ボーレート 300 bps
　　　　232C_600		ボーレート 600 bps
　　　　232C_1200		ボーレート 1200 bps
　　　　232C_2400		ボーレート 2400 bps
　　　　232C_4800		ボーレート 4800 bps
　　　　232C_9600		ボーレート 9600 bps（初期値）
　　　　232C_14400		ボーレート 14400 bps
　　　　232C_19200		ボーレート 19200 bps
　　　　232C_38400		ボーレート 38400 bps
　　　　232C_56000		ボーレート 56000 bps
　　　　232C_57600		ボーレート 57600 bps
　　　　232C_115200		ボーレート 115200 bps
　　　　232C_128000		ボーレート 128000 bps
　　　　232C_256000		ボーレート 256000 bps

　　　　232C_1			ストップビット 1ビット（初期値）
　　　　232C_15			ストップビット 1.5ビット
　　　　232C_2			ストップビット 2ビット

　　　　232C_NO			パリティ なし（初期値）
　　　　232C_ODD		パリティ 奇数
　　　　232C_EVEN		パリティ 偶数
　　　　232C_MARK		パリティ マーク
　　　　232C_SPACE		パリティ スペース

　　　　232C_4			キャラクタ長 4bit

　　　　232C_5			キャラクタ長 5bit
　　　　232C_6			キャラクタ長 6bit
　　　　232C_7			キャラクタ長 7bit
　　　　232C_8			キャラクタ長 8bit（初期値）

　　　　232C_X_N		XON/XOFFは使わない（初期値）
　　　　232C_X_Y		XON/XOFFを用いてフロー制御を行う

　　　　232C_DTR_Y		DTRを有効にする（初期値）
　　　　232C_DTR_N		DTRを無効にする
　　　　232C_DTR_H		DTRをハンドシェークにする

　　　　232C_RTS_Y		RTSを有効にする（初期値）
　　　　232C_RTS_N		RTSを無効にする
　　　　232C_RTS_H		RTSをハンドシェークにする

　　　　232C_RTS_T		RTSをトグルにする

　　　　232C_CTS_Y		CTSを有効にする
　　　　232C_CTS_N		CTSを無効にする（初期値）

　　　　232C_DSR_Y		DSRを有効にする
　　　　232C_DSR_N		DSRを無効にする（初期値）
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（何らかの理由によりクローズに失敗）
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
■説明
　データの送信をする
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　buf		：送信データが格納されている領域の先頭アドレス
  size		：送信データのサイズ（単位：バイト）
■返値
　送信データのバイト数
　オープンされていないor範囲外のポート選択時は0が返る
*/
int RS232CLIB::Send(int n, void *buf, int size)
{
	DWORD m;
	if(Check(n)) return 0;
	WriteFile(handle_com[n-1],buf,size,&m,NULL);
	return m;
}

/*
■説明
　データの受信をする
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　buf		：受信データを格納する領域の先頭アドレス
  size		：受信データのサイズ（単位：バイト）
		size未満のデータしか来ないと、timeoutまで待機する
■返値
　受信できたデータのバイト数
　オープンされていないor範囲外のポート選択時は0が返る
*/
int RS232CLIB::Recv(int n, void *buf, int size)
{
	DWORD m;
	if(Check(n)) return 0;
	ReadFile(handle_com[n-1],buf,size,&m,NULL);
	return m;
}

/*
■説明
　エラーチェック
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　n が範囲外であったりCOMnが未初期化の場合１
*/
int RS232CLIB::Check(int n)
{
	if(n<1 || n>MAXPORT) return 1;
	if(!com_init_en[n-1]) return 1;
	return 0;
}

/*
■説明
　送信バッファにたまっているバイト数を返す
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　送信バッファ内のバイト数
　オープンされていないor範囲外のポート選択時は0が返る
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
■説明
　受信バッファにたまっているバイト数を返す
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　受信バッファ内のバイト数
　オープンされていないor範囲外のポート選択時は0が返る
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
■説明
　送信バッファクリア
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
*/
int RS232CLIB::ClearSend(int n)
{
	if(Check(n)) return 1;
	PurgeComm(handle_com[n-1],PURGE_TXCLEAR);		
	return 0;
}

/*
■説明
　受信バッファクリア
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
*/
int RS232CLIB::ClearRecv(int n)
{
	if(Check(n)) return 1;
	PurgeComm(handle_com[n-1],PURGE_RXCLEAR);		
	return 0;
}

/*
■説明
　送信タイムアウト時間変更or設定解除
　0指定はタイムアウトしない
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　sto		：タイムアウト時間(ms）
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（何らかの理由により設定に失敗）
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
■説明
　受信タイムアウト時間変更or設定解除
　0指定はタイムアウトしない
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　sto		：タイムアウト時間(ms）
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（何らかの理由により設定に失敗）
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
■説明
　シリアル通信(RS-232C)のポートを全てクローズする
■引数
　なし
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（何らかの理由によりクローズに失敗）
*/
void RS232CLIB::CloseAll(void)
{
	int n;
	for(n=1;n<=MAXPORT;n++){
		if (com_init_en[n-1]) Close(n);
	}
}

/*
■説明
　ボーレートの値を変更する
　特殊なボーレートも設定可能？
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　baundrate	：ボーレート値(bps)
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（指定のボーレートが設定できない。ボーレート変更はされていない)
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
■説明
　XON/XOFFフロー制御におけるしきい値を設定する
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
　xoff		：残りサイズ(byte)
　xon		：バッファ内のデータ数(byte)
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（何らかの理由によりクローズに失敗）
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
■説明
　シリアル通信(RS-232C)の設定を変更するダイアログを開く
■引数
　n			：ポート番号 [1～256]　(COM1～COM256に対応します)
■返値
　0	：正常終了（オープンできた）
　1	：エラー（ポート番号が1～256でないorオープンされていない場合）
　2	：エラー（パラメータが範囲外で設定出来ない）
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
