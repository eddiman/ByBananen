using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using ControllerScripts;
using Dreamteck.Splines;
using ControllerScripts;
using UnityEditor;
using Random = UnityEngine.Random;

namespace TramScripts
{
    public class TramMechanics : MonoBehaviour
    {
        [System.Serializable]
        public class CoasterSound
        {
            public float StartPercent = 0f;
            public float EndPercent = 1f;
            public AudioSource Source;
            public float StartPitch = 1f;
            public float EndPitch = 1f;
        }

        public static float Speed;
        public float MinSpeed;
        public float MaxSpeed;
        private static SplineFollower _follower;

        private float _brakeForce = 0f;

        public CoasterSound[] Sounds;
        public float SoundFadeLength = 0.15f;

        public string BellSoundName;
        private AudioSource bellSound;
        public float accelSpeed = 0.01f;
        public float decelSpeed = 0.04f;


        /*Attached children*/
        private GameObject doorFront;
        private GameObject doorBack;




        //States
        public bool isMoving;

        public bool isOpenLeftSide;
        public bool isOpenRightSide;
        public bool isCurrentTram;
        private static readonly int DoorIsOpen = Animator.StringToHash("doorIsOpen");


        // Use this for initialization
        void Start()
        {
            bellSound = gameObject.AddComponent<AudioSource>();
            bellSound.clip = Resources.Load(BellSoundName) as AudioClip;
            _follower = GetComponent<SplineFollower>();

            //_follower.onEndReached += OnEndReached;
        }

        // Update is called once per frame
        void Update()
        {
            if(SceneGlobals.currentTram != null){
                //Setting states
                SetIsMovingState();
                SetCurrentTramState();
                float speedPercent = Mathf.InverseLerp(MinSpeed, MaxSpeed, Speed);

                Speed = Mathf.Clamp(Speed, MinSpeed, MaxSpeed);


                _follower.followSpeed = Speed;

                speedPercent = Mathf.Clamp01(Speed/MaxSpeed)*(1f-_brakeForce);
                for (int i = 0; i < Sounds.Length; i++) {
                    if (speedPercent < Sounds[i].StartPercent - SoundFadeLength || speedPercent > Sounds[i].EndPercent + SoundFadeLength)
                    {
                        if (Sounds[i].Source.isPlaying) Sounds[i].Source.Pause();
                        continue;
                    }
                    if (!Sounds[i].Source.isPlaying) Sounds[i].Source.UnPause();
                    float volume = 1f;
                    if (speedPercent < Sounds[i].StartPercent+SoundFadeLength) volume = Mathf.InverseLerp(Sounds[i].StartPercent, Sounds[i].StartPercent+SoundFadeLength, speedPercent);
                    else if (speedPercent > Sounds[i].EndPercent) volume = Mathf.InverseLerp(Sounds[i].EndPercent + SoundFadeLength, Sounds[i].EndPercent, speedPercent);
                    float pitchPercent = Mathf.InverseLerp(Sounds[i].StartPercent, Sounds[i].EndPercent, speedPercent);
                    Sounds[i].Source.volume = volume;
                    Sounds[i].Source.pitch = Mathf.Lerp(Sounds[i].StartPitch, Sounds[i].EndPitch, pitchPercent);
                }

                CloseDoorsIfMoving();
            }
        }

        private void CloseDoorsIfMoving()
        {

            if (isOpenRightSide && isMoving)
            {
                Debug.Log("closing the door in update");
                ToggleDoors("right", false);
            }
            if (isOpenLeftSide && isMoving)
            {
                Debug.Log("closing the door in update");
                ToggleDoors("left", false);

            }


        }


        private void SetCurrentTramState()
        {
            if (SceneGlobals.currentTram == gameObject)
            {
                isCurrentTram = true;
            }
            else
            {
                isCurrentTram = false;
            }
        }

        private void SetIsMovingState()
        {
            if (Speed > 0.01f)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        public static void AddBrake(float time)
        {
//BrakeSound.Stop();
//BrakeSound.Play();
        }

        public static void RemoveBrake()
        {

        }

        public void SwitchDirection()
        {
            if (_follower.direction == Spline.Direction.Forward )
            {
                _follower.direction = Spline.Direction.Backward;
            }
            else
            {
                _follower.direction = Spline.Direction.Forward;

            }

        }

        public void OpenSideDoors(string side)
        {
           // doorFront = transform.Find("tram_door_" + side + "_1").gameObject;
            //doorBack = transform.Find("tram_door_" + side + "_2").gameObject;
            if((!isOpenLeftSide && !isOpenRightSide && !isMoving) || (!isOpenRightSide && !isOpenLeftSide && !isMoving))
            {
                ToggleDoors(side, true);
            }
            else
            {
                ToggleDoors(side, false);
            }
        }
        private void ToggleDoors(string side, bool isOpen)
        {
            //doorFront.GetComponent<Animator>().SetBool(DoorIsOpen, isOpen);
            //doorBack.GetComponent<Animator>().SetBool(DoorIsOpen, isOpen);

            if (side == "left" )
            {isOpenLeftSide = isOpen;}
            else
            {isOpenRightSide = isOpen;}

            Debug.Log("closing");
        }

        public void RingBell()
        {
            bellSound.Play();
            bellSound.Stop();
        }

        public void Accelerate()
        {
            Speed = Speed + accelSpeed;
        }
        public void Decelerate()
        {
            Speed = Speed - decelSpeed;
        }

        public float getCurrentSpeed()
        {
            return Speed;
        }
//Updates the followers on the SplineComputer
        public void UpdateFollower()
        {

            _follower = GetComponent<SplineFollower>();
            if (gameObject != SceneGlobals.previousTram)
            {
                //TODO: Gotta find a way to retain speed when changing
                Speed = 0;
            }


        }
    }
}
