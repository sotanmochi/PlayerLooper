# UnityPlayerLooper

A utility to integrate pure C# classes (Non-MonoBehaviour classes) into Unity's PlayerLoop.

## Getting started

```csharp
using UnityEngine;
using UnityPlayerLooper;

namespace UnityPlayerLooperSamples
{
    public class Lifecycle : MonoBehaviour
    {
        LoopCounterA _loopCounterA;
        LoopCounterB _loopCounterB;

        void Awake()
        {
            _loopCounterA = new LoopCounterA();
            _loopCounterB = new LoopCounterB();
 
            // Method registeration
            GlobalPlayerLooper.Register(_loopCounterA.Startup, PlayerLoopTiming.Startup);
            GlobalPlayerLooper.Register(_loopCounterA.Tick, PlayerLoopTiming.Update);

            // Instance registration
            GlobalPlayerLooper.Register(_loopCounterB);
        }
    }
}
```

```csharp
namespace UnityPlayerLooperSamples
{
    public class LoopCounterA
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
    }
}
```

Using interfaces for instance registration, it makes easier to register into multiple loop timings.
```csharp
using UnityPlayerLooper;

namespace UnityPlayerLooperSamples
{
    public class LoopCounterB : IStartable, IUpdatable
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
    }
}
```

## How to install
```
// manifest.json
{
  "dependencies": {
    "jp.sotanmochi.playerlooper": "https://github.com/sotanmochi/PlayerLooper.git?path=PlayerLooper/Packages/PlayerLooper",
    ...
  }
}
```

## Available interfaces

| PlayerLooper entry point                 | Timing |
|:-----------------------------------------|:----------------------------------|
| `IInitializable.Initialize()`            | Early `PlayerLoop.Initialization` |
| `IPostInitializable.PostInitialize()`    | Late `PlayerLoop.Initialization`  |
| `IStartable.Startup()`                   | Before `MonoBehaviour.Start()`    |
| `IPostStartable.PostStartup()`           | After `MonoBehaviour.Start()`     |
| `IFixedTickable.FixedTick()`             | Before `MonoBehaviour.FixedUpdate()` |
| `IPostFixedTickable.PostFixedTick()`     | After `MonoBehaviour.FixedUpdate()` |
| `ITickable.Tick()`                       | Before `MonoBehaviour.Update()` |
| `IPostTickable.PostTick()`               | After `MonoBehaviour.Update()` |
| `ILateTickable.LateTick()`               | Before `MonoBehaviour.LateUpdate()` |
| `IPostLateTickable.PostLateTick()`       | After `MonoBehaviour.LateUpdate()` |

## PlayerLoop (Unity2021.3)

