using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 相机系统唯一入口。
/// 对外统一暴露基础相机能力，底层优先对接 Cinemachine，缺失时退回原生 Camera。
/// </summary>
public sealed class CameraManager {
    private static readonly CameraManager INSTANCE = new CameraManager();
    private const float DEFAULT_FOLLOW_DAMP = 8f;
    private const float DEFAULT_LOOK_DAMP = 12f;
    private const int ACTIVE_PRIORITY = 100;
    private const int INACTIVE_PRIORITY = 10;

    private readonly Dictionary<string, CameraTag> cameraTagDict;
    private readonly Dictionary<string, Component> virtualCameraDict;

    private Camera mainCamera;
    private Component cinemachineBrain;
    private Transform followTarget;
    private Transform lookAtTarget;
    private Vector3 followOffset;
    private Vector3 shakeOffset;
    private float shakeTimer;
    private float shakeDuration;
    private float shakeIntensity;
    private string currentCameraId;
    private bool hasCinemachine;
    private bool isInitialized;

    public static CameraManager Instance {
        get {
            return INSTANCE;
        }
    }

    public Camera MainCamera {
        get {
            return mainCamera;
        }
    }

    private CameraManager() {
        cameraTagDict = new Dictionary<string, CameraTag>(8);
        virtualCameraDict = new Dictionary<string, Component>(8);
        followOffset = new Vector3(0f, 4.5f, -8f);
        currentCameraId = string.Empty;
    }

    public void Initialize() {
        if (isInitialized) {
            RefreshSceneCamera();
            return;
        }

        RefreshSceneCamera();
        isInitialized = true;
    }

    public void Clear() {
        cameraTagDict.Clear();
        virtualCameraDict.Clear();
        mainCamera = null;
        cinemachineBrain = null;
        followTarget = null;
        lookAtTarget = null;
        shakeOffset = Vector3.zero;
        shakeTimer = 0f;
        shakeDuration = 0f;
        shakeIntensity = 0f;
        currentCameraId = string.Empty;
        hasCinemachine = false;
        isInitialized = false;
    }

    public void Tick(float delta) {
        if (!isInitialized || mainCamera == null) {
            return;
        }

        UpdateShake(delta);

        if (hasCinemachine) {
            return;
        }

        UpdateUnityCamera(delta);
    }

    public void RefreshSceneCamera() {
        mainCamera = Camera.main;
        if (mainCamera == null) {
            mainCamera = UnityEngine.Object.FindObjectOfType<Camera>();
        }

        cameraTagDict.Clear();
        virtualCameraDict.Clear();
        cinemachineBrain = FindCinemachineBrain();
        hasCinemachine = cinemachineBrain != null;

        CameraTag[] cameraTags = UnityEngine.Object.FindObjectsOfType<CameraTag>(true);
        for (int i = 0; i < cameraTags.Length; ++i) {
            CameraTag cameraTag = cameraTags[i];
            if (cameraTag == null || string.IsNullOrEmpty(cameraTag.CameraId)) {
                continue;
            }

            cameraTagDict[cameraTag.CameraId] = cameraTag;
            Component virtualCamera = FindVirtualCameraComponent(cameraTag.gameObject);
            if (virtualCamera != null) {
                virtualCameraDict[cameraTag.CameraId] = virtualCamera;
            }
        }

        ApplyTargetsToVirtualCameras();
        if (!string.IsNullOrEmpty(currentCameraId)) {
            SwitchCamera(currentCameraId);
        }
    }

    public void SetFollowTarget(Transform target) {
        followTarget = target;
        ApplyTargetsToVirtualCameras();
    }

    public void SetLookAtTarget(Transform target) {
        lookAtTarget = target;
        ApplyTargetsToVirtualCameras();
    }

    public void SetFollowOffset(Vector3 offset) {
        followOffset = offset;
    }

    public void SetPosition(Vector3 position) {
        if (mainCamera == null) {
            return;
        }

        mainCamera.transform.position = position;
    }

    public void SetRotation(Quaternion rotation) {
        if (mainCamera == null) {
            return;
        }

        mainCamera.transform.rotation = rotation;
    }

    public void SetFov(float value) {
        if (mainCamera == null) {
            return;
        }

        mainCamera.fieldOfView = Mathf.Clamp(value, 1f, 179f);
    }

