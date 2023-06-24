# PlayerLooper

A utility to integrate pure C# classes (Non-MonoBehaviour classes) into Unity's PlayerLoop.

## Getting started

```csharp
using UnityEngine;
using PlayerLooper;

namespace PlayerLooperSamples
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
namespace PlayerLooperSamples
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
using PlayerLooper;

namespace PlayerLooperSamples
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
| `IStartable.Start()`                     | Before `MonoBehaviour.Start()`    |
| `IPostStartable.PostStart()`             | After `MonoBehaviour.Start()`     |
| `IFixedUpdatable.FixedUpdate()`          | Before `MonoBehaviour.FixedUpdate()` |
| `IPostFixedUpdatable.PostFixedUpdate()`  | After `MonoBehaviour.FixedUpdate()` |
| `IUpdatable.Update()`                    | Before `MonoBehaviour.Update()` |
| `IPostUpdatable.PostUpdate()`            | After `MonoBehaviour.Update()` |
| `ILateUpdatable.LateUpdate()`            | Before `MonoBehaviour.LateUpdate()` |
| `IPostLateUpdatable.PostLateUpdate()`    | After `MonoBehaviour.LateUpdate()` |

## PlayerLoop (Unity2019.4)

```
------Initialization------
** PlayerLooperInitialization **
Initialization.PlayerUpdateTime
Initialization.AsyncUploadTimeSlicedUpdate
Initialization.SynchronizeInputs
Initialization.SynchronizeState
Initialization.XREarlyUpdate
** PlayerLooperPostInitialization **
------EarlyUpdate------
EarlyUpdate.PollPlayerConnection
EarlyUpdate.ProfilerStartFrame
EarlyUpdate.GpuTimestamp
EarlyUpdate.UnityConnectClientUpdate
EarlyUpdate.CloudWebServicesUpdate
EarlyUpdate.UnityWebRequestUpdate
EarlyUpdate.ExecuteMainThreadJobs
EarlyUpdate.ProcessMouseInWindow
EarlyUpdate.ClearIntermediateRenderers
EarlyUpdate.ClearLines
EarlyUpdate.PresentBeforeUpdate
EarlyUpdate.ResetFrameStatsAfterPresent
EarlyUpdate.UpdateAllUnityWebStreams
EarlyUpdate.UpdateAsyncReadbackManager
EarlyUpdate.UpdateTextureStreamingManager
EarlyUpdate.UpdatePreloading
EarlyUpdate.RendererNotifyInvisible
EarlyUpdate.PlayerCleanupCachedData
EarlyUpdate.UpdateMainGameViewRect
EarlyUpdate.UpdateCanvasRectTransform
EarlyUpdate.UpdateInputManager
EarlyUpdate.ProcessRemoteInput
EarlyUpdate.XRUpdate
EarlyUpdate.TangoUpdate
** PlayerLooperStart **
EarlyUpdate.ScriptRunDelayedStartupFrame
EarlyUpdate.UpdateKinect
EarlyUpdate.DeliverIosPlatformEvents
EarlyUpdate.DispatchEventQueueEvents
EarlyUpdate.DirectorSampleTime
EarlyUpdate.PhysicsResetInterpolatedTransformPosition
EarlyUpdate.NewInputBeginFrame
EarlyUpdate.SpriteAtlasManagerUpdate
EarlyUpdate.PerformanceAnalyticsUpdate
** PlayerLooperPostStart **
------FixedUpdate------
FixedUpdate.ClearLines
FixedUpdate.NewInputEndFixedUpdate
FixedUpdate.DirectorFixedSampleTime
FixedUpdate.AudioFixedUpdate
** PlayerLooperFixedUpdate **
FixedUpdate.ScriptRunBehaviourFixedUpdate
FixedUpdate.DirectorFixedUpdate
FixedUpdate.LegacyFixedAnimationUpdate
FixedUpdate.XRFixedUpdate
FixedUpdate.PhysicsFixedUpdate
FixedUpdate.Physics2DFixedUpdate
FixedUpdate.DirectorFixedUpdatePostPhysics
FixedUpdate.ScriptRunDelayedFixedFrameRate
FixedUpdate.ScriptRunDelayedTasks
FixedUpdate.NewInputBeginFixedUpdate
** PlayerLooperPostFixedUpdate **
------PreUpdate------
PreUpdate.PhysicsUpdate
PreUpdate.Physics2DUpdate
PreUpdate.CheckTexFieldInput
PreUpdate.IMGUISendQueuedEvents
PreUpdate.NewInputUpdate
PreUpdate.SendMouseEvents
PreUpdate.AIUpdate
PreUpdate.WindUpdate
PreUpdate.UpdateVideo
------Update------
** PlayerLooperUpdate **
Update.ScriptRunBehaviourUpdate
Update.ScriptRunDelayedDynamicFrameRate
Update.DirectorUpdate
** PlayerLooperPostUpdate **
------PreLateUpdate------
PreLateUpdate.AIUpdatePostScript
PreLateUpdate.DirectorUpdateAnimationBegin
PreLateUpdate.LegacyAnimationUpdate
PreLateUpdate.DirectorUpdateAnimationEnd
PreLateUpdate.DirectorDeferredEvaluate
PreLateUpdate.UpdateNetworkManager
PreLateUpdate.UpdateMasterServerInterface
PreLateUpdate.UNetUpdate
PreLateUpdate.EndGraphicsJobsLate
PreLateUpdate.ParticleSystemBeginUpdateAll
** PlayerLooperLateUpdate **
PreLateUpdate.ScriptRunBehaviourLateUpdate
PreLateUpdate.ConstraintManagerUpdate
** PlayerLooperPostLateUpdate **
------PostLateUpdate------
PostLateUpdate.PlayerSendFrameStarted
PostLateUpdate.DirectorLateUpdate
PostLateUpdate.ScriptRunDelayedDynamicFrameRate
PostLateUpdate.PhysicsSkinnedClothBeginUpdate
PostLateUpdate.UpdateCanvasRectTransform
PostLateUpdate.PlayerUpdateCanvases
PostLateUpdate.UpdateAudio
PostLateUpdate.ParticlesLegacyUpdateAllParticleSystems
PostLateUpdate.ParticleSystemEndUpdateAll
PostLateUpdate.UpdateCustomRenderTextures
PostLateUpdate.UpdateAllRenderers
PostLateUpdate.EnlightenRuntimeUpdate
PostLateUpdate.UpdateAllSkinnedMeshes
PostLateUpdate.ProcessWebSendMessages
PostLateUpdate.SortingGroupsUpdate
PostLateUpdate.UpdateVideoTextures
PostLateUpdate.UpdateVideo
PostLateUpdate.DirectorRenderImage
PostLateUpdate.PlayerEmitCanvasGeometry
PostLateUpdate.PhysicsSkinnedClothFinishUpdate
PostLateUpdate.FinishFrameRendering
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
PostLateUpdate.ProfilerEndFrame
```

## License
This library is licensed under [CC0](https://creativecommons.org/publicdomain/zero/1.0/).
