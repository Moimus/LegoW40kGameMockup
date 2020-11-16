using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSplatterer : MonoBehaviour
{
    public List<Transform> specialParts = new List<Transform>();
    /// <summary>
    /// 0 = head, 1 = Body, 2 = ArmR, 3 = ArmL, 4 = HandR, 5 = HandL, 6 = Legs
    /// </summary>
    [Tooltip("0 = head, 1 = Body, 2 = ArmR, 3 = ArmL, 4 = HandR, 5 = HandL, 6 = Legs")]
    public List<Transform> miniParts = new List<Transform>(6);
    /// <summary>
    /// 0 = head, 1 = Body, 2 = ArmR, 3 = ArmL, 4 = HandR, 5 = HandL, 6 = Legs
    /// </summary>
    [Tooltip("0 = head, 1 = Body, 2 = ArmR, 3 = ArmL, 4 = HandR, 5 = HandL, 6 = Legs")]
    public List<Material> miniMaterials = new List<Material>(6);
    public float lifetime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void splatter(List<Material> textureSet, List<Transform> miniSpecialParts)
    {
        miniMaterials = textureSet;
        for(int n = 0; n < miniParts.Count; n++)
        {
            miniParts[n].GetComponent<Renderer>().material = miniMaterials[n];
            miniParts[n].GetComponent<Rigidbody>().AddExplosionForce(40, transform.position, 0.3f);
        }

        for (int n = 0; n < miniSpecialParts.Count; n++)
        {
            BoxCollider collider = miniSpecialParts[n].gameObject.AddComponent<BoxCollider>();
            collider.center = miniSpecialParts[n].parent.transform.localPosition; //set collider center to bone position
            miniSpecialParts[n].parent = transform;
            collider.size = new Vector3(0.2f, 0.2f, 0.2f);
            Rigidbody rigidbody = miniSpecialParts[n].gameObject.AddComponent<Rigidbody>();
            rigidbody.mass = 0.1f;
        }
    }
}
