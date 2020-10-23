using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

public static class EnHandlesExtended {

	#region Handes Draw Functions

	public static void DrawScaledCap(Handles.CapFunction capFunction, Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		Handles.color = color;

		Matrix4x4 scaleMatrix = Matrix4x4.Scale(size);

		using (new Handles.DrawingScope(scaleMatrix)) {
			capFunction(0, center, rotation, 1, EventType.Repaint);
		}
	}

	public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size) { DrawWireCube(center, rotation, size, Handles.color); }
	public static void DrawWireCube(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);

		Handles.color = color;

		using (new Handles.DrawingScope(rotationMatrix)) {
			Handles.DrawWireCube(center, size);
		}
	}

	public static void DrawSolidCube(Vector3 center, Quaternion rotation, Vector3 size) { DrawSolidCube(center, rotation, size, Handles.color); }
	public static void DrawSolidCube(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.CubeHandleCap, center, rotation, size, color);
	}

	public static void DrawCylinder(Vector3 center, Quaternion rotation, Vector3 size) { DrawCylinder(center, rotation, size, Handles.color); }
	public static void DrawCylinder(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.CylinderHandleCap, center, rotation, size, color);
	}

	public static void DrawCone(Vector3 center, Quaternion rotation, Vector3 size) { DrawCone(center, rotation, size, Handles.color); }
	public static void DrawCone(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.ConeHandleCap, center, rotation, size, color);
	}

	public static void DrawCircle(Vector3 center, Quaternion rotation, Vector3 size) { DrawCircle(center, rotation, size, Handles.color); }
	public static void DrawCircle(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.CircleHandleCap, center, rotation, size, color);
	}

	public static void DrawArrow(Vector3 center, Quaternion rotation, Vector3 size) { DrawArrow(center, rotation, size, Handles.color); }
	public static void DrawArrow(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.ArrowHandleCap, center, rotation, size, color);
	}

	public static void DrawRectange(Vector3 center, Quaternion rotation, Vector3 size) { DrawRectange(center, rotation, size, Handles.color); }
	public static void DrawRectange(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.RectangleHandleCap, center, rotation, size, color);
	}

	public static void DrawSphere(Vector3 center, Quaternion rotation, Vector3 size) { DrawSphere(center, rotation, size, Handles.color); }
	public static void DrawSphere(Vector3 center, Quaternion rotation, Vector3 size, Color color) {
		DrawScaledCap(Handles.SphereHandleCap, center, rotation, size, color);
	}

	#endregion

	#region Handles controls Functions

	static ArcHandle arcHandle = new ArcHandle();
	static BoxBoundsHandle boxBoundsHandle = new BoxBoundsHandle();
	static CapsuleBoundsHandle capsuleBoundsHandle = new CapsuleBoundsHandle();
	static JointAngularLimitHandle jointAngularLimitHandle = new JointAngularLimitHandle();
	static SphereBoundsHandle sphereBoundsHandle = new SphereBoundsHandle();

	public static void ArcHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float angle, ref float radius) { ArcHandle(center, rotation, size, ref angle, ref radius, arcHandle.fillColor, arcHandle.wireframeColor); }
	public static void ArcHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float angle, ref float radius, Color fillColor, Color wireframeColor) {
		Matrix4x4 trs = Matrix4x4.TRS(center, rotation, size);

		using (new Handles.DrawingScope(trs)) {
			arcHandle.angle = angle;
			arcHandle.radius = radius;
			arcHandle.radiusHandleColor = Color.white;
			arcHandle.fillColor = fillColor;
			arcHandle.wireframeColor = wireframeColor;
			arcHandle.DrawHandle();
			angle = arcHandle.angle;
			radius = arcHandle.radius;
		}
	}

	public static void BoxBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 boxSize) { BoxBoundsHandle(center, rotation, size, ref boxSize, PrimitiveBoundsHandle.Axes.All); }
	public static void BoxBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 boxSize, PrimitiveBoundsHandle.Axes handleAxes) { BoxBoundsHandle(center, rotation, size, ref boxSize, handleAxes, boxBoundsHandle.wireframeColor, boxBoundsHandle.handleColor); }
	public static void BoxBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 boxSize, PrimitiveBoundsHandle.Axes handleAxes, Color wireframeColor, Color handleColor) {
		Matrix4x4 trs = Matrix4x4.TRS(center, rotation, size);

		using (new Handles.DrawingScope(trs)) {
			boxBoundsHandle.axes = handleAxes;
			boxBoundsHandle.size = boxSize;
			boxBoundsHandle.handleColor = handleColor;
			boxBoundsHandle.wireframeColor = wireframeColor;
			boxBoundsHandle.DrawHandle();
			boxSize = boxBoundsHandle.size;
		}
	}

	public static void CapsuleBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float height, ref float radius) { CapsuleBoundsHandle(center, rotation, size, ref height, ref radius, capsuleBoundsHandle.heightAxis, PrimitiveBoundsHandle.Axes.All); }
	public static void CapsuleBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float height, ref float radius, CapsuleBoundsHandle.HeightAxis heightAxis, PrimitiveBoundsHandle.Axes handleAxes) { CapsuleBoundsHandle(center, rotation, size, ref height, ref radius, heightAxis, handleAxes, capsuleBoundsHandle.handleColor, capsuleBoundsHandle.wireframeColor); }
	public static void CapsuleBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float height, ref float radius, CapsuleBoundsHandle.HeightAxis heightAxis, PrimitiveBoundsHandle.Axes handleAxes, Color handleColor, Color wireframeColor) {
		Matrix4x4 trs = Matrix4x4.TRS(center, rotation, size);

		using (new Handles.DrawingScope(trs)) {
			capsuleBoundsHandle.heightAxis = heightAxis;
			capsuleBoundsHandle.axes = handleAxes;
			capsuleBoundsHandle.radius = radius;
			capsuleBoundsHandle.height = height;
			capsuleBoundsHandle.handleColor = handleColor;
			capsuleBoundsHandle.wireframeColor = wireframeColor;
			capsuleBoundsHandle.DrawHandle();
			radius = capsuleBoundsHandle.radius;
			height = capsuleBoundsHandle.height;
		}
	}

	public static void JointAngularLimitHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 minAngles, ref Vector3 maxAngles) { JointAngularLimitHandle(center, rotation, size, ref minAngles, ref maxAngles, jointAngularLimitHandle.xHandleColor, jointAngularLimitHandle.yHandleColor, jointAngularLimitHandle.zHandleColor); }
	public static void JointAngularLimitHandle(Vector3 center, Quaternion rotation, Vector3 size, ref Vector3 minAngles, ref Vector3 maxAngles, Color xHandleColor, Color yHandleColor, Color zHandleColor) {
		Matrix4x4 trs = Matrix4x4.TRS(center, rotation, size);

		using (new Handles.DrawingScope(trs)) {
			jointAngularLimitHandle.xHandleColor = xHandleColor;
			jointAngularLimitHandle.yHandleColor = yHandleColor;
			jointAngularLimitHandle.zHandleColor = zHandleColor;

			jointAngularLimitHandle.xMin = minAngles.x;
			jointAngularLimitHandle.yMin = minAngles.y;
			jointAngularLimitHandle.zMin = minAngles.z;
			jointAngularLimitHandle.xMax = maxAngles.x;
			jointAngularLimitHandle.yMax = maxAngles.y;
			jointAngularLimitHandle.zMax = maxAngles.z;

			jointAngularLimitHandle.DrawHandle();

			minAngles.x = jointAngularLimitHandle.xMin;
			minAngles.y = jointAngularLimitHandle.yMin;
			minAngles.z = jointAngularLimitHandle.zMin;
			maxAngles.x = jointAngularLimitHandle.xMax;
			maxAngles.y = jointAngularLimitHandle.yMax;
			maxAngles.z = jointAngularLimitHandle.zMax;
		}
	}

	public static void SphereBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float radius) { SphereBoundsHandle(center, rotation, size, ref radius, PrimitiveBoundsHandle.Axes.All); }
	public static void SphereBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float radius, PrimitiveBoundsHandle.Axes handleAxes) { SphereBoundsHandle(center, rotation, size, ref radius, handleAxes, sphereBoundsHandle.handleColor, sphereBoundsHandle.wireframeColor); }
	public static void SphereBoundsHandle(Vector3 center, Quaternion rotation, Vector3 size, ref float radius, PrimitiveBoundsHandle.Axes handleAxes, Color handleColor, Color wireframeColor) {
		Matrix4x4 trs = Matrix4x4.TRS(center, rotation, size);

		using (new Handles.DrawingScope(trs)) {
			sphereBoundsHandle.radius = radius;

			sphereBoundsHandle.axes = handleAxes;
			sphereBoundsHandle.wireframeColor = wireframeColor;
			sphereBoundsHandle.handleColor = handleColor;

			sphereBoundsHandle.DrawHandle();

			radius = sphereBoundsHandle.radius;
		}
	}

	#endregion

	#region Full custom handles (sources included)

	static CurveHandle curveHandle = new CurveHandle();
	static KeyframeHandle keyframeHandle = new KeyframeHandle();
	static Free2DMoveHandle free2DMoveHandle = new Free2DMoveHandle();

	public static void CurveHandle(float width, float height, AnimationCurve curve, Vector3 position, Quaternion rotation, Color startColor, Color endColor) {
		curveHandle.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
		curveHandle.SetColors(startColor, endColor);
		curveHandle.Set2DSize(width, height);
		curveHandle.DrawHandle(curve);
	}

	public static void KeyframeHandle(float width, float height, ref Keyframe keyframe, Vector3 position, Quaternion rotation, Color pointColor, Color tangentColor) {
		keyframeHandle.matrix = Matrix4x4.TRS(position, rotation, Vector3.one);
		keyframeHandle.pointColor = pointColor;
		keyframeHandle.tangentColor = tangentColor;

		keyframe = keyframeHandle.DrawHandle(new Vector2(width, height), keyframe, .03f);
	}

	public static void Free2DMoveHandle(ref Vector2 position, float size, Quaternion rotation, Color color, Color selectedColor) {
		free2DMoveHandle.matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
		free2DMoveHandle.color = color;
		free2DMoveHandle.selectedColor = selectedColor;
		free2DMoveHandle.faceCamera = true;
		free2DMoveHandle.texture = null;
		free2DMoveHandle.selectedTexture = null;
		free2DMoveHandle.hoveredTexture = null;
		position = free2DMoveHandle.DrawHandle(position, size);
	}

	public static void Free2DMoveHandle(ref Vector2 position, float size, Quaternion rotation, Texture2D texture = null, Texture2D selectedTexture = null, Texture2D hoverTexture = null) {
		free2DMoveHandle.matrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
		free2DMoveHandle.texture = texture;
		free2DMoveHandle.selectedTexture = selectedTexture;
		free2DMoveHandle.hoveredTexture = hoverTexture;
		free2DMoveHandle.faceCamera = false;
		position = free2DMoveHandle.DrawHandle(position, size);
	}

	#endregion
}

