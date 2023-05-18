Public Class frmPTouch
    Inherits System.Windows.Forms.Form

#Region "変数一覧"

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

#Region " Windows フォーム デザイナで生成されたコード "

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

    End Sub

    ' Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    ' メモ : 以下のプロシージャは、Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使って変更してください。  
    ' コード エディタを使って変更しないでください。
    Friend WithEvents Label1 As System.Windows.Forms.Label
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("MS UI Gothic", 21.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(352, 80)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "材料ラベル印刷中．．"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmPTouch
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(368, 90)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmPTouch"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "材料ラベル印刷"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "関数"

    ''' <summary>
    ''' 材料ラベル印刷
    ''' </summary>
    ''' <param name="textstr"></param>

    ''' <remarks></remarks>
    Public Function fncMatLPRT(ByVal textstr As String) As Boolean

        'P-TOUCH用
        Dim ObjDoc As bpac.Document
        Dim bRet As Boolean
        Dim sPT_TempF As String
        Dim sBC As String

        'b-PACオブジェクト生成
        'ObjDoc = CreateObject("bpac.Document")
        ObjDoc = New bpac.Document

        Try
            fncMatLPRT = False

            sPT_TempF = mclsAppConfig.P_TOUCH_LAYOUT

            '帳票作成中ダイアログ表示
            Me.Show()
            Me.Update()

            'テンプレートファイル取得
            bRet = ObjDoc.Open(sPT_TempF)

            If bRet Then
                'バーコード編集
                sBC = textstr

                'テンプレートファイルへ転送
                ObjDoc.GetObject("materialnm").Text = "today"                          '材料名
                ObjDoc.GetObject("matBC39").Text = sBC                                      'バーコード

                '印刷を実行
                ObjDoc.StartPrint("MatLabel", bpac.PrintOptionConstants.bpoHalfCut)
                ObjDoc.PrintOut(1, bpac.PrintOptionConstants.bpoAutoCut)
                ObjDoc.EndPrint()
            Else
                'テンプレートファイルの取得に失敗しました
                MsgBox("テンプレートファイルの取得に失敗しました。")
                Return False
            End If

            Return True

        Catch ex As Exception
            'エラーメッセージ表示
            MsgBox(ex.Message)
            Return False
        Finally
            '画面を閉じる
            Me.Hide()
            'b-PACオブジェクト終了
            ObjDoc.Close()
            ObjDoc = Nothing
        End Try

    End Function

#End Region

End Class
