// RS232Cライブラリ
#ifndef _RS232LIB
#define _RS232LIB

/*
//【参考】DCB構造体
typedef struct _DCB { // dcb 
    DWORD DCBlength;           // sizeof(DCB) 
    DWORD BaudRate;            // current baud rate 
    DWORD fBinary: 1;          // binary mode, no EOF check 
    DWORD fParity: 1;          // enable parity checking 
    DWORD fOutxCtsFlow:1;      // CTS output flow control 
    DWORD fOutxDsrFlow:1;      // DSR output flow control 
    DWORD fDtrControl:2;       // DTR flow control type 
    DWORD fDsrSensitivity:1;   // DSR sensitivity 
    DWORD fTXContinueOnXoff:1; // XOFF continues Tx 
    DWORD fOutX: 1;            // XON/XOFF out flow control 
    DWORD fInX: 1;             // XON/XOFF in flow control 
    DWORD fErrorChar: 1;       // enable error replacement 
    DWORD fNull: 1;            // enable null stripping 
    DWORD fRtsControl:2;       // RTS flow control 
    DWORD fAbortOnError:1;     // abort reads/writes on error 
    DWORD fDummy2:17;          // reserved 
    WORD wReserved;            // not currently used 
    WORD XonLim;               // transmit XON threshold 
    WORD XoffLim;              // transmit XOFF threshold 
    BYTE ByteSize;             // number of bits/byte, 4-8 
    BYTE Parity;               // 0-4=no,odd,even,mark,space 
    BYTE StopBits;             // 0,1,2 = 1, 1.5, 2 
    char XonChar;              // Tx and Rx XON character 
    char XoffChar;             // Tx and Rx XOFF character 
    char ErrorChar;            // error replacement character 
    char EofChar;              // end of input character 
    char EvtChar;              // received event character 
    WORD wReserved1;           // reserved; do not use 
} DCB;
*/

#define MAXPORT 256

//通信設定のパラメータ定数
#define RS232_110		0x00000001
#define RS232_300		0x00000002
#define RS232_600		0x00000003
#define RS232_1200		0x00000004
#define RS232_2400		0x00000005
#define RS232_4800		0x00000006
#define RS232_9600		0x00000007
#define RS232_14400		0x00000008
#define RS232_19200		0x00000009
#define RS232_38400		0x0000000A
#define RS232_56000		0x0000000B
#define RS232_57600		0x0000000C
#define RS232_115200	0x0000000D
#define RS232_128000	0x0000000E
#define RS232_256000	0x0000000F

#define RS232_1			0x00000010
#define RS232_15		0x00000020
#define RS232_2			0x00000030

#define RS232_NO		0x00000100
#define RS232_ODD		0x00000200
#define RS232_EVEN		0x00000300
#define RS232_MARK		0x00000400
#define RS232_SPACE		0x00000500

#define RS232_4			0x00001000
#define RS232_5			0x00002000
#define RS232_6			0x00003000
#define RS232_7			0x00004000
#define RS232_8			0x00005000

#define RS232_DTR_N		0x00010000
#define RS232_DTR_Y		0x00020000
#define RS232_DTR_H		0x00030000

#define RS232_RTS_N		0x00100000
#define RS232_RTS_Y		0x00200000
#define RS232_RTS_H		0x00300000
#define RS232_RTS_T		0x00400000

#define RS232_CTS_Y		0x01000000
#define RS232_CTS_N		0x02000000
#define RS232_DSR_Y		0x04000000
#define RS232_DSR_N		0x08000000

#define RS232_X_Y		0x10000000
#define RS232_X_N		0x20000000

class __declspec(dllexport) RS232CLIB{
	//com1～256がオープンされているかのイネーブル
	int com_init_en[MAXPORT]; 
	//ハンドル値
	HANDLE handle_com[MAXPORT];

public:
	//コンストラクタ
	RS232CLIB();
	//デストラクタ
	~RS232CLIB();

	//COMポートをオープンする
	int Open(int n, int recv_size, int send_size);
	//COMポートをクローズする
	int Close(int n);
	//COMポートの設定を変更する
	int Config(int n, unsigned int data);
	//送信コマンド
	int Send(int n, void *buf, int size);
	//受信コマンド
	int Recv(int n, void *buf, int size);
	//エラーチェック
	int Check(int n);
	//送信バッファにたまっているバイト数チェック
	int CheckSend(int n);
	//受信バッファにたまっているバイト数チェック
	int CheckRecv(int n);
	//送信バッファをクリアする
	int ClearSend(int n);
	//受信バッファをクリアする
	int ClearRecv(int n);
	//送信タイムアウトを設定する
	int SendTimeOut(int n, int sto);
	//送信タイムアウトを設定する
	int RecvTimeOut(int n, int rto);
	//開いている全てのCOMポートをクローズする
	void CloseAll(void);
	//ボーレートを変更する
	int BaudRate(int n, int baudrate);
	//XON/XOFFフロー制御のしきい値を変更する
	int XoffXon(int n, int xoff, int xon);
	//コンフィグダイアログを表示する
	int ConfigDialog(int n);
};

#endif
