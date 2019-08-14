using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloor : MonoBehaviour
{
    //元々スルーできて、上に来たときだけ当たるようにする方が良いということに気づいた
    /*
    private void OnTriggerEnter(Collider other)
    {
        //レイヤー変更
        //other.gameObject.GetInstanceID();
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) other.gameObject.layer = LayerMask.NameToLayer("ThroughPlayer");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) other.gameObject.layer = LayerMask.NameToLayer("ThroughPlayer");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("ThroughPlayer")) other.gameObject.layer = LayerMask.NameToLayer("Player");
    }
    */

    //暫定案

    [SerializeField] float up;
    RaycastHit m_Hit;
    void FixedUpdate()
    {
        if (Physics.BoxCast(transform.position + Vector3.up * up, new Vector3(transform.localScale.x, 0, transform.localScale.z), Vector3.up, out m_Hit, Quaternion.identity)) //, m_MaxDistance, 1 << 9))
        {
            if (up + transform.localScale.y / 2 < m_Hit.distance && m_Hit.distance < 0.1f)
            {
                if (m_Hit.transform.gameObject.layer == LayerMask.NameToLayer("ThroughPlayer")) m_Hit.transform.gameObject.layer = LayerMask.NameToLayer("Player");
            }
            //else if (m_Hit.transform.gameObject.layer == LayerMask.NameToLayer("Player")) m_Hit.transform.gameObject.layer = LayerMask.NameToLayer("ThroughPlayer");
        }

        //敵とプレイヤーを区別するため、タグによって処理を分けレイヤーもthroughplayerとthroughenemyとかに分けるべき
    }
}
