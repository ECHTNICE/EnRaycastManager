using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

// Tells Unity to use this Editor class with the WaveManager script component.
[CustomEditor(typeof(EnRaycastManager))]
public class EnRaycastManagerEditor : Editor {
    //private GUIStyle buttonStyle;

    // This will contain the <wave> array of the WaveManager. 
    SerializedProperty items;

    // The Reorderable List we will be working with 
    ReorderableList list;

    private Color expectedCollisionColor;
    private Color notExpectedCollisionColor;
    private Color expectedNoCollisionColor;

    private void OnEnable() {

        // Get the <wave> array from WaveManager, in SerializedProperty form.
        // Set up the reorderable list

        // Get the <wave> array from WaveManager, in SerializedProperty form.
        // Note that <serializedObject> is a property of the parent Editor class.
        items = serializedObject.FindProperty("items");

        expectedCollisionColor = serializedObject.FindProperty("m_ExpectedCollisionColor").colorValue;
        notExpectedCollisionColor = serializedObject.FindProperty("m_NotExpectedCollisionColor").colorValue;
        expectedNoCollisionColor = serializedObject.FindProperty("m_ExpectedNoCollisionColor").colorValue;


        // Set up the reorderable list       
        list = new ReorderableList(serializedObject, items, true, true, true, true);

        list.drawElementCallback = DrawListItems; // Delegate to draw the elements on the list
        list.drawHeaderCallback = DrawHeader; // Skip this line if you set displayHeader to 'false' in your ReorderableList constructor.
        //Set the height of each element.
        list.elementHeightCallback += ElementHeight;

        list.onSelectCallback += onSelect;
        list.onAddCallback += onAdd;
    }
    void onAdd(ReorderableList itemList) {
        // first add one element
        itemList.serializedProperty.arraySize++;
        // then get that element
        var newIndex = itemList.serializedProperty.arraySize - 1;
        var newElement = itemList.serializedProperty.GetArrayElementAtIndex(newIndex);
        // now reset all properties like
        var color = newElement.FindPropertyRelative("m_Color");
        color.colorValue = Color.black;
        var direction = newElement.FindPropertyRelative("m_Direction");
        direction.vector3Value = Vector3.forward;
        var maxDistance = newElement.FindPropertyRelative("m_MaxDistance");
        maxDistance.floatValue = 1.5f;
        var radius = newElement.FindPropertyRelative("m_Radius");
        radius.floatValue = 0.5f;
        var height = newElement.FindPropertyRelative("m_Height");
        height.floatValue = 0.5f;
        var size = newElement.FindPropertyRelative("m_Size");
        size.vector3Value = Vector3.one;
        var heightAxis = newElement.FindPropertyRelative("m_HeightAxis");
        heightAxis.enumValueIndex = (int)CapsuleBoundsHandle.HeightAxis.Y;
    }

