using GameSystem.State;

namespace Events.GameSystem {
    public class Evt_OnGameStateChange {
        public GameState gameState { get; private set; }

        public Evt_OnGameStateChange(GameState gameState) {
            this.gameState = gameState;
        }
    }

    public class Evt_OnPauseAction { }
    public class Evt_OnResumeAction { }
}
