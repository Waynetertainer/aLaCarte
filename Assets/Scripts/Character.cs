using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class Character : MonoBehaviour
    {
        public eCarryableType[] pCarrying = new eCarryableType[4];
        [SerializeField] public GameObject[][] pCarryableObjects = new GameObject[3][];

        public GameObject[] pFood = new GameObject[4];
        public GameObject[] pPlates = new GameObject[2];
        public GameObject[] pCustomer = new GameObject[1];

        public Transform pTarget;
        public int pID;

        private NavMeshAgent mAgent;

        private void Start()
        {
            mAgent = GetComponent<NavMeshAgent>();
            mAgent.destination = pTarget.position;
        }

        public void Move(bool doIt)
        {
            mAgent.isStopped = !doIt;
        }

        public void SetTargetPosition(Vector3 position)
        {
            mAgent.destination = position;
        }
    }
}