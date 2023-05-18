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

            '材料コード編集
            Call SetMaterialCD(sMSG)
            If sMSG <> "" Then
                MsgBox(sMSG)
                Me.Close()
                Exit Sub
            End If
            'ロット番号編集
            Call SetLotNo(Me.cmbMaterialCD.Text, sMSG)
            If sMSG <> "" Then
                MsgBox(sMSG)
                Me.Close()
                Exit Sub
            End If



            '画面クリア
            Call subInitScreen()

            bFst = False

        Catch ex As Exception

        End Try

    End Sub

    ''' <summary>
    ''' コンボボックス変更時
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub cmbLblKB_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbMaterialCD.SelectedIndexChanged

        Dim sMSG As String = ""

        If bFst Then
            Exit Sub
        End If

        With Me
            'ロット番号編集
            SetLotNo(.cmbMaterialCD.Text, sMSG)
            If sMSG <> "" Then
                MsgBox("TnMaterialsの検索に失敗しました")
            End If
            If .cmbMaterialCD.Text = String.Empty Then
                .lblLabelKB.Text = String.Empty
                .lblLabelNO.Text = String.Empty
                .lblMaterialNM.Text = String.Empty
            Else
                'TmMatLabel検索
                If clsDBIO.fncSelectTmMatLabel(.cmbMaterialCD.Text,
                                               .lblLabelKB.Text,
                                               .lblLabelNO.Text,
                                               .lblMaterialNM.Text) Then
                Else
                    .lblLabelKB.Text = String.Empty
                    .lblLabelNO.Text = String.Empty
                    .lblMaterialNM.Text = String.Empty
                    MsgBox("ラベルマスタの検索に失敗しました")
                    Exit Sub
                End If
            End If
        End With

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
            '材料コード
            If .cmbMaterialCD.Text = String.Empty Then
                MsgBox("材料コードを選択してください")
                .cmbMaterialCD.Focus()
                Exit Sub
            Else
                If cmbMaterialCD.SelectedIndex = -1 Then
                    MsgBox("材料コードを選択内容に誤りがあります")
                    .cmbMaterialCD.Focus()
                    Exit Sub
                End If
            End If

            If .lblLabelKB.Text <> String.Empty And
               .lblLabelNO.Text <> String.Empty Then
            Else
                MsgBox("材料コードの選択に誤りがあります")
                .cmbMaterialCD.Focus()
                Exit Sub
            End If
            'ロット番号
            If .cmbLotNo.Text = String.Empty Then
                MsgBox("ロット番号を選択てください")
                .cmbLotNo.Focus()
                Exit Sub
            Else
                If cmbLotNo.SelectedIndex = -1 Then
                    MsgBox("ロット番号の選択内容に誤りがあります")
                    .cmbMaterialCD.Focus()
                    Exit Sub
                End If
            End If

            'ラベル印刷確認
            If MsgBox("材料ラベルを印刷しますか", MsgBoxStyle.Information + MsgBoxStyle.OkCancel) = MsgBoxResult.Ok Then
                Call frmPTouch.fncMatLPRT(.lblLabelKB.Text,
                                          .lblLabelNO.Text,
                                          .cmbLotNo.Text,
                                          .lblMaterialNM.Text)
                '画面初期化
                Call subInitScreen()
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
            .cmbMaterialCD.SelectedIndex = 0
            '.cmbLotNo.SelectedIndex = 0
            .lblMaterialNM.Text = ""
            .lblLabelKB.Text = ""
            .lblLabelNO.Text = ""
            .cmbMaterialCD.Focus()
        End With

    End Sub

    ''' <summary>
    ''' コンボボックス（材料コード）編集
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetMaterialCD(ByRef psMSG As String)

        Dim dTBLMAT As New DataTable

        Call clsDBIO.fncGetMaterialCD(dTBLMAT, psMSG)

        If psMSG = "" Then
            With cmbMaterialCD
                .DataSource = dTBLMAT
                .DisplayMember = "materialcd"
                .SelectedIndex = 0
            End With
        End If

        'clsDBIO = Nothing

    End Sub

    ''' <summary>
    ''' コンボボックス（ロット番号）編集
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetLotNo(ByVal psMaterialcd As String, ByRef psMSG As String)

        Dim dTBLMAT As New DataTable

        Call clsDBIO.fncGetLotNo(psMaterialcd, dTBLMAT, psMSG)

        If psMSG = "" Then
            With cmbLotNo
                .DataSource = dTBLMAT
                .DisplayMember = "lotno"
                If .Items.Count = 0 Then
                    .Text = String.Empty
                End If
            End With
        End If

        'clsDBIO = Nothing

    End Sub

#End Region

End Class
