using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class LopeaLine : MonoBehaviour
{
    #region Variables
    //all the nodes used for the mesh
    [SerializeField] Vector3[] nodes;
    //radius of the line
    [SerializeField] float Radius = 0.1f;
    //the amount of vertices surrounding each node
    [SerializeField] int quality = 3;
    //all the vertices in the mesh
    [SerializeField] Vector3[] verts;
    //store mesh
    Mesh mesh;
    //all triangle index values
    int[] triangles;
    //checks if mesh has already generated
    bool generated = false;
    //editor variables: only used if Unity Editor modifies values
#if UNITY_EDITOR
    int _oldtriangles;
    int _oldnodecount;
#endif
    #endregion

    #region Accessors
    /// <summary>
    /// Returns node positions in the line
    /// </summary>
    public Vector3[] Nodes { get { return nodes; } }
    /// <summary>
    /// Returns true if the mesh has already been generated
    /// </summary>
    public bool IsGenerated { get { return generated; } }
    #endregion

    #region Unity functions
    void Start()
    {
       GenerateNewMesh();
    }
    void Update()
    {
        //updating is only necessary when in the editor, not in standalone
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            //checks if there is a node that has been added or removed
            if (_oldnodecount != nodes.Length)
            {
                //create new mesh
                GenerateNewMesh();
            }

            //checks if amount of triangles changes
            if (_oldtriangles != triangles.Length)
            {
                //create new triangle array
                UpdateTriangles();
            }
        }
#endif
        
    }

    #endregion

    #region Modifiers
    void UpdateTriangles()
    {

        //reset triangle array
        triangles = new int[quality * (nodes.Length - 1) * 6];
        //update old values 
        _oldtriangles = triangles.Length;
        for (int i = 0, y = 0, i0 = 0; y < nodes.Length - 1; y++)
        for (int x = 0; x < quality; x++, i += 6, i0++)
        {
            //Generate default triangle variables
            triangles[i] = i0;
            triangles[i + 1] = triangles[i + 4] = quality + i0;
            triangles[i + 2] = triangles[i + 3] = 1 + i0;
            triangles[i + 5] = 1 + quality + i0;

            //last few meshes need to loop back to starting values
            //override default variables if necessary
            if (x == quality - 1)
            {
                triangles[i + 3] = i0;
                triangles[i + 4] = 1 + i0;
                triangles[i + 5] = i0 - (quality - 1);
            }
        }
        //apply triangle array to mesh
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    /// <summary>
    /// Update verts in the mesh, usually executed after modifing node positions.
    /// NOTE: this is an expensive process. Avoid executing this as much as possible.
    /// </summary>
    public void UpdateVerts()
    {
        //init vert triangles
        verts = new Vector3[quality * nodes.Length];
        for (int y= 0,i = 0; y < nodes.Length; y++)
        {
            for (int x = 0; x < quality; x++, i++) {
                // create evenly spaced verts in a circle formation
                verts[i]=new Vector3(Mathf.Sin(2 * Mathf.PI * i / quality + 1) * Radius,
                                       Mathf.Cos(2 * Mathf.PI * i / quality + 1) * Radius,
                                       0) + nodes[y];
            }
        }
        //apply vert positions
        mesh.vertices = verts;
    }
    public void UpdateVerts(int index)
    {
        for (int i = quality * index; i < (quality * index) + quality; i++)
        {
            verts[i] = new Vector3(Mathf.Sin(2 * Mathf.PI * i / quality + 1) * Radius,
                                       Mathf.Cos(2 * Mathf.PI * i / quality + 1) * Radius,
                                       0) + nodes[index];
        }
        mesh.vertices = verts;
    }
    public void SetScale(float scale)
    {
        Radius = scale;
       if(generated) 
            UpdateVerts();
    }
    public void SetQuality(int qual)
    {
        quality = qual;
        if (generated)
            GenerateNewMesh();
    }
   
    void GenerateNewMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        UpdateVerts();
        UpdateTriangles();
        generated = true;
    }

    /// <summary>
    /// Adds a new node at the end of the line
    /// </summary>
    /// <param name="position">The position of the new node </param>
    public void AddNode(Vector3 position)
    {
        if (nodes == null)
        {
            nodes = new Vector3[0];
        }
        var newArray = new Vector3[nodes.Length + 1];
        for (int i = 0; i < nodes.Length; i++)
        {
            newArray[i] = nodes[i];
        }
        newArray[nodes.Length] = position;
        nodes = newArray;
        GenerateNewMesh();
    }
    
    /// <summary>
    /// Adds an array of node positions the mesh
    /// </summary>
    /// <param name="positions">Array of node positions added to the current list of nodes</param>
    public void AddNodes(Vector3[] positions)
    {
        var newArray = new Vector3[positions.Length + nodes.Length];
        for (int i =0; i < newArray.Length; i++)
        {
            newArray[i] = (i < nodes.Length) ? nodes[i] : positions[i];
        }
        nodes = newArray;
        GenerateNewMesh();
    }
    
    /// <summary>
    /// Replaces all nodes with an array of new ones
    /// </summary>
    /// <param name="positions">The new array that replaces the old node array</param>
    public void ReplaceNodes(Vector3[] positions)
    {
        nodes = positions;
        GenerateNewMesh();
    }
    public void UpdateNode(Vector3 position,int index,bool update = true)
    {
        nodes[index] = position;
        if(update)
            UpdateVerts(index);
    }
    #endregion

}
