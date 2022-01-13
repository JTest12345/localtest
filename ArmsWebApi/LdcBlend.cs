using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsWebApi
{
    public class LdcBlend
    {

        public static bool mkblendlot(string plantcd, List<string> magnoarr, ref string msg)
        {
            try
            { 
                var bld = new ArmsWeb.Models.CutBlendModel(plantcd);


                // Btoで指定のマガジンが設備内のブレンド対象であるか確認し、ブレンドリスト(CurrentBlend)に追加
                var NoMag = true;
                bld.CurrentBlend.Clear();
                foreach (var magno in magnoarr)
                {
                    foreach (var bldmag in bld.BlendList)
                    {
                        if (magno == bldmag.MagNo)
                        {
                            NoMag = false;
                            bld.CurrentBlend.Add(bldmag);
                            break;
                        }
                        NoMag = true;
                    }
                }

                if (NoMag)
                {
                    msg = $"Btoファイルで指定されたマガジンNoに設備：{plantcd}のブレンド対象でないロットが含まれています";
                    return false;
                }

                // ブレンド実行
                try
                {
                    bld.FinishBlend();
                }
                catch (Exception ex)
                {
                    msg = "ブレンドロット完了エラー：" + ex.Message;
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;

            }
        }

    }
}
