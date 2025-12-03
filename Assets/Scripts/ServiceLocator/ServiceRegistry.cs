using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A thread-safe, static service locator for providing global access to services.
/// This helps decouple systems and avoids singleton patterns for every manager.
/// It is persistent across scene loads.
/// </summary>
/// 
/// <example>
/// <code>
/// // Registration (e.g., in a manager's Awake)
/// ServiceLocator.Register<IGameManager>(this);
///
/// // Retrieval (e.g., in another script)
/// if (ServiceLocator.TryGet(out IGameManager gameManager))
/// gameManager.LoadLevel("Level1");
/// </code>
/// </example>
namespace ServiceLocator {
    public static class ServiceRegistry {
        // Use ConcurrentDictionary for thread-safe operations without explicit locking.
        private static readonly ConcurrentDictionary<Type, object> services = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Registers a service with the locator. This operation is thread-safe.
        /// </summary>
        /// <typeparam name="T">The interface or class type of the service.</typeparam>
        /// <param name="service">The instance of the service to register.</param>
        public static void Register<T>(T service) where T : class {
            var type = typeof(T);
            if (services.TryAdd(type, service)) {
                Debug.Log($"[ServiceLocator] Service of type {type.Name} registered.");
            }
            else {
                Debug.LogError($"[ServiceLocator] Service of type {type.Name} is already registered.");
            }
        }

        /// <summary>
        /// Unregisters a service from the locator. This operation is thread-safe.
        /// </summary>
        /// <typeparam name="T">The type of the service to unregister.</typeparam>
        public static void Unregister<T>(T service) where T : class {
            var type = typeof(T);

            if (service == null) {
                Debug.LogWarning($"[ServiceLocator] Attempted to unregister a null service of type {type.Name}.");
                return;
            }

            var collection = (ICollection<KeyValuePair<Type, object>>)services;
            if (collection.Remove(new KeyValuePair<Type, object>(type, service))) {
                Debug.Log($"[ServiceLocator] Service of type {type.Name} unregistered.");
            } else {
                if (services.ContainsKey(type)) {
                    Debug.LogWarning($"[ServiceLocator] Attempted to unregister a service of type {type.Name}, but a different instance was registered.");
                }
                else {
                    Debug.LogWarning($"[ServiceLocator] Service of type {type.Name} not registered.");
                }
            }
        }

        /// <summary>
        /// Tries to retrieve a service from the locator. This operation is thread-safe.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <param name="service">The retrieved service, or default if not found.</param>
        /// <returns>True if the service was found, otherwise false.</returns>
        public static bool TryGet<T>(out T service) where T : class {
            var type = typeof(T);
            if (services.TryGetValue(type, out var obj) && obj is T typedService) {
                service = typedService;
                return true;
            }

            service = default(T);
            return false;
        }

        /// <summary>
        /// Retrieves a service from the locator. Logs a warning and returns null if the service is not found.
        /// This method is thread-safe.
        /// </summary>
        /// <typeparam name="T">The type of the service to retrieve.</typeparam>
        /// <returns>The service instance, or null if not found.</returns>
        public static T Get<T>() where T : class {
            if (TryGet<T>(out var service))
                return service;

            Debug.LogWarning($"[ServiceLocator] Service of type {typeof(T).Name} not registered.");
            return null;
        }


        /// <summary>
        /// Checks if a service of a given type is registered. This operation is thread-safe.
        /// </summary>
        /// <typeparam name="T">The type of the service to check.</typeparam>
        /// <returns>True if the service is registered, false otherwise.</returns>
        public static bool IsRegistered<T>() where T : class {
            return services.ContainsKey(typeof(T));
        }

        /// <summary>
        /// Clears all registered services. This is particularly useful for editor scripting and testing.
        /// It's automatically called when exiting play mode in the editor.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Clear() {
            services.Clear();
#if UNITY_EDITOR
            // This log is helpful for development but can be removed in production builds if desired.
            Debug.Log("[ServiceLocator] All services cleared.");
#endif
        }
    }
}
