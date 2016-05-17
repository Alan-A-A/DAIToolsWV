using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using DAILibWV.Frostbite;

namespace DAILibWV
{
    //TODO move compression/decompression functions to another class or use some framework to do that.
    public static class Helpers
    {
        /// <summary>
        /// wrapper class representing <see cref="BinaryReader.Read7BitEncodedInt"/>
        /// </summary>
        public class BinaryReader7Bit : BinaryReader
        {
            public BinaryReader7Bit(Stream stream) : base(stream) { }
            public new int Read7BitEncodedInt()
            {
                return base.Read7BitEncodedInt();
            }
        }

        /// <summary>
        /// Wrapper class representing <see cref="BinaryWriter.Write7BitEncodedInt(int)"/>
        /// </summary>
        public class BinaryWriter7Bit : BinaryWriter
        {
            public BinaryWriter7Bit(Stream stream) : base(stream) { }
            public new void Write7BitEncodedInt(int i)
            {
                base.Write7BitEncodedInt(i);
            }
        }

        /// <summary>
        /// Deletes the given file if it exists using <see cref="File.Exists(string)"/>.
        /// </summary>
        /// <param name="file">The file path to use.</param>
        public static void DeleteFileIfExist(string file)
        {
            if (File.Exists(file))
                File.Delete(file);
        }

        /// <summary>
        /// Runs the given file with the given command (arguments), <see cref="System.Diagnostics.Process"/> and <see cref="System.Diagnostics.ProcessStartInfo"/>.
        /// </summary>
        /// <param name="file">The file to run.</param>
        /// <param name="command">The arguments to be passed to the file.</param>
        public static void RunShell(string file, string command)
        {
            Process process = new System.Diagnostics.Process();
            ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = file;
            startInfo.Arguments = command;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(file) + "\\";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Compares two given byte arrays.
        /// </summary>
        /// <returns>True if both arrays have same length and contain same bytes in same order, otherwise false.</returns>
        public static bool ByteArrayCompare(byte[] b1, byte[] b2)
        {
            if(b1 == null || b2 == null || b1.Length != b2.Length)
                return false;
            for (int i = 0; i < b1.Length; i++)
                if (b1[i] != b2[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Writes the given integer as bytes into the given stream object, <see cref="BitConverter.GetBytes(int)"/> and <see cref="Stream.Write(byte[], int, int)"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/></param>
        /// <param name="i">The integer to be written.</param>
        public static void WriteInt(Stream s, int i)
        {
            s.Write(BitConverter.GetBytes(i), 0, 4);
        }

        /// <summary>
        /// Same as <see cref="WriteInt(Stream, int)"/> but writes the integer in little endian byte order.
        /// </summary>
        public static void WriteLEInt(Stream s, int i)
        {
            List<byte> t = new List<byte>(BitConverter.GetBytes(i));
            t.Reverse();
            s.Write(t.ToArray(), 0, 4);
        }

        /// <summary>
        /// Same as <see cref="WriteInt(Stream, int)"/> but for uint.
        /// </summary>
        public static void WriteUInt(Stream s, uint i)
        {
            s.Write(BitConverter.GetBytes(i), 0, 4);
        }

        /// <summary>
        /// Same as <see cref="WriteLEInt(Stream, int)"/> but for uint.
        public static void WriteLEUInt(Stream s, uint i)
        {
            List<byte> t = new List<byte>(BitConverter.GetBytes(i));
            t.Reverse();
            s.Write(t.ToArray(), 0, 4);
        }

        /// <summary>
        /// Same as <see cref="WriteLEUInt(Stream, uint)"/> but for ushort.
        /// </summary>
        public static void WriteLEUShort(Stream s, ushort u)
        {
            byte[] buff = BitConverter.GetBytes(u);
            buff = buff.Reverse().ToArray();
            s.Write(buff, 0, 2);
        }

        /// <summary>
        /// Reads an integer from the given stream (4 bytes).
        /// </summary>
        public static int ReadInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToInt32(buff, 0);
        }

        /// <summary>
        /// Reads an uint from the given stream (4 bytes).
        /// </summary>
        public static uint ReadUInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToUInt32(buff, 0);
        }

        /// <summary>
        /// Reads a short from the given stream (2 bytes).
        /// </summary>
        public static short ReadShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            return BitConverter.ToInt16(buff, 0);
        }

        /// <summary>
        /// Reads an ushort from the given stream (2 bytes).
        /// </summary>
        public static ushort ReadUShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            return BitConverter.ToUInt16(buff, 0);
        }

