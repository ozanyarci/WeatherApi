using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeatherApi;
class Program
{
    private const string apiKey = "ENTER_API_KEY";
    static async Task Main(string[] args)
    {
        Location currentLocation = await GetLocation();
        await GetWeatherApi(currentLocation);
    }

    private static async Task<Location> GetLocation()
    {
        Location currentLocation = new Location();

        Console.WriteLine("Please enter a city name to get weather information related to that city: ");
        string? location = Console.ReadLine();
        string apiUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={location}&limit=1&appid={apiKey}";
        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                dynamic? locationData = JsonConvert.DeserializeObject(responseBody);
                if(locationData == null || locationData?.Count == 0)
                {
                    Console.WriteLine($"Location: not found");
                    return currentLocation;
                }

                // Extracting relevant information
                currentLocation.Latitude = locationData[0].lat;
                currentLocation.Longitude = locationData[0].lon;

                // Display the location information
                Console.WriteLine($"Location: {location}");
                Console.WriteLine($"Latitude: {currentLocation.Latitude}");
                Console.WriteLine($"Longitude: {currentLocation.Longitude}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error calling the Geocoding API: {e.Message}");
            }
        }

        return currentLocation;
    }
    private static async Task GetWeatherApi(Location currentLocation)
    {
        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={currentLocation.Latitude}&lon={currentLocation.Longitude}&appid={apiKey}";
        double kelvinToCelcius = -273.15;
        int precisionCount = 2;
        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse the JSON response
                dynamic? weatherData = JsonConvert.DeserializeObject(responseBody);
                if (weatherData == null)
                {
                    Console.WriteLine($"City: not found");
                    return;
                }

                // Extracting relevant information
                string cityName = weatherData.name;
                double temperature = weatherData.main.temp;
                string weatherDescription = weatherData.weather[0].description;

                // Display the weather information
                Console.WriteLine($"City: {cityName}");
                Console.WriteLine($"Temperature: {temperature}K");
                double temperatureCelcius = Math.Round(temperature + kelvinToCelcius, precisionCount);
                Console.WriteLine($"Temperature: {temperatureCelcius}°C");
                Console.WriteLine($"Weather: {weatherDescription}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error calling the weather API: {e.Message}");
            }
        }
    }
}
