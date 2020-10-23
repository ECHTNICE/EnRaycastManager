using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

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
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
      Opsive,
#endif
    }

    public int m_ID;
    public int m_Priority;
    public RaycastMethodType m_RaycastMethodType = RaycastMethodType.Update;
    public LayerMask m_LayerMask = ~0;
    public QueryTriggerInteraction m_QueryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
    public bool m_RaycastAllwaysAllItems;


#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
    public RaycastAdapter m_RaycastAdapter = RaycastAdapter.Opsive;
#else
    public RaycastAdapter m_RaycastAdapter;
#endif
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
    public int m_ObjectID;
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
    [EnReadOnly] public Collider m_Collider;
    public RaycastHit m_RaycastHit;
    public RaycastHit[] m_RaycastHits;

    [HideInInspector]
    public EnRaycast[] items;


    // Update is called once per frame
    void Update() {
        if (m_RaycastMethodType == RaycastMethodType.Update) {
            RaycastMethod();
        }
    }

    void FixedUpdate() {
        if (m_RaycastMethodType == RaycastMethodType.FixedUpdate) {
            RaycastMethod();
        }
    }

    void LateUpdate() {
        if (m_RaycastMethodType == RaycastMethodType.LateUpdate) {
            RaycastMethod();
        }
    }

    void RaycastMethod() {
        for (int i = 0; i < items.Length; i++) {
            var item = items[i];
            if (item.RaycastMethod(this, transform, m_LayerMask, out m_RaycastHit)) {
                if (item.m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts) {
                    m_Collider = m_RaycastHit.collider;
                    m_RaycastHit = item.m_RaycastHit;
                }
            }
            items[i] = item;
        }
#if UNITY_EDITOR
        if ((m_DrawType & DrawType.RunTime) == DrawType.RunTime) {
            Draw();
        }
#endif
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

                if (item.Result && item.m_RaycastHit.collider != null) {
                    var color = item.Result ? item.Success ? m_ExpectedCollisionColor : m_NotExpectedCollisionColor : m_ExpectedNoCollisionColor;
                    EnDebugExtension.DrawPoint(item.m_RaycastHit.point, color, m_DrawPointSize);
                }
                if (m_DrawNormals && item.Result && item.m_RaycastHit.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit.point, item.m_RaycastHit.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                }
                if (m_DrawNormals && m_DrawText && item.Result && item.m_RaycastHit.collider != null) {
                    EnDebugExtension.DrawArrow(item.m_RaycastHit.point, item.m_RaycastHit.normal * m_DrawPointSize, m_ExpectedCollisionColor);
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.black;
                    UnityEditor.Handles.Label(item.m_RaycastHit.point+Vector3.up*0.05f, item.m_RaycastHit.collider.gameObject.name + " " + item.m_RaycastHit.point, style);
                }
            }
    }
#endif
}
