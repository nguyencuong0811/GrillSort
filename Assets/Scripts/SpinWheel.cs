using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class SpinWheel : MonoBehaviour
{
    [Header("Wheel")]
    public RectTransform wheel;
    public Button spinButton;

    [Header("Reward")]
    public int[] rewards = { 1, 2, 3, 4, 5, 6, 7, 8};

    //private bool isSpinning;

    //private const string LAST_SPIN_DATE = "LAST_SPIN_DATE";


    public void OnSpin()
    {
        // Random ô thưởng
        int rewardIndex = UnityEngine.Random.Range(0, rewards.Length);

        // Góc mỗi ô
        float anglePerItem = 360f / rewards.Length;

        // Góc cần dừng
        float targetAngle =
            360 * 5 + // quay nhiều vòng cho đẹp
            (rewardIndex * anglePerItem);

        wheel
            .DORotate(new Vector3(0, 0, -targetAngle), 5f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                Debug.Log("Nhận được: " + rewards[rewardIndex] + " vàng");
            });
    }
}