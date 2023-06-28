Imports System.Configuration.ConfigurationManager

''' <summary>
''' コンフィグファイル読み込みクラスです。
''' </summary>
''' <remarks></remarks>
''' <historys>
''' 2021/04/01 新規作成
''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
''' </historys>
Public Class clsAppConfig

#Region ""

    Private _CON_SRV_SQL_STRING As String = ""

    Public Property CON_SRV_SQL_STRING() As String
        Get
            Return _CON_SRV_SQL_STRING
        End Get
        Set(ByVal value As String)
            _CON_SRV_SQL_STRING = value
        End Set
    End Property

    Private _AD_AUTH_STRING As String = ""

    Public Property AD_AUTH_STRING() As String
        Get
            Return _AD_AUTH_STRING
        End Get
        Set(ByVal value As String)
            _AD_AUTH_STRING = value
        End Set
    End Property

    Private _P_TOUCH_LAYOUT As String = ""

    Public Property P_TOUCH_LAYOUT() As String
        Get
            Return _P_TOUCH_LAYOUT
        End Get
        Set(ByVal value As String)
            _P_TOUCH_LAYOUT = value
        End Set
    End Property

    Private _FolderPath As String = ""

    Public Property FolderPath() As String
        Get
            Return _FolderPath
        End Get
        Set(ByVal value As String)
            _FolderPath = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' コンフィグファイル読み込みクラスです。
    ''' </summary>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>

    Public Sub New()

        If IsNothing(AppSettings("Connect.ConSrvSqlString")) Then
            CON_SRV_SQL_STRING = ""
        Else
            CON_SRV_SQL_STRING = AppSettings("Connect.ConSrvSqlString")
        End If

        If IsNothing(AppSettings("AD.Authentication")) Then
            AD_AUTH_STRING = ""
        Else
            AD_AUTH_STRING = AppSettings("AD.Authentication")
        End If

        If IsNothing(AppSettings("P_TOUCH_LAYOUT")) Then
            P_TOUCH_LAYOUT = ""
        Else
            P_TOUCH_LAYOUT = AppSettings("P_TOUCH_LAYOUT")
        End If

        If IsNothing(AppSettings("FolderPath")) Then
            FolderPath = ""
        Else
            FolderPath = AppSettings("FolderPath")
        End If

    End Sub

End Class
