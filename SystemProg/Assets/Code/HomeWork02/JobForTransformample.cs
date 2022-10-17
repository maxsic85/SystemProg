using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public struct JobForTransformample : IJobParallelForTransform
{
    [ReadOnly]
    public NativeArray<Vector3> Positions;
    [ReadOnly]
    public float DeltaTime;

    public void Execute(int index, TransformAccess transform)
    {
       
    }
}
