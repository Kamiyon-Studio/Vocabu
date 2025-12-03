namespace Events.DropContainer {
    public class Evt_OnCorrectCardDrop {
        public int score;
        public int multiplier;
        public string wordType;

        public Evt_OnCorrectCardDrop(int score, int multiplier, string wordType) {
            this.score = score;
            this.multiplier = multiplier;
            this.wordType = wordType;
        }

    }

    public class Evt_OnIncorrectCardDrop { }
}