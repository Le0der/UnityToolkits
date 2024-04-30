using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustByParent : MonoBehaviour
{
    public RectTransform ParentRectTrans;
    public RectTransform TargetRectTrans;
    public AdjustType AdjustType;

    private Vector2 _desigSize;

    // Start is called before the first frame update
    void Start()
    {
        this._desigSize = TargetRectTrans.sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        switch (AdjustType)
        {
            case AdjustType.Width:
                var scaleX = GetScaleRatio(this._desigSize.x, ParentRectTrans.rect.width);
                TargetRectTrans.localScale = new Vector3(scaleX, scaleX, 1);
                break;
            case AdjustType.High:
                var scaleY = GetScaleRatio(this._desigSize.y, ParentRectTrans.rect.height);
                TargetRectTrans.localScale = new Vector3(scaleY, scaleY, 1);
                break;
            default:
                var scaleX1 = GetScaleRatio(this._desigSize.x, ParentRectTrans.rect.width);
                var scaleY1 = GetScaleRatio(this._desigSize.y, ParentRectTrans.rect.height);
                var scale = Mathf.Min(scaleX1, scaleY1);
                TargetRectTrans.localScale = new Vector3(scale, scale, 1);
                break;
        }
    }

    private float GetScaleRatio(float stand, float parent)
    {
        return parent / stand;
    }
}

public enum AdjustType
{
    Width,
    High,
    WidthOrHigh,
}
