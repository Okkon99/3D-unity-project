using UnityEngine;

public class BackpackTrigger : MonoBehaviour
{
    PlayerDivideManager manager;
    PlayerMovement owner;

    public void Initialize(PlayerDivideManager divideManager, PlayerMovement triggerOwner)
    {
        manager = divideManager;
        owner = triggerOwner;
    }

    void OnTriggerEnter(Collider other)
    {
        if (manager == null || owner == null)
        {
            return;
        }

        manager.NotifyBackpackTrigger(owner, other);
    }
}
