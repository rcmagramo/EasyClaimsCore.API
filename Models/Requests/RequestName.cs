namespace EasyClaimsCore.API.Models.Requests
{
    public enum RequestName
    {
        GetRestToken = 1,
        GetRestMemberPIN = 2,
        SearchRestCaseRate = 3,
        SearchServerDateTime = 4,
        SearchDatabaseDateTime = 5,
        GetRestDoctorPAN = 6,
        CheckDoctorIfAccredited = 7,
        FetchServerVersion = 8,
        FindEmployer = 9,
        FetchUploadedClaimsMap = 10,
        FetchClaimStatus = 11,
        isClaimEligibleAPI = 12,
        eClaimsFileCheckAPI = 13,
        isEsoaValidAPI = 14,
        addRequiredApiDocument = 15,
        MockResponseAPI = 16,
        generatePBEFPDF = 17,
        eClaimsApiUpload = 18,
        FetchVoucherDetails = 19,
        isDRGValidAPI = 20,
        MockDecryptResponseAPI = 21
    }
}