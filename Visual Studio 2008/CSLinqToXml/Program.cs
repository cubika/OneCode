/****************************** 模块头 ******************************\
* 模块名称:	    Program.cs
* 工程:		    CSLinqToXml
* 版权 (c) Microsoft Corporation.
* 
* 
* 本例阐明了如何在C#中使用Linq to XML从内存对象和SQL Server数据库来创建XML
* 文档。 它还同时阐明了在C#中如何写Linq to XML查询语句。当从SQL Server数据库
* 查询数据时它使用了Linq to SQL。在本例中，您将看到创建XML文档的基本的Linq to 
* XML 方法，核心的查询方法以及如何编辑XML文档。
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CSLinqToXml;
#endregion


class Program
{
    // 默认的 XML 命名空间
    private static XNamespace ns = "http://cfx.codeplex.com";

    static void Main(string[] args)
    {
        /////////////////////////////////////////////////////////////////////
        // 创建内存对象，基于这些对象生成XML文档并且查询XML文档中的数据 


        // 内存对象XML文档路径
        string path = "Projects.xml";

        // 创建内存对象
        Category[] categories = CreateObjects();

        // 基于这些内存对象创建XML文档
        CreateObjectXml(categories, path);

        // 从内存中对象XML文档中查询数据
        QueryObjectXml(path);

        Console.WriteLine("\r\n");


        /////////////////////////////////////////////////////////////////////
        // 基于SQL Server中的数据创建XML文档并且查询XML文档内的数据（我们使用
        // 了Linq to SQL来查询数据库中的数据，有关Linq to SQL示例的详细信息请
        // 参考All-In-One代码框架中的 CSLinqToSQL工程）


        // 数据库XML文档路径
        path = "School.xml";

        // 基于SQL Server中的数据创建XML文档
        CreateDatabaseXml(path);

        // 查询数据库XML文档内的数据
        QueryDatabaseXml(path);

        Console.WriteLine("\r\n");


        /////////////////////////////////////////////////////////////////////
        // 编辑由内存中对象创建的Projects.xml XML文档。插入，修改和删除特定的
        // XML元素

        // XML 文档路径
        path = "Projects.xml";

        // 编辑和保存XML文档
        EditDatabaseXml(path);


        /////////////////////////////////////////////////////////////////////
        // 从一个特定的URL查询web资源XML文档. 


        // 特定web资源的URL
        //string url = "WebResourceURL";

        // 解析web资源的XML文档
        //AnalyzeWebResourceXml(url);
    }

    #region Data Array Fields
    // 编程语言的String类型的数组
    private static string[] programmeLanguages = new string[3] 
    { 
        "C#", 
        "VB.NET", 
        "C++" 
    };


    // 代码框架的贡献者的String类型的数组
    private static string[] owners = new string[8] 
    {
        "Jialiang Ge", "Colbert Zhou", "Hongye Sun", "Lingzhi Sun", "Jie Wang",
        "Riquel Dong", "Rongchun Zhang", "Zhixin Ye"
    };
    #endregion

    #region In-Memory Object XML Methods
    /// <summary>
    /// 这个方法基于内存对象来创建XML文档
    /// </summary>
    /// <param name="categories">内存对象的数组</param>
    /// <param name="path">内存中XML文档的路径</param>
    private static void CreateObjectXml(Category[] categories, string path)
    {
        // 生成XML文档, 每个元素都在默认的命名空间ns: http://cfx.codeplex.com下
        XDocument doc = new XDocument(
            // XML 声明
            new XDeclaration("1.0", "uft-8", "yes"),
            // 根元素
            new XElement(ns + "Categories",
                // 嵌入Linq查询来生成XML子元素
                from c in categories
                select new XElement(ns + "Category",
                    // 创建元素属性
                    new XAttribute("name", c.Name),
                    from p in c.Projects
                    select new XElement(ns + "Project",
                        // 创建元素属性
                        new XAttribute("name", p.ProjectName),
                        new XElement(ns + "Language", 
                            programmeLanguages[(int)p.Language]),
                        new XElement(ns + "Description", p.Description),
                        new XElement(ns + "Owner", owners[(int)p.Owner])
                    )
                )
            )
        );

        // 保存XML文档到文件系统
        doc.Save(path);
    }


    /// <summary>
    /// 这个方法查询内存对象XML文档
    /// </summary>
    /// <param name="path">内存中XML文档的路径</param>
    private static void QueryObjectXml(string path)
    {
        // 加载内存对象XML文档的根元素
        XElement element = XElement.Load(path);

        // 查询所有拥有者是Jialiang Ge的工程
        var projects = from p in element.Descendants(ns + "Project")
                       where p.Element(ns + "Owner").Value == 
                       owners[(int)Owners.Jialiang]
                       select p.Attribute("name").Value;

        // 显示查询结果
        Console.WriteLine("{0}'s projects:\r\n", owners[(int)Owners.Jialiang]);

        foreach (var p in projects)
        {
            Console.WriteLine(p);
        }
        Console.WriteLine("\r\n");


        // 在编程语言是C++的IPC和RPCQuery工程中查询工程名称和拥有者
        var owner = from c in element.Elements(ns + "Category")
                    from p in c.Elements(ns + "Project")
                    where c.Attribute("name").Value == "IPC and RPC"
                    && p.Element(ns + "Language").Value == 
                    programmeLanguages[(int)ProgrammeLanguage.Cpp]
                    select new
                    {
                        ProjectName = p.Attribute("name").Value,
                        Owner = p.Element(ns + "Owner").Value
                    };

        // 显示查询结果
        Console.WriteLine("{0} examples in category IPC and RPC:\r\n",
            programmeLanguages[(int)ProgrammeLanguage.Cpp]);

        foreach (var o in owner)
        {
            Console.WriteLine("Project {0} by {1}", o.ProjectName, o.Owner);
        }
    }


    /// <summary>
    /// 这个方法基于All-In-One代码框架示例信息来创建内存对象
    /// </summary>
    /// <returns>内存对象的数组</returns>
    private static Category[] CreateObjects()
    {
        // 基于All-In-One代码框架示例信息来生成内存对象数组。这里我们用了
        // C# 3.0的新特性，对象初始化器和集合初始化器来生成对象。 
        Category[] categories = new Category[]
        {
            new Category
            {
                Name = "COM", 
                Projects = new Project[]
                {
                    new Project
                    {
                        ProjectName = "CSDllCOMServer", 
                        Language = ProgrammeLanguage.CS,        
                        Description = "An in-process COM server in C#",        
                        Owner = Owners.Jialiang       
                    },                          
                    new Project
                    {
                        ProjectName = "VBDllCOMServer",
                        Language = ProgrammeLanguage.VB,
                        Description = "An in-process COM server in VB.NET",
                        Owner = Owners.Jialiang
                    },
                    new Project
                    {
                        ProjectName = "ATLDllCOMServer",
                        Language = ProgrammeLanguage.Cpp,
                        Description = "An in-process ATL COM Server",
                        Owner = Owners.Jialiang
                    }
                }
            },
            new Category
            {
                Name = "Data Access",
                Projects = new Project[]
                {
                    new Project
                    {
                        ProjectName = "CSUseADONET",
                        Language = ProgrammeLanguage.CS,
                        Description = "Use ADO.NET in a C# application",
                        Owner = Owners.LingzhiSun
                    },
                    new Project
                    {
                        ProjectName = "CppUseADONET",
                        Language = ProgrammeLanguage.Cpp,
                        Description = "Use ADO.NET in a C++ application",
                        Owner = Owners.Jialiang
                    },
                    new Project
                    {
                        ProjectName = "CSLinqToObject",
                        Language = ProgrammeLanguage.CS,
                        Description = "Use LINQ to Objects in C#",
                        Owner = Owners.Colbert
                    },
                    new Project
                    {
                        ProjectName = "CSLinqToSQL",
                        Language = ProgrammeLanguage.CS,
                        Description = "Use LINQ to SQL in C#",
                        Owner = Owners.RongchunZhang
                    }
                }
            },
            new Category
            {
                Name = "Office",
                Projects = new Project[]
                {
                    new Project
                    {
                        ProjectName = "CSOutlookUIDesigner",
                        Language = ProgrammeLanguage.CS,
                        Description = "Customize Outlook UI using VSTO" + 
                                        " Designers",
                        Owner = Owners.midnightfrank
                    },
                    new Project
                    {
                        ProjectName = "CSOutlookRibbonXml",
                        Language = ProgrammeLanguage.CS,
                        Description = "Customize Outlook UI using Ribbon " + 
                                        "XML",
                        Owner = Owners.midnightfrank
                    },
                    new Project
                    {
                        ProjectName = "CSAutomateExcel",
                        Language = ProgrammeLanguage.CS,
                        Description = "Automate Excel in a C# application",
                        Owner = Owners.Colbert
                    },
                    new Project
                    {
                        ProjectName = "VBAutomateExcel",
                        Language = ProgrammeLanguage.VB,
                        Description = "Automate Excel in a VB.NET " + 
                                        "application",
                        Owner = Owners.Jialiang
                    }
                }
            },
            new Category
            {
                Name = "IPC and RPC",
                Projects = new Project[]
                {
                    new Project
                    {
                        ProjectName = "CppFileMappingServer",
                        Language = ProgrammeLanguage.Cpp,
                        Description = "Create shared memory in C++",
                        Owner = Owners.hongyes
                    },
                    new Project
                    {
                        ProjectName = "CppFileMappingClient",
                        Language = ProgrammeLanguage.Cpp,
                        Description = "Access shared memory in C++",
                        Owner = Owners.hongyes
                    },
                    new Project
                    {
                        ProjectName = "CSReceiveWM_COPYDATA",
                        Language = ProgrammeLanguage.CS,
                        Description = "Receive WMCOPYDATA in C#",
                        Owner = Owners.Riquel
                    },
                    new Project
                    {
                        ProjectName = "CSSendWM_COPYDATA",
                        Language = ProgrammeLanguage.CS,
                        Description = "Send WMCOPYDATA in C#",
                        Owner = Owners.Riquel
                    }
                }
            },
            new Category
            {
                Name = "WinForm",
                Projects = new Project[]
                {
                    new Project
                    {
                        ProjectName = "CSWinFormGeneral",
                        Language = ProgrammeLanguage.CS,
                        Description = "General scenarios in WinForm",
                        Owner = Owners.ZhiXin
                    },
                    new Project
                    {
                        ProjectName = "CSWinFormDataBinding",
                        Language = ProgrammeLanguage.CS,
                        Description = "WinForm Data-binding in C#",
                        Owner = Owners.ZhiXin
                    }
                }
            },
            new Category
            {
                Name = "Hook",
                Projects = new Project[]
                {
                    new Project
                    {
                        ProjectName = "CSWindowsHook",
                        Language = ProgrammeLanguage.CS,
                        Description = "Windows Hook in a C# application",
                        Owner = Owners.RongchunZhang
                    }
                }
            }
        };

        // 返回内存对象的数组
        return categories;
    }
    #endregion

    #region Database XML Methods
    /// <summary>
    /// 这个方法基于All-In-One代码框架中的SQLServer2005DB数据库中的Person表
    /// 来创建XML文档   
    /// </summary>
    /// <param name="path">数据库XML文档路径</param>
    private static void CreateDatabaseXml(string path)
    {
        // 创建Linq to SQL数据上下文对象
        // 有关Linq to SQL的详细信息，请参考All-In-One代码框架中的CSLinqToSQL
        // 工程
        SchoolDataContext db = new SchoolDataContext();

        // 生成XML文档,每个元素都在默认的XML命名空间ns: http://cfx.codeplex.com下
        XDocument doc = new XDocument(
            // XML 声明
            new XDeclaration("1.0", "utf-8", "yes"),
            // 根元素
            new XElement(ns + "Person",
                // Employees 元素
                new XElement(ns + "Employees",
                // 嵌入Linq查询来生成子XML元素
                    from e in db.Persons
                    where e.PersonCategory == 2
                    select new XElement(ns + "Employee",
                        // 创建元素的属性 
                        new XAttribute("id", e.PersonID),
                        new XElement(ns + "Name", e.FirstName + " " + 
                            e.LastName),
                        new XElement(ns + "HireDate", 
                            e.HireDate.Value.ToString())
                        )
                ),
                // Students 元素
                new XElement(ns + "Students",
                // 嵌入Linq查询来生成子XML元素
                    from s in db.Persons
                    where s.PersonCategory == 1
                    select new XElement(ns + "Student",
                        // 创建元素的属性
                        new XAttribute("id", s.PersonID),
                        new XElement(ns + "Name", s.FirstName + " " + 
                            s.LastName),
                        new XElement(ns + "EnrollmentDate", 
                            s.EnrollmentDate.Value.ToString())
                        )
                )
            )
        );

        // 保存XML文档到文件系统
        doc.Save(path);
    }


    /// <summary>
    /// 这个方法查询数据库XML文档
    /// </summary>
    /// <param name="path">数据库XML文档路径</param>
    private static void QueryDatabaseXml(string path)
    {
        // 加载数据库XML文档的根元素 
        XDocument doc = XDocument.Load(path);

        // 查询入职日期在2000/01/01之后的所有员工
        var employees = from e in doc.Descendants(ns + "Employee")
                        where DateTime.Parse(e.Element(ns + "HireDate").Value)
                        > new DateTime(2000, 1, 1)
                        select e.Element(ns + "Name").Value;

        // 显示查询结果
        Console.WriteLine("Employees whose hire date is later than " + 
            "2000/01/01:\r\n");

        foreach (var e in employees)
        {
            Console.WriteLine(e);
        }
    }
    #endregion

    #region Edit XML Method
    /// <summary>
    /// 这个方法编辑文件系统中的XML文档
    /// </summary>
    /// <param name="path">XML文档路径</param>
    private static void EditDatabaseXml(string path)
    {
        // 加载XML文档
        XDocument doc = XDocument.Load(path);

        // 获取根元素
        XElement element = doc.Root;


        // 向XML文档插入新元素

        Console.WriteLine("Insert new Category XML and LINQ to XML " +
                            "projects XML elements...");

        // 生成对象
        Category xmlCategory = new Category()
        {
            Name = "XML",
            Projects = new Project[]
            {
                new Project
                {
                    ProjectName = "CSLinqToXml",
                    Language = ProgrammeLanguage.CS,
                    Description = "Use LINQ to XML in C#",
                    Owner = Owners.LingzhiSun
                },
                new Project
                {
                    ProjectName = "VBLinqToXml",
                    Language = ProgrammeLanguage.VB,
                    Description = "Use LINQ to XML in VB.NET",
                    Owner = Owners.LingzhiSun
                }
            }
        };

        // 将新创建的对象添加到根元素的最后一个元素
        element.Add(
            new XElement(ns + "Category",
                new XAttribute("name", xmlCategory.Name),
                from p in xmlCategory.Projects
                select new XElement(ns + "Project",
                    new XAttribute("name", p.ProjectName),
                    new XElement(ns + "Language", 
                        programmeLanguages[(int)p.Language]),
                    new XElement(ns + "Description", p.Description),
                    new XElement(ns + "Owner", owners[(int)p.Owner])
                )
            )
        );


        // 修改特定XML元素的值

        Console.WriteLine("Modify the prject CppUseADONET project's" +
                            " owner...");

        // 检索特定的Category元素
        XElement categoryElement = element.Elements(ns + "Category").
            Where(c => c.Attribute("name").Value == "Data Access").Single();

        if (categoryElement != null)
        {
            // 检索特定的Project元素
            var projectElement = categoryElement.Elements(ns + "Project").
                Where(p => p.Attribute("name").Value == "CppUseADONET").Single();

            if (projectElement != null)
            {
                // 修改Owner元素的值
                projectElement.Element(ns + "Owner").Value = 
                    owners[(int)Owners.LingzhiSun];
            }
        }


        // 删除特定的XML元素

        Console.WriteLine("Delete the Hook Category element and its descendants...");

        // 检索特定的Category元素
        categoryElement = element.Elements(ns + "Category").
            Where(c => c.Attribute("name").Value == "Hook").Single();

        // 删除元素和所有它的后代
        categoryElement.Remove();

        // 保存XML文档
        doc.Save(path);
    }
    #endregion

    #region Query Web Resource Method
    /// <summary>
    /// 这个方法查询一个web资源XML文档
    /// </summary>
    /// <param name="url">web资源XML文档的URL</param>
    private static void AnalyzeWebResourceXml(string url)
    {
        XDocument doc = XDocument.Load(url);

        // 使用和QueryObjectXml以及QueryDatabaseXml相似的查询方法从web资源
        // 查询数据 
        // ...
    }
    #endregion
}

#region In-Memory Object Classes
// All-In-One代码框架工程Category类
public class Category
{
    // category 名称
    public string Name { get; set; }

    // All-In-One代码框架projects对象数组
    public Project[] Projects { get; set; }
}

// All-In-One代码框架project类
public class Project
{
    // project 名称
    public string ProjectName { get; set; }

    // project 编程语言
    public ProgrammeLanguage Language { get; set; }

    // project 描述
    public string Description { get; set; }

    // project 拥有者
    public Owners Owner { get; set; }
}
#endregion

#region Data Enums
// 编程语言的枚举，包括C#, VB.NET and C++
public enum ProgrammeLanguage
{
    CS,
    VB,
    Cpp
}

// All-In-One代码框架贡献者在CodePlex的显示名称的枚举
public enum Owners
{
    Jialiang, Colbert, hongyes, LingzhiSun, midnightfrank, Riquel, 
    RongchunZhang, ZhiXin
}
#endregion