using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Events;

// Add type: https://documentation.help/Rotorz.ReorderableList/aa178158-c17c-4f27-8994-8355807868c0.htm




[CanEditMultipleObjects]
public class EnRaycastManager : MonoBehaviour {



    [Flags]
    public enum DrawType {
        None = 0,
        Gizmos = 1,
        GizmosSelected = 2,
        RunTime = 4,
    }
    public enum RaycastMethodType {
        None,
        Update,
        FixedUpdate,
        LateUpdate,
    }

    public enum RaycastAdapter {
        None,
        ClosestPoint,
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
        Opsive,
#endif
        Custom
    }

    private IRaycastAdapter m_CurrentRaycastAdapter;
    public virtual IRaycastAdapter GetRaycastAdapter() {
        switch (m_RaycastAdapter) {
            case RaycastAdapter.ClosestPoint:
                if (m_CurrentRaycastAdapter == null)
                    m_CurrentRaycastAdapter = new EnClosestPointAdapter();
                return m_CurrentRaycastAdapter;
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
            case RaycastAdapter.Opsive:
                if (m_CurrentRaycastAdapter == null)
                    m_CurrentRaycastAdapter = new EnOpsiveRaycastAdapter();
                return m_CurrentRaycastAdapter;
#endif
            case RaycastAdapter.Custom:
                Debug.LogError("you need to override EnRaycastManager.GetRaycastAdapter method for custom adapter.");
                break;
        }
        return null;
    }

    public int m_ID;
    public int m_Priority;
    public RaycastMethodType m_RaycastMethodType = RaycastMethodType.Update;
    public LayerMask m_LayerMask = ~0;
    public QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.Ignore;
    public bool m_RaycastAllwaysAllItems;


#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
    public RaycastAdapter m_RaycastAdapter = RaycastAdapter.Opsive;
#else
    public RaycastAdapter m_RaycastAdapter;
#endif
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
    public int m_ObjectID = -1;
#endif

    public Color m_ExpectedCollisionColor = Color.green;
    public Color m_NotExpectedCollisionColor = Color.yellow;
    public Color m_ExpectedNoCollisionColor = Color.red;

    [HideInInspector] public bool m_Editable = false;
    public bool m_HideDefaultHandle = false;
    public bool m_ShowRaycastTool = true;

    [Header("Debug Visualization")] public DrawType m_DrawType = DrawType.Gizmos;
    public bool m_DrawWireView;
    public bool m_DrawNormals;
    public bool m_DrawText;
    [Range(0.1f, 1f)]
    public float m_DrawPointSize = 0.5f;

    [Header("Raycast Result")] [EnReadOnly] public bool m_Success;

    [EnReadOnly] public Collider m_Collider1;
    [EnReadOnly] public Collider m_Collider2;
    [EnReadOnly] public Collider m_Collider3;

    public RaycastHit m_RaycastHit1;
    public RaycastHit m_RaycastHit2;
    public RaycastHit m_RaycastHit3;

    public RaycastHit[] m_RaycastHits;

    [HideInInspector]
    public EnRaycast[] items;

    public bool Success {
        get => m_Success;
    }

    public RaycastHit RaycastHit1 {
        get => m_RaycastHit1;
    }
    public RaycastHit RaycastHit2 {
        get => m_RaycastHit2;
    }
    public RaycastHit RaycastHit3 {
        get => m_RaycastHit3;
    }

    public GameObject DetectedObject1 {
        get => m_RaycastHit1.collider ? m_RaycastHit1.collider.gameObject : null;
    }
    public GameObject DetectedObject2 {
        get => m_RaycastHit2.collider ? m_RaycastHit2.collider.gameObject : null;
    }
    public GameObject DetectedObject3 {
        get => m_RaycastHit3.collider ? m_RaycastHit3.collider.gameObject : null;
    }

