using System.Collections;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class CeilingMover : MonoBehaviour
{
    [Tooltip("Speed at which the ceiling moves out")]
    [SerializeField] private float speed = 0.5f;

    [Tooltip("The EffectMesh spawning our ceiling")]
    [SerializeField] private EffectMesh ceilingEffectMesh;

    private void Start()
    {
        StartCoroutine(WaitForEffectMeshAndMove());
    }

    private IEnumerator WaitForEffectMeshAndMove()
    {
        // 1. grab the current room
        var room = MRUK.Instance.GetCurrentRoom();

        // 2. wait until the mesh is spawned
        while (room.CeilingAnchor &&
               !ceilingEffectMesh.EffectMeshObjects.ContainsKey(room.CeilingAnchor))
        {
            yield return null;
        }

        // 3. fetch the wrapper and its real mesh GameObject
        var wrapper = ceilingEffectMesh.EffectMeshObjects[room.CeilingAnchor];
        var meshGo = wrapper.effectMeshGO;
        if (!meshGo)
        {
            Debug.LogError("No effectMeshGO found on wrapper!");
            yield break;
        }

        // 4. detach from the anchor so we only move the mesh
        meshGo.transform.SetParent(null, /* worldPositionStays= */ true);

        // 5. compute how far to move: half-room + half-ceiling + margin
        var roomBounds  = room.GetRoomBounds();
        var meshBounds  = meshGo.GetComponentInChildren<Renderer>().bounds;
        const float MARGIN = 0.1f;
        var distance  = roomBounds.extents.x + meshBounds.extents.x + MARGIN;

        // 6. slide the mesh straight out in world-space
        var moved = 0f;
        var dir = Vector3.right;
        while (moved < distance)
        {
            var step = speed * Time.deltaTime;
            meshGo.transform.Translate(dir * step, Space.World);
            moved += step;
            yield return null;
        }
    }
}