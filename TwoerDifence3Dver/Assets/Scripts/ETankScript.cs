using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETankScript : MonoBehaviour
{
    private float speed = 1f;                   // 移動速度
    public float rotationSpeed = 200f;         // 回転速度
    public float[] segmentDistances = { 2f, 200f, 2f }; // 各セグメントの移動距離
    private int currentSegment = 0;            // 現在のセグメントインデックス
    private Vector3 startPosition;             // 各セグメントの開始位置
    private bool isTurning = false;            // 回転中フラグ
    public bool isMove = true;                // Enemyの移動を再開するメソッドのフラグ
    private bool isPaused = false;             // 移動を一時停止するフラグ
    public Animator animator;                  // Animatorコンポーネント
    public int enemyTankHp = 100;         // プレイヤータンクのHP
    public int enemyTackPower = 10;       // プレイヤータンクの攻撃力
    private float damageInterval = 1.0f;       // **ダメージを受ける間隔（秒）**
    public SoundManager soundManager;
    public GameManager gameManager;

    public GameObject punchEffect;

    private Coroutine damageCoroutine; // ダメージ処理のコルーチンを保持

    public PAttackerScript pAttackerScript;
    public PTankScript pTankScript;

    void Start()
    {
        //子オブジェクトの名前を見つける
        punchEffect = transform.Find("PunchEffect (1)").gameObject;
        punchEffect.SetActive(false);

        gameManager = FindAnyObjectByType<GameManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        animator = GetComponent<Animator>(); // Animatorのコンポーネントを取得
        startPosition = transform.position;  // 初期位置を設定
    }

    void Update()
    {
        //Debug.Log(isPaused);

        if (isPaused) return; // 一時停止中なら処理しない

        if (!isTurning && currentSegment < segmentDistances.Length)
        {
            animator.SetBool("isWalk", true);    //移動しているとき歩行アニメーションを実行
            animator.SetBool("isAttack", false); //移動しているとき攻撃アニメーションを無効
            animator.SetBool("isIdel", false);   //移動しているとき待機アニメーションを無効

            float movedDistance = Vector3.Distance(startPosition, transform.position);
            if (movedDistance < segmentDistances[currentSegment] - 0.01f)  // 誤差補正
            {
                // 指定距離に達するまで移動
                float remainingDistance = segmentDistances[currentSegment] - movedDistance;
                float moveStep = Mathf.Min(speed * Time.deltaTime, remainingDistance); // 必要以上に進まないよう補正
                transform.position += transform.forward * moveStep;
            }
            else
            {
                // 次のセグメントがある場合のみ回転
                if (currentSegment < segmentDistances.Length - 1)
                {
                    StartCoroutine(Turn90Degrees());
                }
                else
                {
                    // 最後のセグメントが終わったらスクリプトを無効化して終了
                    enabled = false;
                }
            }
        }
    }

    private IEnumerator Turn90Degrees()
    {
        isTurning = true;

        float targetAngle = transform.eulerAngles.y + 90f; // 目標角度
        float currentAngle = transform.eulerAngles.y;

        while (Mathf.Abs(Mathf.DeltaAngle(currentAngle, targetAngle)) > 0.1f)
        {
            float step = rotationSpeed * Time.deltaTime;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }

        // 正確な90度回転
        transform.rotation = Quaternion.Euler(0, targetAngle, 0);

        // 新しいセグメントの開始位置を更新
        startPosition = transform.position;

        // 次の移動フェーズ
        currentSegment++;

        isTurning = false;
    }

    void ETTakeDamage(int damage)
    {

        enemyTankHp -= damage;
        //Debug.Log(playerAttackerHp);

        if (enemyTankHp <= 0)
        {
            Die();
            isMove = false;
        }
    }

    public void Die()
    {
        gameManager.currentEnemyUnits = Mathf.Max(0, gameManager.currentEnemyUnits - 1); // 0未満にならないようにする
        Debug.Log("敵ユニットが倒された！現在の数: " + gameManager.currentEnemyUnits);

        transform.Translate(-500f, 0f, 0f);
        Destroy(gameObject, 1f);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("APlayer"))
        {
            punchEffect.SetActive(true);

            soundManager.PunchPlay();

            pAttackerScript = other.GetComponent<PAttackerScript>();

            isPaused = true;  // 移動を一時停止
            //Debug.Log("OnTriggerEnter内: isPaused = " + isPaused);
            animator.SetBool("isWalk", false);  //移動アニメーションを無効
            animator.SetBool("isAttack", true); //攻撃アニメーションを実行
            animator.SetBool("isIdel", false);  //待機アニメーションを無効

            // すでにダメージ処理が動いていなければ開始
            if (damageCoroutine == null)
            {
                //Debug.Log("DamageOverTimeを処理");
                damageCoroutine = StartCoroutine(ADamageOverTime());
            }
        }

        if (other.CompareTag("TPlayer"))
        {
            punchEffect.SetActive(true);

            soundManager.PunchPlay();

            pTankScript = other.GetComponent<PTankScript>();

            isPaused = true;  // 移動を一時停止
            //Debug.Log("OnTriggerEnter内: isPaused = " + isPaused);
            animator.SetBool("isWalk", false);  //移動アニメーションを無効
            animator.SetBool("isAttack", true); //攻撃アニメーションを実行
            animator.SetBool("isIdel", false);  //待機アニメーションを無効

            // すでにダメージ処理が動いていなければ開始
            if (damageCoroutine == null)
            {
                //Debug.Log("DamageOverTimeを処理");
                damageCoroutine = StartCoroutine(TDamageOverTime());
            }
        }

        if (other.CompareTag("AEnemy"))
        {

            if (transform.position.x < other.transform.position.x)
            {
                return;
            }
            isPaused = true;                       //移動停止  
            animator.SetBool("isWalk", false);     //移動しているとき歩行アニメーションを無効
            animator.SetBool("isAttack", false);   //移動しているとき歩行アニメーションを無効
            animator.SetBool("isIdel", true);      //移動しているとき待機アニメーションを実行

        }

        if (other.CompareTag("TEnemy"))
        {
            if (transform.position.x < other.transform.position.x)
            {
                return;
            }
            isPaused = true;                       //移動停止  
            animator.SetBool("isWalk", false);     //移動しているとき歩行アニメーションを無効
            animator.SetBool("isAttack", false);   //移動しているとき歩行アニメーションを無効
            animator.SetBool("isIdel", true);      //移動しているとき待機アニメーションを実行

        }

        if (other.CompareTag("PlayerTower"))
        {
 
            StartCoroutine(Wait());
            isPaused = true;                      //移動を停止
            animator.SetBool("isWalk", false);    //移動しているとき歩行アニメーションを無効
            animator.SetBool("isAttack", true);  //移動しているとき攻撃アニメーションを無効
            animator.SetBool("isIdel", false);     //移動しているとき待機アニメーションを実行
        }
    }

    // 一定時間ごとにダメージを受ける Coroutine
    private IEnumerator ADamageOverTime()
    {
        while (enemyTankHp > 0) // HPが0になるまでループ
        {
            //Debug.Log("while");
            ETTakeDamage(pAttackerScript.playerAttackerPower);
            yield return new WaitForSeconds(damageInterval); // 指定時間待つ
        }

        // HPが0になったら敵の移動を再開
        //Debug.Log("HP0後");
        pAttackerScript.PAResumeMovement();
        damageCoroutine = null; // コルーチンの参照をリセット
    }

    private IEnumerator TDamageOverTime()
    {
        while (enemyTankHp > 0) // HPが0になるまでループ
        {
            //Debug.Log("while");
            ETTakeDamage(pTankScript.playerTackPower);
            yield return new WaitForSeconds(damageInterval); // 指定時間待つ
        }

        // HPが0になったら敵の移動を再開
        Debug.Log("HP0後");
        pTankScript.PTResumeMovement();
        damageCoroutine = null; // コルーチンの参照をリセット
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("APlayer"))  //Attacker
        {
            punchEffect.SetActive(false);

            soundManager.PunchStop();

            // ダメージを受ける Coroutine を停止
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            ETResumeMovement(); // 通常の移動を再開
        }

        if (other.CompareTag("TPlayer"))   //Tank
        {
            punchEffect.SetActive(false);

            soundManager.PunchStop();

            // ダメージを受ける Coroutine を停止
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }

            ETResumeMovement();
        }

        if (other.CompareTag("AEnemy"))
        {
            ETResumeMovement();
        }

        if (other.CompareTag("TEnemy"))
        {
            ETResumeMovement();
        }
    }

    public void ETResumeMovement()
    {
        //Debug.Log("playerの移動再開");
        isPaused = false; // 移動を再開
        animator.SetBool("isWalk", true);    //移動しているとき歩行アニメーションを実行
        animator.SetBool("isAttack", false); //移動しているとき攻撃アニメーションを無効
        animator.SetBool("isIdel", false);   //移動しているとき待機アニメーションを無効
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.5f);
    }

    //キャラクター生成時のステータスをランダムで決める
    public void Initialize(int minHP, int maxHP, int minAttack, int maxAttack)
    {
        enemyTankHp = Random.Range(minHP, maxHP + 1);
        enemyTackPower = Random.Range(minAttack, maxAttack + 1);
    }

    public void ShowStats()
    {
        //Debug.Log(gameObject.name + " HP: " + enemyTankHp + ", Attack: " + enemyTackPower);
    }
}
