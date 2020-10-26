using UnityEngine;

public interface IRaycastAdapter
{
    bool FilterRaycastHit(EnRaycastManager manager, EnRaycast enRaycast, RaycastHit hit, ref RaycastHit raycastHit);
    bool FilterRaycastHitsAll(EnRaycastManager manager, EnRaycast enRaycast, RaycastHit[] hits, ref RaycastHit raycastHit);
    bool FilterRaycastHitsAllNonAlloc(EnRaycastManager manager, EnRaycast enRaycast, int hitCount, RaycastHit[] hits, ref RaycastHit raycastHit);
}
