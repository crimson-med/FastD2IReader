# FastD2OReader

Having reversed engineered the .d2o files from Dofus for fun, I decided to make a simple reader for them.

This is based on the **2.10** version.

# How to use

```vb
        Dim MyReader As New FastReader("MyFile.d2i", True)
        MyReader.GetText(41903)
        MyReader.Dispose()
```
