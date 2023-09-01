namespace Shippment.Domain.AggregateModels
{
    public enum OrderStatus
    {
        Draft = 0,
        Ordered,
        Accepted,
        Rejected,
        Pickup,
        Standby,
        Transferring,
        Arrived,
        Received
    }

    public enum TransportMethod
    {
        ByRoad,
        BySea,
        ByAir
    }

    public enum EquipmentType
    {
        Vehicle,
        Trackor,
        Ship,
        AirCraft
    }

    public enum ScheduleType
    {
        Pickup = 0,
        Delivery,
        Transport
    }

    public enum ScheduleStatus
    {
        Created,
        Standby,
        Executed,
        Completed,
        Cancelled
    }

    public enum UnitOfVolume
    {
        CubicCentimeter,
        CubicDecimeter,
        CubicMeter
    }

    public enum UnitOfLength
    {
        Centimeter = 1,
        Decimeter = 2,
        Meter = 3
    }
}
