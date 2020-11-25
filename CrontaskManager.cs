using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronjobBase
{
    /// <summary>
    /// A class to manage all Crontask instances and take care about time changings.
    /// </summary>
    public static class CrontaskManager
    {
        /// <summary>
        /// A list of all registered tasks. Will automatically register after instance creation.
        /// </summary>
        public static List<Crontask> RegisteredTasks { get; set; }

        private static DateTime LastTimeCheck { get; set; }

        private static Crontask TimeChecker { get; set; }


        static CrontaskManager()
        {
            RegisteredTasks = new List<Crontask>();

            LastTimeCheck = DateTime.Now;


            //Checker for detect time changes due VM store/recover or others
            TimeChecker = new Crontask(0, 1, 0, Crontask.eRunningType.interval);
            RegisteredTasks.Remove(TimeChecker);
            TimeChecker.DoTask += TimeChecker_DoTask;
            TimeChecker.Start();

            SystemEvents.TimeChanged += SystemEvents_TimeChanged;
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        #region "Time-Checkings"

        private static void TimeChecker_DoTask(object sender, EventArgs e)
        {
            //One minute later, changes just a "minute", no change detectable
            if (DateTime.Now.Subtract(LastTimeCheck).TotalSeconds <= 65)
            {
                LastTimeCheck = DateTime.Now;
                return;
            }

            RestartAllTasks();

            LastTimeCheck = DateTime.Now;
        }


        //Time has changed. Running in VM ?
        private static void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            RestartAllTasks();
        }

        /// <summary>
        /// PowerMode has changed?
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            RestartAllTasks();
        }

        /// <summary>
        /// This will restart all tasks.
        /// </summary>
        public static void RestartAllTasks()
        {
#if DEBUG
            int i = 0;
#endif
            foreach (var ct in RegisteredTasks)
            {
                if (!ct.IsRunning)
                    continue;

                ct.Stop();

                ct.Start();

#if DEBUG
                i++;
#endif
            }
#if DEBUG
            Console.WriteLine($"Restart {i} tasks");
#endif
        }

        #endregion

        /// <summary>
        /// Registers task for future use.
        /// </summary>
        /// <param name="task"></param>
        public static void RegisterTask(Crontask task)
        {
            if (RegisteredTasks.Contains(task))
                return;

            RegisteredTasks.Add(task);
        }

        /// <summary>
        /// Unregisters task from collection.
        /// </summary>
        /// <param name="task"></param>
        public static void UnregisterTask(Crontask task)
        {
            if (!RegisteredTasks.Contains(task))
                return;

            RegisteredTasks.Remove(task);
        }

    }
}
