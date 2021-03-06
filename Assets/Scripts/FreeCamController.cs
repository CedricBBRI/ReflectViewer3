using System;
using UnityEngine.Events;
using UnityEngine.Reflect.Controller.Gestures;
using UnityEngine.Reflect.Controller.Gestures.Desktop;
using UnityEngine.Reflect.Controller.Gestures.Touch;
using UnityEngine.UI;

namespace UnityEngine.Reflect.Controller
{
    [Serializable]
    //public class DoubleClickOrTapEvent : UnityEvent<Vector2>
    //{
//    }
    
    public class FreeCamController : Controller
    {
        static readonly float k_ElasticThreshold = 0.05f;
        static readonly float k_ElasticTime = 0.5f;
        static readonly float k_PanMagnitude = 0.1f;

        [Header("Input parameters")]
        public float DesktopScrollSensitivity = 5;
        public float DesktopAltZoomSensitivity = 1;
        public float DesktopPanSensitivity = 0.003f;
        public float DesktopRotateAroundPivotSensitivity = 0.05f;
        public float DesktopRotateCameraSensitivity = 1;
        public Vector2 DesktopMoveSensitivity = Vector2.one;
        public float DesktopShiftFactor = 2;
        public float TouchZoomSensitivity = 100;
        public float TouchZoomThreshold = .01f;
        public float TouchPanSensitivity = 200;
        public float TouchPanThreshold = .03f;
        public float TouchRotateSensitivity = 500;

        public DoubleClickOrTapEvent m_OnDoubleClickOrTap;
        
        float m_DistanceToPivot = 50;
        Vector3 m_CameraRotationEuler;
        Vector3 m_PivotRotationEuler;

        bool m_ElasticReturn = false;
        Vector3 m_ElasticPanPoint;
        Vector3 m_ElasticVelocity;

        public Toggle togglyFly;

        public Vector3 Target
        {
            get
            {
                var t = transform;
                return t.position + t.forward * m_DistanceToPivot;
            }
            set
            {
                UpdatePosition(value);
            }
        }

        void UpdatePosition(Vector3 target)
        {
            var t = transform;
            t.position = target - t.forward * m_DistanceToPivot;
        }

        public void ResetPosition()
        {
            m_DistanceToPivot = 50;
            transform.localRotation = Quaternion.identity;
            UpdatePosition(Vector3.zero);
        }

        void OnDrawGizmos()
        {
            var t = transform;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(t.position + t.forward * m_DistanceToPivot, .1f);
        }

        protected override void StartController(GestureListener listener)
        {
            // Subscribe to desktop events
            var mouseZoom = new MouseScrollGesture(Zoom)
            {
                Multiplier = DesktopScrollSensitivity
            };
            var mouseAltZoom = new MouseMoveGesture(v => Zoom(v.x))
            {
                NeededButtons = new KeyCode[] {
                    KeyCode.LeftAlt,
                    KeyCode.Mouse1
                },
                Multiplier = Vector2.one * DesktopAltZoomSensitivity
            };
            var mousePan = new MouseMoveGesture(Pan)
            {
                NeededButtons = new KeyCode[] {
                    KeyCode.Mouse2
                },
                Multiplier = - Vector2.one * DesktopPanSensitivity
            };
            var mouseLeftClickRotate = new MouseMoveGesture(RotateAroundPivot)
            {
                NeededButtons = new KeyCode[] {
                    KeyCode.Mouse0
                },
                Multiplier = Vector2.one * DesktopRotateAroundPivotSensitivity
            };
            mouseLeftClickRotate.startMove += StartRotateAroundPivot;
            var mouseRotateCamera = new MouseMoveGesture(RotateCamera)
            {
                NeededButtons = new KeyCode[] {
                    KeyCode.Mouse1
                },
                ExcludedButtons = new KeyCode[] {
                    KeyCode.LeftAlt
                },
                Multiplier = Vector2.one * DesktopRotateCameraSensitivity,
            };
            mouseRotateCamera.startMove += StartRotateCamera;
            var moveCamera = new DirectionButtonsGesture(MoveCamera) {
                Multiplier = DesktopMoveSensitivity,
                ExcludedButtons = new KeyCode[]
                {
                    KeyCode.LeftShift
                }
            };
            var moveCameraDouble = new DirectionButtonsGesture(MoveCamera)
            {
                Multiplier = DesktopMoveSensitivity * DesktopShiftFactor,
                NeededButtons = new KeyCode[]
                {
                    KeyCode.LeftShift
                }
            };
            var mouseTeleport = new MouseClickGesture(OnDoubleClickOrTap)
            {
                ClickNumber = 2
            };
            listener.AddListeners(mouseZoom, mouseAltZoom, mousePan, mouseLeftClickRotate, mouseRotateCamera, 
                moveCamera, moveCameraDouble, mouseTeleport);

            // Subscribe to touch events
            var touchZoom = new TouchPinchGesture(ZoomMobile)
            {
                Multiplier = TouchZoomSensitivity,
                DetectionThreshold = TouchZoomThreshold
            };
            var touchPan = new TouchPanGesture(Pan)
            {
                FingersNumber = 2,
                Multiplier = - Vector2.one * TouchPanSensitivity,
                DetectionThreshold = TouchPanThreshold
            };
            var touchRotate = new TouchPanGesture(RotateAroundPivot)
            {
                Multiplier = Vector2.one * TouchRotateSensitivity
            };
            touchRotate.onPanStart += StartRotateAroundPivot;
            var touchTeleport = new TouchMultiTapGesture(OnDoubleClickOrTap)
            {
                TapNumber = 2
            };
            listener.AddListeners(touchZoom, touchPan, touchRotate, touchTeleport);
        }

