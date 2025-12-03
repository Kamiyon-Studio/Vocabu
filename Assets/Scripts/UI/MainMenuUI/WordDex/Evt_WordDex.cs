using WordSystem;

namespace Events.WordDex {
    public class Evt_OnWordDexCardClicked {
        public WordData wordData;

        public Evt_OnWordDexCardClicked(WordData wordData) {
            this.wordData = wordData;
        }
    }
}