using UnityEngine;

public class KeyPickable : MonoBehaviour
{
    public string keyName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().cardsCollectedYellow++;
            Debug.Log(other.gameObject.GetComponent<PlayerController>().cardsCollectedYellow);
            Destroy(gameObject);
        }
    }
}
