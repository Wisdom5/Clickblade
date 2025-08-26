using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Features.Firebase.Declaration;
using Firebase.RemoteConfig;
using UnityEngine;

namespace Features.Firebase.Implementation
{
    public class RemoteConfig : IRemoteConfig
    {
        private FirebaseRemoteConfig FirebaseConfig => FirebaseRemoteConfig.DefaultInstance;

        public string WelcomeMessage => FirebaseConfig.GetValue(FirebaseRemoteConfigProvider.WELCOME_MESSAGE).StringValue;
        public float KnifeFlySpeed => FirebaseConfig.GetValue(FirebaseRemoteConfigProvider.KNIFE_FLY_SPEED).LongValue;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        internal static void Validate(IRemoteConfig config)
        {
            var properties = typeof(IRemoteConfig).GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    _ = property.GetValue(config);
                }
                catch (Exception exception)
                {
                    var key = GuessKey(property.Name);
                    if (key != null)
                    {
                        var value = FirebaseRemoteConfig.DefaultInstance.GetValue(key);
                        Debug.LogError(
                            $"RemoteConfig key '{key}' for property '{property.Name}' is invalid. Value: '{value.StringValue}' Error: {exception}");
                    }
                    else
                    {
                        Debug.LogError(
                            $"RemoteConfig property '{property.Name}' threw an exception and key could not be determined: {exception}");
                    }
                }
            }
        }

        private static string GuessKey(string propertyName)
        {
            var baseName = Regex.Replace(propertyName, "(?<!^)([A-Z])", "_$1").ToUpperInvariant();
            var provider = typeof(FirebaseRemoteConfigProvider);

            var field = provider.GetField(baseName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                field = provider.GetField(baseName + "_KEY",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            return field != null ? (string)field.GetRawConstantValue() : null;
        }
#endif
    }
}
