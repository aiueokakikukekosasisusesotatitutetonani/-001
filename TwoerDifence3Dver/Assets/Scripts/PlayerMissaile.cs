using UnityEngine;

public class PlayerMissaile : MonoBehaviour
{
    private Transform target; // 目標地点
    private Rigidbody rb;
    public float launchAngle = 30f; // 低めの発射角度
    public float gravity = 9.81f;   // 重力

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // ターゲットを設定
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // 生成時にミサイルを固定（落下しないようにする）
    public void PrepareMissile()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>(); // 念のため取得
        }

        rb.velocity = Vector3.zero;  // 速度をゼロにする
        rb.angularVelocity = Vector3.zero;  // 回転も止める
        rb.useGravity = false;  // 重力無効化
        rb.isKinematic = true;  // 物理演算無効化
    }

    // ミサイルの発射
    public void LaunchMissile()
    {
        if (target == null)
        {
            Debug.LogError("ターゲットが設定されていません！");
            return;
        }

        rb.isKinematic = false; // 物理演算を有効化
        rb.useGravity = true; // 重力を有効化

        Vector3 startPos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 toTarget = targetPos - startPos;

        toTarget.z += 7.0f; // 数値を増やすとさらに左へ

        float heightDifference = toTarget.y;
        toTarget.y = 0; // 水平方向の距離を計算
        float distance = toTarget.magnitude;

        float angleRad = launchAngle * Mathf.Deg2Rad;
        float velocity = Mathf.Sqrt((gravity * distance * distance) /
            (2 * (distance * Mathf.Tan(angleRad) - heightDifference)));

        Vector3 velocityXZ = toTarget.normalized * velocity;
        float velocityY = velocity * Mathf.Tan(angleRad);

        Vector3 launchVelocity = new Vector3(velocityXZ.x, velocityY, velocityXZ.z);
        rb.velocity = launchVelocity;

        // 初期の向き調整
        transform.forward = rb.velocity.normalized;
    }

    void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.up = rb.velocity.normalized;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyTower"))
        {
            //Debug.Log("敵のタワーに衝突しましたので、ミサイルをDestroyします");
            //Debug.Log("ターゲットに命中！");
            Destroy(gameObject);
        }

        if (other.CompareTag("EnemyBarrier"))
        {
            Destroy(gameObject);
        }
    }
}
