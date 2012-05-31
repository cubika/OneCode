=============================================================================
         ASP.NET 应用程序 :  CSASPNETMVCPager 项目 概述 
=============================================================================

/////////////////////////////////////////////////////////////////////////////
用法:

  此项目演示了如何自定义html分页扩展方法.
  在项目中我们将定义名为Pager<T>类 初始化分页基本属性,比如总页数,
  每页显示记录数等等.同时在CustomizePager类中定义了一个html辅助方法.


/////////////////////////////////////////////////////////////////////////////
前提条件:

ASP.NET MVC RTM and .NET Framework 3.5
您可以通过下列链接下载ASP.NET MVC2 RTM.
http://www.microsoft.com/downloads/details.aspx?displaylang=en&FamilyID=c9ba1fe1-3ba8-439a-9e21-def90a8615a9


/////////////////////////////////////////////////////////////////////////////
生成:

步骤1: 在Visual Studio 2008中创建一个Visual C# ASP.NET MVC应用程序 
并命名为CSASPNETMVCPager.

步骤2: 添加一个名为Images的新文件夹向其中添加下一页和上一页的图片.

步骤3: 添加一个名为Helper的新文件夹向其中添加一个名为
CustomizePager.cs的类文件.

步骤4: 创建Pager<T>类包含数据源和用来渲染页面html代码的
CustomizePager类的基本信息.
    public class Pager<T>
    {
        /// <summary>
        /// determine how many records displayed in one page
        /// </summary>
        public int pageSize = 6;
    
        /// <summary>
        /// instantiate a pager object
        /// </summary>
        /// <param name="collection">datasource which implement ICollection<T></param>
        /// <param name="currentPageIndex">Current page index</param>
        /// <param name="requestBaseUrl">Request base url</param>
        public Pager(ICollection<T> collection,int currentPageIndex,string requestBaseUrl)
        {
            //Initialize properties
        }
        
        /// <summary>
        /// instantiate a pager object
        /// </summary>
        /// <param name="collection">datasource which implement ICollection<T></param>
        /// <param name="currentPageIndex">Current page index</param>
        /// <param name="requestBaseUrl">Request base url</param>
        /// <param name="imgUrlForUp">image for previous page</param>
        /// <param name="imgUrlForDown">image for next page</param>
        public Pager(ICollection<T> collection, int currentPageIndex, string requestBaseUrl, string imgUrlForUp, string imgUrlForDown)
        {
            //Initialize properties
        }
        
        /// <summary>
        /// instantiate a pager object
        /// </summary>
        /// <param name="collection">datasource which implement ICollection<T></param>
        /// <param name="currentPageIndex">Current page index</param>
        /// <param name="requestBaseUrl">Request base url</param>
        /// <param name="imgUrlForUp">image for previous page</param>
        /// <param name="imgUrlForDown">image for next page</param>
        /// <param name="pagesSize">determine the size of page numbers displayed</param>
        public Pager(IList<T> collection, int currentPageIndex, string requestBaseUrl, string imgUrlForUp, string imgUrlForDown, int pagesSize)
        {
            //Initialize properties
        }
    
        /// <summary>
        /// current page index
        /// </summary>
        public int CurrentPageIndex { get; private set; }
        
        /// <summary>
        /// total pages
        /// </summary>
        public int TotalPages { get; private set; }
        
        /// <summary>
        /// base url and id value construct a whole url, eg:http://RequestBaseUrl/id
        /// </summary>
        public string RequestBaseUrl { get; private set; }
        
        /// <summary>
        /// image for previous page
        /// </summary>
        public string ImageUrlForUp { get; private set; }
        
        /// <summary>
        /// image for next page
        /// </summary>
        public string ImageUrlForDown { get; private set; }
        
        /// <summary>
        /// size of page numbers which are displayed
        /// </summary>
        public int PagesSize { get; private set; }
    }

步骤5: 在Model文件夹中创建作为示例的模型的Employee类和生成示例数据
的EmployeeSet静态类. 方便起见我们手动生成示例数据. 
你可以根据自己的需求创建任何自ICollection<T>继承的数据结构.

步骤6: 修改Home控制控制器Index视图准备分页.

    public ActionResult Index(int ? id)
    {
        int pageIndex = Convert.ToInt32(id);
        List<Employee> empList=EmployeeSet.Employees;
        int pagesSize = 5;
            
        Pager<Employee> pager = new Pager<Employee>(empList , pageIndex, Url.Content("~/Home/Index"), Url.Content("~/images/left.gif"), Url.Content("~/images/right.gif"), pagesSize);

        ViewData["pager"] =pager;
        return View(empList.Skip(pager.pageSize * pageIndex).Take(pager.pageSize));
    }


步骤7: 修改在Views文件夹中Home控制控制器Index视图渲染雇员信息输出分页.

步骤8: 生成并运行这个ASP.NET 项目.


/////////////////////////////////////////////////////////////////////////////
参考资料:

http://www.asp.net/mvc/fundamentals/


/////////////////////////////////////////////////////////////////////////////