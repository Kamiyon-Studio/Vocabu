using UnityEngine;

namespace Events.SoundSystem {
    public readonly struct Evt_OnMasterVolumeChange {
        public readonly float masterVolume;

        public Evt_OnMasterVolumeChange(float masterVolume) {
            this.masterVolume = masterVolume;
        }
    }

    public readonly struct Evt_OnMusicVolumeChange {
        public readonly float musicVolume;

        public Evt_OnMusicVolumeChange(float musicVolume) {
            this.musicVolume = musicVolume;
        }
    }

    public readonly struct Evt_OnSFXVolumeChange {
        public readonly float sfxVolume;

        public Evt_OnSFXVolumeChange(float sfxVolume) {
            this.sfxVolume = sfxVolume;
        }
    }
}