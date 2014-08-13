using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace osm2mssql.Importer.Model
{
    public class ImporterModel
    {
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string BingApi { get; set; }
        public string WebHost { get; set; }
        public int WebPort { get; set; }
        public string WebDatabase { get; set; }
        public string WebUsername { get; set; }
        public string WebPassword { get; set; }
    }
}
