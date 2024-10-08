// File: OrderDto
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:
using System;
using System.Collections.Generic;

namespace ead_backend.Model.Dtos
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public UserDto Customer { get; set; }
    }

    public class OrderItemDto
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string VendorId { get; set; }
        public string Status { get; set; }
        public ProductDto Product { get; set; }
        public UserDto Vendor { get; set; }
    }

    public class CreateOrderDto
    {
        public List<CreateOrderItemDto> OrderItems { get; set; }
    }

    public class CreateOrderItemDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public string OrderStatus { get; set; }
    }

    public class UpdateOrderItemStatusDto
    {
        public string Status { get; set; }
    }
}