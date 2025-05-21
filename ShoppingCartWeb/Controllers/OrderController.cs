using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.DataAccess.ViewModels;
using System;
using System.Linq.Expressions;

namespace ShoppingCart.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Details(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);

            if (orderHeader == null)
            {
                return NotFound();
            }

            var orderDetails = _unitOfWork.OrderDetail.GetAll(od => od.OrderHeaderId == id);

            var orderVM = new OrderVM
            {
                OrderHeader = orderHeader,
                OrderDetails = orderDetails
            };

            return View(orderVM);
        }
    }
}