# Version 1.0
Author: Wickrama Arachchige Yashoman Wickramasinghe
Date: 24th September 2024

Guide lines to setup the KOICommunication Platform Project

-----------------------------------------Start - Tools---------------------------------------------------------

1. Microsoft Visual Studio 2022 (.Net Core 6.0) - You can use the upper versions.
2. Microsoft SQL Server

-----------------------------------------End - Tools-----------------------------------------------------------


-----------------------------------------Start - "appsettings.jason"--------------------------------------------

1. Give correct credentials for the "ConnectionStrings".

2. Change the below sections of the appsettings.jason file.

//-- All of the email sending through below email address.

"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": "587",
  "Username": "organization email address",
  "Password": "password(You should generate an app password: This feature is available in Gmail.)",
  "From": "organization email address",
  "Domain": "localhost: Your Local Host"
},

Guides: 
- Find localhost: Extract the main project "KOICommunicationPlatform" -> Properties (Folder) -> launchSettings.json

- Generate Password Hash Key (Steps to follow)

1. Enable Two-Factor Authentication (2FA):
	Go to your Google Account settings.
	Navigate to "Security."
	Under "Signing in to Google," enable "2-Step Verification."

2. Create an App Password:
	After enabling 2FA, return to the "Security" section.
	Search "App passwords" and select it.
	Provide a name to the App Password
	Click "Create" to get a 16-character app password.

------------------------------------------End - "appsettings.jason"------------------------------------------------


------------------------------------------Start - "DbInitializer.cs"----------------------------------------------
Intial setup for the System Administrator

Path - KOICommunicationPlatform.DataAccess (Class Library which is in outside of the main project) -> DbInitializer -> DbInitializer.cs

 var adminUser = new ApplicationUser
 {
     UserName = "admin email",
     Email = "admin email",
     GivenName = "Name",
     Surname = "Surname",
     PhoneNumber = "Mobile",
     UserType = SD.Website_Admin,
     IsActive = true,
 };

 _userManager.CreateAsync(adminUser, "Admin123*").GetAwaiter().GetResult();

 var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin email");
 _userManager.AddToRoleAsync(user, SD.Website_Admin).GetAwaiter().GetResult();

-------------------------------------------End - "DbInitializer.cs"----------------------------------------------


------------------------------------------Start - Database Setup-------------------------------------------------

1. Open Microsft Sql Server and Create a Database call "CommunicationPlatformV02"
2. Remember the ConnectionString according to your credintials.
3. Open the class library "KOICommunicationPlatform.DataAccess"
4. Please delete the migration folder if it's existing, otherwise it's all good.
5. "Clean" the entire solution and "Build".
6. Open the package manager console in MS Visual Studio 2022 (Tools -> Nuget Package Manager -> Package Manager Console)
7. You will get a console on the bottom of MS Visual Studio 2022.
8. Then select the correct library before you do the migrations. "KOICommunicationPlatform.DataAccess"
9. Type - > add-migration initial_migtation ("initial_migtation" - this can be any name). If there are no errors.
10. Type -> update-database

------------------------------------------End - Database Setup-------------------------------------------------


------------------------------------------Start - Run-------------------------------------------------

1. Run the program.
2. You will get an email to setup the admin.

------------------------------------------End - Run-------------------------------------------------
