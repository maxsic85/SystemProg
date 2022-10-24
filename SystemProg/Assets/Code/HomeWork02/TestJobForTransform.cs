using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;



namespace HomeWork02
{
    public class TestJobForTransform : MonoBehaviour
    {
        [SerializeField] private Transform[] _transforms;
        [SerializeField] private int[] _velocities;

        private TransformAccessArray _accessArray;
        private NativeArray<int> _velocity;
        private NativeArray<Vector3> _rotationSpeed;


        private void Awake()
        {
            _accessArray = new TransformAccessArray(_transforms);
        }


        private void Start()
        {
            _velocity = new NativeArray<int>(_transforms.Length, Allocator.Persistent);
            _rotationSpeed = new NativeArray<Vector3>(_transforms.Length, Allocator.Persistent);

            for (int i = 0; i < _transforms.Length; i++)
            {
                _velocity[i] = _velocities[i];
            }
        }


        private void Update()
        {
            JobForTransforExample transformJob = new JobForTransforExample()
            {
                Velocity = _velocity,
                RotationSpeed = _rotationSpeed,
                DeltaTime = Time.deltaTime
            };

            JobHandle moveHandle = transformJob.Schedule(_accessArray);
            moveHandle.Complete();
        }


        private void OnDestroy()
        {
            _accessArray.Dispose();
            _velocity.Dispose();
            _rotationSpeed.Dispose();
        }
    }
}