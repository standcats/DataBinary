using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Linq.Expressions;
using System.IO;
namespace Test
{
    using StandCats;
    public class MockDataBinary : DataBinary<MockDataBinary>
    {
        [StandCatsCode(0)]
        public int Test { get; set; }
        [StandCatsCode(1)]
        public uint UIntTest { get; set; }
        [StandCatsCode(2)]
        public long LongTest { get; set; }
        [StandCatsCode(3)]
        public ulong ULongTest { get; set; }
        [StandCatsCode(4)]
        public float FloatTest { get; set; }
        [StandCatsCode(5)]
        public double DoubleTest { get; set; }

        [StandCatsCode(6)]
        public float[] FloatArray { get; set; }
        [StandCatsCode(7)]
        public MockDataBinary RecursiveTest { get; set; }

        [StandCatsCode(8)]
        public List<MockDataBinary> RecursiveListTest { get; set; }

        [StandCatsCode(9)]
        public MockDataBinary[] RecursiveArrayTest { get; set; }
        [StandCatsCode(10)]
        public Dictionary<byte,MockDataBinary> RecursiveByteDictionaryTest { get; set; }
        [StandCatsCode(11)]
        public Dictionary<int, MockDataBinary> RecursiveIntDictionaryTest { get; set; }
        [StandCatsCode(12)]
        public short ShotTest { get; set; }
        [StandCatsCode(13)]
        public ushort UShortTest { get; set; }

    }
    public class RecursiveIn : DataBinary<RecursiveIn>
    {
        [StandCatsCode(0)]
        public int Value { get; set; }
    }
    public class RecursiveArray : DataBinary<RecursiveArray>
    {
        [StandCatsCode(0)]
        public RecursiveIn[] Arrays {get;set;}        
    }
    [TestFixture]
    public class SerializeTest
    {
        [Test]
        public void Test()
        {

        }
        [Test]
        public void RecursiveArrayIn()
        {
            RecursiveArray v = new RecursiveArray()
            {
                Arrays = new RecursiveIn[] { new RecursiveIn() { Value = 123 } }
            };
            var dstdata = new byte[2026];
            int offset = 0;
            v.ToBinary(ref dstdata, ref offset);

            RecursiveArray dst  = new RecursiveArray();
            int dstoffset = 0;
            dst.ByBinary(ref dstdata, ref dstoffset);

            Assert.IsNotNull(dst.Arrays);
            Assert.AreEqual(123, dst.Arrays[0].Value);
        }

        public class MockData
        {
            [StandCatsCode(0)]
            public int Data { get; set; }
        }

        [Test]
        public void ExpressionTest()
        {
            //基本的プロパティ代入
            //MockDataのDataに対して代入するのをExpressionTreeで作成
            var target = Expression.Parameter(typeof(MockData),"target");
            var value = Expression.Parameter(typeof(int), "value");
            var left = Expression.Property(target, "Data");            
            var mockdata = new MockData();
            var lamda = Expression.Lambda<Action<MockData, int>>(Expression.Assign(left, value), target, value);


            var complie = lamda.Compile();        
            complie(mockdata, 2);
            Assert.AreEqual(2, mockdata.Data);                      
        }
        [Test]
        public void MemoryStreamTest()
        {
            var datas = new byte[4];
            using (MemoryStream m = new MemoryStream(datas))
            {
 

            }
            
        }


