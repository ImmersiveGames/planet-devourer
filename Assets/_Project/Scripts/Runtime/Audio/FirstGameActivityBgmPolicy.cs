namespace Project._Project.Scripts.Runtime.Audio
{
    /// <summary>
    /// FIRSTGAME-specific BGM policy for Activity-scoped music decisions.
    /// This is intentionally consumer-side and does not extend the framework package.
    /// </summary>
    public enum FirstGameActivityBgmPolicy
    {
        UseOwnOrRoute = 0,
        UseOwnOrKeepPreviousActivity = 1,
        Stop = 2
    }
}
