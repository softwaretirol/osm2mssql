using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osm2mssql.Importer.Languages
{
    public class Language
    {
        public string this[object key]
        {
            get
            {
                if(App.Current == null)
                    return string.Empty;
                return App.Current.Resources[key] as string ?? string.Empty;
            }
        }

        public static Language CurrentLanguage { get; private set; }
        static Language()
        {
            CurrentLanguage = new Language();
        }
    }
}
