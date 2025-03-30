using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWizard : MonoBehaviour
{
    public Animator animator;  //Wizardのアクションに応じてAnimatorを変更する
    private int totalDestroyedEnemies = 0; // 削除した敵の数をカウントする変数

    public GameObject magicCircle; //魔法発動時のeffect

    public GameObject prefabToSpawn; // スポーンするプレハブ
    public Transform spawnPoint; // スポーン開始位置（X軸は変化する）
    private GameObject[] spawnedObjects = new GameObject[10]; // 生成した10個のオブジェクトを格納する配列

    public GameObject rock;             //プレハブ石
    public Transform rockSpawnPoint; //石の開始位置
    public GameObject circleEffect;     //タワー攻撃時魔法人
    public bool isMagicB = false;  //隕石魔法を使ったフラグ

    public GameObject barrier;  //プレハブバリア
    public Transform barrierSpawnPoint; //バリアの開始位置
    public bool isBarrier = false;  //バリアのフラグ

    //AudioSource
    public AudioSource magic1;
    public AudioSource magic2;
    public AudioSource magic3;

    [SerializeField] GameManager gameManager;

    private void Start()
    {
        animator = GetComponent<Animator>();
        magicCircle.SetActive(false);
        circleEffect.SetActive(false);
    }

    private void Update()
    {
        
    }

    public IEnumerator BrokenEffect()
    {
        yield return new WaitForSeconds(1f);
    }




    public void Attack() // 敵に攻撃するメソッド
    {
        animator.SetBool("AttackMagic", true);
        magicCircle.SetActive(true);
        StartCoroutine(SpawnPrefabs()); // プレハブをスポーンするコルーチンを開始

        // AEnemy と TEnemy をそれぞれ処理
        StartCoroutine(DestroyEnemies("AEnemy"));
        StartCoroutine(DestroyEnemies("TEnemy"));
    }

    // 指定されたタグの敵を削除する
    private IEnumerator DestroyEnemies(string enemyTag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag); // 指定タグの敵を取得

        foreach (GameObject enemy in enemies)
        {
            // 自分自身を除外
            if (enemy != gameObject)
            {
                //ここにTranslateで少しずらす。（万が一、Playerが接触していた場合のバグを起こさないため）
                enemy.transform.Translate(500f, 0f, 0f);
                Destroy(enemy, 1f); // 敵を削除
                totalDestroyedEnemies++; // 削除した敵の数をカウント
            }

            yield return new WaitForSeconds(0.5f); // 少し待機（複数の敵を削除する場合の間隔）
        }
        StartCoroutine(AnimResetA()); //アニメーターなどの変化
        gameManager.currentEnemyUnits = Mathf.Max(0, gameManager.currentEnemyUnits - totalDestroyedEnemies); // 0未満にならないようにする
        Debug.Log("敵ユニットが倒された！現在の数: " + gameManager.currentEnemyUnits);
        //Debug.Log("Total destroyed enemies: " + totalDestroyedEnemies); // 敵を削除した後にトータルの数をログに出力    
    }

    // プレハブをスポーンするコルーチン
    IEnumerator SpawnPrefabs()
    {
        magic2.Play(); //雷の音再生
        float spawnInterval = 0.1f; // 各プレハブのスポーン間隔
        Vector3 spawnPosition = spawnPoint.position; // 初期のスポーン位置

        // 10個のプレハブを1つずつスポーン
        for (int i = 0; i < 10; i++)
        {
            // プレハブをスポーンし、配列に格納
            spawnedObjects[i] = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            // スポーン位置をX軸で1ユニット進める
            spawnPosition.x += 6f;

            // 次のスポーンまで0.1秒待機
            yield return new WaitForSeconds(spawnInterval);
        }

        magic2.Stop(); //雷の音再生
        // 10個目のプレハブが生成されたら、すべてのプレハブを破壊
        DestroyPrefabs();
    }

    // 生成したプレハブをすべて破壊するメソッド
    void DestroyPrefabs()
    {
        for (int i = 0; i < 10; i++)
        {
            Destroy(spawnedObjects[i]);
        }

        
        //Debug.Log("10個のプレハブを破壊しました。");
    }






    public void DirectAttack() //タワーに攻撃するメソッド
    {
        animator.SetBool("DirectAttack", true);
        magicCircle.SetActive(true);

        Fier(); //隕石出現
        StartCoroutine(FierReset());
        StartCoroutine(ResetPosition());
    }

    public void Fier()
    {
        gameObject.transform.Rotate(0f, -90f, 0f);
        circleEffect.SetActive(true);
        isMagicB = true;
        magic1.Play();
        Vector3 spawnPosition = rockSpawnPoint.position; //初期スポーン位置
        Instantiate(rock, spawnPosition, Quaternion.Euler(0f, -180f, 0f));
    }

    public IEnumerator FierReset()
    {
        yield return new WaitForSeconds(5.5f);
        StartCoroutine(AnimResetB());
    }

    public IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(5.5f);
        gameObject.transform.Rotate(0f, 90f, 0f);
        circleEffect.SetActive(false);
    }





    public void Barrier() //バリアを展開するメソッド
    {
        isBarrier = true;

        animator.SetBool("BarrierMagic", true);
        magicCircle.SetActive(true);

        BarrierSpawn(); //インスタンス化して生成

        StartCoroutine(AnimResetC());

    }

    public void BarrierSpawn()
    {
        magic3.Play();
        Vector3 barrierPosition = barrierSpawnPoint.position;
        GameObject barrierInstance = Instantiate(barrier, barrierPosition, Quaternion.identity);
    }


    public IEnumerator AnimResetA()
    {
        yield return new WaitForSeconds(1f);
        magicCircle.SetActive(false);
        animator.SetBool("AttackMagic", false);
        animator.SetBool("isIdel", true);
    }
    public IEnumerator AnimResetB()
    {
        yield return new WaitForSeconds(1f);
        magicCircle.SetActive(false);
        animator.SetBool("DirectAttack", false);
        animator.SetBool("isIdel", true);
    }
    public IEnumerator AnimResetC()
    {
        yield return new WaitForSeconds(1f);
        magicCircle.SetActive(false);
        animator.SetBool("BarrierMagic", false);
        animator.SetBool("isIdel", true);
    }
}
