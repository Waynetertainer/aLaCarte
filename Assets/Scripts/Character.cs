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
        private Animator mAnimator;
        private Vector3 mLastPosition;
        private float mSpeed;

        private void Start()
        {
            mAgent = GetComponent<NavMeshAgent>();
            mAnimator = transform.GetChild(0).GetComponent<Animator>();
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

        private void Update()
        {
            mSpeed = Mathf.Lerp(mSpeed, (transform.position - mLastPosition).magnitude / Time.deltaTime, 0.75f);
            mLastPosition = transform.position;

            mAnimator.SetInteger("Walk", Mathf.RoundToInt(mSpeed));
        }
    }
}