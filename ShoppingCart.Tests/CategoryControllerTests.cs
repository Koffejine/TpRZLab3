using Moq;
using Xunit;
using ShoppingCart.Web.Areas.Admin.Controllers;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ShoppingCart.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _controller = new CategoryController(_unitOfWorkMock.Object);
        }

        [Fact]
        public void GetAllCategories_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<Category> { new Category(), new Category() };

            _unitOfWorkMock
                .Setup(u => u.Category.GetAll(null, null))
                .Returns(categories);

            // Act
            var result = _controller.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Categories.Count());
        }

        [Fact]
        public void GetCategoryById_ReturnsCorrectCategory()
        {
            // Arrange
            var category = new Category { Id = 1 };
            _unitOfWorkMock
                .Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), null))
                .Returns(category);

            // Act
            var result = _controller.GetCategoryById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Category.Id);
        }

        [Fact]
        public void GetCategoryById_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), null))
                .Returns((Category)null);

            // Act
            var result = _controller.GetCategoryById(99);

            // Assert
            Assert.Null(result.Category);
        }

        [Fact]
        public void CreateUpdate_AddsNewCategory_WhenIdIsZero()
        {
            // Arrange
            var category = new Category { Id = 0 };
            var vm = new CategoryVM { Category = category };
            _unitOfWorkMock.Setup(u => u.Category.Add(It.IsAny<Category>()));
            _controller.ModelState.Clear(); // Valid state

            // Act
            _controller.CreateUpdate(vm);

            // Assert
            _unitOfWorkMock.Verify(u => u.Category.Add(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void CreateUpdate_UpdatesCategory_WhenIdIsNotZero()
        {
            // Arrange
            var category = new Category { Id = 2 };
            var vm = new CategoryVM { Category = category };
            _unitOfWorkMock.Setup(u => u.Category.Update(It.IsAny<Category>()));
            _controller.ModelState.Clear();

            // Act
            _controller.CreateUpdate(vm);

            // Assert
            _unitOfWorkMock.Verify(u => u.Category.Update(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void CreateUpdate_ThrowsException_WhenModelInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var vm = new CategoryVM { Category = new Category() };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.CreateUpdate(vm));
            Assert.Equal("Model is invalid", ex.Message);
        }

        [Fact]
        public void DeleteData_RemovesCategory_WhenExists()
        {
            // Arrange
            var category = new Category { Id = 1 };
            _unitOfWorkMock
                .Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), null))
                .Returns(category);

            // Act
            _controller.DeleteData(1);

            // Assert
            _unitOfWorkMock.Verify(u => u.Category.Delete(category), Times.Once);
            _unitOfWorkMock.Verify(u => u.Save(), Times.Once);
        }

        [Fact]
        public void DeleteData_ThrowsException_WhenCategoryNotFound()
        {
            // Arrange
            _unitOfWorkMock
                .Setup(u => u.Category.GetFirstOrDefault(It.IsAny<Expression<Func<Category, bool>>>(), null))
                .Returns((Category)null);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.DeleteData(100));
            Assert.Equal("Category not found", ex.Message);
        }
    }
}