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
        public int colliderHalfScaleInt;
        public int distanceInt;
        public int layerMask;
    }

    private Dictionary<Vector3Int, touchGroup> touchDic;
    private Rigidbody playCharRB;
    private Vector3Int moveDirection; //vector2の方がいい気がする
    int nextTurnDirection; //右折が1、左折が-1 名前変えたほうがいいかも
    int canJumpNum;
    bool isJump;
    bool isMoving;
    public Vector3Int transformPosInt, velocityUpwards;
    Vector3Int Vector3IntForward = new Vector3Int(0, 0, 1), Vector3IntBack = new Vector3Int(0, 0, -1);
    [SerializeField]
    GameObject playCharModel;

    [SerializeField] AudioClip seJump;
    AudioSource audioSource;

    [SerializeField] Camera cam;

    private void Start()
    {
        touchDic = new Dictionary<Vector3Int, touchGroup>
        {
            { Vector3Int.down, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0.0495f,0,0.0495f), hitDistance=0.1001f, colliderHalfScaleInt=128, layerMask=~(1<<LayerMask.NameToLayer("Damager")) } },
            { Vector3Int.up, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0.0495f,0,0.0495f), hitDistance=0.1001f, colliderHalfScaleInt=128, layerMask=~(1<<LayerMask.NameToLayer("Damager") | 1<<LayerMask.NameToLayer("ThroughFloor")) } },
            { Vector3Int.right, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0,0.0995f,0.0495f), hitDistance=0.0501f, colliderHalfScaleInt=64, layerMask=~(1<<LayerMask.NameToLayer("Damager") | 1<<LayerMask.NameToLayer("ThroughFloor")) } },
            { Vector3Int.left, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0,0.0995f,0.0495f), hitDistance=0.0501f, colliderHalfScaleInt=64, layerMask=~(1<<LayerMask.NameToLayer("Damager") | 1<<LayerMask.NameToLayer("ThroughFloor")) } },
            { Vector3IntForward, new touchGroup { touch=false, collision=false, colliderHalfScale=new Vector3(0.0495f,0.0995f,0), hitDistance=0.0501f, colliderHalfScaleInt=64, layerMask=~(1<<LayerMask.NameToLayer("Damager") | 1<<LayerMask.NameToLayer("ThroughFloor")) } },
            { Vector3IntBack, new touchGroup { touch=true, collision=false, colliderHalfScale=new Vector3(0.0495f,0.0995f,0), hitDistance=0.0501f, colliderHalfScaleInt=64, layerMask=~(1<<LayerMask.NameToLayer("Damager") | 1<<LayerMask.NameToLayer("ThroughFloor")) } }
        };

        playCharRB = this.GetComponent<Rigidbody>();

        moveDirection = Vector3Int.right;
        nextTurnDirection = 1;
        transformPosInt = new Vector3Int(0, 192, 64);
        velocityUpwards = Vector3Int.zero;
        isJump = false;
        canJumpNum = 0;
        isMoving = true; //falseにしとけ

        audioSource = cam.gameObject.GetComponent<AudioSource>();
    }

    private void FixedUpdate() //ジャンプや移動等の動作を司る部分と、オートランを司る部分を分離すること
    {
        Vector3 thisPosition = this.transform.position;

        //キャラクターの6方向の当たり判定を取る

        foreach (KeyValuePair<Vector3Int, touchGroup> pair in touchDic)
        {
            if (pair.Key==Vector3Int.down)
            {
                RaycastHit _rayCastHit, _rayCastHit2;
                if (Physics.BoxCast(thisPosition, touchDic[pair.Key].colliderHalfScale, pair.Key, out _rayCastHit, Quaternion.identity, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Damager") | 1 << LayerMask.NameToLayer("Ignore Raycast")))
                    && Physics.BoxCast(thisPosition, touchDic[pair.Key].colliderHalfScale, pair.Key, out _rayCastHit2, Quaternion.identity, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Damager") | 1 << LayerMask.NameToLayer("ThroughFloor") | 1 << LayerMask.NameToLayer("Ignore Raycast"))))
                {
                    touchDic[pair.Key].rayCastHit.distance = (_rayCastHit.distance >= 0.0999f && _rayCastHit.distance < _rayCastHit2.distance && velocityUpwards.y <= 0) ? _rayCastHit.distance : _rayCastHit2.distance;
                }
                else
                {
                    touchDic[pair.Key].rayCastHit.distance = Mathf.Infinity; //ヒットしなかった場合に代入する最大値
                }
            }
            else
            {
                if (!Physics.BoxCast(thisPosition, touchDic[pair.Key].colliderHalfScale, pair.Key, out touchDic[pair.Key].rayCastHit, Quaternion.identity, Mathf.Infinity, ~(1 << LayerMask.NameToLayer("Damager") | 1 << LayerMask.NameToLayer("ThroughFloor") | 1 << LayerMask.NameToLayer("Ignore Raycast"))))
                {
                    touchDic[pair.Key].rayCastHit.distance = Mathf.Infinity; //ヒットしなかった場合に代入する最大値
                }
            }

            if (touchDic[pair.Key].hitDistance - 0.0002f <= touchDic[pair.Key].rayCastHit.distance && touchDic[pair.Key].rayCastHit.distance <= touchDic[pair.Key].hitDistance)
            {
                if (touchDic[pair.Key].touch == false) touchDic[pair.Key].collision = true;
                touchDic[pair.Key].touch = true;
            }
            else
            {
                touchDic[pair.Key].touch = false;
                touchDic[pair.Key].collision = false;
            }

            touchDic[pair.Key].distanceInt = Mathf.RoundToInt(touchDic[pair.Key].rayCastHit.distance * 1280) - touchDic[pair.Key].colliderHalfScaleInt;
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
                    if (moveDirection == Vector3Int.right || moveDirection == Vector3Int.left) moveDirection = reverseDirection; //正しくないな
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
            velocityUpwards = Vector3Int.zero;
        }

        //重力
        if ((touchDic[Vector3Int.right].touch || touchDic[Vector3Int.left].touch) && !touchDic[Vector3Int.down].touch)
        {
            canJumpNum = 2;
            velocityUpwards = ((Vector3)velocityUpwards * 3 / 4).RoundToInt() + Vector3Int.down * 4;
            isMoving = false;

            //暫定的にここに置かせてもらう 壁ズサ中の進行方向は優先度が高い
            if ((touchDic[Vector3Int.right].touch && !touchDic[Vector3Int.left].touch)) moveDirection = Vector3Int.left;
            else if ((!touchDic[Vector3Int.right].touch && touchDic[Vector3Int.left].touch)) moveDirection = Vector3Int.right;
        }
        else
        {
            velocityUpwards += Vector3Int.down * 4;
        }

        //ジャンプ
        if (isJump && !touchDic[Vector3Int.up].touch && !touchDic[moveDirection].touch)
        {
            audioSource.PlayOneShot(seJump);
            if (touchDic[Vector3Int.down].touch) //地面からのジャンプ
            {
                velocityUpwards = Vector3Int.up * 68;
                canJumpNum = 1;
            }
            else if (touchDic[reverseDirection].touch) //壁ジャン
            {
                velocityUpwards = Vector3Int.up * 68;
                transformPosInt += moveDirection * 20;
                canJumpNum = 1;
            }
            else if (canJumpNum > 0 && !touchDic[Vector3Int.down].touch) //空中ジャンプ
            {
                velocityUpwards = Vector3Int.up * 68;
                canJumpNum--;
            }
            isJump = false;
            isMoving = true;
        }

        //上下方向のめり込み防止
        if (-touchDic[Vector3Int.down].distanceInt > velocityUpwards.y)
        {
            transformPosInt += Vector3Int.down * touchDic[Vector3Int.down].distanceInt;
        }
        else if (velocityUpwards.y > touchDic[Vector3Int.up].distanceInt)
        {
            transformPosInt += Vector3Int.up * touchDic[Vector3Int.up].distanceInt;
        }
        else
        {
            transformPosInt += velocityUpwards;
        }

        //水平移動 めり込み防止
        if (isMoving && touchDic[moveDirection].touch == false && (touchDic[Vector3Int.down].touch || (!touchDic[Vector3Int.down].touch && !touchDic[Vector3Int.right].touch && !touchDic[Vector3Int.left].touch)))
        {
            if (20 < touchDic[moveDirection].distanceInt)
            {
                transformPosInt += moveDirection * 20;
            }
            else
            {
                transformPosInt += moveDirection * touchDic[moveDirection].distanceInt;
            }
        }

        playCharRB.MovePosition((Vector3)transformPosInt / 1280); //スローを導入するなら1280では小さい

        playCharModel.transform.rotation = Quaternion.Euler(0, 90 * (moveDirection.z > 0 ? -1 : 1) * (1 - moveDirection.x), 0);
        playCharModel.transform.localScale = new Vector3((moveDirection.x == 0 ? 0.5f : 1), 1, (moveDirection.z == 0 ? 0.5f : 1));

        //斜め進入時にめり込む問題を解決しろ 進行方向にレイを飛ばせ
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!touchDic[Vector3Int.up].touch && moveDirection != Vector3IntForward && moveDirection != Vector3IntBack) //やっぱり前後移動中のジャンプは１回分バッファしたほうがいいのかな ここの条件を外せばバッファになる
            {
                isJump = true;
            }
        }

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if (touch.phase == TouchPhase.Began)
            {
                if (!touchDic[Vector3Int.up].touch && moveDirection != Vector3IntForward && moveDirection != Vector3IntBack) //やっぱり前後移動中のジャンプは１回分バッファしたほうがいいのかな ここの条件を外せばバッファになる
                {
                    isJump = true;
                }
            }
        }
    }

    public void ResetPlayerPosition()
    {
        transformPosInt = new Vector3Int(0, 192, 64);
        velocityUpwards = Vector3Int.zero;
    }
    /*
    [SerializeField]
    bool isEnable = false;

    private void OnDrawGizmos()
    {
        if (isEnable)
        {
            foreach (KeyValuePair<Vector3Int, touchGroup> pair in touchDic)
            {
                Gizmos.DrawRay(transform.position, (Vector3)pair.Key * touchDic[pair.Key].rayCastHit.distance); //厚さ0で0.075以下
                Gizmos.DrawWireCube(transform.position + (Vector3)pair.Key * touchDic[pair.Key].rayCastHit.distance, touchDic[pair.Key].colliderHalfScale * 2);
            }
        }
    }
    */
}