public class CustomHandle {
    Quaternion rotation = Quaternion.identity;
    Vector3 scale = Vector3.one;
    Vector3 position;

    public Matrix4x4 matrix = Matrix4x4.identity;
    public Event e { get { return Event.current; } }

    public void SetTransform(Transform transform) {
        rotation = transform.rotation;
        position = transform.position;
        scale = transform.localScale;
        matrix = Matrix4x4.TRS(position, rotation, scale);
    }

    public void SetTransform(Vector3 position, Quaternion rotation, Vector3 scale) {
        this.rotation = rotation;
        this.position = position;
        this.scale = scale;
        matrix = Matrix4x4.TRS(position, rotation, scale);
    }

    public void SetTransform(CustomHandle handle) {
        this.rotation = Quaternion.LookRotation(
            handle.matrix.GetColumn(2),
            handle.matrix.GetColumn(1)
        );
        this.position = handle.matrix.GetColumn(3);
        this.scale = new Vector3(
            handle.matrix.GetColumn(0).magnitude,
            handle.matrix.GetColumn(1).magnitude,
            handle.matrix.GetColumn(2).magnitude
        );

        matrix = Matrix4x4.TRS(position, rotation, scale);
    }
}

public static class CustomHandleUtility {
    public static bool GetPointOnPlane(Matrix4x4 planeTransform, Ray ray, out Vector3 position) {
        float dist;
        position = Vector3.zero;
        Plane p = new Plane(planeTransform * Vector3.forward, planeTransform.MultiplyPoint3x4(position));

        p.Raycast(ray, out dist);

        if (dist < 0)
            return false;

        position = ray.GetPoint(dist);

        return true;
    }
}

