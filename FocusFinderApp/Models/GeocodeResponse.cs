public class GeocodeResponse
{
    public string? status { get; set; }
    public GeocodeResult[]? results { get; set; }
}

public class GeocodeResult
{
    public Geometry? geometry { get; set; }
}

public class Geometry
{
    public Location? location { get; set; }
}

public class Location
{
    public double lat { get; set; }
    public double lng { get; set; }
}
