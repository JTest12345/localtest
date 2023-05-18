Public Class Form1

    Private Watch As New Watch

    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim i As Integer
        i = 1
        Call Watch.StartWatching()


        'textボックスにパスを格納

        '定期実行される仕組みを作って、指定のパスにテキストファイルがあるか監視
        'テキストファイルの中身を見て、プリンタにデータを送り込んで印刷

    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim i As Integer
        i = 1

    End Sub
End Class
