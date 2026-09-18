using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ring : MonoBehaviour
{
    public bool isComplete = false;

    public LevelLoader ll;
    public SkillResult sr;

    public int nextLevel = 12;
    public bool isStartRing = false;
    public bool isEndRing = false;
    public Ring objNextRing;
    public TMP_Text timerText;
    public TMP_Text task;

    public Material noneMaterial;
    public Material completeMaterial;
    public Material nextMaterial;
    public Material errorMaterial;

    private Renderer rend;
    private float timer;
    private bool isTimer = true;

    void Start()
    {
        sr = FindObjectOfType<SkillResult>();

        isTimer = true;
        rend = GetComponent<Renderer>();
        if (isStartRing) rend.sharedMaterial = nextMaterial;
    }

    private void Update()
    {   
        if (isComplete)
        {
            if (rend.sharedMaterial == nextMaterial)
            {
                rend.sharedMaterial = completeMaterial;
                if (isEndRing) finish();
                else objNextRing.nextRing();
            }
            else if (rend.sharedMaterial == noneMaterial)
            {
                StartCoroutine(error());
            }
        }
        if (isEndRing)
        {
            if (isTimer)
            {
                timer += Time.deltaTime;
                timerText.text = timer.ToString("F2");
            }
        }
        if (task.text == "Для продолжения нажмите Space" && Input.GetKeyDown(KeyCode.Space))
        {
            ll.LoadLevel(nextLevel);
            print("окак");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Frame")
        {
            if (rend.sharedMaterial == nextMaterial)
            {
                rend.sharedMaterial = completeMaterial;
                if (isEndRing) finish();
                else objNextRing.nextRing();
            }
            else if (rend.sharedMaterial == noneMaterial)
            {
                StartCoroutine(error());
            }
        }
    }

    private void finish()
    {
        isTimer = false;
        task.text = "Для продолжения нажмите Space";
        sr.result5 = (int)timer;
    }

    public void nextRing()
    {
        rend.sharedMaterial = nextMaterial;
    }

    IEnumerator error()
    {
        rend.sharedMaterial = errorMaterial;
        yield return new WaitForSeconds(3f);
        rend.sharedMaterial = noneMaterial;
    }
}
