namespace ZamowKsiazke.Tests
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using System.Security.Claims;
    using ZamowKsiazke.Controllers;
    using ZamowKsiazke.Data;
    using ZamowKsiazke.Models;
    using ZamowKsiazke.Services.Interfaces;

    public class BooksControllerTests : IDisposable
    {
        private readonly ZamowKsiazkeContext _context;
        private readonly Mock<IUserActivityService> _mockActivityService;

        public BooksControllerTests()
        {
            var options = new DbContextOptionsBuilder<ZamowKsiazkeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ZamowKsiazkeContext(options);
            _mockActivityService = new Mock<IUserActivityService>();

            SeedTestData();
        }

        private void SeedTestData()
        {
            _context.Book.AddRange(new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Title = "Book 1",
                    Description = "Description 1",
                    Language = "English",
                    ISBN = "1234567890",
                    Author = "Author 1",
                    StockQuantity = 5
                },
                new Book
                {
                    Id = 2,
                    Title = "Book 2",
                    Description = "Description 2",
                    Language = "English",
                    ISBN = "0987654321",
                    Author = "Author 2",
                    StockQuantity = 3
                }
            });
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfBooks()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Book>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewResult_WithBook()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);
            int testBookId = 1;

            // Act
            var result = await controller.Details(testBookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Book>(viewResult.ViewData.Model);
            Assert.Equal(testBookId, model.Id);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);
            int invalidBookId = 99;

            // Act
            var result = await controller.Details(invalidBookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_GET_ReturnsViewResult()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);

            // Act
            var result = controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Create_POST_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);
            SetUpControllerUser(controller);

            var newBook = new Book
            {
                Title = "New Book",
                Description = "New Book Description",
                Language = "English",
                ISBN = "1111111111",
                Author = "New Author",
                StockQuantity = 10
            };

            // Act
            var result = await controller.Create(newBook);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockActivityService.Verify(
                service => service.LogActivityAsync(
                    "test-user-id",
                    "BookCreate",
                    "Wykonano akcję Create na książce",
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Edit_POST_ValidModel_RedirectsToIndex()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);
            SetUpControllerUser(controller);

            var existingBook = _context.Book.First();
            existingBook.Title = "Updated Title";

            // Act
            var result = await controller.Edit(existingBook.Id, existingBook);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockActivityService.Verify(
                service => service.LogActivityAsync(
                    "test-user-id",
                    "BookUpdate",
                    "Wykonano akcję Update na książce",
                    existingBook.Id.ToString()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteConfirmed_ValidId_RedirectsToIndex()
        {
            // Arrange
            var controller = new BooksController(_context, _mockActivityService.Object);
            SetUpControllerUser(controller);
            int testBookId = 1;

            // Act
            var result = await controller.DeleteConfirmed(testBookId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mockActivityService.Verify(
                service => service.LogActivityAsync(
                    "test-user-id",
                    "BookDelete",
                    "Wykonano akcję Delete na książce",
                    testBookId.ToString()),
                Times.Once);
        }

        private void SetUpControllerUser(Controller controller)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
    }
}
