namespace EasyClaimsCore.API.Models.Requests
{
    public class EmployerRequestViewModel : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public object? pen { get; set; }
        public object? employername { get; set; }
    }

    public class newEmployerRequestViewModel
    {
        public string philhealthno { get; set; } = string.Empty;
        public string employername { get; set; } = string.Empty;
    }
}