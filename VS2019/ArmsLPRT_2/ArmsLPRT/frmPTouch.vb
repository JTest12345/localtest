Public Class frmPTouch
    Inherits System.Windows.Forms.Form

#Region "�ϐ��ꗗ"

#End Region

#Region "�v���p�e�B"

    '���ʏ��ێ��N���X
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

#Region " Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h "

    Public Sub New()
        MyBase.New()

        ' ���̌Ăяo���� Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
        InitializeComponent()

        ' InitializeComponent() �Ăяo���̌�ɏ�������ǉ����܂��B

    End Sub

    ' Form �́A�R���|�[�l���g�ꗗ�Ɍ㏈�������s���邽�߂� dispose ���I�[�o�[���C�h���܂��B
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Windows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    Private components As System.ComponentModel.IContainer

    ' ���� : �ȉ��̃v���V�[�W���́AWindows �t�H�[�� �f�U�C�i�ŕK�v�ł��B
    'Windows �t�H�[�� �f�U�C�i���g���ĕύX���Ă��������B  
    ' �R�[�h �G�f�B�^���g���ĕύX���Ȃ��ł��������B
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
        Me.Label1.Text = "�ޗ����x��������D�D"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmPTouch
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 12)
        Me.ClientSize = New System.Drawing.Size(368, 90)
        Me.Controls.Add(Me.Label1)
        Me.Name = "frmPTouch"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "�ޗ����x�����"
        Me.ResumeLayout(False)

    End Sub

#End Region

#Region "�֐�"

    ''' <summary>
    ''' �ޗ����x�����
    ''' </summary>
    ''' <param name="textstr"></param>

    ''' <remarks></remarks>
    Public Function fncMatLPRT(ByVal textstr As String) As Boolean

        'P-TOUCH�p
        Dim ObjDoc As bpac.Document
        Dim bRet As Boolean
        Dim sPT_TempF As String
        Dim sBC As String

        'b-PAC�I�u�W�F�N�g����
        'ObjDoc = CreateObject("bpac.Document")
        ObjDoc = New bpac.Document

        Try
            fncMatLPRT = False

            sPT_TempF = mclsAppConfig.P_TOUCH_LAYOUT

            '���[�쐬���_�C�A���O�\��
            Me.Show()
            Me.Update()

            '�e���v���[�g�t�@�C���擾
            bRet = ObjDoc.Open(sPT_TempF)

            If bRet Then
                '�o�[�R�[�h�ҏW
                sBC = textstr

                '�e���v���[�g�t�@�C���֓]��
                ObjDoc.GetObject("materialnm").Text = "today"                          '�ޗ���
                ObjDoc.GetObject("matBC39").Text = sBC                                      '�o�[�R�[�h

                '��������s
                ObjDoc.StartPrint("MatLabel", bpac.PrintOptionConstants.bpoHalfCut)
                ObjDoc.PrintOut(1, bpac.PrintOptionConstants.bpoAutoCut)
                ObjDoc.EndPrint()
            Else
                '�e���v���[�g�t�@�C���̎擾�Ɏ��s���܂���
                MsgBox("�e���v���[�g�t�@�C���̎擾�Ɏ��s���܂����B")
                Return False
            End If

            Return True

        Catch ex As Exception
            '�G���[���b�Z�[�W�\��
            MsgBox(ex.Message)
            Return False
        Finally
            '��ʂ����
            Me.Hide()
            'b-PAC�I�u�W�F�N�g�I��
            ObjDoc.Close()
            ObjDoc = Nothing
        End Try

    End Function

#End Region

End Class
