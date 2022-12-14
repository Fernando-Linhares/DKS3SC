using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Certificate = System.Security.Cryptography.X509Certificates.X509Certificate2;
using Parser = Org.BouncyCastle.X509.X509CertificateParser;
using BouncyCert = Org.BouncyCastle.X509.X509Certificate;
using CryptoException = System.Security.Cryptography.CryptographicException;
using System.Threading;

namespace DKS3SC
{
    public class Manager : IManager
    {
        public bool MultiThread { get; set; }
        private ISignatureDetails _signatureDetails;

        private Parser _certParser;

        public Manager(ISignatureDetails signatureDetails)
        {
            _certParser = new Parser();
            _signatureDetails = signatureDetails;
        }

        public Dictionary<string, string> Details()
        {
           return new Dictionary<string, string>();
        }

        public void SignAll(List<string> filesinput, List<string> filesoutput, Certificate certificate)
        {
            for(int i = 0; i < filesinput.Count; i++)
            {
                SignFile(filesinput[i], filesoutput[i], certificate);
            }
        }

        public void SignFile(string fileinput, string fileoutput, Certificate certificate)
        {
            if(MultiThread)
            {
                var thread = new Thread(() => ProcessSignature(fileinput, fileoutput, certificate));
                
                thread.Start();
            }
            else
            {
                ProcessSignature(fileinput, fileoutput, certificate);
            }
        }

        public void ProcessSignature(string fileinput, string fileoutput, Certificate certificate)
        {
            BouncyCert[] chain = BuildChain(certificate);

            using var reader = new PdfReader(fileinput);

            using var stream = File.Open(fileoutput, FileMode.Create);

            using var stamper = PdfStamper.CreateSignature(reader, stream, '0', null, true);

            string name = CertName(chain[0]);

            PdfSignature directory = BuildCryptoDirectory(name);

            PdfSignatureAppearance apparence = BuildPreferences(stamper, chain[0], directory, name);

            IExternalSignature external = new Signature(certificate);

            Execute(apparence, external, chain);

            stream.Close();

            reader.Close();

            stamper.Close();
        }

        public BouncyCert[] BuildChain(Certificate certificate)
        {
            BouncyCert[] chain = new BouncyCert[1];

            chain[0] = _certParser.ReadCertificate(certificate.RawData);

            return chain;
        }

        public string CertName(BouncyCert certBouncy)
        {
            return CertificateInfo.GetSubjectFields(certBouncy).GetField("CN");
        }

        public PdfSignatureAppearance BuildPreferences(PdfStamper stamper, BouncyCert chain, PdfSignature directory, string  name)
        {
            var apparence = stamper.SignatureAppearance;

            apparence.Certificate = chain;

            apparence.CryptoDictionary = directory;

            apparence.Reason = _signatureDetails.Reason ?? "I Agree";

            apparence.Location = _signatureDetails.Location ?? "Brasil";

            apparence.Layer2Text = _signatureDetails.Content ?? $"Signed By {name} Date {apparence.SignDate} ";

            apparence.Acro6Layers = _signatureDetails.HasLayers ??  true;

            if (_signatureDetails.Visible)
            {
                _signatureDetails.Apply(apparence);
            }

            return apparence;
        }

        public PdfSignature BuildCryptoDirectory(string name)
        {
            var directory = new PdfSignature(PdfName.ADOBE_PPKLITE, PdfName.ADBE_PKCS7_DETACHED);

            directory.Name = name;

            directory.Date = new PdfDate(DateTime.Now);

            return directory;
        }

        public void Validate(Certificate cetificate)
        {
            try
            {
                if (!cetificate.Verify())
                    throw new Exceptions.NotValidCertificateException(cetificate);
            }
            catch(CryptoException exception)
            {
                throw exception;
            }
        }

        public void Execute(PdfSignatureAppearance appearance, IExternalSignature external, BouncyCert[] chain)
        {
            MakeSignature.SignDetached(appearance, external, chain, null, null, null, 0, CryptoStandard.CMS);
        }
    }
}
