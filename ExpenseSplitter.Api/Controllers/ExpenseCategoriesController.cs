using ExpenseSplitter.Application.DTOs.ExpenseCategory.Request;
using ExpenseSplitter.Application.DTOs.ExpenseCategory.Response;
using ExpenseSplitter.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/expense-categories")]
    public class ExpenseCategoriesController : ControllerBase
    {
        private readonly IExpenseCategoryService _expenseCategoryService;

        public ExpenseCategoriesController(
            IExpenseCategoryService expenseCategoryService)
        {
            _expenseCategoryService = expenseCategoryService;
        }

        [HttpPost]
        public async Task<ActionResult<CreateExpenseCategoryResponse>> Create(
            [FromBody] CreateCategoryExpenseRequest request)
        {
            var response = await _expenseCategoryService.CreateCategoryExpense(request);

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<UpdateExpenseCategoryResponse>> Update(
            [FromBody] UpdateExpenseCategoryRequest request)
        {
            var response = await _expenseCategoryService.UpdateCategoryExpense(request);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetExpenseCategoriesResponse>>> GetAll()
        {
            var response = await _expenseCategoryService.GetAllAsync();

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<RemoveExpenseCategoryResponse>> Remove(
            [FromBody] RemoveCategoryExpenseRequest request)
        {
            var response = await _expenseCategoryService.RemoveCategoryExpense(request);

            return Ok(response);
        }
    }
}
