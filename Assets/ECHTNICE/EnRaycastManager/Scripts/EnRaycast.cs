using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
        CollisionOverrideRaycastHit1,
        CollisionOverrideRaycastHit2,
        CollisionOverrideRaycastHit3,
        NoCollision,
        Ignore
    }

    public enum HeightAxis {
        /// <summary>
        ///   <para>X-axis.</para>
        /// </summary>
        X,
        /// <summary>
        ///   <para>Y-axis.</para>
        /// </summary>
        Y,
        /// <summary>
        ///   <para>Z-axis.</para>
        /// </summary>
        Z,
    }

    public RaycastType m_Type;
    public Expect m_Expect;

    public bool Executed; // 
    public bool Result; // 
    public bool Success; // 

    public int Index; // Index


    public Vector3 m_Origin;
    public Vector3 m_OriginModified;
    public HeightAxis m_HeightAxis;// capsule
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
    public RaycastHit m_RaycastHit1;
    public RaycastHit m_RaycastHit2;
    public RaycastHit m_RaycastHit3;
    public RaycastHit[] m_RaycastAllHits;
    public RaycastHit[] m_RaycastAllNonAllocHits;
    public int m_RaycastAlloNonHitCount;
    public void Clear() {
        Executed = false;
        Result = false;
        Success = false;
        m_RaycastHit1 = default(RaycastHit);
        m_RaycastHit2 = default(RaycastHit);
        m_RaycastHit3 = default(RaycastHit);
        //m_OriginModified = Vector3.zero;
    }

    
    private EnRaycastEventData enRaycastEventData;

    public bool RaycastMethod(EnRaycastManager manager, LayerMask layerMask) {
        if (m_RaycastAllNonAllocHits == null)
            m_RaycastAllNonAllocHits = new RaycastHit[100];

        Transform transform = manager.gameObject.transform;
        Executed = true;

        var worldOrigin = EnDebugExtension.RotatePoint(transform, transform.position + m_Origin);
        var worldDirection = EnDebugExtension.RotateDirection(transform, m_Direction);
        
        if(enRaycastEventData==null)
            enRaycastEventData = new EnRaycastEventData(manager,this);
        enRaycastEventData.Position = worldOrigin;
        enRaycastEventData.Rotation = worldDirection;
        manager.BeforRaycastCheck.Invoke(enRaycastEventData);
        m_OriginModified = enRaycastEventData.Position - worldOrigin;
        worldOrigin = enRaycastEventData.Position;
        worldDirection = enRaycastEventData.Rotation;



        Vector3 axisDirection = Vector3.zero;
        var adapter = manager.GetRaycastAdapter();
        switch (m_Type) {
            case EnRaycast.RaycastType.Raycast:
                Result = Physics.Raycast(worldOrigin, worldDirection, out m_RaycastHit, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.RaycastAll:
                m_RaycastAllHits = Physics.RaycastAll(worldOrigin, worldDirection, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.RaycastNonAlloc:
                m_RaycastAlloNonHitCount = Physics.RaycastNonAlloc(worldOrigin, worldDirection, m_RaycastAllNonAllocHits, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.CapsuleCast:
                switch (m_HeightAxis) {
                    case HeightAxis.X:
                        axisDirection = transform.rotation * Vector3.right * m_Height / 2;
                        break;
                    case HeightAxis.Y:
                        axisDirection = transform.rotation * Vector3.up * m_Height / 2;
                        break;
                    case HeightAxis.Z:
                        axisDirection = transform.rotation * Vector3.forward * m_Height / 2;
                        break;
                }
                Result = Physics.CapsuleCast(worldOrigin + axisDirection,
                    worldOrigin - axisDirection,
                    m_Radius, worldDirection, out m_RaycastHit, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.CapsuleCastAll:
                switch (m_HeightAxis) {
                    case HeightAxis.X:
                        axisDirection = transform.rotation * Vector3.right * m_Height / 2;
                        break;
                    case HeightAxis.Y:
                        axisDirection = transform.rotation * Vector3.up * m_Height / 2;
                        break;
                    case HeightAxis.Z:
                        axisDirection = transform.rotation * Vector3.forward * m_Height / 2;
                        break;
                }
                m_RaycastAllHits = Physics.CapsuleCastAll(worldOrigin + axisDirection,
                    worldOrigin - axisDirection,
                    m_Radius, worldDirection, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.CapsuleCastNonAlloc:

                switch (m_HeightAxis) {
                    case HeightAxis.X:
                        axisDirection = transform.rotation * Vector3.right * m_Height / 2;
                        break;
                    case HeightAxis.Y:
                        axisDirection = transform.rotation * Vector3.up * m_Height / 2;
                        break;
                    case HeightAxis.Z:
                        axisDirection = transform.rotation * Vector3.forward * m_Height / 2;
                        break;
                }
                m_RaycastAlloNonHitCount = Physics.CapsuleCastNonAlloc(worldOrigin + axisDirection,
                    worldOrigin - axisDirection,
                    m_Radius, worldDirection, m_RaycastAllNonAllocHits, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.SphereCast:
                Result = Physics.SphereCast(worldOrigin, m_Radius, worldDirection, out m_RaycastHit, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.SphereCastAll:
                m_RaycastAllHits = Physics.SphereCastAll(worldOrigin, m_Radius, worldDirection, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.SphereCastNonAlloc:
                m_RaycastAlloNonHitCount = Physics.SphereCastNonAlloc(worldOrigin, m_Radius, worldDirection.normalized, m_RaycastAllNonAllocHits, m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.BoxCast:
                Result = Physics.BoxCast(worldOrigin, m_Size, worldDirection, out m_RaycastHit, Quaternion.Euler(worldDirection), m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.BoxCastAll:
                m_RaycastAllHits = Physics.BoxCastAll(worldOrigin, m_Size, worldDirection, Quaternion.Euler(worldDirection), m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
            case EnRaycast.RaycastType.BoxCastNonAlloc:
                m_RaycastAlloNonHitCount = Physics.BoxCastNonAlloc(worldOrigin, m_Size, worldDirection, m_RaycastAllNonAllocHits, Quaternion.Euler(worldDirection), m_MaxDistance, layerMask, manager.m_QueryTriggerInteraction);
                break;
        }

        //result
        switch (m_Type) {
            case RaycastType.Raycast:
            case RaycastType.CapsuleCast:
            case RaycastType.SphereCast:
            case RaycastType.BoxCast:
                if (adapter != null &&
                    manager.m_RaycastAdapter != EnRaycastManager.RaycastAdapter.None &&
                    Result && (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1
                    || m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2
                    || m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)) {

                    if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1)
                        Result = adapter.FilterRaycastHit(manager, this, m_RaycastHit, ref m_RaycastHit1);
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2)
                        Result = adapter.FilterRaycastHit(manager, this, m_RaycastHit, ref m_RaycastHit2);
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)
                        Result = adapter.FilterRaycastHit(manager, this, m_RaycastHit, ref m_RaycastHit3);
                }
                else if(Result) {
                    if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1)
                        m_RaycastHit1 = m_RaycastHit;
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2)
                        m_RaycastHit2 = m_RaycastHit;
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)
                        m_RaycastHit3 = m_RaycastHit;
                }
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                break;
            case RaycastType.RaycastAll:
            case RaycastType.CapsuleCastAll:
            case RaycastType.SphereCastAll:
            case RaycastType.BoxCastAll:
                if (adapter != null &&
                    manager.m_RaycastAdapter != EnRaycastManager.RaycastAdapter.None &&
                    Result && (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1
                               || m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2
                               || m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)) {
                    if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1)
                        Result = adapter.FilterRaycastHitsAll(manager, this, m_RaycastAllHits, ref m_RaycastHit1);
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2)
                        Result = adapter.FilterRaycastHitsAll(manager, this, m_RaycastAllHits, ref m_RaycastHit2);
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)
                        Result = adapter.FilterRaycastHitsAll(manager, this, m_RaycastAllHits, ref m_RaycastHit3);
                }
                else {
                    Result = m_RaycastAllHits.Length > 0;
                    if (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1)
                        m_RaycastHit1 = m_RaycastAllHits.FirstOrDefault();
                    else if (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2)
                        m_RaycastHit2 = m_RaycastAllHits.FirstOrDefault();
                    else if (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)
                        m_RaycastHit3 = m_RaycastAllHits.FirstOrDefault();
                    else if(Result)
                         m_RaycastHit = m_RaycastAllHits.FirstOrDefault();
                }
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                break;
            case RaycastType.RaycastNonAlloc:
            case RaycastType.CapsuleCastNonAlloc:
            case RaycastType.SphereCastNonAlloc:
            case RaycastType.BoxCastNonAlloc:
                if (adapter != null &&
                    manager.m_RaycastAdapter != EnRaycastManager.RaycastAdapter.None &&
                    Result && (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1
                               || m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2
                               || m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)) {
                    if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1)
                        Result = adapter.FilterRaycastHitsAllNonAlloc(manager, this, m_RaycastAlloNonHitCount,
                            m_RaycastAllNonAllocHits, ref m_RaycastHit1);
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2)
                        Result = adapter.FilterRaycastHitsAllNonAlloc(manager, this, m_RaycastAlloNonHitCount,
                            m_RaycastAllNonAllocHits, ref m_RaycastHit2);
                    else if (m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)
                        Result = adapter.FilterRaycastHitsAllNonAlloc(manager, this, m_RaycastAlloNonHitCount,
                            m_RaycastAllNonAllocHits, ref m_RaycastHit3);
                }
                else {

                    Result = m_RaycastAlloNonHitCount > 0;
                    if (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1)
                        m_RaycastHit1 = m_RaycastAllNonAllocHits.FirstOrDefault();
                    else if (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2)
                        m_RaycastHit2 = m_RaycastAllNonAllocHits.FirstOrDefault();
                    else if (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3)
                        m_RaycastHit3 = m_RaycastAllNonAllocHits.FirstOrDefault();
                    else if (Result)
                        m_RaycastHit = m_RaycastAllNonAllocHits.FirstOrDefault();
                }
                Success =
                    (Result && m_Expect == EnRaycast.Expect.Collision) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit1) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit2) ||
                    (Result && m_Expect == EnRaycast.Expect.CollisionOverrideRaycastHit3) ||
                    (!Result && m_Expect == EnRaycast.Expect.NoCollision);
                break;
        }

        return Result;
    }
}