        /// <summary>
        /// Reads a long from the given stream (8 bytes).
        /// </summary>
        public static long ReadLong(Stream s)
        {
            byte[] buff = new byte[8];
            s.Read(buff, 0, 8);
            return BitConverter.ToInt64(buff, 0);
        }

        /// <summary>
        /// Reads an ulong from the given stream (8 bytes).
        /// </summary>
        public static ulong ReadULong(Stream s)
        {
            byte[] buff = new byte[8];
            s.Read(buff, 0, 8);
            return BitConverter.ToUInt64(buff, 0);
        }

        /// <summary>
        /// Reads a float from the given stream (4 bytes), <see cref="BitConverter.ToSingle(byte[], int)"/>.
        /// </summary>
        public static float ReadFloat(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            return BitConverter.ToSingle(buff, 0);
        }

        /// <summary>
        /// Reads an integer in little endian byte order from the given stream.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static int ReadLEInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToInt32(buff, 0);
        }

        /// <summary>
        /// Same as <see cref="ReadLEInt(Stream)"/> but for uint.
        /// </summary>
        public static uint ReadLEUInt(Stream s)
        {
            byte[] buff = new byte[4];
            s.Read(buff, 0, 4);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToUInt32(buff, 0);
        }

        /// <summary>
        /// Same as <see cref="ReadLEInt(Stream)"/> but for short.
        /// </summary>
        public static short ReadLEShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToInt16(buff, 0);
        }

        /// <summary>
        /// Same as <see cref="ReadLEInt(Stream)"/> but for ushort.
        /// </summary>
        public static ushort ReadLEUShort(Stream s)
        {
            byte[] buff = new byte[2];
            s.Read(buff, 0, 2);
            buff = buff.Reverse().ToArray();
            return BitConverter.ToUInt16(buff, 0);
        }

        /// <summary>
        /// <see cref="Stream.Read(byte[], int, int)"/>.
        /// </summary>
        public static byte[] ReadFull(Stream s, uint size)
        {
            byte[] buff = new byte[size];
            int totalread = 0;
            while ((totalread += s.Read(buff, totalread, (int)(size - totalread))) < size) ;
            return buff;
        }

        /// <summary>
        /// Reads the stream byte by byte, <see cref="Stream.ReadByte"/>.
        /// </summary>
        /// <returns>String representing the char representation the read bytes.</returns>
        public static string ReadNullString(Stream s)
        {
            string res = "";
            byte b;
            while ((b = (byte)s.ReadByte()) > 0 && s.Position < s.Length) res += (char)b;
            return res;
        }

        /// <summary>
        /// writes the given string byte by byte into the given stream and appends 0 at the end.
        /// </summary>
        public static void WriteNullString(Stream s, string t)
        {
            foreach (char c in t)
                s.WriteByte((byte)c);
            s.WriteByte(0);
        }

        /// <summary>
        /// Reads LEB128 from the stream as ulong.
        /// </summary>
        public static ulong ReadLEB128(Stream s)
        {
            ulong result = 0;
            byte shift = 0;
            while (true)
            {
                int i = s.ReadByte();
                if (i == -1) return result;
                byte b = (byte)i;
                result |= (ulong)((b & 0x7f) << shift);
                if ((b >> 7) == 0)
                    return result;
                shift += 7;
            }
        }

        /// <summary>
        /// Writes the given integer as LEB128 into the given stream.
        /// </summary>
        public static void WriteLEB128(Stream s, int value)
        {
            int temp = value;
            while (temp != 0)
            {
                int val = (temp & 0x7f);
                temp >>= 7;

                if (temp > 0)
                    val |= 0x80;

                s.WriteByte((byte)val);
            }
        }

        /// <summary>
        /// TODO: is it a duplicate of <see cref="ByteArrayCompare(byte[], byte[])"/> ??
        /// </summary>
        public static bool MatchByteArray(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;
            return true;
        }

        /// <summary>
        /// Returns the byte array representation of the given hex string.
        /// </summary>
        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        /// <summary>
        /// Returns the byte array representation of the given string by casting each char in the string to a byte.
        /// </summary>
        public static byte[] StringAsByteArray(string str)
        {
            MemoryStream m = new MemoryStream();
            foreach (char c in str)
                m.WriteByte((byte)c);
            return m.ToArray();
        }

        /// <summary>
        /// Returns the hex string representation of the given byte array.
        /// </summary>
        /// <param name="start">The start index in the byte array.</param>
        /// <param name="len">The number of bytes to convert.</param>
        public static string ByteArrayToHexString(byte[] data, int start = 0, int len = 0)
        {
            if (data == null)
                data = new byte[0];
            StringBuilder sb = new StringBuilder();
            if (start == 0)
                foreach (byte b in data)
                    sb.Append(b.ToString("X2"));
            else
                if (start > 0 && start + len <= data.Length)
                    for (int i = start; i < start + len; i++)
                        sb.Append(data[i].ToString("X2"));
                else
                    return "";
            return sb.ToString();
        }

        /// <summary>
        /// Returns the string representation of the given byte array by casting each byte to char.
        /// </summary>
        public static string ByteArrayAsString(byte[] data)
        {
            if (data == null)
                data = new byte[0];
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
                sb.Append((char)b);
            return sb.ToString();
        }
        
        /// <summary>
        /// Returns a string with the given number (count) of empty spaces.
        /// </summary>
        public static string MakeTabs(int count)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
                sb.Append("  ");
            return sb.ToString();
        }

        public static int DecompressLZ77(byte[] input, byte[] output, out int decompressedLength)
        {
            int inputPos = 0, outputPos = 0;
            try
            {
                while (true)
                {

                    bool isLookback = true;
                    bool skipParseCopyLength = false;
                    int lookbackLength = 1;
                    int copyLength = 1;
                    byte copyLengthMask = 0;

                    byte code = input[inputPos++];
                    if (code < 0x10)
                    {
                        isLookback = false;
                        copyLength += 2;
                        copyLengthMask = 0x0f;
                    }
                    else if (code < 0x20)
                    {
                        copyLength += 1;
                        copyLengthMask = 0x07;
                        lookbackLength |= (code & 0x08) << 11;
                        lookbackLength += 0x3fff;
                    }
                    else if (code < 0x40)
                    {
                        copyLength += 1;
                        copyLengthMask = 0x1f;
                    }
                    else
                    {
                        skipParseCopyLength = true;
                        copyLength += code >> 5;
                        lookbackLength += (code >> 2) & 0x07;
                        lookbackLength += input[inputPos++] * 8;
                    }

                    if (!isLookback || !skipParseCopyLength)
                    {
                        if ((code & copyLengthMask) == 0)
                        {
                            byte nextCode;
                            for (nextCode = input[inputPos++]; nextCode == 0; nextCode = input[inputPos++])
                            {
                                copyLength += 0xff;
                            }
                            copyLength += nextCode + copyLengthMask;
                        }
                        else
                        {
                            copyLength += code & copyLengthMask;
                        }

                        if (isLookback)
                        {
                            int lookbackCode = input[inputPos++];
                            lookbackCode |= input[inputPos++] << 8;
                            if (code < 0x20 && (lookbackCode >> 2) == 0) break;
                            lookbackLength += lookbackCode >> 2;
                            code = (byte)lookbackCode;
                        }
                    }

                    if (isLookback)
                    {
                        int lookbackPos = outputPos - lookbackLength;
                        for (int i = 0; i < copyLength; ++i)
                        {
                            output[outputPos++] = output[lookbackPos++];
                        }
                        copyLength = code & 0x03;
                    }

                    for (int i = 0; i < copyLength; ++i)
                    {
                        output[outputPos++] = input[inputPos++];
                    }
                }
            }
            catch
            { }

            decompressedLength = outputPos;
            if (inputPos == input.Length) return 0;
            else return inputPos < input.Length ? -8 : -4;
        }

        public static byte[] DecompressZlib(byte[] input, int size)
        {
            byte[] result = new byte[size];
            InflaterInputStream zipStream = new InflaterInputStream(new MemoryStream(input));
            zipStream.Read(result, 0, size);
            zipStream.Flush();
            return result;
        }

        public static byte[] CompressZlib(byte[] input)
        {
            MemoryStream m = new MemoryStream();
            DeflaterOutputStream zipStream = new DeflaterOutputStream(m, new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(8));
            zipStream.Write(input, 0, input.Length);
            zipStream.Finish();
            return m.ToArray();
        }

        /// <summary>
        /// Calcualtes MD5 of the given file, <see cref="MD5"/>"/>
        /// </summary>
        public static byte[] ComputeHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(File.ReadAllBytes(filePath));
            }
        }

        /// <summary>
        /// Calculates the FNV hash. See http://www.isthe.com/chongo/tech/comp/fnv/ .
        /// </summary>
        public static int HashFNV1(string StrToHash, int hashseed = 5381, int hashprime = 33)
        {
            int Hash = hashseed;
            for (int i = 0; i < StrToHash.Length; i++)
            {
                byte b = (byte)StrToHash[i];
                Hash = (int)(Hash * hashprime) ^ b;
            }
            return Hash;
        }

        /// <summary>
        /// Decompiles LUA byte codes using an external program.
        /// </summary>
        public static string DecompileLUAC(byte[] data)
        {
            MemoryStream m = new MemoryStream(data);
            if (m.Length > 0x18)
            {
                uint magic = ReadUInt(m);
                if (magic == 0xe1850009)
                {
                    m.Seek(0x18, 0);
                    try
                    {
                        string name = ReadNullString(m);
                        string clas = ReadNullString(m);
                        int len = (int)(m.Length - m.Position);
                        if (len > 0)
                        {
                            byte[] script = new byte[len];
                            m.Read(script, 0, len);
                            string basepath = Application.StartupPath + "\\ext\\luacdec\\";
                            File.WriteAllBytes(basepath + "temp.luac", script);
                            if (File.Exists(basepath + "temp.lua"))
                                File.Delete(basepath + "temp.lua");
                            Process p = new Process();
                            p.StartInfo.UseShellExecute = false;
                            p.StartInfo.CreateNoWindow = true;
                            p.StartInfo.RedirectStandardOutput = true;
                            p.StartInfo.FileName = basepath + "dec.bat";
                            p.StartInfo.WorkingDirectory = basepath;
                            p.Start();
                            string output = p.StandardOutput.ReadToEnd();
                            p.WaitForExit();
                            if (File.Exists(basepath + "temp.lua"))
                                return "Name: " + name + "\nClass: " + clas + "\n\nDecompilation:\n\n" + File.ReadAllText(basepath + "temp.lua");
                            else
                                return "";
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Retrieves the file's path without extension.
        /// </summary>
        public static string GetFileNameWithOutExtension(string path)
        {
            return Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Retrieves resource type as string by the given uint type from <see cref="ResTypes"/>.
        /// </summary>
        public static string GetResType(uint type)
        {
            if (ResTypes.ContainsKey(type))
                return ResTypes[type];
            else
                return "";
        }

        /// <summary>
        /// Helper for the waiting text...
        /// </summary>
        public static string GetWaiter(int x)
        {
            string waiter = @"_./*\._./*\._./";
            int sublen = 8;
            return waiter.Substring(x % (waiter.Length - sublen) , sublen);
        }
        
        //TODO complete documentation. Below seems to handle the TreeView in the GUI.
        public static TreeNode AddPath(TreeNode t, string path, string sha1, char splitter = '/')
        {
            string[] parts = path.Split(splitter);
            TreeNode f = null;
            foreach (TreeNode c in t.Nodes)
                if (c.Text == parts[0].ToLower())
                {
                    f = c;
                    break;
                }
            if (f == null)
            {
                f = new TreeNode(parts[0].ToLower());
                if (parts.Length == 1)
                    f.Name = sha1;
                else
                    f.Name = "";
                t.Nodes.Add(f);
            }
            if (parts.Length > 1)
            {
                string subpath = path.Substring(parts[0].Length + 1, path.Length - 1 - parts[0].Length);
                f = AddPath(f, subpath, sha1, splitter);
            }
            return t;
        }

        public static string GetPathFromNode(TreeNode t, string seperator = "\\")
        {
            string result = t.Text;
            if (t.Parent != null)
                result = GetPathFromNode(t.Parent, seperator) + seperator + result;
            return result;
        }

        public static void FillTreeFast(TreeView t, List<BJSON.Entry> lines)
        {
            t.Nodes.Clear();
            foreach (BJSON.Entry e in lines)
                t.Nodes.Add(BJSON.MakeEntry(new TreeNode(e.type.ToString("X")), e));
            Debug.LockWindowUpdate(t.Handle);
            Helpers.ExpandTreeByLevel(t.Nodes[0], 1);
            Debug.LockWindowUpdate(System.IntPtr.Zero);
            Application.DoEvents();
        }
        
        public static void ExpandTreeByLevel(TreeNode node, int level)
        {
            node.Expand();
            if (level > 0)
                foreach (TreeNode t in node.Nodes)
                    ExpandTreeByLevel(t, level - 1);
        }

        public static void SelectNext(string text, TreeView tree)
        {
            text = text.ToLower();
            TreeNode t = tree.SelectedNode;
            if (t == null && tree.Nodes.Count != 0)
                t = tree.Nodes[0];
            while (true)
            {
                TreeNode t2 = FindNext(t, text);
                if (t2 != null)
                {
                    tree.SelectedNode = t2;
                    return;
                }
                else if (t.NextNode != null)
                    t = t.NextNode;
                else if (t.Parent != null && t.Parent.NextNode != null)
                    t = t.Parent.NextNode;
                else if (t.Parent != null && t.Parent.NextNode == null)
                    while (t.Parent != null)
                    {
                        t = t.Parent;
                        if (t.Parent != null && t.Parent.NextNode != null)
                        {
                            t = t.Parent.NextNode;
                            break;
                        }
                    }
                else
                    return;
                if (t.Text.Contains(text))
                {
                    tree.SelectedNode = t;
                    return;
                }
            }
        }

        public static TreeNode FindNext(TreeNode t, string text)
        {
            foreach (TreeNode t2 in t.Nodes)
            {
                if (t2.Text.ToLower().Contains(text))
                    return t2;
                if (t2.Nodes.Count != 0)
                {
                    TreeNode t3 = FindNext(t2, text);
                    if (t3 != null)
                        return t3;
                }
            }
            return null;
        }


        /// <summary>
        /// Returns part of the path starting from the given folder index (start).
        /// </summary>
        public static string SkipSubFolder(string path, int start)
        {
            string[] parts = path.Split('\\');
            StringBuilder sb = new StringBuilder();
            for (int i = start; i < parts.Length - 1; i++)
                sb.Append(parts[i] + "\\");
            sb.Append(parts[parts.Length - 1]);
            return sb.ToString();
        }

        public static Dictionary<uint, string> ResTypes = new Dictionary<uint, string>()
        #region data
        {
            {0x5c4954a6, ".itexture"},
            {0x2d47a5ff, ".gfx"},
            {0x22fe8ac8, ""},
            {0x6bb6d7d2, ".streamingstub"},
            {0x1ca38e06, ""},
            {0x15e1f32e, ""},
            {0x4864737b, ".hkdestruction"},
            {0x91043f65, ".hknondestruction"},
            {0x51a3c853, ".ant"},
            {0xd070eed1, ".animtrackdata"},
            {0x319d8cd0, ".ragdoll"},
            {0x49b156d4, ".mesh"},
            {0x30b4a553, ".occludermesh"},
            {0x5bdfdefe, ".lightingsystem"},
            {0x70c5cb3e, ".enlighten"},
            {0xe156af73, ".probeset"},
            {0x7aefc446, ".staticenlighten"},
            {0x59ceeb57, ".shaderdatabase"},
            {0x36f3f2c0, ".shaderdb"},
            {0x10f0e5a1, ".shaderprogramdb"},
            {0xc6dbee07, ".mohwspecific"},
            {0xafecb022, ".luac"},
            {0x59c79990, ".facefx"},
            {0x1091c8c5, ".morphtargets"},
            {0xe36f0d59, ".clothasset"},
            {0x24a019cc, ".material"},
            {0x5e862e05, ".talktable"},
            {0x957c32b1, ".alttexture"},
            {0x76742dc8, ".delayloadbundles"},
            {0xa23e75db, ".layercombinations"},
            {0xc6cd3286, ".static"},
            {0xeb228507, ".headmoprh"},
            {0xefc70728, ".zs"}
        };
#endregion
    }
}