public class CurveHandle : CustomHandle {
	public Gradient curveGradient;
	public float width;
	public float height;

	public int curveSamples = 100;

	public int selectedKeyframeIndex { get; private set; }

	KeyframeHandle keyframeHandle = new KeyframeHandle();
	bool mouseOverCurveEdge = false;
	float mouseCurveEdgeDst;
	const float mouseOverEdgeDstThreshold = .05f;

	Vector3 currentMouseWorld;

	public CurveHandle() {
		selectedKeyframeIndex = -1;
	}

	public AnimationCurve DrawHandle(AnimationCurve curve) {
		AnimationCurve ret;

		//Update the mouse world position:
		Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
		if (CustomHandleUtility.GetPointOnPlane(matrix, r, out currentMouseWorld))
			currentMouseWorld = matrix.inverse.MultiplyPoint3x4(currentMouseWorld);

		if (e.type == EventType.Repaint) {
			PushGLContext();
			DrawBorders();
			DrawCurve(curve);
			DrawLabels(curve);
			PopGLContext();
		}

		//draw curve handles:
		ret = DrawCurvePointsHandle(curve);

		if (e.type == EventType.MouseDown && e.button == 0 && mouseOverCurveEdge) {
			ret = AddCurveKeyframe(curve);
			e.Use();
		}

		return ret;
	}

