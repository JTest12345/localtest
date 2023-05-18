using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class Location
    {
        public Location(int macno, Station st)
        {
            this.MacNo = macno;
            this.Station = st;
        }

        public string DirectoryPath
        {
            get
            {
                return Path.Combine(Convert.ToString(this.MacNo), Station.ToString());
            }
        }

        public int MacNo { get; set; }
        public Station Station { get; set; }
    }


    /// <summary>
    /// ステーション位置
    /// </summary>
    public enum Station
    {
		Loader,
		Unloader,
		EmptyMagazineLoader,
		EmptyMagazineUnloader,

		//複数同時処理設備（プラズマ等）
		Loader1,
		Loader2,
		Loader3,
		Loader4,
		Unloader1,
		Unloader2,
		Unloader3,
		Unloader4,
		EmptyMagazineLoader1,
		EmptyMagazineLoader2,
		EmptyMagazineLoader3,
		EmptyMagazineUnloader1,
		EmptyMagazineUnloader2,
		EmptyMagazineUnloader3,

		//遠心沈降機ダミーマガジン置き場
		DummyMagStationLoader,
		DummyMagStationUnloader,

        //機遠心沈降機(マガジン交換機搭載)
        Unloader5,
        Unloader6,
        Unloader7,
        EmptyMagazineLoader4,
        EmptyMagazineUnloader4,
	}
}
