using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeshUpdater : MonoBehaviour
{
    [SerializeField] float _interval = 0.2f;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private Mesh _bakedMesh;
	private List<int[]> _tris = new();
	private Dictionary<int, int> _indexPairDic = new();
    public List<int[]> Tris => _tris;
    public Mesh BakedMesh => _bakedMesh;
    private Mesh _drawMesh;
    private Mesh _normalMesh;
    private void Start()
    {
        StartCoroutine(UpdateCoroutine());
    }

    public void ChangeTriangles()
    {
	    _indexPairDic.Clear();
	    foreach (var tri in _tris)
	    {
		    foreach (var vert in tri)
		    {
			    if (!_indexPairDic.ContainsKey(vert))
			    {
				    if (_indexPairDic.Count <= 0)
				    {
					    _indexPairDic.Add(vert, 0);
				    }
				    else
				    {
					    _indexPairDic.Add(vert, _indexPairDic.Max(kv=>kv.Value) + 1);
				    }
			    }
		    }
	    }

	    Vector3[] normals = new Vector3[_indexPairDic.Count];
	    Vector3[] verts = new Vector3[_indexPairDic.Count];
	    for (int i = 0; i < _indexPairDic.Count; i++)
	    {
		    verts[i] = _bakedMesh.vertices[_indexPairDic.FirstOrDefault(kv=>kv.Value == i).Key];
		    normals[i] = _bakedMesh.normals[_indexPairDic.FirstOrDefault(kv=>kv.Value == i).Key];
	    }

	    Mesh mesh = new Mesh();
	    mesh.vertices = verts;
	    mesh.normals = normals;
	    List<int> tris = new List<int>();
	    foreach (var triangle in _tris)
	    {
		    foreach (var vert in triangle)
		    {
			    tris.Add(_indexPairDic[vert]);
		    }
	    }

	    mesh.triangles = tris.ToArray();
	    _drawMesh = mesh;
    }
	#if UNITY_EDITOR
	[ContextMenu("calc normal")]
	void CalculateNormalMesh()
	{
		List<Vector3> verticies = new List<Vector3>();
		List<int> indices = new List<int>();
		List<Vector3> normals = new List<Vector3>();
		for (int i = 0; i < _bakedMesh.vertices.Length; i++)
		{
			indices.Add(verticies.Count);
			indices.Add(verticies.Count + 1);
			verticies.Add(_bakedMesh.vertices[i]);
			verticies.Add(_bakedMesh.vertices[i] + _bakedMesh.normals[i] * 1.5f);
			normals.Add(Vector3.zero);
			normals.Add(Vector3.zero);
			// normals.Add(_bakedMesh.normals[i]);
			// normals.Add(_bakedMesh.normals[i]);
		}
	    
		_normalMesh = new Mesh();
		_normalMesh.vertices = verticies.ToArray();
		_normalMesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
		_normalMesh.normals = normals.ToArray();
	}
    private void OnDrawGizmos()
    {
	    // if (_normalMesh == null) CalculateNormalMesh();
	    // Gizmos.color = Color.red;
	    // Gizmos.DrawMesh(_normalMesh, transform.position);
	    
	    if (!_drawMesh) return;
	    Gizmos.color = Color.green;
	    Gizmos.DrawMesh(_drawMesh, transform.position);
    }
    #endif

    public Vector3 ClosestPoint(Vector3 point, out int[] tris)
    {
	    var vt = new VertTriList(_bakedMesh);
	    var objSpacePt = transform.InverseTransformPoint(point);
	    Vector3[] verts = _bakedMesh.vertices;
	    var kd = KDTree.MakeFromPoints(verts);
	    var meshPt = NearestPointOnMesh(objSpacePt, verts, kd, _bakedMesh.triangles, vt, out tris);
	    var worldPt = transform.TransformPoint(meshPt);
	    return worldPt;
    }
    IEnumerator UpdateCoroutine()
    {
        var wait = new WaitForSeconds(_interval);
        while (true)
        {
            _bakedMesh = new Mesh();
            _bakedMesh.name = "skinmesh";
            _skinnedMeshRenderer.BakeMesh(_bakedMesh);
            _bakedMesh.MarkDynamic();
            _bakedMesh.RecalculateNormals();
            _meshCollider.sharedMesh = _bakedMesh;
            yield return wait;
        }
    }
    public Vector3 NearestPointOnMesh(Vector3 pt, Vector3[] verts, KDTree vertProx, int[] tri, VertTriList vt, out int[] nearestTriangles) 
    {
		//	First, find the nearest vertex (the nearest point must be on one of the triangles
		//	that uses this vertex if the mesh is convex).
		var nearests = vertProx.FindNearestEpsilon(pt, new List<int>());
		
		Vector3 nearestPt = Vector3.zero;
		float nearestSqDist = 100000000f;
		Vector3 possNearestPt;
		nearestTriangles = new int[3];
		
		foreach (var nearest in nearests)
		{
			//	Get the list of triangles in which the nearest vert "participates".
			int[] nearTris = vt[nearest];
			
			for (int i = 0; i < nearTris.Length; i++) 
			{
				int triOff = nearTris[i] * 3;
				Vector3 a = verts[tri[triOff]];
				Vector3 b = verts[tri[triOff + 1]];
				Vector3 c = verts[tri[triOff + 2]];
				
				ClosestPointOnTriangleToPoint(ref pt, ref a, ref b, ref c, out possNearestPt);
				float possNearestSqDist = (pt - possNearestPt).sqrMagnitude;
			
				if (possNearestSqDist < nearestSqDist) 
				{
					nearestPt = possNearestPt;
					nearestSqDist = possNearestSqDist;
					nearestTriangles[0] = tri[triOff];
					nearestTriangles[1] = tri[triOff + 1];
					nearestTriangles[2] = tri[triOff + 2];
				}
			}
		}
		
		return nearestPt;
	}
	public static void ClosestPointOnTriangleToPoint(ref Vector3 point, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out Vector3 result)
    {
        //Source: Real-Time Collision Detection by Christer Ericson
        //Reference: Page 136

        //Check if P in vertex region outside A
        Vector3 ab = vertex2 - vertex1;
        Vector3 ac = vertex3 - vertex1;
        Vector3 ap = point - vertex1;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0.0f && d2 <= 0.0f)
        {
            result = vertex1; //Barycentric coordinates (1,0,0)
            return;
        }

        //Check if P in vertex region outside B
        Vector3 bp = point - vertex2;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0.0f && d4 <= d3)
        {
            result = vertex2; // barycentric coordinates (0,1,0)
            return;
        }

        //Check if P in edge region of AB, if so return projection of P onto AB
        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0.0f && d1 >= 0.0f && d3 <= 0.0f)
        {
            float v = d1 / (d1 - d3);
            result = vertex1 + v * ab; //Barycentric coordinates (1-v,v,0)
            return;
        }

        //Check if P in vertex region outside C
        Vector3 cp = point - vertex3;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0.0f && d5 <= d6)
        {
            result = vertex3; //Barycentric coordinates (0,0,1)
            return;
        }

        //Check if P in edge region of AC, if so return projection of P onto AC
        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0.0f && d2 >= 0.0f && d6 <= 0.0f)
        {
            float w = d2 / (d2 - d6);
            result = vertex1 + w * ac; //Barycentric coordinates (1-w,0,w)
            return;
        }

        //Check if P in edge region of BC, if so return projection of P onto BC
        float va = d3 * d6 - d5 * d4;
        if (va <= 0.0f && (d4 - d3) >= 0.0f && (d5 - d6) >= 0.0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            result = vertex2 + w * (vertex3 - vertex2); //Barycentric coordinates (0,1-w,w)
            return;
        }

        //P inside face region. Compute Q through its barycentric coordinates (u,v,w)
        float denom = 1.0f / (va + vb + vc);
        float v2 = vb * denom;
        float w2 = vc * denom;
        result = vertex1 + ab * v2 + ac * w2; //= u*vertex1 + v*vertex2 + w*vertex3, u = va * denom = 1.0f - v - w
    }
	
	Vector3 NearestPointOnMesh(Vector3 pt, Vector3[] verts, int[] tri, VertTriList vt) 
	{
		//	First, find the nearest vertex (the nearest point must be on one of the triangles
		//	that uses this vertex if the mesh is convex).
		int nearest = -1;
		float nearestSqDist = 100000000f;
		
		for (int i = 0; i < verts.Length; i++) 
		{
			float sqDist = (verts[i] - pt).sqrMagnitude;
			
			if (sqDist < nearestSqDist) 
			{
				nearest = i;
				nearestSqDist = sqDist;
			}
		}
		
		//	Get the list of triangles in which the nearest vert "participates".
		int[] nearTris = vt[nearest];
		
		Vector3 nearestPt = Vector3.zero;
		nearestSqDist = 100000000f;
		
		for (int i = 0; i < nearTris.Length; i++) 
		{
			int triOff = nearTris[i] * 3;
			Vector3 a = verts[tri[triOff]];
			Vector3 b = verts[tri[triOff + 1]];
			Vector3 c = verts[tri[triOff + 2]];
			
			Vector3 possNearestPt = NearestPointOnTri(pt, a, b, c);
			float possNearestSqDist = (pt - possNearestPt).sqrMagnitude;
			
			if (possNearestSqDist < nearestSqDist) 
			{
				nearestPt = possNearestPt;
				nearestSqDist = possNearestSqDist;
			}
		}
		
		return nearestPt;
	}
	public Vector3 NearestPointOnTri(Vector3 pt, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 edge1 = b - a;
		Vector3 edge2 = c - a;
		Vector3 edge3 = c - b;
		float edge1Len = edge1.magnitude;
		float edge2Len = edge2.magnitude;
		float edge3Len = edge3.magnitude;
		
		Vector3 ptLineA = pt - a;
		Vector3 ptLineB = pt - b;
		Vector3 ptLineC = pt - c;
		Vector3 xAxis = edge1 / edge1Len;
		Vector3 zAxis = Vector3.Cross(edge1, edge2).normalized;
		Vector3 yAxis = Vector3.Cross(zAxis, xAxis);
		
		Vector3 edge1Cross = Vector3.Cross(edge1, ptLineA);
		Vector3 edge2Cross = Vector3.Cross(edge2, -ptLineC);
		Vector3 edge3Cross = Vector3.Cross(edge3, ptLineB);
		bool edge1On = Vector3.Dot(edge1Cross, zAxis) > 0f;
		bool edge2On = Vector3.Dot(edge2Cross, zAxis) > 0f;
		bool edge3On = Vector3.Dot(edge3Cross, zAxis) > 0f;
		
		//	If the point is inside the triangle then return its coordinate.
		if (edge1On && edge2On && edge3On) 
		{
			float xExtent = Vector3.Dot(ptLineA, xAxis);
			float yExtent = Vector3.Dot(ptLineA, yAxis);
			return a + xAxis * xExtent + yAxis * yExtent;
		}
		
		//	Otherwise, the nearest point is somewhere along one of the edges.
		Vector3 edge1Norm = xAxis;
		Vector3 edge2Norm = edge2.normalized;
		Vector3 edge3Norm = edge3.normalized;
		
		float edge1Ext = Mathf.Clamp(Vector3.Dot(edge1Norm, ptLineA), 0f, edge1Len);
		float edge2Ext = Mathf.Clamp(Vector3.Dot(edge2Norm, ptLineA), 0f, edge2Len);
		float edge3Ext = Mathf.Clamp(Vector3.Dot(edge3Norm, ptLineB), 0f, edge3Len);

		Vector3 edge1Pt = a + edge1Ext * edge1Norm;
		Vector3 edge2Pt = a + edge2Ext * edge2Norm;
		Vector3 edge3Pt = b + edge3Ext * edge3Norm;
		
		float sqDist1 = (pt - edge1Pt).sqrMagnitude;
		float sqDist2 = (pt - edge2Pt).sqrMagnitude;
		float sqDist3 = (pt - edge3Pt).sqrMagnitude;
		
		if (sqDist1 < sqDist2) 
		{
			if (sqDist1 < sqDist3) 
			{
				return edge1Pt;
			} 
			else 
			{
				return edge3Pt;
			}
		} 
		else if (sqDist2 < sqDist3) 
		{
			return edge2Pt;
		} 
		else 
		{
			return edge3Pt;
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshUpdater))]
public class MeshUpdaterEditor : Editor
{
	private MeshUpdater _self;
	private bool _toggle = false;
	private Vector3[] _normalLines;
	public void OnEnable()
	{
		_self = target as MeshUpdater;
		SceneView.duringSceneGui -= OnSceneViewClick;
		SceneView.duringSceneGui += OnSceneViewClick;
	}

