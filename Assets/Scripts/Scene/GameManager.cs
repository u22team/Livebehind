using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playCharPrefab, playCharacter;

    void Retry()
    {
        playCharacter = Instantiate(playCharPrefab, new Vector3(0, 0.15f, 0.05f), Quaternion.identity);
    }

    public void Kill(GameObject gameObject)
    {
        Destroy(gameObject);
        Retry();
    }
}
