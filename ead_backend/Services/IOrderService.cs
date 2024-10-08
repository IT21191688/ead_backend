// File: IOrderService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using ead_backend.Model.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ead_backend.Services
{
    public interface IOrderService
    {
        Task<(bool IsSuccess, string Message, OrderDto Order)> CreateOrderAsync(string customerId, CreateOrderDto createOrderDto);
        Task<OrderDto> GetOrderByIdAsync(string orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(string customerId);
        Task<OrderDto> UpdateOrderStatusAsync(string orderId, string newStatus);
        Task<OrderItemDto> UpdateOrderItemStatusAsync(string orderId, string orderItemId, string newStatus);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    }
}