using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;
using ShopPartner.Filters;
using ShopPartner.Models;
using ShopPartner.Services;

namespace ShopPartner.Controllers
{
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly Service _service;
        private IConfiguration _config;
        public ShopController(Service service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        /// <summary>
        /// Get all Shop Orders (paginated) (Returns: ShopOrdersList)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="per_page"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("/orders")]
        public ActionResult<List<ShopOrder>> GetAllShopOrders(string shopId, int per_page, int page)
        {
            long totalItems;

            if (string.IsNullOrWhiteSpace(shopId))
                throw new ParameterRequiredException("ShopId");

            if (page < 1) page = 1; if (per_page < 1) per_page = 5;

            var shopOrders = _service.GetAllShopOrders(shopId, per_page, page, out totalItems);

            if (shopOrders == null)
                throw new EntityNotFoundException(nameof(ShopOrder), shopId);

            // Set headers for paging
            Response.Headers.Add("X-Total", totalItems.ToString());
            Response.Headers.Add("X-Total-Pages", (totalItems / per_page).ToString());
            Response.Headers.Add("X-Per-Page", per_page.ToString());
            Response.Headers.Add("X-Page", page.ToString());

            return Ok(shopOrders);
        }

        /// <summary>
        /// Get specific Shop Order (Returns: CustomerOrder)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shopId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("/orders/{orderId}/{shopId}")]
        public ActionResult<ShopOrder> GetSpecificShopOrder([FromRoute] string orderId, [FromRoute] string shopId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var shopOrder = _service.GetSpecificShopOrder(orderId, shopId);

            if (shopOrder == null)
                throw new EntityNotFoundException(nameof(ShopOrder), orderId);

            return Ok(shopOrder);
        }

        /// <summary>
        /// Accept order (Returns: ShopOrder)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shopId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("/orders/{orderId}/{shopId}/accept")]
        public ActionResult<ShopOrder> AcceptOrder([FromRoute] string orderId, [FromRoute] string shopId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var shopOrder = _service.UpdateOrder(orderId, shopId, "Order Accepted");
            Log.Information("Order Accepted:", shopOrder.ToJson());

            if (shopOrder == null)
            {
                throw new EntityNotFoundException(nameof(ShopOrder), orderId);
            }
            else
            {
                _service.PublishMessage(shopOrder, TopicNames.ShopAcceptedOrder);
                _service.PublishMessage(shopOrder, TopicNames.DeliveryRequested);
            }

            return Ok(shopOrder);
        }

        /// <summary>
        /// Reject Order (Returns: ShopOrder)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shopId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("/orders/{orderId}/{shopId}/reject")]
        public ActionResult<ShopOrder> RejectOrder([FromRoute] string orderId, [FromRoute] string shopId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var shopOrder = _service.UpdateOrder(orderId, shopId, "Order Rejected");
            Log.Information("Order Rejected:", shopOrder.ToJson());

            if (shopOrder == null)
            {
                throw new EntityNotFoundException(nameof(ShopOrder), orderId);
            }
            else
            {
                _service.PublishMessage(shopOrder, TopicNames.ShopRejectedOrder);
            }

            return Ok(shopOrder);
        }

        /// <summary>
        /// Order is ready (Returns: ShopOrderReadyRecord)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="shopId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("/orders/{orderId}/{shopId}/ready")]
        public ActionResult<ShopOrder> OrderReady([FromRoute] string orderId, [FromRoute] string shopId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var shopOrder = _service.UpdateOrder(orderId, shopId, "Order Ready");
            Log.Information("Order Ready:", shopOrder.ToJson());

            if (shopOrder == null)
            {
                throw new EntityNotFoundException(nameof(ShopOrder), orderId);
            }
            else
            {
                _service.PublishMessage(shopOrder, TopicNames.OrderReady);
            }

            return Ok(shopOrder);
        }
    }
}
