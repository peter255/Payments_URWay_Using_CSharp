using System;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace Payments_URWay_Using_CSharp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await MainAsync();
            Console.ReadKey();
        }
        public static async Task MainAsync()
        {
            using (var client = new HttpClient())
            {

                string base_url = "https://payments-dev.urway-tech.com"; // please change this with production base url 
                string api_url = "/URWAYPGService/transaction/jsonProcess/JSONrequest"; // please change this with production api url 

                string your_terminalId = "";
                string your_password = "";
                string your_Secret_Key = "";

                string Order_ID = "";
                string Order_Amount = "";
                string Order_Currency = "SAR";

                string Customer_Mail = "mail@mail.com";
                string Redirect_Url = "http://localhost:5000";


                client.BaseAddress = new Uri(base_url);
                var myContent = new
                {
                    trackid = Order_ID,
                    terminalId = your_terminalId,
                    password = your_password,
                    action = 1,
                    merchantIp = "10.10.10.11",
                    country = "SA",
                    currency = Order_Currency,
                    amount = Order_Amount,
                    requestHash = getHashParam($"{Order_ID}|{your_terminalId}|{your_password}|{your_Secret_Key}|{Order_Amount}|{Order_Currency}"),
                    customerEmail = Customer_Mail,
                    udf1 = "",
                    udf2 = Redirect_Url,
                    udf3 = "EN",
                    udf4 = ""
                };

                var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(myContent));
                var byteContent = new ByteArrayContent(buffer);

                var result = await client.PostAsync(api_url, byteContent);
                string resultContent = await result.Content.ReadAsStringAsync();

                dynamic response = JsonConvert.DeserializeObject(resultContent);

                string Payment_url = response.targetUrl + "?" + "paymentid=" + response.payid; //IFrame Link
                Console.WriteLine(Payment_url);
            }
        }
        public static string getHashParam(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
