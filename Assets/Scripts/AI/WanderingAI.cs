using UnityEngine;
using UnityEngine.AI;

namespace MMO.Entity.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class WanderingAI : MonoBehaviour
    {

        private EntityGO eGo;
        
        public float wanderRadius = 100;
        public float wanderTimer = 10;
     
        private Transform target;
        private NavMeshAgent agent;
        private float timer;

        void Awake()
        {
            eGo = gameObject.GetComponent<EntityGO>();
        }
        // Use this for initialization
        void OnEnable () {
            
            agent = GetComponent<NavMeshAgent>();
            timer = wanderTimer;
        }
     
        // Update is called once per frame
        void Update () {
            timer += Time.deltaTime;
     
            if (timer >= wanderTimer) {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                agent.SetDestination(newPos);
                timer = 0;
            }

            eGo.EntityPositon = transform.position;
        }
     
        public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
            Vector3 randDirection = Random.insideUnitSphere * dist;
     
            randDirection += origin;
     
            NavMeshHit navHit;
     
            NavMesh.SamplePosition (randDirection, out navHit, dist, layermask);
     
            return navHit.position;
        }
    }
}