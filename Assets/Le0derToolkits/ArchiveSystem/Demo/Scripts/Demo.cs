using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Le0der.Toolbox;

namespace Le0der.ArchiveSystem
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _sourceDataText;
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _saveTimeText;
        [SerializeField] private TextMeshProUGUI _loadTimeText;

        private bool EncryptionEnabled;
        private DemoTestData _testData = new DemoTestData();
        private IDataService _dataService = new JsonDataService();

        private long _saveTime;
        private long _loadTime;

        private void Start()
        {
            _sourceDataText.text = JsonToolkit.SerializeObject(_testData, Newtonsoft.Json.Formatting.Indented);
        }


        public void SerializeJson()
        {
            long startTime = DateTime.Now.Ticks;
            if (_dataService.SaveData("/test-data.json", _testData, EncryptionEnabled))
            {
                _saveTime = DateTime.Now.Ticks - startTime;
                var savemsTime = (float)_saveTime / TimeSpan.TicksPerMillisecond;
                _saveTimeText.SetText($"{savemsTime:N4} ms");

                startTime = DateTime.Now.Ticks;
                try
                {
                    DemoTestData data = _dataService.LoadData<DemoTestData>("/test-data.json", EncryptionEnabled);
                    _loadTime = DateTime.Now.Ticks - startTime;
                    var loadmsTime = (float)_loadTime / TimeSpan.TicksPerMillisecond;

                    _loadTimeText.SetText($"{loadmsTime:N4} ms");
                    _inputField.text = "Loaded from file: \r\n" + JsonToolkit.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not load file due to: {e.Message}; StackTrace: {e.StackTrace}");
                    _inputField.text = "<color=#ff0000>Error loading save data!</color>";
                }

            }
            else
            {
                Debug.LogError("Could not save file! Show something on the UI about it!");
                _inputField.text = "<color=#ff0000>Error saving data!</color>";
            }
        }

        #region UIFunction
        public void ToggleEncryption(bool encryptionEnabled)
        {
            EncryptionEnabled = encryptionEnabled;
        }
        #endregion
    }
}