    public void SwitchCamera(string cameraId) {
        currentCameraId = cameraId ?? string.Empty;
        if (string.IsNullOrEmpty(currentCameraId)) {
            return;
        }

        if (hasCinemachine) {
            foreach (KeyValuePair<string, Component> pair in virtualCameraDict) {
                int priority = pair.Key == currentCameraId ? ACTIVE_PRIORITY : INACTIVE_PRIORITY;
                SetPriority(pair.Value, priority);
            }
            return;
        }

        CameraTag cameraTag;
        if (!cameraTagDict.TryGetValue(currentCameraId, out cameraTag) || cameraTag == null || mainCamera == null) {
            return;
        }

        mainCamera.transform.position = cameraTag.transform.position;
        mainCamera.transform.rotation = cameraTag.transform.rotation;
    }

    public void Shake(float intensity, float duration) {
        shakeIntensity = Mathf.Max(0f, intensity);
        shakeDuration = Mathf.Max(0f, duration);
        shakeTimer = shakeDuration;
    }

    private void UpdateUnityCamera(float delta) {
        if (mainCamera == null) {
            return;
        }

        if (followTarget != null) {
            Vector3 targetPosition = followTarget.position + followOffset + shakeOffset;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, delta * DEFAULT_FOLLOW_DAMP);
        } else if (shakeOffset != Vector3.zero) {
            mainCamera.transform.position += shakeOffset * delta;
        }

        if (lookAtTarget != null) {
            Vector3 lookDirection = lookAtTarget.position - mainCamera.transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f) {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
                mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, targetRotation, delta * DEFAULT_LOOK_DAMP);
            }
        }
    }

    private void UpdateShake(float delta) {
        if (shakeTimer <= 0f) {
            shakeOffset = Vector3.zero;
            return;
        }

        shakeTimer -= delta;
        float factor = shakeDuration <= 0f ? 0f : Mathf.Clamp01(shakeTimer / shakeDuration);
        shakeOffset = UnityEngine.Random.insideUnitSphere * shakeIntensity * factor;
    }

    private void ApplyTargetsToVirtualCameras() {
        if (!hasCinemachine) {
            return;
        }

        foreach (KeyValuePair<string, Component> pair in virtualCameraDict) {
            Component virtualCamera = pair.Value;
            if (virtualCamera == null) {
                continue;
            }

            SetObjectMember(virtualCamera, "Follow", followTarget);
            SetObjectMember(virtualCamera, "LookAt", lookAtTarget);
        }
    }

    private static Component FindCinemachineBrain() {
        Component[] components = UnityEngine.Object.FindObjectsOfType<Component>(true);
        for (int i = 0; i < components.Length; ++i) {
            Component component = components[i];
            if (component == null) {
                continue;
            }

            Type componentType = component.GetType();
            if (componentType.FullName == "Cinemachine.CinemachineBrain") {
                return component;
            }
        }

        return null;
    }

    private static Component FindVirtualCameraComponent(GameObject targetObject) {
        if (targetObject == null) {
            return null;
        }

        Component[] components = targetObject.GetComponents<Component>();
        for (int i = 0; i < components.Length; ++i) {
            Component component = components[i];
            if (component == null) {
                continue;
            }

            Type componentType = component.GetType();
            string fullName = componentType.FullName;
            if (string.IsNullOrEmpty(fullName)) {
                continue;
            }

            if (fullName.StartsWith("Cinemachine.") &&
                (componentType.Name.Contains("VirtualCamera") || componentType.Name == "CinemachineCamera")) {
                return component;
            }
        }

        return null;
    }

    private static void SetPriority(Component targetComponent, int priority) {
        if (targetComponent == null) {
            return;
        }

        if (TrySetMember(targetComponent, "Priority", priority)) {
            return;
        }

        TrySetMember(targetComponent, "m_Priority", priority);
    }

    private static void SetObjectMember(Component targetComponent, string memberName, UnityEngine.Object targetObject) {
        if (targetComponent == null) {
            return;
        }

        TrySetMember(targetComponent, memberName, targetObject);
    }

    private static bool TrySetMember(Component targetComponent, string memberName, object value) {
        Type componentType = targetComponent.GetType();
        PropertyInfo propertyInfo = componentType.GetProperty(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propertyInfo != null && propertyInfo.CanWrite && IsAssignable(propertyInfo.PropertyType, value)) {
            propertyInfo.SetValue(targetComponent, value, null);
            return true;
        }

        FieldInfo fieldInfo = componentType.GetField(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null && IsAssignable(fieldInfo.FieldType, value)) {
            fieldInfo.SetValue(targetComponent, value);
            return true;
        }

        return false;
    }

    private static bool IsAssignable(Type targetType, object value) {
        if (value == null) {
            return !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) != null;
        }

        return targetType.IsInstanceOfType(value);
    }
}
