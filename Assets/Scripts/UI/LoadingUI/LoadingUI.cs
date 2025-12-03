using SceneSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.LoadingSystem {
	public class LoadingUI : MonoBehaviour {

		[SerializeField] private TextMeshProUGUI loadingText;
		[SerializeField] private Slider progressBar;
		[SerializeField] private SceneLoader sceneLoader;
		[SerializeField] private float smoothSpeed = 5f;

		private float displayedProgress;

        private void Awake() {
            displayedProgress = 0f;
        }

        private void Update() {
            if (sceneLoader.isLoading) {
				displayedProgress = Mathf.Lerp(displayedProgress, sceneLoader.progress, Time.deltaTime * smoothSpeed);
                progressBar.value = displayedProgress;
				loadingText.text = (displayedProgress * 100).ToString("F0") + " %";
			}
        }
	}
}