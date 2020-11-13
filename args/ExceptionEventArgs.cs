using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronjobBase.args
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception RaisedException { get; set; }

        public ExceptionEventArgs(Exception ex)
        {
            this.RaisedException = ex;
        }

    }
}
