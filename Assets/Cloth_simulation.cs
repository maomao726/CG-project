// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class Cloth_simulation : MonoBehaviour{
    // public setting variables
    public List<int> pFIXINDEX;

    // static variables
    static Vector3 wind;

    // private variables
    Mesh mesh;
    Particles particles;
    List<Triangle> triangles;
    List<SpringDamper> springdampers;
    int height;
    int width;

    // Start is called before the first frame update
    void Start(){   
        // initilize variables
        wind = new Vector3(0.0f, 0.0f, 0.0f);
        mesh = GetComponent<MeshFilter>().mesh;
        particles = new Particles(mesh.vertices, pFIXINDEX);
        triangles = new List<Triangle>();
        springdampers = new List<SpringDamper>();
        height = 11;
        width = 11;

        // upper triangles
        for(int i = 0; i < height - 1; i++){
            for(int j = 0; j < width - 1; j++){
                int idx1 = i * width + j;
                int idx2 = (i + 1) * width + j;
                int idx3 = i * width + j + 1;
                triangles.Add(new Triangle(idx1, idx2, idx3));
            }
        }

        // lower triangles
        for(int i = 1; i < height; i++){
            for(int j = 0; j < width - 1; j++){
                int idx1 = i * width + j;
                int idx2 = i * width + j + 1;
                int idx3 = (i - 1) * width + j + 1;
                triangles.Add(new Triangle(idx1, idx2, idx3));
            }
        }

        // horizontal spring damper
        for(int i = 0; i < height; i++){
            for (int j = 0; j < width - 1; j++){
                int idx1 = i * width + j;
                int idx2 = i * width + j + 1;
                springdampers.Add(new SpringDamper(idx1, idx2, 
                    Vector3.Distance(particles.vert[idx1], particles.vert[idx2])));
            }
	    }

        // vertical spring damper
	    for(int i = 0; i < height - 1; i++){
            for (int j = 0; j < width; j++){
                int idx1 = i * width + j;
                int idx2 = (i + 1) * width + j;
                springdampers.Add(new SpringDamper(idx1, idx2, 
                    Vector3.Distance(particles.vert[idx1], particles.vert[idx2])));
            }
        }

        // UL to LR diagonal spring damper
        for (int i = 0; i < height - 1; i++){
            for (int j = 0; j < width - 1; j++){
                int idx1 = i * width + j;
                int idx2 = (i + 1) * width + j + 1;
                springdampers.Add(new SpringDamper(idx1, idx2, 
                    Vector3.Distance(particles.vert[idx1], particles.vert[idx2])));
            }
        }

        // LL to UR diagonal spring damper
        for (int i = 1; i < height; i++){
            for (int j = 0; j < width - 1; j++){
                int idx1 = i * width + j;
                int idx2 = (i - 1) * width + j + 1;
                springdampers.Add(new SpringDamper(idx1, idx2, 
                    Vector3.Distance(particles.vert[idx1], particles.vert[idx2])));
            }
        }

        // large horizontal spring damper
        for (int i = 0; i < height - 2; i += 2){
            for (int j = 0; j < width - 2; j += 2){
                int idx1 = i * width + j;
                int idx2 = i * width + j + 2;
                springdampers.Add(new SpringDamper(idx1, idx2, 
                    Vector3.Distance(particles.vert[idx1], particles.vert[idx2])));
            }
        }

        // large vertical spring damper
        for (int i = 0; i < height - 2; i += 2){
            for (int j = 0; j < width - 2; j += 2){
                int idx1 = i * width + j;
                int idx2 = (i + 2) * width + j;
                springdampers.Add(new SpringDamper(idx1, idx2, 
                    Vector3.Distance(particles.vert[idx1], particles.vert[idx2])));
            }
        }
    }

    // Update is called once per frame
    void Update(){
        ControlWind();
        ResetForce();
        ApplySpringDamperForce();
        ApplyWindForce();
        Move();
        mesh.vertices = particles.vert.ToArray();
        mesh.RecalculateNormals();
    }

    void ControlWind(){
        float step = 0.01f;
        if(Input.GetKey("z")){ wind.x += step; }
        else if(Input.GetKey("x")){ wind.x -= step; }
        else if(Input.GetKey("c")){ wind.y += step; }
        else if(Input.GetKey("v")){ wind.y -= step; }
        else if(Input.GetKey("b")){ wind.z += step; }
        else if(Input.GetKey("n")){ wind.z -= step; }
        print("wind(" + wind.x + ", " + wind.y + ", " + wind.z + ")");
    }

    void ResetForce(){
        for(int i = 0; i < particles.acc.Count; i++){
            // reset to the status that only have gravity
            particles.acc[i] = new Vector3(0.0f, 0.0f, -15.0f);
        }
    }

    void ApplySpringDamperForce(){
        for(int i = 0; i < springdampers.Count; i++){
            // calculate current spring length and delta X
            float curlen = Vector3.Distance(particles.vert[springdampers[i].idx1],
                                            particles.vert[springdampers[i].idx2]);
            float deltax = curlen - springdampers[i].restlen;

            // calculate spring direction
            Vector3 dir;
            if(curlen <= 0.000001f){
                dir = new Vector3(0.0f, 0.0f, 0.0f);
            }
            else{ 
                dir = Vector3.Normalize(particles.vert[springdampers[i].idx2] - 
                                        particles.vert[springdampers[i].idx1]);
            }

            // adjust endpoints of spring when the distance is too large or too small
            if(curlen >= springdampers[i].restlen * 1.2f){
                Vector3 center = (particles.vert[springdampers[i].idx1] + 
                                  particles.vert[springdampers[i].idx2]) / 2.0f;
                if(!particles.fix[springdampers[i].idx1]){
                    particles.vert[springdampers[i].idx1] = 
                        center - 0.6f * springdampers[i].restlen * dir;
                }
                if(!particles.fix[springdampers[i].idx2]){
                    particles.vert[springdampers[i].idx2] = 
                        center + 0.6f * springdampers[i].restlen * dir;
                }
            }
            else if(curlen <= springdampers[i].restlen * 0.5f){
                Vector3 center = (particles.vert[springdampers[i].idx1] + 
                                  particles.vert[springdampers[i].idx2]) / 2.0f;
                if(!particles.fix[springdampers[i].idx1]){
                    particles.vert[springdampers[i].idx1] = 
                        center - 0.25f * springdampers[i].restlen * dir;
                }
                if(!particles.fix[springdampers[i].idx2]){
                    particles.vert[springdampers[i].idx2] = 
                        center + 0.25f * springdampers[i].restlen * dir;
                }
            }

            // apply force
            Vector3 vclose = dir * Vector3.Dot(particles.vel[springdampers[i].idx2] - 
                                               particles.vel[springdampers[i].idx1], 
                                               dir);
            Vector3 force = 2000 /* ks */ * deltax * dir + 1.2f /* kd */ * vclose;
            particles.acc[springdampers[i].idx1] += force;
            particles.acc[springdampers[i].idx2] -= force;
        }
    }

    void ApplyWindForce(){
        // generate random wind coefficient
        Vector3 wind_coef = new Vector3(
            /*1.0f + (Random.Range() - 0.5f) * 0.4f,
            1.0f + (random.NextDouble() - 0.5f) * 0.4f,
            1.0f + (random.NextDouble() - 0.5f) * 0.4f*/

            Random.Range(0.0f, 2.0f),
            Random.Range(0.0f, 2.0f),
            Random.Range(0.0f, 2.0f)
        );
        
        for(int i = 0; i < triangles.Count; i++){
            // calculate area of this triangle
            Vector3 v1 = particles.vert[triangles[i].idx2] - particles.vert[triangles[i].idx1];
            Vector3 v2 = particles.vert[triangles[i].idx3] - particles.vert[triangles[i].idx1];
            float area = 0.5f * Vector3.Cross(v1, v2).magnitude;

            // calculate normal of this triangle
            Vector3 norm = (area==0) ? new Vector3(0.0f, 0.0f, 1.0f) : Vector3.Cross(v1, v2);

            // triangle's speed = average speed of 3 vertices
            Vector3 velt = (particles.vel[triangles[i].idx1] + 
                            particles.vel[triangles[i].idx2] +
                            particles.vel[triangles[i].idx3]) / 3.0f;
            
            // close speed = speed difference between triangle and wind 
            Vector3 velc = velt - Vector3.Scale(wind, wind_coef);

            // larger cross area will get large force by wind
            float crossarea = (velc.magnitude == 0) ? 0 : area * Vector3.Dot(velc, norm) / velc.magnitude;

            // apply force, note that each vertex will get only 1/3 of this force
            Vector3 force = - (Vector3.Dot(velc, velc) * crossarea * norm * 0.5f) / 3.0f;
            particles.acc[triangles[i].idx1] += force;
            particles.acc[triangles[i].idx2] += force;
            particles.acc[triangles[i].idx3] += force;
        }
    }

    void Move(){
        float step = 0.01f;
        for(int i = 0; i < particles.size; i++){
            if(!particles.fix[i]){
                particles.vel[i] += step * particles.acc[i];
                particles.vert[i] += step * particles.vel[i];
            }
        }
    }
}

