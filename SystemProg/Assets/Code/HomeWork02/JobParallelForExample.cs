using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;



namespace HomeWork02
{
    public struct JobParallelForExample : IJobParallelFor
    {
        //   [ReadOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<Vector3> Positions;
        // [ReadOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<Vector3> Velocities;
        //  [WriteOnly]
        [NativeDisableParallelForRestriction]
        public NativeArray<Vector3> FinalPositions;
        

        public void Execute(int index)
        {
            int[] arraysLenght = { Positions.Length, Velocities.Length, FinalPositions.Length };
            int arrayLenght=arraysLenght.Min();

            for (int i = 0; i < arrayLenght; i++)
            {
                if (i == index) continue;
                FinalPositions[i] = Positions[i] + Velocities[i];
            }
        }
    }

}
