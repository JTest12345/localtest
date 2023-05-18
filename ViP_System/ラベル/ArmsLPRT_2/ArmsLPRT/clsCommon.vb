Imports System.Net.Dns
Imports System.Data

''' <summary>
''' �V�X�e�����ʃN���X�ł��B
''' </summary>
''' <remarks></remarks>
''' <history>
''' 2021/04/01 FJH)Sugimoto �V�K�쐬
''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
''' </history>
Public Class clsCommon

    ''' <summary>
    ''' ���͂��ꂽ�����񒆂̔C�ӂ̕�����C�ӂ̕����ɒu��������B
    ''' �擪�Ɩ����̋󔒂��폜�����������Ԃ��B
    ''' </summary>
    ''' <param name="varData">varData: �u���Ώە�����</param>
    ''' <param name="strFind">strFind: �u���Ώە���</param>
    ''' <param name="strReplace">strReplace: �u������</param>
    ''' <returns>String: �u���㕶����</returns>
    ''' <remarks></remarks>
    ''' <history>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' Null�l���w��̒l�ɒu��������B
    ''' </summary>
    ''' <param name="varDat">varDat: �����Ώۂł��B</param>
    ''' <param name="varRplace">varRplace: ��֕����ł��B(����l:0)</param>
    ''' <returns>�����l��Null�̏ꍇ�A�u���l���w�肵���ꍇ�͂�����A�w�肵�Ȃ��ꍇ�͂O��Ԃ�</returns>
    ''' <remarks>�����l/�u���l(���̓f�[�^�̖�����NUll������ꍇ�폜����)</remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' �����_�ȉ��̂O����菜���ĕԋp���܂��B
    ''' </summary>
    ''' <param name="rowItem">DataTable �� Row.Item ���w�肵�܂��B</param>
    ''' <returns>�����_�ȉ��̂O����菜�����l�̕�����(String�^)</returns>
    ''' <remarks>
    ''' Decimal�^�̏ꍇ�A�����_�ȉ��̂O��؎̂Ă��������ԋp���܂��B
    ''' ��L�ȊO�̌^�̏ꍇ�A�������ԋp���܂��B
    ''' </remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimtoo �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
    ''' </historys>
    Public Shared Function fTrimPoint(ByVal rowItem As Object) As String
        If DBNull.Value.Equals(rowItem) Then
            'DBNull�̏ꍇ�́A�󕶎���ԋp����B
            Return String.Empty
        End If
        Select Case Type.GetTypeCode(rowItem.GetType)
            Case TypeCode.Decimal
                'Decimal�^�̏ꍇ�A�����_�ȉ��̂O����菜��
                Return CDec(rowItem).ToString("G29")
            Case Else
                '����ȊO�̏ꍇ�A���̂܂�
                Return CStr(clsCommon.fNz(rowItem, ""))
        End Select
    End Function

    ''' <summary>
    ''' ����v���Z�X���`�F�b�N
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>True�F���N���AFalse:�N����</remarks>
    ''' <history>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
    ''' </history>
    Public Shared Function fPrevInstance() As Boolean

        Try
            ' ���̃A�v���P�[�V�����̃v���Z�X�����擾
            Dim stThisProcess As String = System.Diagnostics.Process.GetCurrentProcess().ProcessName
#If DEBUG Then
            '�f�o�b�O�̏ꍇ�Avshost���Y�����Ă��܂����߃R�����g�i#ifdef)��
#Else
            ' �����̃v���Z�X�����ɑ��݂���ꍇ�́A���ɋN�����Ă���Ɣ��f����
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
    ''' yyyyMMdd �� yyyy/MM/dd �`���ɕϊ����܂��B
    ''' </summary>
    ''' <param name="strDate">strDate :yyyyMMdd�`���̕�����ŕ\���������t</param>
    ''' <returns>String: yyyy/MM/dd �`���̕�����</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' yyyy/MM/dd �� yyyyMMdd �`���ɕϊ����܂��B
    ''' </summary>
    ''' <param name="dtDate">dtDate :�ΏۂƂȂ���t</param>
    ''' <returns>String: yyyyMMdd �`���̕�����</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
    ''' </historys>
    Public Shared Function fFormatDate2(ByVal dtDate As Date) As String
        fFormatDate2 = CStr(dtDate.ToString("yyyyMMdd"))
        Return fFormatDate2
    End Function

    ''' <summary>
    ''' �����������擾���܂��B
    ''' </summary>
    ''' <param name="psStyle">strStyle :0:yyyyMMdd,1:HHmmss</param>
    ''' <returns>String: yyyyMMdd or HHmmss</returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' �r�p�k�ŃX�y�[�X�E�����O�̕�����̏ꍇ�ANull�ɕϊ�����(�����p)
    ''' </summary>
    ''' <param name="varData">varData :�ΏۂƂȂ鍀��</param>
    ''' <returns>String: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' �r�p�k�ŃX�y�[�X�E�����O�̕�����̏ꍇ�A�O�ɕϊ�����(���l�p)
    ''' </summary>
    ''' <param name="varData">varData :�ΏۂƂȂ鍀��</param>
    ''' <returns>Integer: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' �r�p�k�ŃX�y�[�X�E�����O�̕�����̏ꍇ�A�O�ɕϊ�����(���������_�p)
    ''' </summary>
    ''' <param name="varData">varData :�ΏۂƂȂ鍀��</param>
    ''' <returns>Double: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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
    ''' Null�ϊ�(�����p)
    ''' </summary>
    ''' <param name="varData">varData :�ΏۂƂȂ鍀��</param>
    ''' <returns>String: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
    ''' </historys>
    Public Shared Function fNVL(ByRef varData As Object) As String

        If IsDBNull(varData) Then
            fNVL = ""
        Else
            fNVL = varData
        End If

    End Function

    ''' <summary>
    ''' Null�ϊ�(���l�p)
    ''' </summary>
    ''' <param name="varData">varData :�ΏۂƂȂ鍀��</param>
    ''' <returns>Integer: </returns>
    ''' <remarks></remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
    ''' </historys>
    Public Shared Function fIntNVL(ByRef varData As Object) As Integer

        If IsDBNull(varData) Then
            fIntNVL = 0
        Else
            fIntNVL = varData
        End If

    End Function

    ''' <summary>
    ''' �����񂩂�o�C�g�����w�肵�ĕ�����������擾����B
    ''' </summary>
    ''' <param name="value">�Ώە�����B</param>
    ''' <param name="startIndex">�J�n�ʒu�B�i�o�C�g���j</param>
    ''' <param name="length">�����B�i�o�C�g���j</param>
    ''' <returns>����������B</returns>
    ''' <remarks>������� <c>Shift_JIS</c> �ŃG���R�[�f�B���O���ď������s���܂��B</remarks>
    ''' <historys>
    ''' 2021/04/01 FJH)Sugimoto �V�K�쐬
    ''' yyyy/MM/dd XXX)XXXXXXXX �w�w�w�w�w�w
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

        ' �ŏ��̕������S�p�̓r���Ő؂�Ă����ꍇ�̓J�b�g
        Dim left As String = sjisEnc.GetString(byteArray, 0, startIndex + 1)
        Dim first As Char = value(left.Length - 1)
        If 0 < cut.Length AndAlso Not first = cut(0) Then
            cut = cut.Substring(1)
        End If

        ' �Ō�̕������S�p�̓r���Ő؂�Ă����ꍇ�̓J�b�g
        left = sjisEnc.GetString(byteArray, 0, startIndex + length)

        Dim last As Char = value(left.Length - 1)
        If 0 < cut.Length AndAlso Not last = cut(cut.Length - 1) Then
            cut = cut.Substring(0, cut.Length - 1)
        End If

        Return cut
    End Function

End Class