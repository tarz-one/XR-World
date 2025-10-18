using System.Collections;
using UnityEngine;
using Meta.XR.MRUtilityKit;

public class MRUKBootstrap : MonoBehaviour
{
    [Header("Hook up in Inspector")]
    public EffectMesh effectMesh;         // your EffectMesh component
    public LayerApplier layerApplier;     // your existing script (ApplyLayer must be public)
    public string wallLayerName = "Wall";

    MRUKRoom lastRoom;

    void OnEnable()
    {
        StartCoroutine(BootstrapLoop());
    }

    IEnumerator BootstrapLoop()
    {
        // Wait for MRUK runtime and a room to exist
        while (MRUK.Instance == null) yield return null;

        // Wait for first room
        while ((lastRoom = FindObjectOfType<MRUKRoom>()) == null) yield return null;

        // Handle first room
        HandleRoom(lastRoom);

        // Light polling in case the room is regenerated/updated later
        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            var room = FindObjectOfType<MRUKRoom>();
            if (room != null && room != lastRoom)
            {
                lastRoom = room;
                HandleRoom(lastRoom);
            }
            else if (room != null && room.transform.hasChanged)
            {
                room.transform.hasChanged = false;
                HandleRoom(room);
            }
            yield return wait;
        }
    }

    void HandleRoom(MRUKRoom room)
    {
        if (room == null) return;

        if (effectMesh != null)
            effectMesh.CreateMesh();

        if (layerApplier != null)
            layerApplier.ApplyLayer(room.gameObject, wallLayerName);

        Debug.Log($"[MRUKBootstrap] Applied layer '{wallLayerName}' to room and created EffectMesh.");
    }
}