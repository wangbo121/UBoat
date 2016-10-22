using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections; // hashs
using System.Diagnostics; // stopwatch
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.IO.Ports;
using System.Drawing;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

using System.Linq;
using System.Reactive.Subjects;

namespace BoatGCS
{
    public class Comms
    {
        const int X25_INIT_CRC = 0xffff;
        const int X25_VALIDATE_CRC = 0xf0b8;

        public static ushort crc_accumulate(byte b, ushort crc)
        {
            unchecked
            {
                byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
                ch = (byte)(ch ^ (ch << 4));
                return (ushort)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
            }
        }

        public static ushort crc_calculate(byte[] pBuffer, int length)
        {
            if (length < 1)
            {
                return 0xffff;
            }
            // For a "message" of length bytes contained in the unsigned char array
            // pointed to by pBuffer, calculate the CRC
            // crcCalculate(unsigned char* pBuffer, int length, unsigned short* checkConst) < not needed

            ushort crcTmp;
            int i;

            crcTmp = X25_INIT_CRC;

            for (i = 1; i < length; i++) // skips header
            {
                crcTmp = crc_accumulate(pBuffer[i], crcTmp);
                //Console.WriteLine(crcTmp + " " + pBuffer[i] + " " + length);
            }

            return (crcTmp);
        }

    }

    public static class MyConverter
    {
        /// <summary>  
        /// 由结构体转换为byte数组  
        /// </summary>  
        public static byte[] StructToByte(object structObj, int _size)
        {
            byte[] bytes = new byte[_size];
            IntPtr structPtr = Marshal.AllocHGlobal(_size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, _size);
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }

        /// <summary>  
        /// 由byte数组转换为结构体  
        /// </summary>  
        public static object ByteToStruct(byte[] bytes, Type type)
        {
            int _size = Marshal.SizeOf(type);
            if (_size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = Marshal.AllocHGlobal(_size);
            Marshal.Copy(bytes, 0, structPtr, _size);
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
    }  

}
