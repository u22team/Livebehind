using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapCreate : MonoBehaviour
{
    // 定数 //
    readonly Vector3 DownCenter = new Vector3(0, 0, 0); // Mapの最下の真ん中の座標
    readonly float BlockSize = 0.1f; // 1ブロックの大きさ
    [SerializeField]
    Transform BlockParent;
    //////////

    void Start()
    {
        int stage_num = StageSelect.stage_num;
        int[,] front = StageSelect.Map_Front[stage_num], back = StageSelect.Map_Front[stage_num];
        GameObject[] blocks = StageSelect.Block_;

        for (int y = 0; y < front.GetLength(0); y++)
        {
            for (int x = 0; x < front.GetLength(1); x++)
            {
                Vector3 pos = DownCenter + new Vector3(x - front.GetLength(1), y, 0.5f) * BlockSize;
                GameObject block = blocks[front[front.GetLength(0) - y - 1,x]];
                if (block != null)
                    Instantiate(block, pos, Quaternion.identity, BlockParent);
            }
        }
        for (int y = 0; y < back.GetLength(0); y++)
        {
            for (int x = 0; x < back.GetLength(1); x++)
            {
                Vector3 pos = DownCenter + new Vector3(x - back.GetLength(1), y, -0.5f) * BlockSize;
                GameObject block = blocks[back[back.GetLength(0) - y - 1, x]];
                if (block != null)
                    Instantiate(block, pos, Quaternion.identity, BlockParent);
            }
        }
    }

}
