using Unity.Collections;
using Unity.Jobs;
using UnityEngine;



namespace HomeWork02
{
    public class TestJob:MonoBehaviour 
    {
        private const int Length = 10;

        void Start()
        {
            NativeArray<int> array = new NativeArray<int>(Length, Allocator.TempJob);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Random.Range(0, 100);
                Debug.Log($"input {array[i]}");
            }

            JobStruct job = new JobStruct();
            job.array = array;
          
            JobHandle handle = job.Schedule();
            handle.Complete();

            if (handle.IsCompleted)
            {
                for (int i = 0; i < job.array.Length; i++)
                {
                    Debug.Log($"output: {job.array[i]}");
                }
              
            }
                  
            array.Dispose();

        }
    }

}
