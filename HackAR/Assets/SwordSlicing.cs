using UnityEngine;
using EzySlice;

public class SwordSlicing : MonoBehaviour
{
    public Material crossSectionMaterial;
    public float sliceForce = 2f; // Adjust as needed
    public bool useBoxCollider = false; // Toggle between BoxCollider and MeshCollider

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is sliceable
        if (other.CompareTag("Sliceable"))
        {
            // Get the point of contact between the sword and the object
            Vector3 contactPoint = other.ClosestPoint(transform.position);

            // Compute the direction of the slicing plane based on the sword's movement
            Vector3 sliceDirection = transform.position - contactPoint;

            // Implement slicing logic here
            SliceObject(other.gameObject, contactPoint, sliceDirection);
        }
    }

    private void SliceObject(GameObject target, Vector3 contactPoint, Vector3 sliceDirection)
    {
        // Ensure the target has a MeshFilter, MeshRenderer, and Collider
        MeshFilter meshFilter = target.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();
        Collider collider = target.GetComponent<Collider>();

        if (meshFilter == null || meshRenderer == null || collider == null)
        {
            Debug.LogWarning("The target does not have the required components for slicing.");
            return;
        }

        // Compute slicing plane orientation
        EzySlice.Plane slicePlane = new EzySlice.Plane(sliceDirection.normalized, contactPoint);

        // Perform the slicing operation
        SlicedHull slicedHull = target.Slice(slicePlane, crossSectionMaterial);

        if (slicedHull != null)
        {
            // Create the upper and lower hull GameObjects
            GameObject upperHull = slicedHull.CreateUpperHull(target, crossSectionMaterial);
            GameObject lowerHull = slicedHull.CreateLowerHull(target, crossSectionMaterial);

            // Optionally add physics to the sliced parts
            AddPhysics(upperHull);
            AddPhysics(lowerHull);

            // Apply the same rotation as the original object
            upperHull.transform.rotation = target.transform.rotation;
            lowerHull.transform.rotation = target.transform.rotation;

            // Apply a force to spread the sliced parts
            ApplySliceForce(upperHull, sliceForce);
            ApplySliceForce(lowerHull, -sliceForce); // Opposite direction

            // Make the sliced parts sliceable again
            upperHull.tag = "Sliceable";
            lowerHull.tag = "Sliceable";

            // Destroy the original object
            Destroy(target);
        }
    }

    private void AddPhysics(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }

        if (useBoxCollider)
        {
            BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = obj.AddComponent<BoxCollider>();
            }
        }
        else
        {
            MeshCollider meshCollider = obj.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = obj.AddComponent<MeshCollider>();
            }
            meshCollider.convex = true;
        }
    }

    private void ApplySliceForce(GameObject obj, float force)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.right * force, ForceMode.Impulse);
        }
    }
}
