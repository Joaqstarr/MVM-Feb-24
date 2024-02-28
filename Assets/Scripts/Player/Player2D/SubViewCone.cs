using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UIElements;

public class SubViewCone : MonoBehaviour
{
    public PlayerSubData data;

    public Camera camera2D; // camera for 2D terminal

    private Mesh _mesh; // mesh for cone polygons
    private Vector3 _origin; // starting point of cone - should be on player
    private float _fov; // how wide cone is in degrees
    private int _resolution; // how many polygons make up the cone mesh - smoothness
    private float _viewDistance; // how far out the cone goes
    private int _rayResolution; // resolution of rays cast for scanning collision
    private float _blipGhostTime; // how long blips stay visible after out of vision
    private int _sampleRate; // how many times a second the cone scans for collision - UNUSED

    private float _angle;
    private float _angleIncrease;
    private float _aimAngle;
    private float _angleRadians;
    private Vector3 _angleVector;
    private float _rayRadians;
    private Vector3 _rayVector;
    private int _vertexIndex;
    private int _triangleIndex;
    private bool _scanWaiting;

    private Vector3[] _vertices;
    private Vector2[] _uv;
    private int[] _triangles;
    private GameObject[] _rayCollisions;

    private void Start()
    {
        // variable assignments
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _origin = transform.localPosition;
        _fov = data.fov;
        _resolution = data.resolution;
        _viewDistance = data.viewDistance;
        _rayResolution = data.rayResolution;
        //_blipGhostTime = data.blipGhostTime;
        _sampleRate = data.sampleRate;


        _angle = 0f;
        _angleIncrease = _fov / _resolution;
        _scanWaiting = false;

        _vertices = new Vector3[_resolution + 2];
        _uv = new Vector2[_vertices.Length];
        _triangles = new int[_resolution * 3];
        _rayCollisions = new GameObject[_rayResolution];

        _vertices[0] = _origin;
    }


    public void DrawViewCone(Vector3 aimPos)
    {
        // delete any blips from last frame
        if (_rayCollisions.Count() > 0)
            for (int i = 0; i < _rayCollisions.Count(); i++)
            {
                if (_rayCollisions[i] != null)
                    StartCoroutine(BlipGhostEffect(_rayCollisions[i]));
                    //Destroy(_rayCollisions[i]);
            }    
        // convert mouse position to angle
        if (camera2D == null)
            aimPos = Camera.main.ScreenToWorldPoint(aimPos);
        else
            aimPos = camera2D.ScreenToWorldPoint(aimPos);

        aimPos = (aimPos - transform.position).normalized;
        aimPos.z = 0;
        _aimAngle = Mathf.Atan2(aimPos.y, aimPos.x) * Mathf.Rad2Deg;
        if (_aimAngle < 0) _aimAngle += 360;
        
        _angle = _aimAngle + (_fov/2f);
         
        _triangleIndex = 0;
        _vertexIndex = 1;
        for (int i = 0; i <= _resolution; i++, _vertexIndex++)
        {
            // convert current angle to vector3
            _angleRadians = _angle * (Mathf.PI / 180f);
            _angleVector = new Vector3(Mathf.Cos(_angleRadians), Mathf.Sin(_angleRadians));

            // set vertices of polygon
            Vector3 vertex = _origin + _angleVector * _viewDistance;
            _vertices[_vertexIndex] = vertex;

            // add vertices of polygon to triangles array
            if (i > 0)
            {
                _triangles[_triangleIndex] = 0;
                _triangles[_triangleIndex + 1] = _vertexIndex - 1;
                _triangles[_triangleIndex + 2] = _vertexIndex;

                _triangleIndex += 3;
            }

            // increase angle clockwise
            _angle -= _angleIncrease;
        }

        // set values to mesh to update cone
        _mesh.vertices = _vertices;
        _mesh.uv = _uv;
        _mesh.triangles = _triangles;

        // Check to scan collision
        if (!_scanWaiting) StartCoroutine(CollisionScan());

    }

    public IEnumerator BlipGhostEffect(GameObject blip)
    {
        yield return new WaitForSeconds((1f / _sampleRate));
        Destroy(blip);
    }

    public IEnumerator CollisionScan()
    {
        _scanWaiting = true;
        // fire standarized raycasts to scan for collision
        for (int i = 0; i < _rayResolution; i++)
        {
            // check if ray angle is within view cone
            float rayAngle = (360 / _rayResolution) * i;

            if (Mathf.Abs(rayAngle - _aimAngle) < _fov / 2 || Mathf.Abs(rayAngle - _aimAngle) > 360 - _fov / 2)
            {
                // convert ray angle to vector
                _rayRadians = rayAngle * (Mathf.PI / 180f);
                _rayVector = new Vector3(Mathf.Cos(_rayRadians), Mathf.Sin(_rayRadians));
                // fire ray
                RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, _rayVector, _viewDistance);
                if (raycastHit2D.collider != null)
                {
                    // draw blip on hit
                    _rayCollisions[i] = Instantiate(data.blip, raycastHit2D.point, Quaternion.identity);
                }
            }
        }
        yield return new WaitForSeconds(1f/_sampleRate);
        _scanWaiting = false;
    }
}
