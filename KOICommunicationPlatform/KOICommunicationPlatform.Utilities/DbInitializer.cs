using KOICommunicationPlatform.Models;
using KOICommunicationPlatform.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.Utilities
{
    //-- 12203888 Edited by W A Yashoman Wickramasinghe 16/07/2024 -- //
    // implement with the IDbInitializer
    // -N-tier Architecture
    public class DbInitializer : IDbInitializer
    {
        //Role manager is use identity roles for these websites
        private UserManager<IdentityUser>  _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;

        public DbInitializer(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        //Create roles if it is not existing in the database
        //check migration then migrate your database
        public void Initialize()
        {
            try
            {
                //If it is pending migration: Need to create a databse
                //After that create the roles for the website
                //create a default admin
                if (_context.Database.GetPendingMigrations().Count() > 0 )
                {
                    //Get all migrations that are defined in the assembly
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            // GetAwaiter().GetResult() : need both the result of the task and direct exception handling
            // GetResult actually means “check the task for errors”


            if (!_roleManager.RoleExistsAsync(WebsiteRoles.Website_Admin).GetAwaiter().GetResult())
            {
                //create a new role asynchronously and then wait for the task to complete.

                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.Website_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.Website_Supervisor)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.Website_Student)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebsiteRoles.Website_Client)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser { 
                    UserName = "yashoman0608@gmail.com",
                    Email = "yashoman0608@gmail.com",
                    PhoneNumber = "0449527860",
                    UserRoleId=1,
                }, "Admin123*").GetAwaiter().GetResult();

                //User :  Admin
                var AppUser = _context.ApplicationUsers.FirstOrDefault(x=>x.Email == "yashoman0608@gmail.com");

                if (AppUser != null)
                {
                    _userManager.AddToRoleAsync(AppUser, WebsiteRoles.Website_Admin).GetAwaiter().GetResult(); 
                }
            }
            
        }
    }
}
