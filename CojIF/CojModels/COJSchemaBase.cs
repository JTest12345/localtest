﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CojIF
{
    /// <summary>
    /// COJ基底クラスの構成クラス
    /// </summary>
    public class Header : ICojHeader
    {
        public string objectId { get; set; }
        public string objectType { get; set; }
        public string docNo { get; set; }
        public string createdBy { get; set; }
        public string createdAt { get; set; }
        public string updatedBy { get; set; }
        public string updatedAt { get; set; }
        public int revision { get; set; }
        public bool isRelesed { get; set; }
        public bool isDeleted { get; set; }
        public string division { get; set; }
        public string site { get; set; }
        public string procno { get; set; }
        public string plantcd { get; set; }
        public string macno { get; set; }
        public string typecd { get; set; }
        public string lotno { get; set; }
        public string magno { get; set; }
        public string description { get; set; }
    }

    public class CoHeader : ICoHeader
    {
        public bool hasValues { get; set; }
        public bool isClosed { get; set; }
    }

    public class MmStatus : IMmStatus
    {
        public string inpStatus { get; set; }
        public bool hasValues { get; set; }
        public bool isClosed { get; set; }
    }

    public class InStatus : IInStatus
    {
        public string inpStatus { get; set; }
        public bool hasValues { get; set; }
        public bool isClosed { get; set; }
    }

    public class Status
    {
        public MmStatus mmStatus { get; set; }
        public InStatus inStatus { get; set; }
    }

    public class CoList
    {
        public Status status { get; set; }
        public string schemaId { get; set; }
        public object Schema { get; set; }
        public object UiSchema { get; set; }
        public object formData { get; set; }
    }

    public class CejObject
    {
        public CoHeader coHeader { get; set; }
        public List<CoList> coList { get; set; }
    }

    /// <summary>
    /// COJ基底クラス
    /// </summary>
    /// 
    public class COJ
    {
        public Header header { get; set; }
        public CejObject cejObject { get; set; }

        //
        // ◇COJヘッダーのデータ格納状態を返すメソッド
        // 　
        public string GetCojHeader()
        {
            var ret = string.Empty;
            foreach (var prop in this.header.GetType().GetProperties())
            {
                ret += $"cojHeder.{prop.Name}: " + prop.GetValue(this.header) + "\r\n";
            }
            return ret;
        }

        //
        // ◇Objヘッダー(CejObjectのヘッダー)のデータ格納状態を返すメソッド
        // 　
        public string GetObjHeader()
        {
            var ret = string.Empty;
            foreach (var prop in this.cejObject.coHeader.GetType().GetProperties())
            {
                ret += $"objHeder.{prop.Name}: " + prop.GetValue(this.cejObject.coHeader) + "\r\n";
            }
            return ret;

        }
    }

}
