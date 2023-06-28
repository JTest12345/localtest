Imports System.Diagnostics

''' <summary>
''' メッセージ管理クラスです。
''' </summary>
''' <remarks></remarks>
''' <historys>
''' 2021/04/01 新規作成
''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
''' </historys>
Public Class clsMessage
    ''' <summary>
    ''' ｴﾗｰﾒｯｾｰｼﾞを出力する。
    ''' </summary>
    ''' <param name="strProcName">strProcName: メソッド名。</param>
    ''' <param name="varErrNo">varErrNo: エラー番号。</param>
    ''' <param name="strErrMsg">strErrMsg: エラーメッセージ。</param>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Sub funMsgErr(ByVal strProcName As String, _
                                ByVal varErrNo As Integer, _
                                ByVal strErrMsg As String)
        Dim intMode As MsgBoxStyle
        Dim strTitle As String
        Dim strMessege As String

        intMode = MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical
        strTitle = strProcName & " エラー"
        strMessege = "エラーが発生しました。" & vbCrLf & vbCrLf
        strMessege = strMessege & "** ｴﾗｰ番号：" & varErrNo & vbCrLf
        strMessege = strMessege & "** ｴﾗｰ内容：" & strErrMsg
        MsgBox(strMessege, intMode, strTitle)
    End Sub

    ''' <summary>
    ''' エラーメッセージを出力します。
    ''' </summary>
    ''' <param name="ex">Exception: 例外オブジェクト。</param>
    ''' <remarks>エラー番号は表示対象外とします。</remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Sub funMsgErr(ByVal ex As Exception)
        Dim st As New StackTrace(ex)

        Dim className As String = st.GetFrame(0).GetMethod.ReflectedType.Name
        Dim methodName As String = st.GetFrame(0).GetMethod.Name

        Dim intMode As MsgBoxStyle
        Dim strTitle As String
        Dim strMessege As String
        intMode = MsgBoxStyle.OkOnly Or MsgBoxStyle.Critical
        strTitle = className & "." & methodName & " エラー"
        strMessege = "エラーが発生しました。" & vbCrLf & vbCrLf
        'strMessege = strMessege & "** ｴﾗｰ番号：" & Err.Number & vbCrLf
        strMessege = strMessege & "** ｴﾗｰ内容：" & ex.Message
        MsgBox(strMessege, intMode, strTitle)
    End Sub

    ''' <summary>
    ''' 確認ﾒｯｾｰｼﾞを出力する。
    ''' </summary>
    ''' <param name="strMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fMsgInf(ByVal strMsg As String) As MsgBoxResult
        Return MsgBox(strMsg, MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, "確認")
    End Function

    ''' <summary>
    ''' 確認ﾒｯｾｰｼﾞを出力する。
    ''' </summary>
    ''' <param name="strMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fMsgStp(ByVal strMsg As String) As MsgBoxResult
        Return MsgBox(strMsg, MsgBoxStyle.Critical Or MsgBoxStyle.OkOnly, "確認")
    End Function

    ''' <summary>
    ''' 確認ﾒｯｾｰｼﾞを出力する。
    ''' </summary>
    ''' <param name="strMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 新規作成
    ''' yyyy/MM/dd XXX)XXXXXXXX ＸＸＸＸＸＸ
    ''' </historys>
    Public Shared Function fMsgExc(ByVal strMsg As String) As MsgBoxResult
        Return MsgBox(strMsg, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, "注意")
    End Function

End Class
