namespace FirstGame.Diagnostics
{
    public enum LifecycleCanvasScope
    {
        Scene = 0,
        Route = 1,
        Activity = 2
    }

    public enum LifecycleCanvasEventKind
    {
        Available = 0,
        Releasing = 1,
        Entered = 2,
        Exited = 3
    }
}
