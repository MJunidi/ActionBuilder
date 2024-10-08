using ActionBuilder.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using Labiba.Actions.Logger.Core.Models;
using ActionBuilder.Entities.Extentions;
using Labiba.Actions.Logger.Core.Repositories.Interfaces;
using Labiba.Actions.Logger.Core.Filters;
using System.Net;


namespace ActionBuilder.Controllers
{
    [Route("Home")]    
    public class HomeController : Controller
    {
        private readonly Stopwatch _timer;
        private readonly IHttpClientFactory _clientFactory;


        public HomeController(IHttpClientFactory clientFactory)
        {
            _timer = new Stopwatch();
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<HttpClientBaseResponse<T>> CallAsync<T>(HttpClientBaseRequest request, LogDetails logDetails)
      where T : new()
        {
            var result = new HttpClientBaseResponse<T>();
            try
            {
                using var client = _clientFactory.CreateClient(); 
                {
                    HttpRequestMessage apiRequest = new HttpRequestMessage
                    {
                        Method = request.Method,
                        RequestUri = new Uri(request.Url),
                    };

                    #region Headers
                    FillHeaders(request, apiRequest);
                    #endregion

                    #region Payload
                    if (request.Payload != null)
                        DeterminePayload(request, apiRequest);
                    #endregion
                    _timer.Restart();
                    var httpResult = await client.SendAsync(apiRequest);
                    _timer.Stop();
                    logDetails.APIExecutionTime = _timer.ElapsedMilliseconds;

                    logDetails.ResponseFromApi = JsonConvert.SerializeObject(DetermineResponseFromApi(httpResult.StatusCode));
            
                    var callContent = httpResult.Content.ReadAsStringAsync().Result;
                    if (string.IsNullOrEmpty(logDetails.ResponseFromApi))
                    {
                        logDetails.ResponseFromApi = JsonConvert.SerializeObject(callContent);
                    }
                    logDetails.APIExecutionTime = _timer.ElapsedMilliseconds;


                    result.IsSuccess = true;
                    result.Data = callContent;

                    return result;
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Errors = new List<string>
                {
                    ex.Message,
                };

            }
            return result;
        }

        private static string DetermineResponseFromApi(HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case System.Net.HttpStatusCode.Unauthorized:
                  return  $"{statusCode}API Returned Unauthorized";
                case System.Net.HttpStatusCode.BadRequest:
                   return $"{statusCode}API Returned BadRequest";
                case System.Net.HttpStatusCode.NotFound:
                   return $"{statusCode}API Returned NotFound";
                case System.Net.HttpStatusCode.NoContent:
                  return  $"{statusCode}API Returned NoContent";
                default:
                    return "";
            }
        }

        private static void DeterminePayload( HttpClientBaseRequest request, HttpRequestMessage apiRequest)
        {
            switch (request.Source)
            {
                case RequestDataSource.FromBody:
                    apiRequest.Content =
                        new StringContent(System.Text.Json.JsonSerializer.Serialize(request.Payload), Encoding.UTF8, "application/json");
                    break;
                case RequestDataSource.FromQueryString:
                    request.Url += "?" + request.Payload.GenerateQueryString();
                    apiRequest.RequestUri = new Uri($"{request.Url}");
                    break;
                case RequestDataSource.FromUrl:
                    request.Url += request.Payload.GenerateUrlParameter();
                    apiRequest.RequestUri = new Uri($"{request.Url}");
                    break;
            }
        }

        private static void FillHeaders(HttpClientBaseRequest request, HttpRequestMessage apiRequest)
        {
            foreach (var header in request.Headers)
            {
                apiRequest.Headers.Add(header.Key, header.Value);
            }
        }


        [HttpPost]
        [Route("SendReq")]
        [LogAction(ActionId = 10625093, ClientId = 6667429)]
        public async Task<IActionResult> SendReq(HttpClientBaseRequest request)
        {
            LogDetails logDetails = new();

            if(!ModelState.IsValid)
            {
                var response = await CallAsync<Object>(request, logDetails);

                if (response.IsSuccess)
                {
                    // Pass the status code to the view using ViewBag
                    ViewBag.StatusCode = "200";
                }
                else
                {
                    ViewBag.StatusCode = "Unknown"; // Fallback in case no status code is available
                }
                return View("Index");
            }
            else
            {
                ViewBag.StatusCode = "Date is wrong"; // Fallback in case no status code is available

                return View("Index");
            }
           

        }
    }
}
