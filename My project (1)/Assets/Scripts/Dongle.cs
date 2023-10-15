using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager manager;
    public int level;
    public bool isDrag;
    public bool isMerge;
    Rigidbody2D rigid;
    CircleCollider2D circle;
    Animator anim;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        anim.SetInteger("Level", level);
    }
    void Update()
    {
        if(isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float leftBorder = -4.2f + transform.localScale.x * 0.5f;
            float rightBorder = 4.2f - transform.localScale.x * 0.5f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }

            mousePos.y = 8;
            mousePos.z = 0;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }
        
    }

    public void Drag()
    {
        isDrag = true;

    }
    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag =="Dongle"){
            Dongle other = collision.gameObject.GetComponent<Dongle>();
            if (level == other.level && !isMerge && !other.isMerge && level <10){
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = other.transform.position.x;
                float otherY = other.transform.position.y;


                if(meY< otherY||(meY==otherY&&meX>otherX)) {
                    other.Hide(transform.position);
                    LevelUp();
                }
            }
            


        }
    }
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;

        rigid.simulated = false;
        circle.enabled = false;

        StartCoroutine(HideRoutine(targetPos));

    }

    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;
        while (frameCount < 20)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            yield return null;
        }
        isMerge = false;
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        isMerge=true;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;

        StartCoroutine(LevelUpRoutine());
    }
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetInteger("Level", level + 1);
        yield return new WaitForSeconds(0.3f);
        level++;

        manager.maxLevel = Mathf.Min(Mathf.Max(level, manager.maxLevel),5);
        isMerge = false;
    }
}
