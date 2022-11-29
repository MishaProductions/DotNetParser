using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp.Tests
{
    /// <summary>
    /// Anything to do with (u)Int 16/32/64 float/decimal classes
    /// </summary>
    public static class NumberTests
    {
        public static void Run()
        {
            //Test byte
            var byteStr = ((byte)255).ToString();
            if (byteStr == "255")
            {
                TestController.TestSuccess("Byte.ToString test");
            }
            else
            {
                //This also tests the concat function
                TestController.TestFail("Byte.ToString test fail, ToString returned " + byteStr + " when it should be 255");
            }
            //Test sbyte
            var sbyteStr = ((sbyte)-50).ToString();
            if (sbyteStr == "-50")
            {
                TestController.TestSuccess("SByte.ToString test");
            }
            else
            {
                TestController.TestFail("SByte.ToString test fail, ToString returned " + sbyteStr + " when it should be -50.");
            }
            //Test ushort
            var ushortStr = ((ushort)52).ToString();
            if (ushortStr == "52")
            {
                TestController.TestSuccess("UInt16.ToString test");
            }
            else
            {
                TestController.TestFail("UInt16.ToString test fail, ToString returned " + ushortStr + " when it should be 52.");
            }
            //Test short
            var shortStr = ((short)-5200).ToString();
            if (shortStr == "-5200")
            {
                TestController.TestSuccess("Int16.ToString test");
            }
            else
            {
                TestController.TestFail("Int16.ToString test fail, ToString returned " + shortStr + " when it should be -5200.");
            }
            //Test int
            var intStr = (-520000).ToString();
            if (intStr == "-520000")
            {
                TestController.TestSuccess("Int32.ToString test");
            }
            else
            {
                TestController.TestFail("Int32.ToString test fail, ToString returned " + intStr + " when it should be -520000.");
            }
            //Test uint
            var uintStr = uint.MaxValue.ToString();
            if (uintStr == "4294967295")
            {
                TestController.TestSuccess("UInt32.ToString test");
            }
            else
            {
                TestController.TestFail("UInt32.ToString test fail, ToString returned " + uintStr + " when it should be 4294967295.");
            }
            //Test ulong
            var ulongStr = ulong.MaxValue.ToString();
            if (ulongStr == "18446744073709551615")
            {
                TestController.TestSuccess("UInt64.ToString test");
            }
            else
            {
                TestController.TestFail("UInt64.ToString test fail, ToString returned " + ulongStr + " when it should be 18446744073709551615.");
            }

            //Test long
            var longStr = long.MinValue.ToString();
            if (longStr == "-9223372036854775808")
            {
                TestController.TestSuccess("Int64.ToString test");
            }
            else
            {
                TestController.TestFail("Int64.ToString test fail, ToString returned " + longStr + " when it should be -9223372036854775808.");
            }

            float fl = 0.1f;
            if (fl == 0.1f)
            {
                TestController.TestSuccess("Float is correct value");
            }
            else
            {
                TestController.TestFail("Float is incorrect value");
            }
            fl += 0.1f;
            if (fl == 0.2f)
            {
                TestController.TestSuccess("Float is correct value after adding .1");
            }
            else
            {
                TestController.TestFail("Float is incorrect value after adding .1");
            }
            fl -= 0.2f;
            if (fl == 0)
            {
                TestController.TestSuccess("Float is correct value after subtracting .2");
            }
            else
            {
                TestController.TestFail("Float is incorrect value after subtracting .2");
            }
            fl = 2.5f;
            fl *= 2;
            if (fl == 5)
            {
                TestController.TestSuccess("Float is correct value after multiplying 2.5f*2");
            }
            else
            {
                TestController.TestFail("Float is incorrect value after multiplying 2.5f*2");
            }
            fl /= 2.5f;
            if (fl == 2)
            {
                TestController.TestSuccess("Float is correct value after dividing by 2.5f");
            }
            else
            {
                TestController.TestFail("Float is incorrect value after dividing by 2.5f");
            }
            fl = 5;
            if ((int)fl == 5)
            {
                TestController.TestSuccess("float was 5 and correctly converted from float to int");
            }
            else
            {
                TestController.TestFail("float was not correctly converted to an int");
            }
            var h = 3;
            fl = h;
            if ((int)fl == 3)
            {
                TestController.TestSuccess("float was 3 and correctly converted from int to float");
            }
            else
            {
                TestController.TestFail("float was not correctly converted from an int");
            }
            int i = 7;
            TestController.TestAssert((i % 3) == 1, "int32 modulus");
            fl = 2.5f;
            TestController.TestAssert((fl % 1f) == 0.5f, "float32 modulus");
            i = 0x12345678;
            var i2 = 0x00ff00ff;
            TestController.TestAssert((i & i2) == 0x00340078, "int32 bitwise and");
            i = 0x12345678;
            i2 = 0x0f0f0f0f;
            TestController.TestAssert((i | i2) == 0x1f3f5f7f, "int32 bitwise or");
            i = 0x12345678;
            i2 = 0x0a0a0a0a;
            TestController.TestAssert((i ^ i2) == 0x183E5C72, "int32 bitwise xor");
            uint ui = 0x12345678;
            TestController.TestAssert(~ui == 0xEDCBA987, "uint32 bitwise not");
            i = 0x12345678;
            TestController.TestAssert((i >> 4) == 0x01234567, "int32 shift right");
            i = 0x12345678;
            TestController.TestAssert((i << 4) == 0x23456780, "int32 shift left");
            long l1 = 5, l2 = 3;
            TestController.TestAssert(l1 + l2 == 8L, "long addition");
            double d1 = 5, d2 = 3.5;
            TestController.TestAssert(d1 + d2 == 8.5, "double addition");
            short s1 = 5, s2 = 11;
            TestController.TestAssert(s1 + s2 == (short)16, "short addition");
            i = -100;
            TestController.TestAssert(-i == 100, "integer negation");
            d1 = -50.5;
            TestController.TestAssert(-d1 == 50.5, "double negation");
            //TestController.TestAssert(float.IsNegative(-1), "float.IsNegative");
            TestController.TestAssert(float.IsPositiveInfinity(float.PositiveInfinity), "float.IsPositiveInfinity");
            TestController.TestAssert(float.IsNegativeInfinity(float.NegativeInfinity), "float.IsNegativeInfinity");

            if (true.ToString() == "True")
            {
                TestController.TestSuccess("true.ToString() is true");
            }
            else
            {
                TestController.TestFail("true.toString != true");
            }
        }
    }
}
