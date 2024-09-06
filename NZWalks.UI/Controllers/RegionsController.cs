using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response =new List<RegionDto>();
            try
            {
                //get All regions from api
                //https://localhost:7266/api/regions/GetAll
                var client = httpClientFactory.CreateClient();
                var httpResponseMessage = await client.GetAsync("https://localhost:7266/api/regions/");
                httpResponseMessage.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());
            }
            catch (Exception ex)
            {

                throw;
            }
            return View(response);


        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            try
            {
                var client = httpClientFactory.CreateClient();
                var httpMessageRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://localhost:7266/api/regions/"),
                    Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
                };
                var responseMessage = await client.SendAsync(httpMessageRequest);
                responseMessage.EnsureSuccessStatusCode();
                var response = httpMessageRequest.Content.ReadFromJsonAsync<RegionDto>();

                if(response!=null)
                {
                    return RedirectToAction("Index", "Regions");
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View();


        }

        [HttpGet]

        public async Task<IActionResult> Edit(Guid Id)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7266/api/regions/{Id}");
            
            if(response!=null)
            {
                return View(response);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto region)
        {
            var client = httpClientFactory.CreateClient();

            var httpMessageRequest = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7266/api/regions/{region.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(region), Encoding.UTF8, "application/json")
            };
            var responseMessage = await client.SendAsync(httpMessageRequest);
            responseMessage.EnsureSuccessStatusCode();
            var response = httpMessageRequest.Content.ReadFromJsonAsync<RegionDto>();

            if (response != null)
            {
                return RedirectToAction("Index", "Regions");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var response = await client.DeleteAsync($"https://localhost:7266/api/regions/{Id}");
                response.EnsureSuccessStatusCode();

                if (response != null)
                {
                    return RedirectToAction("Index", "Regions");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            

            return View("Index");
        }
    }
}
