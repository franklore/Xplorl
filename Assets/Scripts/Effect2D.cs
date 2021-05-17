using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect2D : MonoBehaviour
{
    private Animator anim;

    public float destroyTime;

    private float time = 0;

    private void Start()
    {
    }

    private void Update()
    {
        time += Time.deltaTime;
        if (time > destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
