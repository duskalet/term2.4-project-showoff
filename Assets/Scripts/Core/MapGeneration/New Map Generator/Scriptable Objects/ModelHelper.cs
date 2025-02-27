using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class ModelHelper : MonoBehaviour
{
    [SerializeField] bool isEditorVisual = false;
    bool lastIsEditorVisual = false;

    List<MeshRenderer> allMeshes = new List<MeshRenderer>();
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    List<Collider> colliders = new List<Collider>();

    private void Awake()
    {
        lastIsEditorVisual = !isEditorVisual;
        allMeshes = GetComponentsInChildren<MeshRenderer>(true).ToList();
        meshFilters = GetComponentsInChildren<MeshFilter>(true).ToList();
        colliders = GetComponentsInChildren<Collider>(true).ToList();
    }

    private void OnDrawGizmos()
    {
        if (!isEditorVisual) return;
        Gizmos.color = Color.white;
        foreach (MeshFilter mesh in meshFilters)
        {
            Gizmos.DrawWireMesh(mesh.sharedMesh, mesh.transform.position, mesh.transform.rotation, mesh.transform.localScale);
        }
    }

    private void Update()
    {
        if (lastIsEditorVisual != isEditorVisual)
        {
            lastIsEditorVisual = isEditorVisual;
            OnChange();
        }
    }

    void OnChange()
    {
        foreach (Collider col in colliders)
            col.enabled = isEditorVisual;

        foreach (MeshRenderer renderer in allMeshes)
            renderer.enabled = false;
    }


    public void Display(bool display)
    {
        lastIsEditorVisual = isEditorVisual;
        if (isEditorVisual) return;

        foreach (MeshRenderer renderer in allMeshes)
            renderer.enabled = display;

    }
}
