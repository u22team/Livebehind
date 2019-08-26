using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{

    PlayCharacterManager playCharacterManager;

    private void Start()
    {
        playCharacterManager = this.gameObject.GetComponent<PlayCharacterManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Damager"))
        {
            /*
            GameManager gameManager = new GameManager();
            gameManager.Kill(this.gameObject);
            */
            //this.gameObject.transform.position = new Vector3(0, 0.15f, 0.05f);
            playCharacterManager.transformPosInt = new Vector3Int(0, 192, 64);
            playCharacterManager.velocityUpwards = Vector3Int.zero;
        }
    }
}
