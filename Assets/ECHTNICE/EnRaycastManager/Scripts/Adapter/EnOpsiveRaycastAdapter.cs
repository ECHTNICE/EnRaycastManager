using UnityEngine;
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
//using Opsive.Shared.Game;
#endif

public class EnOpsiveRaycastAdapter : IRaycastAdapter {

    public bool FilterRaycastHit(EnRaycastManager manager, EnRaycast enRaycast, RaycastHit hit, ref RaycastHit raycastHit) {
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
        if (ValidateObject(manager, hit.collider.gameObject)) {
                raycastHit = hit;
                return true;
        }
#endif
        return false;
    }

    public bool FilterRaycastHitsAll(EnRaycastManager manager, EnRaycast enRaycast, RaycastHit[] hits, ref RaycastHit raycastHit) {
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
        for (int i = 0; i < hits.Length; i++) {
            var hitObject = hits[i].collider.gameObject;
            if (ValidateObject(manager, hitObject)) {
                raycastHit = hits[i];
                return true;
            }
        }
#endif
        return false;
    }

    public bool FilterRaycastHitsAllNonAlloc(EnRaycastManager manager, EnRaycast enRaycast, int length, RaycastHit[] hits, ref RaycastHit raycastHit) {
#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
        for (int i = 0; i < length; i++) {
            var hitObject = hits[i].collider.gameObject;
            if (ValidateObject(manager, hitObject)) {
                raycastHit = hits[i];
                return true;
            }
        }
#endif
        return false;
    }

#if FIRST_PERSON_CONTROLLER || THIRD_PERSON_CONTROLLER
    /// <summary>
    /// Validates the object to ensure it is valid for the current ability.
    /// </summary>
    /// <param name="obj">The object being validated.</param>
    /// <returns>True if the object is valid. The object may not be valid if it doesn't have an ability-specific component attached.</returns>
    protected virtual bool ValidateObject(EnRaycastManager manager, GameObject obj) {
        if (obj == null || !obj.activeInHierarchy) {
            return false;
        }

        // If an object id is specified then the object must have the Object Identifier component attached with the specified ID.
        if (manager.m_ObjectID != -1) {
            //var objectIdentifiers = obj.GetCachedParentComponents<Objects.ObjectIdentifier>();
            //if (objectIdentifiers == null) {
            //    return false;
            //}
            //var hasID = false;
            //for (int i = 0; i < objectIdentifiers.Length; ++i) {
            //    if (objectIdentifiers[i].ID == manager.m_ObjectID) {
            //        hasID = true;
            //        break;
            //    }
            //}
            //if (!hasID) {
            //    return false;
            //}
        }
        return true;
    }
#endif
}
