using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BoxcastTest : MonoBehaviour
{
    RaycastHit hit, hitB;

    [SerializeField]
    bool isEnable = false;

    //[SerializeField]
    Vector3 scale; //0.099,0,0.099
    [SerializeField]
    Vector3 scaleB;

    [SerializeField]
    Text text;

    void OnDrawGizmos()
    {
        if (isEnable == false)
            return;

        scale = new Vector3(1f / 16, 0, 1f / 16);

        var isHit = Physics.BoxCast(transform.position, scale / 2, Vector3.down, out hit, transform.rotation);
        if (isHit)
        {
            Gizmos.DrawRay(transform.position, Vector3.down * hit.distance); //厚さ0で0.075以下
            Gizmos.DrawWireCube(transform.position + Vector3.down * hit.distance, scale);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector3.down * 100);
            text.text = "貫通";
        }

        var isHitb = Physics.BoxCast(transform.position, scaleB / 2, Vector3.right, out hitB, Quaternion.identity);
        if (isHitb)
        {
            Gizmos.DrawRay(transform.position, Vector3.right * hitB.distance); //厚さ0で
            Gizmos.DrawWireCube(transform.position + Vector3.right * hitB.distance, scaleB);
            text.text = hitB.distance.ToString();
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector3.right * 100);
        }
    }
}