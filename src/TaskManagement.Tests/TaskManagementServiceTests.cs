using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TaskManagement.API.Data.Entities.Tables;
using TaskManagement.API.Interfaces.Persistence;
using TaskManagement.API.Interfaces.Persistence.Cache;
using TaskManagement.API.Services;
using TaskManagement.Tests.DynamicData;

namespace TaskManagement.Tests
{
    [TestClass]
    public class TaskManagementServiceTests
    {
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<ICacheService> _cacheServiceMock = new Mock<ICacheService>();
        private readonly Mock<IUnitOfWork<TbTask>> _unitOfWorkServiceMock = new Mock<IUnitOfWork<TbTask>>();

        [DynamicData(nameof(DynamicDataRepository.DataRowTasks), typeof(DynamicDataRepository), DynamicDataSourceType.Method)]
        [TestMethod]
        public async Task ReturnsOkResult(IEnumerable<TbTask> tasks)
        {
            // Arrange
            var taskManagementService =
                new TaskManagementService(_unitOfWorkServiceMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object);


            // Act
            foreach (var task in tasks)
            {
                taskManagementService.UpdateTask(task);

                // Assert
                if (task.Completed)
                {
                    Assert.IsFalse(task.InProgress);
                    Assert.IsFalse(task.Pending);
                }
                if (task.InProgress)
                {
                    Assert.IsFalse(task.Completed);
                    Assert.IsTrue(task.Pending);
                }
                if (task.Pending)
                    Assert.IsFalse(task.Completed);
            }
        }
    }
}