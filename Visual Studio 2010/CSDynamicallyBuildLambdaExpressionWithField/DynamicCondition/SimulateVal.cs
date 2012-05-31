/********************************** 模块头 *****************************************\
* 模块名:  SimulateVal.cs
* 项目名:  CSDynamicallyBuildLambdaExpressionWithField
* 版权 (c) Microsoft Corporation.
* 
* SimulateVal.cs 文件定义了处理DateTime和Boolean的不同函数.
*
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
* EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
* MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

public static class SimulateVal
{
    public static double Val(string expression)
    {
        if (expression == null)
        {
            return 0;
        }
        // 先尝试整个字符串，然后逐渐处理较小的子字符串，
        // 以便模拟VB中“Val”的行为.
        // 它忽视了一个可识别值后面的尾随字符.
        for (int size = expression.Length; size > 0; size--)
        {
            double testDouble;
            if (double.TryParse(expression.Substring(0, size), out testDouble))
                return testDouble;
        }

        // 没有识别到任何值，所以返回0.
        return 0;
    }

    public static double Val(object expression)
    {
        if (expression == null)
        {
            return 0;
        }

        double testDouble;
        if (double.TryParse(expression.ToString(), out testDouble))
        {
            return testDouble;
        }
        //C#的Val函数在 true 时，返回-1.
        bool testBool;

        if (bool.TryParse(expression.ToString(), out testBool))
        {
            return testBool ? -1 : 0;
        }
        // C#的Val函数为日期返回月中的天.
        System.DateTime testDate;
        if (System.DateTime.TryParse(expression.ToString(), out testDate))
        {
            return testDate.Day;
        }
        // 没有识别到任何值，所以返回0.
        return 0;

    }

    /// <summary>
    /// 将字符转换为字符串.
    /// </summary>
    public static int Val(char expression)
    {
        int testInt;
        if (int.TryParse(expression.ToString(), out testInt))
        {
            return testInt;
        }
        else
        {
            return 0;
        }
    }
}