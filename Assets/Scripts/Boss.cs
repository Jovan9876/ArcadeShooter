using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private LaserAttack laserAttack;
    private EnemyShotgun shotgunAttack;

    public float laserAttackDelay = 2f;
    public float shotgunAttackDelay = 1f;

    void Start()
    {
        laserAttack = GetComponent<LaserAttack>();
        shotgunAttack = GetComponent<EnemyShotgun>();

        StartCoroutine(AttackPattern());
    }

    IEnumerator AttackPattern()
    {
        while (true)
        {
            StartCoroutine(StartLaserWithDelay());
            StartCoroutine(StartShotgunWithDelay());

            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator StartLaserWithDelay()
    {
        yield return new WaitForSeconds(laserAttackDelay);
        laserAttack.SendMessage("StartLaserAttack");
    }

    IEnumerator StartShotgunWithDelay()
    {
        yield return new WaitForSeconds(shotgunAttackDelay);
        shotgunAttack.SendMessage("Shoot");
    }
}
