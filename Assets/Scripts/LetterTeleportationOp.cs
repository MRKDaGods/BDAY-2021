using UnityEngine;

public class LetterTeleportationOp
{
    private PlayableLetter _letter;
    private int _index;
    private Vector3 _shiftedCenter;
    private Vector3 _initialPosition;
    private float _deltaTime;

    public LetterTeleportationOp(PlayableLetter letter, int index, Vector3 shiftedCenter, bool keepRelative)
    {
        _letter = letter;
        _index = index;
        _shiftedCenter = shiftedCenter;
        _initialPosition = keepRelative ? letter.transform.localPosition : letter.transform.position;
    }

    public void Update(Vector3 dir, float delta, bool relative)
    {
        if (_deltaTime < 1f)
        {
            _deltaTime += Time.deltaTime * 2f;
        }

        //no relative positioning
        Vector3 targetPos = _shiftedCenter + dir * (delta * _index);
        if (relative)
        {
            _letter.transform.localPosition = Vector3.Lerp(_initialPosition, targetPos, _deltaTime);
        }
        else
        {
            _letter.transform.position = Vector3.Lerp(_initialPosition, targetPos, _deltaTime);
        }
    }
}