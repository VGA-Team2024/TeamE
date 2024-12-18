public class BuildState
{
    const string _hash = "b873373c-9280-4d57-9f9f-6bae57c95e9f";
    public const string TeamID = "TeamE2024";

    public static string BuildHash
    {
        get
        {
#if UNITY_EDITOR
            return "UNITY_EDITOR";
#else
            return _hash;
#endif
        }
    }
};