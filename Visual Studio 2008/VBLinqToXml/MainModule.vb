'****************************** 模块头 ******************************'
' 模块名称:	    MainModule.vb
' 工程:		    VBLinqToXml
' 版权 (c) Microsoft Corporation.
' 
' 本例阐明了如何在VB.NET中使用Linq to XML从内存对象和SQL Server数据库来创建
' XML文档。 它还同时阐明了在VB.NET中如何写Linq to XML查询语句。当从SQL Server
' 数据库查询数据时它使用了Linq to SQL。在本例中，您将看到VB.NET XML 文字方法
' 来创建,查询和编辑XML文档。
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

' 导入默认的XML命名空间
Imports <xmlns="http://cfx.codeplex.com">


Module MainModule

    Sub Main()

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 创建内存对象，基于这些对象生成XML文档并且查询XML文档中的数据 
        

        ' 内存对象XML文档路径
        Dim path As String = "Projects.xml"

        ' 创建内存对象
        Dim categories As Category() = CreateObjects()

        ' 基于这些内存对象创建XML文档
        CreateObjectXml(categories, path)

        ' 从内存中对象XML文档中查询数据
        QueryObjectXml(path)

        Console.WriteLine(vbNewLine)


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 基于SQL Server中的数据创建XML文档并且查询XML文档内的数据（我们使用
        ' 了Linq to SQL来查询数据库中的数据，有关Linq to SQL示例的详细信息请
        ' 参考All-In-One代码框架中的 CSLinqToSQL工程）
        '  


        ' 数据库XML文档路径
        path = "School.xml"

        ' 基于SQL Server中的数据创建XML文档
        CreateDatabaseXml(path)

        ' 查询数据库XML文档内的数据
        QueryDatabaseXml(path)

        Console.WriteLine(vbNewLine)


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 编辑由内存中对象创建的Projects.xml XML文档。插入，修改和删除特定的
        ' XML元素
        ' 

        ' XML 文档路径
        path = "Projects.xml"

        ' 编辑和保存XML文档
        EditDatabaseXml(path)


        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ' 从一个特定的URL查询web资源XML文档. . 
        ' 

        ' 特定web资源的URL
        'Dim url As String = "WebResourceURL"

        ' 解析web资源的XML文档
        'AnalyzeWebResourceXml(url)

    End Sub

#Region "Data Array Fields"

    ' 编程语言的String类型的数组
    Dim programmeLanguages As String() = New String() _
    { _
        "C#", _
        "VB.NET", _
        "C++" _
    }

    ' 代码框架的贡献者的String类型的数组
    Dim ownerNames As String() = New String() _
    { _
        "Jialiang Ge", _
        "Colbert Zhou", _
        "Hongye Sun", _
        "Lingzhi Sun", _
        "Jie Wang", _
        "Riquel Dong", _
        "Rongchun Zhang", _
        "Zhixin Ye" _
    }

#End Region