	void PushGLContext() {
		GL.PushMatrix();
		GL.MultMatrix(matrix);
		HandlesMaterials.vertexColor.SetPass(0);
	}

	void PopGLContext() {
		GL.PopMatrix();
	}

	void DrawBorders() {
		//hummm good (or not so) legacy OpenGL ...

		Vector3 bottomLeft = Vector3.zero;
		Vector3 bottomRight = new Vector3(width, 0, 0);
		Vector3 topLeft = new Vector3(0, height, 0);
		Vector3 topRight = new Vector3(width, height, 0);

		GL.Begin(GL.LINE_STRIP);
		{
			HandlesMaterials.vertexColor.SetPass(0);
			GL.Color(Color.black);
			GL.Vertex(bottomLeft);
			GL.Vertex(bottomRight);
			GL.Vertex(topRight);
			GL.Vertex(topLeft);
			GL.Vertex(bottomLeft);
		}
		GL.End();
	}

	void DrawCurveQuad(AnimationCurve curve, float f0, float f1) {
		Vector3 bottomLeft = new Vector3(f0 * width, 0, 0);
		Vector3 topLeft = new Vector3(f0 * width, curve.Evaluate(f0) * height, 0);
		Vector3 topRight = new Vector3(f1 * width, curve.Evaluate(f1) * height, 0);
		Vector3 bottomRight = new Vector3(f1 * width, 0, 0);

		//check if the mouse is near frmo the curve edge:
		float dst = HandleUtility.DistancePointToLineSegment(currentMouseWorld, topLeft, topRight);

		if (dst < mouseCurveEdgeDst)
			mouseCurveEdgeDst = dst;

		if (dst < mouseOverEdgeDstThreshold)
			mouseOverCurveEdge = true;

		GL.Color(curveGradient.Evaluate(f0));
		GL.Vertex(bottomLeft);
		GL.Vertex(topLeft);
		GL.Color(curveGradient.Evaluate(f1));
		GL.Vertex(topRight);
		GL.Vertex(bottomRight);
	}

	void DrawCurveEdge(AnimationCurve curve, float f0, float f1) {
		Vector3 topLeft = new Vector3(f0 * width, curve.Evaluate(f0) * height, 0);
		Vector3 topRight = new Vector3(f1 * width, curve.Evaluate(f1) * height, 0);
		Color c1 = curveGradient.Evaluate(f0);
		Color c2 = curveGradient.Evaluate(f1);

		c1.a = 1;
		c2.a = 1;
		GL.Color(c1);
		GL.Vertex(topLeft);
		GL.Color(c2);
		GL.Vertex(topRight);
	}

