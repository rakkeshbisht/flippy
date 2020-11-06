using System.Collections.Generic;
using DeliveryAgent.Filters;
using DeliveryAgent.Models;
using DeliveryAgent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DeliveryAgent.Controllers
{
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly Service _service;
        private IConfiguration _config;
        public DeliveryController(Service service, IConfiguration config)
        {
            _service = service;
            _config = config;
        }

        /// <summary>
        /// Get all Agent Orders (paginated) (Returns: AgentOrdersList)
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="per_page"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("/orders")]
        public ActionResult<List<DeliveryAgentOrder>> GetAllAgentOrders([FromQuery] string agentId, [FromQuery] int per_page = 5, [FromQuery] int page = 1)
        {
            long totalItems;

            if (string.IsNullOrWhiteSpace(agentId))
                throw new ParameterRequiredException("AgentId");

            if (page < 1) page = 1; if (per_page < 1) per_page = 5;

            var deliveryAgentOrders = _service.GetAllAgentOrders(agentId, per_page, page, out totalItems);

            if (deliveryAgentOrders == null)
                throw new EntityNotFoundException(nameof(DeliveryAgentOrder), agentId);

            // Set headers for paging
            Response.Headers.Add("X-Total", totalItems.ToString());
            Response.Headers.Add("X-Total-Pages", (totalItems / per_page).ToString());
            Response.Headers.Add("X-Per-Page", per_page.ToString());
            Response.Headers.Add("X-Page", page.ToString());

            return Ok(deliveryAgentOrders);
        }

        /// <summary>
        /// Get specific Shop Order (Returns: CustomerOrder)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="agentId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("/orders/{orderId}/{agentId}")]
        public ActionResult<DeliveryAgentOrder> GetSpecificAgentOrder([FromRoute] string orderId, [FromRoute] string agentId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var specificAgentOrder = _service.GetSpecificAgentOrder(orderId, agentId);

            if (specificAgentOrder == null)
                throw new EntityNotFoundException(nameof(DeliveryAgentOrder), orderId);

            return Ok(specificAgentOrder);
        }


        /// <summary>
        /// Arrived at Shop (Returns: AgentArrivedRecord)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="agentId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("/orders/{orderId}/{agentId}/arrive")]
        public ActionResult<DeliveryAgentOrder> ArrivedAtShop([FromRoute] string orderId, [FromRoute] string agentId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var agentOrder = _service.UpdateAgentOrder(orderId, agentId, "Agent Arrived At Shop");
            Log.Information("Agent Arrived At Shop:" + agentOrder.ToJson());

            if (agentOrder == null)
            {
                throw new EntityNotFoundException(nameof(DeliveryAgentOrder), orderId);
            }
            else
            {
                _service.PublishMessage(agentOrder, TopicNames.AgentArrivedAtShop);
            }

            return Ok(agentOrder);
        }

        /// <summary>
        /// Picked up Delivery (Returns: AgentPickupRecord)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="agentId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("/orders/{orderId}/{agentId}/pickup")]
        public ActionResult<DeliveryAgentOrder> PickedUpDelivery([FromRoute] string orderId, [FromRoute] string agentId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var agentOrder = _service.UpdateAgentOrder(orderId, agentId, "Picked Up Delivery");
            Log.Information("Agent Picked Up Delivery:" + agentOrder.ToJson());

            if (agentOrder == null)
            {
                throw new EntityNotFoundException(nameof(DeliveryAgentOrder), orderId);
            }
            else
            {
                _service.PublishMessage(agentOrder, TopicNames.OrderPickedUp);
            }

            return Ok(agentOrder);
        }

        /// <summary>
        /// Order Delivered (Returns: AgentDeliveredRecord)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="agentId"></param>        
        /// <returns></returns>
        [Authorize]
        [HttpPut]
        [Route("/orders/{orderId}/{agentId}/deliver")]
        public ActionResult<DeliveryAgentOrder> OrderDelivered([FromRoute] string orderId, [FromRoute] string agentId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ParameterRequiredException("orderId");

            var agentOrder = _service.UpdateAgentOrder(orderId, agentId, "Order Delivered");
            Log.Information("Agent delivered order :" + agentOrder.ToJson());

            if (agentOrder == null)
            {
                throw new EntityNotFoundException(nameof(DeliveryAgentOrder), orderId);
            }
            else
            {
                _service.PublishMessage(agentOrder, TopicNames.OrderDelivered);
            }

            return Ok(agentOrder);
        }
    }
}
