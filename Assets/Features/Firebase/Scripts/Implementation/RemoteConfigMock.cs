using Features.Firebase.Declaration;

namespace Features.Firebase.Implementation
{
    public class RemoteConfigMock : IRemoteConfig
    {
        public string WelcomeMessage => "WelcomeMessage hello from RemoteConfigMock!";
        public float KnifeFlySpeed => 20f;
    }
}
