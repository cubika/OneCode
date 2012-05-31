/****************************** 模块头 ******************************\
* 模块名:  ExecutableFile.cs
* 项目名:	    CSCheckEXEType
* 版权 (c) Microsoft Corporation.
* 
* 这个类表示一个可执行文件. 它可以这个映像文件中获取这个映像文件头部,
* 可选映像文件头部 和数据目录. 根据这些头部, 
* 我们能知道这是否是一个控制台应用程序, 
* 是否是一个.NET应用程序以及是否是32位Native应用程序. 
* 对于.NET应用程序, 它能生成全名，如  
*  System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace CSCheckEXEType
{
    public class ExecutableFile
    {
        /// <summary>
        /// 可执行文件路径.
        /// </summary>
        public string ExeFilePath { get; private set; }

        private IMAGE.IMAGE_FILE_HEADER imageFileHeader;

        /// <summary>
        /// 映像文件头部.
        /// </summary>
        public IMAGE.IMAGE_FILE_HEADER ImageFileHeader
        {
            get { return imageFileHeader; }
        }

        private IMAGE.IMAGE_OPTIONAL_HEADER32 optinalHeader32;

        /// <summary>
        /// 针对32位可执行的可选映像头部.
        /// </summary>
        public IMAGE.IMAGE_OPTIONAL_HEADER32 OptinalHeader32
        {
            get { return optinalHeader32; }
        }

        private IMAGE.IMAGE_OPTIONAL_HEADER64 optinalHeader64;

        /// <summary>
        /// 针对64位可执行文件的可选映像头部.
        /// </summary>
        public IMAGE.IMAGE_OPTIONAL_HEADER64 OptinalHeader64
        {
            get { return optinalHeader64; }
        }

        private IMAGE.IMAGE_DATA_DIRECTORY_Values directoryValues;

        /// <summary>
        /// 数据目录.
        /// </summary>
        public IMAGE.IMAGE_DATA_DIRECTORY_Values DirectoryValues
        {
            get { return directoryValues; }
        }

        /// <summary>
        /// 表示这是否是一个控制台应用程序. 
        /// </summary>
        public bool IsConsoleApplication
        {
            get
            {
                if (Is32bitImage)
                {
                    return optinalHeader32.Subsystem == 3;
                }
                else
                {
                    return optinalHeader64.Subsystem == 3;
                }
            }
        }

        /// <summary>
        /// 表示这是否是一个.NET应用程序. 
        /// </summary>
        public bool IsDotNetAssembly
        {
            get
            {
                return directoryValues.Values[IMAGE.IMAGE_DATA_DIRECTORY_Values.IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR].VirtualAddress != 0;
            }
        }

        /// <summary>
        /// 表示这是否是一个32位应用程序. 
        /// </summary>
        public bool Is32bitImage { get; private set; }



        public ExecutableFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("进程未找到.");
            }

            this.ExeFilePath = filePath;

            this.ReadHeaders();
        }

        /// <summary>
        /// 读取映像文件头部.
        /// </summary>
        void ReadHeaders()
        {

            FileStream fs = null;
            BinaryReader binReader = null;

            try
            {
                fs = new FileStream(ExeFilePath, FileMode.Open, FileAccess.Read);
                binReader = new BinaryReader(fs);

                // 读取PE头部开始偏移量.
                fs.Position = 0x3C;
                UInt32 headerOffset = binReader.ReadUInt32();

                // 核对偏移量是否有效
                if (headerOffset > fs.Length - 5)
                {
                    throw new ApplicationException("无效映像格式");
                }

                // 读取PE文件签名
                fs.Position = headerOffset;
                UInt32 signature = binReader.ReadUInt32();

                // 0x00004550 表示字母“PE”，其后有两个终止符0.
                if (signature != 0x00004550)
                {
                    throw new ApplicationException("无效映像格式");
                }

                // 读取映像文件头部. 
                this.imageFileHeader.Machine = binReader.ReadUInt16();
                this.imageFileHeader.NumberOfSections = binReader.ReadUInt16();
                this.imageFileHeader.TimeDateStamp = binReader.ReadUInt32();
                this.imageFileHeader.PointerToSymbolTable = binReader.ReadUInt32();
                this.imageFileHeader.NumberOfSymbols = binReader.ReadUInt32();
                this.imageFileHeader.SizeOfOptionalHeader = binReader.ReadUInt16();
                this.imageFileHeader.Characteristics = (IMAGE.IMAGE_FILE_Flag)binReader.ReadUInt16();

                // 判断是否是32位程序集.
                UInt16 magic = binReader.ReadUInt16();
                if (magic != 0x010B && magic != 0x020B)
                {
                    throw new ApplicationException("无效映像格式");
                }

                // 对32应用程序读取 IMAGE_OPTIONAL_HEADER32 字段.
                if (magic == 0x010B)
                {
                    this.Is32bitImage = true;

                    this.optinalHeader32.Magic = magic;
                    this.optinalHeader32.MajorLinkerVersion = binReader.ReadByte();
                    this.optinalHeader32.MinorImageVersion = binReader.ReadByte();
                    this.optinalHeader32.SizeOfCode = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfInitializedData = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfUninitializedData = binReader.ReadUInt32();
                    this.optinalHeader32.AddressOfEntryPoint = binReader.ReadUInt32();
                    this.optinalHeader32.BaseOfCode = binReader.ReadUInt32();
                    this.optinalHeader32.BaseOfData = binReader.ReadUInt32();
                    this.optinalHeader32.ImageBase = binReader.ReadUInt32();
                    this.optinalHeader32.SectionAlignment = binReader.ReadUInt32();
                    this.optinalHeader32.FileAlignment = binReader.ReadUInt32();
                    this.optinalHeader32.MajorOperatingSystemVersion = binReader.ReadUInt16();
                    this.optinalHeader32.MinorOperatingSystemVersion = binReader.ReadUInt16();
                    this.optinalHeader32.MajorImageVersion = binReader.ReadUInt16();
                    this.optinalHeader32.MinorImageVersion = binReader.ReadUInt16();
                    this.optinalHeader32.MajorSubsystemVersion = binReader.ReadUInt16();
                    this.optinalHeader32.MinorSubsystemVersion = binReader.ReadUInt16();
                    this.optinalHeader32.Win32VersionValue = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfImage = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfHeaders = binReader.ReadUInt32();
                    this.optinalHeader32.CheckSum = binReader.ReadUInt32();
                    this.optinalHeader32.Subsystem = binReader.ReadUInt16();
                    this.optinalHeader32.DllCharacteristics = binReader.ReadUInt16();
                    this.optinalHeader32.SizeOfStackReserve = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfStackCommit = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfHeapReserve = binReader.ReadUInt32();
                    this.optinalHeader32.SizeOfHeapCommit = binReader.ReadUInt32();
                    this.optinalHeader32.LoaderFlags = binReader.ReadUInt32();
                    this.optinalHeader32.NumberOfRvaAndSizes = binReader.ReadUInt32();
                }

                // 对64位应用程序读取 IMAGE_OPTIONAL_HEADER64 字段.
                else
                {
                    this.Is32bitImage = false;

                    this.optinalHeader64.Magic = magic;
                    this.optinalHeader64.MajorLinkerVersion = binReader.ReadByte();
                    this.optinalHeader64.MinorImageVersion = binReader.ReadByte();
                    this.optinalHeader64.SizeOfCode = binReader.ReadUInt32();
                    this.optinalHeader64.SizeOfInitializedData = binReader.ReadUInt32();
                    this.optinalHeader64.SizeOfUninitializedData = binReader.ReadUInt32();
                    this.optinalHeader64.AddressOfEntryPoint = binReader.ReadUInt32();
                    this.optinalHeader64.BaseOfCode = binReader.ReadUInt32();
                    this.optinalHeader64.ImageBase = binReader.ReadUInt64();
                    this.optinalHeader64.SectionAlignment = binReader.ReadUInt32();
                    this.optinalHeader64.FileAlignment = binReader.ReadUInt32();
                    this.optinalHeader64.MajorOperatingSystemVersion = binReader.ReadUInt16();
                    this.optinalHeader64.MinorOperatingSystemVersion = binReader.ReadUInt16();
                    this.optinalHeader64.MajorImageVersion = binReader.ReadUInt16();
                    this.optinalHeader64.MinorImageVersion = binReader.ReadUInt16();
                    this.optinalHeader64.MajorSubsystemVersion = binReader.ReadUInt16();
                    this.optinalHeader64.MinorSubsystemVersion = binReader.ReadUInt16();
                    this.optinalHeader64.Win32VersionValue = binReader.ReadUInt32();
                    this.optinalHeader64.SizeOfImage = binReader.ReadUInt32();
                    this.optinalHeader64.SizeOfHeaders = binReader.ReadUInt32();
                    this.optinalHeader64.CheckSum = binReader.ReadUInt32();
                    this.optinalHeader64.Subsystem = binReader.ReadUInt16();
                    this.optinalHeader64.DllCharacteristics = binReader.ReadUInt16();
                    this.optinalHeader64.SizeOfStackReserve = binReader.ReadUInt64();
                    this.optinalHeader64.SizeOfStackCommit = binReader.ReadUInt64();
                    this.optinalHeader64.SizeOfHeapReserve = binReader.ReadUInt64();
                    this.optinalHeader64.SizeOfHeapCommit = binReader.ReadUInt64();
                    this.optinalHeader64.LoaderFlags = binReader.ReadUInt32();
                    this.optinalHeader64.NumberOfRvaAndSizes = binReader.ReadUInt32();
                }

                // 读取数据目录.
                this.directoryValues = new IMAGE.IMAGE_DATA_DIRECTORY_Values();
                for (int i = 0; i < 16; i++)
                {
                    directoryValues.Values[i].VirtualAddress = binReader.ReadUInt32();
                    directoryValues.Values[i].Size = binReader.ReadUInt32();
                }
            }
            finally
            {

                // 释放IO资源.
                if (binReader != null)
                {
                    binReader.Close();
                }

                if (fs != null)
                {
                    fs.Close();
                }

            }
        }

        /// <summary>
        /// 获取.Net应用程序全名.
        /// </summary>
        public string GetFullDisplayName()
        {
            if (!IsDotNetAssembly)
            {
                return ExeFilePath;
            }

            // 获取 IReferenceIdentity 接口.
            Fusion.IReferenceIdentity referenceIdentity =
               Fusion.NativeMethods.GetAssemblyIdentityFromFile(ExeFilePath,
               ref Fusion.NativeMethods.ReferenceIdentityGuid) as Fusion.IReferenceIdentity;
            Fusion.IIdentityAuthority IdentityAuthority = Fusion.NativeMethods.GetIdentityAuthority();  
      
            string fullName = IdentityAuthority.ReferenceToText(0, referenceIdentity);
            return fullName;
        }

        public Dictionary<string,string> GetAttributes()
        {
            if (!IsDotNetAssembly)
            {
                return null;
            }

            var attributeDictionary = new Dictionary<string, string>();

            // 获取 IReferenceIdentity 接口.
            Fusion.IReferenceIdentity referenceIdentity =
               Fusion.NativeMethods.GetAssemblyIdentityFromFile(ExeFilePath,
               ref Fusion.NativeMethods.ReferenceIdentityGuid) as Fusion.IReferenceIdentity;

            var enumAttributes = referenceIdentity.EnumAttributes();
            Fusion.IDENTITY_ATTRIBUTE[] IDENTITY_ATTRIBUTEs=new Fusion.IDENTITY_ATTRIBUTE[1024];

            enumAttributes.Next(1024, IDENTITY_ATTRIBUTEs);

            foreach (var IDENTITY_ATTRIBUTE in IDENTITY_ATTRIBUTEs)
            {
                if (!string.IsNullOrEmpty(IDENTITY_ATTRIBUTE.Name))
                {
                    attributeDictionary.Add(IDENTITY_ATTRIBUTE.Name, IDENTITY_ATTRIBUTE.Value);
                }
            }

            return attributeDictionary;
        }

        /// <summary>
        /// 获取程序集最初的.net Framework 编译版本
        /// (其存放在 metadata里), 需指定文件路径. 
        /// </summary>
        public string GetCompiledRuntimeVersion()
        {
            if (!IsDotNetAssembly)
            {
                return ExeFilePath;
            }

            object metahostInterface=null;
            Hosting.NativeMethods.CLRCreateInstance(
                ref Hosting.NativeMethods.CLSID_CLRMetaHost,
                ref Hosting.NativeMethods.IID_ICLRMetaHost, 
                out metahostInterface);

            if (metahostInterface == null || !(metahostInterface is Hosting.IClrMetaHost))
            {
                throw new ApplicationException("不能获取到IClrMetaHost 接口.");
            }

            Hosting.IClrMetaHost ClrMetaHost = metahostInterface as Hosting.IClrMetaHost;
            StringBuilder buffer=new StringBuilder(1024);
            uint bufferLength=1024;          
            ClrMetaHost.GetVersionFromFile(this.ExeFilePath, buffer, ref bufferLength);
            string runtimeVersion = buffer.ToString();

            return runtimeVersion;
        }      
    }
}
