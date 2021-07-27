using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_2019_3_OR_NEWER
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;
#else
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;
#endif

namespace PlayerLooper
{
    public interface IInitializable { void Initialize(); }
    public interface IPostInitializable { void PostInitialize(); }
    public interface IStartable { void Start(); }
    public interface IPostStartable { void PostStart(); }
    public interface IFixedUpdatable { void FixedUpdate(); }
    public interface IPostFixedUpdatable { void PostFixedUpdate(); }
    public interface IUpdatable { void Update(); }
    public interface IPostUpdatable { void PostUpdate(); }
    public interface ILateUpdatable { void LateUpdate(); }
    public interface IPostLateUpdatable { void PostLateUpdate(); }

    public enum PlayerLoopTiming
    {
        Initialization = 0,
        PostInitialization = 1,

        Startup = 2,
        PostStartup = 3,

        FixedUpdate = 4,
        PostFixedUpdate = 5,

        Update = 6,
        PostUpdate = 7,

        LateUpdate = 8,
        PostLateUpdate = 9,

        Count = 10,
    }

    public static class GlobalPlayerLooper
    {
        static readonly PlayerLoopRunner[] _loopRunners = new PlayerLoopRunner[(int)PlayerLoopTiming.Count];
        static readonly Dictionary<object, PlayerLoopItem> _disposables = new Dictionary<object, PlayerLoopItem>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Application.quitting += Quit;

            _loopRunners[(int)PlayerLoopTiming.Initialization] = new PlayerLoopRunner(true);
            _loopRunners[(int)PlayerLoopTiming.PostInitialization] = new PlayerLoopRunner(true);
            _loopRunners[(int)PlayerLoopTiming.Startup] = new PlayerLoopRunner(true);
            _loopRunners[(int)PlayerLoopTiming.PostStartup] = new PlayerLoopRunner(true);
            _loopRunners[(int)PlayerLoopTiming.FixedUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)PlayerLoopTiming.PostFixedUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)PlayerLoopTiming.Update] = new PlayerLoopRunner();
            _loopRunners[(int)PlayerLoopTiming.PostUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)PlayerLoopTiming.LateUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)PlayerLoopTiming.PostLateUpdate] = new PlayerLoopRunner();

            PlayerLoopSystem playerLoop =
#if UNITY_2019_3_OR_NEWER
                PlayerLoop.GetCurrentPlayerLoop();
#else
                PlayerLoop.GetDefaultPlayerLoop();
