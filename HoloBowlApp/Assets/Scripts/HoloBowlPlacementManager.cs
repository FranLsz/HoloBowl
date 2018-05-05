using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

namespace Assets.Scripts
{
    public class HoloBowlPlacementManager : Singleton<HoloBowlPlacementManager>, IInputClickHandler
    {
        public bool GotTransform { get; set; }

        private void Update()
        {
            // si se ha establecido la posicion, no hacemos nada
            if (GotTransform) return;

            InputManager.Instance.OverrideFocusedObject = gameObject;

            // posición
            transform.position = Vector3.Lerp(transform.position, _proposeTransformPosition(), 0.2f);

            // rotación
            var directionToTarget = Camera.main.transform.position - transform.position;

            directionToTarget.y = 0;

            if (directionToTarget.sqrMagnitude < 0.001f)
                return;

            transform.rotation = Quaternion.LookRotation(-directionToTarget);
        }

        private Vector3 _proposeTransformPosition()
        {
            Vector3 retval;
            RaycastHit hitInfo;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, 30, SpatialMappingManager.Instance.LayerMask))
                retval = hitInfo.point;
            else
            {
                retval = Camera.main.transform.position + Camera.main.transform.forward * 2;
                retval = new Vector3(retval.x, retval.y - .4f, retval.z);
            }

            return retval;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (GotTransform) return;
            InputManager.Instance.OverrideFocusedObject = null;
            GotTransform = true;
        }
    }
}