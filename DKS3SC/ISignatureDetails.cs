using iTextSharp.text.pdf;

namespace DKS3SC
{
    public interface ISignatureDetails
    {
        public string? Reason { get; set; }

        public string? Location { get; set; }

        public string? Content { get; set; }

        public bool? HasLayers { get; set; }
        
        public bool Visible { get; }

        public void AddRender(PdfSignatureAppearance appearance);
    }
}
