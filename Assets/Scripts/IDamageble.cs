using UnityEngine;
using System.Collections;

public interface IDamageble {

    void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDrection);

    void TakeDamage(float damage);
}
