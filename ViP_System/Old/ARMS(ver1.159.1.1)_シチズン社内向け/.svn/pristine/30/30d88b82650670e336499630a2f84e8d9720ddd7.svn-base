using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ワイヤーボンダー (SIGMA用)
    /// </summary>
    public class WireBonder2 : WireBonder
    {
        protected override void concreteThreadWork()
        {
            try
            {
                //マガジン排出時
                workCompleteHigh();

                //作業開始
                //checkWorkStart();

                //基板作業処理ありの場合
                if (this.IsSubstrateComplete)
                {
                    //基板作業開始
                    base.SubstrateWorkStart();

                    //基板作業完了
                    base.SubstrateWorkComplete();
                }

                //Nasca不良ファイル取り込み
                Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        private void workCompleteHigh()
        {
            VirtualMag unloadermag = this.Peek(Station.Unloader);
            if (unloadermag != null)
            {
                return;
            }

            VirtualMag oldmag = this.Peek(Station.Loader);
            if (oldmag == null)
            {
                return;
            }

            string[] files = System.IO.Directory.GetFiles(this.LogOutputDirectoryPath, "*.*");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^MP.*$");
            List<string> mlfiles = files.Where(p => regex.IsMatch(System.IO.Path.GetFileName(p))).ToList();
            if (mlfiles.Count == 0)
            {
                return;
            }

            Magazine svrmag = Magazine.GetCurrent(oldmag.MagazineNo);
            if (svrmag == null) throw new ApplicationException("稼働中マガジン情報が見つかりません:" + oldmag.MagazineNo);
            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            if (svrlot == null) throw new ApplicationException("ロット情報が見つかりません:" + svrmag.NascaLotNO);

            foreach (string file in files)
            {
                //MPDファイルはリネームしない
                string fileExtension = System.IO.Path.GetExtension(file).ToLower();
                if (fileExtension == ".mpd")
                {
                    continue;
                }

                MachineLog.ChangeFileName(file, svrlot.NascaLotNo, svrlot.TypeCd, oldmag.ProcNo.Value, oldmag.MagazineNo);
                OutputSysLog(string.Format("[完了処理] ファイル名称変更 FileName:{0}", file));
            }

            oldmag.LastMagazineNo = oldmag.MagazineNo;
            oldmag.WorkComplete = DateTime.Now;

            this.Enqueue(oldmag, Station.Unloader);
            this.Dequeue(Station.Loader);
        }
    }
}
