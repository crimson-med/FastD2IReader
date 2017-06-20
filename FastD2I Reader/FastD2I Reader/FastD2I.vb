'Main holder of the data
Public Class FastD2I
    'Size of the whole file
    Public Property SizeOfD2I As Long
    'Size of main data
    Public Property SizeOfData As Int32
    'List of the data
    Public Property DataList As New List(Of DataD2I)
    'Size of the indexes
    Public Property SizeOfIndex As Int32
    'List of the indexes
    Public Property IndexList As New List(Of Index)
    'List of the rest of the data
    Public Property RestOfFile As New List(Of Relics)
End Class

Public Class DataD2I
    'Index of the String
    Public Property StrIndex As Int32
    'Size of the String
    Public Property StrSize As Int16
    'The String itself
    Public Property Str As String
End Class

Public Class Index
    'The Key of the string to find
    Public Property IStrKey As Int32
    'The pointer to the string in the file
    Public Property IStrIndex As Int32
End Class

'Handles the extra d2i content at the end of the file
Public Class Relics
    'Index of the String
    Public Property StrIndex As Int32
    'Size of the String
    Public Property StrSize As Int16
    'The String itself
    Public Property Str As String
    'String ID
    Public Property StrID As Int32
End Class