        void Zoom(float amount)
        {
            m_DistanceToPivot = Mathf.Max(m_DistanceToPivot - amount, 0);
            transform.position += transform.forward * amount;
        }

        void ZoomMobile(float amount)
        {
            var delta = Mathf.Min(m_DistanceToPivot, amount);
            m_DistanceToPivot -= delta;
            transform.position += transform.forward * delta;
        }

        void StartElasticPan()
        {
            if (!m_ElasticReturn)
            {
                m_ElasticPanPoint = Target;
                m_ElasticReturn = false;
            }
        }

        void StopElasticPan()
        {
            m_ElasticVelocity = Vector3.zero;
            m_ElasticReturn = true;
        }

        void Pan(Vector2 delta)
        {
            var magnitude = (m_DistanceToPivot + 1) * k_PanMagnitude;
            transform.position += magnitude * (transform.right * delta.x + transform.up * delta.y);
        }

        void StartRotateAroundPivot()
        {
            var rotation = Quaternion.FromToRotation(Vector3.forward, - transform.forward);
            m_PivotRotationEuler = NormalizeEulerAngles(rotation.eulerAngles);
        }

        void RotateAroundPivot(Vector2 delta)
        {
            var target = Target;
            m_PivotRotationEuler = ComputeNewEulerAngles(delta.x, delta.y, m_PivotRotationEuler);
            var rotation = Quaternion.Euler(m_PivotRotationEuler);
            var rotationDirection = rotation * Vector3.forward;

            // Handle rotation
            transform.position = target + rotationDirection;
            transform.LookAt(target);

            // Handle position
            transform.position = target + rotationDirection * m_DistanceToPivot;
        }

        void StartRotateCamera()
        {
            var rotation = Quaternion.FromToRotation(Vector3.forward, transform.forward);
            m_CameraRotationEuler = NormalizeEulerAngles(rotation.eulerAngles);
        }

        void RotateCamera(Vector2 delta)
        {
            m_CameraRotationEuler = ComputeNewEulerAngles(delta.x, -delta.y, m_CameraRotationEuler);
            var rotation = Quaternion.Euler(m_CameraRotationEuler);
            transform.forward = rotation * Vector3.forward;
        }

        void MoveCamera(Vector2 direction)
        {
            transform.position += transform.forward * direction.y + transform.right * direction.x;
        }

        void OnDoubleClickOrTap(Vector2 position)
        {
            m_OnDoubleClickOrTap?.Invoke(position);
        }

        protected override void UpdateController()
        {
            if (m_ElasticReturn)
            {
                if ((Target - m_ElasticPanPoint).magnitude > k_ElasticThreshold)
                {
                    Target = Vector3.SmoothDamp(Target, m_ElasticPanPoint, ref m_ElasticVelocity, k_ElasticTime, float.MaxValue, Time.fixedDeltaTime);
                }
                else
                {
                    m_ElasticReturn = false;
                    Target = m_ElasticPanPoint;
                }
            }
        }
        public void AddPhysics()
        {
            var t = transform;
            Rigidbody rb = t.gameObject.GetComponent<Rigidbody>();
            if (togglyFly.isOn)
            {
                rb.isKinematic = true;
                rb.detectCollisions = false;
                rb.useGravity = false;
            }
            else
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;
                rb.useGravity = true;
            }
        }
    }
}
