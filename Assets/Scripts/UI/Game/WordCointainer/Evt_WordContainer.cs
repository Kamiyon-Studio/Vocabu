
namespace Events.WordContainer {
    public class Evt_OnWordCardDrag { }
    public class Evt_OnWordCardDrop { }

    public class Evt_OnCorrectLetterDrop {
        public int score;

        public Evt_OnCorrectLetterDrop(int score) {
            this.score = score;
        }
    }

    public readonly struct Evt_OnIncorrectLetterDrop { }
}