	void DrawLabels(AnimationCurve curve) {
		Handles.Label(matrix.MultiplyPoint3x4(Vector3.zero), "0");

		foreach (var key in curve.keys) {
			//draw key time:
			Vector3 timePosition = matrix.MultiplyPoint3x4(Vector3.right * key.time * width);
			Handles.Label(timePosition, key.time.ToString("F2"));

			//draw key value:
			Vector3 valuePosition = matrix.MultiplyPoint3x4(Vector3.up * key.value * height + Vector3.left * .1f);
			Handles.Label(valuePosition, key.value.ToString("F2"));
		}
	}

	AnimationCurve AddCurveKeyframe(AnimationCurve curve) {
		AnimationCurve ret = new AnimationCurve(curve.keys);
		Vector2 point = currentMouseWorld;

		float time = point.x / width;
		float value = point.y / height;

		Keyframe newKey = new Keyframe(time, value);

		GUI.changed = true;

		selectedKeyframeIndex = ret.AddKey(newKey);

		return ret;
	}

	AnimationCurve DrawCurvePointsHandle(AnimationCurve curve) {
		AnimationCurve ret = curve;

		if (curve == null)
			return null;

		keyframeHandle.SetTransform(this);
		keyframeHandle.SetCurve(curve);

		for (int i = 0; i < curve.length; i++) {
			Keyframe keyframe = curve.keys[i];
			Keyframe movedKeyframe;

			movedKeyframe = keyframeHandle.DrawHandle(new Vector2(width, height), keyframe, .03f, i != 0, i != curve.length - 1);

			if (selectedKeyframeIndex == i) {
				EditorGUIUtility.keyboardControl = keyframeHandle.pointHandle.controlId;
				EditorGUIUtility.hotControl = keyframeHandle.pointHandle.controlId;
				selectedKeyframeIndex = -1;
			}

			//it the key have been moved
			if (!keyframe.Equal(movedKeyframe)) {
				//we duplicate the curve to return another modified one:
				ret = new AnimationCurve(curve.keys);
				ret.MoveKey(i, movedKeyframe);
			}
		}

		return ret;
	}

	void DrawCurve(AnimationCurve curve) {
		if (curveSamples < 0 || curveSamples > 10000)
			return;

		//We use this function to calcul if the mouse is over the curve edge too
		mouseCurveEdgeDst = 1e20f;
		mouseOverCurveEdge = false;

		//draw curve
		GL.Begin(GL.QUADS);
		{

			for (int i = 0; i < curveSamples; i++) {
				float f0 = (float)i / (float)curveSamples;
				float f1 = (float)(i + 1) / (float)curveSamples;

				DrawCurveQuad(curve, f0, f1);
			}
		}
		GL.End();

		//if mouse is near the curve edge, we draw it
		if (mouseOverCurveEdge) {
			GL.Begin(GL.LINES);
			{
				for (int i = 0; i < curveSamples; i++) {
					float f0 = (float)i / (float)curveSamples;
					float f1 = (float)(i + 1) / (float)curveSamples;

					DrawCurveEdge(curve, f0, f1);
				}
			}
			GL.End();
		}
	}

	public void SetColors(Color startColor, Color endColor) {
		curveGradient = new Gradient();
		GradientColorKey[] colorKeys = new GradientColorKey[2] { new GradientColorKey(startColor, 0), new GradientColorKey(endColor, 1) };
		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2] { new GradientAlphaKey(startColor.a, 0), new GradientAlphaKey(endColor.a, 1) };

		curveGradient.SetKeys(colorKeys, alphaKeys);
	}

	public void SetColors(Gradient gradient) {
		if (gradient != null)
			curveGradient = gradient;
	}

	public void Set2DSize(Vector2 size) { Set2DSize(size.x, size.y); }
	public void Set2DSize(float width, float height) {
		this.width = width;
		this.height = height;
	}
}

public class Free2DMoveHandle : CustomHandle {
	public Texture2D texture = EditorGUIUtility.whiteTexture;
	public Texture2D hoveredTexture = null;
	public Texture2D selectedTexture = null;
	public Color color = Handles.color;
	public Color hoveredColor = Handles.preselectionColor;
	public Color selectedColor = Handles.selectedColor;
	public bool faceCamera = true;

	public float distance { get; private set; }
	public int controlId { get; private set; }

	int free2DMoveHandleHash = "Free2DMoveHandle".GetHashCode();
	bool hovered = false;
	bool selected = false;

