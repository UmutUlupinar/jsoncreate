using jsoncreate;
using Newtonsoft.Json;

internal class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        bool jsonUretme = false;
        bool paycoreTest = true;
        bool walletApiTest = true;
        if (paycoreTest)
        {
            var countries = JsonConvert
                .DeserializeObject<IEnumerable<dynamic>>(
                    File.ReadAllText("../../../../jsoncreate/Test-Jsons/data-paycore-country.json"))!
                .ToDictionary(x => x.code, x => new
                {
                    x.code,
                    x.alphacode,
                    x.description
                });

            var cities = JsonConvert
                .DeserializeObject<IEnumerable<dynamic>>(
                    File.ReadAllText("../../../../jsoncreate/Test-Jsons/data-paycore-city.json"))!
                .ToDictionary(x => x.code, x => x.description);

            var towns = JsonConvert
                .DeserializeObject<IEnumerable<dynamic>>(
                    File.ReadAllText("../../../../jsoncreate/Test-Jsons/data-paycore-town.json"))!
                .ToDictionary(x => x.code, x => new
                {
                    x.code,
                    x.description,
                    x.city_code
                });

            var neighborhoods = JsonConvert
                .DeserializeObject<IEnumerable<dynamic>>(
                    File.ReadAllText("../../../../jsoncreate/Test-Jsons/neighborhoods.json"))!
                .ToDictionary(
                    x => String.Concat(x.description, "-", x.code, "-", new Random().Next(1, 110000).ToString()),
                    x => new
                    {
                        x.code,
                        x.description,
                        x.town_code
                    });

            var result = new List<PaycoreAddress>();
            foreach (var neighborhood in neighborhoods)
            {
                var row = new PaycoreAddress()
                {
                    Neighborhood = neighborhood.Value.description,
                    NeighborhoodCode = neighborhood.Value.code,
                };
                var neighborhoodTownCode = neighborhood.Value.town_code;

                if (towns.ContainsKey(neighborhoodTownCode))
                {
                    var town = towns[neighborhoodTownCode];
                    row.Town = town.description;
                    row.TownCode = town.code;
                    row.CountryCode = "792";
                    row.Country = "TR:=TÜRKİYE;;EN:=TURKEY";
                    row.CountryAlphaCode = "TR";

                    var townCityCode = town.city_code;

                    if (cities.ContainsKey(townCityCode))
                    {
                        var city = cities[townCityCode];
                        row.City = city;
                        row.CityCode = townCityCode;
                    }

                    result.Add(row);
                }
            }

            foreach (var country in countries)
            {
                if (country.Value.alphacode == "TR") continue;

                result.Add(new PaycoreAddress()
                {
                    CountryAlphaCode = country.Value.alphacode,
                    CountryCode = country.Value.code,
                    Country = country.Value.description,

                    City = "YABANCI",
                    CityCode = "999",

                    Town = "",
                    TownCode = ""
                });
            }
            
            var rowsTyped = result.Select(x => new
            {
                countryAlphaCode = (string)x.CountryAlphaCode,
                city = (string)x.City,
                town = (string)x.Town,
                neighborhood = (string)x.Neighborhood
            });

            object data = rowsTyped.GroupBy(x => x.countryAlphaCode)
                .Select(x => new
                {
                    Country = x.Key,
                    Cities = x.GroupBy(y => y.city)
                        .OrderBy(y => y.Key, StringComparer.InvariantCultureIgnoreCase)
                        .Select(z => new
                        {
                            City = z.Key,
                            Towns = z.GroupBy(t => t.town)
                                .Select(t => new
                                {
                                    Town = t.Key,
                                    neighborhoods = t.Select(n => n.neighborhood)
                                                     .OrderBy(n => n)
                                                     .Distinct()
                                })
                                .Distinct()
                                .OrderBy(t => t.Town)
                        })
                })
                .FirstOrDefault(x => x.Country == "TR");

            var walletResult = JsonConvert.SerializeObject(data); // Serialize the list to JSON string

            // Specify the file path where you want to save the JSON data
            string filePathWalletResult =
                @"C:\Users\umut.ulupinar\Desktop\wallet-result.json"; // Replace with your desired path

            // Write the JSON string to the file
            File.WriteAllText(filePathWalletResult, walletResult);
        }

        if (jsonUretme)
        {
            string jsonCity = File.ReadAllText("../../../../jsoncreate/data-paycore-city.json");
            string jsonTown = File.ReadAllText("../../../../jsoncreate/data-paycore-town.json");
            string jsonAddress = File.ReadAllText("../../../../jsoncreate/address_data.json");

            // JSON verisini C# nesnelerine dönüştür
            var cities = JsonConvert.DeserializeObject<IEnumerable<city>>(jsonCity);
            var towns = JsonConvert.DeserializeObject<IEnumerable<town>>(jsonTown);
            var addresses = JsonConvert.DeserializeObject<addressData>(jsonAddress);

            var neighborhoods = new List<neighborhood>();
            var differences = new List<string>();

            foreach (var city in cities)
            {
                var ildekiIlceler = towns.Where(t => t.city_code == city.code);
                var ildekiAdresler = addresses.cities.FirstOrDefault(c => c.name == city.description);

                foreach (var ilce in ildekiIlceler)
                {
                    var ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                        c.name.Equals(ilce.description, StringComparison.InvariantCultureIgnoreCase));
                    var yeniILce = ilce.description;
                    if (ilcedekiAdresler == null)
                    {
                        yeniILce = ilce.description.Replace("S", "Ş");
                        ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                            c.name.Equals(yeniILce, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (ilcedekiAdresler == null)
                    {
                        yeniILce = ilce.description.Replace("O", "Ö");
                        ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                            c.name.Equals(yeniILce, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (ilcedekiAdresler == null)
                    {
                        yeniILce = ilce.description.Replace("I", "İ");
                        ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                            c.name.Equals(yeniILce, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (ilcedekiAdresler == null)
                    {
                        yeniILce = ilce.description.Replace("U", "Ü");
                        ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                            c.name.Equals(yeniILce, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (ilcedekiAdresler == null)
                    {
                        yeniILce = ilce.description.Replace("C", "Ç");
                        ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                            c.name.Equals(yeniILce, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (ilcedekiAdresler == null)
                    {
                        yeniILce = ilce.description.Replace("G", "Ğ");
                        ilcedekiAdresler = ildekiAdresler.counties.FirstOrDefault(c =>
                            c.name.Equals(yeniILce, StringComparison.InvariantCultureIgnoreCase));
                    }

                    if (ilcedekiAdresler == null)
                    {
                        if (ilce.description != "MERKEZ") differences.Add(ilce.description + "-" + city.description);
                    }
                    else
                    {
                        var ilcedekiMahalleler = ilcedekiAdresler?.districts
                            .SelectMany(x => x.neighborhoods).ToList();

                        ilcedekiMahalleler.ForEach(x => neighborhoods.Add(new neighborhood()
                        {
                            code = x.code,
                            description = x.name,
                            town_code = ilce.code
                        }));
                    }
                }
            }

            // var json = JsonConvert.SerializeObject(neighborhoods); // Serialize the list to JSON string
            //
            // // Specify the file path where you want to save the JSON data
            // string filePath = @"C:\Users\umut.ulupinar\Desktop\neighborhoods.json"; // Replace with your desired path
            //
            // // Write the JSON string to the file
            // File.WriteAllText(filePath, json);
            //
            // var json2 = JsonConvert.SerializeObject(differences); // Serialize the list to JSON string
            //
            // // Specify the file path where you want to save the JSON data
            // string filePath2 = @"C:\Users\umut.ulupinar\Desktop\differences.json"; // Replace with your desired path
            //
            // // Write the JSON string to the file
            // File.WriteAllText(filePath2, json2);
        }
    }
}