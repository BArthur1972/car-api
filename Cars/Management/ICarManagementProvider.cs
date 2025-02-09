using System;
using Cars.ApiCommon.Models.Resources;

namespace Cars.Management;

public interface ICarManagementProvider
{
    Task AddCar(CarRequestPayload car);
    Task<IEnumerable<CarResponsePayload>> GetCars();
    Task<CarResponsePayload?> GetCar(string id);
    Task RemoveCar(string id);
}
