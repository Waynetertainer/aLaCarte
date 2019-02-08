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
        public GameObject pDecal;

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

        public void SetDecal(bool value)
        {
            pDecal.SetActive(value);
        }

        public void SetAnimation(eCarryableType? carried=null)
        {
            switch (carried)
            {
                case eCarryableType.Food:
                    mAnimator.SetBool("Dome",true);
                    mAnimator.SetBool("Dish",false);
                    break;
                case eCarryableType.Customer:
                    mAnimator.SetBool("Dome", false);
                    mAnimator.SetBool("Dish", false);
                    break;
                case eCarryableType.Dishes:
                    mAnimator.SetBool("Dome", false);
                    mAnimator.SetBool("Dish", true);
                    break;
                case null:
                    mAnimator.SetBool("Dome", false);
                    mAnimator.SetBool("Dish", false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("carried", carried, null);
            }
        }
    }
}