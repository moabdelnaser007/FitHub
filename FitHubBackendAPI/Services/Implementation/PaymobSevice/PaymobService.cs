using FitHubBackendAPI.DTOs.UserDTOs;
using FitHubBackendAPI.Entities;
using FitHubBackendAPI.Entities.Enums;
using FitHubBackendAPI.Entities.Models;
using FitHubBackendAPI.Repository.Interfaces;
using FitHubBackendAPI.Services.Implementation.UserServices;
using FitHubBackendAPI.Services.Interfaces.PaymobService;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace FitHubBackendAPI.Services.Implementation.PaymobSevice
{
    public class PaymobService : IPaymobService
    {
        
        private readonly PaymobSettings _settings;
        private readonly IConfiguration _config;
        private readonly IGenericRepository<User> _UserRepostory;
        private readonly IGenericRepository<UserCreditTransactions> _transactionRepo;
        private readonly IGenericRepository<FithubPlan> _planRepo;
        private readonly IGenericRepository<FithubUserPlan> _userPlanRepo;

        public PaymobService(
            IConfiguration config,
            IOptions<PaymobSettings> settings,
            IGenericRepository<User> UserRepostory,
            IGenericRepository<UserCreditTransactions> transactionRepo,
            IGenericRepository<FithubPlan> planRepo,
            IGenericRepository<FithubUserPlan> userPlanRepo)
        {
            
            _settings = settings.Value;
            _UserRepostory = UserRepostory;
            _transactionRepo = transactionRepo;
            _userPlanRepo = userPlanRepo;
            _planRepo = planRepo;
            _config = config;
        }
        public async Task<(string redirectionURL,UserCreditTransactions transcation)> CreatePaymentAsync(int userId, int planID,int TransactionId)
        {
            var plan = await _planRepo.GetByIdAsync(planID);
            var user = (await _UserRepostory.GetAsync(u => u.Id == userId)).FirstOrDefault();
            var http = new HttpClient();
            
            var transaction = await _transactionRepo.GetByIdAsync(TransactionId);
            if(transaction == null)
            {
                throw new Exception("Transaction Not Found");
            }
            int SpecialReference = RandomNumberGenerator.GetInt32(1000000, 9999999) + transaction.Id;


            var requestBody = new PaymobIntentionRequest
            {
                Amount = plan.Price*115,
                Currency = "EGP",
                Payment_Methods = new object[]
                {
                    5450006, // from env
                    "card"
                },
                 Items = new List<PaymobItem>
                {
                    new PaymobItem
                    {
                        name = plan.Name,
                        amount = plan.Price*115,
                        description = plan.Description,
                        quantity = 1
                    }
                },
                Billing_Data = new PaymobBillingData
                {
                    Apartment = "N/A",
                    First_Name = user.FullName,
                    Last_Name = "__",
                    Street = "N/A",
                    Building = "N/A",
                    Phone_Number = user.Phone,
                    Country = "EGY",
                    City = user.City,
                    Email = user.Email,
                    Floor = "N/A",
                    State = "N/A"
                },
                customer = new PaymobCustomer
                {
                    first_name = user.FullName,
                    last_name = "__",
                    email = user.Email
                },
                special_reference = SpecialReference,
                expiration = 3600,
                merchant_order_id = SpecialReference.ToString(),
                redirection_url = "http://localhost:5024/api/wallet/callback",
                notification_url = "https://truantly-palpitant-taunya.ngrok-free.dev/api/wallet/ServerCallback"
            };

            var json = JsonSerializer.Serialize(requestBody);
            var seretKey = _config["Paymob:SecretKey"] ?? "no data";
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://accept.paymob.com/v1/intention/");
            request.Headers.Add("Authorization", $"Token {seretKey}");
            request.Content= new StringContent(json, Encoding.UTF8, "application/json");

            
            var response = await http.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Paymob Request");
            }

            var result = await response.Content.ReadAsStringAsync();
            var resultJson = JsonDocument.Parse(result);
            string publicKey =  _config["Paymob:PublicKey"] ?? "no data";
            var clientSecret = resultJson.RootElement.GetProperty("client_secret").GetString();
            var paymentKey = resultJson.RootElement.GetProperty("payment_keys");
            var redirectUrl = $"https://accept.paymob.com/unifiedcheckout/?publicKey={publicKey}&clientSecret={clientSecret}";


            // Save Paymob identifiers
            transaction.ReferenceId = SpecialReference;

            _transactionRepo.Update(transaction);

            await _transactionRepo.SaveChangesAsync();
            return (redirectUrl,transaction);
            

        }
        public async Task<UserCreditTransactions> PaymentSuccess(int specialReference)
        {
            var Transaction = (await _transactionRepo
                        .FindAsync(t=>t.ReferenceId == specialReference)).FirstOrDefault()??
                        throw new Exception($"Payment Reference: {specialReference} Not Found");
            if(Transaction.UserId ==null)
            {
                throw new Exception("Corrupted Transaction");
            }
            Transaction.IsPaid = true;
            Transaction.PaidAt = DateTime.UtcNow;
            Transaction.Status = _TransactionStatus.COMPLETED;

            Transaction.CreditsAfter = (Transaction.CreditsBefore ?? 0) + (Transaction.CreditsChanged ?? 0);
            _transactionRepo.Update(Transaction);
            await _transactionRepo.SaveChangesAsync();
            return (Transaction);
        }
        public async Task<UserCreditTransactions> PaymentFailed(int specialReference)
        {
            
            var Transaction = (await _transactionRepo
                        .FindAsync(t => t.ReferenceId == specialReference)).FirstOrDefault() ??
                        throw new Exception($"Payment Reference: {specialReference} Not Found");
            Transaction.IsPaid = false;
            Transaction.PaidAt = DateTime.UtcNow;
            Transaction.Status = _TransactionStatus.FAILED;
            _transactionRepo.Update(Transaction);
            await _transactionRepo.SaveChangesAsync();
            return (Transaction);
        }
        public string ComputeHmacSHA512(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
