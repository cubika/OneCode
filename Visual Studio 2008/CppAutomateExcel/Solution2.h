/****************************** 模块头 *************************************\
* 模块名:  Solution2.h
* 项目名:  CppAutomateExcel
* 版权(c)  Microsoft Corporation.
* 
* Solution2.h 和Solution2.cpp文件中的代码阐述了使用C/C++及COM API 自动化Excel的
* 过程。这种原始的自动化方式虽然实现起来更加困难,但有时通过它能够有效避免利用MFC方
* 式或#import指令时所产生的开销及问题。基本上,较为常用的有API中的CoCreateInstance()
* 及COM中的IDispatch和IUnknown。
*
* 该来源受微软授予的公共许可证约束。
* 详见 http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* 保留其它所有权。
* 
* 该代码和资料不提供任何形式，明示或暗示的担保， 包括但不限于适销特定用途的暗示性和
* 适用性的保证。
\***************************************************************************/

#pragma once


/*!
 * 使用C++和COM API 自动化Excel
 * 
 * \参数 lpParam
 * \返回值
 * 函数原型作为线程的起始地址
 */
DWORD WINAPI AutomateExcelByCOMAPI(LPVOID lpParam);


/*!
 * 自动化辅助函数
 * 
 * \参数 autoType
 * 可取值：
 * DISPATCH_PROPERTYGET || DISPATCH_PROPERTYPUT || DISPATCH_PROPERTYPUTREF
 * || DISPATCH_METHOD
 * 
 * \参数 pvResult
 * 保存返回值的VARIANT类型
 * 
 * \参数 pDisp
 * IDispatch 接口
 *
 * \参数 ptName
 * 接口提供的属性/方法名
 * 
 * \参数 cArgs
 * 形参数量
 * 
 * \返回值
 * HRESULT类型
 * 
 * AutoWrap()函数简化了直接使用IDispatch接口时所涉及的大部分底层细节问题。
 * 用户可根据自己的情况实现相关调用。需要注意的是，在传递多个参数时，必须以
 * 相反的顺序传递。
 * 
 * \示例
 * AutoWrap(
 *     DISPATCH_METHOD, NULL, pDisp, L"call", 3, parm[2], parm[1], parm[0]);
 */
HRESULT AutoWrap(int autoType, VARIANT *pvResult, IDispatch *pDisp,
				 LPOLESTR ptName, int cArgs...);