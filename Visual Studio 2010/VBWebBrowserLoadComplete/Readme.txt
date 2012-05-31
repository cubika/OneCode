================================================================================
       Windows 应用程序: VBWebBrowserLoadComplete 概述                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
摘要:

这个实例演示了如何确定页面在 WebBrowser 控件中完成加载. 

在页面没有嵌套框架的情况下，DocumentComplete 事件在所有事情完成后，会被引发一次.
当页面有多个嵌套框架时， DocumentComplete 事件会被多次引发. 

要检查一个页面是否已经加载完全,你需要检查是否事件的发送次数与WebBrowser控件的相同.

注意:
1. 在内嵌框架集的一个内嵌框架中，如果用户单击一个链接，该链接在其自身的页面中打开
   一个新页，并保持内嵌框架集的其余部分保持不变，WebBrowser 控件的 LoadCompleted 
   事件将不会被激发，你需要检查该特定内嵌框架的 DocumentComplete 事件. 

2. 如果你访问一些页面,例如 http://www.microsoft.com, 你可能发现，LoadCompleted 事
   件不是最后的事件.这是因为，在页面加载完成后,该页面可能自己加载其他链接.


////////////////////////////////////////////////////////////////////////////////
演示:

步骤1.	运行 VBWebBrowserLoadComplete.exe.