        [Test]
        public void ToBinaryTest()
        {
            MockDataBinary mockdata = new MockDataBinary();
            mockdata.Test = 101;
            mockdata.UIntTest = 102;
            mockdata.LongTest = 1003;
            mockdata.ULongTest = 10000004;
            mockdata.FloatTest = 10.04f;
            mockdata.DoubleTest = 10004.02;
            mockdata.FloatArray = new float[] { 2440f, 23f, 10.13f, 29.9f };
            mockdata.RecursiveTest = new MockDataBinary() { Test = 123 };
            mockdata.RecursiveArrayTest = new MockDataBinary[] { new MockDataBinary() { Test = 111 } };
            mockdata.RecursiveListTest = new List<MockDataBinary>(new MockDataBinary[] { new MockDataBinary() { Test = 400 } });
            mockdata.RecursiveByteDictionaryTest = new Dictionary<byte, MockDataBinary>()
            {
                { 102,new MockDataBinary() {Test=500 } },
            };
            mockdata.RecursiveIntDictionaryTest = new Dictionary<int, MockDataBinary>()
            {
                {100003,new MockDataBinary() {Test=600 } },
            };
            mockdata.ShotTest = 1002;
            mockdata.UShortTest = 1222;
            //Binary化テスト
            //自分が何者であるかをバイナリ化ー＞
            //http://neue.cc/2017/08/28_558.html オートマトンでの探索？
            //クラス名をネームスペースから文字列にしてDictionaryに保存してCreateInstanceをすぐさま呼び出せるようにする

            var bytes = new byte[1024];
            int tobinoffset = 0;
            mockdata.ToBinary(ref bytes,ref tobinoffset);
            MockDataBinary dstdata = new MockDataBinary();
            int bybinoffset = 0;            
            dstdata.ByBinary(ref bytes,ref bybinoffset);
            Assert.AreEqual(101, dstdata.Test);
            Assert.AreEqual(102, dstdata.UIntTest);
            Assert.AreEqual(1003, dstdata.LongTest);
            Assert.AreEqual(10000004, dstdata.ULongTest);
            Assert.AreEqual(10.04f, dstdata.FloatTest, 0.001f);
            Assert.AreEqual(10004.02, dstdata.DoubleTest, 0.001f);
            Assert.IsNotNull(dstdata.FloatArray);
            Assert.AreEqual(10.13f, dstdata.FloatArray[2], 0.001f);
            Assert.AreEqual(1002, dstdata.ShotTest);
            Assert.AreEqual(1222, dstdata.UShortTest);
            Assert.IsNotNull(dstdata.RecursiveTest);
            Assert.AreEqual(123, dstdata.RecursiveTest.Test, 123);
            Assert.IsNotNull(dstdata.RecursiveArrayTest);
            Assert.AreEqual(111, dstdata.RecursiveArrayTest[0].Test);
            Assert.IsNotNull(dstdata.RecursiveListTest);
            Assert.AreEqual(400, dstdata.RecursiveListTest[0].Test);
            Assert.IsNotNull(dstdata.RecursiveByteDictionaryTest);
            Assert.IsTrue(dstdata.RecursiveByteDictionaryTest.ContainsKey(102));
            Assert.AreEqual(500, dstdata.RecursiveByteDictionaryTest[102].Test);
            Assert.IsNotNull(dstdata.RecursiveIntDictionaryTest);
            Assert.IsTrue(dstdata.RecursiveIntDictionaryTest.ContainsKey(100003));
            Assert.AreEqual(600, dstdata.RecursiveIntDictionaryTest[100003].Test);   
        }
        [Test]
        public void BinaryShortCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToByteShort(12, ref bytes, ref offset);
            Assert.AreEqual(2, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12,BinaryUtil.ByByteShort(ref bytes, ref sourceoffset));

        }
        [Test]
        public void BinaryIntCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToByteInt(12, ref bytes, ref offset);
            Assert.AreEqual(4, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12, BinaryUtil.ByByteInt(ref bytes, ref sourceoffset));
            Assert.AreEqual(4, sourceoffset);
        }
        [Test]
        public void BinaryUIntCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToByteUInt(12, ref bytes, ref offset);
            Assert.AreEqual(4, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12, BinaryUtil.ByByteUInt(ref bytes, ref sourceoffset));
            Assert.AreEqual(4, sourceoffset);
        }
        [Test]
        public void BinaryULongCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToBytesULong(12, ref bytes, ref offset);
            Assert.AreEqual(8, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12, BinaryUtil.ByBytesULong(ref bytes, ref sourceoffset));
            Assert.AreEqual(8, sourceoffset);
        }
        [Test]
        public void BinaryLongCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToBytesLong(12, ref bytes, ref offset);
            Assert.AreEqual(8, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12, BinaryUtil.ByBytesLong(ref bytes, ref sourceoffset));
            Assert.AreEqual(8, sourceoffset);
        }
        [Test]
        public void BinaryFloatCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToBytesFloat(12.052f, ref bytes, ref offset);
            Assert.AreEqual(4, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12.052f, BinaryUtil.ByBytesFloat(ref bytes, ref sourceoffset),0.001f);
            Assert.AreEqual(4, sourceoffset);
        }
        [Test]
        public void BinaryDoubleCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            BinaryUtil.ToBytesDouble(12.052, ref bytes, ref offset);
            Assert.AreEqual(8, offset);
            int sourceoffset = 0;
            Assert.AreEqual(12.052f, BinaryUtil.ByBytesDouble(ref bytes, ref sourceoffset), 0.001f);
            Assert.AreEqual(8, sourceoffset);
        }

        [Test]
        public void BinaryFloatArrayCopy()
        {
            var bytes = new byte[1024];
            int offset = 0;
            var sourcefloatarray = new float[] { 1.04f, 1.335f, 203f };
            BinaryUtil.ToBytesFloatArray(ref sourcefloatarray, ref bytes,ref offset);
            Assert.AreEqual(13, offset);
            var dstfloatarray = new float[3];
            int readoffset = 0;
            BinaryUtil.ByBytesFloatArray(ref bytes, ref readoffset, ref dstfloatarray);
            Assert.AreEqual(13, readoffset);
            Assert.AreEqual(1.04f, dstfloatarray[0], 0.001f);
            float[] shortfloatarry = null;
            var shortbytearray = new byte[] { 125 };//フロートの長さだけ保持した配列
            int shortreadarrayoffset = 0;
            Assert.Throws<IOException>(() => BinaryUtil.ByBytesFloatArray(ref shortbytearray, ref shortreadarrayoffset, ref shortfloatarry), "Must Be IOException ");

        }


    }
}
