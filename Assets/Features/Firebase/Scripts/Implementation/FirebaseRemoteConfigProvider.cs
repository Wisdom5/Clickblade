using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
using Firebase.RemoteConfig;
using UnityEngine;
using Utils;

namespace Features.Firebase.Implementation
{
    public class FirebaseRemoteConfigProvider : IFirebaseRemoteConfigProvider
    {
        public event Action Updated;

        private const string REMOTE_CONFIG_EXPIRATION_MINUTES = "remote_config_expiration_minutes";
        internal const string WELCOME_MESSAGE = "welcome_message";

        public IRemoteConfig Config { get; } =
#if UNITY_EDITOR
            new RemoteConfigMock();
#else
            new RemoteConfig();
#endif

        internal static Dictionary<string, object> BuildDefaultsDictionary()
        {
            return new Dictionary<string, object>
            {
                { REMOTE_CONFIG_EXPIRATION_MINUTES, 60 },
                { WELCOME_MESSAGE, "WelcomeMessage hello from FirebaseRemoteConfigProvider!" }
            };
        }

        private static void ValidateDefaultKeys(Dictionary<string, object> defaults)
        {
            var fields = typeof(FirebaseRemoteConfigProvider)
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            foreach (var field in fields)
            {
                var key = (string)field.GetRawConstantValue();
                if (!defaults.ContainsKey(key))
                {
                    Debug.LogError($"[FirebaseRemoteConfigProvider] Default value for key '{field.Name}' is missing");
                }
            }
        }

        public async UniTask Initialize(CancellationToken cancellationToken)
        {
            Debug.Log($"{nameof(FirebaseRemoteConfigProvider)} initialization started");

            await InitializeFirebaseSdkAsync(cancellationToken).SuppressCancellationThrow();

            Debug.Log($"{nameof(FirebaseRemoteConfigProvider)} initialization ended");
        }

        private async UniTask InitializeFirebaseSdkAsync(CancellationToken cancellationToken)
        {
            var defaults = BuildDefaultsDictionary();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            ValidateDefaultKeys(defaults);
#endif

            try
            {
                await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults)
                    .AsUniTask()
                    .AttachExternalCancellation(cancellationToken)
                    .WithTimeout(1000);

                Debug.Log($"{nameof(FirebaseRemoteConfigProvider)} defaults set");

                await FetchRemoteDataAsync(cancellationToken);

                Debug.Log($"{nameof(FirebaseRemoteConfigProvider)} remote data fetched");
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            RemoteConfig.Validate(Config);
#endif
        }

        private async UniTask FetchRemoteDataAsync(CancellationToken cancellationToken)
        {
            var expirationMinutes = 0;

#if UNITY_EDITOR
            var expirationConfigValue = FirebaseRemoteConfig.DefaultInstance.GetValue(REMOTE_CONFIG_EXPIRATION_MINUTES);

            try
            {
                expirationMinutes = (int)expirationConfigValue.LongValue;
            }
            catch
            {
                Debug.LogError("[FirebaseRemoteConfigProvider] Can't parse RemoteConfig expiration time with value: " +
                               expirationConfigValue.StringValue);
            }
#endif

            try
            {
                Debug.Log($"[FirebaseRemoteConfigProvider] Trying to fetch the {nameof(FirebaseRemoteConfig)}");

                var settings = new ConfigSettings
                {
                    FetchTimeoutInMilliseconds = 1000,
                    MinimumFetchIntervalInMilliseconds =
                        Convert.ToUInt64(TimeSpan.FromMinutes(expirationMinutes).TotalMilliseconds)
                };

                await FirebaseRemoteConfig.DefaultInstance.SetConfigSettingsAsync(settings);

                await FirebaseRemoteConfig.DefaultInstance.FetchAsync()
                    .AsUniTask()
                    .AttachExternalCancellation(cancellationToken);

                Debug.Log($"[FirebaseRemoteConfigProvider] Fetched. Trying to Activate {nameof(FirebaseRemoteConfig)}");

                await FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .AsUniTask()
                    .AttachExternalCancellation(cancellationToken);

                Debug.Log($"{nameof(FirebaseRemoteConfig)} Activated");
            }
            catch (Exception exception)
            {
                Debug.LogError($"[FirebaseRemoteConfigProvider] Can't fetch the Firebase RemoteConfig: {exception}");
            }

            UniTask.ReturnToMainThread();

            Updated?.Invoke();
        }
    }
}
