using UnityEngine;

public class PlayerDivideManager : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerPrefab;
    public PlayerMovement originalPlayer;

    [Header("Division Settings")]
    public float recombineRadius;

    PlayerMovement bodyA;
    PlayerMovement bodyB;

    bool isDivided;
    PlayerMovement activePlayer;

    void Start()
    {
        bodyA = originalPlayer;
        activePlayer = bodyA;
    }

    void Update()
    {
        var input = InputManager.instance.Input.Gameplay;

        if (input.Divide.triggered && !isDivided)
        {
            Divide();
        }

        if (input.Swap.triggered && bodyA != null && bodyB != null)
        {
            SwapBodies();
        }

        if (input.Recombine.IsPressed() && isDivided)
        {
            TryRecombine();
        }
    }

    void Divide()
    {
        isDivided = true;

        Vector3 spawnOffset = bodyA.transform.forward * 0.5f;
        bodyB = Instantiate(playerPrefab, bodyA.transform.position + spawnOffset, bodyA.transform.rotation);

        // Momentum split
        Vector3 velocity = bodyA.GetComponent<Rigidbody>().linearVelocity;
        bodyB.GetComponent<Rigidbody>().linearVelocity = velocity * 2f;
        bodyA.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
 

        bodyA.SetActive(false);
        bodyB.SetActive(true);

        activePlayer = bodyB;
    }

    void SwapBodies()
    {
        activePlayer.SetActive(false);

        activePlayer = activePlayer == bodyA ? bodyB : bodyA;

        activePlayer.SetActive(true);
    }

    void TryRecombine()
    {
        float dist = Vector3.Distance(bodyA.transform.position, bodyB.transform.position);

        if (dist <= recombineRadius)
        {
            Recombine();
        }
    }

    void Recombine()
    {
        isDivided = false;

        PlayerMovement survivor = activePlayer;
        PlayerMovement absorbed = (activePlayer == bodyA) ? bodyB : bodyA;

        survivor.SetActive(true);


        Destroy(absorbed.gameObject);

        bodyA = survivor;
        bodyB = null;

        activePlayer = survivor;
        originalPlayer = activePlayer;
    }
}
