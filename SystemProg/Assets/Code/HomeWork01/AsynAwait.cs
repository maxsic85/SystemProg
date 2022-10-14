using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;




namespace HomeWork01
{
    public sealed class AsynAwait : MonoBehaviour
    {
       private CancellationTokenSource _cancelTokenSource;
       private CancellationToken _cancelToken;


         async void Start()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;

            Task task = Task.Run(() => OnePrintAndClose(_cancelToken));
            task.Start();
            await Task.WhenAll(task);

        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _cancelTokenSource.Cancel();
            }
        }


        private async Task OnePrintAndClose(CancellationToken cancelationToken)
        {
            await Task.Delay(5000);
            if (cancelationToken.IsCancellationRequested)
            {
                Debug.Log("Операция прервана токеном.");
                return;
            }
            Debug.Log($"Message system was shutoff.");
        }


        private void OnDestroy()
        {
            _cancelTokenSource.Cancel();
        }
    }
}
