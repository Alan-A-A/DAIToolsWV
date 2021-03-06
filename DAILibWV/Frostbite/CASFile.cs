﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAILibWV.Frostbite
{
    public class CASFile
    {
        public string MyPath;
        public int casnumber;
        public struct CASEntry
        {
            public uint magic;
            public byte[] SHA1;
            public uint datasize;
            public uint padding;
            public uint ucsize;
            public uint csize;
            public uint unk1;
            public uint unk2;
            public byte[] data;
        }
        public CATFile cat;
        public List<int> Indexes;

        #region Static methods

        public static string GetCASFileName(uint casnumber)
        {
            return string.Format("cas_{0:d2}.cas", casnumber);
        }

        public static string GetCASFileName(string basepath, uint casnumber)
        {
            return string.Format("{0}cas_{1:d2}.cas", basepath, casnumber);
        }

        #endregion

        public CASFile(string path)
        {
            MyPath = path;
            string s = Path.GetFileNameWithoutExtension(MyPath);
            s = s.Substring(s.Length - 2, 2);
            casnumber = int.Parse(s);
        }

        public void SetCAT(CATFile Cat)
        {
            cat = Cat;
            Indexes = GetIndexes(cat);
        }

        public CASEntry ReadEntry(uint[] line, int maxsize = 0x7FFFFFFF)
        {
            return ReadEntry(line[5], line[6], maxsize);
        }

        public CASEntry ReadEntry(int index, int maxsize = 0x7FFFFFFF)
        {
            uint[] line = cat.lines[Indexes[index]];
            return ReadEntry(line[5], line[6], maxsize);
        }

        public CASEntry ReadEntry(uint offset, uint size_, int maxsize)
        {
            CASEntry result = new CASEntry();
            MemoryStream s = new MemoryStream(ReadBlock(offset - 0x20, size_ + 0x20));
            result.magic = Helpers.ReadUInt(s);
            result.SHA1 = new byte[20];
            s.Read(result.SHA1, 0, 20);
            result.datasize = Helpers.ReadUInt(s);
            result.padding = Helpers.ReadUInt(s);
            long totalread = 0;
            MemoryStream res = new MemoryStream();
            while (totalread < result.datasize && totalread < maxsize && totalread < size_)
            {
                int ucsize = Helpers.ReadLEInt(s);
                int csize = Helpers.ReadLEInt(s);
                totalread += 8;
                int size = csize & 0xFFFF;
                int type = (int)(csize >> 16);
                if (type == 0x270)
                {
                    byte[] buff = new byte[size];
                    s.Read(buff, 0, size);
                    byte[] r = Helpers.DecompressZlib(buff, ucsize);
                    res.Write(r, 0, r.Length);
                    totalread += size;
                }
                else
                {
                    byte[] buff = new byte[size];
                    s.Read(buff, 0, size);
                    res.Write(buff, 0, size);
                    totalread += size;
                }
            }
            result.data = res.ToArray();
            return result;
        }

        public byte[] ReadBlock(uint offset, uint size)
        {
            FileStream fs = new FileStream(MyPath, FileMode.Open, FileAccess.Read);
            fs.Seek(offset, 0);
            byte[] buff = Helpers.ReadFull(fs, size);
            fs.Close();
            return buff;
        }

        public List<int> GetIndexes(CATFile cat)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < cat.lines.Count; i++)
                if (cat.lines[i][7] == casnumber)
                    result.Add(i);
            return result;
        }

        public static byte[] MakeHeaderAndContainer(byte[] data)
        {
            MemoryStream m = new MemoryStream();
            int pos = 0;
            while (pos < data.Length)
            {
                if (data.Length - pos > 0xFFFF)
                {
                    Helpers.WriteLEInt(m, 0xFFFF);
                    m.WriteByte(0);
                    m.WriteByte(0x70);
                    m.WriteByte(0xFF);
                    m.WriteByte(0xFF);
                    m.Write(data, pos, 0xFFFF);
                    pos += 0xFFFF;
                }
                else
                {
                    int rest = data.Length - pos;
                    Helpers.WriteLEInt(m, rest);
                    m.WriteByte(0);
                    m.WriteByte(0x70);
                    m.WriteByte((byte)(rest >> 8));
                    m.WriteByte((byte)(rest & 0xFF));
                    m.Write(data, pos, rest);
                    pos += rest;
                }
            }
            MemoryStream result = new MemoryStream();
            Helpers.WriteUInt(result, 0xF00FCEFA);
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] Sha1= sha1.ComputeHash(m.ToArray());
            result.Write(Sha1, 0, 0x14);
            Helpers.WriteInt(result, (int)m.Length);
            Helpers.WriteUInt(result, 0);
            result.Write(m.ToArray(),0,(int)m.Length);
            return result.ToArray();
        }
    }
}
