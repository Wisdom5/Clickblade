using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace Features.Firebase.Implementation
{
    public class RemoteConfigService
    {
        public async UniTask InitializeAsync()
        {
            var defaults = new Dictionary<string, object>
            {
                { "welcome_message", "Default hello from local config!" }
            };

            await FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);

            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
                await FirebaseRemoteConfig.DefaultInstance.ActivateAsync();

                var welcomeMessage = FirebaseRemoteConfig.DefaultInstance.GetValue("welcome_message").StringValue;
                Debug.Log($"Fetched remote config value: welcome_message = {welcomeMessage}");
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to fetch or activate remote config: {exception.Message}");
            }
        }
    }
}
