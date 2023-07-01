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

namespace UnityPlayerLooper
{
    public static class GlobalPlayerLooper
    {
        static readonly PlayerLoopRunner[] _loopRunners = new PlayerLoopRunner[(int)LoopTiming.Count];
        static readonly Dictionary<object, PlayerLoopItem> _disposables = new Dictionary<object, PlayerLoopItem>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            Application.quitting += Quit;

            _loopRunners[(int)LoopTiming.PreTimeUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostTimeUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreInitialization] = new PlayerLoopRunner(true);
            _loopRunners[(int)LoopTiming.PostInitialization] = new PlayerLoopRunner(true);
            _loopRunners[(int)LoopTiming.PreStartup] = new PlayerLoopRunner(true);
            _loopRunners[(int)LoopTiming.PostStartup] = new PlayerLoopRunner(true);
            _loopRunners[(int)LoopTiming.PreBehaviourFixedUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostBehaviourFixedUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreBehaviourUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostBehaviourUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreDelayedTasks] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostDelayedTasks] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreBehaviourLateUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostBehaviourLateUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PrePhysicsFixedUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostPhysicsFixedUpdate] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreUpdateAllRenderers] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostUpdateAllRenderers] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreUpdateAllSkinnedMeshes] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostUpdateAllSkinnedMeshes] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreDirectorUpdateAnimationBegin] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostDirectorUpdateAnimationEnd] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PrePlayerUpdateCanvases] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostPlayerUpdateCanvases] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PreUpdateAudio] = new PlayerLoopRunner();
            _loopRunners[(int)LoopTiming.PostUpdateAudio] = new PlayerLoopRunner();

            PlayerLoopSystem playerLoop =
#if UNITY_2019_3_OR_NEWER
                PlayerLoop.GetCurrentPlayerLoop();
#else
                PlayerLoop.GetDefaultPlayerLoop();
