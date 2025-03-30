using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //シングルトン

    public GameObject menus;          //GameClearまたGameOver時のメニュー
    public float delayTime = 3.0f;  　//カウントダウン
    public Text countdownText;     　 //UIテキスト
    public GameObject notPushButton; 　//ボタンを押せないようにする
    public Animator gameOverAnim;  　 //GameOver時のアニメーター
    public Animator gameClearAnim;    //GameClear時のアニメーター

    public bool notPlayerSpawn = false; //キャラクターをスポーンさせる時間のフラグ
    public bool isEnemySpawn = false;   //敵がカウントダウンが終わってから生成するためのフラグ
    public GameObject pAttacker; //味方（attacker）
    public GameObject pTank;     //味方（tanker）
    public GameObject eAttacker; //敵　 (attacker)
    public GameObject eTank;     //敵   (tanker)

    public GameObject missilePrefabP;  // ミサイルのプレハブ味方
    public Transform spawnPointP;      // ミサイルの発射位置味方
    public Transform targetP;          // ミサイルのターゲット味方
    public Image missileReadyImageP;   // ミサイルのイメージ味方
    //private float missileReadyTimeP = 60f; // 1分味方
    public bool isFiring = false;  //発射したフラグ

    private GameObject currentMissileP; // 現在のミサイル味方
    private bool isMissileReadyP = false; // ミサイルが準備完了したか味方

    public GameObject missilePrefabE;  // ミサイルのプレハブ敵
    public Transform spawnPointE;      // ミサイルの発射位置敵
    public Transform targetE;          // ミサイルのターゲット敵

    private GameObject currentMissileE; // 現在のミサイル敵
    private bool isMissileReadyE = false; // ミサイルが準備完了したか敵

    public bool buttonNotPush = true;  //Wizard行動ボタンのフラグ
    public bool enemyWizardSelect = true;  //敵Wizardのフラグ
    public GameObject wizardReady;　　　   //wizard準備中
    public GameObject wizardReadyOk;       //wizard準備オッケー

    public int maxPAttacker = 2;       //ストック数上限
    public int maxPTank = 2;           //ストック数上限
    public Text maxPAttackerText; // UIで表示するためのテキスト
    public Text maxPTankText; // UIで表示するためのテキスト
    public int maxEnemyUnits = 3;      // 最大スポーン数
    public int currentEnemyUnits = 0; // 現在の敵ユニット数

    public float currentMissileTime;
    public float currentWizardTime;

    //AudioSource
    public AudioSource decision;
    public AudioSource missileSound;
    //public AudioSource
    //public AudioSource


    [SerializeField] private PlayerTower playerTower;
    [SerializeField] private EnemyTower enemyTower;
    [SerializeField] private PlayerNotSpawn playerNotSpawn;
    [SerializeField] private EnemyNotSpawn enemyNotSpawn;
    [SerializeField] private PlayerWizard playerWizard;
    [SerializeField] private EnemyWizard enemyWizard;



    void Start()
    {
        menus.SetActive(false);  // メニューを非表示

        notPushButton.SetActive(true); //念のためゲーム開始した時にボタンを押せないようにする

        gameOverAnim = GameObject.Find("GameOverUI").GetComponent<Animator>();

        StartCoroutine(PauseGame());  //開始3秒停止のコルーチン開始

        StartCoroutine(CurrentEnemySpawn());  //enemyの生成を遅らせるコルーチン

        StartCoroutine(EnemyAutoSpawn());  //Enemy自動スポーンコルーチン開始

        StartCoroutine(RegeneratePAttacker()); // 10秒ごとに maxPAttacker を回復する
        StartCoroutine(RegeneratePTank());     // 10秒ごとに maxPTank     を回復する

        StartCoroutine(MissileSpawnRoutine());  //味方
        StartCoroutine(MissileSpawnRoutineE()); //敵

        missileReadyImageP.gameObject.SetActive(false);


        DifficultyMissileWizardTime(); //敵魔法使いとミサイルのチャージ速度


        // 10秒後にボタンを有効化
        StartCoroutine(EnableButtonsAfterDelay(20f));
        // 10秒後に敵Wizard行動可
        StartCoroutine(EnableEnemyWizard());
        //wizardは最初使えない
        wizardReadyOk.SetActive(false);

    }

    void Update()
    {
        GameOver();
        GameClear();

        StartCoroutine(MissileAI());
        //Test用
        DefficultyWizard();　//敵の魔法使いの攻撃メソッドの呼び出し
    }

    /*void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }*/

    void UpdateUI()
    {
        maxPAttackerText.text = maxPAttacker.ToString();
        maxPTankText.text = maxPTank.ToString();
    }

    IEnumerator PauseGame()
    {
        Time.timeScale = 0; // 時間停止
        notPushButton.SetActive(true);

        float remainingTime = delayTime; //カウントダウン初期値設定

        while (remainingTime > 0)
        {
            countdownText.text = Mathf.Ceil(remainingTime).ToString(); //テキスト更新

            yield return new WaitForSecondsRealtime(1.0f); //Ⅰ秒待機

            remainingTime--; //残り時間を減らす
        }

        countdownText.text = "Go!";
        yield return new WaitForSecondsRealtime(0.5f);
        countdownText.text = ""; //テキストを消す
        countdownText.gameObject.SetActive(false);

        Time.timeScale = 1; // 時間を再開
        notPushButton.SetActive(false);
    }

    public void GameOver()
    {
        if (playerTower.towerHp <= 0)
        {
            gameOverAnim.SetBool("isGameOver", true);
            StartCoroutine(StopGame());
        }

    }

    public void GameClear()
    {
        if (enemyTower.towerHp <= 0)
        {
            gameClearAnim.SetBool("isGameClear", true);
            StartCoroutine(StopGame());
        }
    }

    public IEnumerator StopGame()
    {
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 0;
        //AudioListener.pause = true; // SE は止める
        menus.SetActive(true);
        notPushButton.SetActive(true);
    }

    public void RestartOnClick()
    {
        

        // 現在のシーン名を取得
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 現在のシーンを再ロード
        SceneManager.LoadScene(currentSceneName);
    }

    public void TitleOnClick()
    {
        //Time.timeScale = 1;  //移行前にタイムスケールを戻す
        SceneManager.LoadScene("TitleScene");
    }

    public void DifficultyMissileWizardTime()
    {
        switch (DifficultyManager.Instance.difficulty)
        {
            case 0:
                currentMissileTime = 25f;
                currentWizardTime = 20f;
                //Debug.Log(currentMissileTime + ":" + currentWizardTime);
                break;
            case 1:
                currentMissileTime = 20f;
                currentWizardTime = 17f;
                //Debug.Log(currentMissileTime + ":" + currentWizardTime);
                break;
            case 2:
                currentMissileTime = 13f;
                currentWizardTime = 15f;
                //Debug.Log(currentMissileTime + ":" + currentWizardTime);
                break;
        }
    }



    public void PAttackerOnClick() //PlayerAttackerを出現させるメソッド
    {
        if (maxPAttacker > 0 && !playerNotSpawn.notSpawn && !notPlayerSpawn)
        {
            StartCoroutine(SpawnCoolTime());
            decision.Play();
            Vector3 playerAttackerSpawnPosition = new Vector3(-41.4f, 17.651f, -19.67f);
            //インスタンス化して生成
            GameObject playerAttackerInstance = Instantiate(pAttacker, playerAttackerSpawnPosition, Quaternion.Euler(0f, 180f, 0f));

            // ステータスをセット
            PAttackerScript stats = playerAttackerInstance.AddComponent<PAttackerScript>();
            stats.Initialize(100, 150, 20, 32);
            stats.ShowStats(); // 確認のためログ出力

            maxPAttacker--;
            UpdateUI();
            //Debug.Log(maxPAttacker);
        }
        else
        {
            //Debug.Log("[Attacker]現在スポーンさせられません");
        }
    }

    public void PTTankOnClick()   //PlayerTankを出現させるメソッド
    {
        if (maxPTank > 0 && !playerNotSpawn.notSpawn && !notPlayerSpawn)
        {
            StartCoroutine(SpawnCoolTime());
            decision.Play();
            Vector3 playerTankSpawnPosition = new Vector3(-41.34f, 17.65f, -19.28f);
            //インスタンス化して生成
            GameObject playerTankInstance = Instantiate(pTank, playerTankSpawnPosition, Quaternion.Euler(0f, 180f, 0f));

            // ステータスをセット
            PTankScript stats = playerTankInstance.AddComponent<PTankScript>();
            stats.Initialize(200, 300, 10, 23);
            stats.ShowStats(); // 確認のためログ出力

            maxPTank--;
            UpdateUI();
            //Debug.Log(maxPTank);
        }
        else
        {
            //Debug.Log("[Tank]現在スポーンできません");
        }
    }

    public IEnumerator SpawnCoolTime()  //キャラクターのスポーンするクールタイム
    {
        //一回クリックされたらスポーン不可にする
        notPlayerSpawn = true;

        //指定時間待つ
        yield return new WaitForSeconds(3f);

        //再度スポーン可能にする
        notPlayerSpawn = false;
    }

    public IEnumerator CurrentEnemySpawn() //enemyを指定時間遅らせてからスポーンさせる
    {
        yield return new WaitForSeconds(0.1f);
        isEnemySpawn = true;
        //Debug.Log("ゲームが開始されてからEnemyが湧く" + isEnemySpawn);
    }

    public IEnumerator EnemyAutoSpawn()   //すべてのエネミーをオートで湧かせる
    {
        while (!isEnemySpawn)
        {
            yield return null;
        }

        while (true)
        {
            if (currentEnemyUnits < maxEnemyUnits && !enemyNotSpawn.notSpawn && isEnemySpawn) // 3体未満ならスポーン可能
            {

                int rad = Random.Range(1, 11);

                //Debug.Log("抽選された数字は" + rad + ":です。");

                if (rad % 2 == 0)
                {
                    EAttackerSpawn();
                }
                else
                {
                    ETankSpawn();
                }

                currentEnemyUnits++; // スポーン数を増やす
                Debug.Log("敵ユニット数: " + currentEnemyUnits);
            }
            else
            {
                Debug.Log("敵ユニットが最大に達したためスポーン停止");
            }

            yield return new WaitForSeconds(7f); //7秒の間隔
        }
    }

    public void EAttackerSpawn() //EnemyAttackerを出現させるメソッド
    {
        Vector3 enemyAttackerSpawnPosition = new Vector3(6.6f, 17.651f, -19.67f);
        //インスタンス化して生成
        GameObject enemyAttackerInstance = Instantiate(eAttacker, enemyAttackerSpawnPosition, Quaternion.Euler(0f, 180f, 0f));

        //Debug.Log("AttackerEnemyをスポーン");

        // ステータスをセット
        EAttackerScript stats = enemyAttackerInstance.AddComponent<EAttackerScript>();



        switch (DifficultyManager.Instance.difficulty)
        {
            case 0:
                stats.Initialize(120, 170, 23, 32);
                stats.ShowStats(); // 確認のためログ出力
                Debug.Log("難易度Easy選択");
                break;
            case 1:
                stats.Initialize(150, 200, 23, 37);
                stats.ShowStats(); // 確認のためログ出力
                Debug.Log("難易度Normal選択");
                break;
            case 2:
                stats.Initialize(180, 220, 23, 32);
                stats.ShowStats(); // 確認のためログ出力
                Debug.Log("難易度Hard選択");
                break;
        }
    }

    public void ETankSpawn()
    {
        Vector3 enemyTankSpawnPosition = new Vector3(6.6f, 17.651f, -19.67f);
        //インスタンス化して生成
        GameObject enemyTankInstance = Instantiate(eTank, enemyTankSpawnPosition, Quaternion.Euler(0f, 180f, 0f));

        //Debug.Log("TankEnemyをスポーン");

        // ステータスをセット
        ETankScript stats = enemyTankInstance.AddComponent<ETankScript>();

        switch (DifficultyManager.Instance.difficulty)
        {
            case 0:
                stats.Initialize(200, 330, 10, 18);
                stats.ShowStats(); // 確認のためログ出力
                //Debug.Log("難易度Easy選択");
                break;
            case 1:
                stats.Initialize(230, 350, 10, 18);
                stats.ShowStats(); // 確認のためログ出力
                //Debug.Log("難易度Normal選択");
                break;
            case 2:
                stats.Initialize(270, 380, 13, 24);
                stats.ShowStats(); // 確認のためログ出力
                //Debug.Log("難易度Hard選択");
                break;
        }
    }

    IEnumerator RegeneratePAttacker()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10秒待つ

            if (maxPAttacker < 2)
            {
                maxPAttacker++;
                UpdateUI();  // UIを更新
                //Debug.Log("maxPAttacker 回復: " + maxPAttacker);
            }
        }
    }

    IEnumerator RegeneratePTank()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            if (maxPTank < 2)
            {
                maxPTank++;
                UpdateUI();  // UIを更新
                //Debug.Log("maxPTank 回復:" + maxPTank);
            }
        }
    }

    public void DecreaseEnemyCount()
    {
        currentEnemyUnits = Mathf.Max(0, currentEnemyUnits - 1); // 0未満にならないようにする
        //Debug.Log("敵ユニットが倒された！現在の数: " + currentEnemyUnits);
    }


    // 1分ごとにミサイルを生成（ゲーム開始直後はミサイルなし）
    IEnumerator MissileSpawnRoutine()//味方
    {  
        do
        {
            while(currentMissileP != null)
            {
                yield return null;
            }
            yield return new WaitForSeconds(25f); // ゲーム開始後 1 分待つ
            SpawnMissile();
        } while (true) ;

    }

    // ミサイルの生成
    void SpawnMissile()
    {
        if (currentMissileP == null)//味方
        {
            currentMissileP = Instantiate(missilePrefabP, spawnPointP.position, Quaternion.Euler(13.069f, -2.605f, -61.991f));
            missileReadyImageP.gameObject.SetActive(true);
            PlayerMissaile missileScript = currentMissileP.GetComponent<PlayerMissaile>();
            isMissileReadyP = true;
            if (missileScript != null)
            {
                missileScript.PrepareMissile(); // ここで動かないようにする
            }
            //Debug.Log("ミサイル準備完了！");
        }
    }

    // OnClick でミサイルを発射
    public void FireMissileOnClick()//味方
    {
        if (currentMissileP != null && isMissileReadyP)
        {
            PlayerMissaile missileScript = currentMissileP.GetComponent<PlayerMissaile>();
            if (missileScript != null)
            {
                missileScript.SetTarget(targetP); // ターゲット設定
                missileScript.LaunchMissile();  // 発射
                isFiring = true; //発射したフラグ
                missileSound.Play();
                isMissileReadyP = false; // 次のミサイルが来るまで待つ
                isFiring = false; //発射したフラグ
                currentMissileP = null;  // ミサイルをクリア
                missileReadyImageP.gameObject.SetActive(false); // 発射後に画像を非表示にする
            }
        }
    }

    IEnumerator MissileSpawnRoutineE()//敵
    {
        yield return new WaitForSeconds(currentMissileTime); // ゲーム開始後 1 分待つ へんこうてん
        while (true)
        {
            SpawnMissileE();
            yield return new WaitForSeconds(currentMissileTime); // 1分ごとに生成　へんこうてん
        }
    }

    // ミサイルの生成
    void SpawnMissileE()
    {
        if (currentMissileE == null)//敵
        {
            currentMissileE = Instantiate(missilePrefabE, spawnPointE.position, Quaternion.Euler(-11.411f, 17.385f, 73.05f));
            PlayerMissaile missileScript = currentMissileE.GetComponent<PlayerMissaile>();
            isMissileReadyE = true;
            if (missileScript != null)
            {
                missileScript.PrepareMissile(); // ここで動かないようにする
            }
            //Debug.Log("ミサイル準備完了！");
        }
    }

    // 自動でミサイルを発射
    public void EnemyULT()//敵
    {

        if (currentMissileE != null && isMissileReadyE) //ミサイルを撃つ
        {
            EnemyMissile missileScriptE = currentMissileE.GetComponent<EnemyMissile>();
            if (missileScriptE != null)
            {
                missileScriptE.SetTarget(targetE); // ターゲット設定
                missileScriptE.LaunchMissile();  // 発射
                missileSound.Play();
                //Debug.Log("ミサイル発射ミサイル発射ミサイル発射ミサイル発射ミサイル発射");
                isMissileReadyE = false; // 次のミサイルが来るまで待つ
                currentMissileE = null;  // ミサイルをクリア
            }
        }
    }
