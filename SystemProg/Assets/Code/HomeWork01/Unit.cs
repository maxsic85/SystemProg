using System.Collections;
using UnityEngine;


namespace HomeWork01
{
    public class Unit : MonoBehaviour
    {
        [SerializeField] private int _health = 24;
        [SerializeField] private  bool _isUse = false;
        private const int _maxHealth = 100;
        private const int _healthOneCastPower = 5;

        public bool IsUse { get => _isUse; private set => _isUse = value; }

        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {         
                ReceiveHealing(ref _isUse);
                return;
            }
        }

        public void ReceiveHealing(ref bool isUse)
        {
            if (isUse) return;
            isUse = true;
            Coroutine coroutine = StartCoroutine(ResetCast());
        }

        private IEnumerator HealingCoroutine(int healthOneCastPower, int maxHealth)
        {
         
            while (_health <= maxHealth)
            {

                if ((_health + healthOneCastPower) > maxHealth)
                    yield break;

                _health += healthOneCastPower;
                yield return new WaitForSeconds(0.5f);
            }
          
        }

        private IEnumerator ResetCast()
        {
            yield return StartCoroutine(HealingCoroutine(_healthOneCastPower, _maxHealth));
            IsUse = false;
        }
    }
}