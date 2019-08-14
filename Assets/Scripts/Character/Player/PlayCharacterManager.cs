using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCharacterManager : MonoBehaviour
{
    private class touchGroup
    {
        public bool touch;
        public bool collision;
        public Vector3 colliderHalfScale; //ホントはこれget専用にしなきゃだめ
        public float hitDistance;
        public RaycastHit rayCastHit;
    }

    private static Dictionary<Vector3Int, touchGroup> touchDic;
    private Rigidbody playCharRB;
    Quaternion thisRotation = Quaternion.identity;
    private Vector3Int moveDirection; //vector2の方がいい気がする
    int nextTurnDirection; //右折が1、左折が-1 名前変えたほうがいいかも
    int canJumpNum;
    bool isJump;
    bool isMoving;
    Vector3Int transformPosInt, velocityUpwards;
    Vector3Int Vector3IntForward = new Vector3Int(0, 0, 1), Vector3IntBack = new Vector3Int(0, 0, -1);

    private void Start()
    {
        touchDic = new Dictionary<Vector3Int, touchGroup>
        {
            { Vector3Int.down, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0.0495f,0,0.0495f), hitDistance=0.1001f } }, //こいつ以外すり抜け床を判定しない
            { Vector3Int.up, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0.0495f,0,0.0495f), hitDistance=0.1001f } },
            { Vector3Int.right, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0,0.0995f,0.0495f), hitDistance=0.0501f } },
            { Vector3Int.left, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0,0.0995f,0.0495f), hitDistance=0.0501f } },
            { Vector3IntForward, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0.0495f,0.0995f,0), hitDistance=0.0501f } },
            { Vector3IntBack, new touchGroup { touch=true, collision=false, colliderHalfScale=new Vector3(0.0495f,0.0995f,0), hitDistance=0.0501f } }
        };

        playCharRB = this.GetComponent<Rigidbody>();

        moveDirection = Vector3Int.right;
        nextTurnDirection = 1;
        transformPosInt = new Vector3Int(0, 192, 64);
        velocityUpwards = Vector3Int.zero;
        isJump = false;
        canJumpNum = 0;
        isMoving = true; //falseにしとけ
    }

    private void FixedUpdate()
    {
        Vector3 thisPosition = this.transform.position;

        //キャラクターの6方向の当たり判定を取る

        foreach (KeyValuePair<Vector3Int, touchGroup> pair in touchDic)
        {
            if (!Physics.BoxCast(thisPosition, touchDic[pair.Key].colliderHalfScale, pair.Key, out touchDic[pair.Key].rayCastHit, thisRotation))
            {
                touchDic[pair.Key].rayCastHit.distance = 2147483647; //ヒットしなかった場合に代入する最大値
            }

            if (touchDic[pair.Key].rayCastHit.distance <= touchDic[pair.Key].hitDistance)
            {
                if (touchDic[pair.Key].touch == false) touchDic[pair.Key].collision = true;
                touchDic[pair.Key].touch = true;
            }
            else
            {
                touchDic[pair.Key].touch = false;
                touchDic[pair.Key].collision = false;
            }
        }

        /* moveDirectionの決定 */

        Vector3Int rightDirection = moveDirection.Turn(1);
        Vector3Int leftDirection = moveDirection.Turn(-1);
        Vector3Int reverseDirection = moveDirection.Turn(1).Turn(1); //なんか-1*できない

        if (touchDic[moveDirection].collision) //進行方向の壁に衝突した時
        {
            switch ((touchDic[Vector3Int.down].touch ? 4 : 0)
                + (touchDic[rightDirection].touch ? 2 : 0)
                + (touchDic[leftDirection].touch ? 1 : 0))
            {
                case 0: //左壁0、右壁0、接地0
                case 1: //左壁1、右壁0、接地0
                case 2: //左壁0、右壁2、接地0
                case 3:
                    if (moveDirection == Vector3Int.right || moveDirection == Vector3Int.left) moveDirection = reverseDirection;
                    break; //左壁1、右壁2、接地0
                case 4: moveDirection = moveDirection.Turn(nextTurnDirection); nextTurnDirection *= -1; break; //左壁0、右壁0、接地4
                case 5: moveDirection = rightDirection; nextTurnDirection = -1; break; //左壁1、右壁0、接地4
                case 6: moveDirection = leftDirection; nextTurnDirection = 1; break; //左壁0、右壁2、接地4
                case 7: moveDirection *= -1; break; //左壁1、右壁2、接地4
                default: break;
            }
        }
        else if (touchDic[Vector3Int.down].collision) //着地した時
        {
            switch ((touchDic[moveDirection].touch ? 8 : 0)
                + (touchDic[reverseDirection].touch ? 4 : 0)
                + (touchDic[rightDirection].touch ? 2 : 0)
                + (touchDic[leftDirection].touch ? 1 : 0))
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7: break;
                case 8: break; //想定しない
                case 9:
                case 10:
                case 11: moveDirection = reverseDirection; break;
                case 12: break; //想定しない
                case 13: moveDirection = rightDirection; break;
                case 14: moveDirection = leftDirection; break;
                case 15: break; //想定しない
                default: break;
            }
        } //落下中に挟まれて出た時は？

        /* 実際に移動させる 優先順位の高いものをあとに */

        //接地及び頭ごっつんこ
        if (touchDic[Vector3Int.down].touch)
        {
            canJumpNum = 2;
            velocityUpwards = Vector3Int.zero;
            isMoving = true;
        }
        else if (touchDic[Vector3Int.up].collision)
        {
            velocityUpwards *= -1;
        }

        int downwardDistanceInt = Mathf.RoundToInt(touchDic[Vector3Int.down].rayCastHit.distance * 1280) - 128;
        int upwardDistanceInt = Mathf.RoundToInt(touchDic[Vector3Int.up].rayCastHit.distance * 1280) - 128;

        //重力
        if ((touchDic[Vector3Int.right].touch || touchDic[Vector3Int.left].touch) && !touchDic[Vector3Int.down].touch)
        {
            velocityUpwards = ((Vector3)velocityUpwards * 3 / 4).RoundToInt() + Vector3Int.down * 4;
            isMoving = false;
        }
        else
        {
            velocityUpwards += Vector3Int.down * 4;
        }

        //ジャンプ
        if (isJump && !touchDic[Vector3Int.up].touch && !touchDic[moveDirection].touch)
        {
            Debug.Log("ここ来てる？");
            if (touchDic[Vector3Int.down].touch) //地面からのジャンプ
            {
                velocityUpwards = Vector3Int.up * 68;
                canJumpNum = 1;
            }
            else if (touchDic[reverseDirection].touch) //壁ジャン
            {
                velocityUpwards = Vector3Int.up * 68;
                transformPosInt += moveDirection * 16;
                canJumpNum = 1;
            }else if (canJumpNum>0&& !touchDic[Vector3Int.down].touch) //空中ジャンプ
            {
                velocityUpwards = Vector3Int.up * 68;
                canJumpNum--;
            }
            isJump = false;
            isMoving = true;
        }

        //上下方向のめり込み防止
        if (-downwardDistanceInt > velocityUpwards.y)
        {
            transformPosInt += Vector3Int.down * downwardDistanceInt;
        }
        else if (velocityUpwards.y > upwardDistanceInt)
        {
            transformPosInt += Vector3Int.up * upwardDistanceInt;
        }
        else
        {
            transformPosInt += velocityUpwards;
        }

        //横移動
        if (isMoving && touchDic[moveDirection].touch == false && (touchDic[Vector3Int.down].touch || (!touchDic[Vector3Int.down].touch && !touchDic[Vector3Int.right].touch && !touchDic[Vector3Int.left].touch)))
        {
            transformPosInt += moveDirection * 16; //壁にめり込む可能性があるので要注意
        }

        playCharRB.MovePosition((Vector3)transformPosInt / 1280); //スローを導入するなら1280では小さい

        Debug.Log(transformPosInt);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!touchDic[Vector3Int.up].touch && moveDirection != Vector3IntForward && moveDirection != Vector3IntBack) //前後移動中のジャンプはバッファしたほうがいいのかな ここの条件を外せばバッファになる
            {
                isJump = true;
            }
        }
    }

    /*
    [SerializeField]
    bool isEnable = false;
    void OnDrawGizmos()
    {
        if (isEnable == false)
            return;

        foreach (KeyValuePair<Vector3Int, touchGroup> pair in touchDic)
        {
            Gizmos.DrawRay(transform.position, (Vector3)pair.Key * touchDic[pair.Key].rayCastHit.distance); //厚さ0で0.075以下
            Gizmos.DrawWireCube(transform.position + (Vector3)pair.Key * touchDic[pair.Key].rayCastHit.distance, touchDic[pair.Key].colliderHalfScale * 2);
        }
    }
    */

    /*
    Vector3 Parallelized(Vector3 rbv, Vector3Int md)
    {
        return new Vector3(rbv.x * md.x.Abs(), rbv.y * md.y.Abs(), rbv.z * md.z.Abs());
    }

    Vector3Int ParallelizedInt(Vector3Int rbv, Vector3Int md)
    {
        return new Vector3Int(rbv.x * md.x.Abs(), rbv.y * md.y.Abs(), rbv.z * md.z.Abs());
    }
    */
}