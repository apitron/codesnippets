/* How to improve readability of Tiff.
First of all you can increase the DPI value. Say to 300.

And also, for the special cases new property WhiteColorTolerance has been added. This setting defines which colors will be considered white by comparing their brightness with the value specified. So all colors with brightness greater than defined value will be white and others black.
*/
using (FileStream fs = new FileStream("\\Documents\\fw4.pdf", FileMode.Open))
{
    // this objects represents a PDF document
    Document document = new Document(fs);
 
    // save to tiff using CCIT4 compression, black and white tiff.
    // set the DPI to 110.0 for this sample.
    TiffRenderingSettings tiffRenderingSettings = new TiffRenderingSettings(TiffCompressionMethod.CCIT4, 110, 110);
    tiffRenderingSettings.WhiteColorTolerance = (float) 0.9;
    document.SaveToTiff(File.Create("out.tiff"), tiffRenderingSettings);
     
    System.Diagnostics.Process.Start("out.tiff");
}