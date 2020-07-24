/* Adding functions?
 * 
 * 1) Create a new static Vector3 function(int u, int v, int t) in Graph
 * 2) Add the function name to the Graph.functions array
 * 3) Add a label for the function in GraphFunctionEnum
 * 
 * NOTE: Make sure the order of GraphFunctionEnum matches the order of Graph.functions, else your labels won't match up
 * 
 * To run the program: 
 * 1) Enter play mode in the editor
 * 2) Select the graph game object in the inspector and chose your function from the dropdown
 * 3) You can cycle through the functions by pressing the spacebar
 * 
 * To change the resolution (# of points), stop the program, change the resolution on the Graph object, then start the program
 */

using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;
    Transform[] points;
    
    [Range(10, 100)] 
    public int resolution = 10;
    public GraphFunctionEnum function;

    static Dictionary<string, float> constants = new Dictionary<string, float>();

    static GraphFunction[] functions = { 
        SineFunction, Sine2DFunction, MultiSineFunction, MultiSine2DFunction, RippleFunction,
        CylinderFunction, SphereFunction, TorusFunction, SwanFunction, CircularSwanFunction
    };

    void Awake() {
        float step = 2f / resolution;
        Vector3 scale = Vector3.one * step;
        points = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++) {
            Transform point = points[i] = Instantiate(pointPrefab, transform, false);
            point.localScale = scale;
        }
    }

    void Update() {
        float t = Time.time;
        GraphFunction f = functions[(int)function];

        float step = 2f / resolution;
        for (int i = 0, z = 0; z < resolution; z++) {
            float v = (z + 0.5f) * step - 1f;

            for (int x = 0; x < resolution; x++, i++) {
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = f(u, v, t);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            function = (int)function == functions.Length - 1 ? 0 : function + 1;
        }
    }

    const float pi = Mathf.PI;

    static Vector3 SineFunction(float x, float z, float t) {
        return new Vector3(x, Mathf.Sin(pi * (x + t)), z);
    }

    static Vector3 MultiSineFunction(float x, float z, float t) {
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(2f * pi * (x + 2f * t)) * 0.5f;
        y *= 2f / 3f;
        return new Vector3(x, y, z);
    }

    static Vector3 Sine2DFunction(float x, float z, float t) {
        float y = Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(pi * (z + t));
        y *= 0.5f;
        return new Vector3(x, y, z);
    }

    static Vector3 MultiSine2DFunction(float x, float z, float t) {
        float y = 4f * Mathf.Sin(pi * (x + z + (t * 0.5f)));
        y += Mathf.Sin(pi * (x + t));
        y += Mathf.Sin(2f * pi * (z + 2f * t)) * 0.5f;
        y *= 1f / 5.5f;
        return new Vector3(x, y, z);
    }

    static Vector3 RippleFunction(float x, float z, float t) {
        float d = Mathf.Sqrt(x * x + z * z);
        float y = Mathf.Sin(pi * (4f * d - t));
        y /= 1f + 10f * d;
        return new Vector3(x, y, z);
    }

    static Vector3 CylinderFunction(float u, float v, float t) {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * ( 6f * u + 2f * v + t)) * 0.2f;

        p.x = r * Mathf.Sin(pi * u);
        p.y = v;
        p.z = r * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 SphereFunction(float u, float v, float t) {
        Vector3 p;
        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;

        float s = r * Mathf.Cos(pi * 0.5f * v);

        p.x = s * Mathf.Sin(pi * u);
        p.y = r * Mathf.Sin(pi * 0.5f * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 TorusFunction(float u, float v, float t) {
        Vector3 p;
		float r1 = 0.65f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
		float r2 = 0.2f + Mathf.Sin(pi * (4f * v + t)) * 0.05f;
        float s = r2 * Mathf.Cos(pi * v) + r1;

        p.x = s * Mathf.Sin(pi * u);
        p.y = r2 * Mathf.Sin(pi * v);
        p.z = s * Mathf.Cos(pi * u);
        return p;
    }

    static Vector3 SwanFunction(float u, float v, float t)
    {
        if (!constants.ContainsKey("SwanFunction_C")) {
            float newC = 576f / Mathf.Pow(pi, 6f);
            constants.Add("SwanFunction_C", newC);
        }

        float c = constants["SwanFunction_C"];
        float n = 1f;
        float m = 1f;

        float x = 1f + Mathf.Pow(-1f, m + 1f);
        x *= (1f + Mathf.Pow(-1, n + 1f));
        x /= m * m * m * n * n * n; // m^3 * n ^ 3
        x *= Mathf.Sin(m * pi * u * 0.5f);

        float y = Mathf.Sin((n * pi * v) / 3f);
        y *= Mathf.Cos(pi * Mathf.Sqrt(9f * m * m + 4f * n * n * t)); // cos pi root (9m^2 + 4n^2 * t)

        Vector3 p;
        p.x = u;
        p.y = x * y * c;
        p.z = v;

        return p;
    }

    static Vector3 CircularSwanFunction(float u, float v, float t)
    {
        if (!constants.ContainsKey("SwanFunction_C")) {
            float newC = 576f / Mathf.Pow(pi, 6f);
            constants.Add("SwanFunction_C", newC);
        }

        float c = constants["SwanFunction_C"];
        float n = 1f;
        float m = 1f;

        float x = 1f + Mathf.Pow(-1f, m + 1f);
        x *= (1f + Mathf.Pow(-1, n + 1f));
        x /= m * m * m * n * n * n; // m^3 * n ^ 3
        x *= Mathf.Sin(m * pi * u * 0.5f);

        float y = Mathf.Sin((n * pi * v) / 3f);
        y *= Mathf.Cos(pi * Mathf.Sqrt(9f * m * m + 4f * n * n * t)); // cos pi root (9m^2 + 4n^2 * t)

        float r = 0.8f + Mathf.Sin(pi * (6f * u + t)) * 0.1f;
        r += Mathf.Sin(pi * (4f * v + t)) * 0.1f;

        float s = r * Mathf.Cos(pi * 0.5f * v);

        Vector3 p;
        p.x = s * Mathf.Sin(pi * u);
        p.y = x * y * c;
        p.z = s * Mathf.Cos(pi * u);

        return p;
    }

}
