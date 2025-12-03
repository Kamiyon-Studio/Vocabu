using GameSystem.State;
using UnityEngine;

namespace ServiceLocator.Services {
	public interface IGameSystem {
		public GameState GetGameState();
        public float GetTimerDuration();
        public float GetTimer();

    }
}