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
    private List<X509Certificate2>? _certs;

    private X509Store _slot;

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

        _certs = new List<X509Certificate2>();

        foreach (X509Certificate2 localCert in slot.Certificates)
        {
            _certs.Add(localCert);
        }

        _slot = slot;
    }

    public void Apparence(ISignatureDetails signatureDetails)
    {
        if (_adapter is null)
            _adapter = new Manager(signatureDetails);
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
        if (_certs is null || _certs.Count == 0)
        {
            throw new ArgumentNullException("certs not found on device");
        }

        int index = id - 1;

        CertificateSelected = _certs[index];
    }

    public void Sign()
    {
        try
        {
            ValidateCertificate(CertificateSelected);

            Adapter.SignFile(FileInput, FileOutput, CertificateSelected);
        }
        catch (Exceptions.NotValidCertificateException exceptionCertificate)
        {
            throw exceptionCertificate;
        }
        catch (Exception exception)
        {
            throw exception;
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
            throw exception;
        }
    }

    public void ValidateCertificate(X509Certificate2 cetificate)
    {
        _adapter?.Validate(cetificate);
    }

    public Dictionary<string, string> Details()
    {
        return Adapter.Details();
    }

    ~Signer()
    {
        _certs = null;
        _slot.Close();
    }
}
