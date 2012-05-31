/**********************************模块头**********************************\
* 模块名:  CppUseADO.cpp
* 项目名:  CppUseADO
* 版权 (c) Microsoft Corporation.
* 
* CppUseADO示例演示了如何利用#import和Visual C++编写程序，以通过微软
* 的ADO（ActiveX Data Objects）技术来访问数据库。本示例展示了数据库操
* 作的基本结构，如连接数据源，执行SQL命令，使用Recordset对象和执行清理。   
* 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* ***************************************************************************/

#pragma region Includes and Imports
#include "stdafx.h"

#include <atlstr.h>

#import "C:\Program Files\Common Files\System\ADO\msado15.dll" \
	rename("EOF", "EndOfFile")

#include <locale.h>
#pragma endregion


DWORD ReadImage(PCTSTR pszImage, SAFEARRAY FAR **psaChunk);

int _tmain(int argc, _TCHAR* argv[])
{
	setlocale( LC_ALL, "chs" );

	HRESULT hr;
	::CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);

	ADODB::_ConnectionPtr spConn = NULL;
	ADODB::_RecordsetPtr spRst = NULL;

	try
	{
		/////////////////////////////////////////////////////////////////////
		// 连接数据源
		// 

		_tprintf(_T("正在连接数据库 ...\n"));

		//  定义连接字符串 (本数据源是示例数据库SQLServer2005DB) 
		// 

		// 方法1: 使用“Integrated security” 连接数据库 
		_bstr_t bstrConn("Provider=SQLOLEDB.1;Integrated Security=SSPI;Data Source=localhost\\SQLEXPRESS;Initial Catalog=SQLServer2005DB;");

		// 打开连接
		hr = spConn.CreateInstance(__uuidof(ADODB::Connection));
		hr = spConn->Open(bstrConn, "","", NULL);

		//// 方法2: 不使用“Integrated security” 连接数据库. 
		//_bstr_t bstrUserID("HelloWorld");
		//_bstr_t bstrPassword("111111");
		//_bstr_t bstrConn("Provider=SQLOLEDB.1;Data Source=localhost\\SQLEXPRESS;Initial Catalog=SQLServer2005DB;");
		//// 打开连接
		//hr = spConn.CreateInstance(__uuidof(ADODB::Connection));
		//hr = spConn->Open(bstrConn, bstrUserID, bstrPassword, NULL);
		

		/////////////////////////////////////////////////////////////////////
		// 编写并执行ADO命令.
		// 可以是SQL指令(SELECT/UPDATE/INSERT/DELETE),或是调用存储过程。  
		// 本处是INSERT命令的示例。
		// 

		_tprintf(_T("插入一条记录到表Person中\n"));

		//  1. 生成一个命令对象。
		ADODB::_CommandPtr spInsertCmd;
		hr = spInsertCmd.CreateInstance(__uuidof(ADODB::Command));
		
		// 2. 将连接赋值于命令对象。
		spInsertCmd->ActiveConnection = spConn;	

		// 3.  设置命令文本
		// SQL指令或是存储过程名  
		_bstr_t bstrInsertCmd(
			"INSERT INTO Person(LastName, FirstName, EnrollmentDate, Picture)" \
			" VALUES (?, ?, ?, ?)");
		spInsertCmd->CommandText = bstrInsertCmd;

		// 4. 设置命令类型
		// ADODB::adCmdText 用于普通的SQL指令; 
		// ADODB::adCmdStoredProc 用于存储过程.
		spInsertCmd->CommandType = ADODB::adCmdText;

		// 5. 添加参数

		//  LastName (nvarchar(50))参数的添加
		variant_t vtLastName("Ge");
		ADODB::_ParameterPtr spLastNameParam;
		spLastNameParam = spInsertCmd->CreateParameter(
			_bstr_t("LastName"),		// 参数名
			ADODB::adVarWChar,			// 参数类型 (NVarChar)
			ADODB::adParamInput,		// 参数传递方向
			50,							// 参数最大长度 (NVarChar(50)
			vtLastName);				// 参数值
		hr = spInsertCmd->Parameters->Append(spLastNameParam);

		//  FirstName (nvarchar(50))参数的添加
		variant_t vtFirstName("Jialiang");
		ADODB::_ParameterPtr spFirstNameParam;
		spFirstNameParam = spInsertCmd->CreateParameter(
			_bstr_t("FirstName"),		//参数名
			ADODB::adVarWChar,			// 参数类型 (NVarChar)
			ADODB::adParamInput,		// 参数传递方向
			50,							// 参数最大长度 (NVarChar(50)
			vtFirstName);				// 参数值
		hr = spInsertCmd->Parameters->Append(spFirstNameParam);

		// EnrollmentDate (datetime)参数的添加
		SYSTEMTIME sysNow;
		GetSystemTime(&sysNow);
		double dNow;
		SystemTimeToVariantTime(&sysNow, &dNow);
		variant_t vtEnrollmentDate(dNow, VT_DATE);
		ADODB::_ParameterPtr spEnrollmentParam;
		spEnrollmentParam = spInsertCmd->CreateParameter(
			_bstr_t("EnrollmentDate"),	// 参数名
			ADODB::adDate,				// 参数类型(DateTime)
            ADODB::adParamInput,		// 参数传递方向
            -1,							// 参数最大长度(对于datetime无效)
            vtEnrollmentDate);			// 参数值
		hr = spInsertCmd->Parameters->Append(spEnrollmentParam);

		// Image (image)参数的添加

		// 将图片文件读入一个safe array 
		SAFEARRAY FAR *psaChunk = NULL;
		int cbChunkBytes = ReadImage(_T("MSDN.jpg"), &psaChunk);
		variant_t vtChunk;
		if (cbChunkBytes > 0)	// 如果图片成功读取
		{
			// 将safe array赋值于一个variant变量
			vtChunk.vt = VT_ARRAY | VT_UI1;
			// safe array将在vtChunk释放时释放，因此没有必要调用SafeArrayDestroy 
			// 来释放safe array..
			vtChunk.parray = psaChunk;
		}
		else 
		{
			// // 将该参数的最大长度设置为一个有效值
			cbChunkBytes = 1;
			// 将该参数值设置为DBNull
			vtChunk.ChangeType(VT_NULL);
		}
		ADODB::_ParameterPtr spImageParam;
		spImageParam = spInsertCmd->CreateParameter(
			_bstr_t("Image"),			// 参数名
			ADODB::adLongVarBinary,		// 参数类型 (Image)
			ADODB::adParamInput,		// 参数传递方向
			cbChunkBytes,				// 以byte计算的最大长度
			vtChunk);					// 参数值
		hr = spInsertCmd->Parameters->Append(spImageParam);

		// 6.执行该命令
		spInsertCmd->Execute(NULL, NULL, ADODB::adExecuteNoRecords);


		/////////////////////////////////////////////////////////////////////
		// 使用Recordset对象.
		// http://msdn.microsoft.com/en-us/library/ms681510.aspx
		// Recordset对象表示一张表中记录或是命令执行结果的全部集合。 
		// 在任何时候，Recordset对象都指向集合中的单条记录，并将该条
		// 在任何时候，Recordset对象都指向集合中的单条记录，并将该条 
		// 
		// 

		_tprintf(_T("列举出表Person中的记录\n"));

		// 1. 生成一个Recordset对象
		hr = spRst.CreateInstance(__uuidof(ADODB::Recordset));
		
		// 2. 打开Recordset对象
		_bstr_t bstrSelectCmd("SELECT * FROM Person"); // WHERE ...
		hr = spRst->Open(bstrSelectCmd,	// SQL 指令/ 表，视图名 /  
										// 存储过程调用/ 文件名
			spConn.GetInterfacePtr(),	// 连接对象/ 连接字符串
			ADODB::adOpenForwardOnly,	// 游标类型. (只进游标)
			ADODB::adLockOptimistic,	// 锁定类型. (仅当调用Update方法 
										// 时，锁定记录。
			ADODB::adCmdText);			// 将第一个参数作为SQL命令和
										// 存储过程。

		// 3. 通过游标向前移动，枚举出记录
		spRst->MoveFirst();	// 移动到Recordset中的第一条记录
		while (!spRst->EndOfFile)
		{
			int nPersonId = spRst->Fields->Item["PersonID"]->Value.intVal;
			variant_t vtLastName(spRst->Fields->Item["LastName"]->Value);
			variant_t vtFirstName(spRst->Fields->Item["FirstName"]->Value);
			
			// 当在表中定义了一个可空字段，需要测试该字段值 
			// 是否为VT_NULL。
			_tprintf(_T("%d\t%s %s\n"), nPersonId,
				vtFirstName.vt == VT_NULL ? _T("(DBNull)") : (PCTSTR)vtFirstName.bstrVal, 
				vtLastName.vt == VT_NULL ? _T("(DBNull)") : (PCTSTR)vtLastName.bstrVal
				);

			// 当枚举Recordset中记录时，更新当前记录。
			//spRst->Fields->Item["XXXX"]->Value = XXXX
			//spRst->Update(); Or spRst->UpdateBatch(); 循环范围外。

			spRst->MoveNext();			// 移动到下一条记录
		}

	}
	catch (_com_error &err)
	{
		_tprintf(_T("应用程序出现错误: %s\n"), 
			err.ErrorMessage());
		_tprintf(_T("描述 = %s\n"), (LPCTSTR) err.Description());
	}


	/////////////////////////////////////////////////////////////////////////
	// 在退出前清理所有对象。
	// 

	_tprintf(_T("正在关闭连接 ...\n"));

	// 如果Recordset处于打开状态，关闭该对象。
	if (spRst && spRst->State == ADODB::adStateOpen)
		spRst->Close();

	// 如果连接处于打开状态，关闭该连接。
	if (spConn && spConn->State == ADODB::adStateOpen)
		spConn->Close();

	// 卸载本线程中的COM
	::CoUninitialize();

	return 0;
}


