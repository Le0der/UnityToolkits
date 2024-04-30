using System;
using System.Collections;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;
using TMPro;

namespace Le0derToolkit.GraphAndChart
{
    public class UIGraphChartMultiple : MonoBehaviour
    {
        [Header("Y轴单位")][SerializeField] private TextMeshProUGUI _yAxisUnit;
        [Header("图像节点")][SerializeField] protected GraphChart _graph;
        [Header("横坐标")][SerializeField] protected HorizontalAxis _horizontalAxis;
        [Header("图像名称")][SerializeField] protected List<string> _categories;
        [Header("绘制时间")][SerializeField] protected float _duration = 1;

        #region 根据绘画时间画线
        private float _time = 0;
        private int _index = -1;
        private List<GraphChartMultipleDateData> _datas;
        private void Update()
        {
            if (_index < 0) return;

            _time += Time.deltaTime;
            if (_time > _duration / _datas.Count)
            {
                _time = 0;
                _index++;

                var data = _datas[_index - 1];
                if (_index >= _datas.Count) _index = -1;

                for (int i = 0; i < data.values.Count; i++)
                {
                    _graph.DataSource.AddPointToCategoryRealtime(_categories[i], data.date, data.values[i], 0.5f);
                }
            }
        }
        #endregion

        public void StartInteration(string unit, List<GraphChartMultipleDateData> datas, List<Material> lineStyles, List<Material> innerStyles)
        {
            if (datas == null || datas.Count == 0)
            {
                Debug.LogError("图像数据不能为空。");
                return;
            }

            foreach (var category in _categories)
            {
                if (!_graph.DataSource.HasCategory(category))
                {
                    Debug.LogErrorFormat("Error: 图像“ {0} ”不存在，请确保图像存在再开始作图！！！", category);
                    return;
                }
            }

            _yAxisUnit.text = unit;
            StartDrawGraph(_categories, datas, lineStyles, innerStyles);
        }

        private void StartDrawGraph(List<string> _categories, List<GraphChartMultipleDateData> datas, List<Material> lineStyles, List<Material> innerStyles)
        {
            _time = 0;
            _index = 0;
            _datas = datas;

            UpdateYAxisValue(datas);
            for (int i = 0; i < _categories.Count; i++)
            {
                var category = _categories[i];
                var lineStyle = lineStyles[i];
                var innerStyle = innerStyles[i];

                _graph.DataSource.GetCategoryLine(category, out var line, out var lineThickness, out var lineTiling);
                _graph.DataSource.SetCategoryLine(category, lineStyle, lineThickness, lineTiling);
                _graph.DataSource.SetCategoryFill(category, innerStyle, true);
                _graph.DataSource.ClearCategory(category);
            }
        }

        private void UpdateYAxisValue(List<GraphChartMultipleDateData> datas)
        {
            var min = double.MaxValue;
            var max = double.MinValue;
            foreach (var data in datas)
            {
                if (data.values != null && data.values.Count > 0)
                {
                    foreach (var valueItem in data.values)
                    {
                        // 更新全局最大值和最小值
                        max = Math.Max(max, valueItem);
                        min = Math.Min(min, valueItem);
                    }
                }
            }
            // var intMagnitude = GetMinIntegerWithMagnitude(max);

            // min = min - intMagnitude;
            // if (max == min) max = min + intMagnitude;
            // else max = max + intMagnitude;

            _graph.DataSource.VerticalViewOrigin = min;
            _graph.DataSource.VerticalViewSize = max - min;

            var minX = ChartDateUtility.DateToValue(datas[0].date);
            var maxX = ChartDateUtility.DateToValue(datas[datas.Count - 1].date);
            _graph.DataSource.HorizontalViewOrigin = minX;
            _graph.DataSource.HorizontalViewSize = maxX - minX;

            _horizontalAxis.MainDivisions.Total = datas.Count - 1;
        }

        private double GetMinIntegerWithMagnitude(double number)
        {
            // 获取数字的数量级（幂指数）
            int exponent = (int)Math.Floor(Math.Log10(Math.Abs(number)));

            // 计算最小整数，数量级为 10^exponent
            double result = Math.Pow(10, exponent);

            return result;
        }
    }


    public class GraphChartMultipleDateData
    {
        public DateTime date;
        public List<double> values;
    }
}