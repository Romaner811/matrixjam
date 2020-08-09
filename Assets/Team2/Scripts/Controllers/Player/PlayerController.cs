﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatrixJam.Team2
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 10;
        [SerializeField] private float crouchSpeed = 5;
        [SerializeField] private float jumpForce = 2;
        [SerializeField] private float walkAnimSpeedThresh = 0.1f;
        [SerializeField] private int maxJumps = 1;

        float horizontalInput;
        bool jumpInput;
        private Animator playerAnimator;
        private Rigidbody2D rb;
        private bool isGrounded = false;
        private int currentJumpsCount = 0;
        private Checkpoint checkpoint;

        void Awake()
        {
            playerAnimator = gameObject.GetComponentInChildren<Animator>();
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            jumpInput = Input.GetKey(KeyCode.Space);

            if (Input.GetKeyDown(KeyCode.G))
            {
                ActivateCheckpoint();
            }
            AnimWalk();
        }

        void FixedUpdate()
        {
            Move(horizontalInput);
            Jump(jumpInput);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out Checkpoint checkpoint))
            {
                checkpoint.GetComponent<Collider2D>().enabled = false;
                this.checkpoint = checkpoint;
            }

            if (other.TryGetComponent(out GoThroughController goThroughController))
            {
                if (goThroughController.enabled) return;
            }

            if (other.TryGetComponent(out BroniReceiverController broniReceiverController))
            {
                return;
            }

            isGrounded = true;
            currentJumpsCount = 0;
        }
        void OnTriggerExit2D(Collider2D other)
        {
            isGrounded = false;
        }

        public void ActivateCheckpoint()
        {
            if (checkpoint != null)
            {
                checkpoint.Activate(this);
            }
        }

        private void Move(float horizontalDir)
        {
            rb.velocity = new Vector2(horizontalDir * movementSpeed, rb.velocity.y);
        }

        private void Jump(bool jumpInput)
        {
            if (!jumpInput || !CanJump) return;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            currentJumpsCount++;
        }

        private bool CanJump => isGrounded || currentJumpsCount < maxJumps;

        private void AnimWalk()
        {
            if (Mathf.Abs(rb.velocity.x) > walkAnimSpeedThresh
                && !playerAnimator.GetBool("walk"))
            {
                playerAnimator.SetBool("walk", true);
            }

            else if (Mathf.Abs(rb.velocity.x) < walkAnimSpeedThresh
                     && playerAnimator.GetBool("walk"))
            {
                playerAnimator.SetBool("walk", false);
            }
        }
    }
}