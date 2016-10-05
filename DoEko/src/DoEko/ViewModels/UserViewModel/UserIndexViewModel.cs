using DoEko.Models;
using DoEko.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DoEko.ViewModels.UserViewModel
{
    public enum UserIndexSortOrder { NameAsc, NameDesc, MailAsc, MailDesc }

    public class UserIndexViewModel
    {
        public List<UserItemViewModel> users { get; private set; }

        public SelectList RoleList(string selectedRoleId)
        {
            IdentityRole selectedRole = new IdentityRole();

            if (!string.IsNullOrEmpty(selectedRoleId))
            {
                selectedRole = _roleManager.Roles.Where(r => r.Id == selectedRoleId).First();

            }

            return new SelectList(
                _roleManager.Roles.ToList(), 
                nameof(IdentityRole.Id),
                nameof(IdentityRole.Name),
                selectedRole);
        }

        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        
        public UserIndexViewModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            users = new List<UserItemViewModel>();
        }

        public void OrderBy(UserIndexSortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case UserIndexSortOrder.NameDesc:
                    users = users.OrderByDescending(u => u.UserName).ToList();
                    break;
                case UserIndexSortOrder.MailAsc:
                    users = users.OrderBy(u => u.Email).ToList();
                    break;
                case UserIndexSortOrder.MailDesc:
                    users = users.OrderByDescending(u => u.Email).ToList();
                    break;
                case UserIndexSortOrder.NameAsc:
                default:
                    users = users.OrderBy(u => u.UserName).ToList();
                    break;
            }
        }

        public async Task Select(string searchString = null, string roleId = null)
        {
            //
            IQueryable<ApplicationUser> allUsers;

            if (!string.IsNullOrEmpty(roleId))
            {
                allUsers = _userManager.Users.Where(u => u.Roles.Any(r => r.RoleId == roleId ));
            }
            else
            {
                allUsers = _userManager.Users;
            }
            // 
            if (!string.IsNullOrEmpty(searchString))
            {
                allUsers = allUsers.Where(u =>
                u.UserName.Contains(searchString) || u.Email.Contains(searchString));
            }
            // 
            users = allUsers.Select(userItem => new UserItemViewModel
            {
                UserName = userItem.UserName,
                FirstName = userItem.FirstName,
                LastName = userItem.LastName,
                Email = userItem.Email,
                LockoutEnabled = userItem.LockoutEnabled,
                LockoutEnd = userItem.LockoutEnd,
                Id = userItem.Id
            }).ToList();

            foreach (var userItem in users)
            {
                userItem.RoleNames = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(userItem.UserName));
            }

        }

            // Add role collections for each user
            //foreach (var userItem in users)
            //{
            //    foreach (var role in userItem.roles_temp)
            //    {
            //        userItem.RolesColl.Add(await _roleManager.FindByIdAsync(role.RoleId));
            //    }
            //}
        //}

        public void FilterBy(string searchString, string roleId)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                u.UserName.Contains(searchString) || u.Email.Contains(searchString)).ToList();
            }

            if (!string.IsNullOrEmpty(roleId))
            {
                users = users.Where(u => u.RolesColl.Exists(r => r.Id == roleId)).ToList();
            }
        }

        private async Task ReadUsers()
        {

            // 
            users = _userManager.Users.Select(userItem => new UserItemViewModel
            {
                FirstName = userItem.FirstName,
                LastName = userItem.LastName,
                UserName = userItem.UserName,
                Email = userItem.Email,
                LockoutEnabled = userItem.LockoutEnabled,
                LockoutEnd = userItem.LockoutEnd,
        }).ToList();
            
            // Add role collections for each user
            foreach (var userItem in users)
            {
                userItem.RoleNames = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(userItem.UserName));
            }
        }

    } 
    
    public class UserItemViewModel : ApplicationUser
    {
        public List<IdentityRole> RolesColl { get; set; }
        
        public IList<string> RoleNames { get; set; }
    }

}
