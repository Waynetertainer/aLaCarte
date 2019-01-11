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