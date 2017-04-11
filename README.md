# transform-swf-net-core
.NET Core port of [Flagstone Transform SWF](https://github.com/StuartMacKay/transform-swf) Java library for manipulating Adobe Flash swf files

Depends on [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp) and [Iconic.Zlib.Netstandard](https://github.com/HelloKitty/Iconic.Zlib.Netstandard)

The package is available on NuGet https://www.nuget.org/packages/SwfTransform/0.0.1-alpha (Pre-release since ImageSharp is pre-release at the time of publishing)

For original documentation see http://www.flagstonesoftware.com/transform/ and http://www.flagstonesoftware.com/cookbook/index.html

Quick example - replace an image in SWF file:

```csharp
using com.flagstone.transform;
using com.flagstone.transform.image;
using com.flagstone.transform.util.image;

///...

var movie = new Movie();
movie.decodeFromFile(new FileInfo("orig.swf")); //read swf from file

//print all objects in swf file
foreach (var obj in movie.Objects)
{
  Debug.WriteLine(obj.ToString());
}

//find image in SWF file (there can be several types - DefineImage, DefineImage2, DefineJpegImage, etc
var image = movie.Objects.OfType<DefineImage>().FirstOrDefault();
if (image != null)
{
  var imgBytes = imageStream.ToArray(); //in this particular example we have RAW image stream in RGBA format
  PNGDecoder.applyAlpha(imgBytes);
  var compressed = PNGDecoder.zip(imgBytes);
  var newImage = new DefineImage(image.Identifier, (int)size.Width, (int)size.Height, compressed, 24); //create SWF image directly
  movie.Objects.Insert(movie.Objects.IndexOf(image), newImage);
  movie.Objects.Remove(image);
}

movie.encodeToFile(new FileInfo("modified.swf"));
```
