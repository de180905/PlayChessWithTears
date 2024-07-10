namespace OnlineChess.Utils
{
    using System;
    using System.Threading;

    public class CountdownTimer
    {
        private readonly TimeSpan duration;
        private TimeSpan timeRemaining;
        private bool isRunning;
        private bool isPaused;
        private Timer timer;

        public event EventHandler<TimeSpan> TimeChanged;
        public event EventHandler TimerElapsed;

        public TimeSpan TimeRemaining => timeRemaining;

        public CountdownTimer(TimeSpan duration)
        {
            this.duration = duration;
            this.timeRemaining = duration;
        }

        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                isPaused = false;
                timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
        }

        public void Pause()
        {
            if (isRunning && !isPaused)
            {
                isPaused = true;
                timer?.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        public void Resume()
        {
            if (isRunning && isPaused)
            {
                isPaused = false;
                timer?.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                timer?.Dispose();
                timeRemaining = duration;
                OnTimeChanged(timeRemaining);
            }
        }

        private void TimerCallback(object state)
        {
            if (!isPaused)
            {
                timeRemaining = timeRemaining.Subtract(TimeSpan.FromSeconds(1));
                OnTimeChanged(timeRemaining);

                if (timeRemaining <= TimeSpan.Zero)
                {
                    Stop();
                    TimerElapsed?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnTimeChanged(TimeSpan time)
        {
            TimeChanged?.Invoke(this, time);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Create a countdown timer with a duration of 10 minutes
            TimeSpan duration = TimeSpan.FromMinutes(10);
            CountdownTimer timer = new CountdownTimer(duration);

            // Subscribe to timer events
            timer.TimeChanged += Timer_TimeChanged;
            timer.TimerElapsed += Timer_TimerElapsed;

            // Start the timer
            timer.Start();

            Console.WriteLine("Countdown timer started. Press any key to pause/resume, or 'q' to quit.");

            // Allow user interaction
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Q)
                {
                    break;
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    if (timer.TimeRemaining > TimeSpan.Zero)
                    {
                        if (timer.TimeRemaining == duration)
                        {
                            Console.WriteLine("Timer paused.");
                        }
                        else
                        {
                            Console.WriteLine("Timer resumed.");
                        }
                        timer.Pause();
                    }
                    else
                    {
                        Console.WriteLine("Timer has elapsed.");
                    }
                }
            }

            // Stop the timer when done
            timer.Stop();
        }

        private static void Timer_TimeChanged(object sender, TimeSpan e)
        {
            Console.WriteLine($"Time remaining: {e.Minutes:00}:{e.Seconds:00}");
        }

        private static void Timer_TimerElapsed(object sender, EventArgs e)
        {
            Console.WriteLine("Timer elapsed!");
        }
    }

}
