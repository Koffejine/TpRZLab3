using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Web.Controllers;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.Models;
using ShoppingCart.DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;

public class OrderControllerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly OrderController _controller;

    public OrderControllerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _controller = new OrderController(_unitOfWorkMock.Object);
    }

    [Fact]
    public void Details_ReturnsOrderVM_WhenOrderExists()
    {
        // Arrange
        var orderId = 1;
        var orderHeader = new OrderHeader { Id = orderId, Name = "User" };
        var orderDetails = new List<OrderDetail> { new OrderDetail { OrderHeaderId = orderId, Count = 2 } };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>()))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>()))
            .Returns(orderDetails);

        // Act
        var result = _controller.Details(orderId) as ViewResult;
        var model = result?.Model as OrderVM;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(model);
        Assert.Equal(orderHeader, model.OrderHeader);
        Assert.Equal(orderDetails, model.OrderDetails);
    }

    [Fact]
    public void Details_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        int id = 99;

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(
            It.IsAny<Expression<Func<OrderHeader, bool>>>(),
            It.IsAny<string>()))
            .Returns((OrderHeader)null);

        // Act
        var result = _controller.Details(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
    [Fact]
    public void Details_ReturnsViewResult()
    {
        // Arrange
        var orderId = 1;
        var orderHeader = new OrderHeader { Id = orderId };
        var orderDetails = new List<OrderDetail> { new OrderDetail { OrderHeaderId = orderId } };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>()))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>()))
            .Returns(orderDetails);

        // Act
        var result = _controller.Details(orderId);

        // Assert
        Assert.IsType<ViewResult>(result);
    }
    [Fact]
    public void Details_ReturnsCorrectModel()
    {
        // Arrange
        var orderId = 5;
        var orderHeader = new OrderHeader { Id = orderId, Name = "Test Order" };
        var orderDetails = new List<OrderDetail>
    {
        new OrderDetail { OrderHeaderId = orderId, Count = 2 },
        new OrderDetail { OrderHeaderId = orderId, Count = 3 }
    };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>()))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>()))
            .Returns(orderDetails);

        // Act
        var result = _controller.Details(orderId) as ViewResult;
        var model = result?.Model as OrderVM;

        // Assert
        Assert.NotNull(model);
        Assert.Equal(orderHeader, model.OrderHeader);
        Assert.Equal(orderDetails, model.OrderDetails);
    }
    [Fact]
    public void Details_OrderDetailsAreNull_ReturnsModelWithEmptyList()
    {
        // Arrange
        var orderId = 2;
        var orderHeader = new OrderHeader { Id = orderId };

        _unitOfWorkMock.Setup(u => u.OrderHeader.GetFirstOrDefault(It.IsAny<Expression<Func<OrderHeader, bool>>>(), It.IsAny<string>()))
            .Returns(orderHeader);
        _unitOfWorkMock.Setup(u => u.OrderDetail.GetAll(It.IsAny<Expression<Func<OrderDetail, bool>>>(), It.IsAny<string>()))
            .Returns((IEnumerable<OrderDetail>)null);

        // Act
        var result = _controller.Details(orderId) as ViewResult;
        var model = result?.Model as OrderVM;

        // Assert
        Assert.NotNull(model);
        Assert.Equal(orderHeader, model.OrderHeader);
        Assert.Null(model.OrderDetails);
    }
}