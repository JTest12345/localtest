Imports System.Net.Dns
Imports System.Data

''' <summary>
''' システム共通クラスです。
''' </summary>
''' <remarks></remarks>
''' <history>
''' 2021/04/01 FJH)Sugimoto 新規作成
''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
''' </history>
Public Class clsCommon

    ''' <summary>
    ''' 入力された文字列中の任意の文字を任意の文字に置き換える。
    ''' 先頭と末尾の空白を削除した文字列を返す。
    ''' </summary>
    ''' <param name="varData">varData: 置換対象文字列</param>
    ''' <param name="strFind">strFind: 置換対象文字</param>
    ''' <param name="strReplace">strReplace: 置換文字</param>
    ''' <returns>String: 置換後文字列</returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Shared Function fChrChange(ByRef varData As String, _
                                      ByRef strFind As String, _
                                      ByRef strReplace As String) As String
        Try

            Dim strWk As String
            Dim i As Integer
            Dim j As Integer

            Dim strData As String
            If varData <> "" Then   'If Not IsDBNull(varData) Then
                strData = varData
                If Len(strData) > 0 Then
                    If InStr(1, strData, strFind) > 0 Then
                        strWk = "" : j = 1 : i = 0
                        Do
                            If InStr(j, strData, strFind) > 0 Then
                                i = InStr(j, strData, strFind)
                                strWk = strWk & Mid(strData, j, i - j) & strReplace
                            Else
                                strWk = strWk & Mid(strData, j, Len(strData) - i)
                                Exit Do
                            End If
                            j = i + Len(strFind)
                            If j > Len(strData) Then Exit Do
                        Loop
                        strWk = Trim(strWk)
                        fChrChange = strWk
                        Exit Function
                    Else
                        fChrChange = strData
                        Exit Function
                    End If
                End If
            End If

            Return ""

        Catch ex As Exception
            Return ""
        End Try

    End Function

    ''' <summary>
    ''' Null値を指定の値に置き換える。
    ''' </summary>
    ''' <param name="varDat">varDat: 検査対象です。</param>
    ''' <param name="varRplace">varRplace: 代替文字です。(既定値:0)</param>
    ''' <returns>検査値がNullの場合、置換値を指定した場合はそれを、指定しない場合は０を返す</returns>
    ''' <remarks>検査値/置換値(入力データの末尾にNUllがある場合削除する)</remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fNz(ByVal varDat As Object, _
                               Optional ByVal varRplace As String = "0") As String
        If DBNull.Value.Equals(varDat) Then 'IsDBNull(varDat)
            Return varRplace
        Else
            If Len(varDat) < 1 Then
                Return varRplace
            Else
                Dim strDat As String = varDat.ToString
                If InStr(strDat, Chr(0)) > 0 Then
                    If InStr(strDat, Chr(0)) = 1 Then
                        Return varRplace
                    Else
                        Return Left(strDat, InStr(strDat, Chr(0)) - 1)
                    End If
                Else
                    Return strDat
                End If
            End If
        End If
    End Function

    ''' <summary>
    ''' 小数点以下の０を取り除いて返却します。
    ''' </summary>
    ''' <param name="rowItem">DataTable の Row.Item を指定します。</param>
    ''' <returns>小数点以下の０を取り除いた値の文字列(String型)</returns>
    ''' <remarks>
    ''' Decimal型の場合、小数点以下の０を切捨てた文字列を返却します。
    ''' 上記以外の型の場合、文字列を返却します。
    ''' </remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimtoo 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fTrimPoint(ByVal rowItem As Object) As String
        If DBNull.Value.Equals(rowItem) Then
            'DBNullの場合は、空文字を返却する。
            Return String.Empty
        End If
        Select Case Type.GetTypeCode(rowItem.GetType)
            Case TypeCode.Decimal
                'Decimal型の場合、小数点以下の０を取り除く
                Return CDec(rowItem).ToString("G29")
            Case Else
                'それ以外の場合、そのまま
                Return CStr(clsCommon.fNz(rowItem, ""))
        End Select
    End Function

    ''' <summary>
    ''' 同一プロセス名チェック
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>True：未起動、False:起動中</remarks>
    ''' <history>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </history>
    Public Shared Function fPrevInstance() As Boolean

        Try
            ' このアプリケーションのプロセス名を取得
            Dim stThisProcess As String = System.Diagnostics.Process.GetCurrentProcess().ProcessName
#If DEBUG Then
            'デバッグの場合、vshostが該当してしまうためコメント（#ifdef)化
#Else
            ' 同名のプロセスが他に存在する場合は、既に起動していると判断する
            If System.Diagnostics.Process.GetProcessesByName(stThisProcess).Length > 1 Then
                Return True
            End If