步骤2. Textbox 控件中默认的 url 是 "\Resource\FramesPage.htm" 的路径.
       点击按钮"Go". 

       FramesPage.htm 包含3个内嵌框架. 在该页面加载之后，你将在窗体的底部得到以下消
	   息：

       DocumentCompleted:4 LoadCompleted:1.

       这个消息表明，DocumentCompleted 事件被引发了4次，LoadCompleted 事件被引发了
	   一次.

       你也能在列表框里面得到详细的活动记录.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. DWebBrowserEvents2 接口指定了,应用程序必须实现从 WebBrowser 控件或 Windows 
   Internet Explorer 应用程序中，接收事件通知的事件接收接口. 事件通知包括将用于此应 
   用程序的 DocumentCompleted 和 BeforeNavigate2 的事件.

       <ComImport(),
       TypeLibType(TypeLibTypeFlags.FHidden),
       InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
       Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")>
       Public Interface DWebBrowserEvents2
          ''' <summary>
          ''' Fires when a document is completely loaded and initialized.
          ''' </summary>
          <DispId(259)>
          Sub DocumentComplete(<[In](), MarshalAs(UnmanagedType.IDispatch)> ByVal pDisp As Object,
                               <[In]()> ByRef URL As Object)
      
          <DispId(250)>
          Sub BeforeNavigate2(<[In](), MarshalAs(UnmanagedType.IDispatch)> ByVal pDisp As Object,
                              <[In]()> ByRef URL As Object,
                              <[In]()> ByRef flags As Object,
                              <[In]()> ByRef targetFrameName As Object,
                              <[In]()> ByRef postData As Object,
                              <[In]()> ByRef headers As Object,
                              <[In](), Out()> ByRef cancel As Boolean)
       End Interface

2. DWebBrowserEvents2Helper 类实现了 DWebBrowserEvents2 接口来检查是否完成页面加载.
   
   如果 WebBrowser 控件装载一个普通的、无内嵌框架的 HTML 页面, 在所有事情完成之后,
   DocumentComplete 事件会被引发一次.
   
   如果 WebBrowser 控件装载了许多内嵌框架, DocumentComplete 会被多次引发.
   DocumentComplete 事件有一个 pDisp 参数,它是框架( shdocvw )的 IDispatch.该框架中
   DocumentComplete 被引发.  
   
   然后我们可以检查是否 DocumentComplete 的 pDisp 参数与浏览器的 ActiveXInstance 相同. 


       Private Class DWebBrowserEvents2Helper
        Inherits StandardOleMarshalObject
        Implements DWebBrowserEvents2

        Private parent As WebBrowserEx

        Public Sub New(ByVal parent As WebBrowserEx)
            Me.parent = parent
        End Sub

        ''' <summary>
        ''' 当文件完全加载和初始化时，引发.
        ''' 如果框架是顶层窗口元素,那么该页面就加载完全.
        ''' 
        ''' 然后,在 WebBrowser 完全加载之后,重置 glpDisp 为 null.
        ''' </summary>
        Public Sub DocumentComplete(ByVal pDisp As Object, ByRef URL As Object) _
             Implements DWebBrowserEvents2.DocumentComplete

            Dim _url As String = TryCast(URL, String)

            If String.IsNullOrEmpty(_url) OrElse _
                _url.Equals("about:blank", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            If pDisp IsNot Nothing AndAlso pDisp.Equals(parent.ActiveXInstance) Then
                Dim e = New WebBrowserDocumentCompletedEventArgs(New Uri(_url))

                parent.OnLoadCompleted(e)
            End If
        End Sub

        ''' <summary>
        ''' 在给定对象中(一个窗口元素或者一个框架集元素)导航发生前引发.
        ''' 
        ''' </summary>
        Public Sub BeforeNavigate2(ByVal pDisp As Object,
                                   ByRef URL As Object,
                                   ByRef flags As Object,
                                   ByRef targetFrameName As Object,
                                   ByRef postData As Object,
                                   ByRef headers As Object,
                                   ByRef cancel As Boolean) _
                               Implements DWebBrowserEvents2.BeforeNavigate2

            Dim _url As String = TryCast(URL, String)

            If String.IsNullOrEmpty(_url) OrElse _
                _url.Equals("about:blank", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            If pDisp IsNot Nothing AndAlso pDisp.Equals(parent.ActiveXInstance) Then
                Dim e As New WebBrowserNavigatingEventArgs(
                    New Uri(_url), TryCast(targetFrameName, String))

                parent.OnStartNavigating(e)
            End If
        End Sub


    End Class
            
3. WebBrowserEx 类继承的浏览器类，并提供 StartNavigating 和 LoadCompleted 的事件.

         <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust"),
         PermissionSetAttribute(SecurityAction.InheritanceDemand, Name:="FullTrust")>
         Partial Public Class WebBrowserEx
             Inherits WebBrowser
             Private cookie As AxHost.ConnectionPointCookie
         
             Private helper As DWebBrowserEvents2Helper
         
             Public Event LoadCompleted As EventHandler(Of WebBrowserDocumentCompletedEventArgs)
         
             Public Event StartNavigating As EventHandler(Of WebBrowserNavigatingEventArgs)
         
             ''' <summary>
             ''' 将底层的 ActiveX 控件与可以处理控件事件包括 NavigateError 事件的客户端 
             ''' 关联起来.
             ''' </summary>
             Protected Overrides Sub CreateSink()
                 MyBase.CreateSink()
         
                 helper = New DWebBrowserEvents2Helper(Me)
                 cookie = New AxHost.ConnectionPointCookie(
                     Me.ActiveXInstance, helper, GetType(DWebBrowserEvents2))
             End Sub
         
             ''' <summary>
             ''' 从底层的ActiveX控件中,释放附加在 CreateSink 方法中处理事件的客户端.        
             ''' </summary>
             Protected Overrides Sub DetachSink()
                 If cookie IsNot Nothing Then
                     cookie.Disconnect()
                     cookie = Nothing
                 End If
                 MyBase.DetachSink()
             End Sub
         
             ''' <summary>
             ''' 激发 LoadCompleted 事件.
             ''' </summary>
             Protected Overridable Sub OnLoadCompleted(ByVal e As WebBrowserDocumentCompletedEventArgs)
         
                 RaiseEvent LoadCompleted(Me, e)
             End Sub
         
             ''' <summary>
             ''' 激发 StartNavigating 事件.
             ''' </summary>
             Protected Overridable Sub OnStartNavigating(ByVal e As WebBrowserNavigatingEventArgs)
                 RaiseEvent StartNavigating(Me, e)
             End Sub
         End Class
      
4. MainFrom 是该应用程序的用户界面.如果 WebBrowser 被导航到一个 URL, 它将显示 
   DocumentCompleted 事件被引发的次数和 LoadCompleted事件被引发次数的计数状况,
   也包括了详细的活动记录.


/////////////////////////////////////////////////////////////////////////////
参考:

How To Determine When a Page Is Done Loading in WebBrowser Control
http://support.microsoft.com/kb/q180366/

DWebBrowserEvents2 Interface
http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx

WebBrowser.CreateSink Method 
http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.createsink.aspx


/////////////////////////////////////////////////////////////////////////////
