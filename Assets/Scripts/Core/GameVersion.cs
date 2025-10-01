namespace SanicballCore
{
    public class GameVersion
    {
        //The current version as a float, for checking which version is newest
        public const float AS_FLOAT = 1f;

        //To differentiate between testing builds and release builds
        public const bool IS_TESTING = false;

        //As a string, for displaying on the UI
        public const string AS_STRING = "REBORN";

        //Something stupid, usually unique for every version
        public const string TAGLINE = "The Copyrighted Racing Masterpiece\nRebirthed, Reborn and Revived";
    }
}