	public Vector2 DrawHandle(Vector2 position, float size) {
		return DrawHandle(EditorGUIUtility.GetControlID(free2DMoveHandleHash, FocusType.Keyboard), position, size);
	}

	public Vector2 DrawHandle(int controlId, Vector2 position, float size) {
		this.controlId = controlId;
		selected = GUIUtility.hotControl == controlId || GUIUtility.keyboardControl == controlId;
		hovered = HandleUtility.nearestControl == controlId;

		switch (e.type) {
			case EventType.MouseDown:
				if (HandleUtility.nearestControl == controlId && e.button == 0) {
					GUIUtility.hotControl = controlId;
					GUIUtility.keyboardControl = controlId;
					e.Use();
				}
				break;
			case EventType.MouseUp:
				if (GUIUtility.hotControl == controlId && (e.button == 0 || e.button == 2)) {
					GUIUtility.hotControl = 0;
					e.Use();
				}
				break;
			case EventType.MouseDrag:
				if (selected)
					Move2DHandle(ref position);
				break;
			case EventType.Repaint:
				DrawDot(position, size);
				break;
			case EventType.Layout:
				if (e.type == EventType.Layout)
					SceneView.RepaintAll();
				Vector3 pointWorldPos = matrix.MultiplyPoint3x4(position);
				distance = HandleUtility.DistanceToRectangle(pointWorldPos, Camera.current.transform.rotation, size);
				HandleUtility.AddControl(controlId, distance);
				break;
		}

		return position;
	}

	void DrawDot(Vector2 position, float size) {
		Vector3 worldPos = matrix.MultiplyPoint3x4(position);
		Vector3 camRight;
		Vector3 camUp;

		if (faceCamera) {
			camRight = Camera.current.transform.right * size;
			camUp = Camera.current.transform.up * size;
		}
		else {
			camRight = matrix.MultiplyPoint3x4(Vector3.right) * size;
			camUp = matrix.MultiplyPoint3x4(Vector3.up) * size;
		}

		Texture2D t = texture;
		Color c = (t == null) ? color : Color.white;

		if (selected && selectedTexture == null)
			c = selectedColor;
		else if (hovered && hoveredTexture == null)
			c = hoveredColor;

		if (selected && selectedTexture != null)
			t = selectedTexture;
		else if (hovered && hoveredTexture != null)
			t = hoveredTexture;

		HandlesMaterials.textured.SetColor("_Color", c);
		HandlesMaterials.textured.SetTexture("_MainTex", t);
		HandlesMaterials.textured.SetPass(0);
		GL.Begin(GL.QUADS);
		{
			GL.TexCoord2(1, 1);
			GL.Vertex(worldPos + camRight + camUp);
			GL.TexCoord2(1, 0);
			GL.Vertex(worldPos + camRight - camUp);
			GL.TexCoord2(0, 0);
			GL.Vertex(worldPos - camRight - camUp);
			GL.TexCoord2(0, 1);
			GL.Vertex(worldPos - camRight + camUp);
		}
		GL.End();
	}

	bool GetMousePositionInWorld(out Vector3 position) {
		Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
		return CustomHandleUtility.GetPointOnPlane(matrix, r, out position);
	}

	void Move2DHandle(ref Vector2 position) {
		Vector3 mouseWorldPos;
		if (GetMousePositionInWorld(out mouseWorldPos)) {
			Vector3 pointOnPlane = matrix.inverse.MultiplyPoint3x4(mouseWorldPos);

			if (e.delta != Vector2.zero)
				GUI.changed = true;

			position = pointOnPlane;
		}
	}

}

public static class KeyframeExtension {
    public static bool Equal(this Keyframe k1, Keyframe k2) {
        return (k1.time == k2.time && k1.value == k2.value && k1.inTangent == k2.inTangent && k1.outTangent == k2.outTangent);
    }
}

public class KeyframeHandle : CustomHandle {
	public Color pointColor;
	public Color tangentColor;
	public Color wireColor = Color.green;
	public float tangentHandleSpacing = .3f;
	public float tangentHandleScale = .75f;

	public Free2DMoveHandle pointHandle = new Free2DMoveHandle();
	public Free2DMoveHandle tangentHandle = new Free2DMoveHandle();

	readonly int mainHandleHash = "mainCurve2Dhandle".GetHashCode();
	readonly int tangentHandleHash = "tangentCurve2Dhandle".GetHashCode();

