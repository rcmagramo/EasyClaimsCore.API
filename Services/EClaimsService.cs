using Azure;
using EasyClaimsCore.API.Extensions;
using EasyClaimsCore.API.Models.DTOs;
using EasyClaimsCore.API.Models.Exceptions;
using EasyClaimsCore.API.Models.Requests;
using EasyClaimsCore.API.Models.Responses;
using EasyClaimsCore.API.Security.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace EasyClaimsCore.API.Services
{
    public class EClaimsService : IEClaimsService
    {
        private readonly IServiceExecutor _serviceExecutor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICryptoEngine _cryptoEngine;
        private readonly ITokenHandler _tokenHandler;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EClaimsService> _logger;
        private readonly string _restBaseUrl;
        private readonly string _cipherKey;
        private readonly string _euroCertificate;

        public EClaimsService(
            IServiceExecutor serviceExecutor,
            IHttpClientFactory httpClientFactory,
            ICryptoEngine cryptoEngine,
            ITokenHandler tokenHandler,
            IConfiguration configuration,
            ILogger<EClaimsService> logger)
        {
            _serviceExecutor = serviceExecutor;
            _httpClientFactory = httpClientFactory;
            _cryptoEngine = cryptoEngine;
            _tokenHandler = tokenHandler;
            _configuration = configuration;
            _logger = logger;
            _restBaseUrl = _configuration["PhilHealth:RestBaseUrl"] ?? "https://staging.philhealth.gov.ph/";
            _cipherKey = _configuration["PhilHealth:CipherKey"] ?? "PHilheaLthDuMmy311630";
            _euroCertificate = _configuration["PhilHealth:EuroCertificate"] ?? "CLAIMS-01-05-2025-00006";
        }

        public async Task<object> GetRestTokenAsync(TokenRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.GetRestToken,
                request,
                async (req) => await ExecuteTokenRequestAsync(req));
        }

        public async Task<object> GetRestMemberPINAsync(MemberPinViewModelRest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.GetRestMemberPIN,
                request,
                async (req) => await ExecuteMemberPINRequestAsync(req));
        }

        public async Task<object> SearchRestCaseRateAsync(CaseRateRestRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.SearchRestCaseRate,
                request,
                async (req) => await ExecuteCaseRateSearchAsync(req));
        }

        public async Task<object> SearchServerDateTime(TokenCredentialsRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.SearchServerDateTime,
                request,
                async (req) => await ExecuteServerDateTimeAsync(req));
        }

        public async Task<object> SearchDatabaseDateTime(TokenCredentialsRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.SearchDatabaseDateTime,
                request,
                async (req) => await ExecuteDatabaseDateTimeAsync(req));
        }

        public async Task<object> GetRestDoctorPANAsync(DoctorPANRestRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.GetRestDoctorPAN,
                request,
                async (req) => await ExecuteDoctorPANRequestAsync(req));
        }

        public async Task<object> CheckDoctorIfAccredited(DoctorAccreditationRestViewModel request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.CheckDoctorIfAccredited,
                request,
                async (req) => await ExecuteDoctorAccreditationCheckAsync(req));
        }

        public async Task<object> FetchServerVersion(TokenCredentialsRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.FetchServerVersion,
                request,
                async (req) => await ExecuteServerVersionAsync(req));
        }

        public async Task<object> FindEmployer(EmployerRequestViewModel request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.FindEmployer,
                request,
                async (req) => await ExecuteEmployerSearchAsync(req));
        }

        public async Task<object> FetchUploadedClaimsMap(UploadedClaimsMapRestRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.FetchUploadedClaimsMap,
                request,
                async (req) => await ExecuteUploadedClaimsMapAsync(req));
        }

        public async Task<object> FetchClaimStatus(ClaimStatusApiRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.FetchClaimStatus,
                request,
                async (req) => await ExecuteClaimStatusAsync(req));
        }

        public async Task<object> IsClaimEligibleAPI(EligibilityRequestViewModel request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.isClaimEligibleAPI,
                request,
                async (req) => await ExecuteEligibilityCheckAsync(req));
        }

        public async Task<object> FetchVoucherDetails(VoucherRestRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.FetchVoucherDetails,
                request,
                async (req) => await ExecuteVoucherDetailsAsync(req));
        }

        public async Task<object> EClaimsFileCheckAPI(CommonAPIRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.eClaimsFileCheckAPI,
                request,
                async (req) => await ExecuteEClaimsFileCheckAsync(req));
        }

        public async Task<object> IsESOAValid(CommonAPIRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.isEsoaValidAPI,
                request,
                async (req) => await ExecuteESOAValidationAsync(req));
        }

        public async Task<object> AddRequiredApiDocument(RequiredApiDocumentRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.addRequiredApiDocument,
                request,
                async (req) => await ExecuteAddRequiredDocumentAsync(req));
        }

        public async Task<object> GeneratePBEFPDF(PBEFRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.generatePBEFPDF,
                request,
                async (req) => await ExecuteGeneratePBEFAsync(req));
        }

        public async Task<object> EClaimsApiUpload(CommonAPIRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.eClaimsApiUpload,
                request,
                async (req) => await ExecuteClaimsUploadAsync(req),
                chargeableItems: GetClaimsCount(request.Xml));
        }

        public async Task<object> IsDRGValidAPI(DRGRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.isDRGValidAPI,
                request,
                async (req) => await ExecuteDRGValidationAsync(req));
        }

        public async Task<object> MockResponseAPI(MockRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.MockResponseAPI,
                request,
                async (req) => await ExecuteMockResponseAsync(req));
        }

        public async Task<object> MockDecryptResponseAPI(MockDecryptedRequest request)
        {
            return await _serviceExecutor.ExecuteServiceAsync(
                RequestName.MockDecryptResponseAPI,
                request,
                async (req) => await ExecuteMockDecryptResponseAsync(req));
        }

        // Implementation methods following the ExecuteRestRequest pattern

        private async Task<object> ExecuteTokenRequestAsync(TokenRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            var url = $"{_restBaseUrl}PHIC/Claims3.0/getToken";

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("accreditationNo", request.accreditationNo);
            httpClient.DefaultRequestHeaders.Add("softwareCertificateId", _euroCertificate);

            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenResponse>(content);
            }

            throw new ExternalApiException($"Token request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteMemberPINRequestAsync(MemberPinViewModelRest request)
        {
            var memberPin = new MemberPin
            {
                lastname = request.lastname ?? "",
                firstname = request.firstname ?? "",
                middlename = request.middlename ?? "",
                suffix = request.suffix ?? "",
                birthdate = request.birthdate ?? ""
            };

            var newtoken = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
            var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(JsonConvert.SerializeObject(memberPin), _cipherKey);
            ClearHeaders();
            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/getMemberPIN";
            var requestMessage = CreatePostRequestAsync(endpoint, newtoken, encryptedPayload);
            var response = await MakeSendRequestAsync(requestMessage);

            //var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            //requestMessage.Headers.Add("token", newtoken);
            //requestMessage.Content = new StringContent(
            //    encryptedPayload,
            //    Encoding.UTF8,
            //    "application/json"
            //);
            //Stopwatch stopwatch = null;
            //dynamic response = default(dynamic);
            //stopwatch = Stopwatch.StartNew();
            //response = await NewHttpClientFactory.Instance.SendAsync(requestMessage);
            //stopwatch.Stop();

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);
                XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(jsonData);

                return new
                {
                    Message = "Data has been successfully retrieved.",
                    Result = xmlDoc?.InnerText,
                    Success = true
                };
            }

            throw new ExternalApiException($"Member PIN request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteCaseRateSearchAsync(CaseRateRestRequest request)
        {
            var newtoken = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var payload = new
            {
                icdcode = request.icdcode ?? "",
                rvscode = request.rvscode ?? "",
                description = request.description ?? "",
                targetdate = request.targetdate ?? ""
            };

            var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(
                JsonConvert.SerializeObject(payload), _cipherKey);

            ClearHeaders();
            AddHeaders(new Dictionary<string, string> { { "token", newtoken } });

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/searchCaseRates";
            var response = await MakePostRequestAsync(endpoint, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Step 1: Decrypt
                var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);

                // Step 2: First deserialize to a dynamic object to remove escaping
                var unescapedObject = JsonConvert.DeserializeObject(jsonData);

                // Step 3: Convert the cleaned object back to JSON string
                var cleanedJson = JsonConvert.SerializeObject(unescapedObject);

                // Step 4: Deserialize into DTO (now the string is valid JSON)
                var dto = JsonConvert.DeserializeObject<ECaseRatesDto>(cleanedJson);

                var model = new ECaseRatesDto
                {
                    CaseRates = new List<CaseRateDto>()
                };

                foreach (var cr in dto.CaseRates)
                {
                    var caseRate = new CaseRateDto
                    {
                        CaseRateCode = cr.CaseRateCode,
                        CaseRateDescription = cr.CaseRateDescription,
                        ItemCode = cr.ItemCode,
                        ItemDescription = cr.ItemDescription,
                        EffectivityDate = cr.EffectivityDate,
                        Amounts = new List<AmountDto>()
                    };

                    foreach (var amt in cr.Amounts)
                    {
                        caseRate.Amounts.Add(new AmountDto
                        {
                            PrimaryHCIFee = amt.PrimaryHCIFee,
                            PrimaryProfFee = amt.PrimaryProfFee,
                            PrimaryCaseRate = amt.PrimaryCaseRate,
                            SecondaryHCIFee = amt.SecondaryHCIFee,
                            SecondaryProfFee = amt.SecondaryProfFee,
                            SecondaryCaseRate = amt.SecondaryCaseRate,
                            CheckFacilityH1 = amt.CheckFacilityH1,
                            CheckFacilityH2 = amt.CheckFacilityH2,
                            CheckFacilityH3 = amt.CheckFacilityH3,
                            CheckFacilityASC = amt.CheckFacilityASC,
                            CheckFacilityPCF = amt.CheckFacilityPCF,
                            CheckFacilityMAT = amt.CheckFacilityMAT,
                            CheckFacilityFSDC = amt.CheckFacilityFSDC
                        });
                    }

                    model.CaseRates.Add(caseRate);
                }

                return new
                {
                    Message = "Data has been successfully retrieved",
                    Result = model,
                    Success = true
                };
            }

            throw new ExternalApiException($"Case rate search failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteServerDateTimeAsync(TokenCredentialsRequest request)
        {
            var newtoken = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
            ClearHeaders();
            AddHeaders(new Dictionary<string, string> { { "token", newtoken } });
            var url = $"{_restBaseUrl}PHIC/Claims3.0/getServerDateTime";
            //var response = await httpClient.GetAsync(url);
            var response = await MakeGetRequestAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponseJson = await response.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(tokenResponseJson);   //change made on 02-24-2025 after PHIC made some eSOA validation fixing

                JsonDocument doc = JsonDocument.Parse(tokenResponseJson);
                JsonElement root = doc.RootElement;

                // Step 2: Extract the "result" string
                string resultJson = root.GetProperty("result").GetString();

                // Step 3: Parse the nested JSON
                JsonDocument innerDoc = JsonDocument.Parse(resultJson);
                JsonElement innerRoot = innerDoc.RootElement;

                // Step 4: Extract and convert the "datetime" value
                string datetimeStr = innerRoot.GetProperty("datetime").GetString();
                DateTime parsedDateTime = DateTime.ParseExact(datetimeStr, "MM/dd/yyyy hh:mm:ss tt", null);

                return new
                {
                    Message = "SearchServerDateTime successfully retrieved",
                    Result = datetimeStr,
                    Success = true
                };
            }
            throw new ExternalApiException($"Server datetime request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteDatabaseDateTimeAsync(TokenCredentialsRequest request)
        {
            var newtoken = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
            ClearHeaders();
            AddHeaders(new Dictionary<string, string> { { "token", newtoken } });
            var url = $"{_restBaseUrl}PHIC/Claims3.0/getDBServerDateTime";
            var response = await MakeGetRequestAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string tokenResponseJson = await response.Content.ReadAsStringAsync();

                // Deserialize outer JSON
                var jsonObject = JsonConvert.DeserializeObject<OuterRaw>(tokenResponseJson);

                // Deserialize the 'result' string into actual array
                var resultArray = JsonConvert.DeserializeObject<ServerInfo[]>(jsonObject.Result);

                // Return sanitized object
                var sanitizedResponse = new Outer
                {
                    Message = jsonObject.Message,
                    Result = resultArray,
                    Success = jsonObject.Success
                };

                return sanitizedResponse;
            }

            throw new ExternalApiException($"Database datetime request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteDoctorPANRequestAsync(DoctorPANRestRequest request)
        {
            try
            {
                var doctorPAN = new DocPANRequest
                {
                    lastname = request.lastname ?? "",
                    firstname = request.firstname ?? "",
                    middlename = request.middlename ?? "",
                    suffix = request.suffix ?? "",
                    birthdate = request.birthdate ?? ""
                };

                var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
                var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(
                    JsonConvert.SerializeObject(doctorPAN), _cipherKey);

                var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("token", token);

                var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/getDoctorPAN";
                var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = new StringContent(encryptedPayload, Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(requestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);
                    var xmlDoc = JsonConvert.DeserializeXmlNode(jsonData);

                    return new
                    {
                        Message = "Doctor PAN successfully retrieved.",
                        Result = xmlDoc?.InnerText,
                        Success = true
                    };
                }

                throw new ExternalApiException($"Doctor PAN request failed with status: {response.StatusCode}");
            }
            catch (Exception)
            {
                return new
                {
                    Message = "Unable to fetch Doctor PAN.",
                    Result = "Doctor PAN not found. Please verify name and birth date",
                    Success = false
                };
            }
        }

        private async Task<object> ExecuteDoctorAccreditationCheckAsync(DoctorAccreditationRestViewModel request)
        {
            var docAccred = new DoctorAccreditationVM
            {
                accrecode = request.accrecode ?? "",
                admissiondate = request.admissiondate ?? "",
                dischargedate = request.dischargedate ?? ""
            };

            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);
            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/isDoctorAccredited";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(docAccred),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponseJson = await response.Content.ReadAsStringAsync();
                var jsonData = _cryptoEngine.DecryptRestPayloadData(tokenResponseJson, _cipherKey);
                var model = JsonConvert.DeserializeObject<EAccreditationDto>(jsonData);
                return new
                {
                    Message = "Data has been successfully retrieved",
                    Result = model,
                    Success = true
                };
            }
            throw new ExternalApiException($"Doctor accreditation check failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteServerVersionAsync(TokenCredentialsRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);
            var url = $"{_restBaseUrl}PHIC/Claims3.0/getServerVersion";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<TokenResponse>(jsonData);
                return new
                {
                    Message = "Data has been successfully retrieved",
                    Result = model,
                    Success = true
                };
            }

            throw new ExternalApiException($"Server version request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteEmployerSearchAsync(EmployerRequestViewModel request)
        {
            var employerData = new newEmployerRequestViewModel
            {
                philhealthno = request.pen?.ToString() ?? "",
                employername = request.employername?.ToString()?.Trim('%') ?? ""
            };

            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/searchEmployer";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(employerData), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                // Step 1: Decrypt
                var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);

                // Step 2: Remove escaping
                var unescapedObject = JsonConvert.DeserializeObject(jsonData);

                // Step 3: Serialize to clean JSON
                var cleanedJson = JsonConvert.SerializeObject(unescapedObject);

                // Step 4: Deserialize into DTO
                var dto = JsonConvert.DeserializeObject<EmployersDto>(cleanedJson);

                // Step 5: Return JSON in CaseRate style
                return new
                {
                    Message = "",
                    Result = dto,
                    Success = true
                };

            }

            throw new ExternalApiException($"Employer search failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteUploadedClaimsMapAsync(UploadedClaimsMapRestRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/getUploadedClaimsMap?receiptTicketNumber={request.receiptTicketNumber}";
            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponseJson = await response.Content.ReadAsStringAsync();
                var jsonData = _cryptoEngine.DecryptRestPayloadData(tokenResponseJson, _cipherKey);
                var xmlDoc = JsonConvert.DeserializeXmlNode(jsonData, "eCONFIRMATION");

                return new
                {
                    Message = "",
                    Result = xmlDoc?.OuterXml,
                    Success = true
                };
            }

            throw new ExternalApiException($"Uploaded claims map request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteClaimStatusAsync(ClaimStatusApiRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/getClaimStatus?serieslhionos={request.serieslhionos}";
            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var xmlData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);

                return new
                {
                    Message = "",
                    Result = xmlData,
                    Success = true
                };
            }

            throw new ExternalApiException($"Claim status request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteEligibilityCheckAsync(EligibilityRequestViewModel request)
        {
            var _eligibility = new EligibilityRequestVM
            {
                hospitalCode = request.hospitalCode ?? "",
                isForOPDHemodialysisClaim = request.isForOPDHemodialysisClaim ?? "",
                memberPIN = request.memberPIN ?? "",
                memberBasicInformation = request.memberBasicInformation,
                patientIs = request.patientIs ?? "",
                admissionDate = request.admissionDate ?? "",
                patientPIN = request.patientPIN ?? "",
                patientBasicInformation = request.patientBasicInformation,
                membershipType = request.membershipType,
                pEN = request.pEN ?? "",
                employerName = request.employerName ?? "",
                isFinal = request.isFinal ?? ""
            };

            var newtoken = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);
            string jsonPayloadDataCE = JsonConvert.SerializeObject(_eligibility);
            var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(jsonPayloadDataCE, _cipherKey);

            ClearHeaders();
            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/isClaimEligible";
            var requestMessage = CreatePostRequestAsync(endpoint, newtoken, encryptedPayload);
            var response = await MakeSendRequestAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);
                var unescapedObject = JsonConvert.DeserializeObject(jsonData);
                var cleanedJson = JsonConvert.SerializeObject(unescapedObject);
                var dto = JsonConvert.DeserializeObject<IsClaimEligibleDto>(cleanedJson);

                return new
                {
                    Message = "",
                    Result = dto,
                    Success = true
                };
            }

            throw new ExternalApiException($"Eligibility check failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteVoucherDetailsAsync(VoucherRestRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/getVoucherDetails?voucherno={request.VoucherNo}";
            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var tokenResponseJson = await response.Content.ReadAsStringAsync();
                var jsonData = _cryptoEngine.DecryptRestPayloadData(tokenResponseJson, _cipherKey);

                if (jsonData.Contains("PhilhealthException"))
                {
                    return new
                    {
                        Message = jsonData.Trim(),
                        Result = jsonData.Trim(),
                        Success = true
                    };
                }
                else
                {
                    return new
                    {
                        Message = "",
                        Result = jsonData,
                        Success = true
                    };
                }
            }

            throw new ExternalApiException($"Voucher details request failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteEClaimsFileCheckAsync(CommonAPIRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            JObject jsonObj = JObject.Parse(JsonConvert.SerializeObject(new { Xml = request.Xml }));
            string xmlString = jsonObj["Xml"]?.ToString() ?? "";

            // Load into XmlDocument to validate formatting
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(xmlString, _cipherKey);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/eClaimsFileCheck";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(encryptedPayload, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var resultXmlDoc = JsonConvert.DeserializeXmlNode(responseContent);

                return new
                {
                    Message = "",
                    Result = resultXmlDoc?.InnerText,
                    Success = true
                };
            }

            throw new ExternalApiException($"eClaims file check failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteESOAValidationAsync(CommonAPIRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            JObject jsonObj = JObject.Parse(JsonConvert.SerializeObject(new { Xml = request.Xml }));
            string xmlString = jsonObj["Xml"]?.ToString() ?? "";
            var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(xmlString, _cipherKey);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/validateeSOA";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(encryptedPayload, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<dynamic>(responseContent);
                bool isSuccess = data?.success ?? false;

                if (isSuccess)
                {
                    return new
                    {
                        Message = "",
                        Result = true,
                        Success = true
                    };
                }
                //else
                //{
                //    var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);
                //    var errorData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData);

                //    return new
                //    {
                //        Result = errorData != null && errorData.TryGetValue("errors", out List<string>? value) && value.Count > 0
                //            ? string.Join("; ", value)
                //            : true,
                //        Success = false
                //    };
                //}
            }

            throw new ExternalApiException($"eSOA validation failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteAddRequiredDocumentAsync(RequiredApiDocumentRequest request)
        {
            var requestBody = new
            {
                pSeriesLhioNo = request.SeriesLhioNo,
                pXML = request.Xml
            };

            var sanitizedJson = SanitizeJson(JsonConvert.SerializeObject(requestBody));
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/addRequiredDocument";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(sanitizedJson, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);

                if (jsonData.Contains("Claim has been refiled"))
                {
                    return new
                    {
                        Message = "Successfully Refiled.",
                        Result = "Successfully Refiled.",
                        Success = true
                    };
                }
                else if (jsonData.Contains("PhilhealthException"))
                {
                    return new
                    {
                        Message = jsonData.Trim(),
                        Result = jsonData.Trim(),
                        Success = true
                    };
                }
                else
                {
                    return new
                    {
                        Message = "",
                        Result = jsonData,
                        Success = true
                    };
                }
            }

            throw new ExternalApiException($"Add required document failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteGeneratePBEFAsync(PBEFRequest request)
        {
            var requestData = new
            {
                accreno = request.pmcc ?? "",
                referenceno = request.referenceNumber ?? ""
            };

            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/generatePBEFPDF";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var base64String = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey).Trim();

                return new
                {
                    Message = "",
                    Result = base64String,
                    Success = true
                };
            }

            throw new ExternalApiException($"PBEF PDF generation failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteClaimsUploadAsync(CommonAPIRequest request)
        {
            // First validate the file if required (following ExecuteRestRequest pattern)
            var skipEClaimsFileCheck = _configuration.GetValue<bool>("PhilHealth:SkipEClaimsFileCheck", false);

            if (!skipEClaimsFileCheck)
            {
                var validationResult = await ExecuteEClaimsFileCheckAsync(request);
                if (validationResult is IDictionary<string, object> validationDict &&
                    validationDict.TryGetValue("Result", out var result))
                {
                    var isValid = result?.ToString()?.ToLower() == "true";
                    if (!isValid)
                    {
                        throw new FormatException("Error encountered on processing the XML data. Please check the contents of XML for manual validation against latest DTD.");
                    }
                }
            }

            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            JObject jsonObj = JObject.Parse(JsonConvert.SerializeObject(new { Xml = request.Xml }));
            string xmlString = jsonObj["Xml"]?.ToString() ?? "";
            var encryptedPayload = _cryptoEngine.EncryptXmlPayloadData(xmlString, _cipherKey);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/uploadeClaims";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(encryptedPayload, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var xmlData = _cryptoEngine.DecryptRestPayloadData(responseContent, _cipherKey);

                return new
                {
                    Message = "",
                    Result = xmlData,
                    Success = true
                };
            }

            throw new ExternalApiException($"eClaims upload failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteDRGValidationAsync(DRGRequest request)
        {
            var token = await _tokenHandler.MakeApiRequestAsync(request.pmcc, _euroCertificate);

            // Process CF5 XML
            var jsonPayloadDataDRG1 = JsonConvert.SerializeObject(request.CF5Xml);
            jsonPayloadDataDRG1 = jsonPayloadDataDRG1.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');
            var rawClaims1 = CleanAndValidateXml(jsonPayloadDataDRG1);
            var cleanedXml1 = Regex.Replace(rawClaims1, @"<\?xml.*?\?>\s*", "", RegexOptions.Singleline);
            var cf5 = _cryptoEngine.EncryptXmlPayloadData(cleanedXml1, _cipherKey);

            // Process eClaim XML
            var jsonPayloadDataDRG2 = JsonConvert.SerializeObject(request.eClaimXml);
            jsonPayloadDataDRG2 = jsonPayloadDataDRG2.Replace("\\\"", "\"").Replace("\\\\", "\\").Trim('"');
            var rawClaims = CleanAndValidateXml(jsonPayloadDataDRG2);
            var cleanedXml2 = Regex.Replace(rawClaims, @"<\?xml.*?\?>\s*", "", RegexOptions.Singleline);
            var eclaims = _cryptoEngine.EncryptXmlPayloadData(cleanedXml2, _cipherKey);

            var payload = new { cf5, eclaims };
            var encryptedPayloadDRG = JsonConvert.SerializeObject(payload);

            JObject jsonObject = JObject.Parse(encryptedPayloadDRG);
            foreach (var prop in jsonObject.Properties())
            {
                jsonObject[prop.Name] = (JToken?)JsonConvert.DeserializeObject(jsonObject[prop.Name]?.ToString() ?? "");
            }
            string cleanedJson = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);

            var httpClient = _httpClientFactory.CreateClient("EClaimsClient");
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("token", token);

            var endpoint = $"{_restBaseUrl}PHIC/Claims3.0/validateCF5";
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(cleanedJson, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(requestMessage);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<DRGApiResponse>(responseContent);
                var tempResult = JsonConvert.SerializeObject(apiResponse?.Result);
                var jsonData = _cryptoEngine.DecryptXmlPayloadData(tempResult, _cipherKey);
                var data = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);

                if (data != null && data.Count > 0)
                {
                    var errorsValue = data[0].errors?.ToString();

                    if (string.IsNullOrEmpty(errorsValue))
                    {
                        return new
                        {
                            Message = apiResponse?.Message,
                            Result = true,
                            Success = true
                        };
                    }
                    else
                    {
                        var description = GetEnumDescription(errorsValue);
                        return new
                        {
                            Result = $"Number : {errorsValue} : {description} : {apiResponse?.Message}",
                            Success = false
                        };
                    }
                }
            }

            throw new ExternalApiException($"DRG validation failed with status: {response.StatusCode}");
        }

        private async Task<object> ExecuteMockResponseAsync(MockRequest request)
        {
            // Check if mock responses are enabled
            var useMockResult = _configuration.GetValue<bool>("PhilHealth:UseMockResult", false);
            if (!useMockResult)
            {
                throw new System.MethodAccessException("Hospital is not allowed or registered to use this service call!");
            }

            // Generate mock response based on request type
            return new
            {
                Message = "Mock response generated successfully",
                Result = $"Mock data for {request.ClaimSeriesLHIO} - Refiled: {request.Refiled}, Processed: {request.Processed}",
                Success = true
            };
        }

        private async Task<object> ExecuteMockDecryptResponseAsync(MockDecryptedRequest request)
        {
            var Newrequest = new
            {
                result = new MockDecryptRequest
                {
                    docMimeType = request.docMimeType,
                    hash = request.hash,
                    key1 = request.key1,
                    key2 = request.key2,
                    iv = request.iv,
                    doc = request.doc
                }
            };
            string encryptedContent = JsonConvert.SerializeObject(Newrequest);
            var jsonData = _cryptoEngine.DecryptRestPayloadData(encryptedContent, _cipherKey);
            // Generate mock response based on request type
            return new
            {
                Message = "Decrypted mock response generated successfully",
                Result = jsonData,
                Success = true
            };
        }

        // Helper methods
        private int GetClaimsCount(string xml)
        {
            try
            {
                if (string.IsNullOrEmpty(xml)) return 0;

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                var claimNodes = xmlDoc.SelectNodes("//CLAIM");
                return claimNodes?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private static string SanitizeJson(string inputJson)
        {
            if (string.IsNullOrWhiteSpace(inputJson))
                return string.Empty;

            try
            {
                var inputData = JsonConvert.DeserializeObject<InputStructure>(inputJson);
                return JsonConvert.SerializeObject(inputData, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception)
            {
                return inputJson;
            }
        }

        private static string CleanAndValidateXml(string xml)
        {
            try
            {
                xml = xml.Trim();
                var xdoc = System.Xml.Linq.XDocument.Parse(xml);

                using var sw = new StringWriter();
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    Encoding = Encoding.UTF8
                };

                using var writer = XmlWriter.Create(sw, settings);
                xdoc.Save(writer);
                return sw.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing XML: {ex.Message}");
                return string.Empty;
            }
        }

        private static string GetEnumDescription(string errorCodes)
        {
            // Implementation of error code description mapping
            return errorCodes; // Simplified for now
        }

        private HttpRequestMessage CreatePostRequestAsync(string endpoint, string token, string encryptedPayload)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, endpoint);
            requestMessage.Headers.Add("token", token);
            requestMessage.Content = new StringContent(encryptedPayload, Encoding.UTF8, "application/json");
            return requestMessage;
        }

        private async Task<HttpResponseMessage> MakeSendRequestAsync(HttpRequestMessage requestMessage)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await NewHttpClientFactory.Instance.SendAsync(requestMessage);
            stopwatch.Stop();
            return response;
        }

        private async Task<HttpResponseMessage> MakePostRequestAsync(string url, object payload)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await NewHttpClientFactory.Instance.PostAsJsonAsync(url, payload);
            stopwatch.Stop();
            return response;
        }

        private async Task<HttpResponseMessage> MakeGetRequestAsync(string url)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await NewHttpClientFactory.Instance.GetAsync(url);
            stopwatch.Stop();
            return response;
        }

        private void AddHeaders(Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                NewHttpClientFactory.Instance.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        private void ClearHeaders()
        {
            NewHttpClientFactory.Instance.DefaultRequestHeaders.Clear();
        }




        //Helper models
        private class InputStructure
        {
            public string pSeriesLhioNo { get; set; } = string.Empty;
            public string pXML { get; set; } = string.Empty;
        }
        private class DRGApiResponse
        {
            public string Message { get; set; } = string.Empty;
            public object? Result { get; set; }
            public bool Success { get; set; }
        }
        public class Outer
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("result")]
            public ServerInfo[] Result { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }
        }
        private class OuterRaw
        {
            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("result")]
            public string Result { get; set; }

            [JsonProperty("success")]
            public bool Success { get; set; }
        }
        public class ServerInfo
        {
            [JsonProperty("server")]
            public string Server { get; set; }

            [JsonProperty("datetime")]
            public string Datetime { get; set; }

            [JsonProperty("remarks")]
            public string Remarks { get; set; }
        }
    }
}