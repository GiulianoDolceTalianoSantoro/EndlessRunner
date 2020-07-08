using UnityEngine;

public class Coin : MonoBehaviour
{
    private void Update()
    {
        Spin();
    }

    private void Spin()
    {
        transform.Rotate(0, 50 * Time.deltaTime, 0);
    }
}
