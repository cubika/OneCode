================================================================================
            Windows 应用程序: CSWebBrowserLoadComplete 概述                        
================================================================================

////////////////////////////////////////////////////////////////////////////////
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


//////////////////////////////////////////////////////////////////////////////////
演示:

步骤1. 运行 CSWebBrowserLoadComplete.exe.

步骤2. Textbox 控件中默认的 url 是 "\Resource\FramesPage.htm" 的路径.
       点击按钮"Go".

       FramesPage.htm 包含3个内嵌框架. 在该页面加载之后，你将在窗体的底部得到以下消
	   息：

       DocumentCompleted:4 LoadCompleted:1.

       这个消息表明，DocumentCompleted 事件被引发了4次，LoadCompleted 事件被引发了
	   一次. 

       你也能在列表框里面得到详细的活动记录.


//////////////////////////////////////////////////////////////////////////////////
代码逻辑:

1. DWebBrowserEvents2 接口指定了,应用程序必须实现从 WebBrowser 控件或 Windows 
   Internet Explorer 应用程序中，接收事件通知的事件接收接口. 事件通知包括将用于此应 
   用程序的 DocumentCompleted 和 BeforeNavigate2 的事件.

       [ComImport, TypeLibType(TypeLibTypeFlags.FHidden), 
       InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
       Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
       public interface DWebBrowserEvents2
       {
           /// <summary>
           /// 当文档完全加载并初始化时引发.
           /// </summary>
           [DispId(259)]
           void DocumentComplete(
               [In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, 
               [In] ref object URL);
       
       
           [DispId(250)]
           void BeforeNavigate2(
               [In, MarshalAs(UnmanagedType.IDispatch)] object pDisp,
               [In] ref object URL, 
               [In] ref object flags, 
               [In] ref object targetFrameName, 
               [In] ref object postData, 
               [In] ref object headers,
               [In, Out] ref bool cancel);
               
       }

2. DWebBrowserEvents2Helper 类实现了 DWebBrowserEvents2 接口来检查是否完成页面加载.
   
   如果 WebBrowser 控件装载一个普通的、无内嵌框架的 HTML 页面, 在所有事情完成之后,
   DocumentComplete 事件会被引发一次.
   
   如果 WebBrowser 控件装载了许多内嵌框架, DocumentComplete 会被多次引发.
   DocumentComplete 事件有一个 pDisp 参数,它是框架( shdocvw )的 IDispatch.该框架中
   DocumentComplete 被引发. 
   
   然后我们可以检查是否 DocumentComplete 的 pDisp 参数与浏览器的 ActiveXInstance 相同. 


            private class DWebBrowserEvents2Helper : StandardOleMarshalObject, DWebBrowserEvents2
            {
                private WebBrowserEx parent;

                public DWebBrowserEvents2Helper(WebBrowserEx parent)
                {
                    this.parent = parent;
                }
            
                public void DocumentComplete(object pDisp, ref object URL)
                {
                    string url = URL as string;
               
                    if (string.IsNullOrEmpty(url)
                        || url.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
               
                    if (pDisp != null && pDisp.Equals(parent.ActiveXInstance))
                    {
                        var e = new WebBrowserDocumentCompletedEventArgs(new Uri(url)); 
               
                        parent.OnLoadCompleted(e);
                    }
                }
               
                public void BeforeNavigate2(object pDisp, ref object URL, ref object flags,
                    ref object targetFrameName, ref object postData, ref object headers,
                    ref bool cancel)
                {
                    string url = URL as string;
               
                    if (string.IsNullOrEmpty(url)
                        || url.Equals("about:blank", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
               
                    if (pDisp != null && pDisp.Equals(parent.ActiveXInstance))
                    {
                        WebBrowserNavigatingEventArgs e = new WebBrowserNavigatingEventArgs(
                            new Uri(url), targetFrameName as string);
               
                        parent.OnStartNavigating(e);
                    }
                }
            }
            
3. WebBrowserEx 类继承的浏览器类，并提供 StartNavigating 和 LoadCompleted 的事件.

         public partial class WebBrowserEx : WebBrowser
    {
        AxHost.ConnectionPointCookie cookie;

        DWebBrowserEvents2Helper helper;

        public event EventHandler<WebBrowserNavigatingEventArgs> StartNavigating;

        public event EventHandler<WebBrowserDocumentCompletedEventArgs> LoadCompleted;

        /// <summary>
        /// 将底层的 ActiveX 控件与可以处理控件事件包括 NavigateError 事件的客户端 
        /// 关联起来.
        /// </summary>
        protected override void CreateSink()
        {

            base.CreateSink();

            helper = new DWebBrowserEvents2Helper(this);
            cookie = new AxHost.ConnectionPointCookie(
                this.ActiveXInstance, helper, typeof(DWebBrowserEvents2));         
        }

        /// <summary>
        /// 从底层的ActiveX控件中,释放附加在 CreateSink 方法中处理事件的客户端. 
        /// </summary>
        protected override void DetachSink()
        {
            if (cookie != null)
            {
                cookie.Disconnect();
                cookie = null;
            }
            base.DetachSink();
        }

        /// <summary>
        /// 激发 LoadCompleted 事件.
        /// </summary>
        protected virtual void OnLoadCompleted(WebBrowserDocumentCompletedEventArgs e)
        {
            if (LoadCompleted != null)
            {
                this.LoadCompleted(this, e);
            }
        }

        /// <summary>
        /// 激发 StartNavigating 事件.
        /// </summary>
        protected virtual void OnStartNavigating(WebBrowserNavigatingEventArgs e)
        {
            if (StartNavigating != null)
            {
                this.StartNavigating(this, e);
            }
        }
    }
      
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
