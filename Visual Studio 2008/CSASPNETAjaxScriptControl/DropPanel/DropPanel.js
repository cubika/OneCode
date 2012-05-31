/// <reference name="MicrosoftAjax.js"/>


Type.registerNamespace("CSASPNETAjaxScriptControl");



CSASPNETAjaxScriptControl.DropPanel = function(element) {
    CSASPNETAjaxScriptControl.DropPanel.initializeBase(this, [element]);
    //元素属性
    this._headerContainer = null;
    this._titleContainer = null;
    this._contentContainer = null;
    this._closeImage=null;
    //属性
    this._cssClass = null;
    this._headerCssClass = null;
    this._contentCssClass = null;
    this._closeImageCssClass=null;    

    //句柄
    this._headerContainerMouseDownHandler = null;
    this._headerContainerMouseOverHandler = null;
    this._headerContainerMouseOutHandler = null;
    this._closeImageMouseDownHandler = null;

    //

    this._isMoving=false;


}

CSASPNETAjaxScriptControl.DropPanel.prototype = {
    initialize: function() {
        CSASPNETAjaxScriptControl.DropPanel.callBaseMethod(this, 'initialize');
        this.createDelegates();
        this.createHandlers();

    },
    

    
    dispose: function() {

        this.clearHandlers();
        this.clearDelegates();

        CSASPNETAjaxScriptControl.DropPanel.callBaseMethod(this, 'dispose');

    },

    createDelegates: function() {
        this._headerContainerMouseDownHandler = Function.createDelegate(this, this._onHeaderMouseDown);
        this._headerContainerMouseOverHandler = Function.createDelegate(this, this._onHeaderMouseOver);
        this._headerContainerMouseOutHandler  = Function.createDelegate(this, this._onHeaderMouseOut);
        this._closeImageMouseDownHandler = Function.createDelegate(this, this._onCloseImageMouseDown);


    },

    clearDelegates: function() {
        this._headerContainerMouseDownHandler = null;
        this._headerContainerMouseOverHandler = null;
        this._headerContainerMouseOutHandler = null;
        this._closeImageMouseDownHandler =null;
    },

    createHandlers: function() {

        $addHandlers(this.get_headerContainer(),
        {
            'mousedown': this._headerContainerMouseDownHandler,
            'mouseover': this._headerContainerMouseOverHandler,
            'mouseout': this._headerContainerMouseOutHandler
        }, this);
        $addHandler(this.get_closeImage(),"mousedown", this._closeImageMouseDownHandler);

    },

    clearHandlers: function() {
        $clearHandlers(this.get_headerContainer());
        $clearHandlers(this.get_closeImage());
    },

    moveTo: function(targetParentElement) {

        targetParentElement.appendChild(this.get_element());

    },

    //事件句柄 
    _onHeaderMouseDown: function(e) {
        
        if(CSASPNETAjaxScriptControl$DropPanel$DropPanelManager$get_ActiveDropPanel()===null)
        {

            this.set_isMoving(true);
            CSASPNETAjaxScriptControl$DropPanel$DropPanelManager$set_ActiveDropPanel(this);

            //其他
        }
        e.preventDefault();
        e.stopPropagation();
 
    },
    
    
    _onHeaderMouseOver:function(e){
        if(CSASPNETAjaxScriptControl$DropPanel$DropPanelManager$get_ActiveDropPanel()===null)
        {
            this.get_closeImage().style.display="block";      
            var margin=this.get_headerContainer().offsetWidth-this.get_closeImage().offsetWidth-5+"px";
            this.get_titleContainer().style.width = margin;
            this.get_titleContainer().style.overflow="hidden";   
            this.get_closeImage().style.marginLeft=margin;
        }
    
    
    },
    _onHeaderMouseOut:function(e){

        this.get_closeImage().style.display="none";
        this.get_titleContainer().style.width = "100%";
        this.get_titleContainer().style.overflow = 'inherit';
  
    },
    
    _onCloseImageMouseDown:function(e){
        if(CSASPNETAjaxScriptControl$DropPanel$DropPanelManager$get_ActiveDropPanel()===null)
        { 
            this.raiseClosed();
        }   
        e.preventDefault();
        e.stopPropagation();
        return false;
    },
    
    add_closed: function(handler) {
        /// <summary>
        /// 增加一个<code>closed</code>事件的事件句柄.
        /// </summary>
        /// <param name="handler" type="Function">
        /// 增加到事件的句柄.
        /// </param>
        /// <returns />

        this.get_events().addHandler("closed", handler);
    },

    remove_closed: function(handler) {
        /// <summary>
        /// 移除一个<code>closed</code>事件的事件句柄.
        /// </summary>
        /// <param name="handler" type="Function">
        /// 从事件中移除的句柄.
        /// </param>
        /// <returns />

        this.get_events().removeHandler("closed", handler);
    },
    raiseClosed: function() {
        /// <summary>
        /// 引发<code>closed</code>事件
        /// </summary>
        /// <returns />

        var handlers = this.get_events().getHandler("closed");
        if (handlers) {
            handlers(this, Sys.EventArgs.Empty);
        }
    },
    
    //属性 

    set_isMoving: function(val) {
        this._isMoving = val;
    },

    get_isMoving: function() {
        return this._isMoving;
    },

    set_isMoving: function(val) {
        this._isMoving = val;
    },

    get_headerContainer: function() {
        return this._headerContainer;
    },

    set_headerContainer: function(val) {
        if (this._headerContainer !== val) {
            this._headerContainer = val;
            this.raisePropertyChanged('headerContainer');
        }
    },
    get_titleContainer: function() {
        return this._titleContainer;
    },

    set_titleContainer: function(val) {
        if (this._titleContainer !== val) {
            this._titleContainer = val;
            this.raisePropertyChanged('titleContainer');
        }
    },
    
    
    
    get_closeImage: function() {
        return this._closeImage;
    },

    set_closeImage: function(val) {
        if (this._closeImage !== val) {
            this._closeImage = val;
            this.raisePropertyChanged('closeImage');
        }
    },
    
    get_contentContainer: function() {
        return this._contentContainer;
    },

    set_contentContainer: function(val) {
        if (this._contentContainer !== val) {
            this._contentContainer = val;
            this.raisePropertyChanged('contentContainer');
        }
    },

    get_cssClass: function() {

        return this._cssClass;
    },

    set_cssClass: function(val) {
        if (this._cssClass !== val) {
            this._cssClass = val;
            this.raisePropertyChanged('cssClass');
        }
    },

    get_headerCssClass: function() {
        return this._headerCssClass;
    },

    set_headerCssClass: function(val) {
        if (this._headerCssClass !== val) {
            this._headerCssClass = val;
            this.raisePropertyChanged('headerCssClass');
        }
    },
    
    get_closeImageCssClass: function() {

        return this._closeImageCssClass;
    },

    set_closeImageCssClass: function(val) {
        if (this._closeImageCssClass !== val) {
            this._closeImageCssClass = val;
            this.raisePropertyChanged('closeImageCssClass');
        }
    },
    get_contentCssClass: function() {
        return this._contentCssClass;
    },

    set_contentCssClass: function(val) {
        if (this._contentCssClass !== val) {
            this._contentCssClass = val;
            this.raisePropertyChanged('contentCssClass');
        }
    },

    get_fromContextKey: function() {
        return this._fromContextKey;
    },

    set_fromContextKey: function(val) {
        if (this._fromContextKey !== val) {
            this._fromContextKey = val;
            this.raisePropertyChanged('fromContextKey');
        }
    },

    get_toContextKey: function() {
        return this._toContextKey;
    },

    set_toContextKey: function(val) {
        if (this._toContextKey !== val) {
            this._toContextKey = val;
            this.raisePropertyChanged('toContextKey');
        }
    }

}

function CSASPNETAjaxScriptControl$DropPanel$DropPanelManager$get_ActiveDropPanel()
{
    if(this._activeDropPanel == undefined || this._activeDropPanel == null)
        return null;
    return this._activeDropPanel;

}

function CSASPNETAjaxScriptControl$DropPanel$DropPanelManager$set_ActiveDropPanel(val)
{
    this._activeDropPanel=val;
}

CSASPNETAjaxScriptControl.DropPanel.registerClass('CSASPNETAjaxScriptControl.DropPanel', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

