using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;



namespace HomeWork02
{
    public struct JobForTransforExample : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<int> Velocity;
        [ReadOnly] public float DeltaTime;
        public NativeArray<Vector3> RotationSpeed;
        private Quaternion _quaternion;


        public void Execute(int index, TransformAccess transform)
        {
            RotationSpeed[index] += new Vector3(0, 1, 0) * DeltaTime * Velocity[index];
            _quaternion.eulerAngles = RotationSpeed[index];
            transform.rotation = _quaternion;
        }
    }
}