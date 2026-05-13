using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CoinEffect : MonoBehaviour
{
    public static CoinEffect Instance;
    [SerializeField] private RectTransform[] coins;
    [SerializeField] private RectTransform target;
    [SerializeField] private RectTransform target2;

    public AudioClip coinSfx;

    void Awake()
    {
        Instance = this;
    }
    public void PlayEffect(Vector3 startPos, int n)
    {
        StartCoroutine(IE_Play(startPos, n));
    }

    private IEnumerator IE_Play(Vector3 startPos, int n)
    {
        for(int i = 0; i < coins.Length; i++)
        {
            RectTransform coin = coins[i];

            coin.gameObject.SetActive(true);

            coin.position = startPos;
            coin.localScale = Vector3.one;

            Vector3 randomPos = startPos + new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0
            );

            Sequence seq = DOTween.Sequence();

            AudioManager.Instance?.PlaySFX(coinSfx);
            seq.Append(
                coin.DOMove(randomPos, 0.2f)
                    .SetEase(Ease.OutBack)
            );

            seq.Append(
                coin.DOMove(n == 1 ? target.position : target2.position, 0.5f)
                    .SetEase(Ease.InBack)
            );

            // seq.Join(
            //     coin.DORotate(
            //         new Vector3(0,0,360),
            //         0.7f,
            //         RotateMode.FastBeyond360
            //     )
            // );

            seq.OnComplete(() =>
            {
                coin.gameObject.SetActive(false);
            });

            yield return new WaitForSeconds(0.05f);
        }
    }
}