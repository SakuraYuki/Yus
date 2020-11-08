using System;
using System.Collections.Generic;
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

    /// <summary>文件加密模块</summary>
    public class YusFileEncrypt
    {
        /// <summary>
        /// 加密文件随机数生成
        /// </summary>
        protected RandomNumberGenerator rand = new RNGCryptoServiceProvider();

        /// <summary>加密标志，只有相同加密标志才可以加解密</summary>
        public ulong Tag { get; protected set; }

        /// <summary>文件加密分割大小，单位字节</summary>
        public uint BufferSize { get; protected set; }

        /// <summary>
        /// 使用加密标识构造
        /// </summary>
        /// <param name="tag">加密标志，只有相同加密标志才可以加解密，例如0xFC010203040506CF</param>
        /// <param name="bufferSize">文件加密分割大小，单位字节</param>
        public YusFileEncrypt(ulong tag, uint bufferSize = 128 * 1024)
        {
            Tag = tag;
            BufferSize = bufferSize;
        }

        /// <summary>
        /// 检验两个Byte数组是否相同
        /// </summary>
        /// <param name="b1">Byte数组</param>
        /// <param name="b2">Byte数组</param>
        /// <returns>true－相等</returns>
        protected bool CheckByteArrays(byte[] b1, byte[] b2)
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
        protected SymmetricAlgorithm CreateRijndael(string password, byte[] salt)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes(password, salt, "SHA256", 1000);

            SymmetricAlgorithm sma = Rijndael.Create();
            sma.KeySize = 256;
            sma.Key = pdb.GetBytes(32);
            sma.Padding = PaddingMode.PKCS7;
            return sma;
        }

        /// <summary>
        /// 生成指定长度的随机Byte数组
        /// </summary>
        /// <param name="count">Byte数组长度</param>
        /// <returns>随机Byte数组</returns>
        protected byte[] GenerateRandomBytes(int count)
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
        public void EncryptFile(string inFile, string outFile, string password)
        {
            using (FileStream fin = File.OpenRead(inFile), fout = File.OpenWrite(outFile))
            {
                long lSize = fin.Length; // 输入文件长度
                int size = (int)lSize;
                byte[] bytes = new byte[BufferSize]; // 缓存
                int read = -1; // 输入文件读取数量
                int value = 0;

                // 获取IV和salt
                byte[] IV = GenerateRandomBytes(16);
                byte[] salt = GenerateRandomBytes(16);

                // 创建加密对象
                SymmetricAlgorithm sma = CreateRijndael(password, salt);
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

                    bw.Write(Tag);

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
        public void DecryptFile(string inFile, string outFile, string password)
        {
            // 创建打开文件流
            using (FileStream fin = File.OpenRead(inFile), fout = File.OpenWrite(outFile))
            {
                int size = (int)fin.Length;
                byte[] bytes = new byte[BufferSize];
                int read = -1;
                int value = 0;
                int outValue = 0;

                byte[] IV = new byte[16];
                fin.Read(IV, 0, 16);
                byte[] salt = new byte[16];
                fin.Read(salt, 0, 16);

                SymmetricAlgorithm sma = CreateRijndael(password, salt);
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

                    if (Tag != tag)
                        throw new CryptoHelpException("文件被破坏");

                    long numReads = lSize / BufferSize;

                    long slack = lSize % BufferSize;

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
        /// <summary>实例缓存</summary>
        protected static Dictionary<string, YusStringEncrypt> _InstanceCache;

        /// <summary>加解密管理器</summary>
        public RijndaelManaged Manager { get; protected set; }
        /// <summary>加密器</summary>
        public ICryptoTransform Encryptor { get; protected set; }
        /// <summary>解密器</summary>
        public ICryptoTransform Decryptor { get; protected set; }

        #region 构造

        /// <summary>
        /// 使用指定的密钥和向量初始化
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="vi">向量</param>
        /// <param name="flag">唯一标识，如果传入标识则会将加密器缓存，获取时将使用缓存</param>
        /// <param name="setting">额外设置</param>
        public YusStringEncrypt(byte[] key, byte[] vi, string flag = null, YusStringEncryptSetting setting = null)
        {
            // 如果没传设置就使用默认设置
            if (setting == null)
            {
                setting = new YusStringEncryptSetting();
            }

            //加密解密是常用功能，预加载所需资源
            Manager = new RijndaelManaged
            {
                Mode = setting.Mode,
                Padding = setting.Padding,
                KeySize = setting.KeySize,
                BlockSize = setting.BlockSize,
                Key = key, //设定密钥
                IV = vi //设定向量
            };

            // 创建加密器
            Encryptor = Manager.CreateEncryptor();
            // 创建解密器
            Decryptor = Manager.CreateDecryptor();

            // 未设置Flag就不操作缓存
            if (flag.YusNullOrWhiteSpace()) return;

            // 已存在就替换
            if (_InstanceCache.ContainsKey(flag)) _InstanceCache[flag] = this;
            // 否则就新增
            else _InstanceCache.Add(flag, this);
        }

        #endregion

        #region 获取

        /// <param name="flag">唯一标识，尝试获取缓存，没有缓存返回空</param>
        public static YusStringEncrypt GetInstance(string flag)
        {
            if (flag.YusNullOrWhiteSpace()) return null;

            InitCache();

            if (_InstanceCache.ContainsKey(flag)) return _InstanceCache[flag];
            else return null;
        }

        /// <param name="key">密钥</param>
        /// <param name="vi">向量</param>
        /// <param name="flag">唯一标识，如果传入标识则会将加密器缓存，获取时将使用缓存</param>
        /// <param name="setting">额外设置</param>
        public static YusStringEncrypt GetInstance(byte[] key, byte[] vi, string flag = null, YusStringEncryptSetting setting = null)
        {
            var instance = GetInstance(flag);

            if (instance != null) return instance;

            return new YusStringEncrypt(key, vi, flag, setting);
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

            var cipherBytes = Encryptor.TransformFinalBlock(plainText, 0, plainText.Length);

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

            byte[] plainText = plainText = Decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

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

        #region 业务方法

        /// <summary>初始化缓存数据</summary>
        protected static void InitCache()
        {
            // 初始化缓存
            if (_InstanceCache == null) _InstanceCache = new Dictionary<string, YusStringEncrypt>();
        }

        #endregion
    }

    /// <summary>字符串加密额外设置</summary>
    public class YusStringEncryptSetting
    {
        /// <summary>对称算法的运算模式</summary>
        public CipherMode Mode { set; get; } = CipherMode.CBC;

        /// <summary>对称算法中使用的填充模式</summary>
        public PaddingMode Padding { set; get; } = PaddingMode.PKCS7;

        /// <summary>对称算法所用密钥的大小，单位：位</summary>
        public int KeySize { set; get; } = 128;

        /// <summary>加密操作的块大小，单位：位</summary>
        public int BlockSize { set; get; } = 128;
    }
}
