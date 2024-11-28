﻿using CheckListJob.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckListJob.Controllers;
[Authorize(Roles = "admin")]
public class ShiftController : Controller
{
    readonly CheckListContext listContext = new();
    Shift? shiftSelected;

    public IActionResult ListShift() => View(listContext.Shifts);

    [HttpGet]
    public IActionResult CreateShift() => View();

    [HttpPost]
    public IActionResult CreateShift(Shift shift)
    {
        if (ModelState.IsValid)
        {
            listContext.Shifts.Add(shift);
            listContext.SaveChanges();
            TempData["Review"] = "Смена добавлена";
            return RedirectToAction("CreateShift");

        }
        return View(shift);
    }

    [HttpGet]
    public IActionResult EditShift(int shiftId)
    {
        shiftSelected = listContext.Shifts.Find(shiftId);
        if (shiftSelected == null)
            return RedirectToAction("ErrorIndex", "Home");
         return View(shiftSelected);

    }

    [HttpPost]
    public IActionResult EditShift(Shift shift)
    {
        if (ModelState.IsValid)
        {
            listContext.Entry(shift).State = EntityState.Modified;
            listContext.SaveChanges();
            return RedirectToAction("ListShift");
        }
        return View(shift);
    }


}
