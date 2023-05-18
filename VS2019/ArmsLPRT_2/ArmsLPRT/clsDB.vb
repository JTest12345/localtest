Imports System.Data.SqlClient
Imports System.Data

''' <summary>
''' データベース管理クラスです。
''' </summary>
''' <remarks></remarks>
''' <history>
''' 2021/04/01 新規作成
''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
''' </history>
Public Class clsDB
    Implements IDisposable

    Private _dbCnn As SqlConnection
    Private _dbTrn As SqlTransaction
    Private _command As SqlCommand
    Private _disposedValue As Boolean = False        ' 重複する呼び出しを検出するには

    ''' <summary>
    ''' データベースに接続します。
    ''' </summary>
    ''' <param name="connect">接続するデータベースのコネクション文字列を指定します。</param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub New(ByVal connect As String, _
                   Optional ByVal timeOut As Integer = clsGlobal.CONNECTION_TIME_OUT)
        _dbCnn = New SqlConnection(connect & "Connection Timeout=" & timeOut & ";")
        _dbCnn.Open()
    End Sub

    ''' <summary>
    ''' データベースをクローズします。
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Private Sub Close()
        If Not IsNothing(_dbCnn) Then
            _dbCnn.Close()
            _dbCnn.Dispose()
        End If
    End Sub

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me._disposedValue Then
            If disposing Then
                ' TODO: 他の状態を解放します (マネージ オブジェクト)。
            End If

            ' TODO: ユーザー独自の状態を解放します (アンマネージ オブジェクト)。
            ' TODO: 大きなフィールドを null に設定します。
            Close()
        End If
        Me._disposedValue = True
    End Sub

#Region " IDisposable Support "
    ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    ''' <summary>
    ''' トランザクションを開始します。
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub BeginTransaction()
        _dbTrn = _dbCnn.BeginTransaction(IsolationLevel.ReadCommitted)
    End Sub

    ''' <summary>
    ''' トランザクションをコミットします。
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub CommitTransaction()
        If _dbTrn Is Nothing = False Then
            _dbTrn.Commit()
            _dbTrn.Dispose()
            _dbTrn = Nothing
            GC.Collect()
        End If
    End Sub

    ''' <summary>
    ''' トランザクションをロールバックします。
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub RollbackTransaction()
        If _dbTrn Is Nothing = False Then
            _dbTrn.Rollback()
            _dbTrn.Dispose()
            _dbTrn = Nothing
            GC.Collect()
        End If
    End Sub

    ''' <summary>
    ''' コマンドを作成します。
    ''' </summary>
    ''' <param name="commandText">実行するコマンドのテキストを指定します。</param>
    ''' <param name="commandType">実行するコマンドのタイプを指定します。（既定値：Data.CommandType.Text）</param>
    ''' <param name="commandTimeOut">実行するコマンドのタイムアウトする時間を指定します。（既定値：clsGrobal.COMMAND_TIME_OUT）</param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub CreateCommand(ByVal commandText As String, _
                             Optional ByVal commandType As CommandType = Data.CommandType.Text, _
                             Optional ByVal commandTimeOut As Integer = clsGlobal.COMMAND_TIME_OUT)
        If Not _dbCnn Is Nothing Then
            _command = _dbCnn.CreateCommand
            _command.CommandTimeout = commandTimeOut
            _command.CommandType = commandType
            _command.CommandText = commandText
        End If
    End Sub

    ''' <summary>
    ''' パラメーターを追加します。
    ''' </summary>
    ''' <param name="param">SQLパラメーターを指定します。</param>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub AddParameter(ByVal param As SqlParameter)
        _command.Parameters.Add(param)
    End Sub

    ''' <summary>
    ''' パラメーターをクリアします。
    ''' </summary>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Sub ClearParameter()
        _command.Parameters.Clear()
    End Sub

    ''' <summary>
    ''' 結果を返さないSQL(Insert/Update/Delete)を実行する
    ''' </summary>
    ''' <returns>処理後、影響を受けたレコード数</returns>
    ''' <remarks>トランザクションはCommitTransaction関数とRollbackTransactionで制御すること</remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Function ExecuteNonQuery() As Integer
        If Not IsNothing(_dbTrn) Then
            _command.Transaction = _dbTrn
        End If
        Return _command.ExecuteNonQuery()
    End Function

    ''' <summary>
    ''' コマンドに指定された処理を行います。
    ''' </summary>
    ''' <returns>DataTable: 実行にて得られた DataTable を返却します。</returns>
    ''' <remarks>当メソッドは ExecuteProcedure のエイリアスです。</remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Function ExecuteReader() As DataTable
        Return ExecuteProcedure()
    End Function

    ''' <summary>
    ''' ストアドプロシージャを実行します。
    ''' </summary>
    ''' <returns>DataTable: 実行にて得られた DataTable を返却します。</returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Function ExecuteProcedure() As DataTable
        Dim resultDataTable As New DataTable
        Using da As New SqlDataAdapter(_command)
            da.SelectCommand.Transaction = _dbTrn
            da.Fill(resultDataTable)
        End Using
        Return resultDataTable
    End Function

    ''' <summary>
    ''' 単項目の問い合わせを行ないます。
    ''' </summary>
    ''' <param name="strColName">strColName: 項目名です。</param>
    ''' <param name="strTblName">strTblName: テーブル名です。</param>
    ''' <param name="strWhereSql">strWhereSql: 条件を指定します。(Optional)</param>
    ''' <returns>string: 取得した値</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Function fDLookup(ByVal strColName As String, _
                               ByVal strTblName As String, _
                               Optional ByVal strWhereSql As String = "") As String
        If Len(strColName) = 0 Or _
           Len(strTblName) = 0 Then
            Return String.Empty
        End If

        Dim strSql As String
        Dim adoSnp As DataTable

        strSql = "SELECT " & strColName & " AS DAT_COLUMN"
        strSql = strSql & " FROM " & strTblName
        If Len(strWhereSql) > 0 Then
            strSql = strSql & " WHERE " & strWhereSql
        End If

        CreateCommand(strSql)
        adoSnp = ExecuteReader()
        If IsNothing(adoSnp) Then
            Return String.Empty
        End If

        With adoSnp
            If .Rows.Count = 0 Then
                Return String.Empty
            Else
                Return .Rows(0)("DAT_COLUMN").ToString
            End If
        End With
    End Function

    ''' <summary>
    ''' ◆SQL専用Exception
    ''' </summary>
    ''' <remarks>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </remarks>
    Public Class AppSQLException
        Inherits Exception

        Private _sqlex As SqlClient.SqlException

        Friend Sub New(ByVal sql As String, ByVal ex As SqlClient.SqlException)
            MyBase.New("SQL:" & sql & " " & ex.Message, ex)
            _sqlex = ex
        End Sub

        Public ReadOnly Property SqlEx() As SqlClient.SqlException
            Get
                Return _sqlex
            End Get
        End Property
    End Class
End Class
