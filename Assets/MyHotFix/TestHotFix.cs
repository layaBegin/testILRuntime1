using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MyHotFix 
{
    public class TestHotFix
    {
        private int id;

        public void test1()
            
        {
            this.id = 0;
            UnityEngine.Debug.Log("=====测试ILRuntime id："+ id);
        }

        public string test2()
        {
            string str = "更新脚本11：08";
            Debug.Log(str);
            return str;
        }
    }
}
