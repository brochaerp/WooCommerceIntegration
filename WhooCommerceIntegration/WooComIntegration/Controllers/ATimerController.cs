using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WooComIntegration
{
    public abstract class ATimerController
    {
        public event EventHandler Started, Stopped;
        public event EventHandler<ExceptionEventArgs> ErrorOccurred;

        protected Timer repeatingTimer;

        protected void setupTimer(int timerSeconds, ElapsedEventHandler elapsedEventHandler)
        {
            repeatingTimer = new Timer(TimeSpan.FromSeconds(timerSeconds).TotalMilliseconds);
            repeatingTimer.Elapsed += elapsedEventHandler;
            repeatingTimer.AutoReset = true;
            //repeatingTimer.Enabled = true; // Auto start when created.      
        }

        protected void OnStarted(EventArgs e)
        {
            if (Started != null)
                Started(this, e);
        }

        protected void OnStopped(EventArgs e)
        {
            if (Stopped != null)
                Stopped(this, e);
        }

        protected void OnErrorOccurred(Exception exception)
        {
            if (ErrorOccurred != null)
                ErrorOccurred(this, new ExceptionEventArgs(exception));
        }
    }
}
