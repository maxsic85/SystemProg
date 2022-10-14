using System.Threading;
using System.Threading.Tasks;
using UnityEngine;



namespace HomeWork01
{
    public sealed class AsynAwait : MonoBehaviour
    {
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancelToken;
        private const int _fps = 60;
        private const int _delay = 5000;


        async void Start()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = _cancelTokenSource.Token;

            Task task = Task.Run(() => OnePrintAndClose(_cancelToken, _delay));
            Task task2 = Task.Run(() => WaitForFPS(_cancelToken, _fps));

            task.Start();
            task2.Start();

            await Task.WhenAll(task, task2);
        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _cancelTokenSource.Cancel();
            }
        }


        private async Task OnePrintAndClose(CancellationToken cancelationToken, int delay)
        {
            await Task.Delay(delay);
            if (cancelationToken.IsCancellationRequested)
            {
                Debug.Log("Операция прервана токеном.");
                return;
            }
            Debug.Log($"Message system was shutoff.");
        }


        private async Task WaitForFPS(CancellationToken cancellationToken, int fps)
        {
            while (fps > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.Log($"Операция прервана токеном.");
                    return;
                }
                await Task.Yield();
                --fps;
            }
        }


        private void OnDestroy()
        {
            _cancelTokenSource.Cancel();
        }
    }
}
