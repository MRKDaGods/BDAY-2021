using UnityEngine;
using System.Collections.Generic;

public class PlayableWord : MonoBehaviour
{
    [SerializeField]
    private string _word;
    [SerializeField]
    private Transform _attachedTransform;
    private readonly List<PlayableLetter> _letters;
    private bool _idleMode;
    private readonly HashSet<LetterTeleportationOp> _teleOps;
    [SerializeField]
    private float _maxHeight = 2f; //2m?
    private bool _modeChangeQueued;
    private ColorInterpolator _colorInterpolator;
    public string Word
    {
        get { return _word; }
        set { _word = value; }
    }

    public Transform AttachedTransform
    {
        get { return _attachedTransform; }
        set { _attachedTransform = value; }
    }

    public PlayableWord()
    {
        _letters = new List<PlayableLetter>();
        _teleOps = new HashSet<LetterTeleportationOp>();
    }

    private void Start()
    {
        transform.SetParent(_attachedTransform, false);

        foreach (char c in _word)
        {
            var letter = BDAY.Instance.GetLetter();
            letter.Character = c;
            letter.SetWord(this);
            letter.OnUpdate += UpdateLetterColor;

            _letters.Add(letter);
        }

        _colorInterpolator = gameObject.AddComponent<ColorInterpolator>();
        _colorInterpolator.Speed = 2f;
    }

    private void Update()
    {
        if (_letters.Count == 0) return;

        float avgWordHeight = _letters[0].RenderedHeight * _letters.Count;
        float avgWordWidth = _letters[0].RenderedWidth * _letters.Count;

        if (avgWordHeight > _maxHeight)
        {
            //scale
            avgWordHeight *= _maxHeight / avgWordHeight;
        }

        if (_modeChangeQueued)
        {
            _modeChangeQueued = false;
            //_idleMode = !_idleMode;

            if (_teleOps.Count > 0)
            {
                _teleOps.Clear();
            }

            if (_idleMode)
            {
                for (int i = 0; i < _letters.Count; i++)
                {
                    //start letter ops
                    _teleOps.Add(new LetterTeleportationOp(_letters[i], i, _attachedTransform.position - new Vector3(avgWordWidth / 2f, 0f), false));
                }
            }
            else
            {
                if (avgWordHeight > _maxHeight)
                {
                    //scale
                    avgWordHeight *= _maxHeight / avgWordHeight;
                }

                //place our letters
                for (int i = 0; i < _letters.Count; i++)
                {
                    _teleOps.Add(new LetterTeleportationOp(_letters[i], i, new Vector3(avgWordHeight / 2f, 0f), true));
                }
            }
        }

        Vector3 relativeDir = (_attachedTransform.parent.position - transform.position).normalized;

        foreach (var op in _teleOps)
        {
            op.Update(
                _idleMode ? -transform.right : relativeDir,
                _idleMode ? _letters[0].RenderedWidth : (avgWordHeight / _letters.Count),
                !_idleMode
            );
        }
    }

    public void SetIdleMode(bool idle)
    {
        if (_idleMode == idle) return;

        Debug.Log($"{Word} set idle to {idle}");
        _idleMode = idle;
        _modeChangeQueued = true;
    }

    private void UpdateLetterColor(PlayableLetter letter)
    {
        letter.Color = _colorInterpolator.Color;
    }
}
