namespace UnityPlayerLooper
{
    public interface IInitializable { void Initialize(); }
    public interface IPostInitializable { void PostInitialize(); }
    public interface IStartable { void Startup(); }
    public interface IPostStartable { void PostStartup(); }
    public interface IFixedTickable { void FixedTick(); }
    public interface IPostFixedTickable { void PostFixedTick(); }
    public interface ITickable { void Tick(); }
    public interface IPostTickable { void PostTick(); }
    public interface ILateTickable { void LateTick(); }
    public interface IPostLateTickable { void PostLateTick(); }
}