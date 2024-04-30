using System;
using System.Collections;
using System.Collections.Generic;
using ChartAndGraph;
using UnityEngine;
using TMPro;

namespace Le0derToolkit.GraphAndChart
{
    //配合预制体使用
    public class UIGraphChart : MonoBehaviour
    {
        [Header("Y轴单位")][SerializeField] private TextMeshProUGUI _unit;
        [Header("图像节点")][SerializeField] protected GraphChart _graph;
        [Header("图像名称")][SerializeField] protected string _category;
        [Header("绘制时间")][SerializeField] protected float _duration = 1;

        #region 根据绘画时间画线
        private float _time = 0;
        private int _index = -1;
        private List<GraphChartData> _datas;
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

                _graph.DataSource.AddPointToCategoryRealtime(_category, data.x, data.y, 0.5f);
            }
        }
        #endregion

        public void StartInteration(string unit, List<GraphChartData> datas, Material lineStyle, Material innerStyle)
        {
            if (datas == null || datas.Count == 0)
            {
                Debug.LogError("图像数据不能为空。");
                return;
            }

            if (!_graph.DataSource.HasCategory(_category))
            {
                Debug.LogErrorFormat("Error: 图像“ {0} ”不存在，请确保图像存在再开始作图！！！", _category);
                return;
            }
            _unit.text = unit;
            StartDrawGraph(_category, datas, lineStyle, innerStyle);
        }

        private void StartDrawGraph(string category, List<GraphChartData> datas, Material lineStyle, Material innerStyle)
        {
            _time = 0;
            _index = 0;
            _datas = datas;

            UpdateYAxisValue(datas);
            _graph.DataSource.GetCategoryLine(category, out var line, out var lineThickness, out var lineTiling);
            _graph.DataSource.SetCategoryLine(category, lineStyle, lineThickness, lineTiling);
            _graph.DataSource.SetCategoryFill(category, innerStyle, true);
            _graph.DataSource.ClearCategory(category);
        }

        private void UpdateYAxisValue(List<GraphChartData> datas)
        {
            var min = datas[0].y;
            var max = datas[0].y;
            for (int i = 0; i < datas.Count; i++)
            {
                max = Math.Max(max, datas[i].y);
                min = Math.Min(min, datas[i].y);
            }
            var intMagnitude = GetMinIntegerWithMagnitude(max);

            min = min - intMagnitude;
            if (max == min) max = min + intMagnitude;
            else max = max + intMagnitude;

            _graph.DataSource.VerticalViewOrigin = min;
            _graph.DataSource.VerticalViewSize = max - min;
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

    public class GraphChartData
    {
        public double x;
        public double y;
    }

}