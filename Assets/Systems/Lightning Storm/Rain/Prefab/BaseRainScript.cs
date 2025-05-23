﻿//
// Rain Maker (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
//

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace DigitalRuby.RainMaker
{
    public class BaseRainScript : MonoBehaviour
    {
        [Tooltip("Camera the rain should hover over, defaults to main camera")]
        public Camera Camera;

        [Tooltip("Whether rain should follow the camera. If false, rain must be moved manually and will not follow the camera.")]
        public bool FollowCamera = true;

        [Tooltip("Light rain looping clip")]
        public AudioClip RainSoundLight;

        [Tooltip("Medium rain looping clip")]
        public AudioClip RainSoundMedium;

        [Tooltip("Heavy rain looping clip")]
        public AudioClip RainSoundHeavy;

        [Tooltip("AudioMixer used for the rain sound")]
        public AudioMixerGroup RainSoundAudioMixer;

        [Tooltip("Intensity of rain (0-1)")]
        [Range(0.0f, 1.0f)]
        public float RainIntensity;

        [Tooltip("Rain particle system")]
        public ParticleSystem RainFallParticleSystem;

        [Tooltip("Particles system for when rain hits something")]
        public ParticleSystem RainExplosionParticleSystem;

        [Tooltip("Particle system to use for rain mist")]
        public ParticleSystem RainMistParticleSystem;

        [Tooltip("The threshold for intensity (0 - 1) at which mist starts to appear")]
        [Range(0.0f, 1.0f)]
        public float RainMistThreshold = 0.5f;

        [Tooltip("Wind looping clip")]
        public AudioClip WindSound;

        [Tooltip("Wind sound volume modifier, use this to lower your sound if it's too loud.")]
        public float WindSoundVolumeModifier = 0.5f;

        [Tooltip("Wind zone that will affect and follow the rain")]
        public WindZone WindZone;

        [Tooltip("X = minimum wind speed. Y = maximum wind speed. Z = sound multiplier. Wind speed is divided by Z to get sound multiplier value. Set Z to lower than Y to increase wind sound volume, or higher to decrease wind sound volume.")]
        public Vector3 WindSpeedRange = new Vector3(50.0f, 500.0f, 500.0f);

        [Tooltip("How often the wind speed and direction changes (minimum and maximum change interval in seconds)")]
        public Vector2 WindChangeInterval = new Vector2(5.0f, 30.0f);

        [Tooltip("Whether wind should be enabled.")]
        public bool EnableWind = true;

        // Removed LoopingAudioSource fields since we now use SoundFXManager
        protected Material rainMaterial;
        protected Material rainExplosionMaterial;
        protected Material rainMistMaterial;

        private float lastRainIntensityValue = -1.0f;
        private float nextWindTime;

        private void UpdateWind()
        {
            if (EnableWind && WindZone != null && WindSpeedRange.y > 1.0f)
            {
                WindZone.gameObject.SetActive(true);
                if (FollowCamera)
                {
                    WindZone.transform.position = Camera.transform.position;
                }
                if (!Camera.orthographic)
                {
                    WindZone.transform.Translate(0.0f, WindZone.radius, 0.0f);
                }
                if (nextWindTime < Time.time)
                {
                    // Change wind speed/direction randomly
                    WindZone.windMain = UnityEngine.Random.Range(WindSpeedRange.x, WindSpeedRange.y);
                    WindZone.windTurbulence = UnityEngine.Random.Range(WindSpeedRange.x, WindSpeedRange.y);
                    if (Camera.orthographic)
                    {
                        int val = UnityEngine.Random.Range(0, 2);
                        WindZone.transform.rotation = Quaternion.Euler(0.0f, (val == 0 ? 90.0f : -90.0f), 0.0f);
                    }
                    else
                    {
                        WindZone.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-30.0f, 30.0f),
                                                                         UnityEngine.Random.Range(0.0f, 360.0f),
                                                                         0.0f);
                    }
                    nextWindTime = Time.time + UnityEngine.Random.Range(WindChangeInterval.x, WindChangeInterval.y);
                    
                    // Compute volume based on wind speed and call SoundFXManager
                    float volume = (WindZone.windMain / WindSpeedRange.z) * WindSoundVolumeModifier;
                    SoundFXManager.instance.playSoundFXClip(WindSound, transform, volume);
                }
            }
            else
            {
                // Optionally, if you need to stop wind sounds, you can provide extra logic here
                // (SoundFXManager will auto-destroy spawned audio sources after playing)
            }
        }

        private void CheckForRainChange()
        {
            if (lastRainIntensityValue != RainIntensity)
            {
                lastRainIntensityValue = RainIntensity;
                if (RainIntensity <= 0.01f)
                {
                    // Rain is almost off; stop particle systems
                    if (RainFallParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                        e.enabled = false;
                        RainFallParticleSystem.Stop();
                    }
                    if (RainMistParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                        e.enabled = false;
                        RainMistParticleSystem.Stop();
                    }
                }
                else
                {
                    // Choose appropriate rain sound based on intensity
                    AudioClip clipToPlay;
                    if (RainIntensity >= 0.67f)
                    {
                        clipToPlay = RainSoundHeavy;
                    }
                    else if (RainIntensity >= 0.33f)
                    {
                        clipToPlay = RainSoundMedium;
                    }
                    else
                    {
                        clipToPlay = RainSoundLight;
                    }

                    // Call the SoundFXManager to play the selected rain sound
                    SoundFXManager.instance.playSoundFXClipLooped(clipToPlay, transform, 1f);

                    // Enable and update rain particle systems
                    if (RainFallParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                        e.enabled = RainFallParticleSystem.GetComponent<Renderer>().enabled = true;
                        if (!RainFallParticleSystem.isPlaying)
                        {
                            RainFallParticleSystem.Play();
                        }
                        ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                        rate.mode = ParticleSystemCurveMode.Constant;
                        rate.constantMin = rate.constantMax = RainFallEmissionRate();
                        e.rateOverTime = rate;
                    }
                    if (RainMistParticleSystem != null)
                    {
                        ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                        e.enabled = RainMistParticleSystem.GetComponent<Renderer>().enabled = true;
                        if (!RainMistParticleSystem.isPlaying)
                        {
                            RainMistParticleSystem.Play();
                        }
                        float emissionRate = (RainIntensity < RainMistThreshold) 
                                                ? 0.0f 
                                                : MistEmissionRate();
                        ParticleSystem.MinMaxCurve rate = e.rateOverTime;
                        rate.mode = ParticleSystemCurveMode.Constant;
                        rate.constantMin = rate.constantMax = emissionRate;
                        e.rateOverTime = rate;
                    }
                }
            }
        }

        protected virtual void Start()
        {
#if DEBUG
            if (RainFallParticleSystem == null)
            {
                Debug.LogError("Rain fall particle system must be set to a particle system");
                return;
            }
#endif
            if (Camera == null)
            {
                Camera = Camera.main;
            }

            // Remove audio source initialization; we now use SoundFXManager calls instead.
            // Initialize particle system materials
            if (RainFallParticleSystem != null)
            {
                ParticleSystem.EmissionModule e = RainFallParticleSystem.emission;
                e.enabled = false;
                Renderer rainRenderer = RainFallParticleSystem.GetComponent<Renderer>();
                rainRenderer.enabled = false;
                rainMaterial = new Material(rainRenderer.material);
                rainMaterial.EnableKeyword("SOFTPARTICLES_OFF");
                rainRenderer.material = rainMaterial;
            }
            if (RainExplosionParticleSystem != null)
            {
                ParticleSystem.EmissionModule e = RainExplosionParticleSystem.emission;
                e.enabled = false;
                Renderer rainRenderer = RainExplosionParticleSystem.GetComponent<Renderer>();
                rainExplosionMaterial = new Material(rainRenderer.material);
                rainExplosionMaterial.EnableKeyword("SOFTPARTICLES_OFF");
                rainRenderer.material = rainExplosionMaterial;
            }
            if (RainMistParticleSystem != null)
            {
                ParticleSystem.EmissionModule e = RainMistParticleSystem.emission;
                e.enabled = false;
                Renderer rainRenderer = RainMistParticleSystem.GetComponent<Renderer>();
                rainRenderer.enabled = false;
                rainMistMaterial = new Material(rainRenderer.material);
                if (UseRainMistSoftParticles)
                {
                    rainMistMaterial.EnableKeyword("SOFTPARTICLES_ON");
                }
                else
                {
                    rainMistMaterial.EnableKeyword("SOFTPARTICLES_OFF");
                }
                rainRenderer.material = rainMistMaterial;
            }
        }

        protected virtual void Update()
        {
#if DEBUG
            if (RainFallParticleSystem == null)
            {
                Debug.LogError("Rain fall particle system must be set to a particle system");
                return;
            }
#endif
            CheckForRainChange();
            UpdateWind();
            // With SoundFXManager handling audio playback, no Update calls on audio sources are necessary.
        }

        protected virtual float RainFallEmissionRate()
        {
            return (RainFallParticleSystem.main.maxParticles / RainFallParticleSystem.main.startLifetime.constant) * RainIntensity;
        }

        protected virtual float MistEmissionRate()
        {
            return (RainMistParticleSystem.main.maxParticles / RainMistParticleSystem.main.startLifetime.constant) * RainIntensity * RainIntensity;
        }

        protected virtual bool UseRainMistSoftParticles
        {
            get { return true; }
        }
    }
}
