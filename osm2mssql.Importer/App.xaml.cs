using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using osm2mssql.Importer.Classes;

namespace osm2mssql.Importer
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            if (!Trace.Listeners.OfType<OsmTextWriterTraceListener>().Any())
            {
                Trace.Listeners.Add(new OsmTextWriterTraceListener("OsmServiceLog.txt"));
            }
            Trace.AutoFlush = true;
        }

        public static string GetResourceFileText(string resourceName)
        {
            var asm = Assembly.GetExecutingAssembly();
            if (resourceName.Contains("osm2mssql.Library"))
                asm = typeof (OsmTextWriterTraceListener).Assembly;

            using (var stream = asm.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

}
