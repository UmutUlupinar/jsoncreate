namespace jsoncreate;

public class addressData
{
    public List<City> cities { get; set; }
}

public class City
{
    public string name { get; set; }
    public List<County> counties { get; set; }
}

public class County
{
    public string name { get; set; }
    public List<District> districts { get; set; }
}

public class District
{
    public string name { get; set; }
    public List<Neighborhood> neighborhoods { get; set; }
}

public class Neighborhood
{
    public string name { get; set; }
    public string code { get; set; }
}