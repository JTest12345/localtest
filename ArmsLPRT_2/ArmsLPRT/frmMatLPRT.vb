Public Class frmMATLPRT

#Region "定義"

#End Region

#Region "定数"

#End Region

#Region "変数"

    Private clsDBIO As New clsDBIO
    Private bFst As Boolean

#End Region

#Region "プロパティ"

#End Region

#Region "イベント"

    ''' <summary>
    ''' 画面表示
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub frmMainMenu_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim sMSG As String = ""

        Try

            '***多重起動ﾁｪｯｸ
            If (clsCommon.fPrevInstance) Then
                clsMessage.fMsgStp("多重起動は出来ません。")
                Me.Close()
                Exit Sub
            End If

            bFst = True



            Call Watch.StartWatching()

            '画面クリア
            'Call subInitScreen()

            bFst = False

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' 印刷ボタン押下
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click

        'チェック
        With Me


            'ラベル印刷確認
            If MsgBox("ロットラベルを印刷しますか", MsgBoxStyle.Information + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
                Call frmPTouch.fncMatLPRT(Format(Today, "yyyymmdd"))
                '画面初期化
                'Call subInitScreen()
            End If

        End With

    End Sub

    ''' <summary>
    ''' 終了ボタン押下
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Call Watch.StopWatching()
        clsDBIO = Nothing
        Me.Close()

    End Sub

#End Region

#Region "関数"

    ''' <summary>
    ''' 画面初期化
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub subInitScreen()

        With Me

            '.lblMaterialNM.Text = ""

        End With

    End Sub


#End Region

End Class
