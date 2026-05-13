using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class SpinWheel : MonoBehaviour
{
    [Header("Wheel")]
    public RectTransform wheel;

    [Header("Reward")]
    public int[] rewards = { 1, 2, 3, 4, 5, 6, 7, 8};

    public AudioClip spin;

    private bool isSpinning = false;

    public void OnSpin()
    {
        if(isSpinning) return;
        // Random ô thưởng
        int rewardIndex = UnityEngine.Random.Range(0, rewards.Length);
        isSpinning = true;
        // Góc mỗi ô
        float anglePerItem = 360f / rewards.Length;

        // Góc cần dừng
        float targetAngle =
            360 * 5 + // quay nhiều vòng cho đẹp
            (rewardIndex * anglePerItem);

        AudioManager.Instance?.PlaySFX(spin);
        wheel
            .DORotate(new Vector3(0, 0, -targetAngle), 5f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            {
                CoinEffect.Instance.PlayEffect(transform.position, 1);
                BoosterSystem.Gold += rewards[rewardIndex];
                Debug.Log("Nhận được: " + rewards[rewardIndex] + " vàng");
                isSpinning =false;
            });
    }
}