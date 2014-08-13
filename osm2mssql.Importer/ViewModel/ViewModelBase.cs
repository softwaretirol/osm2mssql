using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;

namespace osm2mssql.Importer.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(property));
        }
        
        protected void SaveModelToFile<T>(string filePath, T Model)
        {
            try
            {
                var ser = new XmlSerializer(typeof(T));
                using (var file = File.Create(filePath))
                {
                    ser.Serialize(file, Model);
                }
            }
            catch
            {

            }
        }

        protected T LoadModelFromFile<T>(string filePath)
        {
            try
            {
                var ser = new XmlSerializer(typeof(T));
                using (var file = File.Open(filePath, FileMode.Open))
                {
                    return (T)ser.Deserialize(file);
                }
            }
            catch
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}
