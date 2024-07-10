using CheckListJob.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CheckListJob.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Обязательное поле")]
        public User User { get; set; }

        public SelectList Roles { get; set; }

        public Role Role { get; set; }
    }
}
