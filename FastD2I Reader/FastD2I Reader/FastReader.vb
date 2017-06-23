Imports System.IO
Imports System.Text
Imports System.Linq

'--+------------------------------------+--
'  |                                    |
'  |           The Falcon               |
'  |                                    |
'--+------------------------------------+--

Public Class FastReader : Implements IDisposable
    'Gloabal Variables
    Private MyD2I As New FastD2I
    Private Stream As Stream
    Private Br As BinaryReader
    Private IsFastLoad As Boolean
    Private Pather As String

    ''' <summary>
    ''' Handles the creation of new reader
    ''' </summary>
    ''' <param name="D2IPath">Path to the .d2i file</param>
    ''' <param name="FastLoad">Enable the fast load, by default it's set to True.</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal D2IPath As String, Optional ByVal FastLoad As Boolean = True)
        'Assign fastload information so can be reused in GetText
        IsFastLoad = FastLoad
        'Assign path information so can be reused in GetText
        Pather = D2IPath
        'Slow loading if needed
        If IsFastLoad = False Then
            ' Dim MyData() As Byte = File.ReadAllBytes(D2IPath)
            ' Stream = New MemoryStream(MyData)
            ' Br = New BinaryReader(Stream)
            LoadD2i()
        End If
    End Sub

    Private Sub LoadD2i()
        Br = New BinaryReader(File.Open(Pather, FileMode.Open, FileAccess.Read))
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
            'Check if Dia exists
            temp.IDiaExist = ReadBool()
            temp.IStrIndex = ReadInt()
            If temp.IDiaExist = True Then
                temp.IDiaIndex = ReadInt()
            End If
            'Add it to the list
            MyD2I.IndexList.Add(temp)
        End While
        Br.Dispose()
        GC.Collect()
    End Sub

    ''' <summary>
    ''' Fetch the text associated with the ID
    ''' </summary>
    ''' <param name="ToSearch">ID of the text to get</param>
    ''' <param name="VersionDiacritique">Choose if you prefer the diacritical value or not, by default it's set to True.</param>
    ''' <remarks></remarks>
    Public Function GetText(Of T)(ByVal ToSearch As T, Optional ByVal VersionDiacritique As Boolean = False) As String
        Dim MyId As UInt32
        Dim result As New DataD2I
        result.Str = ""
        If GetType(T) = GetType(String) Then
            MyId = Convert.ToUInt32(ToSearch)
        ElseIf IsNumeric(ToSearch) Then
            MyId = Val(ToSearch)
        End If
        If IsFastLoad = False Then
            Try
                If VersionDiacritique = True Then
                    Dim pointer As UInt32
                    Try
                        pointer = MyD2I.IndexList.Where(Function(n) n.IStrKey = MyId And n.IDiaExist = True).First().IDiaIndex
                    Catch ex As Exception
                        pointer = MyD2I.IndexList.Where(Function(n) n.IStrKey = MyId).First().IStrIndex
                    End Try

                    result.Str = MyD2I.DataList.Where(Function(m) m.StrIndex = pointer).First().Str
                Else
                    Dim pointer As UInt32 = MyD2I.IndexList.Where(Function(n) n.IStrKey = MyId).First().IStrIndex
                    result.Str = MyD2I.DataList.Where(Function(m) m.StrIndex = pointer).First().Str
                End If
            Catch ex As Exception

            End Try

        Else
            Br = New BinaryReader(File.Open(Pather, FileMode.Open, FileAccess.Read))
            MyD2I.SizeOfD2I = Br.BaseStream.Length
            MyD2I.SizeOfData = ReadInt()
            Br.BaseStream.Position = MyD2I.SizeOfData
            MyD2I.SizeOfIndex = ReadInt()
            Try
                While Br.BaseStream.Position - MyD2I.SizeOfData < MyD2I.SizeOfIndex
                    Dim temp As New Index
                    Dim temp2 As UInt32 = ReadInt()
                    If temp2 = MyId Then
                        temp.IStrKey = temp2
                        temp.IDiaExist = ReadBool()
                        temp.IStrIndex = ReadInt()
                        If temp.IDiaExist = True Then
                            temp.IDiaIndex = ReadInt()
                        Else
                        End If
                        Dim pointer As Integer
                        If VersionDiacritique = True Then
                            pointer = temp.IDiaIndex
                        Else
                            pointer = temp.IStrIndex
                        End If
                        Br.BaseStream.Position = pointer
                        result.StrIndex = pointer
                        result.StrSize = ReadShort()
                        result.Str = ReadUtf8(result.StrSize)
                        Exit While
                    Else
                        temp.IDiaExist = ReadBool()
                        temp.IStrIndex = ReadInt()
                        If temp.IDiaExist = True Then
                            temp.IDiaIndex = ReadInt()
                        Else
                        End If
                    End If
                End While
            Catch ex As Exception
            End Try
        End If
        'Return the result
        Return result.Str
    End Function

    'Read 4 bytes to UInteger 32 (reversed for endian)
    Private Function ReadInt() As UInt32
        Dim int32 As Byte() = Br.ReadBytes(4)
        int32 = int32.Reverse().ToArray()
        Return BitConverter.ToUInt32(int32, 0)
    End Function

    'Read 2 bytes to UInteger 16 (reversed for endian)
    Private Function ReadShort() As UInt16
        Dim MyShort As Byte() = Br.ReadBytes(2)
        MyShort = MyShort.Reverse().ToArray()
        Return BitConverter.ToUInt16(MyShort, 0)
    End Function

    'Read 1 byte for Boolean
    Private Function ReadBool() As Boolean
        Dim result As Byte = Br.ReadBoolean
        Return result
    End Function

    'Read X bytes to UTF-8
    Private Function ReadUtf8(ByVal MySize As UInt16) As String
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
            Try
                Br.Dispose()
            Catch ex As Exception

            End Try
            'Dispose stream on class dispose
            Try
                Stream.Dispose()
            Catch ex As Exception

            End Try
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
