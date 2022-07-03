# WebAppCMS

###### This is a practice project created as the final part of .NET Core Web Development full course.  
##### The project contains a fully functional Content Management System as a full-blown web application developed using .NET Core technology. 

###### The underlying architecture has separated "Web UI", "Data" and "Api" layers: <br><br> The "Web UI" layer is implemented in MVC architecture. <br> The "Data" layer is implemented conforming to repository pattern (concrete implementation uses Entity Framework Core ORM to connect to SQL Server). <br> The "Api" layer is implemented conforming to REST principles. 

## How To Setup Development Environment? 

### Prerequisites
 - <b>Visual Studio 2019</b> (or newer) with .NET5
   - Installed packages ("Manage NuGet Packages for Solution"):
   - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore (5.0.17)
   - Microsoft.AspNetCore.Identity.EntityFrameworkCore (5.0.17)
   - Microsoft.AspNetCore.Identity.UI (5.0.17)
   - Microsoft.EntityFrameworkCore.SqlServer (5.0.17)
   - Microsoft.EntityFrameworkCore.Tools (5.0.17)
   - Microsoft.VisualStudio.Web.CodeGeneration.Design (5.0.2)
   - Swashbuckle.AspNetCore (5.6.3)

 - <b>SQL Server 2016</b> (or newer)
   - The initial ConnectionString settings are set up assuming a locally running SQL Server instance (localdb)\mssqllocaldb (login using Windows Authentication). 
   The following projects are already set up accordingly (<b>appsettings.json</b> in <b>each</b> of the following): 
     - WebAppCMS
     - WebAppCMS.Api
     - WebAppCMS.Data
     - <i>In case of changing the ConnectionString settings, all 3 projects have to be updated accordingly. </i>
     
### Steps
 1. Create a local copy of the repository (Visual Studio)
 2. Configure "Set as Startup Project" for <b>WebAppCMS</b>
 3. Open Package Manager Console
 4. Set up "Default project" within Package Manager Console to <b>WebAppCMS.Data</b>
 5. Within Package Manager Console, execute the Update-Database command: 
    - To create the database with all the required tables, including inserting initial users and <b>including test data</b>:
      - <i>Update-Database</i>
    - To create an empty database (<b>without test data</b>), execute the following command instead: 
      - <i>Update-Database InsertInitialAppUsers</i>
    
 6. Start the application (from Visual Studio)
 
 ##### When the application is started, you can login with one of the following initial users:
  - User Name: admin@admin.com | Password: Pass123! 
    - <b>Admin</b> role, has Administration access and can configure App Users
  - User Name: supervisor@supervisor.com | Password: Pass123! 
    - <b>Supervisor</b> role, has Administration access but cannot configure App Users
  - User Name: client@client.com | Password: Pass123! 
    - <b>Client</b> role, does not have Administration access
    
###### Swagger can be used to test exposed endpoints by configuring "Set as Startup Project" for <b>WebAppCMS.Api</b>. 
