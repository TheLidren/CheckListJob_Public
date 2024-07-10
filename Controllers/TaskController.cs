using CheckListJob.Models;
using CheckListJob.ViewModels.AdminShift;
using CheckListJob.ViewModels.Journal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CheckListJob.Controllers
{
    public class TaskController : Controller
    {
        private readonly CheckListContext listContext = new();
        private User user = null!;
        private ListLog listLog = null!;
        private ShiftTaskHst hst = null!;
        private readonly string AppId = "c30481e2-2ab2-415a-b092-9419f4b413f4";
        private readonly string ApiKey = "ZTdkMDZiNGQtNTkwYy00Njk5LTk3MGYtODA5ZTY4NzVkY2I4";
        private readonly string ApiUrl = "https://onesignal.com/api/v1/notifications";

        public void SendPushNotification(string name, short shiftId)
        {
            var webRequest = WebRequest.Create($"{ApiUrl}") as HttpWebRequest;
            webRequest.KeepAlive = true;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json; charset=utf-8";
            webRequest.Headers.Add("authorization", $"Basic {ApiKey}");

            var obj = new
            {
                app_id = AppId,
                headings = new { en = name }, //Заголовок push-уведомления
                contents = new { en = "Пора выполнить задание" }, //описание
                included_segments = new string[] { "All" },
                url = $"Shift/ListShift/?shiftId={shiftId}"
            };
            var param = JsonConvert.SerializeObject(obj);
            var byteArray = Encoding.UTF8.GetBytes(param);

            using (var writer = webRequest.GetRequestStream())
            {
                writer.Write(byteArray, 0, byteArray.Length);
            }

            using var response = webRequest.GetResponse() as HttpWebResponse;
            if (response != null)
            {
                using var reader = new StreamReader(response.GetResponseStream());
                var responseContent = reader.ReadToEnd();
            }
        }


        [Authorize(Roles = "user,admin")]
        public IActionResult ListTask(short shiftId, SortState sortShift = SortState.StartTimeAsc)
        {

            ViewData["shiftId"] = shiftId;
            ViewData["taskName"] = "";
            ViewData["StartTimeSort"] = sortShift == SortState.StartTimeAsc ? SortState.StartTimeDesc : SortState.StartTimeAsc;
            ViewData["FinishTimeSort"] = sortShift == SortState.FinishTimeAsc ? SortState.FinishTimeDesc : SortState.FinishTimeAsc;
            ViewData["NumberSort"] = sortShift == SortState.NumberAsc ? SortState.NumberDesc : SortState.NumberAsc;
            IQueryable<ShiftTask> listShifts = listContext.ShiftTasks.Where(a => a.ShiftId == shiftId && a.Status && (a.LastAction.Value.Date < DateTime.Now.Date || a.LastAction == null));
            listShifts = sortShift switch
            {
                SortState.StartTimeAsc => listShifts.OrderBy(s => s.StartTime),
                SortState.StartTimeDesc => listShifts.OrderByDescending(s => s.StartTime),
                SortState.FinishTimeAsc => listShifts.OrderBy(s => s.FinishTime),
                SortState.FinishTimeDesc => listShifts.OrderByDescending(s => s.FinishTime),
                SortState.NumberAsc => listShifts.OrderBy(s => s.Number),
                SortState.NumberDesc => listShifts.OrderByDescending(s => s.Number),
                _ => throw new NotImplementedException(),
            };
            hst = listContext.ShiftTaskHsts.Include(u => u.ShiftTask).Include(u => u.User)
                .Where(u => u.ShiftTask.StartTime < DateTime.Now.TimeOfDay 
                        && u.ShiftTask.FinishTime > DateTime.Now.TimeOfDay 
                        && u.User.Login == HttpContext.User.Identity.Name 
                        && (u.LastNotify.Value.Date < DateTime.Now.Date || u.LastNotify.Value.Date == null) 
                        && (u.ShiftTask.LastAction.Value.Date < DateTime.Now.Date || u.ShiftTask.LastAction == null)
                        && u.ShiftTask.NotifyUser == true && u.ShiftTask.Status == true).FirstOrDefault();
            if (hst != null)
            {
                //SendPushNotification(hst.ShiftTask.Name, shiftId);
                ViewData["taskName"] = hst.ShiftTask.Name;
                hst.LastNotify = DateTime.Now;
                listContext.Entry(hst).State = EntityState.Modified;
                listContext.SaveChanges();
            }
            return View(listShifts);
        }

        [Authorize(Roles = "admin")]
        public IActionResult AdminShift(string? tittle, short shiftId = 0, AdminShiftEnum pageSort = AdminShiftEnum.StartTimeAsc)
        {
            IQueryable<ShiftTask> shiftTasks = listContext.ShiftTasks.Include(task => task.Shift);
            if (!string.IsNullOrEmpty(tittle))
                shiftTasks = shiftTasks.Where(t => t.Name.Contains(tittle) || t.Description.Contains(tittle));
            if (shiftId != 0)
                shiftTasks = shiftTasks.Where(s => s.ShiftId == shiftId);
            shiftTasks = pageSort switch
            {
                AdminShiftEnum.StartTimeAsc => shiftTasks.OrderBy(task => task.StartTime),
                AdminShiftEnum.StartTimeDesc => shiftTasks.OrderByDescending(task => task.StartTime),
                AdminShiftEnum.ShiftAsc => shiftTasks.OrderBy(task => task.Shift.Name),
                AdminShiftEnum.ShiftDesc => shiftTasks.OrderByDescending(task => task.Shift.Name),
                AdminShiftEnum.StatusAsc => shiftTasks.OrderBy(task => task.Status),
                AdminShiftEnum.StatusDesc => shiftTasks.OrderByDescending(task => task.Status),
                AdminShiftEnum.NotifyAsc => shiftTasks.OrderBy(task => task.NotifyUser),
                AdminShiftEnum.NotifyDesc => shiftTasks.OrderByDescending(task => task.NotifyUser),
                AdminShiftEnum.NumberAsc => shiftTasks.OrderBy(s => s.Number),
                AdminShiftEnum.NumberDesc => shiftTasks.OrderByDescending(s => s.Number),
                _ => throw new NotSupportedException("Что-то пошло не так при сортировке")

            };
            AdminShiftViewModel adminShiftView = new(listContext.Shifts.ToList())
            {
                ShiftTasks = shiftTasks,
                Title = tittle,
                AdminShiftSort = new(pageSort),
                SelectedShift = shiftId
            };
            return View(adminShiftView);
        }


        [Authorize(Roles = "admin, user")]
        public IActionResult CompleteTask(int taskId, short shiftId)
        {
            user = listContext.Users.Where(u => u.Login == HttpContext.User.Identity.Name).FirstOrDefault();
            listLog = new ListLog { ShiftTaskId = taskId, UserId = user.Id, MarkAction = DateTime.Now };
            listContext.ListLogs.Add(listLog);
            ShiftTask shiftTask = listContext.ShiftTasks.Find(taskId);
            shiftTask.LastAction = DateTime.Now;
            listContext.Entry(shiftTask).State = EntityState.Modified;

            listContext.SaveChanges();
            return RedirectToAction("ListTask", new { shiftId });
        }

        [Authorize(Roles = "admin")]
        public IActionResult JournalAction(int userId = 0, short shiftId = 0, int countRows = 50, JournalEnum pageSort = JournalEnum.MarkActionDesc, DateTime? dateStart = null, DateTime? dateEnd = null)
        {
            IQueryable<ListLog> listLogs = listContext.ListLogs.Include(user => user.User).Include(task => task.ShiftTask).Include(shift => shift.ShiftTask.Shift);
            if (userId != 0)
                listLogs = listLogs.Where(u => u.UserId == userId);
            if (shiftId != 0)
                listLogs = listLogs.Include(task => task.ShiftTask).Where(u => u.ShiftTask.ShiftId == shiftId);
            if (dateStart != null)
                listLogs = listLogs.Where(u => u.MarkAction.Date >= dateStart);
            if (dateEnd != null)
                listLogs = listLogs.Where(u => u.MarkAction.Date <= dateEnd);
            listLogs = pageSort switch
            {
                JournalEnum.MarkActionAsc => listLogs.OrderBy(act => act.MarkAction),
                JournalEnum.MarkActionDesc => listLogs.OrderByDescending(act => act.MarkAction),
                JournalEnum.EmailAsc => listLogs.OrderBy(act => act.User.Login),
                JournalEnum.EmailDesc => listLogs.OrderByDescending(act => act.User.Login),
                JournalEnum.ShiftAsc => listLogs.OrderBy(act => act.ShiftTask.Shift.Name),
                JournalEnum.ShiftDesc => listLogs.OrderByDescending(act => act.ShiftTask.Shift.Name),
                _ => throw new NotSupportedException("Что-то пошло не так при сортировке")

            };
            JournalViewModel journalViewModel = new(listContext.Users.ToList(),listContext.Shifts.ToList())
            {
                Log = listLogs,
                JournalSort = new(pageSort),
                DateStart = dateStart,
                DateEnd = dateEnd,
                CountRows = countRows

            };
            return View(journalViewModel);

        }


        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult CreateTask()
        {
            ViewBag.Shifts = new SelectList(listContext.Shifts, "Id", "Name");
            ViewBag.ShiftTasks = listContext.ShiftTasks;
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult CreateTask(ShiftTask shiftTask)
        {
            ViewBag.Shifts = new SelectList(listContext.Shifts, "Id", "Name", shiftTask.ShiftId);
            IQueryable<ShiftTask>? shiftNumber = listContext.ShiftTasks.Where(x => x.ShiftId == shiftTask.ShiftId && x.Status == true);
            if (shiftNumber.Where(n => n.Number == shiftTask.Number).Count() > 0)
            {
                ViewBag.LastNumber = "Последний номер: " + shiftNumber.Max(u => u.Number);
                ModelState.AddModelError("Number", "Данный номер уже отмечён в заданиях");
            }
            if (ModelState.IsValid)
            {
                shiftTask.Status = true;
                shiftTask.NotifyUser = false;
                if (shiftNumber.Count() > 0)
                    if (shiftNumber.Min(x => x.StartTime) >= new TimeSpan(20, 0, 0))
                    {
                        shiftTask.StartTime = shiftTask.StartTime >= new TimeSpan(0, 0, 0) && shiftTask.StartTime < new TimeSpan(13, 0, 0) ? shiftTask.StartTime.Add(new TimeSpan(1, 0, 0, 0)) : shiftTask.StartTime;
                        shiftTask.FinishTime = shiftTask.FinishTime >= new TimeSpan(0, 0, 0) && shiftTask.FinishTime < new TimeSpan(13, 0, 0) ? shiftTask.FinishTime.Add(new TimeSpan(1, 0, 0, 0)) : shiftTask.FinishTime;
                    }
                listContext.ShiftTasks.Add(shiftTask);
                listContext.SaveChanges();
                foreach (var item in listContext.Users)
                {
                    listContext.ShiftTaskHsts.Add(new ShiftTaskHst { ShiftTaskId = shiftTask.Id, UserId = item.Id });
                }
                listContext.SaveChanges();
                TempData["Review"] = "Задание добавлено";
                return RedirectToAction("CreateTask") ;
            }
            return View(shiftTask);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult EditTask(int taskId)
        {
            ShiftTask listShift = listContext.ShiftTasks.Find(taskId);
            if (listShift == null) return RedirectToAction("ErrorIndex", "Home");
            if (listShift.StartTime.Days >= 1)
                listShift.StartTime = listShift.StartTime.Add(new TimeSpan(-1, 0, 0, 0));
            if (listShift.FinishTime.Days >= 1)
                listShift.FinishTime = listShift.FinishTime.Add(new TimeSpan(-1, 0, 0, 0));
            ViewBag.LastNumber = "Последний номер: " + listContext.ShiftTasks.Where(x => x.ShiftId == listShift.ShiftId).Max(u => u.Number);
            ViewBag.Shifts = new SelectList(listContext.Shifts, "Id", "Name");
            return View(listShift);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult EditTask(ShiftTask listShift)
        {
            ViewBag.Shifts = new SelectList(listContext.Shifts, "Id", "Name", listShift.ShiftId);
            IQueryable<ShiftTask>? shiftNumber = listContext.ShiftTasks.Where(x => x.ShiftId == listShift.ShiftId && !x.Status);
            if (shiftNumber.Where(n => n.Number == listShift.Number).Count() > 0)
                if (shiftNumber.Where(n => n.Number == listShift.Number).FirstOrDefault().Id != listShift.Id)
                {
                    ViewBag.LastNumber = "Последний номер: " + shiftNumber.Max(u => u.Number);
                    ModelState.AddModelError("Number", "Данный номер уже отмечён в заданиях");
                }
            if (ModelState.IsValid)
            {
                if (shiftNumber.Count() > 0)
                    if (shiftNumber.Min(x => x.StartTime) >= new TimeSpan(20, 0, 0))
                    {
                        listShift.StartTime = listShift.StartTime >= new TimeSpan(0, 0, 0) && listShift.StartTime < new TimeSpan(13, 0, 0) ? listShift.StartTime.Add(new TimeSpan(1, 0, 0, 0)) : listShift.StartTime;
                        listShift.FinishTime = listShift.FinishTime >= new TimeSpan(0, 0, 0) && listShift.FinishTime < new TimeSpan(13, 0, 0) ? listShift.FinishTime.Add(new TimeSpan(1, 0, 0, 0)) : listShift.FinishTime;
                    }
                listContext.Entry(listShift).State = EntityState.Modified;
                listContext.SaveChanges();
                return RedirectToAction("AdminShift");
            }
            return View(listShift);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult DeleteTask(int taskId, string? tittle, short shiftId = 0, AdminShiftEnum pageSort = AdminShiftEnum.StartTimeAsc)
        {
            ShiftTask listShift = listContext.ShiftTasks.Find(taskId);
            listShift.Status = listShift.Status ? false : true;
            listContext.Entry(listShift).State = EntityState.Modified;
            listContext.SaveChanges();
            return RedirectToAction("AdminShift", new { tittle, shiftId, pageSort });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult DeleteNotify(int taskId, string? tittle, short shiftId = 0, AdminShiftEnum pageSort = AdminShiftEnum.StartTimeAsc)
        {
            ShiftTask listShift = listContext.ShiftTasks.Find(taskId);
            listShift.NotifyUser = listShift.NotifyUser ? false : true;
            listContext.Entry(listShift).State = EntityState.Modified;
            listContext.SaveChanges();
            return RedirectToAction("AdminShift", new { tittle, shiftId, pageSort });
        }




    }
}