#Region "In-Memory Object XML Methods"

    ''' <summary>
    ''' 这个方法基于内存对象来创建XML文档
    ''' </summary>
    ''' <param name="categories">内存对象的数组</param>
    ''' <param name="path">内存中XML文档的路径</param>
    Sub CreateObjectXml(ByVal categories As Category(), ByVal path As String)
        ' 生成XML文档, 每个元素都在默认的命名空间ns: http://cfx.codeplex.com下
        ' 我们使用VB.NET 9.0的新特性,XML 文字.
        Dim doc As XDocument = _
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <Categories>
                <%= From category In categories _
                    Select _
                    <Category name=<%= category.Name %>>
                        <%= From project In category.Projects _
                            Select _
                            <Project name=<%= project.ProjectName %>>
                                <Language><%= programmeLanguages(project.Language) %></Language>
                                <Description><%= project.Description %></Description>
                                <Owner><%= ownerNames(project.Owner) %></Owner>
                            </Project> _
                        %>
                    </Category> _
                %>
            </Categories>

        ' 保存XML文档到文件系统
        doc.Save(path)
    End Sub


    ''' <summary>
    ''' 这个方法查询内存对象XML文档
    ''' </summary>
    ''' <param name="path">内存中XML文档的路径</param>
    Sub QueryObjectXml(ByVal path As String)
        ' 加载内存对象XML文档的根元素
        Dim element As XElement = XElement.Load(path)

        ' 查询所有拥有者是Jialiang Ge的工程
        Dim projects = From p In element...<Project> _
                       Where p.<Owner>.Value = ownerNames(Owners.Jialiang) _
                       Select p.@name

        ' 显示查询结果
        Console.WriteLine("{0}'s projects:", ownerNames(Owners.Jialiang))
        Console.WriteLine()

        For Each p In projects
            Console.WriteLine(p)
        Next

        Console.WriteLine()

        ' 在编程语言是C++的IPC和RPCQuery工程中查询工程名称和拥有者
        Dim owner = From c In element.<Category> _
                    From p In c.<Project> _
                    Where c.@name = "IPC and RPC" _
                    And p.<Language>.Value = _
                    programmeLanguages(ProgrammeLanguage.Cpp) _
                    Select New With _
                    { _
                        .ProjectName = p.@name, _
                        .Owner = p.<Owner>.Value _
                    }

        ' 显示查询结果
        Console.WriteLine("{0} examples in category IPC and RPC:", _
                          programmeLanguages(ProgrammeLanguage.Cpp))
        Console.WriteLine()

        For Each o In owner
            Console.WriteLine("Project {0} by {1}", o.ProjectName, o.Owner)
        Next
    End Sub


    ''' <summary>
    ''' 这个方法基于All-In-One代码框架示例信息来创建内存对象
    ''' </summary>
    ''' <returns>内存对象的数组</returns>
    Function CreateObjects() As Category()
        ' 基于All-In-One代码框架示例信息来生成内存对象数组。这里我们用了
        ' VB.NET 9.0的新特性，对象初始化器来生成对象。 
        Dim categories As Category() = New Category() _
        { _
            New Category With _
            { _
                .Name = "COM", _
                .Projects = New Project() _
                { _
                    New Project With _
                    { _
                        .ProjectName = "CSDllCOMServer", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "An in-process COM server in C#", _
                        .Owner = Owners.Jialiang _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "VBDllCOMServer", _
                        .Language = ProgrammeLanguage.VB, _
                        .Description = "An in-process COM server in " + _
                                        "VB.NET", _
                        .Owner = Owners.Jialiang _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "ATLDllCOMServer", _
                        .Language = ProgrammeLanguage.Cpp, _
                        .Description = "An in-process ATL COM Server", _
                        .Owner = Owners.Jialiang _
                    } _
                } _
            }, _
            New Category With _
            { _
                .Name = "Data Access", _
                .Projects = New Project() _
                { _
                    New Project With _
                    { _
                        .ProjectName = "CSUseADONET", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Use ADO.NET in a C# application", _
                        .Owner = Owners.LingzhiSun _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CppUseADONET", _
                        .Language = ProgrammeLanguage.Cpp, _
                        .Description = "Use ADO.NET in a C++ application", _
                        .Owner = Owners.Jialiang _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSLinqToObject", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Use LINQ to Objects in C#", _
                        .Owner = Owners.Colbert _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSLinqToSQL", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Use LINQ to SQL in C#", _
                        .Owner = Owners.RongchunZhang _
                    } _
                } _
            }, _
            New Category With _
            { _
                .Name = "Office", _
                .Projects = New Project() _
                { _
                    New Project With _
                    { _
                        .ProjectName = "CSOutlookUIDesigner", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Customize Outlook UI using VSTO " + _
                                        "Designers", _
                        .Owner = Owners.midnightfrank _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSOutlookRibbonXml", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Customize Outlook UI using Ribbon " + _
                                        "XML", _
                        .Owner = Owners.midnightfrank _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSAutomateExcel", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Automate Excel in a C# application", _
                        .Owner = Owners.Colbert _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "VBAutomateExcel", _
                        .Language = ProgrammeLanguage.VB, _
                        .Description = "Automate Excel in a VB.NET " + _
                                            "application", _
                        .Owner = Owners.Jialiang _
                    } _
                } _
            }, _
            New Category With _
            { _
                .Name = "IPC and RPC", _
                .Projects = New Project() _
                { _
                    New Project With _
                    { _
                        .ProjectName = "CppFileMappingServer", _
                        .Language = ProgrammeLanguage.Cpp, _
                        .Description = "Create shared memory in C++", _
                        .Owner = Owners.hongyes _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CppFileMappingClient", _
                        .Language = ProgrammeLanguage.Cpp, _
                        .Description = "Access shared memory in C++", _
                        .Owner = Owners.hongyes _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSReceiveWM_COPYDATA", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Receive WMCOPYDATA in C#", _
                        .Owner = Owners.Riquel _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSSendWM_COPYDATA", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Send WMCOPYDATA in C#", _
                        .Owner = Owners.Riquel _
                    } _
                } _
            }, _
            New Category With _
            { _
                .Name = "WinForm", _
                .Projects = New Project() _
                { _
                    New Project With _
                    { _
                        .ProjectName = "CSWinFormGeneral", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "General scenarios in WinForm", _
                        .Owner = Owners.ZhiXin _
                    }, _
                    New Project With _
                    { _
                        .ProjectName = "CSWinFormDataBinding", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "WinForm Data-binding in C#", _
                        .Owner = Owners.ZhiXin _
                    } _
                } _
            }, _
            New Category With _
            { _
                .Name = "Hook", _
                .Projects = New Project() _
                { _
                    New Project With _
                    { _
                        .ProjectName = "CSWindowsHook", _
                        .Language = ProgrammeLanguage.CS, _
                        .Description = "Windows Hook in a C# application", _
                        .Owner = Owners.RongchunZhang _
                    } _
                } _
            } _
        }

        ' 返回内存对象的数组
        Return categories
    End Function

#End Region

#Region "Database XML Methods"

    ''' <summary>
    ''' 这个方法基于All-In-One代码框架中的SQLServer2005DB数据库中的Person表 
    ''' 来创建XML文档 
    ''' </summary>
    ''' <param name="path">数据库XML文档路径</param>
    Sub CreateDatabaseXml(ByVal path As String)

        ' 创建Linq to SQL数据上下文对象
        ' 有关Linq to SQL的详细信息，请参考All-In-One代码框架中的CSLinqToSQL
        ' 工程
        Dim db As SchoolDataContext = New SchoolDataContext()

        ' 生成XML文档,每个元素都在默认的XML命名空间ns: http://cfx.codeplex.com下
        Dim doc As XDocument = _
            <?xml version="1.0" encoding="utf-8" standalone="yes"?>
            <Person>
                <Employees>
                    <%= From employee In db.Persons _
                        Where employee.PersonCategory = 2 _
                        Select _
                        <Employee id=<%= employee.PersonID %>>
                            <Name><%= employee.FirstName & " " & employee.LastName %></Name>
                            <HireDate><%= employee.HireDate.ToString() %></HireDate>
                        </Employee> _
                    %>
                </Employees>
                <Students>
                    <%= From student In db.Persons _
                        Where student.PersonCategory = 1 _
                        Select _
                        <Student id=<%= student.PersonID %>>
                            <Name><%= student.FirstName & " " & student.LastName %></Name>
                            <EnrollmentDate><%= student.EnrollmentDate.Value.ToString() %></EnrollmentDate>
                        </Student> _
                    %>
                </Students>
            </Person>

        ' 保存XML文档到文件系统
        doc.Save(path)
    End Sub


    ''' <summary>
    ''' 这个方法查询数据库XML文档
    ''' </summary>
    ''' <param name="path">数据库XML文档路径</param>
    Sub QueryDatabaseXml(ByVal path As String)
        ' 加载数据库XML文档的根元素  
        Dim doc = XDocument.Load(path)

        ' 查询入职日期在2000/01/01之后的所有员工
        Dim employees = From e In doc...<Employee> _
                        Where DateTime.Parse(e.<HireDate>.Value) > _
                        New DateTime(2000, 1, 1) _
                        Select e.<Name>.Value

        ' 显示查询结果
        Console.WriteLine("Employees whose hire date is later than " + _
                            "2000/01/01:")
        Console.WriteLine()

        For Each e In employees
            Console.WriteLine(e)
        Next
    End Sub

