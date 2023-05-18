/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC110 メインサーバーIP情報
 * 
 * 概略           : 流動検証用ダミーデータを生成するメインサーバーのIP情報を登録/編集します。
 * 
 * 作成           : 2016/08/08 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Reflection;
using System.IO;
using Excel;

namespace FVDC
{
    public partial class FVDC110 : Form
    {
        private string DragName             = "";
        private string SaveServerName       = "";
        public FVDC110()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 画面が読み込まれたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC110_Load(object sender, EventArgs e)
        {
            /// レジストリより保存していた情報を取得
            try
            {
                //デフォルト値の設定
                this.txtServer.Text         = (string)fvdcRegistry.GetRegistory(fvdcRegistry.fvdcRefistryKey.defaultServer);
                SaveServerName              = this.txtServer.Text;

                if ((this.txtServer.Text == null) || (this.txtServer.Text == ""))
                {
                    this.btnCancel.Enabled  = false;
                }
                else
                {
                    this.btnCancel.Enabled  = true;
                }
            }
            catch
            {
                this.txtServer.Text         = "";
                this.btnCancel.Enabled      = false;
            }
        }

        /// <summary>
        /// キャンセルが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 更新が押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            /// 必須文字が含まれてい無いとき
            if ((this.txtServer.Text.Trim() != "") && 
                ((!this.txtServer.Text.Trim().Contains("\\")) 
                    || (!this.txtServer.Text.Trim().Contains(".")))) return;
            
            try
            {
                Transfer.ServerName     = this.txtServer.Text.Trim().Replace("\\\\", "\\");
                
                /// サーバーがクリアされたとき
                if (Transfer.ServerName == "")
                {
                    /// レジストリに保存している内容をクリアする
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultServer);
                    }
                    catch { }
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultType);
                    }
                    catch { }
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultLine);
                    }
                    catch { }
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultLot);
                    }
                    catch { }
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultTestLot);
                    }
                    catch { }
                    try
                    {
                        fvdcRegistry.DelRegistory(fvdcRegistry.fvdcRefistryKey.defaultAddLot);
                    }
                    catch { }

                    this.Dispose();
                    System.Windows.Forms.Application.Exit();
                    return;
                }
                else
                {
                    /// サーバーにログイン可能かチェックする
                    try
                    {
                        using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrCheckDB, true))
                        {
                            if (sqlInfo.Transaction == null)
                            {
                                /// ログインエラー
                                MessageBox.Show("　サーバーに接続できません。\n\n　入力内容を確認願います。", "サーバー接続エラー",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Transfer.ServerName         = SaveServerName;
                            }
                            else
                            {
                                //入力された内容をレジストリに登録します。
                                fvdcRegistry.SetRegistory(fvdcRegistry.fvdcRefistryKey.defaultServer, (object)this.txtServer.Text.Trim().Replace("\\\\", "\\"));
                                this.Close();
                            }
                        }
                    }
                    catch 
                    {
                        Transfer.ServerName     = "";
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// ドラッグしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblServer_DragOver(object sender, DragEventArgs e)
        {
            /// ドラッグしたファイルの情報を取得する。
            try
            {
                object FileNasme    = e.Data.GetData("FileNameW");
                string[] strFile    = (string[])FileNasme;
                DragName            = strFile[0];
                e.Effect            = DragDropEffects.Link;
            }
            catch { }
        }

        /// <summary>
        /// ドラッグドロップしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblServer_DragDrop(object sender, DragEventArgs e)
        {
            StreamReader ArmsConfig;
            /// ドロップされたファイルがArmsConfig.xmlならファイル読込を行う
            try
            {
                if (DragName != "")
                {
                    this.TopMost                = true;
                    string[] sptName            = DragName.Split('\\');
                    string[] sptCheckName       = sptName[sptName.Length - 1].Split('.');
                    if (sptCheckName.Length == 1)
                    {
                        MessageBox.Show("　フォルダは選択出来ません。", "ファイル形式確認",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    /// ファイル形式チェック
                    if (!sptName[sptName.Length - 1].Contains("ArmsConfig.xml"))
                    {
                        MessageBox.Show("　ArmsConfig.xmlで無いため読込めません。", "ファイル確認",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    ArmsConfig                  = new StreamReader(DragName);
                    string ReadLine             = ArmsConfig.ReadLine();
                    for (int i = 0; !ArmsConfig.EndOfStream; i++)
                    {
                        if (ReadLine.Trim().Length > 0)
                        {
                            if (ReadLine.Contains("LocalConnString"))
                            {
                                string[] sptEq  = ReadLine.Split('=');
                                if (sptEq.Length > 2)
                                {
                                    string[] sptEn = sptEq[1].Split(';');
                                    if (sptEn.Length > 1)
                                    {
                                        if (sptEn[0].ToUpper().Contains("SQLEXPRESS"))
                                        {
                                            this.txtServer.Text     = sptEn[0].Replace("\\\\","\\");
                                            break;
                                        }
                                    }
                                }
                            }
                        } 
                        ReadLine                = ArmsConfig.ReadLine();
                    }
                }
            }
            catch { }
        }
    }
}
