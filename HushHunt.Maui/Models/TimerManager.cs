using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HushHunt.Maui.Models
{
    public class TimerManager
    {
        private Stopwatch _stopwatch;
        private TimeSpan _initialTime;
        private TimeSpan _remainingTime;

        public TimerManager(TimeSpan initialTime)
        {
            _stopwatch = new Stopwatch();
            _initialTime = initialTime;
            _remainingTime = initialTime;
        }

        public TimeSpan RemainingTime => _remainingTime;

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }

        public void Reset()
        {
            _stopwatch.Reset();
            _remainingTime = _initialTime;
        }

        public void Update()
        {
            if (_stopwatch.IsRunning)
            {
                var elapsed = _stopwatch.Elapsed;
                var newRemainingTime = _initialTime - elapsed;

                _remainingTime = newRemainingTime < TimeSpan.Zero ? TimeSpan.Zero : newRemainingTime;
            }
        }
    }
}
