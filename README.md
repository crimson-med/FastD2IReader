# FastD2IReader

Having reversed engineered the .d2i files from Dofus for fun, I decided to make a simple reader for them.

This is based on the **2.10** version.

# How to use

```vb
        Dim MyReader As New FastReader("MyFile.d2i", True)
        MyReader.GetText(41903)
        MyReader.Dispose()
```

# Normal Load **VS** Fast Load

Normal Load:

![Slow Load](/Screens/slowload.PNG)

![Slow Ram](/Screens/slowram.PNG)

Fast Load:

![Fast Load](/Screens/fastload.PNG)

![Fast Ram](/Screens/fastram.PNG)