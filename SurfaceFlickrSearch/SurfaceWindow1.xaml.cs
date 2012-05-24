/**************************************************************************
 * Flickr Client for Microsoft Surface 2.0
 * Code by sascha.corti@microsoft.com 
 * Bugs after this line.
 * 
 *  \__/      \__/       \__/       \__/       \__/       \__/       \__/
 *  (oo)      (o-)       (@@)       (xx)       (--)       (  )       (OO)
 * //||\\    //||\\     //||\\     //||\\     //||\\     //||\\     //||\\
 *  bug       bug        bug/w      dead       bug       blind     bug after
 *          winking    hangover     bug      sleeping     bug      seeing a
 *                                                                  female
 *                                                                   bug
 *                                                                   
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Net;
using System.Xml.Linq;
using System.Windows.Media.Animation;

namespace SurfaceFlickrSearch
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

        string sFlickrAPIKey;
        WebClient flickr = new WebClient();
        Settings settings = new Settings();
        Storyboard sb;
        int count = 0;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            flickr.DownloadStringCompleted += new DownloadStringCompletedEventHandler(flickr_DownloadStringCompleted);
            sb = this.FindResource("Animate") as Storyboard;

            if (settings.Load() == false)
            {
                tbError.Text = "Error loading Settings.";
                sb.Begin();
            }
            else
            {
                sFlickrAPIKey = settings.FlickrApiKey;
                if (sFlickrAPIKey == "")
                {
                    tbError.Text = "Flickr API Key not set.";
                    sb.Begin();
                }

                TagVisualizationDefinition1.Value = settings.ByteTag;
                if (TagVisualizationDefinition1.Value == "")
                {
                    tbError.Text = "No Byte Tag defined in Settings.";
                    sb.Begin();
                }
            }
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        void flickr_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
                return;

            XElement xmlPhotos = XElement.Parse(e.Result);

            IEnumerable<FlickrItem> list;
            list = from photo in xmlPhotos.Descendants("photo")
                   select new FlickrItem()
                   {
                       ImageSource = "http://farm" + photo.Attribute("farm").Value + ".static.flickr.com/" + photo.Attribute("server").Value + "/" + photo.Attribute("id").Value + "_" + photo.Attribute("secret").Value + "_b.jpg",
                       Description = photo.Attribute("title").Value
                   };

            int c = 0;

            foreach (FlickrItem i in list)
            {
                if (c < count)
                {
                    FlickrPhoto p = new FlickrPhoto();
                    p.image1.Source = new BitmapImage(new Uri(i.ImageSource, UriKind.Absolute));
                    p.textBlock1.Text = i.Description;
                    scatterView1.Items.Add(p);
                    c++;
                }
            }
        }

        private void TagVisualizer_VisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            ((TagVisualization1)e.TagVisualization).ExecuteSearch += new EventHandler(SurfaceWindow1_ExecuteSearch);
            ((TagVisualization1)e.TagVisualization).ExecuteClear += new EventHandler(SurfaceWindow1_ExecuteClear);
        }

        void SurfaceWindow1_ExecuteSearch(object sender, EventArgs e)
        {
            try
            {
                flickr.DownloadStringAsync(new Uri("http://api.flickr.com/services/rest/?method=flickr.photos.search&api_key="
                              + sFlickrAPIKey + "&page=1&per_page=30&text=" + ((MyQuery)e).Text));
                count = ((MyQuery)e).Results;
            }
            catch (Exception ex)
            {
                tbError.Text = "Problems Connecting to Flickr.";
                sb.Begin();
            }
        }

        void SurfaceWindow1_ExecuteClear(object sender, EventArgs e)
        {
            scatterView1.Items.Clear();
        }
    }
}