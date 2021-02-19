using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronjobBase.tasks
{
    /// <summary>
    /// Runs every day one at a specific time.
    /// </summary>
    public class DailyTasks : Crontask
    {
        public DailyTasks(TimeSpan timeOfDay) : base(timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds, eRunningType.daily)
        {


        }


    }
}