    // Update is called once per frame
    void Update() {
        if (m_RaycastMethodType == RaycastMethodType.Update) {
            RaycastCheck();
        }
    }

    void FixedUpdate() {
        if (m_RaycastMethodType == RaycastMethodType.FixedUpdate) {
            RaycastCheck();
        }
    }

    void LateUpdate() {
        if (m_RaycastMethodType == RaycastMethodType.LateUpdate) {
            RaycastCheck();
        }
    }

    public string Copy() {
        string json = JsonUtility.ToJson(this);
        GUIUtility.systemCopyBuffer = json;
        return json;
    }

    public void Paste() {
        string json = GUIUtility.systemCopyBuffer;
        Paste(json);
    }

    public void Paste(string json) {
        var gameObject = new GameObject("Temp");
        EnRaycastManager jsonEnRaycastManager = gameObject.AddComponent<EnRaycastManager>();
        JsonUtility.FromJsonOverwrite(json, jsonEnRaycastManager);
        if (jsonEnRaycastManager != null) {
            items = jsonEnRaycastManager.items.ToArray();
            m_ID = jsonEnRaycastManager.m_ID + 1;
            m_Priority = jsonEnRaycastManager.m_Priority;
            m_LayerMask = jsonEnRaycastManager.m_LayerMask;
            m_RaycastMethodType = jsonEnRaycastManager.m_RaycastMethodType;
            m_QueryTriggerInteraction = jsonEnRaycastManager.m_QueryTriggerInteraction;
            m_RaycastAllwaysAllItems = jsonEnRaycastManager.m_RaycastAllwaysAllItems;
            m_ExpectedCollisionColor = jsonEnRaycastManager.m_ExpectedCollisionColor;
            m_NotExpectedCollisionColor = jsonEnRaycastManager.m_NotExpectedCollisionColor;
            m_ExpectedNoCollisionColor = jsonEnRaycastManager.m_ExpectedNoCollisionColor;
            m_HideDefaultHandle = jsonEnRaycastManager.m_HideDefaultHandle;
            m_ShowRaycastTool = jsonEnRaycastManager.m_ShowRaycastTool;
            m_DrawType = jsonEnRaycastManager.m_DrawType;
            m_DrawWireView = jsonEnRaycastManager.m_DrawWireView;
            m_DrawNormals = jsonEnRaycastManager.m_DrawNormals;
            m_DrawText = jsonEnRaycastManager.m_DrawText;
            m_DrawPointSize = jsonEnRaycastManager.m_DrawPointSize;
        }
        GameObject.DestroyImmediate(gameObject);
    }

    public UnityEvent<EnRaycastEventData> BeforRaycastCheck;

    public void Clear() {
        for (int i = 0; i < items.Length; i++) {
            items[i].Clear();
        }
    }

    public bool RaycastCheck() {
        var success = true;
        Clear();
        for (int i = 0; i < items.Length; i++) {
            var item = items[i];
            item.Index = i;
            if (item.RaycastMethod(this, m_LayerMask)) {
                if (item.m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1) {
                    m_RaycastHit1 = item.m_RaycastHit1;
                    m_Collider1 = item.m_RaycastHit1.collider;
                }
                if (item.m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2) {
                    m_RaycastHit2 = item.m_RaycastHit2;
                    m_Collider2 = item.m_RaycastHit2.collider;
                }
                if (item.m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3) {
                    m_RaycastHit3 = item.m_RaycastHit3;
                    m_Collider3 = item.m_RaycastHit3.collider;
                }
            }

            if (!item.Success) {
                success = false;
                if (!m_RaycastAllwaysAllItems) {
                    break;
                }
            }

            items[i] = item;
        }
        m_Success = success;

#if UNITY_EDITOR
        if ((m_DrawType & DrawType.RunTime) == DrawType.RunTime) {
            Draw();
        }
#endif
        return success;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
        if ((m_DrawType & DrawType.GizmosSelected) == DrawType.GizmosSelected) {
            Draw();
        }
    }

