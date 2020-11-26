# CronjobBase
 Tools for managing recurring events within an application.

 For instance: sending notifications, mails, cleanup of old sessions, statistic calculations, etc


 ## Basic example:

 Sending a console output every 5 minutes.

 ```

    public class sendOutput : Crontask
    {
        public sendOutput() : base(0, 0, 5, eRunningType.interval)
        {


        }

        public override void DoCronTask()
        {
            
            Console.WriteLine("Test Test");
        }

        //Catch exception details when necessary
        public override void ExceptionRaised(Exception ex)
        {
            
        }


    }



 ```


 ## Another example

 Send an output to the console daily only once at 2 PM (14:00)


```


    public class sendAlert : Crontask
    {
        public sendAlert() : base(0, 14, 0, eRunningType.daily)
        {


        }

        public override void DoCronTask()
        {
            
            Console.WriteLine("Alert Alert");
        }

        //Catch exception details when necessary
        public override void ExceptionRaised(Exception ex)
        {
            
        }


    }




```


 ## Yet another example

 Send an output to the console only on fridays


```


    public class sendFriday : Crontask
    {
        public sendFriday() : base(0, 12, 0, DayOfWeek.Friday, eRunningType.weekly)
        {


        }

        public override void DoCronTask()
        {
            
            Console.WriteLine("Its friday again!");
        }

        //Catch exception details when necessary
        public override void ExceptionRaised(Exception ex)
        {
            
        }


    }




```