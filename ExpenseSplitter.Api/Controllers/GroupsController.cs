using ExpenseSplitter.Application.DTOs.Groups.Request;
using ExpenseSplitter.Application.DTOs.Groups.Response;
using ExpenseSplitter.Application.Interfaces.Groups;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/groups")]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateGroupResponse>> Create(
            [FromBody] CreateGroupRequest request)
        {
            var response = await _groupService.CreateAsync(request);
            return Ok(response);
        }

        [HttpPost("members")]
        public async Task<ActionResult<AddMemberResponse>> AddMember(
            [FromBody] AddMemberRequest request)
        {
            var response = await _groupService.AddMember(request);
            return Ok(response);
        }

        [HttpDelete("members")]
        public async Task<ActionResult<RemoveMemberResponse>> RemoveMember(
            [FromBody] RemoveMemberRequest request)
        {
            var response = await _groupService.RemoveMember(request);
            return Ok(response);
        }

        [HttpPut("ownership")]
        public async Task<ActionResult<ChangeOwnershipResponse>> ChangeOwnership(
            [FromBody] ChangeOwnershipRequest request)
        {
            var response = await _groupService.ChangeOwnership(request);
            return Ok(response);
        }
    }
}