//------------------------------------------------↓敵のミサイルをAI化-----------------------------------------------//

    public IEnumerator MissileAI()
    {
        if (!playerWizard.isBarrier)
        {
            EnemyULT();  //敵のミサイル発射メソッドの呼び出し
        }
        else
        {
            yield return new WaitForSeconds(5f);
            EnemyULT();  //敵のミサイル発射メソッドの呼び出し
        }
    }

//--------------------------------------------------------------------------------------------------------------------//

    public void PlayerWizardOnClickA()
    {
        if (!buttonNotPush)
        {
            decision.Play();
            playerWizard.Attack();
            StartCoroutine(DisableButtonsForDuration(20f));
        }
        
    }
    public void PlayerWizardOnClickB()
    {
        if (!buttonNotPush)
        {
            decision.Play();
            playerWizard.DirectAttack();
            StartCoroutine(DisableButtonsForDuration(20f));
        }
        
    }
    public void PlayerWizardOnClickC()
    {
        if (!buttonNotPush)
        {
            decision.Play();
            playerWizard.Barrier();
            StartCoroutine(DisableButtonsForDuration(20f));
        }
        
    }
    private IEnumerator EnableButtonsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("Wizard行動できます");
        buttonNotPush = false;
    }
    private IEnumerator DisableButtonsForDuration(float duration)
    {
        // ボタンを無効にする
        buttonNotPush = true;

        // wizardテキストを準備中
        wizardReady.SetActive(true);
        wizardReadyOk.SetActive(false);

        // 指定時間待つ
        yield return new WaitForSeconds(duration);

        // ボタンを再び有効にする
        buttonNotPush = false;

        wizardReady.SetActive(false);
        wizardReadyOk.SetActive(true);

    }

    private IEnumerator EnableEnemyWizard()  //敵Wizardをゲーム開始10秒後に行動可にする
    {
        yield return new WaitForSeconds(currentWizardTime); //へんこうてん　
        wizardReady.SetActive(false);
        wizardReadyOk.SetActive(true);
        enemyWizardSelect = false;
    }

    private IEnumerator DisableEnemyWizardForDuration()
    {
        enemyWizardSelect = true;

        yield return new WaitForSeconds(currentWizardTime);  //へんこうてん

        enemyWizardSelect = false;
    }

    //Test用
    public void WizardTestA()  //Attackテスト結果OK
    {
        if (!enemyWizardSelect)
        {
            enemyWizard.Attack();
            StartCoroutine(DisableEnemyWizardForDuration());
        }
        
    }

    public void WizardTestB() //DirectAttackテスト結果OK
    {
        if (!enemyWizardSelect)
        {
            enemyWizard.DirectAttack();
            StartCoroutine(DisableEnemyWizardForDuration());
        }
    }

    public void WizardTestC()
    {
        if (!enemyWizardSelect)
        {
            enemyWizard.Barrier();
            StartCoroutine(DisableEnemyWizardForDuration());
        }
    }

//-----------------------------------------------------敵の魔法使いAI化----------------------------------------------------

    public void WizardAI() //簡単・普通用
    {
        if (enemyNotSpawn.notSpawn)
        {
            WizardTestA();
        }
        else if (isMissileReadyP && !enemyWizard.isBarrier)
        {
            WizardTestC();
        }
        else if(!playerWizard.isBarrier && !isMissileReadyP)
        {
            WizardTestB();
        }
    }

    public void WizardAI1()  //難しい用
    {
        if (enemyNotSpawn.notSpawn)
        {
            WizardTestA();
        }
        else if (isFiring || playerWizard.isMagicB && !enemyWizard.isBarrier)
        {
            WizardTestC();
        }
        else
        {
            WizardTestB();
        }
    }

    public void DefficultyWizard()
    {
        switch (DifficultyManager.Instance.difficulty)
        {
            case 0:
                WizardAI();
                break;
            case 1:
                WizardAI();
                break;
            case 2:
                WizardAI1();
                break;
        }
    }
//-----------------------------------------------------------------------------------------------------------------
}
