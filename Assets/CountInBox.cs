using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountInBox : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    public int Count => objects.Count;

    public TMPro.TextMeshPro text;

    public int lastBallOwner = -1;

    public UnityEvent onBallLoss;
    public UnityEvent onBallGain;

    public void Reset()
    {
        objects.Clear();
        if (text != null)
            text.text = Count.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            objects.Add(other.gameObject);
            lastBallOwner = other.GetComponent<Ball>().Owner;
            onBallGain.Invoke();
        }
        if(text != null)
            text.text = Count.ToString();
    }

    private void OnTriggerExit(Collider other)
    {
        if (objects.Contains(other.gameObject))
        {
            objects.Remove(other.gameObject);
            onBallLoss.Invoke();
        }
        if (text != null)
            text.text = Count.ToString();
    }

    private void Start()
    {
        Knowledge.boxes.Add(this);
        if (text != null)
            text.text = Count.ToString();
    }
}
