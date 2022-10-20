using Unity.Collections;
using Unity.Jobs;
using UnityEngine;



namespace HomeWork02
{
    public class TestIJobParallelFor : MonoBehaviour
    {
        private const int _lenght = 2;
        private const int _startDistance = 100;
        private const int _startVelocity = 10;

        private NativeArray<Vector3> _positions;
        private NativeArray<Vector3> _velosities;
        private NativeArray<Vector3> _finalPositions;


        private void Start()
        {
            FillData();

            JobShcedule();
        }

        private void JobShcedule()
        {
            JobParallelForExample jobParallel = new JobParallelForExample()
            {
                Positions = _positions,
                Velocities = _velosities,
                FinalPositions = _finalPositions
            };

            JobHandle parrallelJobHandle = jobParallel.Schedule(_lenght, 0);
            parrallelJobHandle.Complete();

            if (parrallelJobHandle.IsCompleted)
            {
                for (int i = 0; i < _lenght; i++)
                {
                    Debug.Log($"Positions: {jobParallel.Positions[i]}");
                    Debug.Log($"Velocities: {jobParallel.Velocities[i]}");
                    Debug.Log($"FinalPositions: {jobParallel.FinalPositions[i]}");
                }
            }
        }

        private void FillData()
        {
            _positions = new NativeArray<Vector3>(_lenght, Allocator.Persistent);
            _velosities = new NativeArray<Vector3>(_lenght, Allocator.Persistent);
            _finalPositions = new NativeArray<Vector3>(_lenght, Allocator.Persistent);

            for (int i = 0; i < _lenght; i++)
            {
                _positions[i] = Random.insideUnitSphere * Random.Range(0, _startDistance);
                _velosities[i] = Random.insideUnitSphere * Random.Range(0, _startVelocity);
            }
        }

        private void OnDestroy()
        {
            _positions.Dispose();
            _velosities.Dispose();
            _finalPositions.Dispose();
        }
    }
}