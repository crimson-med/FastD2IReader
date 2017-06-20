Imports System.IO
Imports System.Text
Imports System.Linq

Public Class FastReader : Implements IDisposable
    'Gloabal Variables
    Private MyD2I As New FastD2I
    Private Stream As Stream
    Private Br As BinaryReader
    Private IsFastLoad As Boolean

    'Handle new instance creation
    Public Sub New(ByVal D2IPath As String, Optional ByVal FastLoad As Boolean = False)
        'Assign fastload information so can be reused in GetText
        IsFastLoad = FastLoad
        '
        'To implement:
        '/!\ for fast load shouldn't load file to variable but read from position only. /!\
        '
        'Load all file data to variable
        Dim MyData() As Byte = File.ReadAllBytes(D2IPath)
        'Initiate stream from the data 
        Stream = New MemoryStream(MyData)
        'Create the binary stream
        Br = New BinaryReader(Stream)
        'Chheck how to load all the data
        If IsFastLoad = False Then
            LoadD2i()
        Else
            FastLoadD2I()
        End If
    End Sub

    Private Sub FastLoadD2I()
        'Get the total size
        MyD2I.SizeOfD2I = Br.BaseStream.Length
        'Get the data size
        MyD2I.SizeOfData = ReadInt()
        'Jump the data
        Br.BaseStream.Position = MyD2I.SizeOfData
        'Get indexes size
        MyD2I.SizeOfIndex = ReadInt()
        'Load only indexes
        While Br.BaseStream.Position - MyD2I.SizeOfData < MyD2I.SizeOfIndex
            'Store the current read index
            Dim temp As New Index
            temp.IStrKey = ReadInt()
            temp.IStrIndex = ReadInt()
            'Add it to the list
            MyD2I.IndexList.Add(temp)
        End While
    End Sub

    Private Sub LoadD2i()
        'Get the total size
        MyD2I.SizeOfD2I = Br.BaseStream.Length
        'Get the data size
        MyD2I.SizeOfData = ReadInt()
        'Load the data
        While Br.BaseStream.Position < MyD2I.SizeOfData
            'Store the current data
            Dim temp As New DataD2I
            temp.StrIndex = Br.BaseStream.Position
            temp.StrSize = ReadShort()
            temp.Str = ReadUtf8(temp.StrSize)
            'Add the data to the list
            MyD2I.DataList.Add(temp)
        End While
        'Get indexes size
        MyD2I.SizeOfIndex = ReadInt()
        'Load indexes
        While Br.BaseStream.Position - MyD2I.SizeOfData < MyD2I.SizeOfIndex
            'Store the current read index
            Dim temp As New Index
            temp.IStrKey = ReadInt()
            temp.IStrIndex = ReadInt()
            'Add it to the list
            MyD2I.IndexList.Add(temp)
        End While
        'Get the ending data "Relic"
        Dim inter As Integer = MyD2I.SizeOfD2I - MyD2I.SizeOfData - MyD2I.SizeOfIndex - 4
        'Load ending data
        While Br.BaseStream.Position < MyD2I.SizeOfD2I - 4
            'Store the current read relic
            Dim temp As New Relics
            temp.StrIndex = Br.BaseStream.Position
            temp.StrSize = ReadShort()
            temp.Str = ReadUtf8(temp.StrSize)
            temp.StrID = ReadInt()
            'Add the relic to the list
            MyD2I.RestOfFile.Add(temp)
        End While
    End Sub

    Public Function GetText(ByVal MyId As Integer) As DataD2I
        'Declare result variable
        Dim Result As New DataD2I
        'Check which get method to use
        If IsFastLoad = False Then
            'Get the pointer
            Dim pointer As Integer = MyD2I.IndexList.Where(Function(n) n.IStrKey = MyId).First().IStrIndex
            'Get the result from the pointer
            Result = MyD2I.DataList.Where(Function(m) m.StrIndex = pointer).First()
        Else
            'Get the pointer
            Dim pointer As Integer = MyD2I.IndexList.Where(Function(n) n.IStrKey = MyId).First().IStrIndex
            'Place the position
            Br.BaseStream.Position = pointer
            'Read the value
            Result.StrIndex = pointer
            Result.StrSize = ReadShort()
            Result.Str = ReadUtf8(Result.StrSize)
        End If
        'Return the result
        Return Result
    End Function

    'Read 4 bytes to integer 32 (reversed for endian)
    Private Function ReadInt() As Integer
        Dim int32 As Byte() = Br.ReadBytes(4)
        int32 = int32.Reverse().ToArray()
        Return BitConverter.ToInt32(int32, 0)
    End Function

    'Read 2 bytes to integer 16 (reversed for endian)
    Private Function ReadShort() As Short
        Dim MyShort As Byte() = Br.ReadBytes(2)
        MyShort = MyShort.Reverse().ToArray()
        Return BitConverter.ToUInt16(MyShort, 0)
    End Function

    'Read X bytes to UTF-8
    Private Function ReadUtf8(ByVal MySize As Short) As String
        Dim buffer As Byte()
        buffer = Br.ReadBytes(MySize)
        Return Encoding.UTF8.GetString(buffer)
    End Function


#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If
            'Dispose reader on class dispose
            Br.Dispose()
            'Dispose stream on class dispose
            Stream.Dispose()
            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