/*!
 * \概要
 * 将图片文件读入safe array.
 * 
 * \参数 pszImage
 * 图片文件的地址.
 * 
 * \参数 ppsaChunk
 * 输出safe array.
 * 
 * \返回值
 * *图片文件的byte数值.0表示失败
 */
DWORD ReadImage(PCTSTR pszImage, SAFEARRAY FAR **ppsaChunk)
{
	// 打开图片文件
	HANDLE hImage = CreateFile(pszImage, GENERIC_READ, FILE_SHARE_READ, NULL,
		OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);

	if (hImage == INVALID_HANDLE_VALUE) 
	{
		_tprintf(_T("无法打开图片文件 %s\n"), pszImage);
		return 0;
	}

	// 得到文件以byte表示的大小。
	LARGE_INTEGER liSize;
	if (!GetFileSizeEx(hImage, &liSize))
	{
		_tprintf(_T("无法得到图片的大小 w/err 0x%08lx\n"), 
			GetLastError());
		CloseHandle(hImage);
		return 0;
	}
	if (liSize.HighPart != 0)
	{
		_tprintf(_T("图片太大\n"));
		CloseHandle(hImage);
		return 0;
	}
	DWORD dwSize = liSize.LowPart, dwBytesRead;

	// 通过 cbChunkBytes元素生成一个safe array
	*ppsaChunk = SafeArrayCreateVector(VT_UI1, 1, dwSize);

	// 初始化safe array中的内容
	BYTE *pbChunk;
	SafeArrayAccessData(*ppsaChunk, (void **)&pbChunk);

	// 读取图片文件
	if (!ReadFile(hImage, pbChunk, dwSize, &dwBytesRead, NULL) 
		|| dwBytesRead == 0 || dwBytesRead != dwSize)
	{
		_tprintf(_T("无法从该文件中读取 w/err 0x%08lx\n"),
			GetLastError());
		CloseHandle(hImage);
		// 释放 safe array
		SafeArrayUnaccessData(*ppsaChunk);
		SafeArrayDestroy(*ppsaChunk);
        return 0;
	}

	SafeArrayUnaccessData(*ppsaChunk);

	CloseHandle(hImage);

	return dwSize;
}