/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Infinite Dawn
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright © 2017 Tamerlan Shakirov All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.InputSystem;
using AuroraFPSRuntime.SystemModules;
using AuroraFPSRuntime.SystemModules.ControllerModules;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace AuroraFPSRuntime.WeaponModules
{
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(IAmmoSystem))]
    public abstract class WeaponShootingSystem : MonoBehaviour
    {
        private const float ELAPSED_RECOIL_RATIO = 1.5f;

        [SerializeField]
        [Order(-100)]
        private FireMode fireMode = FireMode.Free | FireMode.Single;

        [SerializeField]
        [ValueDropdown("GetFireModes")]
        [Order(-99)]
        private string startFireMode = FireMode.Free.ToString();

        [SerializeField]
        [NotNull]
        [Order(-98)]
        private Transform firePoint;

        [SerializeField]
        [Label("Single")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [Suffix("rpm", true)]
        [MinValue(0.0f)]
        [Order(99)]
        private float singleRPM = 700f;

        [SerializeField]
        [Label("Free")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [Suffix("rpm", true)]
        [MinValue(0.0f)]
        [Order(100)]
        private float freeRPM = 700f;

        [SerializeField]
        [Label("Queue")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [Suffix("rpm", true)]
        [MinValue(0.0f)]
        [Order(101)]
        private float queueRPM = 700f;

        [SerializeField]
        [Label("Count")]
        [Foldout("Fire Mode Settings", Style = "Header")]
        [MinValue(2)]
        [Indent(1)]
        [Order(102)]
        private int queueCount = 3;

        [SerializeField]
        [Foldout("Recoil Settings", Style = "Header")]
        [Order(103)]
        private RecoilMapping recoilMapping;

        [SerializeField]
        [HideExpandButton]
        [Foldout("Sound Settings", Style = "Header")]
        [Order(104)]
        private FireSounds fireSounds;

        [SerializeField]
        [HideExpandButton]
        [Foldout("Sound Settings", Style = "Header")]
        [Order(105)]
        private FireSounds dryFireSounds;

        [SerializeField]
        [ReorderableList(DisplayHeader = false, ElementLabel = null)]
        [Foldout("Effect Settings", Style = "Header")]
        [Order(106)]
        private ParticleSystem[] particleEffects;

        // Stored required components.
        private AmmoSystem ammoSystem;
        private PlayerController controller;
        private CameraShaker cameraShaker;
        private AudioSource audioSource;
        private IReloadSystem reloadSystem;

        // Stored required properties.
        private bool isAttackKeyPerformed;
        private bool isShooting;
        private bool lastShootingState;
        private Vector2 recoilVector;
        private Vector2 elapsedRecoilVector;
        private float recoilTime;
        private float recoilDuration;
        private bool recoilReturn;
        private int recoilIndex;
        private int spreadIndex;
        private FireMode activeFireMode;
        private CoroutineObject fireModeCoroutine;
        private Vector3 originalFirePointEulerAngles;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            ammoSystem = GetComponent<AmmoSystem>();
            reloadSystem = GetComponent<IReloadSystem>();
            audioSource = GetComponent<AudioSource>();
            controller = GetComponentInParent<PlayerController>();
            cameraShaker = CameraShaker.GetRuntimeInstance();
            fireModeCoroutine = new CoroutineObject(this);
            originalFirePointEulerAngles = firePoint.localEulerAngles;
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected virtual void OnEnable()
        {
            isAttackKeyPerformed = false;
            RegisterInputActions();
            if(Enum.TryParse<FireMode>(startFireMode, out FireMode fireMode))
            {
                ActiveFireMode(fireMode);
            }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        protected virtual void OnDisable()
        {
            isAttackKeyPerformed = false;
            fireModeCoroutine.Stop();
            RemoveInputActions();
        }

        protected virtual void Update()
        {
            CalculateRecoil();
        }

        /// <summary>
        /// Implement this method to make logic of shooting. 
        /// </summary>
        protected abstract void MakeShoot();

        /// <summary>
        /// Weapon single fire mode processing.
        /// </summary>
        protected virtual IEnumerator SingleFireModeProcessing()
        {
            WaitForSeconds fireRate = new WaitForSeconds(ConvertRPMToDelay(singleRPM));
            while (true)
            {
                if (!(reloadSystem?.IsReloading() ?? false) && isAttackKeyPerformed)
                {
                    if (ammoSystem.RemoveAmmo(1))
                    {
                        if (!lastShootingState)
                        {
                            OnBeforeShooting();
                            lastShootingState = true;
                        }
                        if (recoilMapping != null && recoilMapping.TryGetValue(controller.GetState(), out RecoilPattern pattern))
                        {
                            ExecuteRecoil(pattern);
                        }

                        MakeShoot();
                        PlayFireSound();
                        PlayParticleEffects();
                        ResetRecoilChanges();
                        isShooting = true;
                        OnFireCallback?.Invoke();
                    }
                    else
                    {
                        PlayDryFireSound();
                        OnDryFireCallback?.Invoke();
                    }
                    isAttackKeyPerformed = false;
                    yield return fireRate;
                }
                if (!isShooting && lastShootingState)
                {
                    OnAfterShooting();
                    lastShootingState = false;
                }
                isShooting = false;
                yield return null;
            }
        }

        /// <summary>
        /// Weapon queue fire mode processing.
        /// </summary>
        protected virtual IEnumerator QueueFireModeProcessing()
        {
            WaitForSeconds fireRate = new WaitForSeconds(ConvertRPMToDelay(queueRPM));
            while (true)
            {
                if (!(reloadSystem?.IsReloading() ?? false) && isAttackKeyPerformed)
                {
                    int storedQueueCount = queueCount;
                    while (storedQueueCount > 0)
                    {
                        if (ammoSystem.RemoveAmmo(1))
                        {
                            if (!lastShootingState)
                            {
                                OnBeforeShooting();
                                lastShootingState = true;
                            }
                            if (recoilMapping != null && recoilMapping.TryGetValue(controller.GetState(), out RecoilPattern pattern))
                            {
                                ExecuteRecoil(pattern);
                            }
                            MakeShoot();
                            PlayFireSound();
                            PlayParticleEffects();
                            ResetRecoilChanges();
                            storedQueueCount--;
                            isShooting = true;
                            OnFireCallback?.Invoke();
                        }
                        else
                        {
                            PlayDryFireSound();
                            storedQueueCount = 0;
                            OnDryFireCallback?.Invoke();
                        }
                        isAttackKeyPerformed = false;
                        yield return fireRate;
                    }
                }
                if (!isShooting && lastShootingState)
                {
                    OnAfterShooting();
                    lastShootingState = false;
                }
                isShooting = false;
                yield return null;
            }
        }

        /// <summary>
        /// Weapon free fire mode processing.
        /// </summary>
        protected virtual IEnumerator FreeFireModeProcessing()
        {
            WaitForSeconds fireRate = new WaitForSeconds(ConvertRPMToDelay(freeRPM));
            while (true)
            {
                if (!(reloadSystem?.IsReloading() ?? false))
                {
                    if (isAttackKeyPerformed && ammoSystem.RemoveAmmo(1))
                    {
                        if (!lastShootingState)
                        {
                            OnBeforeShooting();
                            lastShootingState = true;
                        }
                        if (recoilMapping != null && recoilMapping.TryGetValue(controller.GetState(), out RecoilPattern pattern))
                        {
                            ExecuteRecoil(pattern);
                        }
                        MakeShoot();
                        PlayFireSound();
                        PlayParticleEffects();
                        ResetRecoilChanges();
                        isShooting = true;
                        OnFireCallback?.Invoke();
                        yield return fireRate;
                    }
                    else if(ammoSystem.GetAmmoCount() == 0 && isAttackKeyPerformed)
                    {
                        PlayDryFireSound();
                        isAttackKeyPerformed = false;
                        OnDryFireCallback?.Invoke();
                        yield return null;
                    }
                }
                if (!isShooting && lastShootingState)
                {
                    OnAfterShooting();
                    lastShootingState = false;
                }
                isShooting = false;
                yield return null;
            }
        }

        /// <summary>
        /// Called before start shooting.
        /// </summary>
        protected virtual void OnBeforeShooting()
        {
            spreadIndex = 0;
            recoilIndex = 0;
            elapsedRecoilVector = Vector2.zero;
            recoilReturn = false;
        }

        /// <summary>
        /// Called after shooting complete.
        /// </summary>
        protected virtual void OnAfterShooting()
        {
            recoilReturn = true;
            recoilTime = recoilDuration;
            recoilVector = Vector2.zero;
        }

        protected virtual void CalculateRecoil()
        {
            if (recoilTime > 0)
            {
                Vector2 recoil = Vector2.zero;
                if (!recoilReturn)
                {
                    recoil.x = recoilVector.x * Time.deltaTime / recoilDuration;
                    recoil.y = recoilVector.y * Time.deltaTime / recoilDuration;
                    if (controller.GetPlayerCamera().GetControlInput() == Vector2.zero)
                        elapsedRecoilVector += recoil / ELAPSED_RECOIL_RATIO;
                }
                else
                {
                    recoil.x = elapsedRecoilVector.x * Time.deltaTime / recoilDuration;
                    recoil.y = elapsedRecoilVector.y * Time.deltaTime / recoilDuration;
                    recoil = -recoil;
                }
                controller.GetPlayerCamera().AddRotation(recoil);
                recoilTime -= Time.deltaTime;
            }
        }

        protected virtual void ExecuteRecoil(RecoilPattern pattern)
        {
            RecoilPattern.BulletSpread spread = pattern.FetchSpread(ref spreadIndex);
            GenerateSpread(spread.xAxis, spread.yAxis);

            recoilVector = pattern.FetchRecoil(ref recoilIndex);
            recoilDuration = pattern.recoilDuration;
            recoilTime = recoilDuration;

            cameraShaker.RegisterShake(new BounceShake(pattern.shakeSettings));
        }

        public virtual void GenerateSpread(Vector2 xAxis, Vector2 yAxis)
        {
            Vector3 accuracy = firePoint.localEulerAngles;
            accuracy.x += Random.Range(xAxis.x, xAxis.y);
            accuracy.y += Random.Range(yAxis.x, yAxis.y);
            firePoint.localEulerAngles = accuracy;
        }

        protected virtual void ResetRecoilChanges()
        {
            firePoint.localEulerAngles = originalFirePointEulerAngles;
        }

        protected virtual void PlayFireSound()
        {
            AudioClip clip = fireSounds.FetchClip();
            if(clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        protected virtual void PlayDryFireSound()
        {
            AudioClip clip = dryFireSounds.FetchClip();
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        protected virtual void PlayParticleEffects()
        {
            for (int i = 0; i < particleEffects.Length; i++)
            {
                ParticleSystem particle = particleEffects[i];
                if(particle != null)
                {
                    particle.Play();
                }
            }
        }

        /// <summary>
        /// Change current fire mode on next.
        /// </summary>
        protected void MoveNextFireMode()
        {
            switch (activeFireMode)
            {
                case FireMode.Mute:
                    if((fireMode & FireMode.Single) != 0)
                    {
                        fireModeCoroutine.Start(SingleFireModeProcessing, true);
                    }
                    break;
                case FireMode.Single:
                    if ((fireMode & FireMode.Queue) != 0)
                    {
                        fireModeCoroutine.Start(QueueFireModeProcessing, true);
                    }
                    break;
                case FireMode.Queue:
                    if ((fireMode & FireMode.Free) != 0)
                    {
                        fireModeCoroutine.Start(FreeFireModeProcessing, true);
                    }
                    break;
                case FireMode.Free:
                    if ((fireMode & FireMode.Single) != 0)
                    {
                        fireModeCoroutine.Start(SingleFireModeProcessing, true);
                    }
                    break;
                default:
                    fireModeCoroutine.Start(SingleFireModeProcessing, true);
                    break;
            }
        }

        public void ActiveFireMode(FireMode fireMode)
        {
            switch (fireMode)
            {
                case FireMode.Mute:
                    fireModeCoroutine.Stop();
                    break;
                case FireMode.Single:
                    fireModeCoroutine.Start(SingleFireModeProcessing, true);
                    break;
                case FireMode.Queue:
                    fireModeCoroutine.Start(QueueFireModeProcessing, true);
                    break;
                case FireMode.Free:
                    fireModeCoroutine.Start(FreeFireModeProcessing, true);
                    break;
                default:
                    fireModeCoroutine.Stop();
                    break;
            }
            activeFireMode = fireMode;
        }

        protected virtual void RegisterInputActions()
        {
            InputReceiver.AttackAction.performed += OnShootAction;
            InputReceiver.AttackAction.canceled += OnShootAction;
            InputReceiver.SwitchFireModeAction.performed += OnFireModeChangedAction;
        }

        protected virtual void RemoveInputActions()
        {
            InputReceiver.AttackAction.performed -= OnShootAction;
            InputReceiver.AttackAction.canceled -= OnShootAction;
            InputReceiver.SwitchFireModeAction.performed -= OnFireModeChangedAction;
        }

        public IEnumerable<string> GetFireModes()
        {
            yield return FireMode.Mute.ToString();
            yield return FireMode.Single.ToString();
            yield return FireMode.Queue.ToString();
            yield return FireMode.Free.ToString();
        }

        #region [Input Action Wrapper]
        private void OnShootAction(InputAction.CallbackContext context)
        {
            if (context.performed)
                isAttackKeyPerformed = true;
            else if (context.canceled)
                isAttackKeyPerformed = false;
        }

        private void OnFireModeChangedAction(InputAction.CallbackContext context)
        {
            MoveNextFireMode();
        }
        #endregion

        #region [Static Methods]
        /// <summary>
        /// Create new fire point for weapon.
        /// </summary>
        public Transform CreateFirePoint()
        {
            Transform camera = Camera.main.transform;
            GameObject point = new GameObject(string.Format("Fire point [{0}]", name));
            point.transform.SetParent(transform);
            point.transform.localPosition = camera.localPosition;
            point.transform.localRotation = transform.localRotation;
            point.transform.localScale = Vector3.one;
            return point.transform;
        }

        public static float ConvertRPMToDelay(float rpm)
        {
            return 1 / (rpm / 60);
        }

        public static WaitForSeconds ConvertRPMToInstruction(float rpm)
        {
            return new WaitForSeconds(1 / (rpm / 60));
        }
        #endregion

        #region [Event Callback Functions]
        /// <summary>
        /// Called when weapon is fired.
        /// </summary>
        public event Action OnFireCallback;

        /// <summary>
        /// Called when weapon is dry fired.
        /// </summary>
        public event Action OnDryFireCallback;
        #endregion

        #region [Getter / Setter]
        public FireMode GetFireMode()
        {
            return fireMode;
        }

        public void SetFireMode(FireMode value)
        {
            fireMode = value;
        }

        public Transform GetFirePoint()
        {
            return firePoint;
        }

        public void SetFirePoint(Transform value)
        {
            firePoint = value;
        }

        public float GetSingleFireRate()
        {
            return singleRPM;
        }

        public void SetSingleFireRate(float value)
        {
            singleRPM = value;
        }

        public float GetFreeFireRate()
        {
            return freeRPM;
        }

        public void SetFreeFireRate(float value)
        {
            freeRPM = value;
        }

        public float GetQueueFireRate()
        {
            return queueRPM;
        }

        public void SetQueueFireRate(float value)
        {
            queueRPM = value;
        }

        public int GetQueueCount()
        {
            return queueCount;
        }

        public void SetQueueCount(int value)
        {
            queueCount = value;
        }

        public FireSounds GetFireSounds()
        {
            return fireSounds;
        }

        public void SetFireSounds(FireSounds value)
        {
            fireSounds = value;
        }

        public FireSounds GetDryFireSounds()
        {
            return dryFireSounds;
        }

        public void SetDryFireSounds(FireSounds value)
        {
            dryFireSounds = value;
        }

        public ParticleSystem[] GetParticleEffects()
        {
            return particleEffects;
        }

        public void SetParticleEffects(ParticleSystem[] value)
        {
            particleEffects = value;
        }

        public bool IsShooting()
        {
            return isShooting;
        }
        #endregion
    }
}