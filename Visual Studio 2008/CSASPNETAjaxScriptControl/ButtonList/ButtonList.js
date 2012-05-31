/// <reference name="MicrosoftAjax.js"/>


Type.registerNamespace("CSASPNETAjaxScriptControl");

CSASPNETAjaxScriptControl.ButtonList = function(element) {
    CSASPNETAjaxScriptControl.ButtonList.initializeBase(this, [element]);
    // 元素属性
    this._buttonListOptionList = null;
    this._buttonListHiddenField = null;
    this._buttonListLastElement = null;
    // 属性


    // 句柄
    this._mouseOutHandler=null;
    this._listMouseOverHandler = null;
    this._listMouseDownHandler = null;

    //
    this._highlightedIndex=null;
}

CSASPNETAjaxScriptControl.ButtonList.prototype = {
    initialize: function() {
        CSASPNETAjaxScriptControl.ButtonList.callBaseMethod(this, 'initialize');        
        this.createDelegates();
        this.createHandlers();

    },

    dispose: function() {
        this.clearHandlers();
        this.clearDelegates();
        CSASPNETAjaxScriptControl.ButtonList.callBaseMethod(this, 'dispose');

    },

    createDelegates: function() {
        this._mouseOutHandler= Function.createDelegate(this, this._onMouseOut);
        this._listMouseOverHandler = Function.createDelegate(this, this._onListMouseOver);
        this._listMouseDownHandler = Function.createDelegate(this, this._onListMouseDown);

    },

    clearDelegates: function() {
        this._mouseOutHandler=null;
        this._listMouseOverHandler = null;
        this._listMouseDownHandler = null;

    },

    createHandlers: function() {
        $addHandler(this.get_element(),"mouseout",this._mouseOutHandler);
        $addHandlers(this.get_buttonListOptionList(),
        {
            'mouseover': this._listMouseOverHandler,
            'mousedown': this._listMouseDownHandler

        }, this);
    },

    clearHandlers: function() {
        $clearHandlers(this.get_element());
        $clearHandlers(this.get_buttonListOptionList());

    },

    // 事件句柄
    _onMouseOut:function() {
        var children = $(this.get_buttonListOptionList()).find(">li");
        var oldLiElement = this._highlightedIndex==null?null: children[this._highlightedIndex];
        if(oldLiElement!=null && this._highlightedIndex!=this.get_selectedIndex())
            oldLiElement.className=this.get_listItemCssClass();
    },
    _onListMouseOver: function(e) {

        if (e.target !== this.get_buttonListOptionList()) {
            var target = e.target;
            var children = $(this.get_buttonListOptionList()).find(">li");

            // 循环查找子节点以找到匹配的
            for (var i = 0; i < children.length; ++i) {
                // 找到匹配, 高亮对象并终止循环
                if (target === children[i]) {
                    this._highlightListItem(i);
                    break;
                }
            }
        }
        
    },

    _onListMouseDown: function(e) {

        if (e.target == this.get_buttonListOptionList() || e.target.tagName == 'scrollbar') {
            return true;
        }

        // 设定TextBox以高亮ListItem的文字并更新selectedIndex
        if (e.target !== this.get_buttonListOptionList()) {
            if(this.get_selectedIndex() != this._highlightedIndex){
                var children = $(this.get_buttonListOptionList()).find(">li");
                children[this.get_selectedIndex()].className = this.get_listItemCssClass();
                
                this.set_selectedIndex(this._highlightedIndex);
 
                // TODO: 计划设定clientselectedchanged事件
                if(this.get_autoPostBack())
                {
                    __doPostBack(this.get_element().id, '');
            
                }
            }
        }
        else {
            return true;
        }
        e.preventDefault();
        e.stopPropagation();
        return false;
    },
    
    _highlightListItem: function(index){
        // 只高亮有效的指数
        if (index == undefined || index < 0) {
            if (this._highlightedIndex != undefined && this._highlightedIndex >= 0) {
                this._highlightListItem(this._highlightedIndex);
            }
            return;
        }
        var children = $(this.get_buttonListOptionList()).find(">li");
        var newLiElement = children[index];
        var oldLiElement = this._highlightedIndex==null?null: children[this._highlightedIndex];
        
        
        if(oldLiElement!=null && this._highlightedIndex!=this.get_selectedIndex())
            oldLiElement.className=this.get_listItemCssClass();
        newLiElement.className=this.get_listItemHighLightCssClass();     
        
        this._highlightedIndex=index;
        
        
    },
    
    _toggleCssClass: function(element,cssClassName1,cssClassName2){
    
        var oldClassName=element.className;
        if(oldClassName!=cssClassName1 && oldClassName!=cssClassName2)
            return;
        var newClassName=(oldClassName==cssClassName1)?cssClassName2:cssClassName1;
        Sys.UI.DomElement.removeCssClass(element,oldClassName);
        Sys.UI.DomElement.addCssClass(element,newClassName);
    },
    
    // 属性
    get_buttonListOptionList: function() {
        return this._buttonListOptionList;
    },

    set_buttonListOptionList: function(val) {
        if (this._buttonListOptionList !== val) {
            this._buttonListOptionList = val;
            this.raisePropertyChanged('buttonListOptionList');
        }
    },

    get_buttonListHiddenField: function() {
        return this._buttonListHiddenField;
    },

    set_buttonListHiddenField: function(val) {
        if (this._buttonListHiddenField !== val) {
            this._buttonListHiddenField = val;
            this.raisePropertyChanged('buttonListHiddenField');
        }
    },
    get_buttonListLastElement: function() {
        return this._buttonListLastElement;
    },

    set_buttonListLastElement: function(val) {
        if (this._buttonListLastElement !== val) {
            this._buttonListLastElement = val;
            this.raisePropertyChanged('buttonListLastElement');
        }
    },
    
    
    get_autoPostBack: function() {
        return this._autoPostBack;
    },

    set_autoPostBack: function(val) {
        if (this._autoPostBack !== val) {
            this._autoPostBack = val;
            this.raisePropertyChanged('autoPostBack');
        }
    },
    get_selectedIndex: function() {
        this._ensureSelectedIndex();
        var selectedIndex = this.get_buttonListHiddenField().value;
        return parseInt(selectedIndex);
    },

    set_selectedIndex: function(val) {
        if (this.get_buttonListHiddenField().value !== val.toString()) {
            this.get_buttonListHiddenField().value = val.toString();
            this._ensureSelectedIndex();
            this.raisePropertyChanged('selectedIndex');
        }
    },

    _ensureSelectedIndex: function() {

        // 服务器不可以一直调用 set_selectedIndex(), 需要保持为一个整数
        var selectedIndex = this.get_buttonListHiddenField().value;
        if (selectedIndex == '') {
            selectedIndex = -1;
            this.get_buttonListHiddenField().value = selectedIndex.toString();
        }
    },

    
    get_listItemHighLightCssClass:function(){
        return this._listItemHighLightCssClass;
    },
    
    set_listItemHighLightCssClass:function(val){
        if (this._listItemHighLightCssClass !== val) {
            this._listItemHighLightCssClass = val;
            this.raisePropertyChanged('listItemHighLightCssClass');
        }
    },
    
    get_listItemCssClass:function(){
        return this._listItemCssClass;
    },
    
    set_listItemCssClass:function(val){
        if (this._listItemCssClass !== val) {
            this._listItemCssClass = val;
            this.raisePropertyChanged('listItemCssClass');
        }
    }

}

CSASPNETAjaxScriptControl.ButtonList.registerClass('CSASPNETAjaxScriptControl.ButtonList', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();