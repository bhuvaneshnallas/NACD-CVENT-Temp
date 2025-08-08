using Event_Grid_Sync_Poc.Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Event_Grid_Sync_Poc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventSyncController : ControllerBase
    {
        public readonly EventSync _eventSync;

        public EventSyncController(EventSync eventSync)
        {
            _eventSync = eventSync;
        }


        [HttpGet("CventToCosmos")]
        public async Task<ActionResult> EventSyncToCosmos(string fetchAfter)
        {
            try
            {
                await _eventSync.FetchDataWithRetryAsync("Event", fetchAfter);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpGet("CosmostoEventgrid")]
        public async Task<ActionResult> CosmosToEventGridSync()
        {
            try
            {
                
                await _eventSync.ProcessAndPublishUnprocessedEventsAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        [HttpPatch("UpdateSFEventId")]
        public async Task<ActionResult> UpdateSFEventId(string sdId,string eventId)
        {
            try
            {
                await _eventSync.UpdateSfId(sdId,eventId);
                return Ok();

            }catch(Exception ex)
            {
                return NotFound();
            }
        }

    }
}
