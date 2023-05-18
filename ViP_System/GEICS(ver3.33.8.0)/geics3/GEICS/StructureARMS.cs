using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GEICS
{
    public class LotStbInfo
    {
        /// <summary>ロット番号</summary>
        public string LotNO { get; set; }

        /// <summary>設備分類名</summary>
        public string PlantClasNM { get; set; }

        /// <summary>設備CD</summary>
        public string PlantCD { get; set; }

        /// <summary>設備名</summary>
        public string PlantNM { get; set;}
    }

    public class MaterialStbInfo
    {
        /// <summary>品目CD</summary>
        public string CMaterialCD { get; set; }

        /// <summary>ロット番号</summary>
        public string CLotNO { get; set; }

        /// <summary>材料品目グループCD</summary>
        public string MateGroupCD { get; set; }

        /// <summary>材料品目グループ名</summary>
        public string MateGroupNM { get; set; }

        /// <summary>材料品目CD</summary>
        public string MaterialCD { get; set; }

        /// <summary>材料品目名</summary>
        public string MaterialNM { get; set; }

        /// <summary>材料ロット番号</summary>
        public string LotNO { get; set; }

        /// <summary>交換日時</summary>
        public DateTime? ChangeDT { get; set; }

        /// <summary>実績完了日時</summary>
        public DateTime? CompleteDT { get; set; }

        /// <summary>設備CD</summary>
        public string PlantCD { get; set; }

        /// <summary>調合結果ID</summary>
        public string MixResultID { get; set; }
    }

}
