using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MiniWorldGravity : MonoBehaviour
{
    //World setup
    [SerializeField] float gravity = -9.8f;
    [SerializeField] float attractRadius = 20f;

    //Debug Variables
    [SerializeField] Color debugAttractRadiusColor;

    //Variables
    SphereCollider miniWorldCollider;

    


    void Start()
    {
        miniWorldCollider = GetComponent<SphereCollider>();
    }

    void FixedUpdate()
    {
        AttractNearbyObjects();
    }



    //Find attractable objects inside the defined radius, and pull them to the mini world
    void AttractNearbyObjects()
    {
        //You can optimize this by having the attractive objects instanciated and accessing them directly, instead of search in the radius every update
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attractRadius);
        GameObject gameObj;
        for (int i = 0; i < hitColliders.Length; i++)
        {
            gameObj = hitColliders[i].gameObject;
            if(gameObj.tag == "Player") //You can also filter which ones you want to affected by the mini world gravity here
                Attract(gameObj.GetComponent<Rigidbody>());
        }
    }

    //Attract object to the mini world
    void Attract(Rigidbody rbody)
    {
        Vector3 gravityUp = (rbody.position - transform.position).normalized;
        rbody.AddForce(gravityUp * gravity);

        RotateBody(rbody);
    }

    void HoldinMiniWorldSurface(Rigidbody rbody)
    {
        rbody.MovePosition((rbody.position - transform.position).normalized * (transform.localScale.x * miniWorldCollider.radius));

        RotateBody(rbody);
    }

    //Correctly rotate the object to the mini world surface
    void RotateBody(Rigidbody rbody)
    {
        Vector3 gravityUp = (rbody.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(rbody.transform.up, gravityUp) * rbody.rotation;
        rbody.MoveRotation(Quaternion.Slerp(rbody.rotation, targetRotation, 50f * Time.deltaTime));
    }




    #region Debug functions
    void OnDrawGizmosSelected()
    {
        Gizmos.color = debugAttractRadiusColor;
        Gizmos.DrawSphere(transform.position, attractRadius);
    }
    #endregion

}
