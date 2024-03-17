using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public TorpedoData _data;

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((_data.collideWith & (1 << collision.gameObject.layer)) != 0)
        {
            Debug.Log(collision.gameObject);
            IHittable hitInterface = collision.gameObject.GetComponentInParent<IHittable>();
            if (hitInterface != null)
            {
                hitInterface.Hit(_data.damage, this.gameObject);
            }
            this.gameObject.SetActive(false);
        }
    }
}