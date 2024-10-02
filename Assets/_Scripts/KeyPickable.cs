using UnityEngine;

public class KeyPickable : MonoBehaviour
{
    public string keyName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerInteraction>().cardsCollectedYellow++;
            Debug.Log(other.gameObject.GetComponent<PlayerInteraction>().cardsCollectedYellow);
            Destroy(gameObject);
        }
    }
}
