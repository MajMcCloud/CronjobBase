using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronjobBase.tasks
{
    /// <summary>
    /// Runs on a specific time on specific week days.
    /// </summary>
    public class SomeDaysTask : Crontask
    {
        public List<DayOfWeek> DaysOfWeek { get; set; }

        public SomeDaysTask(TimeSpan timeOfDay, params DayOfWeek[] days) : base(timeOfDay.Hours, timeOfDay.Minutes, timeOfDay.Seconds, eRunningType.daily)
        {
            this.DaysOfWeek = days.ToList();
        }

        public override bool ShouldRun()
        {
            return (DaysOfWeek.Contains(DateTime.Now.DayOfWeek));
        }


    }
}
