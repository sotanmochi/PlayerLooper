using UnityEngine;
using UnityEngine.UI;

namespace PlayerLooperSamples
{
    public class LoopCountPresenter : MonoBehaviour
    {
        [SerializeField] Text _countViewA;
        [SerializeField] Text _countViewB;

        private LoopCounterA _counterA;
        private LoopCounterB _counterB;

        public void Construct(LoopCounterA counterA, LoopCounterB counterB)
        {
            _counterA = counterA;
            _counterB = counterB;
        }

        void LateUpdate()
        {
            _countViewA.text = $"LoopCounterA: {_counterA.LoopCount}";
            _countViewB.text = $"LoopCounterB: {_counterB.LoopCount}";
        }
    }
}