#endif

            PlayerLoopSystem[] subSystemList = playerLoop.subSystemList.ToArray();

            int initializationSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(Initialization), subSystemList);
            int earlyUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(EarlyUpdate), subSystemList);
            int fixedUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(FixedUpdate), subSystemList);
            int updateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(Update), subSystemList);
            int lateUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(PreLateUpdate), subSystemList);

            ref var initializationSystem = ref subSystemList[initializationSystemIndex];
            ref var earlyUpdateSystem = ref subSystemList[earlyUpdateSystemIndex];
            ref var fixedUpdateSystem = ref subSystemList[fixedUpdateSystemIndex];
            ref var updateSystem = ref subSystemList[updateSystemIndex];
            ref var lateUpdateSystem = ref subSystemList[lateUpdateSystemIndex];

            var playerLooperInitializationSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperInitialization>(_loopRunners[(int)PlayerLoopTiming.Initialization].Run);

            var playerLooperPostInitializationSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostInitialization>(_loopRunners[(int)PlayerLoopTiming.PostInitialization].Run);

            var playerLooperStartupSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperStartup>(_loopRunners[(int)PlayerLoopTiming.Startup].Run);

            var playerLooperPostStartupSystem =
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostStartup>(_loopRunners[(int)PlayerLoopTiming.PostStartup].Run);

            var playerLooperFixedUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperFixedUpdate>(_loopRunners[(int)PlayerLoopTiming.FixedUpdate].Run);

            var playerLooperPostFixedUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostFixedUpdate>(_loopRunners[(int)PlayerLoopTiming.PostFixedUpdate].Run);

            var playerLooperUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperUpdate>(_loopRunners[(int)PlayerLoopTiming.Update].Run);

            var playerLooperPostUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostUpdate>(_loopRunners[(int)PlayerLoopTiming.PostUpdate].Run);

            var playerLooperLateUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperLateUpdate>(_loopRunners[(int)PlayerLoopTiming.LateUpdate].Run);

            var playerLooperPostLateUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostLateUpdate>(_loopRunners[(int)PlayerLoopTiming.PostLateUpdate].Run);

            PlayerLoopHelper.InsertSubSystem(ref initializationSystem, 0, ref playerLooperInitializationSystem);
            PlayerLoopHelper.AddSubSystem(ref initializationSystem, ref playerLooperPostInitializationSystem);

            PlayerLoopHelper.InsertSubSystem(ref earlyUpdateSystem, typeof(EarlyUpdate.ScriptRunDelayedStartupFrame), ref playerLooperStartupSystem);
            PlayerLoopHelper.AddSubSystem(ref earlyUpdateSystem, ref playerLooperPostStartupSystem);

            PlayerLoopHelper.InsertSubSystem(ref fixedUpdateSystem, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), ref playerLooperFixedUpdateSystem);
            PlayerLoopHelper.AddSubSystem(ref fixedUpdateSystem, ref playerLooperPostFixedUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref updateSystem, typeof(Update.ScriptRunBehaviourUpdate), ref playerLooperUpdateSystem);
            PlayerLoopHelper.AddSubSystem(ref updateSystem, ref playerLooperPostUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref lateUpdateSystem, typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), ref playerLooperLateUpdateSystem);
            PlayerLoopHelper.AddSubSystem(ref lateUpdateSystem, ref playerLooperPostLateUpdateSystem);

            playerLoop.subSystemList = subSystemList;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        static void Quit()
        {
            foreach (var loopRunner in _loopRunners)
            {
                loopRunner.Clear();
            }

            foreach (var disposable in _disposables.Values)
            {
                disposable.Action?.Invoke();
            }

            _disposables.Clear();
        }

        public static void Register(Action action, PlayerLoopTiming timing)
        {
            _loopRunners[(int)timing].Register(action.Target, action);

            if (action.Target is IDisposable disposable)
            {
                if (!_disposables.ContainsKey(action.Target))
                {
                    PlayerLoopItem playerLoopItem = new PlayerLoopItem(){ Target = action.Target, Action = disposable.Dispose };
                    _disposables.Add(playerLoopItem.Target, playerLoopItem);
                }
            }
        }

        public static void Register(object target, Action action, PlayerLoopTiming timing)
        {
            _loopRunners[(int)timing].Register(target, action);

            if (target is IDisposable disposable)
            {
                if (!_disposables.ContainsKey(target))
                {
                    PlayerLoopItem playerLoopItem = new PlayerLoopItem(){ Target = target, Action = disposable.Dispose };
                    _disposables.Add(playerLoopItem.Target, playerLoopItem);
                }
            }
        }

        public static void Register(object target)
        {
            if (target is IInitializable initializable)
            {
                Register(target, initializable.Initialize, PlayerLoopTiming.Initialization);
            }

            if (target is IPostInitializable postInitializable)
            {
                Register(target, postInitializable.PostInitialize, PlayerLoopTiming.PostInitialization);
            }

            if (target is IStartable startable)
            {
                Register(target, startable.Start, PlayerLoopTiming.Startup);
            }

            if (target is IPostStartable postStartable)
            {
                Register(target, postStartable.PostStart, PlayerLoopTiming.PostStartup);
            }

            if (target is IFixedUpdatable fixedUpdatable)
            {
                Register(target, fixedUpdatable.FixedUpdate, PlayerLoopTiming.FixedUpdate);
            }

            if (target is IPostFixedUpdatable postFixedUpdatable)
            {
                Register(target, postFixedUpdatable.PostFixedUpdate, PlayerLoopTiming.PostFixedUpdate);
            }

            if (target is IUpdatable updatable)
            {
                Register(target, updatable.Update, PlayerLoopTiming.Update);
            }

            if (target is IPostUpdatable postUpdatable)
            {
                Register(target, postUpdatable.PostUpdate, PlayerLoopTiming.PostUpdate);
            }

            if (target is ILateUpdatable lateUpdatable)
            {
                Register(target, lateUpdatable.LateUpdate, PlayerLoopTiming.LateUpdate);
            }

            if (target is IPostLateUpdatable postLateUpdatable)
            {
                Register(target, postLateUpdatable.PostLateUpdate, PlayerLoopTiming.PostLateUpdate);
            }
        }

        public static void Unregister(object target, PlayerLoopTiming timing)
        {
            _loopRunners[(int)timing].Unregister(target);
        }

        public static void Unregister(object target)
        {
            foreach (var loopRunner in _loopRunners)
            {
                loopRunner.Unregister(target);
            }

            if (_disposables.TryGetValue(target, out PlayerLoopItem disposable))
            {
                disposable.Action?.Invoke();
                _disposables.Remove(target);
            }
        }

        struct PlayerLooperInitialization {}
        struct PlayerLooperPostInitialization {}
        struct PlayerLooperStartup {}
        struct PlayerLooperPostStartup {}
        struct PlayerLooperFixedUpdate {}
        struct PlayerLooperPostFixedUpdate {}
        struct PlayerLooperUpdate {}
        struct PlayerLooperPostUpdate {}
        struct PlayerLooperLateUpdate {}
        struct PlayerLooperPostLateUpdate {}
    }
}
