using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;
using System.IO;
using System.Text;

namespace ArmsWeb.Models
{
    public class CasseteRelationModel
    {
        public string LotNo { get; set; }
        public List<string> listCassetteNo { get; set; }
        //public DateTime AttachDt { get; set; }
        //public int ProcNo { get; set; }

        public CasseteRelationModel(string magno)
        {
            string[] elms = magno.Split(' ');

            #region バーコードヘッダー部判定

            if (elms.Length == 2 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
            {
                magno = elms[1];
            }
            else if (elms.Length == 2 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
            {
                magno = elms[1];
            }
            else if (elms.Length == 4 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
            {
                magno = elms[1];
            }
            else
            {
                throw new ApplicationException("マガジンバーコードを読み込んでください");
            }
            #endregion

            this.listCassetteNo = new List<string>();
            this.LotNo = magno;

            this.listCassetteNo = Cassette.GetCassette(this.LotNo,1);
        }

        public void UpdateDB()
        {
            AsmLot asmLot = new AsmLot();
            ArmsApi.Model.LENS.Mag magData = new ArmsApi.Model.LENS.Mag();

            if (ArmsApi.Config.Settings.CreateAllOKMappingForMainte == true)
            {
                asmLot = AsmLot.GetAsmLot(this.LotNo);
                if(asmLot == null)
                {
                    throw new ApplicationException($"ARMSにロット情報が存在しません。ロット『{this.LotNo}』");
                }

                magData = ArmsApi.Model.LENS.Mag.GetData(asmLot.TypeCd);
                if (magData == null)
                {
                    throw new ApplicationException($"LENSに基板構成情報が存在ません。タイプ『{asmLot.TypeCd}』");
                }
            }

            foreach (var cassetno in this.listCassetteNo)
            {
                //他のロットに割りついている(別のロットでNewfgが1でHitした)場合はエラーとする
                var exists = Cassette.GetCassetteOther(this.LotNo, cassetno, 1);
                if (exists.CassetteNo != null)
                {
                    throw new ApplicationException("他のロットに割りついています[LOTNO]" + this.LotNo + "[RINGNO]" + cassetno);
                }

                exists = Cassette.GetCassette(this.LotNo, cassetno);
                if (exists.CassetteNo != null && exists.Newfg == 1)
                {
                    continue;
                }
                else if (exists.CassetteNo != null && exists.Newfg == 0)
                {
                    exists.Newfg = 1;
                    exists.Update();
                    continue;
                }

                ArmsApi.Model.Cassette cs = new Cassette();
                cs.LotNo = this.LotNo;
                cs.Newfg = 1;
                cs.SeqNo = cs.GetSeqNo();
                cs.CassetteNo = cassetno;
                cs.ProcNo = 1;
                cs.NextCassetteNo = null;
                cs.Lastupddt = DateTime.Now;
                cs.Attachdt = DateTime.Now;
                cs.Detachdt = null;

                //デフォルトマッピングデータ生成がONになっている場合はARMS4の出力先を取得してファイルが無い場合のみ
                //新規ファイルを生成する。トリガがARMS側にしかないのでマッピング系処理であるがARMSが管轄する。
                if (ArmsApi.Config.Settings.CreateAllOKMappingForMainte == true)
                {
                    List<string> files = DirectoryHelper.GetFiles(ArmsApi.Config.Settings.ARMS4MappingDirectoryPath, string.Format(@".*_{0}_{1}\.mpd$", this.LotNo, cs.SeqNo)).ToList();
                    if (files.Count() == 0)
                    {
                        string defaultMap = string.Empty;
                        for (int i = 0; i < magData.PackageQtyX * magData.PackageQtyY; i++)
                        {
                            if (i != 0)
                            {
                                defaultMap = defaultMap + ",";
                            }
                            defaultMap = defaultMap + "0";
                        }

                        string fileNm = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + this.LotNo + "_" + cs.SeqNo + ".mpd";

                        StreamWriter sw = new StreamWriter(Path.Combine(ArmsApi.Config.Settings.ARMS4MappingDirectoryPath, fileNm), true, Encoding.ASCII);
                        sw.Write(defaultMap);
                        sw.Close();
                    }
                }

                cs.CreateNew();
            }
        }
    }
}