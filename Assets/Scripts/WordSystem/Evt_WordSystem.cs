namespace Events.WordSystem {
    public class Evt_OnWordCompleted { }
    public class Evt_TotalWordCards {
        public int wordCardCount;

        public Evt_TotalWordCards(int wordCardCount) {
            this.wordCardCount = wordCardCount;
        }
    }
}