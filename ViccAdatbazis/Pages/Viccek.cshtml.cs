using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json; // Add this using directive
using System.Threading.Tasks;
using ViccAdatbazis.Models; // Include your existing model namespace
using System.Text.Json; // For JSON serialization

namespace ViccAdatbazis.Pages
{
    public class ViccekModel : PageModel
    {
        public List<Vicc> Jokes { get; set; }

        public async Task OnGetAsync()
        {
            // Call your API to get jokes
            using (var httpClient = new HttpClient())
            {
                // Adjust the URL according to your setup
                var response = await httpClient.GetAsync("http://localhost:5271/api/vicc");

                if (response.IsSuccessStatusCode)
                {
                    // Use ReadFromJsonAsync for deserializing the content
                    Jokes = await response.Content.ReadFromJsonAsync<List<Vicc>>();
                }
                else
                {
                    Jokes = new List<Vicc>();
                }
            }
        }
    }
}