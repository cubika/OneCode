'------------------------------------------------------------------------------
' <自动生成>
' 该代码由工具自动生成
' 运行时版本:2.0.50727.3521
'
' 修改该文件将导致异常，在重新生成代码后恢复
' </自动生成 >
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My.Resources
    
    ' 该类由 StronglyTypedResourceBuilder类通过类似ResGen 或Visual Studio的工具自动生成

    ' 需要添加或删除成员, 请编辑.ResX 文件，然后使用/str选项重新运行ResGen或重新构建VS项目 
    ' <总结>
    ' 强类型资源类,用于查询本地string等
    ' </总结>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '<总结>
        ' 返回被该类使用的缓存的ResourceManager 实例
        '</总结>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("VBAutomateExcel.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '<总结>
        ' 覆盖当前线程的CurrentUICulture属性
        ' 使用强类型资源类进行资源查询
        '</总结>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set(ByVal value As Global.System.Globalization.CultureInfo)
                resourceCulture = value
            End Set
        End Property
    End Module
End Namespace
