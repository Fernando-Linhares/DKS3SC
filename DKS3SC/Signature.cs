using System.Security.Cryptography;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography.X509Certificates;
using Certificate = System.Security.Cryptography.X509Certificates.X509Certificate2;

namespace DKS3SC
{
    public class Signature : IExternalSignature
    {
        private Certificate _cert;

        public Signature(Certificate cert)
        {
            _cert = cert;
        }

        public virtual byte[] Sign(byte[] message)
        {
            byte[] signedData = SignatureComplex(message);

            if(signedData.Length == 0)
            {
                throw new Exception(
                    "Error on Sign data. Not found private key RSA in certificate selected. Certificate - " + _cert.Issuer
                );
            }

            return signedData;
        }

        public byte[] SignatureComplex(byte[]  message)
        {
            return _cert
               .GetRSAPrivateKey()?
               .SignData(
                   message,
                   HashAlgorithmName.SHA256,
                   RSASignaturePadding.Pkcs1
               )
               ??
               new byte[0];
        }

        public virtual string GetHashAlgorithm()
        {
            return "SHA-256";
        }

        public virtual string GetEncryptionAlgorithm()
        {
            return "RSA";
        }
    }
}
