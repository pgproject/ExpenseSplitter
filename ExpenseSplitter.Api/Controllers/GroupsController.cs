using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ExpenseSplitter.Application.DTOs.Groups;
using ExpenseSplitter.Application.Interfaces.Groups;

namespace ExpenseSplitter.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateGroupRequest request)
        {
            var id = await _groupService.CreateAsync(request);

            return Ok(id);
        }
    }
}
