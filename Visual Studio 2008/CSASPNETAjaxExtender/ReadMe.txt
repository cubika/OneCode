========================================================================
    ASP.Net 应用程序 : CSASPNETAjaxExtender 项目概述
========================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

这个CSASPNETAjaxExtender示例演示了如何创建一个ASP.Net Ajax 
ExtenderControl, 即一个TimePicker允许用户在一个钟面上拖动
时针或分针选择一天内的时间.


/////////////////////////////////////////////////////////////////////////////
代码逻辑:

Microsoft ASP.Net Ajax扩展可以使你扩展ASP.Net Web 应用程序的可用性, 
提供更丰富的客户端体验. 
封装供ASP.NET页面开发人员使用的客户端行为, 你可以使用
使用扩展控件. 
扩展控件是继承了命名空间System.Web.UI中的虚拟类ExtenderControl的Web服务器控件. 

1. 创建一个扩展控件.
   我们可以使用"Asp.Net Ajax Server Control Extender"模板创建ExtenderControl. 
   它默认提供一个类文件, 一个资源文件(事实上, 在这个示例中我们不需要.) 
   和一个js文件. 我们可以在类文件中创建扩展,在js文件中创建行为.
   
2. 扩展控件是一个现存的web空间使用客户端脚本的功能性扩展. 
   它可应用在特定的Web服务器控件类型. 
   使用TargetControlTypeAttribute属性定义扩展控件应用的的Web服务器控件类型.

   [TargetControlType(typeof(TextBox))]
   public class TimePicker: ExtenderControl


3. 下列两个ExtenderControl虚拟类中的方法必须在扩展控件中实现.

   protected override IEnumerable<ScriptDescriptor> 
GetScriptDescriptors(Control targetControl)
   {
      ScriptControlDescriptor descriptor = new ScriptControlDescriptor
("CSASPNETAjaxExtender.TimePicker", targetControl.ClientID);

      descriptor.AddElementProperty("errorSpan", this.NamingContainer.FindControl
(ErrorPresentControlID).ClientID);
			
      descriptor.AddProperty("timeType", TimeType);

      descriptor.AddEvent("showing", OnClientShowing);

      yield return descriptor;
   }


   protected override IEnumerable<ScriptReference> GetScriptReferences()
   {
      yield return new ScriptReference(Page.ClientScript.GetWebResourceUrl
(this.GetType(), "CSASPNETAjaxExtender.TimePicker.TimePicker.js"));
   }

4. 如果你有一个css style文件修饰扩展控件, 在PreRender阶段嵌入Css引用.

   private void RenderCssReference()
   {
      string cssUrl = Page.ClientScript.GetWebResourceUrl
(this.GetType(), "CSASPNETAjaxExtender.TimePicker.TimePicker.css");

      HtmlLink link = new HtmlLink();
      link.Href = cssUrl;
      link.Attributes.Add("type", "text/css");
      link.Attributes.Add("rel", "stylesheet");
      Page.Header.Controls.Add(link);
   }
   
5. 设定所有资源(包括图片, css文件和js文件)嵌入这个扩展控件
   作为"Embedded Resource"(属性 "Build Action").

6. 这个控件可以衍生于其他服务器控件若继承于某个服务器控件而非ExtenderControl. 
   在此场合, 衍生于IExtenderControl接口和某个服务器控件类. 
   同时, 我们有另三步需要完成:
   1) 定义TargetControl属性
   2) 重写OnPreRender方法. 在OnPreRender阶段将这个Web控件注册为ExtenerControl.
   
            ScriptManager manager = ScriptManager.GetCurrent(this.Page);
            if (manager == null)
            {
                throw new InvalidOperationException("A ScriptManager is required on the page.");
            }
            manager.RegisterExtenderControl<TimePicker>(this);
   3) 重写Render方法. 注册已定义的脚本描述器.
   
            ScriptManager.GetCurrent(this.Page).RegisterScriptDescriptors(this);

