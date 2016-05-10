using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public enum FireMode { Auto,Burst,Single};
    public FireMode fireMode;

    public Transform[] projectilteSpawn;
    public Projectile projecttile;
    public float msBetweenShots = 100;
    public float muzzleeVelocity = 35;
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = .3f;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(2, 5);
    public float recoilMoveSettleTime = .1f;
    public float recoilRotationSettleTime = .1f;

    [Header("Effect")]
    public Transform Shell;
    public Transform ShellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    float nextShootTime;
    MuzzleFlash muzzleFlash;


    Transform bulletShoot;
    Transform shell;

    bool triggerReleasedSinceLastShot;
    int shotRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    void Start()
    {
        bulletShoot = new GameObject("Bullet").transform;
        shell = new GameObject("Shell").transform;

        muzzleFlash = GetComponent<MuzzleFlash>();
        shotRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, .1f);
        transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
    }

    void Shoot()
    {
        if (!isReloading && Time.time > nextShootTime && projectilesRemainingInMag >0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotRemainingInBurst == 0)
                {
                    return;
                }

                shotRemainingInBurst--;
            }
            else
            {
                if(fireMode == FireMode.Single)
                {
                    if (!triggerReleasedSinceLastShot)
                    {
                        return;
                    }
                }
            }

            for(int i = 0; i < projectilteSpawn.Length; i++)
            {
                if (projectilesRemainingInMag == 0) break;
                projectilesRemainingInMag--;
                nextShootTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projecttile, projectilteSpawn[i].position, projectilteSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleeVelocity);
                newProjectile.transform.parent = bulletShoot;                
            }

            Transform newShell = Instantiate(Shell, ShellEjection.position, ShellEjection.rotation) as Transform;
            newShell.parent = shell;
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.intance.PlaySound(shootAudio, transform.position);

        }

    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
            AudioManager.intance.PlaySound(reloadAudio, transform.position);
        }
       
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1 / reloadTime;
        float precent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;


        while (precent < 1)
        {
            precent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(precent, 2) + precent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;


            yield return null;
        }


        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }

    
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotRemainingInBurst = burstCount;
    }
}
