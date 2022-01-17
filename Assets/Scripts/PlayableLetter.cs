using System.Linq;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class PlayableLetter : MonoBehaviour
{
    private TextMeshPro _text;
    private static PlayableWord _lastLookedupWord;
    private static readonly List<Transform> _transformBuffer;

    public event System.Action<PlayableLetter> OnUpdate;
    public char Character
    {
        get
        {
            return _text.text.FirstOrDefault();
        }

        set
        {
            _text.text = value.ToString();
        }
    }

    public float RenderedHeight
    {
        get { return _text.renderedHeight; }
    }

    public float RenderedWidth
    {
        get { return _text.renderedWidth; }
    }

    public Color Color
    {
        set { _text.color = value; _text.fontSharedMaterial.SetColor(ShaderUtilities.ID_GlowColor, value); }
    }

    private void OnEnable()
    {
        _text = GetComponent<TextMeshPro>();
    }

    static PlayableLetter()
    {
        _transformBuffer = new List<Transform>();
    }

    public void SetWord(PlayableWord word)
    {
        if (_lastLookedupWord != word)
        {
            _lastLookedupWord = word;

            _transformBuffer.Clear();
            word.transform.parent.GetComponentsInChildren<Transform>(true, _transformBuffer);
            FilterTransformBuffer();
        }

        Debug.Log(_transformBuffer.Count);
        //attach to a random bone, hopefully with a parent
        transform.SetParent(_transformBuffer[Random.Range(0, _transformBuffer.Count)]);
        transform.localPosition = transform.localEulerAngles = Vector3.zero;
    }

    private void FilterTransformBuffer()
    {
        for (int i = _transformBuffer.Count - 1; i >= 0; i--)
        {
            if (!_transformBuffer[i].name.StartsWith("mixamorig:"))
            {
                _transformBuffer.RemoveAt(i);
            }
        }
    }

    private void FixedUpdate()
    {
        if (BDAY.Instance.BillboardLetters)
        {
            Vector3 dir = (BDAY.Instance.Camera.transform.position - transform.position).normalized;
            Quaternion q = new Quaternion();
            q.SetLookRotation(-dir);
            transform.rotation = q;
        }

        if (OnUpdate != null)
        {
            OnUpdate(this);
        }
    }
}