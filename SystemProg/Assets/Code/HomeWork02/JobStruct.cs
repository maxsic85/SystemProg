using Unity.Collections;
using Unity.Jobs;



namespace HomeWork02
{

    public struct JobStruct : IJob
    {
        public NativeArray<int> array;
        public void Execute()
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 10) array[i] = 0;
            }
        }

    }
}
