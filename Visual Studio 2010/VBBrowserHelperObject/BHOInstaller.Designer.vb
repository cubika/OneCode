
Partial Public Class BHOInstaller
    ''' <summary>
    ''' 需要设计器变量.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary> 
    ''' 清空用过的资源.
    ''' </summary>
    ''' <param name="disposing">如果托管资源需要被释放则为true;否则为false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Component Designer generated code"

    ''' <summary>
    ''' 需要设计器支持的方法 -不要用代码编辑器
    '''修改这个方法中的内容.
    ''' </summary>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
    End Sub

#End Region
End Class