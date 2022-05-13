using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Pickup : MonoBehaviour
{
    public GameObject inHand;


    private List<GameObject> itemsInReach = new List<GameObject>();

    public bool BallInReach => itemsInReach.Count > 0;
    public bool BallInHand => inHand != null;

    private PlayerInput playerInput;
    private InputAction use;
    private InputAction drop;
    void Start()
    {
        if (playerInput == null)
        {
            playerInput = FindObjectOfType<PlayerInput>();
            if (playerInput == null)
                throw new MissingComponentException("Missing a PlayerInput componenet");
            use = playerInput.actions["Use"];
            drop = playerInput.actions["Drop"]; 
        }
    }

    private int lastUse = 0;
    private int lastDrop = 0;

    private int currentUse = 0;
    private int currentDrop = 0;

    public void SetUse(int use) 
    {
        currentUse = use;
    }

    public void Reload()
    {
        itemsInReach.Clear();
    }

    public void SetDrop(int drop) 
    {
        currentDrop = drop;
    }

    private void Update()
    {
        if (currentUse != lastUse && lastUse == 0) 
        {
            RunPickup();
        }
        lastUse = currentUse;
        if (currentDrop != lastDrop && lastDrop == 0) 
        {
            RunDrop();
        }
        lastDrop = currentDrop; 
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) 
        {
            itemsInReach.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (itemsInReach.Contains(other.gameObject))
            {
                itemsInReach.Remove(other.gameObject);
            }
        }
    }

    public UnityEvent BallPickup;

    public void RunPickup() 
    {
        if (itemsInReach.Count > 0) 
        {
            RunDrop();
            BallPickup.Invoke();
            inHand = itemsInReach[0];
            itemsInReach.RemoveAt(0);
            if (inHand == null)
            {
                Debug.Log("Ball Error");
                return;
            }
            inHand.tag = "Untagged";
            inHand.transform.SetParent(this.transform);
            inHand.transform.position = transform.position;
            inHand.GetComponent<Ball>().SetOwner(GetComponentInParent<BehaviorParameters>().TeamId);
            var rb = inHand.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;
        }
    }

    public void RunDrop() 
    {
        if (inHand != null) 
        {
            inHand.transform.SetParent(null);
            var rb = inHand.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.velocity = Vector3.zero;
            inHand.tag = "Ball";
            inHand = null;
        }
    }
}
