// How to convert a part of the PDF page into image (partial page rendering).
// open and load the file
using (FileStream fs = new FileStream(@"testfile.pdf", FileMode.Open))
{
    // this object represents a PDF document
    Document document = new Document(fs);            
 
    // process and save pages one by one
    for (int i = 0; i < document.Pages.Count; i++)
    {
        Page currentPage = document.Pages[i];
 
        // we'd like to convert half of original PDF page
        Rectangle rectangle = new Rectangle(0, currentPage.Width/2, (int)currentPage.Width, (int)currentPage.Height);
 
        // we use original page's width and height for image
        using (Bitmap bitmap = currentPage.Render((int)currentPage.Width, (int)currentPage.Height, rectangle, new RenderingSettings()))
        {
            bitmap.Save(string.Format("{0}.png", i), ImageFormat.Png);
        }
    }
 
    // preview first rendered page
    Process.Start("0.png");
}