	AnimationCurve curve = null;

	public enum TangentDirection {
		In,
		Out,
	}

	public Keyframe DrawHandle(Vector2 zone, Keyframe keyframe, float size, bool rightEditable = true, bool leftEditable = true) {
		if (e.type == EventType.MouseDown) {
			//we add the context menu when right clicking on the point Handle:
			if (HandleUtility.nearestControl == pointHandle.controlId && e.button == 1)
				KeyframeContextMenu(keyframe);
		}

		return DrawKeyframeHandle(zone, keyframe, size, rightEditable, leftEditable);
	}

	public void SetCurve(AnimationCurve curve) {
		this.curve = curve;
	}

	Vector2 TangentToDirection(float radTangent) {
		if (float.IsInfinity(radTangent))
			return new Vector2(0, -tangentHandleSpacing);
		return (new Vector2(1f, radTangent)).normalized * tangentHandleSpacing;
	}

	float DirectionToTangent(Vector2 direction, TangentDirection tangentDirection) {
		if (tangentDirection == TangentDirection.In && direction.x > 0.0001f)
			return float.PositiveInfinity;
		if (tangentDirection == TangentDirection.Out && direction.x < -0.0001f)
			return float.PositiveInfinity;

		return direction.y / direction.x;
	}

	bool IsSelected(int controlId) {
		return (EditorGUIUtility.hotControl == controlId || EditorGUIUtility.keyboardControl == controlId);
	}

	Keyframe DrawKeyframeHandle(Vector2 zone, Keyframe keyframe, float size, bool rightEditable, bool leftEditable) {
		pointHandle.SetTransform(this);
		tangentHandle.SetTransform(this);

		int pointControlId = EditorGUIUtility.GetControlID(mainHandleHash, FocusType.Keyboard);
		int inTangentControlId = EditorGUIUtility.GetControlID(tangentHandleHash, FocusType.Keyboard);
		int outTangentControlId = EditorGUIUtility.GetControlID(tangentHandleHash, FocusType.Keyboard);

		// Debug.Log("hotContorl: " + EditorGUIUtility.keyboardControl + ", " + outTangentControlId + ", " + inTangentControlId + ", selected: " + selected);

		//point position
		Vector2 keyframePosition = new Vector2(zone.x * keyframe.time, zone.y * keyframe.value);

		//tangent positions:
		Vector2 inTangentPosition = -TangentToDirection(keyframe.inTangent);
		Vector2 outTangentPosition = TangentToDirection(keyframe.outTangent);

		if (e.type == EventType.Repaint) {
			//tangent Wires:
			HandlesMaterials.vertexColor.SetPass(0);
			GL.Begin(GL.LINES);
			{
				GL.Color(wireColor);
				if (rightEditable) {
					GL.Vertex(matrix.MultiplyPoint3x4(keyframePosition));
					GL.Vertex(matrix.MultiplyPoint3x4(inTangentPosition + keyframePosition));
				}
				if (leftEditable) {
					GL.Vertex(matrix.MultiplyPoint3x4(keyframePosition));
					GL.Vertex(matrix.MultiplyPoint3x4(outTangentPosition + keyframePosition));
				}
			}
			GL.End();
		}

		//draw main point Handle
		keyframePosition = pointHandle.DrawHandle(pointControlId, keyframePosition, size);

		//draw tangents Handles
		inTangentPosition += keyframePosition;
		outTangentPosition += keyframePosition;

		if (rightEditable) {
			inTangentPosition = tangentHandle.DrawHandle(inTangentControlId, inTangentPosition, size * tangentHandleScale);
			keyframe.inTangent = DirectionToTangent(inTangentPosition - keyframePosition, TangentDirection.In);
		}
		if (leftEditable) {
			outTangentPosition = tangentHandle.DrawHandle(outTangentControlId, outTangentPosition, size * tangentHandleScale);
			keyframe.outTangent = DirectionToTangent(outTangentPosition - keyframePosition, TangentDirection.Out);
		}

		//set back keyframe values
		keyframe.time = keyframePosition.x / zone.x;
		keyframe.value = keyframePosition.y / zone.y;

		return keyframe;
	}

