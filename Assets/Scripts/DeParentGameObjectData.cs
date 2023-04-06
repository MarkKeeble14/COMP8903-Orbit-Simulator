using UnityEngine;

[System.Serializable]
public struct DeParentGameObjectData
{
    public Transform Transform;
    public bool DisableCollider;
    public bool AddRigidBody;
    public Vector3 AddedForcePerAxis;
    public Vector3 AddedTorquePerAxis;
}
