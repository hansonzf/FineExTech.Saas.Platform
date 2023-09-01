namespace LocationApi.Payload
{
    public class RoutePayload : PayloadBase
    {
        public long Id { get; set; }
        public long Owner { get; set; }
        public string RouteName { get; set; }
        public LocationDto Origin { get; set; }
        public LocationDto Destination { get; set; }
        public SegmentDto[] Segments { get; set; }
    }

    public class SegmentDto
    {
        public LocationDto From { get; set; }
        public LocationDto To { get; set; }
        public double Distance { get; set; }
    }

    public class LocationDto
    {
        public long LocationId { get; set; }
        public string LocationName { get; set; }
        public string City { get; set; }
    }
}
