'****************************** 模块 标识 ******************************'
' 模块名:	Samples.svc.vb
' 项目:		VBADONETDataService
' 版权 (c)  Microsoft Corporation.
' 
' Samples.svc 展示了如何建立基于非关系型数据源的 ADO.NET Data Service。
' 非关系型数据源指的是一些存储于内存中的对象，其中储存了 All-In-One Code Framework
' 的一些信息。非关系型实体类也实现了 IUpdate 接口来允许客户端插入新的数据。
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directive"
Imports System.Data.Services
Imports System.Linq
Imports System.ServiceModel.Web
Imports System.Data.Services.Common
Imports System.Reflection

#End Region

Public Class Samples
    Inherits DataService(Of SampleProjects)

    '  本方法只会被调用一次，用以初始化整个服务的规则。
    Public Shared Sub InitializeService(ByVal config As  _
                                        IDataServiceConfiguration)
        ' 设置规则，表示那些实体集合以及服务操作可见的，可更改的，等等
        config.UseVerboseErrors = True
        config.SetEntitySetAccessRule("*", EntitySetRights.All)
    End Sub
End Class


#Region "Non-relational data entity classes"
' 示例 Project 实体类，它的关键属性为 'ProjectName'。
<DataServiceKey("ProjectName")> _
Public Class Project

    Private _projectName As String
    Private _owner As String
    Private _projectCategory As Category

    Public Property ProjectName() As String
        Get
            Return _projectName
        End Get
        Set(ByVal value As String)
            _projectName = value
        End Set
    End Property

    Public Property Owner() As String
        Get
            Return _owner
        End Get
        Set(ByVal value As String)
            _owner = value
        End Set
    End Property

    Public Property ProjectCategory() As Category
        Get
            Return _projectCategory
        End Get
        Set(ByVal value As Category)
            _projectCategory = value
        End Set
    End Property

End Class

' 示例 Category 实体类，它的关键属性为 'CategoryName'。
<DataServiceKey("CategoryName")> _
Public Class Category

    Private _categoryName As String

    Public Property CategoryName() As String
        Get
            Return _categoryName
        End Get
        Set(ByVal value As String)
            _categoryName = value
        End Set
    End Property

End Class

' 示例 data 实体类。
Public Class SampleProjects
    Implements IUpdatable

    Shared _categories As List(Of Category)
    Shared _projects As List(Of Project)

    ' 静态构造函数。
    Shared Sub New()

        '  初始化 categories。
        _categories = New List(Of Category)()
        _categories.Add(New Category With {.CategoryName = "COM"})
        _categories.Add(New Category With {.CategoryName = "Data Access"})
        _categories.Add(New Category With {.CategoryName = "Office"})
        _categories.Add(New Category With {.CategoryName = "IPC and RPC"})
        _categories.Add(New Category With {.CategoryName = "WinForm"})
        _categories.Add(New Category With {.CategoryName = "Hook"})

        ' 初始化 projects。
        _projects = New List(Of Project)()
        _projects.Add(New Project With _
        {.ProjectName = "CSDllCOMServer", .Owner = "Jialiang Ge", .ProjectCategory = _categories(0)})
        _projects.Add(New Project With _
        {.ProjectName = "VBDllCOMServer", .Owner = "Jialiang Ge", .ProjectCategory = _categories(0)})
        _projects.Add(New Project With _
        {.ProjectName = "ATLDllCOMServer", .Owner = "Jialiang Ge", .ProjectCategory = _categories(0)})
        _projects.Add(New Project With _
        {.ProjectName = "CSUseADONET", .Owner = "Lingzhi Sun", .ProjectCategory = _categories(1)})
        _projects.Add(New Project With _
        {.ProjectName = "CppUseADONET", .Owner = "Jialiang Ge", .ProjectCategory = _categories(1)})
        _projects.Add(New Project With _
        {.ProjectName = "CSLinqToObject", .Owner = "Colbert Zhou", .ProjectCategory = _categories(1)})
        _projects.Add(New Project With _
        {.ProjectName = "CSLinqToSQL", .Owner = "Rongchun Zhang", .ProjectCategory = _categories(1)})
        _projects.Add(New Project With _
        {.ProjectName = "CSOutlookUIDesigner", .Owner = "Jie Wang", .ProjectCategory = _categories(2)})
        _projects.Add(New Project With _
        {.ProjectName = "CSOutlookRibbonXml", .Owner = "Jie Wang", .ProjectCategory = _categories(2)})
        _projects.Add(New Project With _
        {.ProjectName = "CSAutomateExcel", .Owner = "Jialiang Ge", .ProjectCategory = _categories(2)})
        _projects.Add(New Project With _
        {.ProjectName = "VBAutomateExcel", .Owner = "Jialiang Ge", .ProjectCategory = _categories(2)})
        _projects.Add(New Project With _
        {.ProjectName = "CppFileMappingServer", .Owner = "Hongye Sun", .ProjectCategory = _categories(3)})
        _projects.Add(New Project With _
        {.ProjectName = "CppFileMappingClient", .Owner = "Hongye Sun", .ProjectCategory = _categories(3)})
        _projects.Add(New Project With _
        {.ProjectName = "CSReceiveWM_COPYDATA", .Owner = "Riquel Dong", .ProjectCategory = _categories(3)})
        _projects.Add(New Project With _
        {.ProjectName = "CSSendWM_COPYDATA", .Owner = "Riquel Dong", .ProjectCategory = _categories(3)})
        _projects.Add(New Project With _
        {.ProjectName = "CSWinFormGeneral", .Owner = "Zhixin Ye", .ProjectCategory = _categories(4)})
        _projects.Add(New Project With _
        {.ProjectName = "CSWinFormDataBinding", .Owner = "Zhixin Ye", .ProjectCategory = _categories(4)})
        _projects.Add(New Project With _
        {.ProjectName = "CSWindowsHook", .Owner = "Rongchun Zhang", .ProjectCategory = _categories(5)})
    End Sub

    ' 公共属性用以在客户端从 ADO.NET Data Service 获得 projects 信息。
    Public ReadOnly Property Projects() As IQueryable(Of Project)
        Get
            Return _projects.AsQueryable()
        End Get
    End Property

    ' 公共属性用以在客户端从 ADO.NET Data Service 获得 categories 信息。
    Public ReadOnly Property Categories() As IQueryable(Of Category)
        Get
            Return _categories.AsQueryable()
        End Get
    End Property

