using UnityEngine;
using UnityEngine.UI;

namespace PadlockSystem
{
    [RequireComponent(typeof(Camera))]
    public class PadlockRaycast : MonoBehaviour
    {
        [Header("Raycast Features")]
        [SerializeField] private float rayLength = 5;
        private PadlockItemController examinableItem;
        private Camera _camera;

        [Header("Crosshair")]
        [SerializeField] private Image uiCrosshair = null;

        public bool IsLookingAtExaminable
        {
            get { return examinableItem != null; }
        }

        void Start()
        {
            _camera = GetComponent<Camera>();
        }

        void Update()
        {
            if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, rayLength))
            {
                var examineItem = hit.collider.GetComponent<PadlockItemController>();
                if (examineItem != null)
                {
                    examinableItem = examineItem;
                    CrosshairChange(true);
                }
                else
                {
                    ClearExaminable();
                }
            }
            else
            {
                ClearExaminable();
            }

            if (IsLookingAtExaminable)
            {
                if (Input.GetKeyDown(PLInputManager.instance.interactKey))
                {
                    examinableItem.ShowPadlock();
                }
            }
        }

        private void ClearExaminable()
        {
            if (examinableItem != null)
            {
                CrosshairChange(false);
                examinableItem = null;
            }
        }

        void CrosshairChange(bool on)
        {
            if (on)
            {
                uiCrosshair.color = Color.red;
            }
            else
            {
                uiCrosshair.color = Color.white;
            }
        }
    }
}