    void OnDrawGizmos() {
        if ((m_DrawType & DrawType.Gizmos) == DrawType.Gizmos)
            Draw();
    }

    void Draw() {
        if (items != null)
            for (int i = 0; i < items.Length; i++) {
                var item = items[i];
                Gizmos.color = item.m_Color;
                var radius = item.m_Radius;// < 0.01f ? 0.5f : item.m_Radius;
                var worldOrigin = EnDebugExtension.RotatePoint(transform, transform.position + item.m_Origin);
                var worldDirection = EnDebugExtension.RotateDirection(transform, item.m_Direction);

                Gizmos.DrawLine(worldOrigin,
                    worldOrigin + worldDirection * item.m_MaxDistance
                );
                EnDebugExtension.DrawArrow(worldOrigin + worldDirection * item.m_MaxDistance
                                           - worldDirection * m_DrawPointSize,
                    worldDirection * m_DrawPointSize, Gizmos.color);



                switch (item.m_Type) {
                    case EnRaycast.RaycastType.Raycast:
                    case EnRaycast.RaycastType.RaycastAll:
                    case EnRaycast.RaycastType.RaycastNonAlloc:

                        break;
                    case EnRaycast.RaycastType.SphereCast:
                    case EnRaycast.RaycastType.SphereCastAll:
                    case EnRaycast.RaycastType.SphereCastNonAlloc:
                        if (m_DrawWireView) {
                            Gizmos.DrawWireSphere(worldOrigin, radius);
                        }
                        else {
                            Gizmos.DrawSphere(worldOrigin, radius);
                        }
                        if (m_DrawWireView) {
                            Gizmos.DrawWireSphere(worldOrigin + worldDirection * item.m_MaxDistance, radius);
                        }
                        else {
                            Gizmos.DrawSphere(worldOrigin + worldDirection * item.m_MaxDistance, radius);
                        }
                        break;
                    case EnRaycast.RaycastType.CapsuleCast:
                    case EnRaycast.RaycastType.CapsuleCastAll:
                    case EnRaycast.RaycastType.CapsuleCastNonAlloc:
                        Vector3 axisDirection = Vector3.zero;
                        switch (item.m_HeightAxis) {
                            case CapsuleBoundsHandle.HeightAxis.X:
                                axisDirection = transform.rotation * Vector3.right * item.m_Height / 2;
                                break;
                            case CapsuleBoundsHandle.HeightAxis.Y:
                                axisDirection = transform.rotation * Vector3.up * item.m_Height / 2;
                                break;
                            case CapsuleBoundsHandle.HeightAxis.Z:
                                axisDirection = transform.rotation * Vector3.forward * item.m_Height / 2;
                                break;
                        }
                        EnDebugExtension.DrawCapsule(worldOrigin + axisDirection,
                            worldOrigin - axisDirection,
                                 Gizmos.color, radius);

                        EnDebugExtension.DrawCapsule(worldOrigin + axisDirection + worldDirection * item.m_MaxDistance,
                            worldOrigin - axisDirection + worldDirection * item.m_MaxDistance,
                            Gizmos.color, radius);
                        break;
                    case EnRaycast.RaycastType.BoxCast:
                    case EnRaycast.RaycastType.BoxCastAll:
                    case EnRaycast.RaycastType.BoxCastNonAlloc:
                        if (m_DrawWireView) {
                            Gizmos.DrawWireCube(worldOrigin,
                                 (item.m_Size != Vector3.zero ? item.m_Size : Vector3.one));
                        }
                        else {
                            Gizmos.DrawCube(worldOrigin,
                                 (item.m_Size != Vector3.zero ? item.m_Size : Vector3.one));
                        }

                        if (m_DrawWireView) {
                            Gizmos.DrawWireCube(worldOrigin + worldDirection * item.m_MaxDistance,
                                (item.m_Size != Vector3.zero ? item.m_Size : Vector3.one));
                        }
                        else {
                            Gizmos.DrawCube(worldOrigin + worldDirection * item.m_MaxDistance,
                                 (item.m_Size != Vector3.zero ? item.m_Size : Vector3.one));
                        }
                        break;
                }

                if (item.Result && item.m_RaycastHit1.collider != null) {
                    var color = item.Result ? item.Success ? m_ExpectedCollisionColor : m_NotExpectedCollisionColor : m_ExpectedNoCollisionColor;
                    EnDebugExtension.DrawPoint(item.m_RaycastHit1.point, color, m_DrawPointSize);
                }
                if (m_DrawNormals && item.Result && item.m_RaycastHit1.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit1.point, item.m_RaycastHit1.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                }
                if (m_DrawNormals && m_DrawText && item.Result && item.m_RaycastHit1.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit1.point, item.m_RaycastHit1.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    UnityEditor.Handles.Label(item.m_RaycastHit1.point + Vector3.up * 0.05f, item.m_RaycastHit1.collider.gameObject.name + " " + item.m_RaycastHit1.point, style);
                }

                if (item.Result && item.m_RaycastHit2.collider != null) {
                    var color = item.Result ? item.Success ? m_ExpectedCollisionColor : m_NotExpectedCollisionColor : m_ExpectedNoCollisionColor;
                    EnDebugExtension.DrawPoint(item.m_RaycastHit2.point, color, m_DrawPointSize);
                }
                if (m_DrawNormals && item.Result && item.m_RaycastHit2.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit2.point, item.m_RaycastHit2.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                }
                if (m_DrawNormals && m_DrawText && item.Result && item.m_RaycastHit2.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit2.point, item.m_RaycastHit2.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    UnityEditor.Handles.Label(item.m_RaycastHit2.point + Vector3.up * 0.05f, item.m_RaycastHit2.collider.gameObject.name + " " + item.m_RaycastHit2.point, style);
                }

                if (item.Result && item.m_RaycastHit3.collider != null) {
                    var color = item.Result ? item.Success ? m_ExpectedCollisionColor : m_NotExpectedCollisionColor : m_ExpectedNoCollisionColor;
                    EnDebugExtension.DrawPoint(item.m_RaycastHit3.point, color, m_DrawPointSize);
                }
                if (m_DrawNormals && item.Result && item.m_RaycastHit3.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit3.point, item.m_RaycastHit3.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                }
                if (m_DrawNormals && m_DrawText && item.Result && item.m_RaycastHit3.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit3.point, item.m_RaycastHit3.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    UnityEditor.Handles.Label(item.m_RaycastHit3.point + Vector3.up * 0.05f, item.m_RaycastHit3.collider.gameObject.name + " " + item.m_RaycastHit3.point, style);
                }
            }
    }
#endif
    public static void InitEnRaycastManagerPropertyAttributes(GameObject go, object obj) {
        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var root = go.transform.Find("EnRaycasts");
        if (!root) {
            Debug.LogError("Can't find EnRaycasts GameObject.");
        }

        IEnumerable<EnRaycastManagerPropertyAttribute> attributes;
        for (int i = 0; i < fields.Length; i++) {
            var field = fields[i];
            attributes = field.GetCustomAttributes<EnRaycastManagerPropertyAttribute>();
            foreach (var attr in attributes) {
                if (field.GetValue(obj) != null)
                    break;
                var fieldName = field.Name;
                if (fieldName.StartsWith("m_"))
                    fieldName = fieldName.Remove(0, 2);
                var item = root.transform.Find(fieldName);
                if (!item) {
                    var gameobjectItem = new GameObject(fieldName);
                    gameobjectItem.transform.parent = root;
                    var manager = gameobjectItem.AddComponent<EnRaycastManager>();
                    manager.Paste(attr.Json);
                    field.SetValue(obj, manager);
                }
            }
        }
    }
}
