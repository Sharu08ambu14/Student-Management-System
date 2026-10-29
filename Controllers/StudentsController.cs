using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Data;
using StudentManagement.Models;

namespace StudentManagement.Controllers;

public class StudentsController(SchoolContext context) : Controller
{
    public async Task<IActionResult> Index(string? search, string? department, string? status, int page = 1)
    {
        const int pageSize = 8;
        var query = context.Students.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.Name.Contains(search) || s.Email.Contains(search) || s.StudentNumber.Contains(search));
        if (!string.IsNullOrWhiteSpace(department)) query = query.Where(s => s.Department == department);
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(s => s.Status == status);

        var total = await query.CountAsync();
        ViewBag.Search = search;
        ViewBag.Department = department;
        ViewBag.Status = status;
        ViewBag.Departments = await context.Students.Select(s => s.Department).Distinct().OrderBy(s => s).ToListAsync();
        ViewBag.Total = await context.Students.CountAsync();
        ViewBag.Active = await context.Students.CountAsync(s => s.Status == "Active");
        ViewBag.Average = total == 0 ? 0 : await context.Students.AverageAsync(s => (double)s.Marks);
        ViewBag.Page = page;
        ViewBag.Pages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Filtered = total;
        return View(await query.OrderBy(s => s.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    public IActionResult Create() => View(new Student { EnrollmentDate = DateTime.Today });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Student student)
    {
        if (await context.Students.AnyAsync(s => s.StudentNumber == student.StudentNumber))
            ModelState.AddModelError(nameof(student.StudentNumber), "This student ID is already in use.");
        if (!ModelState.IsValid) return View(student);
        context.Add(student);
        await context.SaveChangesAsync();
        TempData["Success"] = $"{student.Name} was added to your students.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var student = await context.Students.FindAsync(id);
        return student is null ? NotFound() : View(student);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Student student)
    {
        if (id != student.Id) return NotFound();
        if (await context.Students.AnyAsync(s => s.StudentNumber == student.StudentNumber && s.Id != id))
            ModelState.AddModelError(nameof(student.StudentNumber), "This student ID is already in use.");
        if (!ModelState.IsValid) return View(student);
        context.Update(student);
        await context.SaveChangesAsync();
        TempData["Success"] = $"{student.Name}'s details were updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var student = await context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        return student is null ? NotFound() : View(student);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await context.Students.FindAsync(id);
        if (student is not null)
        {
            context.Students.Remove(student);
            await context.SaveChangesAsync();
            TempData["Success"] = $"{student.Name} was removed from your students.";
        }
        return RedirectToAction(nameof(Index));
    }
}
