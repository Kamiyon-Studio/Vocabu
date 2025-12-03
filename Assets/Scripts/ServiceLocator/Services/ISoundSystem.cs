using UnityEngine;

namespace ServiceLocator.Services {
	public interface ISoundSystem {
        public float GetMasterVolume();
        public float GetMusicVolume();
        public float GetSFXVolume();

        public void SetMasterVolume(float volume);
        public void SetMusicVolume(float volume);
        public void SetSFXVolume(float volume);

    }
}