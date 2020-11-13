using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronjobBase
{
    interface ICrontask
    {

        void Start(bool doInitialRun = false);


        void Stop();

        /// <summary>
        /// Wird aufgerufen, wenn der Zeitraum abgelaufen ist.
        /// </summary>
        void DoCronTask();

        /// <summary>
        /// Wird aufgerufen wenn ein Fehler aufgetreten ist.
        /// </summary>
        /// <param name="ex"></param>
        void ExceptionRaised(Exception ex);

    }
}
