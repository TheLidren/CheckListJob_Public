using CheckListJob.Models;
using CheckListJob.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CheckListJob.Controllers;
public class UserController : Controller
{
    readonly CheckListContext listContext = new();
    User? userSelect;

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult ListUser() => View(listContext.Users.Include(r => r.Role).ToList());

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult DeleteUser(int userId)
    {
        userSelect = listContext.Users.Find(userId);
        if (userSelect != null)
        {
            userSelect.Status = userSelect.Status ? false : true;
            listContext.Entry(userSelect).State = EntityState.Modified;
            listContext.SaveChanges();
            return RedirectToAction("ListUser");
        }
        else
            return RedirectToAction("ErrorIndex", "Home");

    }

    [HttpGet] //Авторизация
    public IActionResult SignIn() => View();

    [HttpPost]
    public async Task<IActionResult> SignIn(User user)
    {
        if (string.IsNullOrEmpty(user.Password))
        {
            ModelState.AddModelError("Password", "Проверьте корректность ввода данных");
            return View(user);
        }
        userSelect = listContext.Users.Where(u => u.Login == user.Login && u.Password == user.Password.ToSHA256String()).Include(r => r.Role).FirstOrDefault();
        if (userSelect == null)
        {
            ModelState.AddModelError("Password", "Проверьте корректность ввода данных");
            return View(user);
        }
        if (!userSelect.Status)
        {
            ModelState.AddModelError("Login", "Администратору необходимо включить учётную запись");
            return View(user);
        }
        var claims = new List<Claim> { new(ClaimTypes.Name, userSelect.Login), new(ClaimTypes.Role, userSelect.Role!.Name) };
        ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        return RedirectToAction("WelcomeIndex", "Home");
    }

    [HttpGet] //Регистрация
    public IActionResult SignUp() => View();

    [HttpPost] 
    public IActionResult SignUp(User user)
    {
        if (ModelState.IsValid)
        {
            userSelect = listContext.Users.Where(m => m.Login == user.Login).FirstOrDefault();
            if (userSelect != null)
            {
                ModelState.AddModelError("Login", "Данный login уже зарегестрирован в системе");
                return View(user);
            }
            user.Status = false; 
            user.RoleId = listContext.Roles.Where(m => m.Name == "user").FirstOrDefault()!.Id; 
            user.Password = user.Password.ToSHA256String();
            listContext.Users.Add(user);
            listContext.SaveChanges();
            foreach (var item in listContext.ShiftTasks)
                listContext.ShiftTaskHsts.Add(new ShiftTaskHst { ShiftTaskId = item.Id, UserId = user.Id });
            listContext.SaveChanges();
            return RedirectToAction("SignIn");
        }
        return View();
    }

    [HttpGet]
    public async new Task<IActionResult> SignOut()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("SignIn");
    }

    [HttpGet]
    public IActionResult EditPassword() =>View();

    [HttpPost]
    public IActionResult EditPassword(ResetPasswordViewModel reset)
    {
        if (ModelState.IsValid)
        {
            userSelect = listContext.Users.Where(u => u.Login == HttpContext.User.Identity!.Name).FirstOrDefault();
            if (userSelect != null)
            {
                userSelect.Password = reset.NewPassword.ToSHA256String();
                listContext.Entry(userSelect).State = EntityState.Modified;
                listContext.SaveChanges();
                return RedirectToAction("SignOut");
            }
            else return RedirectToAction("ErrorIndex", "Home");
        }
        return View(reset);
    }

    [HttpGet, Authorize]
    public IActionResult EditUser()
    {
        userSelect = listContext.Users.Where(u => u.Login == HttpContext.User.Identity!.Name).FirstOrDefault();
        if (userSelect == null)
            return RedirectToAction("ErrorIndex", "Home");
        UserViewModel userView = new() { Id = userSelect.Id, Login = userSelect.Login, Name = userSelect.Name, Surname = userSelect.Surname, Patronymic = userSelect.Patronymic };
        return View(userView);
    }

    [HttpPost, Authorize]
    public IActionResult EditUser(UserViewModel userView)
    {
        if (ModelState.IsValid)
        {
            userSelect = listContext.Users.Where(u => u.Login == HttpContext.User.Identity!.Name).FirstOrDefault();
            if (userSelect == null)
                return RedirectToAction("ErrorIndex", "Home");
            userSelect.Name = userView.Name; userSelect.Surname = userView.Surname; userSelect.Patronymic = userView.Patronymic;
            userSelect.Login = userView.Login;
            listContext.Entry(userSelect).State = EntityState.Modified;
            listContext.SaveChanges();
            if (userSelect.Login != HttpContext.User.Identity!.Name)
                return RedirectToAction("SignOut");
            return RedirectToAction("WelcomeIndex", "Home");
        }
        return View(userView);
    }

    [HttpGet, Authorize(Roles = "admin")]
    public IActionResult ResetPassword(int userId)
    {
        userSelect = listContext.Users.Find(userId);
        if (userSelect != null)
        {
            ResetPasswordViewModel reset = new()
            {
                Login = userSelect.Login
            };
            return View(reset);
        }
        else return RedirectToAction("ErrorIndex", "Home");
    }
    [HttpPost, ValidateAntiForgeryToken, Authorize(Roles = "admin")]
    public IActionResult ResetPassword(ResetPasswordViewModel reset)
    {
        userSelect = listContext.Users.Where(u => u.Login == reset.Login).FirstOrDefault();
        if (userSelect != null)
        {
            userSelect.Password = reset.NewPassword.ToSHA256String();
            listContext.Entry(userSelect).State = EntityState.Modified;
            listContext.SaveChanges();
            return RedirectToAction("ListUser");
        }
        else return RedirectToAction("ErrorIndex", "Home");
    }

    [HttpGet, Authorize(Roles = "admin")]
    public IActionResult EditRole(int userId)
    {
        userSelect = listContext.Users.Find(userId);
        if (userSelect != null)
        {
            RoleViewModel roleViewModel = new()
            {
                User = userSelect,
                Roles = new SelectList(listContext.Roles, "Id", "Name", userSelect.RoleId)
            };
            return View(roleViewModel);
        }
        else return RedirectToAction("ErrorIndex", "Home");
    }

    [HttpPost, Authorize(Roles = "admin")]
    public IActionResult EditRole(int userId, int roleId)
    {
        userSelect = listContext.Users.Find(userId);
        if (userSelect != null)
        {
            Role? role = listContext.Roles.Find(roleId);
            if (role is null)
                return RedirectToAction("ErrorIndex", "Home");
            userSelect.RoleId = role.Id;
            listContext.Entry(userSelect).State = EntityState.Modified;
            listContext.SaveChanges();
            return RedirectToAction("ListUser");
        }
        else return RedirectToAction("ErrorIndex", "Home");
    }

}
