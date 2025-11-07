using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene_EnemyController : MonoBehaviour
{
    public static TestScene_EnemyController instance;

    public event Action OnSetIsTest;
    public event Action OnTestAction;
    public event Action OnTestAction1;
    public event Action OnTestAction2;
    public event Action OnTestAction3;
    public event Action OnTestAction4;
    public event Action OnTestAction5;
    public event Action OnTestAction6;
    public event Action OnTestAction7;
    public event Action OnTestAction8;
    public event Action OnTestAction9;
    public event Action OnTestAction10;
    public event Action OnTestAction11;
    public event Action OnTestAction12;
    public event Action OnTestAction13;
    public event Action OnTestAction14;
    public event Action OnTestAction15;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetIsTest()
    {
        OnSetIsTest?.Invoke();
    }
    public void EnemyAction()
    {
        OnTestAction?.Invoke();
    }
    public void EnemyAction1()
    {
        OnTestAction1?.Invoke();
    }
    public void EnemyAction2()
    {
        OnTestAction2?.Invoke();
    }
    public void EnemyAction3()
    {
        OnTestAction3?.Invoke();
    }
    public void EnemyAction4()
    {
        OnTestAction4?.Invoke();
    }
    public void EnemyAction5()
    {
        OnTestAction5?.Invoke();
    }
    public void EnemyAction6()
    {
        OnTestAction6?.Invoke();
    }
    public void EnemyAction7()
    {
        OnTestAction7?.Invoke();
    }
    public void EnemyAction8()
    {
        OnTestAction8?.Invoke();
    }
    public void EnemyAction9()
    {
        OnTestAction9?.Invoke();
    }
    public void EnemyAction10()
    {
        OnTestAction10?.Invoke();
    }
    public void EnemyAction11()
    {
        OnTestAction11?.Invoke();
    }
    public void EnemyAction12()
    {
        OnTestAction12?.Invoke();
    }
    public void EnemyAction13()
    {
        OnTestAction13?.Invoke();
    }
    public void EnemyAction14()
    {
        OnTestAction14?.Invoke();
    }
    public void EnemyAction15()
    {
        OnTestAction15?.Invoke();
    }
}
