using Moq;
using Shipment.Domain.Test.MockAggregate;
using Shippment.Domain.AggregateModels.EquipmentAggregate;

namespace Shipment.Domain.Test.TestFixture
{
    public class EquipmentTestFixture
    {
        private readonly List<Equipment> _equipmentTestData;
        private Mock<IEquipmentRepository> _mockEquipmentRepository;

        public EquipmentTestFixture()
        {
            _equipmentTestData = EquipmentProxy.SeedTestData();
            InitMockRepositories();
        }

        public IEquipmentRepository EquipmentRepository => _mockEquipmentRepository.Object;

        private void InitMockRepositories()
        {
            #region initialize mock equipment repository
            _mockEquipmentRepository = new Mock<IEquipmentRepository>();

            _mockEquipmentRepository.Setup(rp => rp.GetAsync(It.IsAny<long>()))
                .ReturnsAsync((long id) => _equipmentTestData.FirstOrDefault(e => e.Id == id));

            _mockEquipmentRepository.Setup(
                rp => rp.GetAvailableEquipmentAsync(
                    It.Is<long>(locationId => locationId > 0),
                    It.IsAny<DateTime?>())
                ).ReturnsAsync((long locationId, DateTime? requireTime) => {
                    IEnumerable<Equipment> willComing = _equipmentTestData.Where(e => e.IsInuse && e.Destination.LocationId == locationId).AsQueryable();
                    if (requireTime.HasValue)
                    {
                        willComing = willComing.Where(e => e.EstimateReleaseTime < requireTime.Value);
                    }
                    
                    var available = _equipmentTestData.Where(e => e.CurrentLocation.LocationId == locationId && !e.IsInuse).ToList();
                    available.AddRange(willComing);

                    return available;
                });
            #endregion
        }
    }
}
