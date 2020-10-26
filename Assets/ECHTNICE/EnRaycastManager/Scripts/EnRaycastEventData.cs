using UnityEngine;

public class EnRaycastEventData {
    public EnRaycastManager Manager { get; set; }
    public EnRaycast EnRaycast { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }

    public EnRaycastEventData(EnRaycastManager manager, EnRaycast cast) {
        Manager = manager;
        EnRaycast = cast;
    }
}