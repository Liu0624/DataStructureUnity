using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class MonoHelper : MonoSingleton<MonoHelper>
{
    public delegate void ActionOnUpdate();
    public delegate void ActOnceOnUpdate();

    public ActionOnUpdate actions;
    public ActOnceOnUpdate actOnce;
    Delegate[] delg;

    public IEnumerator ie;
    public IEnumerator audioIE;

    private void Awake()
    {
        actions = null;
    }

    void Update()
    {
        if (actions != null)
            actions();


    }

    private void FixedUpdate()
    {
        if (actOnce != null && (delg = actOnce.GetInvocationList()).Length > 0)
        {
            for (int i = 0; i < delg.Length; i++)
            {
                ActOnceOnUpdate aoou = (ActOnceOnUpdate)delg[i];
                aoou();
            }
            actOnce = null;
        }
    }

    public void CoroutineStart()
    {
        if (ie != null)
            StartCoroutine(ie);
    }

    public void CoroutineStop()
    {
        if (ie != null)
        {
            StopCoroutine(ie);
            ie = null;
        }
    }

    public void AudioCoroutineStart()
    {
        if (audioIE != null)
            StartCoroutine(audioIE);
    }

    public void AudioCoroutineStop()
    {
        if (audioIE != null)
        {
            StopCoroutine(audioIE);
            audioIE = null;
        }
    }

    public void AllCoroutinesStop()
    {
        StopAllCoroutines();
    }

}
