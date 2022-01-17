using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class BDAY : MonoBehaviour
{
    [SerializeField]
    private GameObject _letterPrefab;
    [SerializeField]
    private GameObject _actorPrefab;
    [SerializeField]
    private bool _billboardLetters;
    [SerializeField]
    private Vector3 _placementRegion;
    [SerializeField, Multiline]
    private string _names;
    private readonly ObjectPool<PlayableLetter> _letterPool;
    private Vector2 _camRotation;
    private readonly List<PlayableWord> _words;
    private int _currentWordIndex;
    private float _deltaTimeCam;
    private bool _isTeleportingCam;
    private Vector3 _camInitialPos;
    private Vector3 _camTargetPos;
    private Quaternion _camInitialRot;
    private Quaternion _camTargetRot;
    private PlayableWord _selectedWord;

    public Camera Camera
    {
        get; private set;
    }

    public bool BillboardLetters
    {
        get { return _billboardLetters; }
    }

    public static BDAY Instance
    {
        get; private set;
    }

    public BDAY()
    {
        _letterPool = new ObjectPool<PlayableLetter>(InstantiateLetter);
        _words = new List<PlayableWord>();
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;

        Instance = this;
        Camera = Camera.main;
    }

    private void Start()
    {
        string[] names = _names.Split('\n');
        foreach (string name in names)
        {
            GameObject obj = Instantiate(_actorPrefab);
            obj.transform.position = GetRandomPositionInRegion();

            PlayableWord word = new GameObject($"WORD: {name}").AddComponent<PlayableWord>();
            word.Word = name;
            word.AttachedTransform = obj.GetComponent<TargetTransform>().Target;
            
            _words.Add(word);
            Debug.Log($"Instantiated w={name}");
        }

        _letterPrefab.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            _selectedWord = _words[_currentWordIndex++ % _words.Count];

            Vector3 dir = (_selectedWord.transform.position - Camera.transform.position).normalized;
            Vector3 targetPos = _selectedWord.transform.position - dir * 3f; //3m away towards cam
            StartCameraTeleportation(targetPos, dir);
        }

        UpdateCamMovement();

        foreach (PlayableWord w in _words)
        {
            w.SetIdleMode((Camera.transform.position - w.transform.position).sqrMagnitude < 5f * 5f
                || _selectedWord == w);
        }
    }

    private void StartCameraTeleportation(Vector3 targetPos, Vector3 lookDir)
    {
        _isTeleportingCam = true;
        _deltaTimeCam = 0f;

        _camInitialPos = Camera.transform.position;
        _camTargetPos = targetPos;

        _camInitialRot = Camera.transform.rotation;
        _camTargetRot = Quaternion.LookRotation(lookDir);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, _placementRegion);
    }

    private PlayableLetter InstantiateLetter()
    {
        GameObject obj = Instantiate(_letterPrefab);
        obj.SetActive(true);
        return obj.GetComponent<PlayableLetter>();
    }

    public PlayableLetter GetLetter()
    {
        return _letterPool.Get();
    }

    private Vector3 GetRandomPositionInRegion()
    {
        return new Vector3(
            transform.position.x + Random.Range(-0.5f, 0.5f) * _placementRegion.x,
            transform.position.y + Random.Range(-0.5f, 0.5f) * _placementRegion.y,
            transform.position.z + Random.Range(-0.5f, 0.5f) * _placementRegion.z
        );
    }

    private void UpdateCamMovement()
    {
        Transform camTransform = Camera.transform;

        if (_isTeleportingCam)
        {
            _deltaTimeCam += Time.deltaTime * 3f;

            if (_deltaTimeCam > 1f)
            {
                _isTeleportingCam = false;
            }

            camTransform.position = Vector3.Lerp(_camInitialPos, _camTargetPos, _deltaTimeCam);
            camTransform.rotation = Quaternion.Lerp(_camInitialRot, _camTargetRot, _deltaTimeCam);

            _camRotation.x = camTransform.rotation.eulerAngles.y;
            _camRotation.y = -camTransform.rotation.eulerAngles.x;
            return;
        }

        //move/rot cam
        _camRotation += new Vector2
        {
            x = Input.GetAxis("Mouse X") * Time.deltaTime * 50f,
            y = Input.GetAxis("Mouse Y") * Time.deltaTime * 50f
        };

        camTransform.rotation = Quaternion.AngleAxis(_camRotation.x, Vector3.up);
        camTransform.rotation *= Quaternion.AngleAxis(_camRotation.y, Vector3.left);

        camTransform.position += camTransform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 10f
            + camTransform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 10f;
    }
}