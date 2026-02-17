using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Controllers
{
    public class ExpensesController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        // GET: Expenses
        public async Task<IActionResult> Index(ExpenseFilterViewModel filter)
        {
            if (filter.PageNumber < 1) filter.PageNumber = 1;
            if (filter.PageSize < 1) filter.PageSize = 10;

            var result = await _expenseService.GetFilteredExpensesAsync(filter);
            return View(result);
        }

        // GET: Expenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseService.GetExpenseByIdAsync(id.Value);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // GET: Expenses/Create
        public IActionResult Create()
        {
            var expense = new Expense { Date = DateTime.Now };
            return View(expense);
        }

        // POST: Expenses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Amount,Category,Date")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                await _expenseService.CreateExpenseAsync(expense);
                TempData["SuccessMessage"] = "Expense created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseService.GetExpenseByIdAsync(id.Value);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        // POST: Expenses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Amount,Category,Date,CreatedAt")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _expenseService.UpdateExpenseAsync(expense);
                    TempData["SuccessMessage"] = "Expense updated successfully!";
                }
                catch
                {
                    var expenseExists = await _expenseService.GetExpenseByIdAsync(id);
                    if (expenseExists == null)
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _expenseService.GetExpenseByIdAsync(id.Value);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _expenseService.DeleteExpenseAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Expense deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Error deleting expense.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
