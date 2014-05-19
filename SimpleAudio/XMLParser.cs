using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SimpleAudio
{
    public class XMLParser
    {
        ///XML example Format
        /*
         <?xml version="1.0" encoding="UTF-8"?>                     
            <Settings>  
		        <media>   
			        <local path="C:\Users\Stefan\Downloads"/>                                                
			        <local path="C:\Users\Stefan\Music"/>                                       
		        </media>
            </Settings>
         */

        /// <summary>
        /// Load settings from an xmlfile into the application
        /// </summary>
        /// <param name="path">Path to the settings file</param>
        /// <returns></returns>
        public Settings LoadSettings(string path)
        {
            Settings settings = new Settings();
            using (FileStream filestream = File.OpenRead(path))
            {
                XmlReaderSettings s = new XmlReaderSettings();

                using (XmlReader reader = XmlReader.Create(filestream, s))
                {
                    reader.ReadToFollowing("media");
                    XmlReader inner = reader.ReadSubtree();
                    settings.AddMediaPaths(ReadMedia(inner));
                }
            }

            return settings;
        }
        private IEnumerable<string> ReadMedia(XmlReader media)
        {

            while (media.ReadToFollowing("local"))
            {
                media.MoveToFirstAttribute();
                yield return media.ReadContentAsString();
            }
        }
    }
}
