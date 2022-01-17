using UnityEngine;

public class ColorInterpolator : MonoBehaviour
{
    private float _deltaTime;
    private Color _c1;
    private Color _c2;

    public float Speed
    {
        get; set;
    }

    public Color Color
    {
        get; private set;
    }

    private void Start()
    {
        _c1 = Random.ColorHSV();
        _c2 = Random.ColorHSV();
    }

    private void Update()
    {
        if (_deltaTime > 1f)
        {
            //reset
            _deltaTime = 0f;
            _c1 = _c2;
            _c2 = Random.ColorHSV();
        }

        _deltaTime += Time.deltaTime * Speed;
        Color = Color.Lerp(_c1, _c2, _deltaTime);
    }
}