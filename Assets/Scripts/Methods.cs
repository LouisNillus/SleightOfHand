using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Methods
{

    public static Vector3 ZeroZMousePos()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
    }
    
    public static Vector3 ZDistanceMousePos(float distanceFromCamera)
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCamera));
    }

    public static Vector3 ChangeX(this Vector3 vec, float xValue)
    {
        return new Vector3(xValue, vec.y, vec.z);
    }
    public static Vector3 ChangeY(this Vector3 vec, float yValue)
    {
        return new Vector3(vec.x, yValue, vec.z);
    }
    public static Vector3 ChangeZ(this Vector3 vec, float zValue)
    {
        return new Vector3(vec.x, vec.y, zValue);
    }

    public static Vector3 OffsetX(this Vector3 vec, float xValue)
    {
        return new Vector3(vec.x + xValue, vec.y, vec.z);
    }
    public static Vector3 OffsetY(this Vector3 vec, float yValue)
    {
        return new Vector3(vec.x, vec.y + yValue, vec.z);
    }
    public static Vector3 OffsetZ(this Vector3 vec, float zValue)
    {
        return new Vector3(vec.x, vec.y, vec.z + zValue);
    }

    public static Vector3 ClampX(this Vector3 vec, float min, float max)
    {
        return new Vector3(Mathf.Clamp(vec.x, min, max), vec.y, vec.z);
    }
    public static Vector3 ClampY(this Vector3 vec, float min, float max)
    {
        return new Vector3(vec.x, Mathf.Clamp(vec.y, min, max), vec.z);
    }
    public static Vector3 ClampZ(this Vector3 vec, float min, float max)
    {
        return new Vector3(vec.x, vec.y, Mathf.Clamp(vec.z, min, max));
    }

    public static Vector3 MultiplyVector(Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1.x * vec2.x, vec1.y * vec2.y, vec1.z * vec2.z);
    }
    public static Vector3 MultiplyVector(Vector3 vec1, Vector3 vec2, Vector3 vec3)
    {
        return new Vector3(vec1.x * vec2.x * vec3.x, vec1.y * vec2.y * vec3.y, vec1.z * vec2.z * vec3.z);
    }
    public static Vector3 MultiplyVector(Vector3 vec1, Vector3 vec2, float value)
    {
        return new Vector3(vec1.x * vec2.x * value, vec1.y * vec2.y * value, vec1.z * vec2.z * value);
    }
    public static Vector3 NoisyVector(this Vector3 vec, float range)
    {
        return new Vector3(vec.x + Random.Range(-range, range), vec.y + Random.Range(-range, range), vec.z + Random.Range(-range, range));
    }


    //Courbes de bézier

    public static Vector3 QuadraticInterpolation(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }
    public static Vector3 CubicInterpolation(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 ab_bc = QuadraticInterpolation(a, b, c, t);
        Vector3 bc_cd = QuadraticInterpolation(b, c, d, t);

        return Vector3.Lerp(ab_bc, bc_cd, t);
    }

    public static void SetMaterialOpacity(GameObject go, float value)
    {
        MaterialPropertyBlock matProp = new MaterialPropertyBlock();
        matProp.SetFloat("_Treshold", value); //A save et Load en opaque ou carrément changer le shader
        go.GetComponent<MeshRenderer>().SetPropertyBlock(matProp);
    }
    
    public static void SetMaterialColor(GameObject go, Color value)
    {
        if (go.GetComponent<MeshRenderer>() == null) return;

        MaterialPropertyBlock matProp = new MaterialPropertyBlock();
        matProp.SetColor("_BaseColor", value); //HDRP
        //matProp.SetColor("_Color", value); //NON HDRP
        go?.GetComponent<MeshRenderer>()?.SetPropertyBlock(matProp);
    }

    public static void SetEmissiveColor(GameObject go, Color value)
    {
        MaterialPropertyBlock matProp = new MaterialPropertyBlock();
        matProp.SetColor("_EmissiveColorLDR", value); //HDRP
        go?.GetComponent<MeshRenderer>()?.SetPropertyBlock(matProp);
    }

    public static float GetMaterialValue(GameObject go, string value)
    {
        return go.GetComponent<MeshRenderer>().material.GetFloat(value);
    }

    public static bool CheckAboveOrRight(Vector3 vec1, Vector3 vec2, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                if (vec1.x >= vec2.x) return true; else return false;
            case Axis.Y:
                if (vec1.y >= vec2.y) return true; else return false;
            case Axis.Z:
                if (vec1.z >= vec2.z) return true; else return false;
        }

        return true;
    }

    public static bool CheckUnderOrLeft(Vector3 vec1, Vector3 vec2, Axis axis)
    {
        switch (axis)
        {
            case Axis.X:
                if (vec1.x <= vec2.x) return true; else return false;
            case Axis.Y:
                if (vec1.y <= vec2.y) return true; else return false;
            case Axis.Z:
                if (vec1.z <= vec2.z) return true; else return false;
        }

        return true;
    }

    public static float TriangleFonction(float time, float period = 2, float amplitude = 1, float offset = 0)
    {
        return Mathf.Abs((((time/(period/2))+(offset+1)) % (amplitude * 2)) - amplitude);
    }

    public static float NegativeInclusionTriangleFonction(float time, float period = 2, float amplitude = 1, float offset = 0)
    {
        return (((time / (period / 2)) + (offset + 1)) % (amplitude * 2)) - amplitude;
    }

    public static Vector2 xz(this Vector3 vv)
    {
        return new Vector2(vv.x, vv.z);
    }

    public static Vector2 xy(this Vector3 vv)
    {
        return new Vector2(vv.x, vv.y);
    }

    public static Vector2 yz(this Vector3 vv)
    {
        return new Vector2(vv.y, vv.z);
    }

    public static float FlatDistanceTo(this Vector3 from, Vector3 to)
    {
        Vector2 a = from.xz();
        Vector2 b = to.xz();
        return Vector2.Distance(a, b);
    }

    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
}
public enum Axis {X, Y, Z}
