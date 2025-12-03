namespace Events.Tutorial {
    public readonly struct Evt_OnSequenceChange {
        public readonly int sequenceNumber;

        public Evt_OnSequenceChange(int sequenceNumber) {
            this.sequenceNumber = sequenceNumber;
        }
    }

    public readonly struct Evt_OnSequenceTrigger { }
}