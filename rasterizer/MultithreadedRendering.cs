// How to use Multithreaded Rendering.
namespace MultithreadRendering
{
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;
 
    using Apitron.PDF.Rasterizer;
    using Apitron.PDF.Rasterizer.Configuration;
 
    class Program
    {
        private static object syncRoot = new object();
 
        static void Main( string[] args )
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
 
            ThreadPool.SetMaxThreads( 4, 4 );
 
            // Get pdf files collection
            string pdfFilesCollection = "Input\\";
            string[] pdfFiles = Directory.GetFiles( pdfFilesCollection, "*.pdf", SearchOption.AllDirectories );
            List<Stream&dt; streams = new List<Stream>();
 
            // Wait handles
            List<ManualResetEvent> manualResetEvents = new List<ManualResetEvent>();
 
            // Create streams
            for (int i = 0; i < pdfFiles.Length; i++)
            {
                FileStream stream = new FileStream( pdfFiles[i], FileMode.Open, FileAccess.Read );
                streams.Add( stream );
            }
 
            // Queue work items for documents
            for (int i = 0; i < pdfFiles.Length; i++)
            {
                ThreadPool.QueueUserWorkItem( RenderFile, new object[] { streams[i], pdfFiles[i], manualResetEvents } );
            }
 
            // Wait while all documents isn't ready
            int eventsCount = pdfFiles.Length - 1;
            while (eventsCount > 0)
            {
                WaitHandle[] events;
                lock (syncRoot)
                {
                    events = manualResetEvents.ToArray();
                }
                if (events.Length == 0)
                {
                    continue;
                }
 
                for (int i = 0; i < events.Length; i++)
                {
                    WaitHandle.WaitAll( new[] { events[i] } );
                    lock (syncRoot)
                    {
                        manualResetEvents.Remove( (ManualResetEvent)events[i] );
                        eventsCount--;
                    }
                }
            }
 
            // Close streams
            for (int i = 0; i < streams.Count; i++)
            {
                streams[i].Close();
            }
 
            // Show resulting time
            stopwatch.Stop();
            Console.WriteLine( stopwatch.Elapsed.TotalMilliseconds );
            Console.ReadKey();
        }
 
 
        private static void RenderFile( object state )
        {
            // Parse arguments
            object[] args = (object[])state;
            Stream stream = (Stream)args[0];
            List<ManualResetEvent> events = (List<ManualResetEvent>)args[2];
            string fileName = (string)args[1];
 
            // Open pdf document
            try
            {
                Document document = new Document( stream );
 
                // Queue work items for pages
                for (int i = 0; i < document.Pages.Count; i++)
                {
                    ManualResetEvent manualReset = new ManualResetEvent( false );
                    try
                    {
                        lock (syncRoot)
                        {
                            events.Add( manualReset );
                        }
                        Page page = document.Pages[i];
 
                        // Render a page
                        Bitmap bmp = page.Render( (int)page.Width, (int)page.Height, new RenderingSettings() );
                        bmp.Save( string.Format( "{0}_{1}_{2}.png", fileName, i, xScale ), ImageFormat.Png );
                    }
                    finally
                    {
                        manualReset.Set();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine( e );
            }
        }
 
        private static int xScale = 1;
    }
}