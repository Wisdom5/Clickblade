using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;

namespace Features.Firebase
{
    public class FirebaseInitializer : MonoBehaviour
    {
        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(OnDependencyStatusReceived);
        }

        private void OnDependencyStatusReceived(Task<DependencyStatus> task)
        {
            try
            {
                if (!task.IsCompletedSuccessfully)
                {
                    throw new Exception("Could not resolve all Firebase dependencies", task.Exception);
                }

                var status = task.Result;
                if (status != DependencyStatus.Available)
                {
                    throw new Exception($"Could not resolve all Firebase dependencies: {status}");
                }

                Debug.Log("Firebase initialized successfully");

                InitializeRemoteConfig();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void InitializeRemoteConfig()
        {
            var defaults = new Dictionary<string, object>
            {
                { "welcome_message", "Default hello from local config!" }
            };

            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(_ =>
            {
                FetchRemoteConfig();
            });
        }

        private void FetchRemoteConfig()
        {
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero).ContinueWithOnMainThread(fetchTask =>
            {
                if (!fetchTask.IsCompletedSuccessfully)
                {
                    Debug.LogError("Failed to fetch remote config.");
                    return;
                }

                FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWithOnMainThread(_ =>
                {
                    var welcomeMessage = FirebaseRemoteConfig.DefaultInstance.GetValue("welcome_message").StringValue;
                    Debug.Log($"Fetched remote config value: welcome_message = {welcomeMessage}");
                });
            });
        }
    }
}
