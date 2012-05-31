/******************************************模块头******************************************\
 * 模块名:  Program.cs
 * 项目名:  CSUseADO
 * 版权 (c) Microsoft Corporation.
 *
 * CSUseADO示例演示了使用Visual C#编写程序，通过微软的ADO（ActiveX Data Objects）技术来访 
 * 问数据库。本示例显示了数据库操作的基本结构，如连接数据源，执行SQL命令，使用DataSet对象 
 * 和执行清理。  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\******************************************************************************************/

using System;

class Program
{
    static void Main(string[] args)
    {
        ADODB.Connection conn = null;
        ADODB.Recordset rs = null;



        try
        {
            ////////////////////////////////////////////////////////////////////////////////
            // 连接数据源.
            // 

            Console.WriteLine("正在连接数据库 ...");

            // 获取连接字符串 
            string connStr = string.Format("Provider=SQLOLEDB;Data Source={0};Initial Catalog={1};Integrated Security=SSPI",
                ".\\sqlexpress", "SQLServer2005DB");
            

            // 打开连接
            conn = new ADODB.Connection();
            conn.Open(connStr, null, null, 0);


            ////////////////////////////////////////////////////////////////////////////////
            // 编写并执行ADO命令.
            // 可以是SQL指令(SELECT/UPDATE/INSERT/DELETE),或是调用存储过程.  
            // 此处是一个INSERT命令示例.
            // 

            Console.WriteLine("将一条记录插入表CountryRegion中...");

            // 1. 生成一个Command对象
            ADODB.Command cmdInsert = new ADODB.Command();

            // 2. 将连接赋值于命令
            cmdInsert.ActiveConnection = conn;

            // 3. 设置命令文本
            //  SQL指令或者存储过程名 
            cmdInsert.CommandText = "INSERT INTO CountryRegion(CountryRegionCode, Name, ModifiedDate)"
                + " VALUES (?, ?, ?)";

            // 4. 设置命令类型
            // ADODB.CommandTypeEnum.adCmdText 用于普通的SQL指令; 
            // ADODB.CommandTypeEnum.adCmdStoredProc 用于存储过程.
            cmdInsert.CommandType = ADODB.CommandTypeEnum.adCmdText;

            // 5. 添加参数

            //  CountryRegionCode (nvarchar(20)参数的添加
            ADODB.Parameter paramCode = cmdInsert.CreateParameter(
                "CountryRegionCode",                        // 参数名
                ADODB.DataTypeEnum.adVarChar,               // 参数类型 (nvarchar(20))
                ADODB.ParameterDirectionEnum.adParamInput,  // 参数类型
                20,                                         // 参数的最大长度
                "ZZ"+DateTime.Now.Millisecond);             // 参数值
            cmdInsert.Parameters.Append(paramCode);

            // Name (nvarchar(200))参数的添加
            ADODB.Parameter paramName = cmdInsert.CreateParameter(
                "Name",                                     // 参数名
                ADODB.DataTypeEnum.adVarChar,               // 参数类型 (nvarchar(200))
                ADODB.ParameterDirectionEnum.adParamInput,  // 参数传递方向
                200,                                        // 参数的最大长度
                "Test Region Name");                        // 参数值
            cmdInsert.Parameters.Append(paramName);

            // ModifiedDate (datetime)参数的添加
            ADODB.Parameter paramModifiedDate = cmdInsert.CreateParameter(
                "ModifiedDate",                             // 参数名
                ADODB.DataTypeEnum.adDate,                  // 参数类型 (datetime)
                ADODB.ParameterDirectionEnum.adParamInput,  // 参数传递方向
                -1,                                         // 参数的最大长度 (datetime忽视该值)
                DateTime.Now);                              // 参数值
            cmdInsert.Parameters.Append(paramModifiedDate);


            // 6. 执行命令
            object nRecordsAffected = Type.Missing;
            object oParams = Type.Missing;
            cmdInsert.Execute(out nRecordsAffected, ref oParams,
                (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);


            ////////////////////////////////////////////////////////////////////////////////
            // 使用Recordset对象.
            // http://msdn.microsoft.com/en-us/library/ms681510.aspx
            // Recordset表示了数据表中记录或执行命令获得的结果的集合。  
            // 在任何时候， Recordset对象都指向集合中的单条记录，并将  
            // 该记录作为它的当前记录。
            // 

            Console.WriteLine("列出表CountryRegion中的所有记录");

            // 1. 生成Recordset对象
            rs = new ADODB.Recordset();

            // 2. 打开Recordset对象
            string strSelectCmd = "SELECT * FROM CountryRegion"; // WHERE ...
            rs.Open(strSelectCmd,                                // SQL指令/表,视图名 / 
                                                                 // 存储过程调用 /文件名
                conn,                                            // 连接对象/连接字符串
                ADODB.CursorTypeEnum.adOpenForwardOnly,          // 游标类型. (只进游标)
                ADODB.LockTypeEnum.adLockOptimistic,	         // 锁定类型. (仅当需要调用 
                                                                 // 更新方法时，才锁定记录）
                (int)ADODB.CommandTypeEnum.adCmdText);	         // 将第一个参数视为SQL命令
                                                                 // 或存储过程.

            // 3. 通过向前移动游标列举记录

            // 移动到Recordset中的第一条记录
            rs.MoveFirst(); 
            while (!rs.EOF)
            {
                // 当在表中定义了一个可空字段，需要检验字段中的值是否为DBNull.Value.
                string code = (rs.Fields["CountryRegionCode"].Value == DBNull.Value) ?
                    "(DBNull)" : rs.Fields["CountryRegionCode"].Value.ToString();

                string name = (rs.Fields["Name"].Value == DBNull.Value) ?
                    "(DBNull)" : rs.Fields["Name"].Value.ToString();

                DateTime modifiedDate = (rs.Fields["ModifiedDate"].Value == DBNull.Value) ?
                    DateTime.MinValue : (DateTime)rs.Fields["ModifiedDate"].Value;

                Console.WriteLine(" {2} \t{0}\t{1}", code, name, modifiedDate.ToString("yyyy-MM-dd"));

                // 移动到下一条记录
                rs.MoveNext();   
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("应用程序出现错误: {0}", ex.Message);
            if (ex.InnerException != null)
                Console.WriteLine("描述: {0}", ex.InnerException.Message);
        }
        finally
        {
            ////////////////////////////////////////////////////////////////////////////////
            // 退出前清理对象.
            // 

            Console.WriteLine("正在关闭连接 ...");

            // 关闭record set，当它处于打开状态时
            if (rs != null && rs.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                rs.Close();

            // 关闭数据库连接，当它处于打开状态时
            if (conn != null && conn.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                conn.Close();
        }
    }
}