    void Duplicate() {
        if (list.index != -1) {
            EnRaycastManager manager = (EnRaycastManager)serializedObject.targetObject;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);

            list.serializedProperty.arraySize++;
            // then get that element
            var newIndex = list.serializedProperty.arraySize - 1;
            var newElement = list.serializedProperty.GetArrayElementAtIndex(newIndex);

            var typeProp = newElement.FindPropertyRelative("m_Type");
            typeProp.enumValueIndex = element.FindPropertyRelative("m_Type").enumValueIndex;
            var expect = newElement.FindPropertyRelative("m_Expect");
            expect.enumValueIndex = element.FindPropertyRelative("m_Expect").enumValueIndex;
            var color = newElement.FindPropertyRelative("m_Color");
            color.colorValue = element.FindPropertyRelative("m_Color").colorValue;
            var origin = newElement.FindPropertyRelative("m_Origin");
            origin.vector3Value = element.FindPropertyRelative("m_Origin").vector3Value;
            var direction = newElement.FindPropertyRelative("m_Direction");
            direction.vector3Value = element.FindPropertyRelative("m_Direction").vector3Value;
            var maxDistance = newElement.FindPropertyRelative("m_MaxDistance");
            maxDistance.floatValue = element.FindPropertyRelative("m_MaxDistance").floatValue;
            var radius = newElement.FindPropertyRelative("m_Radius");
            radius.floatValue = element.FindPropertyRelative("m_Radius").floatValue;
            var height = newElement.FindPropertyRelative("m_Height");
            height.floatValue = element.FindPropertyRelative("m_Height").floatValue;
            var size = newElement.FindPropertyRelative("m_Size");
            size.vector3Value = element.FindPropertyRelative("m_Size").vector3Value;
            var heightAxis = newElement.FindPropertyRelative("m_HeightAxis");
            heightAxis.enumValueIndex = element.FindPropertyRelative("m_HeightAxis").enumValueIndex;
        }
    }

    void onSelect(ReorderableList itemList) {
        // We when select an item, init the properties list for that item:
        if (0 <= itemList.index && itemList.index < itemList.count) {
            SerializedProperty element = itemList.serializedProperty.GetArrayElementAtIndex(itemList.index);
            Repaint();
        }
        if (serializedObject != null && serializedObject.targetObject != null) {
            EnRaycastManager manager = (EnRaycastManager)serializedObject.targetObject;
            if (manager.m_Editable && manager.m_HideDefaultHandle) {
                Tools.current = Tool.None;
            }
        }
    }

    // Draws the elements on the list
    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused) {
        var defaultFontSize = GUI.skin.button.fontSize;
        GUI.skin.button.fontSize = 9;
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;

        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

        float spacing = EditorGUIUtility.singleLineHeight / 2;
        var currentViewWidth = EditorGUIUtility.currentViewWidth - 60;
        // Create a property field and label field for each property. 

        var typeProp = element.FindPropertyRelative("m_Type");
        var raycastTypeValue = (EnRaycast.RaycastType)typeProp.enumValueIndex;
        // The 'm_Type' property. Since the enum is self-evident, I am not making a label field for it. 
        // The property field for m_Type (width 100, height of a single line)
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y + spacing, currentViewWidth * 0.4f, EditorGUIUtility.singleLineHeight),
            typeProp,
            GUIContent.none
        );

        var expectProp = element.FindPropertyRelative("m_Expect");
        var expectPropValue = (EnRaycast.Expect)typeProp.enumValueIndex;
        EditorGUI.PropertyField(
            new Rect(rect.x + currentViewWidth * 0.4f, rect.y + spacing, currentViewWidth * 0.6f - 50, EditorGUIUtility.singleLineHeight),
            expectProp,
            GUIContent.none
        );

        var executed = element.FindPropertyRelative("Executed").boolValue;
        var result = element.FindPropertyRelative("Result").boolValue;
        var status = element.FindPropertyRelative("Success").boolValue;

        EditorGUI.DrawRect(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2 - spacing / 2,
            currentViewWidth, 2), result ? status ? expectedCollisionColor : notExpectedCollisionColor : expectedNoCollisionColor);

        var controlCount = RaycastTypeControlLength(raycastTypeValue) - 1;
        var heightStatusLine = EditorGUIUtility.singleLineHeight * controlCount + spacing * 3 - 2;
        EditorGUI.DrawRect(new Rect(rect.x - 11, rect.y + EditorGUIUtility.singleLineHeight * 2 - spacing * 2,
            2, heightStatusLine), executed ? status ? expectedCollisionColor : expectedNoCollisionColor : Color.gray);


        EditorGUI.PropertyField(
            new Rect(rect.x + currentViewWidth - 50, rect.y + spacing, 50, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("m_Color"),
            GUIContent.none
        );


        EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Origin Offset");
        EditorGUI.PropertyField(
            new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 2, currentViewWidth * 0.60f - 88, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("m_Origin"),
            GUIContent.none
        );

        //copy
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 88, rect.y + EditorGUIUtility.singleLineHeight * 2, 44, 18), "copy", GUI.skin.button)) {
            string json = JsonUtility.ToJson(element);
            GUIUtility.systemCopyBuffer = json;
        }
        //paste
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 44, rect.y + EditorGUIUtility.singleLineHeight * 2, 44, 18), "paste", GUI.skin.button)) {
            string json = GUIUtility.systemCopyBuffer;
            EnRaycast enRaycast = default(EnRaycast);
            JsonUtility.FromJsonOverwrite(json, enRaycast);
            element.FindPropertyRelative("m_Type").enumValueIndex = (int)enRaycast.m_Type;
            element.FindPropertyRelative("m_Expect").enumValueIndex = (int)enRaycast.m_Expect;
            element.FindPropertyRelative("m_Color").colorValue = enRaycast.m_Color;
            element.FindPropertyRelative("m_Origin").vector3Value = enRaycast.m_Origin;
            element.FindPropertyRelative("m_Direction").vector3Value = enRaycast.m_Direction;
            element.FindPropertyRelative("m_MaxDistance").floatValue = enRaycast.m_MaxDistance;
            element.FindPropertyRelative("m_Radius").floatValue = enRaycast.m_Radius;
            element.FindPropertyRelative("m_Height").floatValue = enRaycast.m_Height;
            element.FindPropertyRelative("m_HeightAxis").enumValueIndex = (int)enRaycast.m_HeightAxis;
            element.FindPropertyRelative("m_Size").vector3Value = enRaycast.m_Size;
        }

        EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 3, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Direction");
        EditorGUI.PropertyField(
            new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 3, currentViewWidth * 0.60f - 88, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("m_Direction"),
            GUIContent.none
        );

        //x
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 88, rect.y + EditorGUIUtility.singleLineHeight * 3, 22, 18), "↔", GUI.skin.button)) {
            element.FindPropertyRelative("m_Direction").vector3Value = element.FindPropertyRelative("m_Direction").vector3Value != Vector3.right ? Vector3.right : Vector3.left;
        }
        //y
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 66, rect.y + EditorGUIUtility.singleLineHeight * 3, 22, 18), "↕", GUI.skin.button)) {
            element.FindPropertyRelative("m_Direction").vector3Value = element.FindPropertyRelative("m_Direction").vector3Value != Vector3.up ? Vector3.up : Vector3.down;
        }
        //z
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 44, rect.y + EditorGUIUtility.singleLineHeight * 3, 22, 18), element.FindPropertyRelative("m_Direction").vector3Value != Vector3.forward ? "→" : "←", GUI.skin.button)) {
            element.FindPropertyRelative("m_Direction").vector3Value = element.FindPropertyRelative("m_Direction").vector3Value != Vector3.forward ? Vector3.forward : Vector3.back;
        }
        //±
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 22, rect.y + EditorGUIUtility.singleLineHeight * 3, 22, 18), "‼", GUI.skin.button)) {
            element.FindPropertyRelative("m_Direction").vector3Value *= -1;
        }

        EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 4, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Max Distance");
        EditorGUI.PropertyField(
            new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 4, currentViewWidth * 0.60f - 88, EditorGUIUtility.singleLineHeight),
            element.FindPropertyRelative("m_MaxDistance"),
            GUIContent.none
        );
        //duplicate
        if (GUI.Button(new Rect(rect.x + currentViewWidth - 88, rect.y + EditorGUIUtility.singleLineHeight * 4, 88, 18), "duplicate", GUI.skin.button)) {
            Duplicate();
        }


        switch (raycastTypeValue) {
            case EnRaycast.RaycastType.Raycast:
            case EnRaycast.RaycastType.RaycastAll:
            case EnRaycast.RaycastType.RaycastNonAlloc:
                break;
            case EnRaycast.RaycastType.SphereCast:
            case EnRaycast.RaycastType.SphereCastAll:
            case EnRaycast.RaycastType.SphereCastNonAlloc:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 5, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Radius");
                EditorGUI.PropertyField(
                    new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 5, currentViewWidth * 0.60f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("m_Radius"),
                    GUIContent.none
                );
                break;
            case EnRaycast.RaycastType.CapsuleCast:
            case EnRaycast.RaycastType.CapsuleCastAll:
            case EnRaycast.RaycastType.CapsuleCastNonAlloc:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 5, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Radius");
                EditorGUI.PropertyField(
                    new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 5, currentViewWidth * 0.60f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("m_Radius"),
                    GUIContent.none
                );

                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 6, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Height");
                EditorGUI.PropertyField(
                    new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 6, currentViewWidth * 0.60f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("m_Height"),
                    GUIContent.none
                );

                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 7, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Axis Direction");

                var heightAxis = element.FindPropertyRelative("m_HeightAxis");
                var heightAxisValue = (CapsuleBoundsHandle.HeightAxis)heightAxis.enumValueIndex;
                EditorGUI.PropertyField(
                    new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 7, currentViewWidth * 0.6f, EditorGUIUtility.singleLineHeight),
                    heightAxis,
                    GUIContent.none
                );

                break;
            case EnRaycast.RaycastType.BoxCast:
            case EnRaycast.RaycastType.BoxCastAll:
            case EnRaycast.RaycastType.BoxCastNonAlloc:
                EditorGUI.LabelField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 5, currentViewWidth * 0.40f, EditorGUIUtility.singleLineHeight), "Size");
                EditorGUI.PropertyField(
                    new Rect(rect.x + currentViewWidth * 0.40f, rect.y + EditorGUIUtility.singleLineHeight * 5, currentViewWidth * 0.60f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("m_Size"),
                    GUIContent.none
                );
                break;
        }
        GUI.skin.button.fontSize = defaultFontSize;
    }

    int RaycastTypeControlLength(EnRaycast.RaycastType raycastTypeValue) {
        switch (raycastTypeValue) {
            case EnRaycast.RaycastType.Raycast:
            case EnRaycast.RaycastType.RaycastAll:
            case EnRaycast.RaycastType.RaycastNonAlloc:
                return 4;
            case EnRaycast.RaycastType.SphereCast:
            case EnRaycast.RaycastType.SphereCastAll:
            case EnRaycast.RaycastType.SphereCastNonAlloc:
            case EnRaycast.RaycastType.BoxCast:
            case EnRaycast.RaycastType.BoxCastAll:
            case EnRaycast.RaycastType.BoxCastNonAlloc:
                return 5;
            case EnRaycast.RaycastType.CapsuleCast:
            case EnRaycast.RaycastType.CapsuleCastAll:
            case EnRaycast.RaycastType.CapsuleCastNonAlloc:
                return 7;
            default:
                return 0;
        }
    }
    //Draws the header
    void DrawHeader(Rect rect) {
        string name = "Raycasts";
        EditorGUI.LabelField(rect, name);
    }

    /// <summary>
    /// Calculates the height of a single element in the list.
    /// This is extremely useful when displaying list-items with nested data.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private float ElementHeight(int index) {

        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list
        float spacing = EditorGUIUtility.singleLineHeight / 2;
        // Create a property field and label field for each property. 
        var typeProp = element.FindPropertyRelative("m_Type");
        var raycastTypeValue = (EnRaycast.RaycastType)typeProp.enumValueIndex;

        //Gets the height of the element. This also accounts for properties that can be expanded, like structs.
        float propertyHeight = EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index), true);

        var controlCount = RaycastTypeControlLength(raycastTypeValue);
        return propertyHeight * controlCount + spacing * 3;
    }

    //This is the function that makes the custom editor work
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        serializedObject.Update(); // Update the array property's representation in the inspector

        list.DoLayoutList(); // Have the ReorderableList do its work

        GUILayout.BeginHorizontal();
        var defaultColor = GUI.color;
        var defaultBackgroundColor = GUI.backgroundColor;
        var editable = ((EnRaycastManager)target).m_Editable;
        GUI.backgroundColor = editable ? Color.gray : Color.green;
        if (GUILayout.Button(editable ? "Disable Editing" : "Editing")) {
            ((EnRaycastManager)target).m_Editable = !editable;
        }
        GUI.color = defaultColor;
        GUI.backgroundColor = defaultBackgroundColor;
        if (GUILayout.Button("Copy")) {
            string json = JsonUtility.ToJson(serializedObject.targetObject);
            GUIUtility.systemCopyBuffer = json;
        }
        if (GUILayout.Button("Paste")) {
            string json = GUIUtility.systemCopyBuffer;
            var gameObject = new GameObject("Temp");
            EnRaycastManager jsonEnRaycastManager = gameObject.AddComponent<EnRaycastManager>();
            JsonUtility.FromJsonOverwrite(json, jsonEnRaycastManager);
            if (jsonEnRaycastManager != null) {
                var manager = ((EnRaycastManager)target);
                manager.items = jsonEnRaycastManager.items.ToArray();
                manager.m_ID = jsonEnRaycastManager.m_ID + 1;
                manager.m_Priority = jsonEnRaycastManager.m_Priority;
                manager.m_LayerMask = jsonEnRaycastManager.m_LayerMask;
                manager.m_RaycastMethodType = jsonEnRaycastManager.m_RaycastMethodType;
                manager.m_QueryTriggerInteraction = jsonEnRaycastManager.m_QueryTriggerInteraction;
                manager.m_RaycastAllwaysAllItems = jsonEnRaycastManager.m_RaycastAllwaysAllItems;
                manager.m_ExpectedCollisionColor = jsonEnRaycastManager.m_ExpectedCollisionColor;
                manager.m_NotExpectedCollisionColor = jsonEnRaycastManager.m_NotExpectedCollisionColor;
                manager.m_ExpectedNoCollisionColor = jsonEnRaycastManager.m_ExpectedNoCollisionColor;
                manager.m_HideDefaultHandle = jsonEnRaycastManager.m_HideDefaultHandle;
                manager.m_ShowRaycastTool = jsonEnRaycastManager.m_ShowRaycastTool;
                manager.m_DrawType = jsonEnRaycastManager.m_DrawType;
                manager.m_DrawWireView = jsonEnRaycastManager.m_DrawWireView;
                manager.m_DrawNormals = jsonEnRaycastManager.m_DrawNormals;
                manager.m_DrawText = jsonEnRaycastManager.m_DrawText;
                manager.m_DrawPointSize = jsonEnRaycastManager.m_DrawPointSize;
            }
            GameObject.DestroyImmediate(gameObject);
        }
        GUILayout.EndHorizontal();

        // We need to call this so that changes on the Inspector are saved by Unity.
        serializedObject.ApplyModifiedProperties();
    }


    private BoxBoundsHandle m_BoxBoundsHandle = new BoxBoundsHandle();
    private SphereBoundsHandle m_SphereBoundsHandle = new SphereBoundsHandle();
    private CapsuleBoundsHandle m_CapsuleBoundsHandle = new CapsuleBoundsHandle();


    public void OnSceneGUI() {
        if (list.index != -1 && serializedObject != null && serializedObject.targetObject != null) {
            EnRaycastManager manager = (EnRaycastManager)serializedObject.targetObject;
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (manager.m_Editable) {
                //EnRaycast enRaycast = element.
                var typeProp = element.FindPropertyRelative("m_Type");
                var raycastTypeValue = (EnRaycast.RaycastType)typeProp.enumValueIndex;

                var origin = element.FindPropertyRelative("m_Origin");
                var direction = element.FindPropertyRelative("m_Direction");
                var maxDistance = element.FindPropertyRelative("m_MaxDistance");
                var radius = element.FindPropertyRelative("m_Radius");
                var height = element.FindPropertyRelative("m_Height");
                var size = element.FindPropertyRelative("m_Size");
                var heightAxis = element.FindPropertyRelative("m_HeightAxis");
                var heightAxisValue = (CapsuleBoundsHandle.HeightAxis)heightAxis.enumValueIndex;

                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;

                var worldOrigin =
                    EnDebugExtension.RotatePoint(manager.transform, manager.transform.position + origin.vector3Value);
                var worldDirection = EnDebugExtension.RotateDirection(manager.transform, direction.vector3Value);

                var position1 = worldOrigin; //go.transform.position + origin.vector3Value;
                var position2 =
                    worldOrigin +
                    worldDirection *
                    maxDistance
                        .floatValue; //go.transform.position + origin.vector3Value + (go.transform.rotation * direction.vector3Value) * maxDistance.floatValue;

                Vector3 destination = Vector3.zero;
                Vector3 heading = Vector3.zero;

                //Origin
                Handles.Label(position1, manager.m_DrawText ? "Origin " + origin.vector3Value : "", style);
                origin.vector3Value =
                    EnDebugExtension.RotatePointInverse(manager.transform,
                        Handles.PositionHandle(position1, Quaternion.identity)) - manager.transform.position;

                //Destination
                Handles.Label(position2, manager.m_DrawText ? "Destination " + position2 : "", style);
                destination =
                    EnDebugExtension.RotatePointInverse(manager.transform,
                        Handles.PositionHandle(position2, Quaternion.identity)) - manager.transform.position;
                heading = destination - origin.vector3Value;
                maxDistance.floatValue = heading.magnitude;
                direction.vector3Value = heading / maxDistance.floatValue; //This is now the normalized direction.

                switch (raycastTypeValue) {
                    case EnRaycast.RaycastType.Raycast:
                    case EnRaycast.RaycastType.RaycastAll:
                    case EnRaycast.RaycastType.RaycastNonAlloc:
                        break;
                    case EnRaycast.RaycastType.CapsuleCast:
                    case EnRaycast.RaycastType.CapsuleCastAll:
                    case EnRaycast.RaycastType.CapsuleCastNonAlloc:
                        m_CapsuleBoundsHandle.center = worldOrigin;
                        m_CapsuleBoundsHandle.radius = radius.floatValue;
                        m_CapsuleBoundsHandle.height = height.floatValue;
                        m_CapsuleBoundsHandle.heightAxis = (CapsuleBoundsHandle.HeightAxis)heightAxis.enumValueIndex;
                        EditorGUI.BeginChangeCheck();
                        m_CapsuleBoundsHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck()) {
                            //Undo.RecordObject(element, "Change Bounds");
                            origin.vector3Value =
                                EnDebugExtension.RotatePointInverse(manager.transform, m_CapsuleBoundsHandle.center) -
                                manager.transform.position;
                            radius.floatValue = m_CapsuleBoundsHandle.radius;
                            height.floatValue = m_CapsuleBoundsHandle.height;
                        }

                        break;
                    case EnRaycast.RaycastType.SphereCast:
                    case EnRaycast.RaycastType.SphereCastAll:
                    case EnRaycast.RaycastType.SphereCastNonAlloc:
                        m_SphereBoundsHandle.center = worldOrigin;
                        m_SphereBoundsHandle.radius = radius.floatValue;
                        EditorGUI.BeginChangeCheck();
                        m_SphereBoundsHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck()) {
                            //Undo.RecordObject(element, "Change Bounds");
                            origin.vector3Value =
                                EnDebugExtension.RotatePointInverse(manager.transform, m_SphereBoundsHandle.center) -
                                manager.transform.position;
                            radius.floatValue = m_SphereBoundsHandle.radius;
                        }

                        break;
                    case EnRaycast.RaycastType.BoxCast:
                    case EnRaycast.RaycastType.BoxCastAll:
                    case EnRaycast.RaycastType.BoxCastNonAlloc:
                        // copy the target object's data to the handle
                        m_BoxBoundsHandle.center = worldOrigin;
                        m_BoxBoundsHandle.size = size.vector3Value;
                        // draw the handle
                        EditorGUI.BeginChangeCheck();
                        m_BoxBoundsHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck()) {
                            // record the target object before setting new values so changes can be undone/redone
                            //Undo.RecordObject((Object)element, "Change Bounds");
                            // copy the handle's updated data back to the target object
                            origin.vector3Value =
                                EnDebugExtension.RotatePointInverse(manager.transform, m_BoxBoundsHandle.center) -
                                manager.transform.position;
                            size.vector3Value = m_BoxBoundsHandle.size;
                            //boundsExample.bounds = newBounds;
                        }

                        break;
                    default:
                        break;
                }

                serializedObject.ApplyModifiedProperties();
            }

            if (serializedObject != null && serializedObject.targetObject != null) {
                Handles.BeginGUI();
                Rect windowRect = new Rect(Screen.width - 215, Screen.height - 180, 200,
                    EditorGUIUtility.singleLineHeight * 2f);
                GUILayout.Window(667, windowRect, DrawWindowInspector, "Raycast Tool");
                Handles.EndGUI();
            }
        }

    }

    void DrawWindowInspector(int windowID) {
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        //GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
        buttonStyle.margin = new RectOffset(0, 5, 2, 0);
        EnRaycastManager manager = (EnRaycastManager)serializedObject.targetObject;
        if (!manager.m_ShowRaycastTool)
            return;

        var defaultColor = GUI.color;
        var defaultBackgroundColor = GUI.backgroundColor;
        var editable = manager.m_Editable;
        GUI.backgroundColor = editable ? Color.gray : Color.green+ Color.green*0.2f;
        if (GUILayout.Button(editable ? "Disable Editing" : "Editing", buttonStyle, GUILayout.Width(200), GUILayout.Height(20))) {
            manager.m_Editable = !editable;
        }
        GUI.color = defaultColor;
        GUI.backgroundColor = defaultBackgroundColor;

        if (GUILayout.Button("Test Raycast", buttonStyle, GUILayout.Width(200), GUILayout.Height(20))) {
            RaycastHit raycastHit = new RaycastHit();
            var item = manager.items[list.index];
            if (item.RaycastMethod(manager, manager.m_LayerMask))
            {
                if (item.Success && item.m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts)
                {
                    manager.m_Collider = item.m_RaycastHit.collider;
                }
            }
            manager.items[list.index] = item;
        }

        if (GUILayout.Button("Test Raycasts", buttonStyle, GUILayout.Width(200), GUILayout.Height(20))) {
            RaycastHit raycastHit = new RaycastHit();
            bool success = true;
            for (int i = 0; i < manager.items.Length; i++) {
                var item = manager.items[i];
                if (item.RaycastMethod(manager, manager.m_LayerMask)) {
                    if (item.Success && item.m_Expect == EnRaycast.Expect.CollisionOverrideRaycasts) {
                        manager.m_RaycastHit = raycastHit;
                        manager.m_Collider = raycastHit.collider;
                    }
                }
                if (!item.Success && success) {
                    success = false;
                }
                manager.items[i] = item;
                if (!success && !manager.m_RaycastAllwaysAllItems) {
                    manager.m_Success = false;
                    break;
                }
            }
            manager.m_Success = success;
        }

        if (list.index != -1) {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(list.index);
            var origin = element.FindPropertyRelative("m_Origin");
            var radius = element.FindPropertyRelative("m_Radius");
            var direction = element.FindPropertyRelative("m_Direction");
            var maxDistance = element.FindPropertyRelative("m_MaxDistance");

            if (GUILayout.Button("Round Values .0", buttonStyle, GUILayout.Width(200), GUILayout.Height(20))) {
                origin.vector3Value = new Vector3(
                    Mathf.Round(origin.vector3Value.x * 10) / 10,
                    Mathf.Round(origin.vector3Value.y * 10) / 10,
                    Mathf.Round(origin.vector3Value.z * 10) / 10
                );
                direction.vector3Value = new Vector3(
                    Mathf.Round(direction.vector3Value.x * 10) / 10,
                    Mathf.Round(direction.vector3Value.y * 10) / 10,
                    Mathf.Round(direction.vector3Value.z * 10) / 10
                );
                maxDistance.floatValue = Mathf.Round(maxDistance.floatValue * 10) / 10;
                radius.floatValue = Mathf.Round(radius.floatValue * 10) / 10;
            }

            if (GUILayout.Button("Round Values .00", buttonStyle, GUILayout.Width(200), GUILayout.Height(20))) {
                origin.vector3Value = new Vector3(
                    Mathf.Round(origin.vector3Value.x * 100) / 100,
                    Mathf.Round(origin.vector3Value.y * 100) / 100,
                    Mathf.Round(origin.vector3Value.z * 100) / 100
                );
                direction.vector3Value = new Vector3(
                    Mathf.Round(direction.vector3Value.x * 100) / 100,
                    Mathf.Round(direction.vector3Value.y * 100) / 100,
                    Mathf.Round(direction.vector3Value.z * 100) / 100
                );
                maxDistance.floatValue = Mathf.Round(maxDistance.floatValue * 100) / 100;
                radius.floatValue = Mathf.Round(radius.floatValue * 100) / 100;
            }
        }

        if (GUILayout.Button("Clear", buttonStyle, GUILayout.Width(200), GUILayout.Height(20))) {
            for (int i = 0; i < manager.items.Length; i++) {
                var item = manager.items[i];
                item.Clear();
                manager.items[i] = item;
            }
        }
    }
}
