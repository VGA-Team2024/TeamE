using UnityEngine;

public class BoneChecker : MonoBehaviour
{
    private BoneContainer _boneContainer;
    public BoneContainer BoneContainer => _boneContainer;
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out BoneContainer bone))
        {
            _boneContainer = bone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _boneContainer = null;
    }
}
