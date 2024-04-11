using Microsoft.AspNetCore.Mvc;

namespace IDS_TFI.Controllers
{
	[Route("api/ping")]
	[ApiController]
	public class PingController : ControllerBase
	{
		// GET: api/ping
		[HttpGet]
		public ActionResult<string> Ping()
		{
			return Ok("pong");
		}
	}
}
