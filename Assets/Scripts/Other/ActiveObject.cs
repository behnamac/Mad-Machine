using UnityEngine;

namespace Other
{
    public class ActiveObject : MonoBehaviour
    {
        private void OnDisable()
        {
            Invoke(nameof(Active), 3);
        }

        private void Active()
        {
            gameObject.SetActive(true);
        }
    }
}
