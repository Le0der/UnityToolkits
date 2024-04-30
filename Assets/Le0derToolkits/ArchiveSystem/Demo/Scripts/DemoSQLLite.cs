using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Le0derToolkit.Toolbox;
using System.Text;


namespace Le0derToolkit.ArchiveSystem
{
    public class DemoSQLLite : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _sourceDataText;
        [SerializeField] private TextMeshProUGUI _loadDataText;
        [SerializeField] private TextMeshProUGUI _saveTimeText;
        [SerializeField] private TextMeshProUGUI _loadTimeText;
        [SerializeField] private TMP_InputField _saveDataTimes;
        [SerializeField] private TMP_InputField _loadDataId;

        private DatabaseDataService _dataService;

        private void Start()
        {
            _dataService = new DatabaseDataService("demotest");
            _sourceDataText.text = "Sources Data Here.";
            _loadDataText.text = "Load Data Here.";
        }

        public void OnSaveClick()
        {
            float totalSaveTime = 0;

            int saveCount;
            int.TryParse(_saveDataTimes.text, out saveCount);
            if (saveCount <= 0) saveCount = 1;

            StringBuilder saveBuilder = new StringBuilder(saveCount);

            for (int i = 0; i < saveCount; i++)
            {
                var testData = new DemoTestPersonData();
                long startTime = DateTime.Now.Ticks;
                if (_dataService.SaveData(testData))
                {
                    var saveTime = DateTime.Now.Ticks - startTime;
                    var savemsTime = (float)saveTime / TimeSpan.TicksPerMillisecond;
                    totalSaveTime += savemsTime;
                    var dataStr = JsonToolkit.SerializeObject(testData, Newtonsoft.Json.Formatting.Indented);
                    saveBuilder.Append("\r\n");
                    saveBuilder.Append(dataStr);
                }
                else
                {
                    saveBuilder.Append("\r\n <color=#ff0000>Save failed!</color>");
                }
            }
            _saveTimeText.SetText($"{totalSaveTime:N4} ms");
            _sourceDataText.text = string.Format("Sources Data Here.\r\n{0}", saveBuilder.ToString());
        }

        public void OnLoadSingleDataClick()
        {
            int id = 0;
            if (!string.IsNullOrWhiteSpace(_loadDataId.text))
            {
                id = int.Parse(_loadDataId.text);
            }

            long startLoadTime = DateTime.Now.Ticks;
            DemoTestPersonData data = _dataService.LoadDataById<DemoTestPersonData>(id);
            CheckLoadTime(startLoadTime);
            if (data != null)
            {
                _loadDataText.text = "Loaded from file: \r\n" + JsonToolkit.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);
            }
            else
            {
                _loadDataText.text = "<color=#ff0000>load data is null!</color>";
            }
        }

        public void OnLoadAllDataClick()
        {
            long startLoadTime = DateTime.Now.Ticks;
            List<DemoTestPersonData> datas = _dataService.LoadAllDatas<DemoTestPersonData>();
            CheckLoadTime(startLoadTime);
            if (datas != null)
            {
                StringBuilder stringBuilder = new StringBuilder(datas.Count);
                foreach (var data in datas)
                {
                    stringBuilder.Append(JsonToolkit.SerializeObject(data, Newtonsoft.Json.Formatting.Indented));
                }
                _loadDataText.text = "Loaded from file: \r\n" + stringBuilder.ToString();
            }
            else
            {
                _loadDataText.text = "<color=#ff0000>load datas is null!</color>";
            }
        }

        public void OnClearDataClick()
        {
            _dataService.DeleteTableDatas<DemoTestPersonData>();
        }

        private void CheckSaveTime(long startTime)
        {
            var saveTime = DateTime.Now.Ticks - startTime;
            var savemsTime = (float)saveTime / TimeSpan.TicksPerMillisecond;
            _saveTimeText.SetText($"{savemsTime:N4} ms");
        }

        private void CheckLoadTime(long startTime)
        {
            var loadTime = DateTime.Now.Ticks - startTime;
            var loadmsTime = (float)loadTime / TimeSpan.TicksPerMillisecond;

            _loadTimeText.SetText($"{loadmsTime:N4} ms");
        }
    }
}