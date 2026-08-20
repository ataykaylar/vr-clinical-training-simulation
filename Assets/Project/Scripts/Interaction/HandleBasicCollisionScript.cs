using UnityEngine;

public class HandleBasicCollisionScript : MonoBehaviour
{
    public string collideWithThatName = " ";
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.CollideWithThat(collideWithThatName);
        }
    }
}
