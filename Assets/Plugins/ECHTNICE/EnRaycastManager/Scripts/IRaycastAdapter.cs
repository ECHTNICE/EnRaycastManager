using UnityEngine;

public interface IRaycastAdapter
{
    bool FindRaycast(EnRaycastManager manager, RaycastHit[] hits, ref RaycastHit raycastHit);
}