#Region "IUpdatable Members"

    ' 临时保存添加的对象。
    Private tempObj As Object = Nothing

    ''' <summary>
    ''' 创建一个所给对象的资源并附加到给定的Container上。
    ''' </summary>
    ''' <param name="containerName">容器名，创建的资源将会被附加到该容器上 。</param>
    ''' <param name="fullTypeName">程序集限定名。</param>
    ''' <returns>资源对象。</returns>
    ''' <remarks></remarks>
    Function CreateResource(ByVal containerName As String, ByVal _
                            fullTypeName As String) As Object Implements _
                            IUpdatable.CreateResource

        ' 获得资源的类型。
        Dim t As Type = Type.GetType(fullTypeName, True)

        ' 创建一个资源对象的实例。
        Dim resource As Object = Activator.CreateInstance(t)

        ' 返回资源对象。
        Return resource
    End Function

    ''' <summary>
    ''' 向集合中添加给定的值。
    ''' </summary>
    ''' <param name="targetResource">目标对象定义了相关属性。
    ''' </param>
    ''' <param name="propertyName">需要更新的属性名。
    ''' </param>
    ''' <param name="resourceToBeAdded">需要添加的属性值。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub AddReferenceToCollection(ByVal targetResource As Object, _
                                        ByVal propertyName As String, _
                                        ByVal resourceToBeAdded As Object) _
                                        Implements System.Data.Services. _
                                        IUpdatable.AddReferenceToCollection
        ' 获得目标对象的类型。
        Dim t As Type = targetResource.GetType()

        ' 获得需要更新的属性。
        Dim pi = t.GetProperty(propertyName)
        If Not pi Is Nothing Then
            ' 获得集合中的属性值。
            Dim collection As IList = DirectCast(pi.GetValue(targetResource, _
                                                         Nothing), IList)
            ' 将资源添加进集合中。
            collection.Add(resourceToBeAdded)
        End If
    End Sub

    ''' <summary>
    ''' 回滚所有未保存的操作。
    ''' </summary>
    Public Sub ClearChanges() Implements System.Data.Services.IUpdatable. _
    ClearChanges
        Throw New NotImplementedException
    End Sub

    ''' <summary>
    ''' 删除给定资源。
    ''' </summary>
    ''' <param name="targetResource">需要删除的资源对象。
    ''' </param>
    Public Sub DeleteResource(ByVal targetResource As Object) Implements _
    System.Data.Services.IUpdatable.DeleteResource
        Throw New NotImplementedException
    End Sub

    ''' <summary>
    ''' 获得查询指向的给定类型的资源。
    ''' </summary>
    ''' <param name="query">指向特定资源的查询。
    ''' </param>
    ''' <param name="fullTypeName">f程序集限定名
    ''' </param>
    ''' <returns>资源对象
    ''' </returns>
    Public Function GetResource(ByVal query As System.Linq.IQueryable, _
                                ByVal fullTypeName As String) As Object _
                                Implements System.Data.Services.IUpdatable. _
                                GetResource
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 获得给定对象相关属性的值。
    ''' </summary>
    ''' <param name="targetResource">定义了相关属性的目标对象。
    ''' </param>
    ''' <param name="propertyName">需要更新的属性名
    ''' </param>
    ''' <returns>给定目标资源的属性值
    ''' </returns>
    Public Function GetValue(ByVal targetResource As Object, ByVal _
                             propertyName As String) As Object Implements _
                             System.Data.Services.IUpdatable.GetValue
        ' 获得目标对象类型。
        Dim t As Type = targetResource.GetType()

        ' 获得属性。
        Dim pi = t.GetProperty(propertyName)

        If Not pi Is Nothing Then
            ' 返回属性值。
            Return pi.GetValue(targetResource, Nothing)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' 从集合中移除给定值。
    ''' </summary>
    ''' <param name="targetResource">t定义了相关属性的目标对象。
    ''' </param>
    ''' <param name="propertyName">需要更新的属性名。
    ''' </param>
    ''' <param name="resourceToBeRemoved">需要移除的属性值。
    ''' </param>
    Public Sub RemoveReferenceFromCollection(ByVal targetResource As Object, _
                                             ByVal propertyName As String, _
                                             ByVal resourceToBeRemoved As Object) _
                                             Implements System.Data.Services. _
                                             IUpdatable.RemoveReferenceFromCollection
        Throw New NotImplementedException
    End Sub

    ''' <summary>
    ''' 重置给定资源为默认值。
    ''' </summary>
    ''' <param name="resource">需要被重置的资源。
    ''' </param>
    ''' <returns>被重置后的资源。</returns>
    Public Function ResetResource(ByVal resource As Object) As Object _
    Implements System.Data.Services.IUpdatable.ResetResource
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 返回被所给资源对象所代表的实际对象实例。
    ''' </summary>
    ''' <param name="resource">需要被用来解析的资源对象。 
    ''' </param>
    ''' <returns>被资源对象所代表的实际对象实例。
    ''' </returns>
    Public Function ResolveResource(ByVal resource As Object) As Object _
    Implements System.Data.Services.IUpdatable.ResolveResource
        Return resource
    End Function

    ''' <summary>
    ''' 保存所有未保存的当前操作。
    ''' </summary>
    Public Sub SaveChanges() Implements System.Data.Services.IUpdatable. _
    SaveChanges
        ' 将临时对象加到集合中去。
        If Not tempObj Is Nothing Then
            Dim t = tempObj.GetType()
            If t.Name = "Category" Then
                SampleProjects._categories.Add(DirectCast(tempObj, Category))
            ElseIf t.Name = "Project" Then
                SampleProjects._projects.Add(DirectCast(tempObj, Project))
            End If
        End If
    End Sub

    ''' <summary>
    ''' 设置给定目标对象相关属性的引用值。 
    ''' </summary>
    ''' <param name="targetResource">定义了相关属性的目标对象。 
    ''' </param>
    ''' <param name="propertyName">需要更新的属性名。
    ''' </param>
    ''' <param name="propertyValue">属性值。
    ''' </param>
    ''' <remarks></remarks>
    Public Sub SetReference(ByVal targetResource As Object, ByVal _
                            propertyName As String, ByVal propertyValue _
                            As Object) Implements System.Data.Services. _
                            IUpdatable.SetReference
        DirectCast(Me, IUpdatable).SetValue(targetResource, _
                                            propertyName, propertyValue)
    End Sub

    ''' <summary>
    ''' 设置给定对象属性值。
    ''' </summary>
    ''' <param name="targetResource">定义了相关属性的目标对象。
    ''' </param>
    ''' <param name="propertyName">需要被更新的属性名。
    ''' </param>
    ''' <param name="propertyValue">属性值</param>
    ''' <remarks></remarks>
    Public Sub SetValue(ByVal targetResource As Object, ByVal _
                        propertyName As String, ByVal propertyValue _
                        As Object) Implements System.Data.Services. _
                        IUpdatable.SetValue
        ' 获得资源对象类型。
        Dim t As Type = targetResource.GetType()

        ' 获得需要更新的属性。
        Dim pi = t.GetProperty(propertyName)

        If Not pi Is Nothing Then
            ' 设置属性值
            pi.SetValue(targetResource, propertyValue, Nothing)
        End If

        ' 将目标对象保存到临时对象中。
        tempObj = targetResource
    End Sub
#End Region

End Class
#End Region
