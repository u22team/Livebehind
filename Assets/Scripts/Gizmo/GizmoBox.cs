using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoBox : MonoBehaviour
{
    [SerializeField] bool northBoxesGizmo = false;
    [SerializeField] bool southBoxesGizmo = false;

    void OnDrawGizmos()
    {
        if (northBoxesGizmo)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (float ii = 0; ii < 10; ii += 0.1f)
            {
                for (float i = 0; i < 1.2f; i += 0.1f)
                {
                    Gizmos.DrawWireCube(new Vector3(i - 0.55f, 0.05f + ii - 0.15f, 0.1f), new Vector3(0.1f, 0.1f, 0));
                    //Gizmos.DrawWireCube(new Vector3(i - 0.55f, ii - 0.15f, 0.15f), new Vector3(0.1f, 0.1f, 0.1f));
                }
            }
        }

        if (southBoxesGizmo)
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            for (float ii = 0; ii < 10; ii += 0.1f)
            {
                for (float i = 0; i < 1.2f; i += 0.1f)
                {
                    //Gizmos.DrawWireCube(new Vector3(i - 0.55f, ii - 0.15f, 0), new Vector3(0.1f, 0.1f, 0));
                    Gizmos.DrawWireCube(new Vector3(i - 0.55f, 0.05f + ii - 0.15f, 0.05f), new Vector3(0.1f, 0.1f, 0.1f));
                }
            }
        }
    }
}
