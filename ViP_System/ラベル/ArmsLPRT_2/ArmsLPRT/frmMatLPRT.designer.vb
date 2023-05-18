<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMATLPRT
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.lblMaterialNM = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.cmbMaterialCD = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblLabelKB = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.lblLabelNO = New System.Windows.Forms.Label()
        Me.cmbLotNo = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(262, 190)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(70, 30)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "終了"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnPrint
        '
        Me.btnPrint.Location = New System.Drawing.Point(182, 190)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(70, 30)
        Me.btnPrint.TabIndex = 3
        Me.btnPrint.Text = "印刷"
        Me.btnPrint.UseVisualStyleBackColor = True
        '
        'lblMaterialNM
        '
        Me.lblMaterialNM.BackColor = System.Drawing.Color.Azure
        Me.lblMaterialNM.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblMaterialNM.Location = New System.Drawing.Point(82, 85)
        Me.lblMaterialNM.Name = "lblMaterialNM"
        Me.lblMaterialNM.Size = New System.Drawing.Size(250, 17)
        Me.lblMaterialNM.TabIndex = 7
        Me.lblMaterialNM.Text = "材料名称"
        Me.lblMaterialNM.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(16, 85)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "材料名"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(16, 15)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(62, 13)
        Me.Label6.TabIndex = 7
        Me.Label6.Text = "材料コード"
        '
        'cmbMaterialCD
        '
        Me.cmbMaterialCD.FormattingEnabled = True
        Me.cmbMaterialCD.Location = New System.Drawing.Point(82, 12)
        Me.cmbMaterialCD.Name = "cmbMaterialCD"
        Me.cmbMaterialCD.Size = New System.Drawing.Size(250, 21)
        Me.cmbMaterialCD.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(16, 111)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(64, 13)
        Me.Label2.TabIndex = 7
        Me.Label2.Text = "ラベル区分"
        '
        'lblLabelKB
        '
        Me.lblLabelKB.BackColor = System.Drawing.Color.Azure
        Me.lblLabelKB.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblLabelKB.Location = New System.Drawing.Point(82, 109)
        Me.lblLabelKB.Name = "lblLabelKB"
        Me.lblLabelKB.Size = New System.Drawing.Size(41, 17)
        Me.lblLabelKB.TabIndex = 7
        Me.lblLabelKB.Text = "cxx"
        Me.lblLabelKB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(132, 111)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(64, 13)
        Me.Label5.TabIndex = 7
        Me.Label5.Text = "ラベル番号"
        '
        'lblLabelNO
        '
        Me.lblLabelNO.BackColor = System.Drawing.Color.Azure
        Me.lblLabelNO.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblLabelNO.Location = New System.Drawing.Point(198, 109)
        Me.lblLabelNO.Name = "lblLabelNO"
        Me.lblLabelNO.Size = New System.Drawing.Size(39, 17)
        Me.lblLabelNO.TabIndex = 7
        Me.lblLabelNO.Text = "aa"
        Me.lblLabelNO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbLotNo
        '
        Me.cmbLotNo.FormattingEnabled = True
        Me.cmbLotNo.Location = New System.Drawing.Point(82, 39)
        Me.cmbLotNo.Name = "cmbLotNo"
        Me.cmbLotNo.Size = New System.Drawing.Size(250, 21)
        Me.cmbLotNo.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(16, 42)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(59, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "ロット番号"
        '
        'frmMATLPRT
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Lavender
        Me.ClientSize = New System.Drawing.Size(366, 252)
        Me.Controls.Add(Me.cmbLotNo)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cmbMaterialCD)
        Me.Controls.Add(Me.btnPrint)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.lblLabelNO)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.lblLabelKB)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lblMaterialNM)
        Me.Controls.Add(Me.Label3)
        Me.Font = New System.Drawing.Font("MS UI Gothic", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "frmMATLPRT"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ラベル印刷"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents lblMaterialNM As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cmbMaterialCD As System.Windows.Forms.ComboBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblLabelKB As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblLabelNO As System.Windows.Forms.Label
    Friend WithEvents cmbLotNo As System.Windows.Forms.ComboBox
    Friend WithEvents Label4 As System.Windows.Forms.Label

End Class
