using System;
using Cysharp.Threading.Tasks;
using Features.Firebase.Declaration;
using Firebase;
using UnityEngine;

namespace Features.Firebase.Implementation
{
    public class FirebaseService : IFirebaseService
    {
        public async UniTask InitializeAsync()
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status != DependencyStatus.Available)
            {
                throw new Exception($"Could not resolve Firebase dependencies: {status}");
            }

            Debug.Log("Firebase initialized successfully.");
        }
    }
}
