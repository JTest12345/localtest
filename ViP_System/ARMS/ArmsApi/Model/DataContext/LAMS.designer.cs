﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArmsApi.Model.DataContext
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="LAMS")]
	public partial class LAMSDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region 拡張メソッドの定義
    partial void OnCreated();
    partial void InsertTnMachineRequire(TnMachineRequire instance);
    partial void UpdateTnMachineRequire(TnMachineRequire instance);
    partial void DeleteTnMachineRequire(TnMachineRequire instance);
    #endregion
		
		public LAMSDataContext() : 
				base(global::ArmsApi.Properties.Settings.Default.LAMSConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public LAMSDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LAMSDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LAMSDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LAMSDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<TnMachineRequire> TnMachineRequire
		{
			get
			{
				return this.GetTable<TnMachineRequire>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.TnMachineRequire")]
	public partial class TnMachineRequire : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _plantcd;
		
		private int _requireinputfg;
		
		private int _inputforbiddenfg;
		
		private System.DateTime _lastupddt;
		
    #region 拡張メソッドの定義
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnplantcdChanging(string value);
    partial void OnplantcdChanged();
    partial void OnrequireinputfgChanging(int value);
    partial void OnrequireinputfgChanged();
    partial void OninputforbiddenfgChanging(int value);
    partial void OninputforbiddenfgChanged();
    partial void OnlastupddtChanging(System.DateTime value);
    partial void OnlastupddtChanged();
    #endregion
		
		public TnMachineRequire()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_plantcd", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string plantcd
		{
			get
			{
				return this._plantcd;
			}
			set
			{
				if ((this._plantcd != value))
				{
					this.OnplantcdChanging(value);
					this.SendPropertyChanging();
					this._plantcd = value;
					this.SendPropertyChanged("plantcd");
					this.OnplantcdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_requireinputfg", DbType="Int NOT NULL")]
		public int requireinputfg
		{
			get
			{
				return this._requireinputfg;
			}
			set
			{
				if ((this._requireinputfg != value))
				{
					this.OnrequireinputfgChanging(value);
					this.SendPropertyChanging();
					this._requireinputfg = value;
					this.SendPropertyChanged("requireinputfg");
					this.OnrequireinputfgChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_inputforbiddenfg", DbType="Int NOT NULL")]
		public int inputforbiddenfg
		{
			get
			{
				return this._inputforbiddenfg;
			}
			set
			{
				if ((this._inputforbiddenfg != value))
				{
					this.OninputforbiddenfgChanging(value);
					this.SendPropertyChanging();
					this._inputforbiddenfg = value;
					this.SendPropertyChanged("inputforbiddenfg");
					this.OninputforbiddenfgChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_lastupddt", DbType="DateTime NOT NULL")]
		public System.DateTime lastupddt
		{
			get
			{
				return this._lastupddt;
			}
			set
			{
				if ((this._lastupddt != value))
				{
					this.OnlastupddtChanging(value);
					this.SendPropertyChanging();
					this._lastupddt = value;
					this.SendPropertyChanged("lastupddt");
					this.OnlastupddtChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591