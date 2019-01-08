using UnityEngine;

namespace MMO.Entity
{
    public class ClientUpdate : MonoBehaviour
    {
        private Collider[] hitColliders = new Collider[50];
        
        void Update()
        {
            Physics.OverlapSphereNonAlloc(transform.position, 10.0f, hitColliders);
            int i = 0;
            while (i < hitColliders.Length)
            {
                Debug.Log(hitColliders[i]);

                i++;
            }
        }
    }
}