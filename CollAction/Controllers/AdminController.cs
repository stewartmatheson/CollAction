﻿using CollAction.Data;
using CollAction.Helpers;
using CollAction.Models;
using CollAction.Models.AdminViewModels;
using CollAction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CollAction.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController: Controller
    {
        public AdminController(
            UserManager<ApplicationUser> userManager,
            IStringLocalizer<AccountController> localizer,
            IHostingEnvironment hostingEnvironment,
            IEmailSender emailSender,
            IProjectService projectService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _localizer = localizer;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _emailSender = emailSender;
            _projectService = projectService;
        }

        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IProjectService _projectService;

        [HttpGet]
        public IActionResult Index()
            => View();

        [HttpGet]
        public async Task<IActionResult> ManageProjectsIndex()
            => View(await _context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).ToListAsync());

        [HttpGet]
        public async Task<IActionResult> ManageProject(int id)
        {
            Project project = await _context.Projects.Include(p => p.Tags).ThenInclude(t => t.Tag).Include(p => p.DescriptionVideoLink).Include(p => p.BannerImage).Include(p => p.DescriptiveImage).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null)
                return NotFound();

            ManageProjectViewModel model = new ManageProjectViewModel()
            {
                UserList = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", null),
                CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", null),
                DisplayPriorityList = new SelectList(Enum.GetValues(typeof(ProjectDisplayPriority))),
                StatusList = new SelectList(Enum.GetValues(typeof(ProjectStatus))),
                Hashtag = project.HashTags,
                Name = project.Name,
                Description = project.Description,
                CategoryId = project.CategoryId,
                CreatorComments = project.CreatorComments,
                DescriptionVideoLink = project.DescriptionVideoLink?.Link,
                BannerImageDescription = project.BannerImage?.Description,
                DescriptiveImageDescription = project.DescriptiveImage?.Description,
                End = project.End,
                Start = project.Start,
                Target = project.Target,
                DisplayPriority = project.DisplayPriority,
                Goal = project.Goal,
                OwnerId = project.OwnerId,
                Status = project.Status,
                Proposal = project.Proposal,
                Id = project.Id
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ParticipantsDataExport(int id)
        {
            string csv = await _projectService.GenerateParticipantsDataExport(id);
            if (csv != null)
                return Content(csv, "text/csv");
            else
                return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageProject(ManageProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = await _context.Projects.Where(p => p.Id == model.Id).Include(p => p.Owner).Include(p => p.Tags).ThenInclude(t => t.Tag).Include(p => p.DescriptionVideoLink).FirstAsync();

                bool approved = model.Status == ProjectStatus.Running && project.Status == ProjectStatus.Hidden;
                bool successfull = model.Status == ProjectStatus.Successful && project.Status == ProjectStatus.Running;
                bool failed = model.Status == ProjectStatus.Failed && project.Status == ProjectStatus.Running;

                if (approved)
                {
                    string approvalEmail =
                        "Hi!<br>" +
                        "<br>" +
                        "The CollAction Team has reviewed your project proposal and is very happy to share that your project has been approved and now live on www.collaction.org!<br>" +
                        "<br>" +
                        "So feel very welcome to start promoting it! If you have any further questions, feel free to contact the CollAction Team at collactionteam@gmail.com. And don’t forget to tag CollAction in your messages on social media so we can help you spread the word(FB: @collaction.org, Twitter: @collaction_org)!<br>" +
                        "<br>" +
                        "Thanks again for driving the CollAction / crowdacting movement!<br>" +
                        "<br>" +
                        "Warm regards,<br>" +
                        "The CollAction team<br>";

                    string subject = $"Approval - {project.Name}";

                    await _emailSender.SendEmailAsync(project.Owner.Email, subject, approvalEmail);
                }
                else if (successfull)
                {
                    string successEmail =
                        "Hi!<br>" +
                        "<br>" +
                        "The deadline of the project you have started on www.collaction.org has passed. We're very happy to see that the target you have set has been reached! Congratulations! Now it's time to act collectively!<br>" +
                        "<br>" +
                        "The CollAction Team might reach out to you with more specifics (this is an automated message). If you have any further questions yourself, feel free to contact the CollAction Team at collactionteam@gmail.com. And don’t forget to tag CollAction in your messages on social media so we can help you spread the word on your achievement (FB: @collaction.org, Twitter: @collaction_org)!<br>" +
                        "<br>" +
                        "Thanks again for driving the CollAction / crowdacting movement!<br>" +
                        "<br>" +
                        "Warm regards,<br>" +
                        "The CollAction team<br>";

                    string subject = $"Success - {project.Name}";

                    await _emailSender.SendEmailAsync(project.Owner.Email, subject, successEmail);
                }
                else if (failed)
                {
                    string failedEmail =
                        "Hi!<br>" +
                        "<br>" +
                        "The deadline of the project you have started on www.collaction.org has passed. Unfortunately the target that you have set has not been reached. Great effort though!<br>" +
                        "<br>" +
                        "The CollAction Team might reach out to you with more specifics (this is an automated message). If you have any further questions yourself, feel free to contact the CollAction Team at collactionteam@gmail.com.<br>" +
                        "<br>" +
                        "Thanks again for driving the CollAction / crowdacting movement and better luck next time!<br>" +
                        "<br>" +
                        "Warm regards,<br>" +
                        "The CollAction team<br>";

                    string subject = $"Failed - {project.Name}";

                    await _emailSender.SendEmailAsync(project.Owner.Email, subject, failedEmail);
                }

                project.Name = model.Name;
                project.Description = model.Description;
                project.Goal = model.Goal;
                project.Proposal = model.Proposal;
                project.CreatorComments = model.CreatorComments;
                project.CategoryId = model.CategoryId;
                project.Target = model.Target;
                project.Start = model.Start;
                project.End = model.End;
                project.Status = model.Status;
                project.OwnerId = model.OwnerId;
                project.DisplayPriority = model.DisplayPriority;

                if (model.HasBannerImageUpload)
                {
                    var manager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "bannerimages"));
                    if (project.BannerImage != null)
                    {
                        manager.DeleteImageFile(project.BannerImage);
                    }
                    project.BannerImage = await manager.UploadFormFile(model.BannerImageUpload, Guid.NewGuid().ToString(), model.BannerImageDescription);
                }

                if (model.HasDescriptiveImageUpload)
                {
                    var manager = new ImageFileManager(_context, _hostingEnvironment.WebRootPath, Path.Combine("usercontent", "descriptiveimages"));
                    if (project.DescriptiveImage != null)
                    {
                        manager.DeleteImageFile(project.DescriptiveImage);
                    }
                    project.DescriptiveImage = await manager.UploadFormFile(model.DescriptiveImageUpload, Guid.NewGuid().ToString(), model.DescriptiveImageDescription);
                }

                await project.SetTags(_context, model.Hashtag?.Split(';') ?? new string[0]);

                project.SetDescriptionVideoLink(_context, model.DescriptionVideoLink);

                await _context.SaveChangesAsync();
                return RedirectToAction("ManageProjectsIndex");
            }
            else
            {
                model.UserList = new SelectList(await _context.Users.ToListAsync(), "Id", "UserName", null);
                model.CategoryList = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", null);
                model.DisplayPriorityList = new SelectList(Enum.GetValues(typeof(ProjectDisplayPriority)));
                model.StatusList = new SelectList(Enum.GetValues(typeof(ProjectStatus)));
                return View(model);
            }
        }
    }
}
