using UnityEngine;
using PlayerLooper;

namespace PlayerLooperSamples
{
    public class Lifecycle : MonoBehaviour
    {
        [SerializeField] LoopCountPresenter _loopCountPresenter;

        LoopCounterA _loopCounterA;
        LoopCounterB _loopCounterB;

        void Awake()
        {
            _loopCounterA = new LoopCounterA();
            _loopCounterB = new LoopCounterB();
            _loopCountPresenter.Construct(_loopCounterA, _loopCounterB);

            // Method registeration
            GlobalPlayerLooper.Register(_loopCounterA.Startup, PlayerLoopTiming.Startup);
            GlobalPlayerLooper.Register(_loopCounterA.Tick, PlayerLoopTiming.Update);

            // Instance registration
            GlobalPlayerLooper.Register(_loopCounterB);
        }
    }
}
