namespace Events.ScoreSystem {
    /// <summary>
    /// Event for initial score
    /// </summary>
    public readonly struct Evt_OnInitialScoreChange {
        public readonly int initialScore;

        public Evt_OnInitialScoreChange(int initialScore) {
            this.initialScore = initialScore;
        }
    }

    /// <summary>
    /// Event for total score
    /// </summary>
    public readonly struct Evt_OnTotalScoreChange {
        public readonly int totalScore;

        public Evt_OnTotalScoreChange(int totalScore) {
            this.totalScore = totalScore;
        }
    }

    /// <summary>
    /// Event for multiplier
    /// </summary>
    public readonly struct Evt_OnMultiplierChange {
        public readonly float multiplier;

        public Evt_OnMultiplierChange(float multiplier) {
            this.multiplier = multiplier;
        }
    }

    public readonly struct Evt_OnCorrectAnswer { }

    public readonly struct Evt_OnIncorrectAnswer { }
}