7. 剩下的工作都在客户端. 首先注册客户端命名空间.
   
   Type.registerNamespace("CSASPNETAjaxExtender");

8. 建立客户端类. 

   CSASPNETAjaxExtender.TimePicker = function(element) {
    
   }

   CSASPNETAjaxExtender.TimePicker.prototype = {

   }

9. 注册这个类,继承"Sys.UI.Behavior".
   
   CSASPNETAjaxExtender.TimePicker.registerClass('CSASPNETAjaxExtender.TimePicker', Sys.UI.Behavior);

10.在构造化方法中调用基本方法
   
   CSASPNETAjaxExtender.TimePicker.initializeBase(this, [element]);

11. 实现Initialize和Dispose方法.

   建立类原形中的"initialize"和"dispose"方法.initialize方法在一个行为的实例
   被创建时调用. 使用这个方法设定默认的属性值, 创建函数委托, 添加委托为事件句柄. 
   dispose方法在一个行为的实例不再被页面使用和被移出时调用. 
   使用这个方法释放任何不再被行为需要的资源, 比如DOM事件句柄.


   initialize: function() {
       CSASPNETAjaxExtender.TimePicker.callBaseMethod(this, 'initialize');       

   },

   dispose: function() {        
       CSASPNETAjaxExtender.TimePicker.callBaseMethod(this, 'dispose');
   }

12. 定义属性的Get和Set方法.

   每个在扩展控件的GetScriptDescriptors(Control)得到的ScriptDescriptor
   中定义的属性必须有相应的客户端访问函数. 
   客户端属性访问函数必须定义为客户端类原形的get_<property name>和set_<property name> 
   方法.


   get_timeType: function() {
       return this._timeType;
   },

   set_timeType: function(val) {
       if (this._timeType !== val) {
           this._timeType = val;
           this.raisePropertyChanged('timeType');
       }
   },

13. 定义DOM元素的事件句柄
   1) 在构造函数中定义句柄:
        this._element_focusHandler = null;
   2) 在初始化方法中关联句柄到DOM元素事件:
		this._element_focusHandler = Function.createDelegate(this, this._element_onfocus);
   3) 在初始化方法中新建句柄:
		$addHandler(this.get_element(), 'focus', this._element_focusHandler) 
   4) 建立事件的回调方法:
		_element_onfocus:function(){

		}

14. 定义行为的事件句柄

    每个在扩展控件的GetScriptDescriptors(Control)得到的ScriptDescriptor
    中定义的事件必须有相应的客户端访问函数. 
    客户端事件访问函数必须定义为客户端类原形的add_<event name> and remove_<event name> 
    方法. 
    方法Raise<event name>定义为事件的触发器.  

    add_showing: function(handler) {
        this.get_events().addHandler("showing", handler);
    },
    remove_showing: function(handler) {

        this.get_events().removeHandler("showing", handler);
    },
    raiseShowing: function(eventArgs) {

        var handler = this.get_events().getHandler('showing');
        if (handler) {
            handler(this, eventArgs);
        }
    },
    
15. 在页面中使用这个TimePicker控件.
    这个扩展控件的用法和自定义控件一样.    
	1) 在页面中注册程序集.
	   <%@ Register TagPrefix="CSASPNETAjaxExtender" Assembly="CSASPNETAjaxExtender" 
	   Namespace="CSASPNETAjaxExtender" %>
	2) 在页面中新建一个ScriptManager控件, 然后创建TimePicker控件绑定到一个为TextBox.
	
	   <asp:TextBox ID="TextBox1" Text="" runat="server"></asp:TextBox>
	   <CSASPNETAjaxExtender:TimePicker runat="server" ID="t1" TargetControlID="TextBox1" TimeType="H24" />


/////////////////////////////////////////////////////////////////////////////
参考资料:

Creating an Extender Control to Associate a Client Behavior with a Web Server Control
http://www.asp.net/AJAX/Documentation/Live/tutorials/ExtenderControlTutorial1.aspx


/////////////////////////////////////////////////////////////////////////////
