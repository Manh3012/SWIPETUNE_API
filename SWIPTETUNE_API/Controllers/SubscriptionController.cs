using Repository.Interface;
using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SWIPTETUNE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IAccountRepository accountRepository;

        public SubscriptionController(ISubscriptionRepository subscriptionRepository, IAccountRepository accountRepository)
        {
            this.subscriptionRepository = subscriptionRepository;
            this.accountRepository = accountRepository;
        }

        [HttpGet]
        [Route("GetSubscriptions")]
        public async Task<IActionResult> GetAll()
        {
            List<Subscription> list = new List<Subscription>();
            try
            {
                list = await subscriptionRepository.GetSubscriptions();
            }
            catch (Exception ex)
            {
                throw new Exception("Nothing to show");
            }
            return Ok(list);
        }

        [HttpDelete]
        [Route("DeleteSubscription")]
        public async Task<IActionResult> DeleteSubsciption(int id)
        {
            var isDeleted = false;
            try
            {
                isDeleted = subscriptionRepository.DeleteSubscription(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete");
            }
            if (isDeleted)
            {
                return Ok("Delete Success");
            }
            else
            {
                return BadRequest("Failed to delete");
            }
        }
        [HttpPost]
        [Route("AddSubscription")]
        public async Task<IActionResult> AddSubsciption(Subscription sub)
        {
            try
            {
                await subscriptionRepository.AddSubscription(sub);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to add");
            }
            return Ok();
        }

        [HttpPut]
        [Route("UpdateSubscription")]
        public async Task<IActionResult> UpdateSubsciption(Subscription sub)
        {
            try
            {
                await subscriptionRepository.UpdateSubscription(sub);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update");
            }
            return Ok();
        }

        [HttpPost]
        [Route("SetAccountSubscriptionToFree/{id}")]
        public async Task<IActionResult> AddAccountSub(Guid id)
        {

            try
            {
                var account = await accountRepository.GetUserById(id);
                if (account == null)
                {
                    throw new Exception("Account does not exist");
                }
                Guid accountId = account.Id;
                subscriptionRepository.AddAccountSubscription(accountId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Set to Free");


        }
        [HttpPut]
        [Route("UpdateToPremium/{id}")]
        public async Task<IActionResult> UpdateToPremium(Guid id)
        {
            try
            {
                var account = await accountRepository.GetUserById(id);
                if (account == null)
                {
                    throw new Exception("Account does not exist");
                }
                Guid accountId = account.Id;
                subscriptionRepository.UpdateToPremium(accountId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Set to Premium");
        }
    }
}
