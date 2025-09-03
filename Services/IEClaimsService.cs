using EasyClaimsCore.API.Models.Requests;

namespace EasyClaimsCore.API.Services
{
    public interface IEClaimsService
    {
        Task<object> GetRestTokenAsync(TokenRequest request);
        Task<object> GetRestMemberPINAsync(MemberPinViewModelRest request);
        Task<object> SearchRestCaseRateAsync(CaseRateRestRequest request);
        Task<object> SearchServerDateTime(TokenCredentialsRequest request);
        Task<object> SearchDatabaseDateTime(TokenCredentialsRequest request);
        Task<object> GetRestDoctorPANAsync(DoctorPANRestRequest request);
        Task<object> CheckDoctorIfAccredited(DoctorAccreditationRestViewModel request);
        Task<object> FetchServerVersion(TokenCredentialsRequest request);
        Task<object> FindEmployer(EmployerRequestViewModel request);
        Task<object> FetchUploadedClaimsMap(UploadedClaimsMapRestRequest request);
        Task<object> FetchClaimStatus(ClaimStatusApiRequest request);
        Task<object> IsClaimEligibleAPI(EligibilityRequestViewModel request);
        Task<object> FetchVoucherDetails(VoucherRestRequest request);
        Task<object> EClaimsFileCheckAPI(CommonAPIRequest request);
        Task<object> IsESOAValid(CommonAPIRequest request);
        Task<object> AddRequiredApiDocument(RequiredApiDocumentRequest request);
        Task<object> GeneratePBEFPDF(PBEFRequest request);
        Task<object> EClaimsApiUpload(CommonAPIRequest request);
        Task<object> IsDRGValidAPI(DRGRequest request);
        Task<object> MockResponseAPI(MockRequest request);
        Task<object> MockDecryptResponseAPI(MockDecryptedRequest request);
        Task<object> MockEncryptResponseAPI(CommonAPIRequest request);

    }
}