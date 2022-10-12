using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Oskas;

namespace ProcMasterIF
{
    class DbKeeper : Oskas.fmMain
    {
        string crlf = "\r\n";
        string msg;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        public bool interLock = false;

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btServerStart
            // 
            this.btServerStart.Text = "Make JSON";
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(512, 536);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(127, 30);
            this.button1.TabIndex = 11;
            this.button1.Text = "Make Arms SQL";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(379, 536);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(127, 30);
            this.button2.TabIndex = 12;
            this.button2.Text = "Make ProcMaster";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DbKeeper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "DbKeeper";
            this.Text = "DBKeeper";
            this.Controls.SetChildIndex(this.consoleBox, 0);
            this.Controls.SetChildIndex(this.btServerStart, 0);
            this.Controls.SetChildIndex(this.ErrorLogComsole, 0);
            this.Controls.SetChildIndex(this.bt_ClearErrLogs, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.button2, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        public DbKeeper()
        {
            InitializeComponent();
            msg = "起動しました";
            OskNLog.Log(msg, Cnslcnf.msg_info);
        }


        private void makeCoJson()
        {
            if (!interLock)
            {
                interLock = true;

                Task readHankan = Task.Run(() =>
                {
                    var jci = new JissekiCojIF();

                    if (!jci.Excute())
                    {
                        interLock = false;
                        return;
                    }

                    toolStripStatusLabel2.Text = "";
                    interLock = false;

                    msg = "makeを完了しました。";
                    OskNLog.Log(msg, Cnslcnf.msg_info);

                });
            }
        }

        private void makeArmsMaster()
        {
            if (!interLock)
            {
                interLock = true;

                Task readHankan = Task.Run(() =>
                {
                    var asi = new ArmsSqlIF();

                    if (!asi.Excute())
                    {
                        interLock = false;
                        return;
                    }

                    toolStripStatusLabel2.Text = "";
                    interLock = false;

                    msg = "makeを完了しました。";
                    OskNLog.Log(msg, Cnslcnf.msg_info);

                });
            }
        }

        private void makeprocMaster()
        {
            if (!interLock)
            {
                interLock = true;

                Task readHankan = Task.Run(() =>
                {
                    var pm = new MakeProcMst();

                    if (!pm.Excute())
                    {
                        interLock = false;
                        return;
                    }

                    toolStripStatusLabel2.Text = "";
                    interLock = false;

                    msg = "makeを完了しました。";
                    OskNLog.Log(msg, Cnslcnf.msg_info);

                });
            }
        }

        private void btServerStart_Click(object sender, EventArgs e)
        {
            makeCoJson();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            makeArmsMaster();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            makeprocMaster();
        }
    }

}
