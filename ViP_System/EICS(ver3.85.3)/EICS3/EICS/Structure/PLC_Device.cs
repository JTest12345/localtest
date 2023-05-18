using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EICS
{

    // Keyence用のみ、三菱・オムロンは考慮していない。 ← SGA42 永尾
    public class PLC_Address
    {
        public string Address { get; set; }
        public string Device { get; set; }      // アドレスのヘッダ (DM, LR etc)
        private string AdrNum { get; set; }     // アドレス番号(ヘッダ以降の文字)
        public int AdrNum10 { get; set; }       // アドレス番号を10進数に変えた数
        private string Num_Digit { get; set; }  // アドレス番号の文字数(例： "B0000" ⇒ 4)
        public string DataTypeCD { get; set; }  // アドレスのデータ形式 (DEC_16BIT etc)
        public int DataLen { get; set; }        // アドレスのデータ長

        // コンストラクタ
        public PLC_Address(string Address, string DataTypeCD, int DataLen)
        {
            this.Address = Address;
            UpDate_DeviceAndNumber();
            this.Num_Digit = get_Digit(this.Device);
            this.DataTypeCD = DataTypeCD;
            this.DataLen = DataLen;
        }

        // アドレスからアドレスのヘッダとアドレス番号(10進数)を算出して、メンバ変数に格納
        private void UpDate_DeviceAndNumber()
        {
            int o;
            for (int i = 1; i <= Address.Length; i++)
            {
                if (int.TryParse(Address[i-1].ToString(), out o))
                {
                    this.Device = Address.Substring(0, i-1);
                    this.AdrNum = Address.Substring(i-1);
                    this.AdrNum10 = BaseChange10();
                    break;
                }
            }
            return;
        }
        // アドレス番号の最低桁数を取得
        public static string get_Digit(string Device)
        {
            KeyenceDeviceMasterList dList = PLC_Keyence.DEVICEMASTERLIST_5500.Find(d => d.DeviceNM == Device);
            if (dList != null) return dList.Digit.ToString();
            else return string.Empty;
        }

        #region 10進数へ変換

        public int BaseChange10()
        {
            return BaseChange10(this.AdrNum, this.Device);
        }        
        public static int BaseChange10(string Str, string AdrDevice)
        {
            switch (AdrDevice)
            {
                case "DM":
                case "T": 
                case "C":
                case "EM":
                case "ZF":
                case "TM":
                case "Z":
                case "CM":
                    return int.Parse(Str);
                case "LR":
                case "MR":
                case "R":
                case "CR":
                    return (int.Parse(Str) / 100) * 16 + (int.Parse(Str) % 100);
                case "B":
                case "W":
                    return Convert.ToInt32(Str, 16);
                default:
                    return 0;
            }
        }

        #endregion

        #region 元の進数へ戻す

        public string BaseReturn(int Val)
        {
            return BaseReturn(Val, this.Device, this.Num_Digit);
        }
        public static string BaseReturn(int Val, string AdrDevice)
        {
            return BaseReturn(Val, AdrDevice, get_Digit(AdrDevice));
        }
        public static string BaseReturn(int Val, string AdrDevice, string Digit)
        {
            switch (AdrDevice)
            {
                case "DM":
                case "T":
                case "C":
                case "EM":
                case "ZF":
                case "TM":
                case "Z":
                case "CM":
                    return Val.ToString();
                case "LR":
                case "MR":
                case "R":
                case "CR":
                    return ((int)((Val / 16) * 100 + (Val % 16))).ToString("D" + Digit);
                case "B":
                case "W":
                    return Val.ToString("X" + Digit);
                default:
                    return string.Empty;
            }
        }

        #endregion

    }

    public class PLC_Device
    {
        // キー部分
        public string Device { get; set; }      // アドレスのヘッダ (DM, LR etc)
        public string DataTypeCD { get; set; }  // アドレスのデータ形式 (DEC_16BIT etc)
        public int Div2 { get; set; }           // アドレス番号を2で割った余り (0：偶数, 1：奇数)

        public int readLimit { get; set; }      // PLCから一度に読める限界アドレス数
        public int plcSize { get; set; }        // 1アドレスが占有するPLCアドレス数
        
        public List<PLC_Address> AddressList = new List<PLC_Address>();


        public PLC_Device(PLC_Address Adr)
        {
            this.Device = Adr.Device;           
            this.DataTypeCD = Adr.DataTypeCD;
            this.Div2 = Adr.AdrNum10 % 2;

            this.readLimit = 1;
            this.plcSize = 1;
            
            this.AddressList.Add(Adr);
        }

        // アドレス種類毎の先頭・最後尾アドレスを算出 (リストを作成)
        public static Dictionary<string, PLC_Device> GetPLCDeviceList(LSETInfo lsetInfo,  List<PLC_Address> AdrList)
        {
            Dictionary<string, PLC_Device> PlcDevList = new Dictionary<string, PLC_Device>();

            PLC_Keyence plc = new PLC_Keyence(lsetInfo.IPAddressNO, lsetInfo.PortNO);

            // アドレスを配列に格納 + 各アドレス種類毎の取得範囲(先頭アドレス・最後尾アドレス・アドレス数)を確定
            foreach (PLC_Address tmpAdr in AdrList)
            {
                // 文字列型は無視する。
                if (tmpAdr.DataTypeCD == PLC.DT_STR) continue;

                // データが空 または ハイフンは対象外
                if (tmpAdr.Address == null) continue;
                if (tmpAdr.Address == "-") continue;
                if (tmpAdr.Address == string.Empty) continue;

                // キー ： アドレスのヘッダ + データ形式(DEC_16BIT etc) + アドレス番号の2の除数
                string key = tmpAdr.Device + "_" + tmpAdr.DataTypeCD;
                // 特定のデータ型は、アドレス番号の2の除数をキーに追加する。
                if (PLC.GetDataSizeFromDataType(tmpAdr.DataTypeCD) == 2)
                {
                    key += "_" + ((int)(tmpAdr.AdrNum10 % 2)).ToString();
                }

                PLC_Device tmp;  // TryGetValue用の引数
                if (PlcDevList.TryGetValue(key, out tmp))
                {
                    // リストに一致したアドレス種類のアドレスリストに追加
                    PlcDevList[key].AddressList.Add(tmpAdr);
                }
                else
                {   
                    // リストに無い場合、リストに追加
                    PlcDevList.Add(key, new PLC_Device(tmpAdr));
                }                
            }
            return PlcDevList;
        }
        // 各アドレスの値をDirectoryで取得する (Keyence版)
        public static Dictionary<string, decimal> GetAdrValList(PLC_Keyence plc, Dictionary<string, PLC_Device> PlcDevList)
        {
            Dictionary<string, decimal> tmpAVList = new Dictionary<string, decimal>();

            foreach(KeyValuePair<string, PLC_Device> PDL in PlcDevList)
            { 
                // デバイスクラス単位の処理
                PLC_Device PD = PDL.Value;
                KeyenceDeviceMasterList KDM = PLC_Keyence.DEVICEMASTERLIST_5500.Find(k => k.DeviceNM == PD.Device);

                // 特定のデータ型は、2アドレス分のデータを使用する。
                if (PLC.GetDataSizeFromDataType(PD.DataTypeCD) == 2)
                {
                    PD.readLimit = KDM.ReadLimit2;
                    PD.plcSize = 2;
                }
                else
                {
                    PD.readLimit = KDM.ReadLimit1;
                    PD.plcSize = 1;
                }

                // ① PLCアドレスリストを並び替え (アドレス番号順)
                //List<PLC_Address> PAL = PD.AddressList.OrderBy(a => a.AdrNum10).ToList();
                PD.AddressList.Sort((a,b) => a.AdrNum10 - b.AdrNum10);
                List<PLC_Address> PAL = PD.AddressList;

                // ② PLCアドレスのデータを取得して返り値に格納する

                int AryAdrNum10 = 0;       // PLCから取得した配列の最初のアドレス(10進数)
                int AryReadLimit = 0;   // 配列の取得数

                decimal[] d = {0};      // PLCから取得したデータ(数値型)

                for (int i = 0; i < PAL.Count; i++)
                {
                    // データを取得するアドレス
                    PLC_Address PA = PAL[i];

                    // 返り値に既にデータがある場合、飛ばす
                    decimal tmp;  // TryGetValue用の引数
                    if (tmpAVList.TryGetValue(PA.Address, out tmp))
                    {
                        continue;
                    }

                    // ②-1 PLCデバイスからデータを取得するかの判定。取得しない場合は、配列を使いまわす。
                    if ( (AryAdrNum10 + AryReadLimit * PD.plcSize) <= (PA.AdrNum10 + (PD.plcSize - 1) ) )
                    {
                        // ②-2 PLCデバイスから取得するデータの個数を決める。(常に可能なリミットを取得しては、処理時間が増えてしまう)
                        AryReadLimit = PD.getAryReadLimit(i);                      
                                                
                        // ②-2 PLCデバイスからデータをまとめて取得  ⇒ 配列に格納
                        d = plc.GetDataAsIntArray(PA.Address, AryReadLimit, PD.DataTypeCD);

                        // 取得した配列の最初のアドレスを更新
                        AryAdrNum10 = PA.AdrNum10;
                    }
                    
                    // ②-3 配列データから戻り値のDictionaryに(Key = アドレス, Value = PLCデータ)を格納する
                    int Idx = (PA.AdrNum10 - AryAdrNum10) / PD.plcSize;
                    decimal AdrValue = d[Idx];

                    tmpAVList.Add(PA.Address, AdrValue);
                    
                }
            }

            return tmpAVList;
        }

        // PLCデバイスから取得するデータの個数
        private int getAryReadLimit(int Index)
        {
            int retv = 1;

            List<PLC_Address> PAL = this.AddressList;
            int readLimit = this.readLimit;
            int plcSize = this.plcSize;

            // リストの最後なら1アドレスのみ取得
            if (Index == PAL.Count - 1)
            {
                retv = 1;
            }
            // 定義上の最大数を指定してもリスト上の次アドレスまで届かない場合は、1アドレスのみ取得
            else if (PAL[Index].AdrNum10 + readLimit * plcSize < PAL[Index + 1].AdrNum10)
            {
                retv = 1;
            }
            // 定義上の最大数を取得した場合の最終アドレスがアドレスリストの最後尾を超える場合、最後尾アドレスぴったりまで取得する
            else if (PAL[PAL.Count-1].AdrNum10 < PAL[Index].AdrNum10 + readLimit * plcSize)
            {
                retv = (PAL[PAL.Count-1].AdrNum10 + (plcSize - 1) - PAL[Index].AdrNum10 + 1) / plcSize;
            }
            else
            {
                retv = readLimit;
            }    

            return retv;
        }
    }

}