	void CalculateNormalLines()
	{
		List<Vector3> points = new List<Vector3>();
		for (int i = 0; i < _self.BakedMesh.vertices.Length; i++)
		{
			points.Add(_self.BakedMesh.vertices[i]);
			points.Add(_self.BakedMesh.vertices[i] + _self.BakedMesh.normals[i] * 1.5f);
		}

		_normalLines = points.ToArray();
	}
	public void OnSceneViewClick(SceneView sceneView)
	{
		// イベントを変数として保存
		var e = Event.current;

		// マウスの左クリックのときtrue
		if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0 && _toggle)
		{
			// レイキャストを飛ばす
			if (EditorRaycastHelper.RaycastAgainstScene(out var hit))
			{
				// ヒット位置(オブジェクトSpace)
				var tris = new int[3];
				var meshPt = _self.ClosestPoint(hit.point, out tris);
				_self.Tris.Add(tris);
				_self.ChangeTriangles();
				//var hitPositionOS = _self.transform.InverseTransformPoint(hit.point); 
			}
		}
	}

	private void OnSceneGUI()
	{
		// if (_normalLines == null) CalculateNormalLines();
		// Handles.DrawLines(_normalLines);
		if (!_toggle) return;
		//自身を選択している時は無効(固定を解除出来ないので)
		if (Selection.activeGameObject == _self.gameObject) {
			return;
		}
    
		//強制的に対象を選択し続けるように、ただしProjectのものを選択している時は除く
		if (Selection.activeGameObject != null || Selection.activeObject == null) {
			Selection.objects = new Object[]{_self.gameObject};
		}
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		_toggle = EditorGUILayout.Toggle("Edit Mode", _toggle);
	}
}
#endif