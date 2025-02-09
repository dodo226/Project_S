using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIceCube : MonoBehaviour
{
    // 현재 큐브 사이즈
    [field: SerializeField] public int nowSize { get; private set; }

    // 큐브 스케일 조정 값
    [SerializeField] private Vector3 bigScale;
    [SerializeField] private Vector3 halfScale;
    [SerializeField] private Vector3 smallScale;

    // 촛불에 얼마나 닿아야 녹는지 결정하는 시간
    [SerializeField] private float meltingPointTime;

    // 현재 촛불에 닿은 시간
    [SerializeField] private float meltingTime;

    // 현재 촛불에 닿았는지 체크하는 Bool값
    [SerializeField] private bool isTorchTouch;

    // 마지막으로 녹았는지 체크하는 Bool값
    // [SerializeField] private bool isFinalMelt;

    // 다 녹았을 때 나올 사과 아이템
    [SerializeField] private GameObject apple;

    // 리지드바디
    private Rigidbody rb;

    // 이펙트
    private ParticleSystem meltingParticle;
    private ParticleSystem finalMeltParticle;

    // 메쉬
    private List<MeshRenderer> meshes = new List<MeshRenderer>();

    // 얼음 안의 아이템
    private Transform InIceItem;

    // 토치 스크립트의 트리거를 받아올 변수
    private SphereCollider sphereCollider;

    private void Awake()
    {
        // 스케일 캐싱 (미리 연구된 값)
        bigScale = new Vector3(0.4f, 0.4f, 0.4f);
        halfScale = new Vector3(0.35f, 0.35f, 0.35f);
        smallScale = new Vector3(0.3f, 0.3f, 0.3f);

        // 스케일 초기화, 녹는 시간 초기화
        nowSize = 0;
        meltingTime = 0f;

        // 리지드 바디 캐싱
        rb = GetComponent<Rigidbody>();

        // 파티클 캐싱
        meltingParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        finalMeltParticle = transform.GetChild(1).GetComponent<ParticleSystem>();

        // 얼음 안의 아이템 오브젝트 캐싱
        InIceItem = transform.GetChild(2).transform;

        // 메쉬 렌더러 캐싱
        meshes.Add(GetComponent<MeshRenderer>());
        meshes.Add(InIceItem.GetChild(0).GetComponent<MeshRenderer>());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Torch"))
        {
            sphereCollider = other.GetComponent<SphereCollider>();

            if(sphereCollider != null && sphereCollider.enabled) 
            {
                isTorchTouch = true;
                meltingParticle.transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
                meltingParticle.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Torch"))
        {
            isTorchTouch = false;
            sphereCollider = null;
            meltingParticle.Stop();
        }
    }

    private void Update()
    {
        // { 닿고 있는 촛불의 트리거가 꺼졌다면 시간 체크 중지
        if (sphereCollider != null)
        {
            if (sphereCollider.enabled == false)
            {
                isTorchTouch = false;
                sphereCollider = null;
                meltingParticle.Stop();
            }
        }   // } 닿고 있는 촛불의 트리거가 꺼졌다면 시간 체크 중지

        // { 촛불에 닿으면 시간 체크
        if (isTorchTouch)
        {
            meltingTime += Time.deltaTime;

            if (meltingTime >= meltingPointTime)
            {
                ChangeIceCubeScale(nowSize);
                meltingTime = 0f;
            }
        }   // } 촛불에 닿으면 시간 체크
    }

    /// <summary>
    /// 아이스 큐브의 크기를 변경시키는 메서드.
    /// </summary>
    /// <param name="_index">0 = 빅 사이즈, 1 = 중간 사이즈, 2 = 작은 사이즈</param>
    public void ChangeIceCubeScale(int _index)
    {
        // 사이즈 ++
        nowSize++;

        // 작은 사이즈에서 사이즈 변경 시 초기화
        if (nowSize >= 3)
        {
            DropApple();
        }

        // 사이즈 변경
        switch (_index + 1)
        {
            case 0:
                transform.DOShakeScale(0.2f, 0.2f);
                transform.localScale = bigScale;
                InIceItem.localScale = Vector3.one;
                break;
            case 1:
                transform.DOShakeScale(0.2f, 0.2f);
                transform.localScale = halfScale;
                InIceItem.localScale = Vector3.one * 1.1f;
                break;
            case 2:
                transform.DOShakeScale(0.2f, 0.2f);
                transform.localScale = smallScale;
                InIceItem.localScale = Vector3.one * 1.2f;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 녹은 뒤 사과 아이템을 드랍하는 메서드.
    /// </summary>
    public void DropApple()
    {
        // 토치 터치 끄기
        isTorchTouch = false;

        // 최종적으로 녹았는지 체크
        // isFinalMelt = true;

        // 현재 녹는 시간 초기화
        meltingTime = 0f;

        // 메쉬 렌더러 끄기
        foreach (MeshRenderer mr in meshes)
        {
            mr.enabled = false;
        }

        // 사이즈 초기화
        transform.localScale = bigScale;
        InIceItem.localScale = Vector3.one;
        nowSize = 0;

        // 녹는 이펙트 플레이
        finalMeltParticle.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        finalMeltParticle.transform.position = this.transform.position;
        finalMeltParticle.Play();

        // 상호작용 멈춤
        rb.isKinematic = true;

        // 코루틴 활성화
        StartCoroutine(ActiveMeshRenderer());
    }

    /// <summary>
    /// 이 아이템을 몇 초 후에 SetActive(false)해주는 코루틴.
    /// </summary>
    /// <returns></returns>
    IEnumerator ActiveMeshRenderer()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        apple.SetActive(true);
        apple.transform.position = this.gameObject.transform.position;

        this.gameObject.SetActive(false);
    }
}
