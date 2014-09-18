﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace SimpleAudio
{
    public class Settings
    {
        /*
         * XML example Format
         *
           
           <?xml version="1.0" encoding="UTF-8"?>
           <settings>
		       <media>
			       <local path="C:\Users\Stefan\Downloads"/>
			       <local path="C:\Users\Stefan\Music"/>
		       </media>
           </settings>
           
        */

        private MediaPathCollection mediapaths;

        public MediaPathCollection Mediapaths
        {
            get { return mediapaths; }
        }

        public Settings(XElement settingsElement)
        {
            XElement media = settingsElement.Element("media");
            if (media == null)
                settingsElement.Add(media = new XElement("media"));

            this.mediapaths = new MediaPathCollection(media);
        }

        public class MediaPathCollection : IEnumerable<string>
        {
            private XElement mediaElement;
            private List<string> paths;

            public MediaPathCollection(XElement mediaElement)
            {
                this.mediaElement = mediaElement;
                paths = new List<string>(from e in mediaElement.Elements()
                                         let p = e.Attribute("path")
                                         where e.Name == "local" && p != null
                                         select p.Value);
            }

            public void Add(string path)
            {
                if (paths.Contains(path))
                    throw new ArgumentException("A mediapath can only be added once.", "path");

                paths.Add(path);
                mediaElement.Add(new XElement("local", new XAttribute("path", path)));
            }
            public bool Remove(string path)
            {
                if (!paths.Contains(path))
                    return false;

                paths.Remove(path);
                var e = getElementFromPath(path);

                if (e != null)
                    e.Remove();

                return true;
            }

            private XElement getElementFromPath(string path)
            {
                foreach (var e in mediaElement.Elements("local"))
                {
                    var a = e.Attribute("path");
                    if (a != null && a.Value == path)
                        return e;
                }
                return null;
            }

            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                foreach (var p in paths)
                    yield return p;
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                foreach (var p in paths)
                    yield return p;
            }
        }
    }
}
