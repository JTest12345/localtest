Public Class clsDBIO

#Region "定義"

#End Region

#Region "定数"

#End Region

#Region "変数"

    Private clsDB As clsDB

#End Region

#Region "プロパティ"

    '共通情報保持クラス
    Private mclsAppConfig As New clsAppConfig
    Public Property clsAppConfig() As clsAppConfig
        Get
            Return mclsAppConfig
        End Get
        Set(value As clsAppConfig)
            mclsAppConfig = value
        End Set
    End Property

#End Region

#Region "構造体"

#End Region

#Region "関数"

    ''' <summary>
    ''' TmMatLabel検索（labelkb, labelnoを取得）
    ''' </summary>
    ''' <param name="psMaterialCD"></param>
    ''' <param name="psLabelKB"></param>
    ''' <param name="psLabelNO"></param>
    ''' <param name="psMaterialNm"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function fncSelectTmMatLabel(ByVal psMaterialCD As String,
                                        ByRef psLabelKB As String,
                                        ByRef psLabelNO As String,
                                        ByRef psMaterialNm As String) As Boolean

        Dim sSQL As String
        Dim dTBL As DataTable = Nothing

        Try

            'ＤＢ接続
            clsDB = New clsDB(mclsAppConfig.CON_SRV_SQL_STRING, clsGlobal.CONNECTION_TIME_OUT)

            'マスタ検索
            sSQL = "SELECT a.labelkb"
            sSQL = sSQL & Space(1) & "     , a.labelno"
            sSQL = sSQL & Space(1) & "     , ISNULL(b.materialnm, '') as materialnm"
            sSQL = sSQL & Space(1) & "  FROM TmMatLabel  a LEFT JOIN"
            sSQL = sSQL & Space(1) & "       TnMaterials b ON"
            sSQL = sSQL & Space(1) & "       a.materialcd = b.materialcd"
            sSQL = sSQL & Space(1) & " WHERE a.materialcd = '" & psMaterialCD & "'"
            sSQL = sSQL & Space(1) & "   AND a.labelkb LIKE 'C%'"

            clsDB.CreateCommand(sSQL, CommandType.Text, clsGlobal.CONNECTION_TIME_OUT)
            dTBL = clsDB.ExecuteReader()

            If dTBL.Rows.Count = 0 Then
                Return False
            Else
                psLabelKB = dTBL.Rows(0).Item(dTBL.Columns(0).Caption)
                psLabelNO = dTBL.Rows(0).Item(dTBL.Columns(1).Caption)
                psMaterialNm = dTBL.Rows(0).Item(dTBL.Columns(2).Caption)
            End If

            Return True

        Catch ex As Exception
            Return False
        Finally
            If Not IsNothing(dTBL) Then
                dTBL.Dispose()
                clsDB.Dispose()
            End If
            If Not IsNothing(clsDB) Then
                clsDB.Dispose()
            End If
        End Try

    End Function

    ''' <summary>
    ''' TmMatLabel.materialcd取得
    ''' </summary>
    ''' <param name="pdTBL"></param>
    ''' <param name="psErrMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function fncGetMaterialCD(ByRef pdTBL As DataTable,
                                     ByRef psErrMsg As String) As Boolean

        Dim sSQL As String

        Try
            'ＤＢ接続
            clsDB = New clsDB(mclsAppConfig.CON_SRV_SQL_STRING, clsGlobal.CONNECTION_TIME_OUT)

            'マスタ検索
            sSQL = "SELECT '' as materialcd"
            sSQL = sSQL & Space(1) & "UNION ALL"
            sSQL = sSQL & Space(1) & "SELECT RTRIM(materialcd) as materialcd"
            sSQL = sSQL & Space(1) & "  FROM TmMatLabel"
            sSQL = sSQL & Space(1) & " WHERE labelkb LIKE 'C%'"

            With clsDB
                .CreateCommand(sSQL, CommandType.Text, clsGlobal.CONNECTION_TIME_OUT)
                pdTBL = .ExecuteReader()
            End With

            Return True

        Catch ex As Exception
            psErrMsg = ex.Message.ToString
            Return False
        Finally
            If Not IsNothing(clsDB) Then
                clsDB.Dispose()
            End If
        End Try

    End Function

    ''' <summary>
    ''' TmMatLabel.lotno取得
    ''' </summary>
    ''' <param name="psMaterialCD"></param>
    ''' <param name="pdTBL"></param>
    ''' <param name="psErrMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function fncGetLotNo(ByVal psMaterialCD As String,
                                ByRef pdTBL As DataTable,
                                ByRef psErrMsg As String) As Boolean

        Dim sSQL As String

        Try
            'ＤＢ接続
            clsDB = New clsDB(mclsAppConfig.CON_SRV_SQL_STRING, clsGlobal.CONNECTION_TIME_OUT)

            'マスタ検索
            'sSQL = "SELECT '' as lotno"
            'sSQL = sSQL & Space(1) & "UNION ALL"
            sSQL = "SELECT RTRIM(lotno) as lotno"
            sSQL = sSQL & Space(1) & "  FROM TnMaterials"
            sSQL = sSQL & Space(1) & " WHERE materialcd = '" & psMaterialCD & "'"

            With clsDB
                .CreateCommand(sSQL, CommandType.Text, clsGlobal.CONNECTION_TIME_OUT)
                pdTBL = .ExecuteReader()
            End With

            Return True

        Catch ex As Exception
            psErrMsg = ex.Message.ToString
            Return False
        Finally
            If Not IsNothing(clsDB) Then
                clsDB.Dispose()
            End If
        End Try

    End Function

#End Region

End Class
