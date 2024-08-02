using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoListExam.Models.ViewModels;
using ToDoListExam.ToDoList;

namespace ToDoListExam.Controllers
{
    public class ToDoListController : Controller
    {
        private readonly ToDoListContext _context;
        private readonly UserManager<IdentityUser> userManager;
        IdentityUser? user;
        public ToDoListController(ToDoListContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            this.userManager = userManager;  
        }
        public async Task<IActionResult> Index(int? SelectedCategoryId = -1, string? SelectedEnded = "Всі", string? SelectedSort = "Пріорітет", string? Search="")
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToAction("Login", "Account");
            else
                user = await userManager.GetUserAsync(User);
            
            ToDoListMainViewModel model = new ToDoListMainViewModel()
            {
                Items = await _context.ToDoListItems.Include(t => t.Category).Where(t => t.UserId == user!.Id).ToListAsync(),
                Categories = new SelectList(_context.Categories, "Id", "CategoryName"),
                SelectedCategoryId = SelectedCategoryId,
                SelectedEnded = SelectedEnded,
                SelectedSort = SelectedSort,
                Search = Search
            };

            // Фільтр за категорією
            if(SelectedCategoryId != -1)
                model.Items = model.Items.Where(c => c.CategoryId == SelectedCategoryId).ToList();
            // Фільтр за виконанням
            switch(SelectedEnded)
            {
                case "Виконані":
                    model.Items = model.Items.Where(c => c.IsEnded == true).ToList();
                    break;
                case "Не виконані":
                    model.Items = model.Items.Where(c => c.IsEnded == false).ToList();
                    break;
            }
            // Фільтр пріорітет/дата
            switch(SelectedSort)
            {
                case "Пріорітет":
                    model.Items = model.Items.OrderByDescending(c => c.Priority).ToList();
                    break;
                case "Дата виконання":
                    model.Items = model.Items.OrderBy(c => c.CompleteDate).ToList();
                    break;
            }
            // Пошук за назвою чи описом
            if (Search != "")
                model.Items = model.Items.Where(c => c.Name.ToLower().Contains(Search.ToLower())).Union(model.Items.Where(c => c.Description.ToLower().Contains(Search.ToLower()))).Distinct().ToList();
            
            return View(model);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            var toDoListItem = await _context.ToDoListItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoListItem == null)
                return NotFound();
            return View(toDoListItem);
        }
        public async Task<IActionResult> Create()
        {
            if (User.Identity.IsAuthenticated == false)
                return RedirectToAction("Login", "Account");
            else
                user = await userManager.GetUserAsync(User);
            ToDoListItemViewModel viewModel = new ToDoListItemViewModel()
            {
                ToDoListItem = new ToDoListItem() { UserId = user.Id },
                Categories = new SelectList(_context.Categories, "Id", "CategoryName")
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoListItemViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.ToDoListItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Categories = new SelectList(_context.Categories, "Id", "CategoryName");
            return View(viewModel);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var toDoListItem = await _context.ToDoListItems.FindAsync(id);
            if (toDoListItem == null) return NotFound();
            ToDoListItemViewModel viewModel = new ToDoListItemViewModel()
            {
                ToDoListItem = toDoListItem,
                Categories = new SelectList(_context.Categories, "Id", "CategoryName")
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToDoListItemViewModel viewModel)
        {
            if (id != viewModel.ToDoListItem.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.ToDoListItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToDoListItemExists(viewModel.ToDoListItem.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var toDoListItem = await _context.ToDoListItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toDoListItem == null)
                return NotFound();
            return View(toDoListItem);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toDoListItem = await _context.ToDoListItems.FindAsync(id);
            if (toDoListItem != null)
            {
                _context.ToDoListItems.Remove(toDoListItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToDoListItemExists(int id)
        {
            return _context.ToDoListItems.Any(e => e.Id == id);
        }
        public async Task<IActionResult> MakeChecked(int? id)
        {
            if (id == null) return NotFound();

            var toDoListItem = await _context.ToDoListItems.FindAsync(id);
            if (toDoListItem == null) return NotFound();

            toDoListItem.IsEnded = !toDoListItem.IsEnded;
            _context.Update(toDoListItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
