using System;

namespace UnityPlayerLooperSamples
{
    public class LoopCounterA : IDisposable
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
            UnityEngine.Debug.Log("LoopCounterA.Dispose");
        }
    }
}