#End If

            Return False

        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' yyyyMMdd を yyyy/MM/dd 形式に変換します。
    ''' </summary>
    ''' <param name="strDate">strDate :yyyyMMdd形式の文字列で表現した日付</param>
    ''' <returns>String: yyyy/MM/dd 形式の文字列</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fFormatDate(ByVal strDate As String) As String
        Dim strTmpDate As String
        fFormatDate = strDate
        If Len(strDate) <> 8 Then Return String.Empty
        strTmpDate = Left(strDate, 4) & "/" & Mid(strDate, 5, 2) & "/" & Right(strDate, 2)
        If Not IsDate(strTmpDate) Then Return String.Empty
        fFormatDate = strTmpDate
        Return fFormatDate
    End Function

    ''' <summary>
    ''' yyyy/MM/dd を yyyyMMdd 形式に変換します。
    ''' </summary>
    ''' <param name="dtDate">dtDate :対象となる日付</param>
    ''' <returns>String: yyyyMMdd 形式の文字列</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fFormatDate2(ByVal dtDate As Date) As String
        fFormatDate2 = CStr(dtDate.ToString("yyyyMMdd"))
        Return fFormatDate2
    End Function

    ''' <summary>
    ''' 処理日時を取得します。
    ''' </summary>
    ''' <param name="psStyle">strStyle :0:yyyyMMdd,1:HHmmss</param>
    ''' <returns>String: yyyyMMdd or HHmmss</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fNowGet(ByVal psStyle As String) As String

        Dim sTmpDateTime As String

        Select Case psStyle
            Case "0"
                sTmpDateTime = CStr(DateTime.Now.ToString("yyyyMMdd"))
            Case "1"
                sTmpDateTime = CStr(DateTime.Now.ToString("HHmmss"))
            Case Else
                Return String.Empty
        End Select

        fNowGet = sTmpDateTime

        Return fNowGet

    End Function

    ''' <summary>
    ''' ＳＱＬでスペース・長さ０の文字列の場合、Nullに変換する(文字用)
    ''' </summary>
    ''' <param name="varData">varData :対象となる項目</param>
    ''' <returns>String: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fSQLStr(ByRef varData As Object) As String
        Dim strSqlData As String
        Dim strMoji As String
        Dim I As Integer

        If IsDBNull(varData) Then
            fSQLStr = "Null"
        ElseIf Trim(varData) = "" Then
            fSQLStr = "Null"
        Else
            strSqlData = ""
            For I = 1 To Len(varData)
                strMoji = Mid(varData, I, 1)
                If strMoji = "'" Then
                    strSqlData = strSqlData & "'"
                End If
                strSqlData = strSqlData & strMoji
            Next
            fSQLStr = "'" & strSqlData & "'"
        End If

    End Function

    ''' <summary>
    ''' ＳＱＬでスペース・長さ０の文字列の場合、０に変換する(数値用)
    ''' </summary>
    ''' <param name="varData">varData :対象となる項目</param>
    ''' <returns>Integer: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fSQLNum(ByRef varData As Object) As Integer

        If IsDBNull(varData) Then
            fSQLNum = 0
        ElseIf Trim(varData) = "" Then
            fSQLNum = 0
        Else
            fSQLNum = varData
        End If

    End Function

    ''' <summary>
    ''' ＳＱＬでスペース・長さ０の文字列の場合、０に変換する(浮動小数点用)
    ''' </summary>
    ''' <param name="varData">varData :対象となる項目</param>
    ''' <returns>Double: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fSQLDbl(ByRef varData As Object) As Double

        If IsDBNull(varData) Then
            fSQLDbl = 0
        ElseIf Trim(varData) = "" Then
            fSQLDbl = 0
        Else
            fSQLDbl = varData
        End If

    End Function

    ''' <summary>
    ''' Null変換(文字用)
    ''' </summary>
    ''' <param name="varData">varData :対象となる項目</param>
    ''' <returns>String: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fNVL(ByRef varData As Object) As String

        If IsDBNull(varData) Then
            fNVL = ""
        Else
            fNVL = varData
        End If

    End Function

    ''' <summary>
    ''' Null変換(数値用)
    ''' </summary>
    ''' <param name="varData">varData :対象となる項目</param>
    ''' <returns>Integer: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fIntNVL(ByRef varData As Object) As Integer

        If IsDBNull(varData) Then
            fIntNVL = 0
        Else
            fIntNVL = varData
        End If

    End Function

    ''' <summary>
    ''' 文字列からバイト数を指定して部分文字列を取得する。
    ''' </summary>
    ''' <param name="value">対象文字列。</param>
    ''' <param name="startIndex">開始位置。（バイト数）</param>
    ''' <param name="length">長さ。（バイト数）</param>
    ''' <returns>部分文字列。</returns>
    ''' <remarks>文字列は <c>Shift_JIS</c> でエンコーディングして処理を行います。</remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fSubstringByte(ByVal value As String, ByVal startIndex As Integer, ByVal length As Integer) As String
        Dim sjisEnc As System.Text.Encoding = System.Text.Encoding.GetEncoding("Shift_JIS")
        Dim byteArray() As Byte = sjisEnc.GetBytes(value)

        If byteArray.Length < startIndex + 1 Then
            Return ""
        End If

        If byteArray.Length < startIndex + length Then
            length = byteArray.Length - startIndex
        End If

        Dim cut As String = sjisEnc.GetString(byteArray, startIndex, length)

        ' 最初の文字が全角の途中で切れていた場合はカット
        Dim left As String = sjisEnc.GetString(byteArray, 0, startIndex + 1)
        Dim first As Char = value(left.Length - 1)
        If 0 < cut.Length AndAlso Not first = cut(0) Then
            cut = cut.Substring(1)
        End If

        ' 最後の文字が全角の途中で切れていた場合はカット
        left = sjisEnc.GetString(byteArray, 0, startIndex + length)

        Dim last As Char = value(left.Length - 1)
        If 0 < cut.Length AndAlso Not last = cut(cut.Length - 1) Then
            cut = cut.Substring(0, cut.Length - 1)
        End If

        Return cut
    End Function

End Class