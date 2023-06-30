namespace PlayerLooper
{
    public enum LoopTiming
    {
        PreTimeUpdate = 0,
        PostTimeUpdate = 1,

        PreProfilerStartFrame = 2,

        PreInitialization = 3,
        PostInitialization = 4,

        PreStartup = 5,
        PostStartup = 6,

        PreBehaviourFixedUpdate = 7,
        PostBehaviourFixedUpdate = 8,

        PreBehaviourUpdate = 9,
        PostBehaviourUpdate = 10,

        PreDelayedTasks = 11,
        PostDelayedTasks = 12,

        PreBehaviourLateUpdate  = 13,
        PostBehaviourLateUpdate = 14,

        PrePhysicsFixedUpdate = 15,
        PostPhysicsFixedUpdate = 16,

        PreUpdateAllRenderers = 17,
        PostUpdateAllRenderers = 18,
        PreUpdateAllSkinnedMeshes = 19,
        PostUpdateAllSkinnedMeshes = 20,

        PreDirectorUpdateAnimationBegin = 21,
        PostDirectorUpdateAnimationEnd = 22,

        PostFinishFrameRendering = 23,

        PostProfilerEndFrame = 24,

        Count = 25,
    }
}
