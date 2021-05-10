# FastD2IReader

Having reversed engineered the .d2i files from Dofus for fun, I decided to make a simple reader for them.

This is based on the **2.10** version.

There is now a TypeScript Version available: https://github.com/crimson-med/d2i-reader

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

# Fichiers D2I

---

## Introduction 

Le format D2I est un format utilisé par Ankama pour stocker des chaînes de caractères \(string\) comme par exemple les noms d’items ou dialogues et plus. Ce fichier varie en fonction de la langue mais la structure reste la même.

## La Structure

###     _Le Fichier_

Le fichier est lui composé en 4 majeures parties:

* Les Datas
* Les Indexes
* Les UI Messages
* de l'extra data

Chacune des ces parties sont composées d'un Index \(**4 bytes**\) donnant la taille des données qui suivent hormis l'extra data.

###     _Les Datas_

Les datas sont eux composés de 3 parties:

* Taille de tous les datas \(**4 bytes**\)
* Taille de la chaîne de caractères \(**2 bytes orange**\)
* La chaîne de caractères en UTF-8 \(**X bytes gris**\)

![](/Screens/data.PNG)

###     _Les indexes_

Les Indexes eux depuis la mise à jour 2.4X sont plus complexe avec l'introduction des diacritiques \( la chaîne de caractères sans accents ou majuscules\).

* Taille de tous les indexes \(**4 bytes**\)
* l'ID de la chaîne ; généralement appelé dans les d2o \(**4 bytes orange**\)
* Diacritique existant? \(**boolean**\)\(**1 byte bleu clair**\)
* Pointeur vers la chaîne \(**4 bytes marron**\)
* Si diacritique existe Pointeur vers la chaîne diacritique \(**4 bytes bleu marine**\)

![](/Screens/indexes.PNG)

###     _Les UI messages_

Les UI messages sont des messages qui sont donnés dans certain paquets mais qui ne permette pas d'avoir un ID \(integer qui pointe vers le texte\)

**Example:** ui.message.check0



###     _L'extra data_

N'ayant pas vraiment eu le temps de me pencher dessus je ne sais pas son contenue ou son utilité pour l'instant.

###     _Schéma_

![](/Screens/total.PNG)



### 