#endif

            PlayerLoopSystem[] subSystemList = playerLoop.subSystemList.ToArray();

            int timeUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(TimeUpdate), subSystemList);
            int initializationSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(Initialization), subSystemList);
            int earlyUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(EarlyUpdate), subSystemList);
            int fixedUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(FixedUpdate), subSystemList);
            int updateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(Update), subSystemList);
            int lateUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(PreLateUpdate), subSystemList);
            int preLateUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(PreLateUpdate), subSystemList);
            int postLateUpdateSystemIndex = PlayerLoopHelper.FindLoopSystemIndex(typeof(PostLateUpdate), subSystemList);

            ref var timeUpdateSystem = ref subSystemList[timeUpdateSystemIndex];
            ref var initializationSystem = ref subSystemList[initializationSystemIndex];
            ref var earlyUpdateSystem = ref subSystemList[earlyUpdateSystemIndex];
            ref var fixedUpdateSystem = ref subSystemList[fixedUpdateSystemIndex];
            ref var updateSystem = ref subSystemList[updateSystemIndex];
            ref var lateUpdateSystem = ref subSystemList[lateUpdateSystemIndex];
            ref var preLateUpdateSystem = ref subSystemList[preLateUpdateSystemIndex];
            ref var postLateUpdateSystem = ref subSystemList[postLateUpdateSystemIndex];

            var preTimeUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreTimeUpdate>(_loopRunners[(int)LoopTiming.PreTimeUpdate].Run);

            var postTimeUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostTimeUpdate>(_loopRunners[(int)LoopTiming.PostTimeUpdate].Run);

            var preInitializationSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreInitialization>(_loopRunners[(int)LoopTiming.PreInitialization].Run);

            var postInitializationSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostInitialization>(_loopRunners[(int)LoopTiming.PostInitialization].Run);

            var preStartupSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreStartup>(_loopRunners[(int)LoopTiming.PreStartup].Run);

            var postStartupSystem =
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostStartup>(_loopRunners[(int)LoopTiming.PostStartup].Run);

            var preBehaviourFixedUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreBehaviourFixedUpdate>(_loopRunners[(int)LoopTiming.PreBehaviourFixedUpdate].Run);

            var postBehaviourFixedUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostBehaviourFixedUpdate>(_loopRunners[(int)LoopTiming.PostBehaviourFixedUpdate].Run);

            var preBehaviourUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreBehaviourUpdate>(_loopRunners[(int)LoopTiming.PreBehaviourUpdate].Run);

            var postBehaviourUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostBehaviourUpdate>(_loopRunners[(int)LoopTiming.PostBehaviourUpdate].Run);

            var preDelayedTasksSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreDelayedTasks>(_loopRunners[(int)LoopTiming.PreDelayedTasks].Run);

            var postDelayedTasksSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostDelayedTasks>(_loopRunners[(int)LoopTiming.PostDelayedTasks].Run);

            var preBehaviourLateUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreBehaviourLateUpdate>(_loopRunners[(int)LoopTiming.PreBehaviourLateUpdate].Run);

            var postBehaviourLateUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostBehaviourLateUpdate>(_loopRunners[(int)LoopTiming.PostBehaviourLateUpdate].Run);

            var prePhysicsFixedUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPrePhysicsFixedUpdate>(_loopRunners[(int)LoopTiming.PrePhysicsFixedUpdate].Run);

            var postPhysicsFixedUpdateSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostPhysicsFixedUpdate>(_loopRunners[(int)LoopTiming.PostPhysicsFixedUpdate].Run);

            var preUpdateAllRenderersSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreUpdateAllRenderers>(_loopRunners[(int)LoopTiming.PreUpdateAllRenderers].Run);

            var postUpdateAllRenderersSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostUpdateAllRenderers>(_loopRunners[(int)LoopTiming.PostUpdateAllRenderers].Run);

            var preUpdateAllSkinnedMeshesSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreUpdateAllSkinnedMeshes>(_loopRunners[(int)LoopTiming.PreUpdateAllSkinnedMeshes].Run);

            var postUpdateAllSkinnedMeshesSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostUpdateAllSkinnedMeshes>(_loopRunners[(int)LoopTiming.PostUpdateAllSkinnedMeshes].Run);

            var preDirectorUpdateAnimationBeginSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreDirectorUpdateAnimationBegin>(_loopRunners[(int)LoopTiming.PreDirectorUpdateAnimationBegin].Run);

            var postDirectorUpdateAnimationEndSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostDirectorUpdateAnimationEnd>(_loopRunners[(int)LoopTiming.PostDirectorUpdateAnimationEnd].Run);

            var prePlayerUpdateCanvasesSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPrePlayerUpdateCanvases>(_loopRunners[(int)LoopTiming.PrePlayerUpdateCanvases].Run);

            var postPlayerUpdateCanvasesSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostPlayerUpdateCanvases>(_loopRunners[(int)LoopTiming.PostPlayerUpdateCanvases].Run);

            var preUpdateAudioSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPreUpdateAudio>(_loopRunners[(int)LoopTiming.PreUpdateAudio].Run);

            var postUpdateAudioSystem = 
                PlayerLoopHelper.CreateLoopSystem<PlayerLooperPostUpdateAudio>(_loopRunners[(int)LoopTiming.PostUpdateAudio].Run);

            PlayerLoopHelper.InsertSubSystem(ref timeUpdateSystem, 0, ref preTimeUpdateSystem);
            PlayerLoopHelper.AppendSubSystem(ref timeUpdateSystem, ref postTimeUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref initializationSystem, 0, ref preInitializationSystem);
            PlayerLoopHelper.AppendSubSystem(ref initializationSystem, ref postInitializationSystem);

            PlayerLoopHelper.InsertSubSystem(ref earlyUpdateSystem, typeof(EarlyUpdate.ScriptRunDelayedStartupFrame), PlayerLoopHelper.InsertPosition.Before, ref preStartupSystem);
            PlayerLoopHelper.InsertSubSystem(ref earlyUpdateSystem, typeof(EarlyUpdate.ScriptRunDelayedStartupFrame), PlayerLoopHelper.InsertPosition.After, ref postStartupSystem);

            PlayerLoopHelper.InsertSubSystem(ref fixedUpdateSystem, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), PlayerLoopHelper.InsertPosition.Before, ref preBehaviourFixedUpdateSystem);
            PlayerLoopHelper.InsertSubSystem(ref fixedUpdateSystem, typeof(FixedUpdate.ScriptRunBehaviourFixedUpdate), PlayerLoopHelper.InsertPosition.After, ref postBehaviourFixedUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref updateSystem, typeof(Update.ScriptRunBehaviourUpdate), PlayerLoopHelper.InsertPosition.Before, ref preBehaviourUpdateSystem);
            PlayerLoopHelper.InsertSubSystem(ref updateSystem, typeof(Update.ScriptRunBehaviourUpdate), PlayerLoopHelper.InsertPosition.After, ref postBehaviourUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref updateSystem, typeof(Update.ScriptRunDelayedTasks), PlayerLoopHelper.InsertPosition.Before, ref preDelayedTasksSystem);
            PlayerLoopHelper.InsertSubSystem(ref updateSystem, typeof(Update.ScriptRunDelayedTasks), PlayerLoopHelper.InsertPosition.After, ref postDelayedTasksSystem);

            PlayerLoopHelper.InsertSubSystem(ref preLateUpdateSystem, typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), PlayerLoopHelper.InsertPosition.Before, ref preBehaviourLateUpdateSystem);
            PlayerLoopHelper.InsertSubSystem(ref preLateUpdateSystem, typeof(PreLateUpdate.ScriptRunBehaviourLateUpdate), PlayerLoopHelper.InsertPosition.After, ref postBehaviourLateUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref fixedUpdateSystem, typeof(FixedUpdate.PhysicsFixedUpdate), PlayerLoopHelper.InsertPosition.Before, ref prePhysicsFixedUpdateSystem);
            PlayerLoopHelper.InsertSubSystem(ref fixedUpdateSystem, typeof(FixedUpdate.PhysicsFixedUpdate), PlayerLoopHelper.InsertPosition.After, ref postPhysicsFixedUpdateSystem);

            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.UpdateAllRenderers), PlayerLoopHelper.InsertPosition.Before, ref preUpdateAllRenderersSystem);
            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.UpdateAllRenderers), PlayerLoopHelper.InsertPosition.After, ref postUpdateAllRenderersSystem);
            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.UpdateAllSkinnedMeshes), PlayerLoopHelper.InsertPosition.Before, ref preUpdateAllSkinnedMeshesSystem);
            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.UpdateAllSkinnedMeshes), PlayerLoopHelper.InsertPosition.After, ref postUpdateAllSkinnedMeshesSystem);

            PlayerLoopHelper.InsertSubSystem(ref preLateUpdateSystem, typeof(PreLateUpdate.DirectorUpdateAnimationBegin), PlayerLoopHelper.InsertPosition.Before, ref preDirectorUpdateAnimationBeginSystem);
            PlayerLoopHelper.InsertSubSystem(ref preLateUpdateSystem, typeof(PreLateUpdate.DirectorUpdateAnimationEnd), PlayerLoopHelper.InsertPosition.After, ref postDirectorUpdateAnimationEndSystem);

            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.PlayerUpdateCanvases), PlayerLoopHelper.InsertPosition.Before, ref prePlayerUpdateCanvasesSystem);
            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.PlayerUpdateCanvases), PlayerLoopHelper.InsertPosition.After, ref postPlayerUpdateCanvasesSystem);

            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.UpdateAudio), PlayerLoopHelper.InsertPosition.Before, ref preUpdateAudioSystem);
            PlayerLoopHelper.InsertSubSystem(ref postLateUpdateSystem, typeof(PostLateUpdate.UpdateAudio), PlayerLoopHelper.InsertPosition.After, ref postUpdateAudioSystem);

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

        public static void Register(Action action, LoopTiming timing)
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

        public static void Register(object target, Action action, LoopTiming timing)
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
                Register(target, initializable.Initialize, LoopTiming.PreInitialization);
            }

            if (target is IPostInitializable postInitializable)
            {
                Register(target, postInitializable.PostInitialize, LoopTiming.PostInitialization);
            }

            if (target is IStartable startable)
            {
                Register(target, startable.Startup, LoopTiming.PreStartup);
            }

            if (target is IPostStartable postStartable)
            {
                Register(target, postStartable.PostStartup, LoopTiming.PostStartup);
            }

            if (target is IFixedTickable fixedTickable)
            {
                Register(target, fixedTickable.FixedTick, LoopTiming.PreBehaviourFixedUpdate);
            }

            if (target is IPostFixedTickable postFixedTickable)
            {
                Register(target, postFixedTickable.PostFixedTick, LoopTiming.PostBehaviourFixedUpdate);
            }

            if (target is ITickable Tickable)
            {
                Register(target, Tickable.Tick, LoopTiming.PreBehaviourUpdate);
            }

            if (target is IPostTickable postTickable)
            {
                Register(target, postTickable.PostTick, LoopTiming.PostBehaviourUpdate);
            }

            if (target is ILateTickable lateTickable)
            {
                Register(target, lateTickable.LateTick, LoopTiming.PreBehaviourLateUpdate);
            }

            if (target is IPostLateTickable postLateTickable)
            {
                Register(target, postLateTickable.PostLateTick, LoopTiming.PostBehaviourLateUpdate);
            }
        }

        public static void Unregister(object target, LoopTiming timing)
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

        struct PlayerLooperPreTimeUpdate {}
        struct PlayerLooperPostTimeUpdate {}
        struct PlayerLooperPreProfilerStartFrame {}
        struct PlayerLooperPreInitialization {}
        struct PlayerLooperPostInitialization {}
        struct PlayerLooperPreStartup {}
        struct PlayerLooperPostStartup {}
        struct PlayerLooperPreBehaviourFixedUpdate {}
        struct PlayerLooperPostBehaviourFixedUpdate {}
        struct PlayerLooperPreBehaviourUpdate {}
        struct PlayerLooperPostBehaviourUpdate {}
        struct PlayerLooperPreDelayedTasks {}
        struct PlayerLooperPostDelayedTasks {}
        struct PlayerLooperPreBehaviourLateUpdate {}
        struct PlayerLooperPostBehaviourLateUpdate {}
        struct PlayerLooperPrePhysicsFixedUpdate {}
        struct PlayerLooperPostPhysicsFixedUpdate {}
        struct PlayerLooperPreUpdateAllRenderers {}
        struct PlayerLooperPostUpdateAllRenderers {}
        struct PlayerLooperPreUpdateAllSkinnedMeshes {}
        struct PlayerLooperPostUpdateAllSkinnedMeshes {}
        struct PlayerLooperPreDirectorUpdateAnimationBegin {}
        struct PlayerLooperPostDirectorUpdateAnimationEnd {}
        struct PlayerLooperPrePlayerUpdateCanvases {}
        struct PlayerLooperPostPlayerUpdateCanvases {}
        struct PlayerLooperPreUpdateAudio {}
        struct PlayerLooperPostUpdateAudio {}
    }
}
