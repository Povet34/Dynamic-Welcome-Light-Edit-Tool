using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestTest : MonoBehaviour
{
    public A testclass;

    List<A> testList;
    private void Start()
    {
        testList = new List<A>()
            {
                new A(0,10),
                new A(1,11),
                new A(2,12),
                new A(3,13),
                new A(4,14),
            };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            for(int i = 0; i < testList.Count; i++)
            {
                var a = testList[i];
                if(a.num == testclass.num)
                {
                    a.Update(testclass.order, testclass.num);
                }
            }
        }
    }

    [Serializable]
    public class A
    {
        public A(int order, int num) { this.order = order; this.num = num; }
        public int order; 
        public int num;

        public void Update(int order, int num)
        {
            this.order = order; this.num = num;
        }
    }
}
