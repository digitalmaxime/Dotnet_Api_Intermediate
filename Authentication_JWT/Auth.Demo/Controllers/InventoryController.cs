using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Demo.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    // GET
    [Authorize(Roles = "Administrator, User")]
    [HttpGet]
    public ActionResult<string []> GetInventory()
    {
        return Ok(new string[] { "Value1", "Value2" });
    }


    // POST
    [Authorize(Roles = "Administrator, User")]
    [HttpPost]
    public IActionResult PostItemToInventory([FromBody] Item item)
    {
        return Ok();
    }
}

public class Item
{
    public int Id { get; set; }
    public string Description { get; set; }
}