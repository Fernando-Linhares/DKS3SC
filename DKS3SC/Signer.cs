using System.Security.Cryptography.X509Certificates;

namespace DKS3SC;

/*
 * Author: Fernando Linhares Silvestre
 * Created at: 2022-12-12
 * Repository: https://github.com/Fernando-Linhares/DKS3SC
 * Dedication: I dedicate this code to the best song game "Dark Souls 3 Soul of Cinder"
 */

public class Signer
{
    public bool MultiThread = false;

    private List<X509Certificate2> _certs;

    public List<X509Certificate2> Certs
    {
        get
        {
            if (_certs is null)
            {
                _certs = new List<X509Certificate2>();
            }

            return _certs;
        }
        set
        {
            _certs = value;
        }
    }

    private X509Store _slot;

    private X509Store Slot
    {
        get
        {
            if(_slot is null)
            {
                _slot = new X509Store();
            }

            return _slot;
        }
        set
        {
            _slot = value;
        }
      }

    private IManager? _adapter;

    public IManager Adapter
    {
        get
        {
            if(_adapter is null)
                _adapter = new Manager(new SignatureDetails());

            return _adapter;
        }
        set
        {
            _adapter = value;
        }
    }

    public static IManager BuildManager(ISignatureDetails details)
    {
        return new Manager(details);
    }

    public string FileInput { get; set; }

    public string FileOutput { get; set; }

    public X509Certificate2 CertificateSelected { get; set; }

    public List<string> FilesInput { get; set; }

    public List<string> FilesOutput { get; set; }

    public Signer()
    {
        StoreCertificates();
    }

    public Signer(string fileInput, string fileOutput)
    {
        FileInput = fileInput;
        
        FileOutput = fileOutput;
        
        StoreCertificates();
    }

    public Signer(string fileInput)
    {
        FileInput = fileInput;

        FileOutput = fileInput;

        StoreCertificates();
    }

    public void StoreCertificates()
    {
        X509Store slot = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        slot.Open(OpenFlags.ReadWrite);

        Certs = new List<X509Certificate2>();

        foreach (X509Certificate2 localCert in slot.Certificates)
        {
            Certs.Add(localCert);
        }

        Slot = slot;
    }

    public void Apparence(ISignatureDetails signatureDetails)
    {
            Adapter = new Manager(signatureDetails);
    }

    public Dictionary<int, string> Certificates { get => CertificatesFromSlot();  }

    public Dictionary<int, string> CertificatesFromSlot()
    {
        if(_certs is null || _certs.Count == 0)
        {
            throw new ArgumentNullException("certs not found on device");
        }

        var certList = new Dictionary<int, string>();

        int id = 1;

        foreach(var localCert in _certs)
        {
            var chain = Adapter.BuildChain(localCert);

            string name = Adapter.CertName(chain[0]);

            certList.Add(id, name);

            id += 1;
        }

        return certList;
    }

    public void SelectCertificate(int id)
    {
        if (Certs is null || Certs.Count == 0)
        {
            throw new ArgumentNullException("certs not found on device");
        }

        int index = id - 1;

        CertificateSelected = Certs[index];
    }

    public void Sign()
    {
        try
        {

            Adapter.MultiThread = MultiThread;

            ValidateCertificate(CertificateSelected);

            Adapter.SignFile(FileInput, FileOutput, CertificateSelected);
        }
        catch (Exceptions.NotValidCertificateException exceptionCertificate)
        {
            throw exceptionCertificate;
        }
        catch (Exception exception)
        {
            throw exception?? new Exception(exception?.Message);
        }
    }

    public void SignAll()
    {
        try
        {
            ValidateCertificate(CertificateSelected);

            Adapter.SignAll(FilesInput, FilesOutput, CertificateSelected);
        }
        catch (Exceptions.NotValidCertificateException exceptionCertificate)
        {
            throw exceptionCertificate;
        }
        catch (Exception exception)
        {
            throw exception ?? new Exception(exception?.Message);
        }
    }

    public void ValidateCertificate(X509Certificate2 cetificate)
    {
        Adapter?.Validate(cetificate);
    }

    public Dictionary<string, string> Details()
    {
        return Adapter.Details();
    }

    ~Signer()
    {
        Certs.Clear();
        Slot.Close();
    }
}
