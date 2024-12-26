using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ZamowKsiazke.Controllers;
using ZamowKsiazke.Models;
using ZamowKsiazke.Services.Interfaces;
using ZamowKsiazke.ViewModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MockQueryable;

namespace ZamowKsiazke.Tests
{
    public class AdminControllerTests
    {
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<UserManager<DefaultUser>> _mockUserManager;
        private readonly Mock<IUserActivityService> _mockUserActivityService;
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IBookBorrowingService> _mockBookBorrowingService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockRoleManager = new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                new IRoleValidator<IdentityRole>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

            _mockUserManager = new Mock<UserManager<DefaultUser>>(
                Mock.Of<IUserStore<DefaultUser>>(),
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<DefaultUser>>().Object,
                new IUserValidator<DefaultUser>[0],
                new IPasswordValidator<DefaultUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<DefaultUser>>>().Object);

            _mockUserActivityService = new Mock<IUserActivityService>();
            _mockOrderService = new Mock<IOrderService>();
            _mockBookBorrowingService = new Mock<IBookBorrowingService>();

            _controller = new AdminController(
                _mockRoleManager.Object,
                _mockUserManager.Object,
                _mockUserActivityService.Object,
                _mockOrderService.Object,
                _mockBookBorrowingService.Object);
        }

        [Fact]
        public async Task ActivityReport_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            _mockUserActivityService.Setup(service => service.GetRecentActivitiesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<UserActivity>());
            _mockUserActivityService.Setup(service => service.GetActivityStatisticsAsync())
                .ReturnsAsync(new Dictionary<string, int>());
            _mockUserActivityService.Setup(service => service.GetUserLoginStatisticsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Dictionary<string, int>());
            _mockUserActivityService.Setup(service => service.GetAllActivitiesAsync())
                .ReturnsAsync(new List<UserActivity>());

            // Act
            var result = await _controller.ActivityReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserActivityReportViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task UserLoginReport_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            var users = new List<DefaultUser>().AsQueryable().BuildMock();
            _mockUserManager.Setup(manager => manager.Users).Returns(users);

            _mockUserActivityService.Setup(service => service.GetUserLoginStatisticsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Dictionary<string, int>());

            // Act
            var result = await _controller.UserLoginReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserActivitySummary>>(viewResult.Model);
        }

        [Fact]
        public async Task RecentActivities_ReturnsViewResult_WithActivities()
        {
            // Arrange
            _mockUserActivityService.Setup(service => service.GetRecentActivitiesAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<UserActivity>());

            // Act
            var result = await _controller.RecentActivities();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<UserActivity>>(viewResult.Model);
        }

        [Fact]
        public async Task ManageOrders_ReturnsViewResult_WithOrderManagementViewModel()
        {
            // Arrange
            _mockOrderService.Setup(service => service.GetAllOrdersAsync())
                .ReturnsAsync(new List<Order>());
            _mockUserManager.Setup(manager => manager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new DefaultUser());

            // Act
            var result = await _controller.ManageOrders();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<OrderManagementViewModel>>(viewResult.Model);
        }

        [Fact]
        public async Task UpdatePaymentStatus_ValidOrderId_UpdatesStatusAndRedirects()
        {
            // Arrange
            var orderId = 1;
            var order = new Order { Id = orderId, UserId = "user1", IsPaid = false };

            _mockOrderService.Setup(service => service.GetOrderWithItemsAsync(orderId))
                .ReturnsAsync(order);
            _mockOrderService.Setup(service => service.UpdatePaymentStatusAsync(orderId, true))
                .Returns(Task.CompletedTask);
            _mockUserActivityService.Setup(service => service.LogActivityAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()
            )).Returns(Task.CompletedTask);

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            // Act
            var result = await _controller.UpdatePaymentStatus(orderId, true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageOrders", redirectResult.ActionName);

            _mockOrderService.Verify(service => service.UpdatePaymentStatusAsync(orderId, true), Times.Once);
            _mockUserActivityService.Verify(service => service.LogActivityAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePaymentStatus_InvalidOrderId_ReturnsError()
        {
            // Arrange
            var orderId = 1;
            _mockOrderService.Setup(service => service.GetOrderWithItemsAsync(orderId))
                .ReturnsAsync((Order?)null);

            var tempDataMock = new Mock<ITempDataDictionary>();
            _controller.TempData = tempDataMock.Object;

            // Act
            var result = await _controller.UpdatePaymentStatus(orderId, true);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ManageOrders", redirectResult.ActionName);

            tempDataMock.VerifySet(tempData => tempData["Error"] = $"Nie znaleziono zamówienia #{orderId}.", Times.Once);
        }

        [Fact]
        public async Task BookActivityReport_ReturnsViewResult_WithViewModel()
        {
            // Arrange
            _mockOrderService.Setup(service => service.GetAllOrdersAsync())
                .ReturnsAsync(new List<Order>());
            _mockBookBorrowingService.Setup(service => service.GetAllBorrowingsAsync())
                .ReturnsAsync(new List<BookBorrowing>());
            _mockUserManager.Setup(manager => manager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new DefaultUser());

            // Act
            var result = await _controller.BookActivityReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BookActivityReportViewModel>(viewResult.Model);
        }

        [Fact]
        public async Task ActivityReport_ReturnsCorrectViewModel()
        {
            // Arrange
            var activities = new List<UserActivity>
            {
                new UserActivity { UserId = "user1", ActivityType = "Login", Description = "User logged in" },
                new UserActivity { UserId = "user2", ActivityType = "Order", Description = "User placed an order" }
            };

            _mockUserActivityService.Setup(service => service.GetRecentActivitiesAsync(It.IsAny<int>()))
                .ReturnsAsync(activities);
            _mockUserActivityService.Setup(service => service.GetActivityStatisticsAsync())
                .ReturnsAsync(new Dictionary<string, int> { { "Login", 1 }, { "Order", 1 } });
            _mockUserActivityService.Setup(service => service.GetUserLoginStatisticsAsync(It.IsAny<int>()))
                .ReturnsAsync(new Dictionary<string, int>());
            _mockUserActivityService.Setup(service => service.GetAllActivitiesAsync())
                .ReturnsAsync(activities);

            // Act
            var result = await _controller.ActivityReport();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserActivityReportViewModel>(viewResult.Model);
            Assert.Equal(2, model.RecentActivities.Count());
            Assert.Equal(2, model.TotalActivities);
            Assert.Equal(2, model.UniqueUsers);
        }
    }
}
