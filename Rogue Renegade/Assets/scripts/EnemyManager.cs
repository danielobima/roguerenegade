using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<EnemyMech> enemies;
    public Vector3 lastKnownPos;
    public Target enemyTarget;

    private void Start()
    {
        
    }
    public void EnemySpotted()
    {
        Debug.Log("EnemySpotted");
        foreach (EnemyMech enemyMech in enemies)
        {
            enemyMech.enemyTarget = enemyTarget;
            enemyMech.enemyTransform = enemyTarget.transform;
            enemyMech.timeToAttack = true;
            enemyMech.searchForPlayer();
            enemyMech.vision.isChecking = false;
        }
    }
    public void SearchingAroundForEnemy(EnemyMech caller)
    {
        Debug.Log("Searching");
        int i = 0;
        foreach(EnemyMech enemyMech in enemies)
        {
            if (!enemyMech.vision.stillSeeing)
            {
                i++;
            }
            else
            {
                Debug.Log("Enemy still visible");
                caller.canAttack = true;
                caller.enemyTarget = enemyTarget;
                caller.enemyTransform = enemyTarget.transform;
                caller.searchForPlayer();
                return;
            }
          
                
        }
        
        if(i >= enemies.Count)
        {
            EnemyLost();
        }
    }
    public void EnemyLost()
    {
        Debug.Log("EnemyLost");
        enemyTarget = null;
        foreach (EnemyMech enemyMech in enemies)
        {
            enemyMech.enemyTarget = null;
            enemyMech.enemyTransform = null;
            enemyMech.timeToAttack = false;
            enemyMech.vision.isChecking = true;
        }
    }
   
}
