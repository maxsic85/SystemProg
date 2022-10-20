using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;


public class TestJobForTransform:MonoBehaviour
{
    [SerializeField] public Transform[] m_Transforms;
    TransformAccessArray m_AccessArray;
    private void Awake()
    {
        m_AccessArray = new TransformAccessArray(m_Transforms);
    }

    private void Update()
    {

        var velocity = new NativeArray<int>(m_Transforms.Length, Allocator.Persistent);
    
        // for (var i = 0; i < m_Transforms.Length; ++i)
        //   velocity[i] = 100;
      
        JobForTransforExample transformJob = new JobForTransforExample()
        {
           Velocity = velocity,
           DeltaTime = Time.deltaTime
        };
        JobHandle moveHandle = transformJob.Schedule(m_AccessArray);
        moveHandle.Complete();
        velocity.Dispose();
        
    }

    private void OnDestroy()
    {      
        m_AccessArray.Dispose();
    }
}