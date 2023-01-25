using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcendaService
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception PassedException { get; set; }

        public ExceptionEventArgs()
        {
        }

        public ExceptionEventArgs(Exception exception)
        {
            PassedException = exception;
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return PassedException.ToString();
        }
    }
}
