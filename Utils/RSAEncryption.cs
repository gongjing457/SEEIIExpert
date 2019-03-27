using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SEEIPro.Utils
{
    public class RSAEncryption
    {
        #region RSA Encryption&Decryption



        #region RSA private key generator

        /// <summary>
        /// RSA key generator
        /// </summary>
        /// <param name="xmlKeys"></param>
        /// <param name="xmlPublicKey"></param>
        public void RSAKey(out string xmlKeys, out string xmlPublicKey)
        {
            var rsa = new RSACryptoServiceProvider(1024);
          
            xmlKeys = rsa.ToXmlString(true);
            xmlPublicKey = rsa.ToXmlString(false);
        }

        #endregion



        #region RSA encryption        

        public string RSAEncrypt(string xmlPublicKey, string m_strEncryptString)

        {



            byte[] PlainTextBArray;

            byte[] CypherTextBArray;

            string Result;

            var rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPublicKey);

            PlainTextBArray = (new UnicodeEncoding()).GetBytes(m_strEncryptString);

            CypherTextBArray = rsa.Encrypt(PlainTextBArray, false);

            Result = Convert.ToBase64String(CypherTextBArray);

            return Result;



        }

        public string RSAEncrypt(string xmlPublicKey, byte[] encryptString)

        {



            byte[] CypherTextBArray;

            string Result;

            var rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPublicKey);

            CypherTextBArray = rsa.Encrypt(encryptString, false);

            Result = Convert.ToBase64String(CypherTextBArray);

            return Result;



        }

        #endregion



        #region RSA decryption


        public static string RSADecrypt(string xmlPrivateKey, string m_strDecryptString)

        {

            byte[] PlainTextBArray;

            byte[] DypherTextBArray;

            string Result;

            var rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPrivateKey);

            PlainTextBArray = Convert.FromBase64String(m_strDecryptString);

            DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);

            Result = (new ASCIIEncoding()).GetString(DypherTextBArray);

            return Result;



        }

        public static string RSADecrypt(string xmlPrivateKey, byte[] decryptString)

        {

            byte[] DypherTextBArray;

            string Result;

            var rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(xmlPrivateKey);

            DypherTextBArray = rsa.Decrypt(decryptString, false);

            Result = (new UnicodeEncoding()).GetString(DypherTextBArray);

            return Result;



        }

        #endregion

        #endregion





        #region RSA Digital Signature



        #region GetHashDescription

        public bool GetHash(string m_strSource, ref byte[] hashData)

        {

            byte[] Buffer;

            var MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");

            Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(m_strSource);

            hashData = MD5.ComputeHash(Buffer);



            return true;

        }





        public bool GetHash(string m_strSource, ref string strHashData)

        {



            //Get Hash Description from String

            byte[] Buffer;

            byte[] HashData;

            var MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");

            Buffer = System.Text.Encoding.GetEncoding("GB2312").GetBytes(m_strSource);

            HashData = MD5.ComputeHash(Buffer);



            strHashData = Convert.ToBase64String(HashData);

            return true;



        }



        public bool GetHash(System.IO.FileStream objFile, ref byte[] hashData)

        {



            //Get Hash Description from file

            var MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");

            hashData = MD5.ComputeHash(objFile);

            objFile.Close();



            return true;



        }





        public bool GetHash(System.IO.FileStream objFile, ref string strHashData)

        {



            //Get Hash Description from file

            byte[] HashData;

            var MD5 = System.Security.Cryptography.HashAlgorithm.Create("MD5");

            HashData = MD5.ComputeHash(objFile);

            objFile.Close();



            strHashData = Convert.ToBase64String(HashData);



            return true;



        }

        #endregion



        #region RSA signature

        public bool SignatureFormatter(string p_strKeyPrivate, byte[] hashbyteSignature, ref byte[] encryptedSignatureData)

        {



            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPrivate);

            var RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);

            RSAFormatter.SetHashAlgorithm("MD5");

            encryptedSignatureData = RSAFormatter.CreateSignature(hashbyteSignature);

            return true;



        }





        public bool SignatureFormatter(string p_strKeyPrivate, byte[] hashbyteSignature, ref string m_strEncryptedSignatureData)

        {



            byte[] EncryptedSignatureData;



            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPrivate);

            var RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);



            RSAFormatter.SetHashAlgorithm("MD5");



            EncryptedSignatureData = RSAFormatter.CreateSignature(hashbyteSignature);



            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);



            return true;



        }





        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref byte[] encryptedSignatureData)

        {



            byte[] HashbyteSignature;



            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);

            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPrivate);

            var RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);



            RSAFormatter.SetHashAlgorithm("MD5");



            encryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);



            return true;



        }



        public bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)

        {



            byte[] HashbyteSignature;

            byte[] EncryptedSignatureData;



            HashbyteSignature = Convert.FromBase64String(m_strHashbyteSignature);

            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPrivate);

            var RSAFormatter = new System.Security.Cryptography.RSAPKCS1SignatureFormatter(RSA);



            RSAFormatter.SetHashAlgorithm("MD5");



            EncryptedSignatureData = RSAFormatter.CreateSignature(HashbyteSignature);



            m_strEncryptedSignatureData = Convert.ToBase64String(EncryptedSignatureData);



            return true;



        }

        #endregion



        #region RSA Signature



        public bool SignatureDeformatter(string p_strKeyPublic, byte[] hashbyteDeformatter, byte[] deformatterData)

        {



            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPublic);

            var RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);

            RSADeformatter.SetHashAlgorithm("MD5");



            if (RSADeformatter.VerifySignature(hashbyteDeformatter, deformatterData))

            {

                return true;

            }

            else

            {

                return false;

            }



        }



        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, byte[] deformatterData)

        {



            byte[] HashbyteDeformatter;



            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);



            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPublic);

            var RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);

            RSADeformatter.SetHashAlgorithm("MD5");



            if (RSADeformatter.VerifySignature(HashbyteDeformatter, deformatterData))

            {

                return true;

            }

            else

            {

                return false;

            }



        }



        public bool SignatureDeformatter(string p_strKeyPublic, byte[] hashbyteDeformatter, string p_strDeformatterData)

        {



            byte[] DeformatterData;



            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPublic);

            var RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);

            RSADeformatter.SetHashAlgorithm("MD5");



            DeformatterData = Convert.FromBase64String(p_strDeformatterData);



            if (RSADeformatter.VerifySignature(hashbyteDeformatter, DeformatterData))

            {

                return true;

            }

            else

            {

                return false;

            }



        }



        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)

        {



            byte[] DeformatterData;

            byte[] HashbyteDeformatter;



            HashbyteDeformatter = Convert.FromBase64String(p_strHashbyteDeformatter);

            var RSA = new System.Security.Cryptography.RSACryptoServiceProvider();



            RSA.FromXmlString(p_strKeyPublic);

            var RSADeformatter = new System.Security.Cryptography.RSAPKCS1SignatureDeformatter(RSA);

            RSADeformatter.SetHashAlgorithm("MD5");



            DeformatterData = Convert.FromBase64String(p_strDeformatterData);



            if (RSADeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))

            {

                return true;

            }

            else

            {

                return false;

            }



        }





        #endregion





        #endregion
    }
}