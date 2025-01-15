public interface IVehicleShop
{
    void DisplayVehicleDetails(VehicleData vehicle);
    bool PurchaseVehicle(VehicleData vehicle, int quantity);
    VehicleData[] GetAvailableVehicles();
} 