SET obj = CreateObject("VBExeCOMServer.VBSimpleObject")

MsgBox "A VBExeCOMServer.VBSimpleObject object is created"

' 执行HelloWorld方法并返回一个字符串
MsgBox "The HelloWorld method returns " & obj.HelloWorld

' 设置FloatProperty属性
MsgBox "Set the FloatProperty property to 1.2"
obj.FloatProperty = 1.2

'获取FloatProperty属性
MsgBox "The FloatProperty property returns " & obj.FloatProperty

SET obj = Nothing