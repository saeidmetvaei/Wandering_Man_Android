using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CollisionCounter_City : MonoBehaviour
{

    int counter;
    public Text Counttxt;
    public Text Obstacletxt;

    public CanvasGroup MyCanvGroup;

    bool i;


    public AudioSource Audiosource;
    public AudioClip painSound;


    private void Start()
    {
        counter = 0;

        MyCanvGroup.alpha = 0;

        i = false;


        Audiosource.clip = painSound;

    }

    Coroutine co;
    private void OnTriggerEnter(Collider Col)
    {
        

        if (Col.GetComponent<ObstacleType>() == true)
        {

            if (i == true)
            {
                StopCoroutine(co);
            }

            MyCanvGroup.alpha = 1;

            counter += 1;

            Audiosource.Play();

            Counttxt.text = "Overally, You Have Collided to Obstacles for " + counter + " Times!";
            Obstacletxt.text = "You have Collided to a " + Col.GetComponent<ObstacleType>().obstacleType + "!";



            co = StartCoroutine(DoFade());
            i = true;
        }
    }
    public IEnumerator DoFade()
    {

        yield return new WaitForSeconds(5);


        float timer = 0f;

        while (timer <= 3)
        {
            timer += Time.deltaTime;

            MyCanvGroup.alpha = Mathf.Lerp(1, 0, timer / 2);

            Debug.Log(MyCanvGroup.alpha);

            //yield return new WaitForSeconds(Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        
    }
}
