using UnityEngine;

public class TargetTransform : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    public Transform Target
    {
        get { return _target; }
    }
}