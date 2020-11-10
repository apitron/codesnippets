//How to convert PDF document WinRT (Windows Phone or Windows Store).
private async void OnRenderClicked(object sender, RoutedEventArgs e)
{          
    // get the assets folder for the application
    StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
 
    // get the file included in application' assets
    StorageFile file = await folder.GetFileAsync("testfile.pdf");
 
    // open the file and render first page
    using (Stream stream = await file.OpenStreamForReadAsync())
    {
        Document doc = new Document(stream);
 
        Apitron.PDF.Rasterizer.Page page = doc.Pages[0];
 
        ErrorLogger logger = new ErrorLogger();
         
        WriteableBitmap bm = page.Render((int) page.Width, (int) page.Height, new RenderingSettings(), logger);               
 
        myImage.Source = bm;               
    }
}