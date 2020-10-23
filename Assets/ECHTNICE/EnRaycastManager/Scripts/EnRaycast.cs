using UnityEditor.IMGUI.Controls;
using UnityEngine;

[System.Serializable]
public struct EnRaycast {
    // Declares the Mobs enum type.
    public enum RaycastType {
        Raycast,
        RaycastAll,
        RaycastNonAlloc,
        CapsuleCast,
        CapsuleCastAll,
        CapsuleCastNonAlloc,
        SphereCast,
        SphereCastAll,
        SphereCastNonAlloc,
        BoxCast,
        BoxCastAll,
        BoxCastNonAlloc,

    }

    public enum Expect {
        Collision,
        CollisionOverrideRaycasts,
        NoCollision
    }


    public RaycastType m_Type;
    public Expect m_Expect;

    public bool Executed; // 
    public bool Result; // 
    public bool Success; // 

    public int level; // Level of the enemy.
    public int quantity; // How many enemies of this type should we spawn?


    public Vector3 m_Origin;
    public CapsuleBoundsHandle.HeightAxis m_HeightAxis;// capsule
    public float m_Radius; // SphereCast & 
    public float m_Height; // capsule
    public Vector3 m_Size;
    public Vector3 m_Direction;
    public float m_MaxDistance;



    public Vector3 Destination {
        get => m_Origin + m_Direction * m_MaxDistance;
        set {
            // Gets a vector that points from the player's position to the target's.
            var heading = value - m_Origin;
            m_MaxDistance = heading.magnitude;
            m_Direction = heading / m_MaxDistance; // This is now the normalized direction.
        }
    }


    public Color m_Color;
    public RaycastHit m_RaycastHit;
    public RaycastHit[] m_RaycastHits;

    public void Clear() {
        Executed = false;
        Result = false;
        Success = false;
        m_RaycastHit = default(RaycastHit);
    }

    public bool RaycastMethod(EnRaycastManager manager, Transform transform, LayerMask layerMask, out RaycastHit raycastHit) {
        raycastHit = default(RaycastHit);
        Executed = true;

        var worldOrigin = EnDebugExtension.RotatePoint(transform, transform.position + m_Origin);
        var worldDirection = EnDebugExtension.RotateDirection(transform, m_Direction);

        switch (m_Type) {
            case EnRaycast.RaycastType.Raycast:
                Ray ray = new Ray(worldOrigin, worldDirection);
                Result = Physics.Raycast(ray, out m_RaycastHit, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                if (Result) {
                    raycastHit = m_RaycastHit;
                }
                break;
            case EnRaycast.RaycastType.RaycastAll:
            case EnRaycast.RaycastType.RaycastNonAlloc:

                break;
            case EnRaycast.RaycastType.CapsuleCast:
                Vector3 axisDirection = Vector3.zero;
                switch (m_HeightAxis) {
                    case CapsuleBoundsHandle.HeightAxis.X:
                        axisDirection = transform.rotation * Vector3.right * m_Height / 2;
                        break;
                    case CapsuleBoundsHandle.HeightAxis.Y:
                        axisDirection = transform.rotation * Vector3.up * m_Height / 2;
                        break;
                    case CapsuleBoundsHandle.HeightAxis.Z:
                        axisDirection = transform.rotation * Vector3.forward * m_Height / 2;
                        break;
                }
                Result = Physics.CapsuleCast(worldOrigin + axisDirection,
                    worldOrigin - axisDirection,
                    m_Radius, worldDirection, out m_RaycastHit, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                if (Result) {
                    raycastHit = m_RaycastHit;
                }
                break;
            case EnRaycast.RaycastType.SphereCast:
                Result = Physics.SphereCast(worldOrigin, m_Radius, worldDirection, out m_RaycastHit, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                if (Result) {
                    raycastHit = m_RaycastHit;
                }
                break;
            case EnRaycast.RaycastType.BoxCast:
                Result = Physics.BoxCast(worldOrigin, m_Size, worldDirection, out m_RaycastHit, Quaternion.Euler(worldDirection), m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                if (Result) {
                    raycastHit = m_RaycastHit;
                }
                break;
            default:

                break;
        }

        return Result;
    }
}