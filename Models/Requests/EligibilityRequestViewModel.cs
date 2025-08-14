namespace EasyClaimsCore.API.Models.Requests
{
    public class EligibilityRequestViewModel : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string HospitalCode { get; set; } = string.Empty;
        public string IsForOPDHemodialysisClaim { get; set; } = string.Empty;
        public string MemberPIN { get; set; } = string.Empty;
        public object? MemberBasicInformation { get; set; }
        public string PatientIs { get; set; } = string.Empty;
        public string AdmissionDate { get; set; } = string.Empty;
        public string PatientPIN { get; set; } = string.Empty;
        public object? PatientBasicInformation { get; set; }
        public object? MembershipType { get; set; }
        public string PEN { get; set; } = string.Empty;
        public string EmployerName { get; set; } = string.Empty;
        public string IsFinal { get; set; } = string.Empty;
    }

    public class EligibilityRequestVM
    {
        public string hospitalCode { get; set; } = string.Empty;
        public string isForOPDHemodialysisClaim { get; set; } = string.Empty;
        public string memberPIN { get; set; } = string.Empty;
        public object? memberBasicInformation { get; set; }
        public string patientIs { get; set; } = string.Empty;
        public string admissionDate { get; set; } = string.Empty;
        public string patientPIN { get; set; } = string.Empty;
        public object? patientBasicInformation { get; set; }
        public object? membershipType { get; set; }
        public string pEN { get; set; } = string.Empty;
        public string employerName { get; set; } = string.Empty;
        public string isFinal { get; set; } = string.Empty;
    }
}