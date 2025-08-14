namespace EasyClaimsCore.API.Models.Requests
{
    public interface IBaseRequest
    {
        string pmcc { get; set; }
        string certificateId { get; set; }

    }
}