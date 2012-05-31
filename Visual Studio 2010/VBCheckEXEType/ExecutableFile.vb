'*************************** 模块头 ******************************'
' 模块名:  ExecutableFile.vb
' 项目名:	    VBCheckEXEType
' 版权 (c) Microsoft Corporation.
' 
' 这个类表示一个可执行文件. 它可以这个映像文件中获取这个映像文件头部,
' 可选映像文件头部 和数据目录. 根据这些头部, 
' 我们能知道这是否是一个控制台应用程序, 
' 是否是一个.NET应用程序以及是否是32位Native应用程序. 
' 对于.NET应用程序, 它能生成全名，如 
'  System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, processorArchitecture=MSIL
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.IO
Imports System.Text

Public Class ExecutableFile
    ''' <summary>
    ''' 可执行文件路径.
    ''' </summary>
    Private _exeFilePath As String
    Public Property ExeFilePath() As String
        Get
            Return _exeFilePath
        End Get
        Private Set(ByVal value As String)
            _exeFilePath = value
        End Set
    End Property

    Private _imageFileHeader As IMAGE.IMAGE_FILE_HEADER

    ''' <summary>
    ''' 映像文件头部.
    ''' </summary>
    Public ReadOnly Property ImageFileHeader() As IMAGE.IMAGE_FILE_HEADER
        Get
            Return _imageFileHeader
        End Get
    End Property

    Private _optinalHeader32 As IMAGE.IMAGE_OPTIONAL_HEADER32

    ''' <summary>
    ''' 针对32位可执行的可选映像头部.
    ''' </summary>
    Public ReadOnly Property OptinalHeader32() As IMAGE.IMAGE_OPTIONAL_HEADER32
        Get
            Return _optinalHeader32
        End Get
    End Property

    Private _optinalHeader64 As IMAGE.IMAGE_OPTIONAL_HEADER64

    ''' <summary>
    ''' 针对64位可执行文件的可选映像头部.
    ''' </summary>
    Public ReadOnly Property OptinalHeader64() As IMAGE.IMAGE_OPTIONAL_HEADER64
        Get
            Return _optinalHeader64
        End Get
    End Property

    Private _directoryValues As IMAGE.IMAGE_DATA_DIRECTORY_Values

    ''' <summary>
    ''' 数据目录.
    ''' </summary>
    Public ReadOnly Property DirectoryValues() As IMAGE.IMAGE_DATA_DIRECTORY_Values
        Get
            Return _directoryValues
        End Get
    End Property

    ''' <summary>
    ''' 表示这是否是一个控制台应用程序. 
    ''' </summary>
    Public ReadOnly Property IsConsoleApplication() As Boolean
        Get
            If Is32bitImage Then
                Return _optinalHeader32.Subsystem = 3
            Else
                Return _optinalHeader64.Subsystem = 3
            End If
        End Get
    End Property

    ''' <summary>
    ''' 表示这是否是一个.NET应用程序. 
    ''' </summary>
    Public ReadOnly Property IsDotNetAssembly() As Boolean
        Get
            Return _directoryValues.Values(IMAGE.IMAGE_DATA_DIRECTORY_Values.IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR).VirtualAddress <> 0
        End Get
    End Property

    ''' <summary>
    ''' 表示这是否是一个32位应用程序. 
    ''' </summary>
    Private _is32bitImage As Boolean
    Public Property Is32bitImage() As Boolean
        Get
            Return _is32bitImage
        End Get
        Private Set(ByVal value As Boolean)
            _is32bitImage = value
        End Set
    End Property



    Public Sub New(ByVal filePath As String)
        If Not File.Exists(filePath) Then
            Throw New ArgumentException("进程未找到.")
        End If

        Me.ExeFilePath = filePath

        Me.ReadHeaders()
    End Sub

    ''' <summary>
    ''' 读取映像文件头部.
    ''' </summary>
    Private Sub ReadHeaders()

        Dim fs As FileStream = Nothing
        Dim binReader As BinaryReader = Nothing

        Try
            fs = New FileStream(ExeFilePath, FileMode.Open, FileAccess.Read)
            binReader = New BinaryReader(fs)

            ' 读取PE头部开始偏移量.
            fs.Position = &H3C
            Dim headerOffset As UInt32 = binReader.ReadUInt32()

            ' 核对偏移量是否有效
            If headerOffset > fs.Length - 5 Then
                Throw New ApplicationException("无效映像格式")
            End If

            ' 读取PE文件签名
            fs.Position = headerOffset
            Dim signature As UInt32 = binReader.ReadUInt32()

            ' 0x00004550 表示字母“PE”，其后有两个终止符0.
            If signature <> &H4550 Then
                Throw New ApplicationException("无效映像格式")
            End If

            ' 读取映像文件头部. 
            Me._imageFileHeader.Machine = binReader.ReadUInt16()
            Me._imageFileHeader.NumberOfSections = binReader.ReadUInt16()
            Me._imageFileHeader.TimeDateStamp = binReader.ReadUInt32()
            Me._imageFileHeader.PointerToSymbolTable = binReader.ReadUInt32()
            Me._imageFileHeader.NumberOfSymbols = binReader.ReadUInt32()
            Me._imageFileHeader.SizeOfOptionalHeader = binReader.ReadUInt16()
            Me._imageFileHeader.Characteristics = CType(binReader.ReadUInt16(), IMAGE.IMAGE_FILE_Flag)

            ' 判断是否是32位程序集.
            Dim magic As UInt16 = binReader.ReadUInt16()
            If magic <> &H10B AndAlso magic <> &H20B Then
                Throw New ApplicationException("无效映像格式")
            End If

            ' 对32应用程序读取 IMAGE_OPTIONAL_HEADER32 字段.
            If magic = &H10B Then
                Me.Is32bitImage = True

                Me._optinalHeader32.Magic = magic
                Me._optinalHeader32.MajorLinkerVersion = binReader.ReadByte()
                Me._optinalHeader32.MinorImageVersion = binReader.ReadByte()
                Me._optinalHeader32.SizeOfCode = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfInitializedData = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfUninitializedData = binReader.ReadUInt32()
                Me._optinalHeader32.AddressOfEntryPoint = binReader.ReadUInt32()
                Me._optinalHeader32.BaseOfCode = binReader.ReadUInt32()
                Me._optinalHeader32.BaseOfData = binReader.ReadUInt32()
                Me._optinalHeader32.ImageBase = binReader.ReadUInt32()
                Me._optinalHeader32.SectionAlignment = binReader.ReadUInt32()
                Me._optinalHeader32.FileAlignment = binReader.ReadUInt32()
                Me._optinalHeader32.MajorOperatingSystemVersion = binReader.ReadUInt16()
                Me._optinalHeader32.MinorOperatingSystemVersion = binReader.ReadUInt16()
                Me._optinalHeader32.MajorImageVersion = binReader.ReadUInt16()
                Me._optinalHeader32.MinorImageVersion = binReader.ReadUInt16()
                Me._optinalHeader32.MajorSubsystemVersion = binReader.ReadUInt16()
                Me._optinalHeader32.MinorSubsystemVersion = binReader.ReadUInt16()
                Me._optinalHeader32.Win32VersionValue = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfImage = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfHeaders = binReader.ReadUInt32()
                Me._optinalHeader32.CheckSum = binReader.ReadUInt32()
                Me._optinalHeader32.Subsystem = binReader.ReadUInt16()
                Me._optinalHeader32.DllCharacteristics = binReader.ReadUInt16()
                Me._optinalHeader32.SizeOfStackReserve = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfStackCommit = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfHeapReserve = binReader.ReadUInt32()
                Me._optinalHeader32.SizeOfHeapCommit = binReader.ReadUInt32()
                Me._optinalHeader32.LoaderFlags = binReader.ReadUInt32()
                Me._optinalHeader32.NumberOfRvaAndSizes = binReader.ReadUInt32()

                ' 对64位应用程序读取 IMAGE_OPTIONAL_HEADER64 字段.
            Else
                Me.Is32bitImage = False

                Me._optinalHeader64.Magic = magic
                Me._optinalHeader64.MajorLinkerVersion = binReader.ReadByte()
                Me._optinalHeader64.MinorImageVersion = binReader.ReadByte()
                Me._optinalHeader64.SizeOfCode = binReader.ReadUInt32()
                Me._optinalHeader64.SizeOfInitializedData = binReader.ReadUInt32()
                Me._optinalHeader64.SizeOfUninitializedData = binReader.ReadUInt32()
                Me._optinalHeader64.AddressOfEntryPoint = binReader.ReadUInt32()
                Me._optinalHeader64.BaseOfCode = binReader.ReadUInt32()
                Me._optinalHeader64.ImageBase = binReader.ReadUInt64()
                Me._optinalHeader64.SectionAlignment = binReader.ReadUInt32()
                Me._optinalHeader64.FileAlignment = binReader.ReadUInt32()
                Me._optinalHeader64.MajorOperatingSystemVersion = binReader.ReadUInt16()
                Me._optinalHeader64.MinorOperatingSystemVersion = binReader.ReadUInt16()
                Me._optinalHeader64.MajorImageVersion = binReader.ReadUInt16()
                Me._optinalHeader64.MinorImageVersion = binReader.ReadUInt16()
                Me._optinalHeader64.MajorSubsystemVersion = binReader.ReadUInt16()
                Me._optinalHeader64.MinorSubsystemVersion = binReader.ReadUInt16()
                Me._optinalHeader64.Win32VersionValue = binReader.ReadUInt32()
                Me._optinalHeader64.SizeOfImage = binReader.ReadUInt32()
                Me._optinalHeader64.SizeOfHeaders = binReader.ReadUInt32()
                Me._optinalHeader64.CheckSum = binReader.ReadUInt32()
                Me._optinalHeader64.Subsystem = binReader.ReadUInt16()
                Me._optinalHeader64.DllCharacteristics = binReader.ReadUInt16()
                Me._optinalHeader64.SizeOfStackReserve = binReader.ReadUInt64()
                Me._optinalHeader64.SizeOfStackCommit = binReader.ReadUInt64()
                Me._optinalHeader64.SizeOfHeapReserve = binReader.ReadUInt64()
                Me._optinalHeader64.SizeOfHeapCommit = binReader.ReadUInt64()
                Me._optinalHeader64.LoaderFlags = binReader.ReadUInt32()
                Me._optinalHeader64.NumberOfRvaAndSizes = binReader.ReadUInt32()
            End If

            ' 读取数据目录.
            Me._directoryValues = New IMAGE.IMAGE_DATA_DIRECTORY_Values()
            For i As Integer = 0 To 15
                _directoryValues.Values(i).VirtualAddress = binReader.ReadUInt32()
                _directoryValues.Values(i).Size = binReader.ReadUInt32()
            Next i
        Finally

            ' 释放IO资源.
            If binReader IsNot Nothing Then
                binReader.Close()
            End If

            If fs IsNot Nothing Then
                fs.Close()
            End If

        End Try
    End Sub

    ''' <summary>
    ''' 获取.Net应用程序全名.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetFullDisplayName() As String
        If Not IsDotNetAssembly Then
            Return ExeFilePath
        End If

        Dim buffer(1024) As Char

        ' 获取 IReferenceIdentity 接口.
        Dim referenceIdentity As Fusion.IReferenceIdentity =
            TryCast(Fusion.NativeMethods.GetAssemblyIdentityFromFile(
                    ExeFilePath, Fusion.NativeMethods.ReferenceIdentityGuid), 
                Fusion.IReferenceIdentity)

        Dim IdentityAuthority As Fusion.IIdentityAuthority =
            Fusion.NativeMethods.GetIdentityAuthority()

        Dim fullName As String =
            IdentityAuthority.ReferenceToText(0, referenceIdentity)

        Return fullName
    End Function


    Public Function GetAttributes() As Dictionary(Of String, String)
        If Not IsDotNetAssembly Then
            Return Nothing
        End If

        Dim attributeDictionary = New Dictionary(Of String, String)()

        ' 获取 IReferenceIdentity 接口.
        Dim referenceIdentity As Fusion.IReferenceIdentity =
            TryCast(Fusion.NativeMethods.GetAssemblyIdentityFromFile(ExeFilePath, Fusion.NativeMethods.ReferenceIdentityGuid), Fusion.IReferenceIdentity)

        Dim enumAttributes = referenceIdentity.EnumAttributes()
        Dim IDENTITY_ATTRIBUTEs(1024) As Fusion.IDENTITY_ATTRIBUTE

        enumAttributes.Next(1024, IDENTITY_ATTRIBUTEs)

        For Each IDENTITY_ATTRIBUTE In IDENTITY_ATTRIBUTEs
            If Not String.IsNullOrEmpty(IDENTITY_ATTRIBUTE.Name) Then
                attributeDictionary.Add(IDENTITY_ATTRIBUTE.Name,
                                        IDENTITY_ATTRIBUTE.Value)
            End If
        Next IDENTITY_ATTRIBUTE

        Return attributeDictionary
    End Function

    ''' <summary>
    ''' 获取程序集最初的.net Framework 编译版本
    ''' (其存放在 metadata里), 需指定文件路径.  
    ''' </summary>
    Public Function GetCompiledRuntimeVersion() As String
        Dim metahostInterface As Object = Nothing
        Hosting.NativeMethods.CLRCreateInstance(Hosting.NativeMethods.CLSID_CLRMetaHost,
                                                Hosting.NativeMethods.IID_ICLRMetaHost, metahostInterface)

        If metahostInterface Is Nothing OrElse Not (TypeOf metahostInterface Is Hosting.IClrMetaHost) Then
            Throw New ApplicationException("不能获取到IClrMetaHost 接口.")
        End If

        Dim ClrMetaHost As Hosting.IClrMetaHost = TryCast(metahostInterface, Hosting.IClrMetaHost)
        Dim buffer As New StringBuilder(1024)
        Dim bufferLength As UInteger = 1024
        ClrMetaHost.GetVersionFromFile(Me.ExeFilePath, buffer, bufferLength)
        Dim runtimeVersion As String = buffer.ToString()

        Return runtimeVersion
    End Function

End Class
