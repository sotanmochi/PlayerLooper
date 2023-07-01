namespace UnityPlayerLooper
{
    public enum LoopTiming
    {
        PreTimeUpdate = 0,
        PostTimeUpdate = 1,

        PreInitialization = 2,
        PostInitialization = 3,

        PreStartup = 4,
        PostStartup = 5,

        PreBehaviourFixedUpdate = 6,
        PostBehaviourFixedUpdate = 7,

        PreBehaviourUpdate = 8,
        PostBehaviourUpdate = 9,

        PreDelayedTasks = 10,
        PostDelayedTasks = 11,

        PreBehaviourLateUpdate  = 12,
        PostBehaviourLateUpdate = 13,

        PrePhysicsFixedUpdate = 14,
        PostPhysicsFixedUpdate = 15,

        PreUpdateAllRenderers = 16,
        PostUpdateAllRenderers = 17,
        PreUpdateAllSkinnedMeshes = 18,
        PostUpdateAllSkinnedMeshes = 19,

        PreDirectorUpdateAnimationBegin = 20,
        PostDirectorUpdateAnimationEnd = 21,

        PrePlayerUpdateCanvases = 22,
        PostPlayerUpdateCanvases = 23,

        PreUpdateAudio = 24,
        PostUpdateAudio = 25,

        Count = 26,
    }
}
