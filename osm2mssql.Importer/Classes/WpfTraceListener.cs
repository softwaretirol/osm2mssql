using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osm2mssql.Importer.Classes
{
    internal class WpfTraceListener : TraceListener
    {
        private Action<string> _action;
        public WpfTraceListener(Action<string> action)
        {
            _action = action;
        }

        public override void Write(string message)
        {
            if (_action != null)
                _action(message);
        }

        public override void WriteLine(string message)
        {
            if (_action != null)
                _action(message);
        }
    }
}
