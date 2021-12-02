using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHotFix
{
    class TestClass2
    {
        private int id;

        public void test1()

        {
            this.id = 0;
            UnityEngine.Debug.Log("=====测试ILRuntime id：" + id);
        }

        public void test2(int id)
        {
            this.id = id;
            UnityEngine.Debug.Log("=====测试ILRuntime id = " + id);
        }
    }
}
