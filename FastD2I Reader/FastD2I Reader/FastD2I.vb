'Main holder of the data
Public Class FastD2I
    'Size of the whole file
    Public Property SizeOfD2I As Long
    'Size of main data
    Public Property SizeOfData As UInt32
    'List of the data
    Public Property DataList As New List(Of DataD2I)
    'Size of the indexes
    Public Property SizeOfIndex As UInt32
    'List of the indexes
    Public Property IndexList As New List(Of Index)
End Class

Public Class DataD2I
    'Index of the String
    Public Property StrIndex As UInt32
    'Size of the String
    Public Property StrSize As UInt16
    'The String itself
    Public Property Str As String
End Class

Public Class Index
    'The Key of the string to find
    Public Property IStrKey As UInt32
    'The pointer to the string in the file
    Public Property IStrIndex As UInt32
    'Store if Diacritical exists
    Public Property IDiaExist As Boolean = False
    'Store its pointer if exists
    Public Property IDiaIndex As UInt32 = 0
End Class

