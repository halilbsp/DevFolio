using System.IO;
using System.IO.Compression;

var memStream = new MemoryStream();
using (var archive = new ZipArchive(memStream, ZipArchiveMode.Update, true))
{
    var zipEntryName = "assets/82bf5a48-c7e6-4cf1-93ca-6fc294f2f4db.jpg";
    if (archive.GetEntry(zipEntryName) == null)
    {
        archive.CreateEntryFromFile(@"C:\Users\HalilBaspi\Desktop\DevFolio\DevFolio.Web\wwwroot\uploads\82bf5a48-c7e6-4cf1-93ca-6fc294f2f4db.jpg", zipEntryName);
    }
}
File.WriteAllBytes("test.zip", memStream.ToArray());
