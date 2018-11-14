using System.Collections.Generic;
using System.Linq;
using UnityBulletML.Bullets;
using UnityEngine;
using UnityEngine.UI;

namespace UnityBulletMLSample
{
    public class DebugManager : MonoBehaviour
    {
        public BulletManager BulletManager;
        public BulletEmitter BulletEmitter;
        public Text CurrentPatternText;

        private int _currentPatternIndex;
        private List<string> _patternNames;

        public void Start()
        {
            UpdateCurrentPatternText();

            BulletManager.LoadPatterns();
            _patternNames = BulletManager.GetPatternDictionary().Keys.ToList();

            _currentPatternIndex = 0;
        }

        private void UpdateCurrentPatternText()
        {
            if (BulletEmitter.PatternFile)
                CurrentPatternText.text = BulletEmitter.PatternFile.name;
        }

        public void NextPattern()
        {
            _currentPatternIndex++;

            if (_currentPatternIndex >= _patternNames.Count)
                _currentPatternIndex = 0;

            BulletManager.Clear();
            BulletEmitter.SetPattern(BulletManager.GetPattern(_patternNames[_currentPatternIndex]));

            UpdateCurrentPatternText();
        }

        public void PreviousPattern()
        {
            _currentPatternIndex--;

            if (_currentPatternIndex < 0)
                _currentPatternIndex = _patternNames.Count - 1;

            BulletManager.Clear();
            BulletEmitter.SetPattern(BulletManager.GetPattern(_patternNames[_currentPatternIndex]));
            UpdateCurrentPatternText();
        }
    }
}