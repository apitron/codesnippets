// How to create PDF file with grid on Windows Phone 8.1 Silverlight ( Apitron.PDF.Kit.FlowLayout )
private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
{
    // start progress
    ShowProgress(true);
 
    Apitron.PDF.Kit.FixedLayout.Resources.ResourceManager resourceManager = new Apitron.PDF.Kit.FixedLayout.Resources.ResourceManager();           
 
    // preload logos, each logo is named after the team
    StorageFolder dataPath = await StorageFolder.GetFolderFromPathAsync(Path.Combine(Package.Current.InstalledLocation.Path, "Assets\\data"));                       
 
    foreach (StorageFile logoFile in await  dataPath.GetFilesAsync())
    {
        if (logoFile.FileType.ToLower() == ".png")
        {
            resourceManager.RegisterResource(new Apitron.PDF.Kit.FixedLayout.Resources.XObjects.Image(Path.GetFileNameWithoutExtension(logoFile.Path), logoFile.Path));
        }
    }
 
    // create document and register styles
    FlowDocument document = new FlowDocument() { Margin = new Thickness(18) };
 
    // common style for the grid with results
    document.StyleManager.RegisterStyle("grid#results", new Style() { InnerBorderColor = RgbColors.LightGray,
                                                                      Align = Align.Center,
                                                                      Font = new Font(StandardFonts.Helvetica, 12) });
    // set height for each row of the results grid
    document.StyleManager.RegisterStyle("grid#results > gridrow", new Style() { Height = 16 });
    // set header row color
    document.StyleManager.RegisterStyle("grid#results > gridrow#header", new Style() { Background = RgbColors.LightGray });
    // set header text font
    document.StyleManager.RegisterStyle("textblock.h1", new Style() { Font = new Font(StandardFonts.HelveticaBold, 20) });
    // set winners cell style
    document.StyleManager.RegisterStyle(".Winners", new Style() { Align = Align.Right,
                                                                  BackgroundPosition = BackgroundPosition.LeftCenter,
                                                                  BackgroundRepeat = BackgroundRepeat.NoRepeat,
                                                                  Margin = new Thickness(3, 0, 3, 0) });
    // set runners-up cell style
    document.StyleManager.RegisterStyle(".Runners_up", new Style() { Align = Align.Left,
                                                                     BackgroundPosition = BackgroundPosition.RightCenter,
                                                                     BackgroundRepeat = BackgroundRepeat.NoRepeat,
                                                                     Margin = new Thickness(3, 0, 3, 0) });
 
    // define the grid
    Grid grid = new Grid(Length.FromPercentage(10),
                         Length.FromPercentage(20),
                         Length.FromPercentage(10),
                         Length.FromPercentage(20),
                         Length.FromPercentage(30),
                         Length.FromPercentage(10)) { Id = "results" };
 
    // add header row
    grid.Add(new GridRow(new TextBlock("Season"),
                         new TextBlock("Winners"),
                         new TextBlock("Score"),
                         new TextBlock("Runners-up"),
                         new TextBlock("Venue"),
                         new TextBlock("Attendance")) { Id = "header" });
 
    // read cup data from file and fill the grid, each row is represented as string contaning values separated by semicolon
    using (StreamReader reader = File.OpenText(Path.Combine(dataPath.Path, "data.txt")))
    {
        string rowData = null;
 
        while (!string.IsNullOrEmpty(rowData = reader.ReadLine()))
        {
            string[] cells = rowData.Split(';');
 
            GridRow newRow = new GridRow();
 
            for (int i = 0; i < cells.Length; i++)
            {
                TextBlock cell = new TextBlock(cells[i]);
 
                // for cols 1 and 3 we set background image by using team name as resource id for the image,
                // this image will be positioned according to the style that matches cell's class
                if (i == 1)
                {
                    cell.BackgroundImage = new BackgroundImage(cells[i]);
                    cell.Class = "Winners";
                }
                else if (i == 3)
                {
                    cell.BackgroundImage = new BackgroundImage(cells[i]);
                    cell.Class = "Runners_up";
                }
 
                newRow.Add(cell);
            }
 
            grid.Add(newRow);
        }
    }
 
    // add header and grid into the document
    document.Add(new TextBlock("European Cup Winners") { Class = "h1" });
    document.Add(grid);          
          
    // save to file
    StorageFile outputFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("grid.pdf", CreationCollisionOption.ReplaceExisting);
 
    using (Stream outputStream = await  outputFile.OpenStreamForWriteAsync())
    {
        document.Write(outputStream,resourceManager, new PageBoundary(Boundaries.Ledger));
    }
 
    ShowProgress(false);          
 
    Launcher.LaunchFileAsync(outputFile);
}