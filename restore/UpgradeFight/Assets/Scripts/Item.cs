using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { Ammo, Coin, Grenade, Heart, Weapon };
    public Type type;
    public int value;
    Vector3 randVec;

    Rigidbody rigid;
    SphereCollider sphereCollider;

    private void Awake()
    {
        randVec = new Vector3(Random.Range(-40, 40), 3, Random.Range(-40, 40));
        transform.position = randVec;
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void Update()
    {

        transform.Rotate(Vector3.up * 10 * Time.deltaTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
