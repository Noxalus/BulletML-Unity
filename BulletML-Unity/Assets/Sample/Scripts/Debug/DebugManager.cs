using System.Collections.Generic;
using System.IO;
using UnityBulletML.Bullets;
using UnityEngine;
using UnityEngine.UI;

namespace UnityBulletML
{
    public class DebugManager : MonoBehaviour
    {
        public BulletManager BulletManager;
        public BulletEmitter BulletEmitter;
        public Text CurrentPatternText;

        [SerializeField] private string _patternFolder = null;

        private List<TextAsset> _patternFiles;
        private int _currentPatternIndex;

        public void Start()
        {
            UpdateCurrentPatternText();

            GetAllPatternFiles();

            _currentPatternIndex = 0;
        }

        private void UpdateCurrentPatternText()
        {
            CurrentPatternText.text = BulletEmitter.PatternFile.name;
        }

        private void GetAllPatternFiles()
        {
            _patternFiles = new List<TextAsset>();

            var directoryInfo = new DirectoryInfo(_patternFolder);
            var filesInfo = directoryInfo.GetFiles();

            var resourcesFolder = "Resources/";
            var pathFromResourcesFolder = _patternFolder.Substring(_patternFolder.IndexOf(resourcesFolder) + resourcesFolder.Length);

            foreach (var fileInfo in filesInfo)
            {
                var fileExtension = Path.GetExtension(fileInfo.Name);

                if (fileExtension.Equals(".xml"))
                {
                    TextAsset patternFile = Resources.Load<TextAsset>(pathFromResourcesFolder + "/" + Path.GetFileNameWithoutExtension(fileInfo.Name));
                    _patternFiles.Add(patternFile);

                    Debug.Log("Found: " + fileInfo);
                }
            }
        }

        public void NextPattern()
        {
            _currentPatternIndex++;

            if (_currentPatternIndex >= _patternFiles.Count)
                _currentPatternIndex = 0;

            BulletManager.Clear();
            BulletEmitter.SetPatternFile(_patternFiles[_currentPatternIndex]);

            UpdateCurrentPatternText();
        }

        public void PreviousPattern()
        {
            _currentPatternIndex--;

            if (_currentPatternIndex < 0)
                _currentPatternIndex = _patternFiles.Count - 1;

            BulletManager.Clear();
            BulletEmitter.SetPatternFile(_patternFiles[_currentPatternIndex]);
            UpdateCurrentPatternText();
        }
    }
}