using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZamowKsiazke.Controllers;
using ZamowKsiazke.Data;
using ZamowKsiazke.Models;
using Moq;

namespace ZamowKsiazke.Tests
{
    public class StoreControllerTests : IDisposable
    {
        private readonly ZamowKsiazkeContext _context;

        public StoreControllerTests()
        {
            var options = new DbContextOptionsBuilder<ZamowKsiazkeContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ZamowKsiazkeContext(options);

            SeedTestData();
        }

        private void SeedTestData()
        {
            _context.Book.AddRange(new Book[]
            {
                    new Book
                    {
                        Id = 1,
                        Title = "Test Book 1",
                        Author = "Author 1",
                        Description = "Description 1",
                        Language = "English",
                        ISBN = "1234567890",
                        StockQuantity = 5
                    },
                    new Book
                    {
                        Id = 2,
                        Title = "Test Book 2",
                        Author = "Author 2",
                        Description = "Description 2",
                        Language = "Polish",
                        ISBN = "0987654321",
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
            var controller = new StoreController(_context);

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
            var controller = new StoreController(_context);
            int testBookId = 1;

            // Act
            var result = await controller.Details(testBookId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Book>(viewResult.ViewData.Model);
            Assert.Equal(testBookId, model.Id);
        }

        [Fact]
        public async Task Details_NullId_ReturnsNotFound()
        {
            // Arrange
            var controller = new StoreController(_context);

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var controller = new StoreController(_context);
            int invalidBookId = 999;

            // Act
            var result = await controller.Details(invalidBookId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ExceptionThrown_ReturnsErrorView()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Book>>();
            var mockContext = new Mock<ZamowKsiazkeContext>(new DbContextOptions<ZamowKsiazkeContext>());

            var queryable = new List<Book>().AsQueryable();
            mockSet.As<IQueryable<Book>>().Setup(m => m.Provider).Throws(new Exception("Database error"));
            mockSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            mockContext.Setup(m => m.Book).Returns(mockSet.Object);

            var controller = new StoreController(mockContext.Object);

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
        }



    }
}
