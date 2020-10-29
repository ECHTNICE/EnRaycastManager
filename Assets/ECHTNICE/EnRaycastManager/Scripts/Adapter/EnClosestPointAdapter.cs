using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
//using Opsive.Shared.Game;
#endif

public class EnClosestPointAdapter : IRaycastAdapter {

    public bool FilterRaycastHit(EnRaycastManager manager, EnRaycast enRaycast, RaycastHit hit, ref RaycastHit raycastHit) {
        raycastHit = hit;
        return true;
    }

    public bool FilterRaycastHitsAll(EnRaycastManager manager, EnRaycast enRaycast, RaycastHit[] hits, ref RaycastHit raycastHit) {
        raycastHit = GetClosestRaycastHit(enRaycast.m_Origin, hits);
        return true;
    }

    public bool FilterRaycastHitsAllNonAlloc(EnRaycastManager manager, EnRaycast enRaycast, int hitCount, RaycastHit[] hits, ref RaycastHit raycastHit) {
        raycastHit = GetClosestRaycastHit(enRaycast.m_Origin, hitCount, hits);
        return true;
    }


    private RaycastHit GetClosestRaycastHit(Vector3 origin, RaycastHit[] hits) {
        // If only one location is available then it is the closest.
        if (hits.Length == 1) {
            return hits[0];
        }

        RaycastHit startLocation = default(RaycastHit);
        var closestDistance = float.MaxValue;
        for (int i = 0; i < hits.Length; ++i) {
            float distance;
            if ((distance = Mathf.Abs(Vector3.Distance(origin, hits[i].point))) < closestDistance) {
                closestDistance = distance;
                startLocation = hits[i];
            }
        }

        return startLocation;
    }
    private RaycastHit GetClosestRaycastHit(Vector3 origin, int hitCount, RaycastHit[] hits) {
        // If only one location is available then it is the closest.
        if (hits.Length == 1) {
            return hits[0];
        }

        RaycastHit startLocation = default(RaycastHit);
        var closestDistance = float.MaxValue;
        for (int i = 0; i < hitCount; ++i) {
            float distance;
            if ((distance = Mathf.Abs(Vector3.Distance(origin, hits[i].point))) < closestDistance) {
                closestDistance = distance;
                startLocation = hits[i];
            }
        }

        return startLocation;
    }


}

