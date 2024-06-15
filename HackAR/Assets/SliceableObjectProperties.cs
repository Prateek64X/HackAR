using UnityEngine;

public class SliceableObjectProperties : MonoBehaviour
{
    public MeshFilter meshFilterToCopy;
    public Renderer rendererToCopy;

    void Start()
    {
        if (meshFilterToCopy != null)
        {
            // Copy mesh from the specified MeshFilter
            MeshFilter myMeshFilter = GetComponent<MeshFilter>();
            if (myMeshFilter != null)
            {
                myMeshFilter.mesh = meshFilterToCopy.mesh;
            }
            else
            {
                Debug.LogWarning("MeshFilter component not found on the Sliceable object.");
            }
        }

        if (rendererToCopy != null)
        {
            // Copy materials from the specified Renderer
            Renderer myRenderer = GetComponent<Renderer>();
            if (myRenderer != null)
            {
                myRenderer.sharedMaterials = rendererToCopy.sharedMaterials;
            }
            else
            {
                Debug.LogWarning("Renderer component not found on the Sliceable object.");
            }
        }
    }
}