```

---------- TimeUpdate ----------
***** TimeUpdate.PlayerLooperPreTimeUpdate *****
TimeUpdate.WaitForLastPresentationAndUpdateTime
***** TimeUpdate.PlayerLooperPostTimeUpdate *****
---------- Initialization ----------
***** Initialization.PlayerLooperPreProfilerStartFrame *****
Initialization.ProfilerStartFrame
***** Initialization.PlayerLooperPreInitialization *****
Initialization.UpdateCameraMotionVectors
Initialization.DirectorSampleTime
Initialization.AsyncUploadTimeSlicedUpdate
Initialization.SynchronizeInputs
Initialization.SynchronizeState
Initialization.XREarlyUpdate
***** Initialization.PlayerLooperPostInitialization *****
---------- EarlyUpdate ----------
EarlyUpdate.PollPlayerConnection
EarlyUpdate.GpuTimestamp
EarlyUpdate.AnalyticsCoreStatsUpdate
EarlyUpdate.UnityWebRequestUpdate
EarlyUpdate.ExecuteMainThreadJobs
EarlyUpdate.ProcessMouseInWindow
EarlyUpdate.ClearIntermediateRenderers
EarlyUpdate.ClearLines
EarlyUpdate.PresentBeforeUpdate
EarlyUpdate.ResetFrameStatsAfterPresent
EarlyUpdate.UpdateAsyncReadbackManager
EarlyUpdate.UpdateStreamingManager
EarlyUpdate.UpdateTextureStreamingManager
EarlyUpdate.UpdatePreloading
EarlyUpdate.RendererNotifyInvisible
EarlyUpdate.PlayerCleanupCachedData
EarlyUpdate.UpdateMainGameViewRect
EarlyUpdate.UpdateCanvasRectTransform
EarlyUpdate.XRUpdate
EarlyUpdate.UpdateInputManager
EarlyUpdate.ProcessRemoteInput
***** EarlyUpdate.PlayerLooperPreStartup *****
EarlyUpdate.ScriptRunDelayedStartupFrame
***** EarlyUpdate.PlayerLooperPostStartup *****
EarlyUpdate.UpdateKinect
EarlyUpdate.DeliverIosPlatformEvents
EarlyUpdate.ARCoreUpdate
EarlyUpdate.DispatchEventQueueEvents
EarlyUpdate.PhysicsResetInterpolatedTransformPosition
EarlyUpdate.SpriteAtlasManagerUpdate
EarlyUpdate.PerformanceAnalyticsUpdate
---------- FixedUpdate ----------
FixedUpdate.ClearLines
FixedUpdate.NewInputFixedUpdate
FixedUpdate.DirectorFixedSampleTime
FixedUpdate.AudioFixedUpdate
***** FixedUpdate.PlayerLooperPreBehaviourFixedUpdate *****
FixedUpdate.ScriptRunBehaviourFixedUpdate
***** FixedUpdate.PlayerLooperPostBehaviourFixedUpdate *****
FixedUpdate.DirectorFixedUpdate
FixedUpdate.LegacyFixedAnimationUpdate
FixedUpdate.XRFixedUpdate
***** FixedUpdate.PlayerLooperPrePhysicsFixedUpdate *****
FixedUpdate.PhysicsFixedUpdate
***** FixedUpdate.PlayerLooperPostPhysicsFixedUpdate *****
FixedUpdate.Physics2DFixedUpdate
FixedUpdate.PhysicsClothFixedUpdate
FixedUpdate.DirectorFixedUpdatePostPhysics
FixedUpdate.ScriptRunDelayedFixedFrameRate
---------- PreUpdate ----------
PreUpdate.PhysicsUpdate
PreUpdate.Physics2DUpdate
PreUpdate.CheckTexFieldInput
PreUpdate.IMGUISendQueuedEvents
PreUpdate.NewInputUpdate
PreUpdate.SendMouseEvents
PreUpdate.AIUpdate
PreUpdate.WindUpdate
PreUpdate.UpdateVideo
---------- Update ----------
***** Update.PlayerLooperPreBehaviourUpdate *****
Update.ScriptRunBehaviourUpdate
***** Update.PlayerLooperPostBehaviourUpdate *****
Update.ScriptRunDelayedDynamicFrameRate
***** Update.PlayerLooperPreDelayedTasks *****
Update.ScriptRunDelayedTasks
***** Update.PlayerLooperPostDelayedTasks *****
Update.DirectorUpdate
---------- PreLateUpdate ----------
PreLateUpdate.AIUpdatePostScript
***** PreLateUpdate.PlayerLooperPreDirectorUpdateAnimationBegin *****
PreLateUpdate.DirectorUpdateAnimationBegin
PreLateUpdate.LegacyAnimationUpdate
PreLateUpdate.DirectorUpdateAnimationEnd
***** PreLateUpdate.PlayerLooperPostDirectorUpdateAnimationEnd *****
PreLateUpdate.DirectorDeferredEvaluate
PreLateUpdate.UIElementsUpdatePanels
PreLateUpdate.EndGraphicsJobsAfterScriptUpdate
PreLateUpdate.ConstraintManagerUpdate
PreLateUpdate.ParticleSystemBeginUpdateAll
PreLateUpdate.Physics2DLateUpdate
***** PreLateUpdate.PlayerLooperPreBehaviourLateUpdate *****
PreLateUpdate.ScriptRunBehaviourLateUpdate
***** PreLateUpdate.PlayerLooperPostBehaviourLateUpdate *****
---------- PostLateUpdate ----------
PostLateUpdate.PlayerSendFrameStarted
PostLateUpdate.DirectorLateUpdate
PostLateUpdate.ScriptRunDelayedDynamicFrameRate
PostLateUpdate.PhysicsSkinnedClothBeginUpdate
PostLateUpdate.UpdateRectTransform
PostLateUpdate.PlayerUpdateCanvases
PostLateUpdate.UpdateAudio
PostLateUpdate.VFXUpdate
PostLateUpdate.ParticleSystemEndUpdateAll
PostLateUpdate.EndGraphicsJobsAfterScriptLateUpdate
PostLateUpdate.UpdateCustomRenderTextures
PostLateUpdate.XRPostLateUpdate
***** PostLateUpdate.PlayerLooperPreUpdateAllRenderers *****
PostLateUpdate.UpdateAllRenderers
***** PostLateUpdate.PlayerLooperPostUpdateAllRenderers *****
PostLateUpdate.UpdateLightProbeProxyVolumes
PostLateUpdate.EnlightenRuntimeUpdate
***** PostLateUpdate.PlayerLooperPreUpdateAllSkinnedMeshes *****
PostLateUpdate.UpdateAllSkinnedMeshes
***** PostLateUpdate.PlayerLooperPostUpdateAllSkinnedMeshes *****
PostLateUpdate.ProcessWebSendMessages
PostLateUpdate.SortingGroupsUpdate
PostLateUpdate.UpdateVideoTextures
PostLateUpdate.UpdateVideo
PostLateUpdate.DirectorRenderImage
PostLateUpdate.PlayerEmitCanvasGeometry
PostLateUpdate.PhysicsSkinnedClothFinishUpdate
PostLateUpdate.FinishFrameRendering
***** PostLateUpdate.PlayerLooperPostFinishFrameRendering *****
PostLateUpdate.BatchModeUpdate
PostLateUpdate.PlayerSendFrameComplete
PostLateUpdate.UpdateCaptureScreenshot
PostLateUpdate.PresentAfterDraw
PostLateUpdate.ClearImmediateRenderers
PostLateUpdate.PlayerSendFramePostPresent
PostLateUpdate.UpdateResolution
PostLateUpdate.InputEndFrame
PostLateUpdate.TriggerEndOfFrameCallbacks
PostLateUpdate.GUIClearEvents
PostLateUpdate.ShaderHandleErrors
PostLateUpdate.ResetInputAxis
PostLateUpdate.ThreadedLoadingDebug
PostLateUpdate.ProfilerSynchronizeStats
PostLateUpdate.MemoryFrameMaintenance
PostLateUpdate.ExecuteGameCenterCallbacks
PostLateUpdate.XRPreEndFrame
PostLateUpdate.ProfilerEndFrame
***** PostLateUpdate.PlayerLooperPostProfilerEndFrame *****
PostLateUpdate.GraphicsWarmupPreloadedShaders
```

## License
This library is licensed under [CC0](https://creativecommons.org/publicdomain/zero/1.0/).
