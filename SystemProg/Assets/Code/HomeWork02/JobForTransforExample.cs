using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

public struct JobForTransforExample : IJobParallelForTransform
{
     public NativeArray<int> Velocity;
    [ReadOnly] public float DeltaTime;
    private Quaternion _quaternion;
    private Vector3 _speed;
    private int a ;
    public void Execute(int index, TransformAccess transform)
    {
        Velocity[index] = (Velocity[index] >= 180) ? 0 : Velocity[index]+=1;
       // _speed += new Vector3(0, Velocity[index], 0) * DeltaTime * 1000;// Velocity[index];
        _speed += new Vector3(0, Velocity[index], 0) * DeltaTime * 0.1f;// Velocity[index];
        _quaternion.eulerAngles  = _speed;
        transform.rotation = _quaternion;
    }
}