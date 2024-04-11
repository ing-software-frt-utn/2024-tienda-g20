#if DEBUG
using Microsoft.AspNetCore.Mvc;

namespace IDS_TFI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ResetController(DataContext context) : ControllerBase
	{
		private readonly DataContext context = context;

		[HttpPost]
		public ActionResult ResetDB()
		{
			if (context.Reset())
				return Ok();
			else
				return BadRequest();
		}
	}
}
#endif