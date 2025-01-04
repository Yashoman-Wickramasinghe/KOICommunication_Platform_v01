using KOICommunicationPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOICommunicationPlatform.DataAccess
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentGroupHD> StudentGroupHDs { get; set; }
        public DbSet<StudentGroupDetail> StudentGroupDetails { get; set; }
        public DbSet<ProjectDeliverable> ProjectDeliverables { get; set; }
        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        public DbSet<CommentsOnDocumentUpload> CommentsOnDocumentUploads { get; set; }
        public DbSet<TaskBoard> TaskBoards { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<SprintTask> SprintTasks { get; set; }
        public DbSet<TaskAllocationMember> TaskAllocationMembers { get; set; }
        public DbSet<ChatGroupHD> ChatGroupHDs { get; set; }
        public DbSet<ChatGroupDetail> ChatGroupDetails { get; set; }
        public DbSet<ClientMeeting> ClientMeetings { get; set; }
        public DbSet<Tutorial> Tutorials { get; set; }
        public DbSet<Student> Students {  get; set; }
        public DbSet<SprintTasksPosition> SprintTasksPositions { get; set; }
        public DbSet<SprintTaskAssignment> SprintTaskAssignments { get; set; }
        public DbSet<CommentsOnTaskBoard> CommentsOnTaskBoards { get; set; }
    }
}
