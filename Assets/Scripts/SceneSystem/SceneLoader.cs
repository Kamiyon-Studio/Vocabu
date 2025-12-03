using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSystem {
	public class SceneLoader : MonoBehaviour {
		public static string SCENE_TO_LOAD;

        [SerializeField] private float loadingTimeDelay = 3f;

        public bool isLoading { get; private set; }
        public float progress { get; private set; }

        private void Start() {
            Time.timeScale = 1f;

            isLoading = false;
            progress = 0f;
            StartCoroutine(LoadSceneAsync());
        }

        private IEnumerator LoadSceneAsync() {
            AsyncOperation operation = SceneManager.LoadSceneAsync(SCENE_TO_LOAD);
            operation.allowSceneActivation = false;

            isLoading = true;
            while (!operation.isDone) {
                progress = Mathf.Clamp01(operation.progress / 0.9f);

                if (operation.progress >= 0.9f) {

                    yield return new WaitForSeconds(loadingTimeDelay);
                    isLoading = false;
                    progress = 1f;
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }
}