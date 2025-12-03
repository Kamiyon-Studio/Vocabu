using UnityEngine;

namespace ServiceLocator.Services {
	public interface IScoreSystem {
        public int GetTotalScore();
        public int GetNounCount();
        public int GetVerbCount();
        public int GetAdjCount();
    }

}