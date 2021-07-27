using System;
using PlayerLooper;

namespace PlayerLooperSamples
{
    public class LoopCounterB : IStartable, IUpdatable, IDisposable
    {
        public int LoopCount => _loopCount;
        private int _loopCount;

        public void Start()
        {
            _loopCount = -1000;
        }

        public void Update()
        {
            _loopCount++;
        }

        public void Dispose()
        {
            UnityEngine.Debug.Log("LoopCounterB.Dispose");
        }
    }
}
