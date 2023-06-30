using System;
using UnityPlayerLooper;

namespace UnityPlayerLooperSamples
{
    public class LoopCounterB : IStartable, ITickable, IDisposable
    {
        public int LoopCount => _loopCount;
        private int _loopCount;

        public void Startup()
        {
            _loopCount = -1000;
        }

        public void Tick()
        {
            _loopCount++;
        }

        public void Dispose()
        {
            UnityEngine.Debug.Log("LoopCounterB.Dispose");
        }
    }
}
