using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Le0der.Toolbox
{
    public class SingleMouseObservation : MonoBehaviour
    {
        [Header("观测的目标")]
        [SerializeField] private Transform target;  //获取旋转目标

        #region UnityMethods
        private void Start()
        {
            InitialState();
            InitRotate();
        }

        private void Update()
        {
            if (!isFocus) return;

            if (!isResting)
            {
                float distanceFactor = distance / distanceLimit.y;
                MoveTarget(distanceFactor);
                Rotate();
                SetZoom(distanceFactor);
                UpdatePosition();
            }
            else
            {
                ResetStateProcess();
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                ResetState();
            }
        }

        private bool isFocus = true;
        private void OnApplicationFocus(bool focusStatus)
        {
            isFocus = focusStatus;
        }

        #endregion

        #region 共有方法
        public void SetTarget(Transform target)
        {
            if (target == null)
            {
                Debug.LogError("target is null");
                return;
            }
            else
            {
                this.target.position = target.position;
            }
        }

        public void SetTarget(Vector3 target)
        {
            this.target.position = target;
        }

        public void ResetState()
        {
            ResetCameraState();
        }
        #endregion

        #region 初始化

        [Space]
        [Header("初始化参数")]

        private float initialDistance;
        private Vector3 initialPosition;
        private Vector3 initialTargetPosition;
        private Quaternion initialRotation;
        private void InitialState()
        {
            initialDistance = Vector3.Distance(transform.position, target.position);
            distance = initialDistance;
            offsetDistance = initialDistance;

            initialPosition = transform.position;
            initialTargetPosition = target.position;
            initialRotation = transform.rotation;
        }

        private bool isResting = false;
        private Vector3 startResetPosition;
        private Vector3 startResetTargetPostion;
        private Quaternion startRotation;

        private void ResetCameraState()
        {
            isResting = true;
            resetingTime = 0;
            startResetPosition = transform.position;
            startResetTargetPostion = target.position;
            startRotation = transform.rotation;
        }

        [SerializeField] private float resetTime = 0.5f;
        private float resetingTime;

        private void ResetStateProcess()
        {
            resetingTime += Time.deltaTime;
            if (resetingTime >= resetTime)
            {
                OnResetedState();
            }

            var process = resetingTime / resetTime;
            transform.position = Vector3.Lerp(startResetPosition, initialPosition, process);
            transform.rotation = Quaternion.Lerp(startRotation, initialRotation, process);
            target.position = Vector3.Lerp(startResetTargetPostion, initialTargetPosition, process);
        }

        private void OnResetedState()
        {
            isResting = false;
            resetingTime = resetTime;

            rotX = initialRotation.eulerAngles.y;
            rotY = initialRotation.eulerAngles.x;

            distance = initialDistance;
            offsetDistance = initialDistance;
        }
        #endregion

        #region 旋转
        [Space]
        [Header("旋转参数")]

        [SerializeField] private float rotateSpeed = 600f;                          // 旋转速度
        [SerializeField] private float smoothnessFactor = 0.5f;                     // 旋转平滑度
        [SerializeField] private Vector2 yRotLimit = new Vector2(-90f, 90f);        //最大旋转角度限制

        private float rotX;                                 // 旋转角度
        private float rotY;                                 // 旋转角度
        private Vector3 rotateLastPosition;                 // 鼠标上一帧的位置

        private void InitRotate()
        {
            rotX = initialRotation.eulerAngles.y;
            rotY = initialRotation.eulerAngles.x;
        }

        private void Rotate()
        {
            if (Input.GetMouseButtonDown(0))
            {
                rotateLastPosition = Input.mousePosition;

            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 currentMousePosition = Input.mousePosition;                     // 获取当前帧的鼠标位置
                Vector3 mouseDelta = currentMousePosition - rotateLastPosition;         // 计算鼠标位置的变化量
                rotateLastPosition = currentMousePosition;                              // 更新上一帧的鼠标位置

                float horz = mouseDelta.x;
                float vert = -mouseDelta.y;

                float step = Time.deltaTime * rotateSpeed;
                rotX += horz * step;
                rotY += vert * step;

                rotY = Mathf.Clamp(rotY, yRotLimit.x, yRotLimit.y);
                Quaternion addRot = Quaternion.Euler(0f, rotX, 0f);
                var targetRotation = addRot * Quaternion.Euler(rotY, 0f, 0f);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothnessFactor);


            }
        }

        #endregion

        #region 平移
        [Space]
        [Header("目标移动参数")]
        [SerializeField] private Vector2 xLimit;
        [SerializeField] private Vector2 yLimit;
        [SerializeField] private Vector2 zLimit;
        [SerializeField] private Vector2 targetSpeedLimit;                // 摄像机目标移动速度

        private Vector3 moveLastPosition;                               // 鼠标上一帧的位置
        private void MoveTarget(float distanceFactor)
        {
            if (Input.GetMouseButtonDown(1))
            {
                moveLastPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 currentMousePosition = Input.mousePosition;                     // 获取当前帧的鼠标位置
                Vector3 mouseDelta = currentMousePosition - moveLastPosition;         // 计算鼠标位置的变化量
                moveLastPosition = currentMousePosition;                              // 更新上一帧的鼠标位置

                float horz = mouseDelta.x;
                float vert = mouseDelta.y;

                Vector3 moveDirection = (transform.right * -horz) + (transform.up * -vert);
                float targetSpeed = Mathf.Lerp(targetSpeedLimit.x, targetSpeedLimit.y, distanceFactor);
                moveDirection *= targetSpeed * 0.001f;
                var position = target.position + moveDirection;

                position.x = Mathf.Clamp(position.x, xLimit.x, xLimit.y);
                position.y = Mathf.Clamp(position.y, yLimit.x, yLimit.y);
                position.z = Mathf.Clamp(position.z, zLimit.x, zLimit.y);

                target.position = position;
            }

        }
        #endregion

        #region 缩放
        [Space]
        [Header("缩放参数")]
        [SerializeField] private float zoomValue = 30f;                             //滑动鼠标滚轮缩放值
        [SerializeField] private Vector2 zoomSpeedLimit = new Vector2(50f, 200f);
        [SerializeField] private Vector2 distanceLimit = new Vector2(10f, 200f);    //缩放限制
        public void SetZoom(float distanceFactor)
        {
            float value = Input.GetAxis("Mouse ScrollWheel");

            var changeSpeed = Mathf.Lerp(zoomSpeedLimit.x, zoomSpeedLimit.y, distanceFactor);

            float delta = value * -zoomValue * changeSpeed;

            distance += delta;

            distance = Mathf.Clamp(distance, distanceLimit.x, distanceLimit.y);
        }
        #endregion

        #region 更新位置

        [Space]
        [Header("位置参数")]

        [SerializeField] private float offsetHeight = 0f;           // 垂直方向上的偏移量，用于调整摄像机与目标高度差
        [SerializeField] private float lateralOffset = 0f;          // 横向偏移量，用于调整摄像机在目标侧面的偏移

        [SerializeField] private float distance = 130f;              // 摄像机与目标之间的理想距离
        [SerializeField] private float zoomSmoothSpeed = 60f;              // 缩放平滑速度
        [SerializeField] private float offsetDistance = 130f;        // 实时更新的摄像机与目标之间的距离，用于实现平滑缩放
        private Vector3 relativePosition = Vector3.zero;            // 存储摄像机相对于目标的最终位置，用于计算更新后的摄像机位置
        private void UpdatePosition()
        {
            offsetDistance = Mathf.MoveTowards(offsetDistance, distance, Time.deltaTime * zoomSmoothSpeed);

            Vector3 targetPosition = target != null ? target.position : Vector3.zero;
            relativePosition = targetPosition + (Vector3.up * offsetHeight) +
                               (transform.rotation * (Vector3.forward * -offsetDistance)) +
                               (transform.right * lateralOffset);

            transform.position = relativePosition;
        }
        #endregion
    }
}