#End Region

#Region "Edit XML Method"

    ''' <summary>
    ''' 这个方法编辑文件系统中的XML文档
    ''' </summary>
    ''' <param name="path">XML文档路径</param>
    Sub EditDatabaseXml(ByVal path As String)
        Dim doc As XDocument = XDocument.Load(path)  ' 加载XML文档

        Dim element As XElement = doc.Root  ' 获取根元素

        ' 向XML文档插入新元素

        Console.WriteLine("Insert new Category XML and LINQ to XML projects XML elements...")

        ' 生成对象
        Dim xmlCategory As New Category() With _
        { _
            .Name = "XML", _
            .Projects = New Project() _
            { _
                New Project With _
                { _
                    .ProjectName = "CSLinqToXml", _
                    .Language = ProgrammeLanguage.CS, _
                    .Description = "Use LINQ to XML in C#", _
                    .Owner = Owners.LingzhiSun _
                }, _
                New Project With _
                { _
                    .ProjectName = "VBLinqToXml", _
                    .Language = ProgrammeLanguage.VB, _
                    .Description = "Use LINQ to XML in VB.NET", _
                    .Owner = Owners.LingzhiSun _
                } _
            } _
        }

        ' 将新创建的对象添加到根元素的最后一个元素
        element.Add( _
            <Category name=<%= xmlCategory.Name %>>
                <%= From project In xmlCategory.Projects _
                    Select _
                    <Project name=<%= project.ProjectName %>>
                        <Language><%= programmeLanguages(project.Language) %></Language>
                        <Description><%= project.Description %></Description>
                        <Owner><%= ownerNames(project.Owner) %></Owner>
                    </Project> _
                %>
            </Category>)

        ' 修改特定XML元素的值

        Console.WriteLine("Modify the prject CppUseADONET project's owner...")

        ' 检索特定的Category元素
        Dim categoryElement As XElement = element.<Category>. _
        Where(Function(c As XElement) c.@name = "Data Access").Single()

        If Not categoryElement Is Nothing Then
            ' 检索特定的Project元素
            Dim projectElement = categoryElement.<Project>. _
            Where(Function(p As XElement) p.@name = "CppUseADONET").Single()

            If Not projectElement Is Nothing Then
                ' 修改Owner元素的值
                projectElement.<Owner>.Value = ownerNames(Owners.LingzhiSun)
            End If
        End If

        ' 删除特定的XML元素

        Console.WriteLine("Delete the Hook Category element and its descendants...")

        ' 检索特定的Category元素
        categoryElement = element.<Category>. _
        Where(Function(c As XElement) c.@name = "Hook").Single()

        categoryElement.Remove()  ' 删除元素和所有它的后代

        doc.Save(path)  ' 保存XML文档
    End Sub

