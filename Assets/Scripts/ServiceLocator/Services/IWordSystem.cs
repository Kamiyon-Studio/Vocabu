using System.Collections.Generic;
using UnityEngine;
using WordSystem;

namespace ServiceLocator.Services {
	public interface IWordSystem {
        public List<WordData> GetNextWords(string difficulty, int count);
        public List<WordData> GetAllWords(bool alphabeticalOrder = false);
    } 
}
