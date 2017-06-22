# FastD2IReader

Having reversed engineered the .d2i files from Dofus for fun, I decided to make a simple reader for them.

This is based on the **2.10** version.

# How to use

```vb
        Dim MyReader As New FastReader("MyFile.d2i", True)
        MyReader.GetText(41903)
        MyReader.Dispose()
```

# Normal Load VS Fast Load

**Normal Load:**

![Slow Load](/Screens/sload.PNG)

![Slow Ram](/Screens/sram.PNG)

**Fast Load:**

![Fast Load](/Screens/fload.PNG)

![Fast Ram](/Screens/fram.PNG)

# Format D2I 2.10

![Format D2I](/Screens/2-10-d2i.PNG)
