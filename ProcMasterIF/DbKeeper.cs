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
        public bool interLock = false;

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // btServerStart
            // 
            this.btServerStart.Text = "Start DBK";
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // DbKeeper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Name = "DbKeeper";
            this.Text = "DBKeeper";
            this.Shown += new System.EventHandler(this.DbKeeper_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        public DbKeeper()
        {
            InitializeComponent();
            msg = "起動しました";
            OskNLog.Log(msg, Cnslcnf.msg_info);
        }


        private void makeProcMaster()
        {
            if (!interLock)
            {
                interLock = true;

                Task readHankan = Task.Run(() =>
                {
                    //var pmpc = new ProcMstProtController();

                    //if (!pmpc.Excute())
                    //{
                    //    interLock = false;
                    //    return;
                    //}

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


        private void btServerStart_Click(object sender, EventArgs e)
        {
            makeProcMaster();
        }

        private void DbKeeper_Shown(object sender, EventArgs e)
        {

        }

    }

}
