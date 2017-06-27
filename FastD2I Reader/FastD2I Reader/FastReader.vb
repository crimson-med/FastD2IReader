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
    ''' Handles the creation of new reader. Check the documentation if not sure how to use.
    ''' </summary>
    ''' <param name="D2IPath">Path to the .d2i file</param>
    ''' <param name="FastLoad">Enable the fast load, by default it's set to True.</param>
    ''' <remarks>Test</remarks>
    Public Sub New(ByVal D2IPath As String, Optional ByVal FastLoad As Boolean = True)
        'Assign fastload information so can be reused in GetText
        IsFastLoad = FastLoad
        'Assign path information so can be reused in GetText
        Pather = D2IPath
        Br = New BinaryReader(File.Open(Pather, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        MyD2I.SizeOfD2I = Br.BaseStream.Length
        MyD2I.SizeOfData = ReadInt()
        Br.BaseStream.Position = MyD2I.SizeOfData
        MyD2I.SizeOfIndex = ReadInt()
        Br.BaseStream.Position = Br.BaseStream.Position + MyD2I.SizeOfIndex
        MyD2I.SizeOfUi = ReadInt()
        'Slow loading if needed
        If IsFastLoad = False Then
            LoadD2i()
        End If
    End Sub

#Region "Pre-Load Data"
    Private Sub LoadD2i()
        'Dim timerload As New Stopwatch
        'timerload.Start()
        Br.BaseStream.Position = MyD2I.SizeOfData + 4
        While Br.BaseStream.Position - MyD2I.SizeOfData < MyD2I.SizeOfIndex
            Dim OldPos As Integer
            Dim temp As New DataD2I
            Dim StrID As UInt32 = ReadInt()
            temp.HasDia = ReadBool()
            Dim Pointer As UInt32 = ReadInt()
            Dim DPointer As UInt32 = 0
            OldPos = Br.BaseStream.Position
            If temp.HasDia = True Then
                DPointer = ReadInt()
                OldPos = Br.BaseStream.Position
                Br.BaseStream.Position = DPointer
                temp.StrDia = ReadUtf8(ReadShort)
            End If
            Br.BaseStream.Position = Pointer
            temp.Str = ReadUtf8(ReadShort)
            MyD2I.DataList.Add(StrID, temp)
            MyD2I.IDList.Add(StrID)
            Br.BaseStream.Position = OldPos
        End While
        'timerload.Stop()
        'Console.WriteLine("Loaded Data in: " & timerload.ElapsedMilliseconds & "ms")
        GC.Collect()
    End Sub
#End Region

#Region "Get Text From ID"

    ''' <summary>
    ''' Fetch the text associated with the ID
    ''' </summary>
    ''' <param name="ToSearch">ID of the text to get</param>
    ''' <param name="VersionDiacritique">Choose if you prefer the diacritical value or not, by default it's set to False.</param>
    ''' <remarks></remarks>
    Public Function GetText(Of T)(ByVal ToSearch As T, Optional ByVal VersionDiacritique As Boolean = False) As String
        Dim MyId As UInt32
        Dim Result As String = "No Result"
        If GetType(T) = GetType(String) Then
            MyId = Convert.ToUInt32(ToSearch)
        ElseIf IsNumeric(ToSearch) Then
            MyId = Val(ToSearch)
        End If
        If IsFastLoad = False Then
            Result = GetSlowText(MyId, VersionDiacritique)
        Else
            Result = GetFastText(MyId, VersionDiacritique)
        End If
        Return Result
    End Function

    Public Function GetSlowText(ByVal ToSearch As UInt32, Optional ByVal MyDia As Boolean = False) As String
        'Dim timerget As New Stopwatch
        'timerget.Start()
        Dim Result As String = ""
        If MyDia = True Then
            Try
                Result = MyD2I.DataList(ToSearch).getval(True)
            Catch ex As Exception
                Result = "No Result"
            End Try
        Else
            Try
                Result = MyD2I.DataList(ToSearch).getval(False)
            Catch ex As Exception
                Result = "No Result"
            End Try
        End If
        'timerget.Stop()
        'Console.WriteLine("Query: " & ToSearch & "  --  " & Result & "  --  " & "Time: " & timerget.ElapsedMilliseconds & "ms")
        Return Result
    End Function

    Public Function GetFastText(ByVal ToSearch As UInt32, Optional ByVal MyDia As Boolean = False) As String
        'Dim timerget As New Stopwatch
        'timerget.Start()
        Dim Result As String = ""
        Br.BaseStream.Position = MyD2I.SizeOfData + 4
        Try
            While Br.BaseStream.Position - MyD2I.SizeOfData < MyD2I.SizeOfIndex
                Dim temp As New DataD2I
                Dim ReadID As UInt32 = ReadInt()
                'implement cache process for future
                temp.HasDia = ReadBool()
                Dim Pointer As UInt32 = ReadInt()
                Dim DPointer As UInt32
                If temp.HasDia = True Then
                    DPointer = ReadInt()
                End If
                If ToSearch = ReadID Then
                    If temp.HasDia = True And MyDia = True Then
                        Br.BaseStream.Position = DPointer
                        Result = ReadUtf8(ReadShort)
                        Exit While
                    Else
                        Br.BaseStream.Position = Pointer
                        Result = ReadUtf8(ReadShort)
                        Exit While
                    End If
                End If
            End While
        Catch ex As Exception
        End Try
        'timerget.stop()
        'Console.WriteLine("Query: " & ToSearch & "  --  " & Result & "  --  " & "Time: " & timerget.ElapsedMilliseconds & "ms")
        Return Result
    End Function

#End Region

#Region "Get ui.messages"
    Public Function GetUi(ByVal MySearch As String) As String
        'Dim TimeUi As New Stopwatch
        'TimeUi.Start()
        Br.BaseStream.Position = MyD2I.SizeOfData + MyD2I.SizeOfIndex + 8
        Dim UIResult As String = "No Result"
        Try
            While Br.BaseStream.Position < Br.BaseStream.Length
                Dim ReadUI As String = ReadUtf8(ReadShort())
                Dim UIPointer = ReadInt()
                If String.Compare(MySearch, ReadUI, True) = 0 Then
                    Br.BaseStream.Position = UIPointer
                    UIResult = ReadUtf8(ReadShort())
                    Exit While
                End If
            End While
        Catch ex As Exception
        End Try
        'TimeUi.Stop()
        'Console.WriteLine("Query: " & MySearch & "  --  " & UIResult & "  --  " & "Time: " & TimeUi.ElapsedMilliseconds & "ms")
        Return UIResult
    End Function

    'LATER IMPLEMENTATION

    'Public Function GetFastUi(ByVal MySearch As String) As String

    'End Function

    'Public Function GetSlowUi(ByVal MySearch As String) As String

    'End Function
#End Region

#Region "Readers"

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
#End Region

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
