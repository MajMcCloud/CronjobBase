using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronjobBase.tasks
{
    /// <summary>
    /// Runs only once a day on work days.
    /// </summary>
    public class WorkingDaysTask : Crontask
    {
        public WorkingDaysTask(TimeSpan timeOfDay) : base(timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds, eRunningType.daily)
        {

        }


        public override bool ShouldRun()
        {
            return (DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday);
        }



    }
}
