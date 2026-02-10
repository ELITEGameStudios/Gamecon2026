using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    public bool containsPlayer;
    [SerializeField] UnityEvent playerEnterEvent;
    [SerializeField] UnityEvent playerStayEvent;
    [SerializeField] UnityEvent playerExitEvent;
    

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null){
            containsPlayer = true;
            playerEnterEvent.Invoke();
        }
    }
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null){
            containsPlayer = true;
            playerStayEvent.Invoke();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null){
            containsPlayer = false;
            playerExitEvent.Invoke();
        }
    }
}
