namespace ZoaIdsBackend.Models;

public class ArtccDocument
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string OriginalPdfUrl { get; set; }
    public string LocalRelativePdfUrl { get; set; }
    public ArtccDocumentType Type { get; set; }
    public DateOnly EffectiveDate { get; set; }
}

public enum ArtccDocumentType
{
    CentralPolicyStatement,
    StandardOperatingProcedures,
    LetterOfAgreement,
    Other
}