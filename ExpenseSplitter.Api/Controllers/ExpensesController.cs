using ExpenseSplitter.Application.DTOs.Expense.Request;
using ExpenseSplitter.Application.DTOs.Expense.Response;
using ExpenseSplitter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/expenses")]
    public class ExpensesController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateExpenseResponse>> Create(
            [FromBody] CreateExpenseRequest request)
        {
            var response = await _expenseService.CreateAsync(request);

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<UpdateExpenseResponse>> Update(
            [FromBody] UpdateExpenseRequest request)
        {
            var response = await _expenseService.UpdateAsync(request);

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<RemoveExpenseResponse>> Remove(
            [FromBody] RemoveExpenseRequest request)
        {
            var response = await _expenseService.RemoveAsync(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetExpensesResponse>>> GetAll()
        {
            var expenses = await _expenseService.GetAllAsync();
            return Ok(expenses);
        }

        [HttpGet("group/{groupId:guid}")]
        public async Task<ActionResult<List<GetExpensesResponse>>> GetByGroup(
            Guid groupId)
        {
            var expenses = await _expenseService.GetByGroupAsync(groupId);

            return Ok(expenses);
        }
    }
}
