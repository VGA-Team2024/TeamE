using System;
using UnityEngine;

public class PlayerDummy : MonoBehaviour
{
    Camera cam;
    
    public float moveSpeed = 10.0f;  // 移動速度
    public float lookSpeed = 1.0f;  // 回転速度

    private float yaw;  // ヨー（左右回転）
    private float pitch;  // ピッチ（上下回転）

    public GameObject arrow;
    public float shootForce;

    private void Start()
    {
        cam = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // マウス入力による回転
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        yaw += mouseX;  // 水平方向の回転
        pitch -= mouseY;  // 垂直方向の回転（マウスY方向は逆に動くので-）

        // 回転制限（ピッチ角度を上下90度の範囲に制限）
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // カメラの回転設定
        cam.transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // プレイヤーの回転を反映（プレイヤーはy軸のみ回転）
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        // キー入力による移動
        Vector3 moveDirection = Vector3.zero;

        // カメラの向きに基づいて移動方向を計算
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        // 垂直方向（Y軸）は無視して、地面に沿った移動方向を計算
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= right;  // 左移動
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += right;  // 右移動
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += forward;  // 前進
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= forward;  // 後退
        }

        // 相対的な移動を行う
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        var spawnPos = cam.transform.position + cam.transform.forward;
        var projectile = Instantiate(arrow, spawnPos, Quaternion.identity);
        var rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(cam.transform.forward * shootForce, ForceMode.VelocityChange);
    }
}
