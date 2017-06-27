'--+------------------------------------+--
'  |                                    |
'  |           The Falcon               |
'  |                                    |
'--+------------------------------------+--
'Main holder of the data
Public Class FastD2I
    'Size of the whole file
    Public Property SizeOfD2I As Long
    'Size of main data
    Public Property SizeOfData As UInt32
    'List of the data
    Public Property DataList As New Dictionary(Of Integer, DataD2I)
    Public Property IDList As New List(Of Integer)
    'Size of the indexes
    Public Property SizeOfIndex As UInt32
    'Size of UI Index
    Public Property SizeOfUi As UInt32
End Class

Public Class DataD2I
    Public Property HasDia As Boolean
    Public Property Str As String = ""
    Public Property StrDia As String = ""
    Function getval(ByVal GetDia As Boolean) As String
        If HasDia = True And GetDia = True Then
            Return StrDia
        Else
            Return Str
        End If
    End Function
End Class

Public Class UI
    'The Key of the string to find
    Public Property IStrKey As UInt32
    'The pointer to the string in the file
    Public Property IStrIndex As UInt32
    'Store if Diacritical exists
    Public Property IDiaExist As Boolean = False
    'Store its pointer if exists
    Public Property IDiaIndex As UInt32 = 0
End Class

