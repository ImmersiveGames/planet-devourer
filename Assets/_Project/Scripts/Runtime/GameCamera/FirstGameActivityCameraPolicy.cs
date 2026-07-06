namespace Project._Project.Scripts.Runtime.GameCamera
{
    /// <summary>
    /// FIRSTGAME-specific camera policy for Activity-scoped camera decisions.
    /// This remains consumer-side and does not extend the framework package.
    /// </summary>
    public enum FirstGameActivityCameraPolicy
    {
        UseOwnOrRoute = 0,
        UseOwnOrKeepPreviousActivity = 1,
        UseRoute = 2
    }
}
