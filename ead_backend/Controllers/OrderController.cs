using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using ead_backend.Services;
using ead_backend.Model.Dtos;
using ead_backend.Utills;

namespace ead_backend.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public OrderController(IOrderService orderService, IUserService userService)
        {
            _orderService = orderService;
            _userService = userService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            var result = await _orderService.CreateOrderAsync(user.Id.ToString(), createOrderDto);

            if (!result.IsSuccess)
            {
                return this.CustomResponse(false, 400, result.Message, null);
            }

            return this.CustomResponse(true, 201, "Order created successfully", result.Order);
        }

        [HttpGet("get-order-by-id/{orderId}")]
        public async Task<IActionResult> GetOrder(string orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return this.CustomResponse(false, 404, "Order not found", null);
            }

            return this.CustomResponse(true, 200, "Order retrieved successfully", order);
        }

        [HttpGet("get-customer-orders")]
        public async Task<IActionResult> GetCustomerOrders()
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userService.GetUserByEmailAsync(userEmail);

            if (user == null)
            {
                return this.CustomResponse(false, 403, "Unauthorized access", null);
            }

            var orders = await _orderService.GetOrdersByCustomerIdAsync(user.Id.ToString());

            return this.CustomResponse(true, 200, "Customer orders retrieved successfully", orders);
        }

        [HttpPut("update-order-status/{orderId}")]
        [Authorize(Policy = "VendorOrAdmin")]
        public async Task<IActionResult> UpdateOrderStatus(string orderId, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            var updatedOrder = await _orderService.UpdateOrderStatusAsync(orderId, updateOrderStatusDto.OrderStatus);

            if (updatedOrder == null)
            {
                return this.CustomResponse(false, 404, "Order not found", null);
            }

            return this.CustomResponse(true, 200, "Order status updated successfully", updatedOrder);
        }

        [HttpPut("update-order-item-status/{orderId}/items/{orderItemId}")]
        [Authorize(Policy = "VendorOrAdmin")]
        public async Task<IActionResult> UpdateOrderItemStatus(string orderId, string orderItemId, [FromBody] UpdateOrderItemStatusDto updateOrderItemStatusDto)
        {
            var updatedOrderItem = await _orderService.UpdateOrderItemStatusAsync(orderId, orderItemId, updateOrderItemStatusDto.Status);

            if (updatedOrderItem == null)
            {
                return this.CustomResponse(false, 404, "Order item not found", null);
            }

            return this.CustomResponse(true, 200, "Order item status updated successfully", updatedOrderItem);
        }

        [HttpGet("get-all-orders")]
        public async Task<IActionResult> getAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return this.CustomResponse(true, 200, "Customer orders retrieved successfully", orders);
        }
    }
}