#End Region

#Region "Query Web Resource Method"

    ''' <summary>
    ''' 这个方法查询一个web资源XML文档
    ''' </summary>
    ''' <param name="url">web资源XML文档的URL</param>
    Sub AnalyzeWebResourceXml(ByVal url As String)
        Dim doc As XDocument = XDocument.Load(url)

        ' 使用和QueryObjectXml以及QueryDatabaseXml相似的查询方法从web资源
        ' 查询数据   
        '...
    End Sub

#End Region

End Module


#Region "In-Memory Object Classes"

' All-In-One代码框架工程Category类
Class Category
    ' category 名称属性
    Public Property Name() As String
        Get
            Return Me._name
        End Get
        Set(ByVal value As String)
            Me._name = value
        End Set
    End Property

    ' category 名称字段
    Private _name As String

    ' All-In-One代码框架projects对象数组属性
    Public Property Projects() As Project()
        Get
            Return Me._projects
        End Get
        Set(ByVal value As Project())
            Me._projects = value
        End Set
    End Property

    ' All-In-One代码框架projects对象数组字段
    Private _projects As Project()
End Class

' All-In-One代码框架project类
Class Project
    ' project名称属性
    Public Property ProjectName() As String
        Get
            Return Me._projectName
        End Get
        Set(ByVal value As String)
            Me._projectName = value
        End Set
    End Property

    ' project名称字段
    Private _projectName As String

    ' project编程语言属性
    Public Property Language() As ProgrammeLanguage
        Get
            Return Me._language
        End Get
        Set(ByVal value As ProgrammeLanguage)
            Me._language = value
        End Set
    End Property

    ' project编程语言字段
    Private _language As ProgrammeLanguage

    ' project描述属性
    Public Property Description() As String
        Get
            Return Me._description
        End Get
        Set(ByVal value As String)
            Me._description = value
        End Set
    End Property

    ' project描述字段
    Private _description As String

    ' project拥有者属性
    Public Property Owner() As Owners
        Get
            Return Me._owner
        End Get
        Set(ByVal value As Owners)
            Me._owner = value
        End Set
    End Property

    ' project拥有者字段
    Private _owner As Owners
End Class

#End Region


#Region "Data Enums"

' 编程语言的枚举，包括C#, VB.NET and C++
Enum ProgrammeLanguage
    CS
    VB
    Cpp
End Enum

' All-In-One代码框架贡献者在CodePlex的显示名称的枚举
Enum Owners
    Jialiang
    Colbert
    hongyes
    LingzhiSun
    midnightfrank
    Riquel
    RongchunZhang
    ZhiXin
End Enum

#End Region