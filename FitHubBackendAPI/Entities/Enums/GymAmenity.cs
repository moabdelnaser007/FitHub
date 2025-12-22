namespace FitHubBackendAPI.Entities.Enums
{
    [Flags]
    public enum GymAmenity
    {
        Wifi = 1,
        Parking = 2,
        Locker = 4,
        Shower = 8,
        Sauna = 16,
        SwimmingPool = 32,
        AirConditioning = 64,
        PersonalTrainer = 128
    }
}
