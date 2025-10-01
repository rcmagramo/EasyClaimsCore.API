namespace EasyClaimsCore.API.Models.Requests
{
    public class EligibilityRequestViewModel : IBaseRequest
    {
        public string pmcc { get; set; } = string.Empty;
        public string certificateId { get; set; } = string.Empty;
        public string token { get; set; } = string.Empty;
        public string hospitalCode { get; set; } = string.Empty;
        public string isForOPDHemodialysisClaim { get; set; } = string.Empty;
        public string memberPIN { get; set; } = string.Empty;
        public MemberBasicInformation memberBasicInformation { get; set; }
        public string patientIs { get; set; } = string.Empty;
        public string admissionDate { get; set; } = string.Empty;
        public string patientPIN { get; set; } = string.Empty;
        public PatientBasicInformation patientBasicInformation { get; set; }
        public string membershipType { get; set; }
        public string pEN { get; set; } = string.Empty;
        public string employerName { get; set; } = string.Empty;
        public string isFinal { get; set; } = string.Empty;
    }

    public class EligibilityRequestVM
    {
        public string hospitalCode { get; set; } = string.Empty;
        public string isForOPDHemodialysisClaim { get; set; } = string.Empty;
        public string memberPIN { get; set; } = string.Empty;
        public MemberBasicInformation memberBasicInformation { get; set; }
        public string patientIs { get; set; } = string.Empty;
        public string admissionDate { get; set; } = string.Empty;
        public string patientPIN { get; set; } = string.Empty;
        public PatientBasicInformation patientBasicInformation { get; set; }
        public string membershipType { get; set; }
        public string pEN { get; set; } = string.Empty;
        public string employerName { get; set; } = string.Empty;
        public string isFinal { get; set; } = string.Empty;
    }

    public class MemberBasicInformation
    {
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string maidenname { get; set; }
        public string suffix { get; set; }

        private string _sex;
        public string sex
        {
            get
            {
                switch (_sex)
                {
                    case "1":
                        return "M";
                    case "2":
                        return "F";
                    default:
                        return _sex;
                }
            }
            set
            {
                string normalized = value;
                if (normalized != null)
                {
                    normalized = normalized.Trim().ToUpper();
                }

                switch (normalized)
                {
                    case "1":
                    case "M":
                        _sex = "1";
                        break;
                    case "2":
                    case "F":
                        _sex = "2";
                        break;
                    default:
                        _sex = normalized;
                        break;
                }
            }
        }
        public string dateOfBirth { get; set; }
    }

    public class PatientBasicInformation
    {
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string maidenname { get; set; }
        public string suffix { get; set; }

        private string _sex;
        public string sex
        {
            get
            {
                switch (_sex)
                {
                    case "1":
                        return "M";
                    case "2":
                        return "F";
                    default:
                        return _sex;
                }
            }
            set
            {
                string normalized = value;
                if (normalized != null)
                {
                    normalized = normalized.Trim().ToUpper();
                }

                switch (normalized)
                {
                    case "1":
                    case "M":
                        _sex = "1";
                        break;
                    case "2":
                    case "F":
                        _sex = "2";
                        break;
                    default:
                        _sex = normalized;
                        break;
                }
            }
        }
        public string dateOfBirth { get; set; }
    }
}