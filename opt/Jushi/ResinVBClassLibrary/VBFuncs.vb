

Public Class VBFuncs

    ''' <summary>
    ''' 機種名が正規表現に当てはまるか確認する
    ''' </summary>
    ''' <param name="check_name">機種名</param>
    ''' <param name="regex_name">正規表現</param>
    ''' <returns></returns>
    Public Shared Function CheckProductName(check_name As String, regex_name As String) As Boolean

        If check_name Like regex_name Then

            Return True

        Else
            Return False

        End If

    End Function


End Class
