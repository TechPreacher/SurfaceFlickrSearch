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
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace SurfaceFlickrSearch
{
    public class Settings
    {
        public string FlickrApiKey{ get; set; }
        public string ByteTag { get; set; }

        public bool Save()
        {
            bool bSuccess = false;

            // Save the FileName token to "SoMeSettings.xml".
            try
            {
                using (Stream fs = File.Open("Settings.xml", FileMode.OpenOrCreate))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings));
                    ser.Serialize(fs, this);
                    bSuccess = true;
                }
            }
            catch (Exception ex)
            {
            }
            return bSuccess;
        }

        public bool Load()
        {
            bool bSuccess = false;

            // Find "SoMeSettings.xml" and try loading the Filename.
            if (File.Exists("Settings.xml"))
            {
                using (Stream fs = File.Open("settings.xml", FileMode.Open))
                {
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(Settings));
                        object SettingsLoaded = ser.Deserialize(fs);

                        if (null != SettingsLoaded && SettingsLoaded is Settings)
                        {
                            this.FlickrApiKey = ((Settings)SettingsLoaded).FlickrApiKey.ToString();
                            this.ByteTag = ((Settings)SettingsLoaded).ByteTag.ToString();

                            bSuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return bSuccess;
        }

        public Settings()
        {
            // Return instance with default values.

            FlickrApiKey = "";
            ByteTag = "0x3A";
        }
    }
}
