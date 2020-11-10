// How to create and save PDF document ( Apitron.PDF.Kit.FixedLayout ).
// open and load the file
using (FileStream fs = new FileStream(out_path, FileMode.Create))
{
    // this object represents a PDF fixed document.
    FixedDocument document = new FixedDocument();
     
    // add page
    document.Pages.Add(new Page());
 
    // add some text content
    TextObject text = new TextObject("Helvetica", 38);
    text.Translate(10, 550);
    text.SetTextRenderingMode(RenderingMode.FillText);
    text.AppendText("Very simple PDF creation process");
 
    // register some image content
    FixedLayout.Resources.XObjects.Image image = new FixedLayout.Resources.XObjects.Image("Image1", @"image.jpg");
    document.ResourceManager.RegisterResource(image);
 
    // append text and image
    document.Pages[0].Content.AppendText(text);
    document.Pages[0].Content.AppendImage("Image1", 10, 50, Boundaries.A4.Width-20, image.Height);
     
    // save document
    document.Save(fs);
}