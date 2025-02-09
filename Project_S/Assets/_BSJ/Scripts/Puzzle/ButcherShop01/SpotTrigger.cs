using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotTrigger : MonoBehaviour
{
    ButcherShop01Clear butcherShop01Clear;

    [Header("고기 갯수 정답")]
    [SerializeField] private int chickenAnswer;
    [SerializeField] private int meatAnswer;
    [SerializeField] private int porkAnswer;

    [Header("현재 고기 갯수")]
    [SerializeField] private int chickenCount;
    [SerializeField] private int meatCount;
    [SerializeField] private int porkCount;

    [Header("현재 카운팅 된 고기 총 갯수")]
    [SerializeField] private int nowCounting;

    private int AnswerCheck;

    private ParticleSystem _particleSystem;
    private IceCube iceCube;

    private void Awake()
    {
        // 이 퍼즐의 인덱스는 12번입니다.
        butcherShop01Clear = transform.root.GetChild(12).GetComponent<ButcherShop01Clear>();

        AnswerCheck = 0;
        _particleSystem = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<IceCube>() != null)
        {
            iceCube = other.GetComponent<IceCube>();

            // 닭고기
            if(iceCube.iceCubeType == IceCubeType.Chicken)
            {
                chickenCount++;
                nowCounting++;
            }

            // 소고기
            else if(iceCube.iceCubeType == IceCubeType.Meat)
            {
                meatCount++;
                nowCounting++;
            }

            // 돼지고기
            else if (iceCube.iceCubeType == IceCubeType.Pork)
            {
                porkCount++;
                nowCounting++;
            }

            other.gameObject.SetActive(false);
            _particleSystem.Play();
        }

        // 정답 체크
        if(nowCounting == butcherShop01Clear.iceCubeCount)
        {
            if(chickenAnswer == chickenCount)
            {
                AnswerCheck++;
            }

            if(meatAnswer == meatCount)
            {
                AnswerCheck++;
            }

            if(porkAnswer == porkCount)
            {
                AnswerCheck++;
            }
        }

        // 3은 3가지 고기 종류를 모두 맞췄다는 의미.
        if (AnswerCheck == 3)
        {
            // 클리어 했음을 표시하는 메서드 호출
            butcherShop01Clear.ClearText();

            // 이미 퍼즐을 클리어 했으면 리턴 ( 이 퍼즐은 12번 )
            if (PuzzleManager.instance.puzzles[12]) { return; }

            // 퍼즐을 완전히 클리어 했다는 메서드 호출
            butcherShop01Clear.ButcherShopClear();
        }

        else
        {
            // 클리어하지 못했음을 표시하는 메서드 호출
            butcherShop01Clear.NoClearText();
        }
    }

    /// <summary>
    /// 정답 체크 초기화
    /// </summary>
    public void ResetCounting()
    {
        chickenCount = 0;
        meatCount = 0;
        porkCount = 0;
        AnswerCheck = 0;
        nowCounting = 0;
    }
}
