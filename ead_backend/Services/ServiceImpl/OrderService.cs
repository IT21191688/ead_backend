// File: OrderService
// Author: M.W.H.S.L Ruwanpura
// IT Number: IT21191688
// Description:

using ead_backend.Data;
using ead_backend.Model;
using ead_backend.Model.Dtos;
using MongoDB.Driver;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ead_backend.Services.ServiceImpl
{
    public class OrderService : IOrderService
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Product> _products;
        private readonly IMongoCollection<User> _users;

        public OrderService(MongoDbContext context)
        {
            _orders = context.Orders;
            _products = context.Products;
            _users = context.Users;
        }

        public async Task<(bool IsSuccess, string Message, OrderDto Order)> CreateOrderAsync(string customerId, CreateOrderDto createOrderDto)
        {
            using var session = await _orders.Database.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                var order = new Order
                {
                    CustomerId = ObjectId.Parse(customerId).ToString(), // ObjectId to store the customer ID
                    OrderStatus = "Processing",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;

                foreach (var item in createOrderDto.OrderItems)
                {
                    if (!ObjectId.TryParse(item.ProductId, out ObjectId productObjectId))
                    {
                        await session.AbortTransactionAsync();
                        return (false, $"Invalid product ID format: {item.ProductId}", null);
                    }

                    var product = await _products.Find(p => p.Id == productObjectId).FirstOrDefaultAsync();
                    if (product == null)
                    {
                        await session.AbortTransactionAsync();
                        return (false, $"Product with ID {item.ProductId} not found.", null);
                    }

                    if (product.Qty < item.Quantity)
                    {
                        await session.AbortTransactionAsync();
                        return (false, $"Insufficient quantity for product {product.Name}.", null);
                    }

                    var orderItem = new OrderItem
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        ProductId =productObjectId.ToString(),
                        Quantity = item.Quantity,
                        Price = product.Price,
                        VendorId = product.VendorId,
                        Status = "Processing"
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.Price * orderItem.Quantity;

                    // Reduce product quantity
                    var update = Builders<Product>.Update.Inc(p => p.Qty, -item.Quantity);
                    await _products.UpdateOneAsync(session, p => p.Id == productObjectId, update);
                }

                order.TotalAmount = totalAmount;

                await _orders.InsertOneAsync(session, order);

                await session.CommitTransactionAsync();

                var createdOrder = await GetOrderByIdAsync(order.Id);
                return (true, "Order created successfully", createdOrder);
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync();
                return (false, $"An error occurred while creating the order: {ex.Message}", null);
            }
        }

        public async Task<OrderDto> GetOrderByIdAsync(string orderId)
        {
            if (!ObjectId.TryParse(orderId, out ObjectId orderObjectId))
            {
                return null;
            }

            var order = await _orders.Find(o => o.Id.ToString() == orderObjectId.ToString()).FirstOrDefaultAsync();
            if (order == null)
            {
                return null;
            }

            var customer = await _users.Find(u => u.Id.ToString() == order.CustomerId).FirstOrDefaultAsync();

            var orderDto = new OrderDto
            {
                Id = order.Id.ToString(),
                CustomerId = order.CustomerId.ToString(), // Converting ObjectId to string
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Customer = MapUserToDto(customer),
                OrderItems = new List<OrderItemDto>()
            };

            foreach (var item in order.OrderItems)
            {
                var product = await _products.Find(p => p.Id.ToString() == item.ProductId).FirstOrDefaultAsync();
                var vendor = await _users.Find(u => u.Id.ToString() == item.VendorId).FirstOrDefaultAsync();

                orderDto.OrderItems.Add(new OrderItemDto
                {
                    Id = item.Id,
                    OrderId = order.Id,
                    ProductId = item.ProductId, // Converting ObjectId to string
                    Quantity = item.Quantity,
                    Price = item.Price,
                    VendorId = item.VendorId, // Converting ObjectId to string
                    Status = item.Status,
                    Product = MapProductToDto(product),
                    Vendor = MapUserToDto(vendor)
                });
            }

            return orderDto;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerIdAsync(string customerId)
        {
            if (!ObjectId.TryParse(customerId, out ObjectId customerObjectId))
            {
                return new List<OrderDto>();
            }

            var orders = await _orders.Find(o => o.CustomerId.ToString() == customerObjectId.ToString()).ToListAsync();
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                orderDtos.Add(await GetOrderByIdAsync(order.Id.ToString()));
            }

            return orderDtos;
        }

        public async Task<OrderDto> UpdateOrderStatusAsync(string orderId, string newStatus)
        {
            if (!ObjectId.TryParse(orderId, out ObjectId orderObjectId))
            {
                return null;
            }

            var update = Builders<Order>.Update.Set(o => o.OrderStatus, newStatus)
                                               .Set(o => o.UpdatedAt, DateTime.UtcNow);

            await _orders.UpdateOneAsync(o => o.Id.ToString() == orderObjectId.ToString(), update);

            return await GetOrderByIdAsync(orderId);
        }

        public async Task<OrderItemDto> UpdateOrderItemStatusAsync(string orderId, string orderItemId, string newStatus)
        {
            if (!ObjectId.TryParse(orderId, out ObjectId orderObjectId))
            {
                return null;
            }

            var order = await _orders.Find(o => o.Id.ToString() == orderObjectId.ToString()).FirstOrDefaultAsync();
            if (order == null)
            {
                return null;
            }

            var orderItem = order.OrderItems.FirstOrDefault(oi => oi.Id == orderItemId); // Compare as string
            if (orderItem == null)
            {
                return null;
            }

            orderItem.Status = newStatus;

            var update = Builders<Order>.Update.Set(o => o.OrderItems, order.OrderItems)
                                               .Set(o => o.UpdatedAt, DateTime.UtcNow);

            await _orders.UpdateOneAsync(o => o.Id.ToString() == orderObjectId.ToString(), update);

            var updatedOrder = await GetOrderByIdAsync(orderId);
            return updatedOrder.OrderItems.FirstOrDefault(oi => oi.Id.ToString() == orderItemId); // Compare as string
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orders = await _orders.Find(_ => true).ToListAsync(); // Retrieve all orders
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                orderDtos.Add(await GetOrderByIdAsync(order.Id.ToString())); // Reuse the method to map each order to OrderDto
            }

            return orderDtos;
        }

        private UserDto MapUserToDto(User user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Email = user.Email,
                Role = user.Role
            };
        }

        private ProductDto MapProductToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Qty = product.Qty
            };
        }
    }
}