public class Particles{
    public List<Vector3> vert; // mesh vertices
    public List<Vector3> vel;  // velocity
    public List<Vector3> acc;  // acceleration
    public List<bool> fix;     // if the vertex is fixed
    public int size;           // size of all Lists above

    public Particles(Vector3[] _vert, List<int> _pfixindex){
        size = _vert.Length;
        vert = new List<Vector3>();
        vel = new List<Vector3>();
        acc = new List<Vector3>();
        fix = new List<bool>();
        for(int i = 0; i < _vert.Length; i++){
            vert.Add(_vert[i]);
        }
        for(int i = 0; i < size; i++){
            vel.Add(new Vector3());
            acc.Add(new Vector3());
            fix.Add(false);
        }
        for(int i = 0; i < _pfixindex.Count; i++){
            fix[_pfixindex[i]] = true;
        }
    }
}

public class Triangle{
    public int idx1;
    public int idx2;
    public int idx3;

    public Triangle(int _idx1, int _idx2, int _idx3){
        idx1 = _idx1;
        idx2 = _idx2;
        idx3 = _idx3;
    }
}

public class SpringDamper{
    public int idx1;
    public int idx2;
    public float restlen; // default length of springs

    public SpringDamper(int _idx1, int _idx2, float _restlen){
        idx1 = _idx1;
        idx2 = _idx2;
        restlen = _restlen;
    }
}
