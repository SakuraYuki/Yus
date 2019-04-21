using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Yus.Security
{
    /// <summary>
    /// 异常处理类
    /// </summary>
    public class CryptoHelpException : ApplicationException
    {
        /// <summary>
        /// 异常处理类构造器
        /// </summary>
        /// <param name="msg">异常消息</param>
        public CryptoHelpException(string msg) : base(msg) { }
    }

    /// <summary>
    /// CryptHelp
    /// </summary>
    public class YusFileEncrypt
    {
        private const ulong FC_TAG = 0xFC010203040506CF;

        private const int BUFFER_SIZE = 128 * 1024;

        /// <summary>
        /// 检验两个Byte数组是否相同
        /// </summary>
        /// <param name="b1">Byte数组</param>
        /// <param name="b2">Byte数组</param>
        /// <returns>true－相等</returns>
        private static bool CheckByteArrays(byte[] b1, byte[] b2)
        {
            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; ++i)
                {
                    if (b1[i] != b2[i])
                        return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 创建加密对象
        /// </summary>
        /// <param name="password">密码</param>
        /// <param name="salt"></param>
        /// <returns>加密对象</returns>
        private static SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);

            SymmetricAlgorithm sma = Rijndael.Create();
            sma.KeySize = 256;
            sma.Key = pdb.GetBytes(32);
            sma.Padding = PaddingMode.PKCS7;
            return sma;
        }

        /// <summary>
        /// 加密文件随机数生成
        /// </summary>
        private static RandomNumberGenerator rand = new RNGCryptoServiceProvider();

        /// <summary>
        /// 生成指定长度的随机Byte数组
        /// </summary>
        /// <param name="count">Byte数组长度</param>
        /// <returns>随机Byte数组</returns>
        private static byte[] GenerateRandomBytes(int count)
        {
            byte[] bytes = new byte[count];
            rand.GetBytes(bytes);
            return bytes;
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="inFile">待加密文件</param>
        /// <param name="outFile">加密后输入文件</param>
        /// <param name="password">加密密码</param>
        public static void EncryptFile(string inFile, string outFile, string password)
        {
            using (FileStream fin = File.OpenRead(inFile), fout = File.OpenWrite(outFile))
            {
                long lSize = fin.Length; // 输入文件长度
                int size = (int)lSize;
                byte[] bytes = new byte[BUFFER_SIZE]; // 缓存
                int read = -1; // 输入文件读取数量
                int value = 0;

                // 获取IV和salt
                byte[] IV = GenerateRandomBytes(16);
                byte[] salt = GenerateRandomBytes(16);

                // 创建加密对象
                SymmetricAlgorithm sma = YusFileEncrypt.CreateRijndael(password, salt);
                sma.IV = IV;

                // 在输出文件开始部分写入IV和salt
                fout.Write(IV, 0, IV.Length);
                fout.Write(salt, 0, salt.Length);

                // 创建散列加密
                HashAlgorithm hasher = SHA256.Create();
                using (CryptoStream cout = new CryptoStream(fout, sma.CreateEncryptor(), CryptoStreamMode.Write),
                    chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    BinaryWriter bw = new BinaryWriter(cout);
                    bw.Write(lSize);

                    bw.Write(FC_TAG);

                    // 读写字节块到加密流缓冲区
                    while ((read = fin.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        cout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                    }
                    // 关闭加密流
                    chash.Flush();
                    chash.Close();

                    // 读取散列
                    byte[] hash = hasher.Hash;

                    // 输入文件写入散列
                    cout.Write(hash, 0, hash.Length);

                    // 关闭文件流
                    cout.Flush();
                    cout.Close();
                }
            }
        }

        /// <summary>
        /// 解密文件
        /// </summary>
        /// <param name="inFile">待解密文件</param>
        /// <param name="outFile">解密后输出文件</param>
        /// <param name="password">解密密码</param>
        public static void DecryptFile(string inFile, string outFile, string password)
        {
            // 创建打开文件流
            using (FileStream fin = File.OpenRead(inFile), fout = File.OpenWrite(outFile))
            {
                int size = (int)fin.Length;
                byte[] bytes = new byte[BUFFER_SIZE];
                int read = -1;
                int value = 0;
                int outValue = 0;

                byte[] IV = new byte[16];
                fin.Read(IV, 0, 16);
                byte[] salt = new byte[16];
                fin.Read(salt, 0, 16);

                SymmetricAlgorithm sma = YusFileEncrypt.CreateRijndael(password, salt);
                sma.IV = IV;

                value = 32;
                long lSize = -1;

                // 创建散列对象, 校验文件
                HashAlgorithm hasher = SHA256.Create();

                using (CryptoStream cin = new CryptoStream(fin, sma.CreateDecryptor(), CryptoStreamMode.Read),
                    chash = new CryptoStream(Stream.Null, hasher, CryptoStreamMode.Write))
                {
                    // 读取文件长度
                    BinaryReader br = new BinaryReader(cin);
                    lSize = br.ReadInt64();
                    ulong tag = br.ReadUInt64();

                    if (FC_TAG != tag)
                        throw new CryptoHelpException("文件被破坏");

                    long numReads = lSize / BUFFER_SIZE;

                    long slack = (long)lSize % BUFFER_SIZE;

                    for (int i = 0; i < numReads; ++i)
                    {
                        read = cin.Read(bytes, 0, bytes.Length);
                        fout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                        outValue += read;
                    }

                    if (slack > 0)
                    {
                        read = cin.Read(bytes, 0, (int)slack);
                        fout.Write(bytes, 0, read);
                        chash.Write(bytes, 0, read);
                        value += read;
                        outValue += read;
                    }

                    chash.Flush();
                    chash.Close();

                    fout.Flush();
                    fout.Close();

                    byte[] curHash = hasher.Hash;

                    // 获取比较和旧的散列对象
                    byte[] oldHash = new byte[hasher.HashSize / 8];
                    read = cin.Read(oldHash, 0, oldHash.Length);
                    if ((oldHash.Length != read) || (!CheckByteArrays(oldHash, curHash)))
                        throw new CryptoHelpException("文件被破坏");
                }

                if (outValue != lSize)
                    throw new CryptoHelpException("文件大小不匹配");
            }
        }
    }

    /// <summary>字符串加密模块</summary>
    public class YusStringEncrypt
    {
        // string.Join(", ", "6927213255B4523C".Select(s=>"0x" + Convert.ToInt32(s)).ToArray()) // 南京华苏
        static byte[] key = { 0x54, 0x57, 0x50, 0x55, 0x50, 0x49, 0x51, 0x50, 0x53, 0x53, 0x66, 0x52, 0x53, 0x50, 0x51, 0x67 };
        // string.Join(", ", "54BBA15F83B26530".Select(s=>"0x" + Convert.ToInt32(s)).ToArray()) // NanjingHuasu
        static byte[] vi = { 0x53, 0x52, 0x66, 0x66, 0x65, 0x49, 0x53, 0x70, 0x56, 0x51, 0x66, 0x50, 0x54, 0x53, 0x51, 0x48 };

        static YusStringEncrypt instance;
        static RijndaelManaged rijndaelCipher;
        static ICryptoTransform encryptor;
        static ICryptoTransform decryptor;

        #region 构造、单例

        YusStringEncrypt()
        {
            //加密解密是常用功能，预加载所需资源
            rijndaelCipher = new RijndaelManaged
            {
                Mode = CipherMode.CBC, //设置对称算法的运算模式

                Padding = PaddingMode.PKCS7, //设置对称算法中使用的填充模式

                KeySize = 128, //设置对称算法所用密钥的大小，单位：位

                BlockSize = 128, //设置加密操作的块大小，单位：位

                Key = key, //设定密钥

                IV = vi //设定向量
            };

            encryptor = rijndaelCipher.CreateEncryptor(); //创建加密器

            decryptor = rijndaelCipher.CreateDecryptor(); //创建解密器
        }

        /// <summary>
        /// 获得单例已用于加密/解密数据
        /// </summary>
        /// <returns></returns>
        public static YusStringEncrypt Get()
        {
            if (instance == null || rijndaelCipher == null ||
                encryptor == null || decryptor == null)
            {
                instance = new YusStringEncrypt();
            }
            return instance;
        }

        #endregion

        #region AES128加密

        /// <summary>
        /// 有密码的AES加密，使用默认加密密钥和向量
        /// </summary>
        /// <param name="txt">需要加密的字符串</param>
        /// <returns>加密后的密文</returns>
        public string Encrypt(string txt)
        {
            //当传入空字符串或String.Empty时不加密，直接返回该字符串
            if (string.IsNullOrEmpty(txt))
                return txt;

            var plainText = Encoding.UTF8.GetBytes(txt);

            var cipherBytes = encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

            return Convert.ToBase64String(cipherBytes);
        }

        /// <summary>
        /// 有密码的AES加密，使用默认加密密钥和向量，直接将原字符串替换为加密后的字符串
        /// </summary>
        /// <param name="txt">需要加密的字符串，加密后原字符串将被替换为加密后的字符串</param>
        /// <returns>加密后的密文</returns>
        public string Encrypt(ref string txt)
        {
            return txt = Encrypt(txt);
        }

        /// <summary>
        /// AES解密,使用默认密匙和向量
        /// </summary>
        /// <param name="txt">密文</param>
        /// <exception cref="FormatException">当输入的密文格式不正确时抛出此异常，发生此异常时无法解密密文</exception>
        /// <exception cref="CryptographicException">//解密密文中遇到的错误，比如长度无效、无法解密之类的错误</exception>
        /// <returns>解密后的明文</returns>
        public string Decrypt(string txt)
        {
            //当传入空字符串或String.Empty时不加密，直接返回该字符串
            if (string.IsNullOrEmpty(txt))
                return txt;

            var encryptedData = Convert.FromBase64String(txt);

            byte[] plainText = plainText = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(plainText);
        }

        /// <summary>
        /// AES解密,使用默认密匙和向量，直接将密文替换为解密后的字符串
        /// </summary>
        /// <param name="txt">密文，解密后明文将直接替换原密文</param>
        /// <exception cref="FormatException">当输入的密文格式不正确时抛出此异常，发生此异常时无法解密密文</exception>
        /// <exception cref="CryptographicException">//解密密文中遇到的错误，比如长度无效、无法解密之类的错误</exception>
        /// <returns>解密后的明文</returns>
        public string Decrypt(ref string txt)
        {
            return txt = Decrypt(txt);
        }

        #endregion
    }
}
