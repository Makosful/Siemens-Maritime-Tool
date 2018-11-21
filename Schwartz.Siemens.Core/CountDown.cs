using System;
using System.Timers;

namespace Schwartz.Siemens.Core
{
    public class CountDown
    {
        public CountDown()
        {
            Timer = new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);
            Timer.Elapsed += Timer_Elapsed;
            Timer.AutoReset = true;
            Timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Elapsed");
        }

        public Timer Timer { get; }
    }
}