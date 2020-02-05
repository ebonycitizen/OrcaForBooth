using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BrunoMikoski.TextJuicer;
using SWS;

public class PathMoveEvent : MonoBehaviour
{
    [SerializeField]
    private Transform pathRef;
    [SerializeField]
    private float moveTime;
   

    [SerializeField]
    private Transform orca;
    
    [SerializeField]
    private GameObject tutorial_2;

    [SerializeField]
    private GameObject tutorial_3;

    [SerializeField]
    private GameObject tutorial_4;
    [SerializeField]
    private GameObject tutorial_5;

    [SerializeField]
    private GameObject tutorial_6;


    private Sequence s;
    private Sequence s2;
    
    public bool canTouch { get; private set; }

    public bool hasDone { get; private set; }

    [SerializeField]
    private ParticleSystem[] illusionFishes;

    [SerializeField]
    private MyCinemachineDollyCart dollyCart;

    [SerializeField]
    private GameObject sendObj;
    private OrcaState orcaState;

    // Start is called before the first frame update
    void Start()
    {

        Vector3[] movePath = new Vector3[pathRef.childCount];

        for (int i = 0; i < movePath.Length; i++)
        {
            movePath[i] = pathRef.GetChild(i).position;
        }
        

        s = DOTween.Sequence();
        
        s.AppendCallback(() => DisableUISeq())
            .Join(transform.DOPath(movePath, moveTime, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .SetLookAt(0.05f, Vector3.forward));

        s2 = DOTween.Sequence();
        s2.AppendInterval(moveTime - 2)
            .AppendCallback(() => hasDone = true)
           .AppendCallback(() => UISeq1());

        InitUI();
        orcaState = Object.FindObjectOfType<OrcaState>();

    }

    private void InitUI()
    {
        var textGroup1 = tutorial_3.GetComponentInChildren<CanvasGroup>();
        textGroup1.DOFade(0f, 0f);

        var textGroup2 = tutorial_4.GetComponentInChildren<CanvasGroup>();
        textGroup2.DOFade(0f, 0f);

        var textGroup3 = tutorial_5.GetComponentInChildren<CanvasGroup>();
        textGroup3.DOFade(0f, 0f);

        var textGroup4 = tutorial_6.GetComponentInChildren<CanvasGroup>();
        textGroup3.DOFade(0f, 0f);
    }

    private void UISeq1()
    {
        Sequence s_1 = DOTween.Sequence();

        var textGroup = tutorial_3.GetComponentInChildren<CanvasGroup>();
        var anim1 = tutorial_3.GetComponentInChildren<JuicedText>();
        var duration = 1f;

        s_1.Append(textGroup.DOFade(1f, duration))
            .AppendCallback(() => anim1.Play());

        s_1.Play();
    }

    private void UISeq2()
    {
        Sequence s_1 = DOTween.Sequence();

        var textGroup1 = tutorial_3.GetComponentInChildren<CanvasGroup>();
        var textGroup2 = tutorial_4.GetComponentInChildren<CanvasGroup>();
        var textGroup3 = tutorial_5.GetComponentInChildren<CanvasGroup>();

        var anim2 = tutorial_4.GetComponentInChildren<JuicedText>();
        var anim3 = tutorial_5.GetComponentInChildren<JuicedText>();

        var duration = 1f;

        s_1.Append(textGroup1.DOFade(0f, duration))
            //show tutorial_4
            .Join(textGroup2.DOFade(1f, duration))
            .AppendCallback(() => StartCoroutine("ShowIllusionFish"))
            .AppendCallback(() => anim2.Play())
            //del tutorial_3
            .AppendInterval(1f)
            .AppendCallback(() => tutorial_3.SetActive(false))

            .AppendInterval(10f)
            .Append(textGroup2.DOFade(0f, duration))
            //show tutorial_5
            .Join(textGroup3.DOFade(1f, duration))
            .AppendCallback(() => anim3.Play())
            //del tutorial_4
            .AppendInterval(1f)
            .AppendCallback(() => tutorial_4.SetActive(false))

            .AppendInterval(7f)
            .Append(textGroup3.DOFade(0f, duration))
            //del tutorial_5
            .AppendInterval(1f)
            .AppendCallback(() => tutorial_4.SetActive(false));

        s_1.Play();
    }

    private IEnumerator ShowIllusionFish()
    {
        foreach (var p in illusionFishes)
        {
            p.Play();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void DisableUISeq()
    {
        Sequence s_1 = DOTween.Sequence();

        var textGroup = tutorial_2.GetComponentInChildren<CanvasGroup>();
        var textGroup2 = tutorial_6.GetComponentInChildren<CanvasGroup>();
        var duration = 1f;

        var anim1 = tutorial_6.GetComponentInChildren<JuicedText>();
        
        s_1.Append(textGroup.DOFade(0f, 1f))
            .Join(textGroup2.DOFade(1f, duration))
            
            .AppendInterval(1f)
            .AppendCallback(() => anim1.Play())
            
            .AppendCallback(() => tutorial_2.SetActive(false))
            .AppendInterval(6f)
            .Append(textGroup2.DOFade(0f, duration))
            .AppendCallback(() => tutorial_6.SetActive(false));

        s_1.Play();
    }
    public void EndEvent()
    {
        StartCoroutine("EventEnd");

        bool hasChangeState = orcaState.ChangeState("G_Swim", sendObj);

        sendObj.GetComponent<splineMove>().StartMove();

        if (!hasChangeState)
            return;

        StartCoroutine("ChangeState");
    }
    private IEnumerator ChangeState()
    {
        yield return new WaitForSeconds(4f);
        {
            bool hasChangeState = orcaState.ChangeState("G_Idle", sendObj);
        }
    }
    private IEnumerator EventEnd()
    {
        UISeq2();
        yield return new WaitForSeconds(3f);
        float time = 10f;
        while (time < 10)
        {

            dollyCart.m_Speed = time;
            time += Time.deltaTime * 3f;
            yield return null;
        }

        dollyCart.m_Speed = 10f;

        yield return null;
    }

    public void startEvent()
    {
        s.Play();
        s2.Play();
    }
}
