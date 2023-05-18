Imports System.IO
Imports bpac
Public Class Watch
    Public Shared Watcher As FileSystemWatcher

    Public Shared Sub StartWatching()
        Watcher = New FileSystemWatcher()

        ' 監視するパス
        Watcher.Path = "C:\test1"

        ' ファイル名と最終書き込み時間
        Watcher.NotifyFilter = NotifyFilters.LastWrite Or NotifyFilters.FileName

        ' フィルタで監視するファイルを.txtのみにする
        Watcher.Filter = "*.txt"

        ' サブディレクトリ以下も監視する
        Watcher.IncludeSubdirectories = True

        ' 変更発生時のイベントを定義する
        AddHandler Watcher.Created, AddressOf Changed ' 新規作成
        AddHandler Watcher.Changed, AddressOf Changed ' 変更
        AddHandler Watcher.Deleted, AddressOf Changed ' 削除
        AddHandler Watcher.Renamed, AddressOf Changed ' リネーム

        ' 監視開始
        Watcher.EnableRaisingEvents = True

        ' 必要がなくなったら監視終了処理(StopWatching)を呼ぶ
    End Sub

    Public Shared Sub Changed(ByVal source As Object, ByVal e As FileSystemEventArgs)
        Dim sPT_TempF As String
        Dim bRet As Boolean

        Dim ObjDoc As bpac.Document
        ObjDoc = New bpac.Document

        sPT_TempF = "arms_material_lbl.lbx"
        bRet = ObjDoc.Open(sPT_TempF)

        Select Case e.ChangeType
            Case WatcherChangeTypes.Created
                'Console.WriteLine($"新規作成: {e.FullPath}")

            Case WatcherChangeTypes.Changed
                Console.WriteLine($"変更: {e.FullPath}")
            Case WatcherChangeTypes.Deleted
                Console.WriteLine($"削除: {e.FullPath}")
            Case WatcherChangeTypes.Renamed
                Dim renameEventArgs = DirectCast(e, RenamedEventArgs)
                Console.WriteLine($"リネーム: {renameEventArgs.OldFullPath} => {renameEventArgs.FullPath}")
        End Select
    End Sub

    Public Shared Sub StopWatching()
        If (Not IsNothing(Watcher)) Then
            Watcher.EnableRaisingEvents = False
            Watcher.Dispose()
        End If
    End Sub
End Class
