/********************************** 模块头 **********************************\
* 模块名:        DBProcess.cs
* 项目名:        CSExcelLikeGridView
* 版权(c) Microsoft Corporation
*
* 此模块管理链接,适配器以及数据表实例.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSExcelLikeGridView
{
    /// <summary>
    /// 此类管理整个db链接,创建基于内存的数据表,维持状态以及批量保存不同函数的相关信息.
    /// </summary>
    public sealed class DBProcess
    {
        private static SqlConnection conn = null;
        private static SqlDataAdapter adapter = null;
        private DataTable dt = null;

        /// <summary>
        /// 这个静态构造器将读取web.config中定义的全部连接字符串. 
        /// 链接和适配器将同时指向同一db, 因此只需要创建一次.
        /// </summary>
        static DBProcess()
        {
            string constr = ConfigurationManager.ConnectionStrings["MyConn"]
                                                .ConnectionString;
            conn = new SqlConnection(constr);
            string command = "select * from tb_personInfo";
            adapter = new SqlDataAdapter(command, conn);
            SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
            builder.GetDeleteCommand(true);
            builder.GetInsertCommand(true);
            builder.GetUpdateCommand(true);
        }

        /// <summary>
        /// 这个函数将创建一个数据表和一个"Table.dat"文件.
        /// 这是一个的序列化类型文件以避免基于内存的丢失问题.
        /// </summary>
        public  DataTable GetDataTable(bool create)
        {
            if (create)
            {
                using (FileStream fs = new FileStream("Table.dat", FileMode.Create))
                {
                    dt = new DataTable();
                    adapter.Fill(dt);
                    dt.Columns[0].AutoIncrement = true;
                    dt.Columns[0].AutoIncrementStep = 1;
                    dt.Columns[0].AutoIncrementSeed = dt.Rows.Count;
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, dt);
                }
            }
            else
            {
                using (FileStream fs = new FileStream("Table.dat",FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    dt = (DataTable)bf.Deserialize(fs);
                }
            }
            return dt;
        }

        /// <summary>
        /// 写入一个新的基于内存的数据表到文件.
        /// </summary>
        public void WriteDataTable(DataTable dt)
        {
            using (FileStream fs = new FileStream("Table.dat", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, dt);
            }
        }
        /// <summary>
        /// 更新DataTable删除序列化文件.
        /// </summary>
        public void BatchSave(DataTable dt)
        {
            adapter.Update(dt);
            File.Delete("Table.dat");
        }
    }
}