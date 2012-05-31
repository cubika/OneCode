'********************************* 模块头 *********************************\
' 模块名: FlyInTransition.vb
' 项目名: VideoStoryCreator
' 版权(c) Microsoft Corporation.
' 
' 实现简单的飞入特效.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Imports System.Xml.Linq

Public Interface ITransition
    ReadOnly Property Name() As String
    Property TransitionDuration() As TimeSpan
    ReadOnly Property ImageZIndexModified() As Boolean

    ' Foreground/BackgroundElement 可以是Image 或 MediaElement.
    ' 在PreviewPage中设置这些属性.
    Property ForegroundElement() As FrameworkElement
    Property BackgroundElement() As FrameworkElement

    Event TransitionCompleted As EventHandler

    Function Clone() As ITransition

    ' 开始/停止特效.
    Sub Begin()
    Sub [Stop]()

    ' 序列化/饭序列化特效.
    Sub Save(transitionElement As XElement)
End Interface
