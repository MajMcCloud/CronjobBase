using CronjobBase.args;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CronjobBase.extensions;
using Microsoft.Win32;

namespace CronjobBase
{
    public class Crontask : ICrontask
    {
        public enum eRunningType
        {
            /// <summary>
            /// Daily on a specific time. (i.e. daily at 1 PM)
            /// </summary>
            daily = 0,
            /// <summary>
            /// One after one (i.e. after every 5 minutes)
            /// </summary>
            interval = 1,
            /// <summary>
            /// Interval (every 3 hours z.B.: 0, 3, 6, 9, 12, 15, 18, 21)
            /// </summary>
            absolute = 2,

            /// <summary>
            /// weekly on a specific day and time (z.b. sunday at 2 am).
            /// </summary>
            weekly = 3,
        }

        protected TimeSpan _duration { get; set; }

        protected DayOfWeek _dayofWeek { get; set; }

        protected DateTime? _lastrun { get; set; }

        protected eRunningType _runningType { get; set; } = eRunningType.daily;

        protected Timer _timer { get; set; }


        private EventHandlerList __Events = new EventHandlerList();

        private static object __evDoTask = new object();
        private static object __evException = new object();


        public bool CatchExceptions { get; set; } = true;

        public bool StopOnException { get; set; } = false;

        /// <summary>
        /// Returns if this task is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return (_timer != null);
            }
        }


        public Crontask()
        {
            CrontaskManager.RegisterTask(this);
        }

        public Crontask(TimeSpan duration, eRunningType runningType) : this()
        {
            this._duration = duration;
            this._runningType = runningType;
        }

        public Crontask(int hours, int minutes, int seconds, eRunningType runningType) : this()
        {
            this._duration = new TimeSpan(hours, minutes, seconds);
            this._runningType = runningType;
        }

        public Crontask(int hours, int minutes, int seconds, DayOfWeek weekDay, eRunningType runningType) : this()
        {
            this._duration = new TimeSpan(hours, minutes, seconds);
            this._runningType = runningType;
            this._dayofWeek = weekDay;
        }

        ~Crontask()
        {
            CrontaskManager.UnregisterTask(this);
        }

        public void Start(bool doInitialRun = false)
        {
            if (_duration == null)
                return;

            if (doInitialRun)
            {
                InvokeAction();

                return;
            }



            _timer = new Timer();
            _timer.AutoReset = false;
            _timer.Interval = CalculateInterval();
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();

        }



        public void InvokeAction()
        {

            _timer_Elapsed(null, null);

        }

        private double CalculateInterval()
        {
            if (this._runningType == eRunningType.interval)
            {
                return _duration.TotalMilliseconds;
            }

            if (this._runningType == eRunningType.daily)
            {
                var startDay = _lastrun?.Date ?? DateTime.Today;
                var nextDay = startDay;

                //Today already done? Search for tomorrow
                if (startDay.Add(this._duration) <= DateTime.Now | (_lastrun != null && _lastrun?.Date == DateTime.Today))
                {
                    nextDay = startDay.AddDays(1);
                }

                return (nextDay.Add(this._duration) - DateTime.Now).TotalMilliseconds;
            }

            if (this._runningType == eRunningType.absolute)
            {
                DateTime dt = DateTime.Now;
                TimeSpan ts = dt.TimeOfDay;

                TimeSpan tsNew = dt.TimeOfDay;
                DateTime dtTarget = dt;
                if (this._duration.Hours > 0)
                {
                    tsNew = new TimeSpan(ts.Hours + this._duration.Hours, 0, 0);
                    dtTarget = DateTime.Today.Add(tsNew);
                }

                if (this._duration.Minutes > 0)
                {
                    dtTarget = dtTarget.RoundUp(new TimeSpan(0, this._duration.Minutes, 0));
                }

                if (this._duration.Seconds > 0)
                {
                    dtTarget = dtTarget.RoundUp(new TimeSpan(0, 0, this._duration.Seconds));
                }

                return (dtTarget - DateTime.Now).TotalMilliseconds;
            }

            if (this._runningType == eRunningType.weekly)
            {
                var nextDay = DateTimeExtensions.GetNextWeekday(DateTime.Now, this._dayofWeek).Date;
                if (nextDay.Date == DateTime.Today)
                    nextDay = nextDay.AddDays(7);

                var nextTime = nextDay.Add(_duration);


                return (nextTime - DateTime.Now).TotalMilliseconds;
            }


            return 0;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_timer != null)
                _timer.Stop();

            try
            {
                _lastrun = DateTime.Now;

                OnDoTask(new EventArgs());

                DoCronTask();
            }
            catch (Exception ex)
            {
                OnException(new ExceptionEventArgs(ex));

                ExceptionRaised(ex);

                if (!this.CatchExceptions)
                    throw ex;

                //When enabled, stop on exception
                if (this.StopOnException)
                {
                    return;
                }
            }

            //Restart
            Start();
        }

        /// <summary>
        /// Stops the task.
        /// </summary>
        public void Stop()
        {
            if (_timer == null)
                return;

            _timer.Stop();

            _timer = null;

        }

        public event EventHandler DoTask
        {
            add
            {
                this.__Events.AddHandler(__evDoTask, value);
            }
            remove
            {
                this.__Events.RemoveHandler(__evDoTask, value);
            }
        }


        private void OnDoTask(EventArgs e)
        {
            (__Events[__evDoTask] as EventHandler)?.Invoke(this, e);
        }

        public event EventHandler<ExceptionEventArgs> Exception
        {
            add
            {
                this.__Events.AddHandler(__evException, value);
            }
            remove
            {
                this.__Events.RemoveHandler(__evException, value);
            }
        }


        private void OnException(ExceptionEventArgs e)
        {
            (__Events[__evException] as EventHandler<ExceptionEventArgs>)?.Invoke(this, e);
        }

        /// <summary>
        /// Does a Task in sync
        /// </summary>
        public virtual void DoCronTask()
        {

            var task = DoAsyncCronTask();

            task.GetAwaiter().GetResult();

        }

        /// <summary>
        /// Does one or multiple tasks async and waites for completion.
        /// </summary>
        /// <returns></returns>
        public virtual async Task DoAsyncCronTask()
        {

        }

        public virtual void ExceptionRaised(Exception ex)
        {

        }

        public void Log(String message)
        {
            Console.WriteLine(DateTime.Now.ToString() + " " + message);

        }
    }
}