	void KeyframeContextMenu(Keyframe keyframe) {
		GenericMenu menu = new GenericMenu();

		if (curve != null) {
			int keyframeIndex = curve.keys.ToList().FindIndex(k => k.Equal(keyframe)) - 1;

			Action<bool, string, AnimationUtility.TangentMode> SetTangentModeMenu = (right, text, tangentMode) => {
				menu.AddItem(new GUIContent(text), false, () => {
					if (right)
						AnimationUtility.SetKeyRightTangentMode(curve, keyframeIndex, tangentMode);
					else
						AnimationUtility.SetKeyLeftTangentMode(curve, keyframeIndex, tangentMode);
					GUI.changed = true;
				});
			};
			SetTangentModeMenu(false, "Left Tangent/Auto", AnimationUtility.TangentMode.Auto);
			SetTangentModeMenu(false, "Left Tangent/ClampedAuto", AnimationUtility.TangentMode.ClampedAuto);
			SetTangentModeMenu(false, "Left Tangent/Constant", AnimationUtility.TangentMode.Constant);
			SetTangentModeMenu(false, "Left Tangent/Free", AnimationUtility.TangentMode.Free);
			SetTangentModeMenu(false, "Left Tangent/Linear", AnimationUtility.TangentMode.Linear);
			SetTangentModeMenu(true, "Right Tangent/Auto", AnimationUtility.TangentMode.Auto);
			SetTangentModeMenu(true, "Right Tangent/ClampedAuto", AnimationUtility.TangentMode.ClampedAuto);
			SetTangentModeMenu(true, "Right Tangent/Constant", AnimationUtility.TangentMode.Constant);
			SetTangentModeMenu(true, "Right Tangent/Free", AnimationUtility.TangentMode.Free);
			SetTangentModeMenu(true, "Right Tangent/Linear", AnimationUtility.TangentMode.Linear);

			menu.AddItem(new GUIContent("remove"), false, () => {
				GUI.changed = true;
				if (keyframeIndex == -1)
					keyframeIndex = curve.keys.Length - 1;
				curve.RemoveKey(keyframeIndex);
			});
		}
		else
			menu.AddDisabledItem(new GUIContent("Curve not set for keyframe !"));
		menu.ShowAsContext();
	}
}

public class MeshPreviewHandle : CustomHandle {
    int meshPreviewHash = "MeshPreviewHandle".GetHashCode();

    struct MeshInfo {
        public Mesh mesh;
        public Matrix4x4 trs;
        public Material material;

        public MeshInfo(Mesh mesh, Matrix4x4 trs, Material material) {
            this.mesh = mesh;
            this.material = material;
            this.trs = trs;
        }
    }

    Dictionary<int, MeshInfo> meshInfos = new Dictionary<int, MeshInfo>();

    Mesh currentMesh;
    Matrix4x4 currentTRS;

    public void DrawHandle(Mesh mesh, Material material, Vector3 position, Quaternion rotation, Vector3 scale) {
        int controlId = EditorGUIUtility.GetControlID(meshPreviewHash, FocusType.Passive);
        Matrix4x4 trs = Matrix4x4.TRS(position, rotation, scale);
        meshInfos[controlId] = new MeshInfo(mesh, trs, material);

        Handles.FreeMoveHandle(controlId, position, rotation, 0f, Vector3.zero, MeshHandleCap);
    }

    public void MeshHandleCap(int controlId, Vector3 position, Quaternion rotation, float size, EventType eventType) {
        MeshInfo meshInfo = meshInfos[controlId];

        if (eventType == EventType.Repaint) {
            meshInfo.material.SetPass(0);
            Graphics.DrawMeshNow(meshInfo.mesh, meshInfo.trs);
        }
        else if (eventType == EventType.Layout) {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            bool intersect = meshInfo.mesh.bounds.IntersectRay(mouseRay);
            if (intersect)
                HandleUtility.AddControl(controlId, 0);
            else
                HandleUtility.AddControl(controlId, 1e20f);
        }
    }
}

public static class HandlesMaterials {
    public static Material vertexColor;
    public static Material textured;
    public static Material overlayColor;

    static HandlesMaterials() {
        vertexColor = Resources.Load<Material>("VertexColorMaterial");
        textured = Resources.Load<Material>("TexturedMaterial");
        overlayColor = Resources.Load<Material>("OverlayColorHandle");
    }
}