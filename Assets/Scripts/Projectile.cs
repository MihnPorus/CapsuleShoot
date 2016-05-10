using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
    public LayerMask collisionMask;
    public Color trailColour;
    float speed = 10;
    float damage = 1;

    float lifetime = 2;

    float skinWifth = .1f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0],transform.position);
        }
        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColour);
    }

    public void SetSpeed( float newSpeed)
    {
        speed = newSpeed;
    }
	// Update is called once per frame
	void Update () {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance,Space.Self);
	}

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray,out hit, moveDistance + skinWifth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider,hit.point);
        }
    }

    

    void OnHitObject(Collider c,Vector3 hitPoint)
    {
        IDamageble damageableObject = c.GetComponent<IDamageble>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage,hitPoint,transform.forward);
        }
        GameObject.Destroy(gameObject);
    }
}
