using System.Collections.Generic;
using UnityEngine;

public class ConstraintObject : MonoBehaviour
{
    List<Transform> _targets = new();
    Vector3 _prevPosition;
    public List<Transform> Targets => _targets;
    void Awake()
    {
        _prevPosition = transform.position;
    }

    private void Update()
    {
        if (_prevPosition != transform.position)
        {
            var offset = transform.position - _prevPosition;
            foreach (var target in _targets)
            {
                if (target != null)
                {
                    target.position += offset;
                }
            }
        
            _prevPosition = transform.position;
        }
    }
}
