using UnityEngine;

[System.Serializable]
public class Circle3D
{
    public float radius;
    public Vector3 position;

    public Circle3D(float x, float y, float z, float radius)
    {
        position = new Vector3(x, y, z);
        this.radius = radius;
    }

    public Circle3D(Vector3 position, float radius)
    {
        this.position = position;
        this.radius = radius;
    }

    public void SetPosition(float x, float y, float z) => position.Set(x, y, z);

    public void SetPosition(Vector3 position) => this.position = position;

    public Vector3 GetPosition() => position;
}