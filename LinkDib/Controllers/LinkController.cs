﻿using System;
using System.Linq;
using System.Web.Mvc;
using LinkDib.Models;
using LinkDib.ViewModels;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;

namespace LinkDib.Controllers
{
    public class LinkController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LinkController()
        {
            _context = new ApplicationDbContext();
        }

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new LinkFormViewModel
            {
                Categories = _context.Categories.ToList()
            };

            return View("LinkForm", viewModel);

        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LinkFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View("LinkForm", viewModel);

            var userId = User.Identity.GetUserId();

            var link = new Link(viewModel.Url, userId, viewModel.Message, viewModel.CategoryId, viewModel.Permission);
            _context.Links.Add(link);

            var notification = new Notification(userId, link, NotificationType.LinkNew);
            _context.Notifications.Add(notification);

            var followers = _context.Followings.Where(f => f.FolloweeId == userId).Select(f => f.Follower).ToList();

            foreach (var follower in followers)
            {
                follower.Notify(notification);

            }

            _context.SaveChanges();

            return RedirectToAction("List", "Link");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(LinkFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View("LinkForm", viewModel);

            var userId = User.Identity.GetUserId();
            var link = _context.Links.Single(l => l.Id == viewModel.Id && l.UserId == userId);

            link.Category = viewModel.Category;
            link.Url = viewModel.Url;
            link.Message = viewModel.Message;
            link.Permission = viewModel.Permission;

            _context.SaveChanges();

            return RedirectToAction("List", "Link");
        }

        public ActionResult List()
        {

            var links = _context.Links
                .Where(l => !l.IsDeleted)
                .Include(l => l.User)
                .Include(l => l.Category)
                .OrderByDescending(l => l.DateTime);

            var userId = User.Identity.GetUserId();

            var likes = _context.Likes.Where(l => l.UserId == userId).ToList().ToLookup(l => l.LinkId);
            var favorites = _context.Favorites.Where(f => f.UserId == userId).ToList().ToLookup(l => l.LinkId);
            var followees = _context.Followings.Where(f => f.FollowerId == userId).ToList().ToLookup(l => l.FolloweeId);

            var viewModel = new LinkListViewModel
            {
                Links = links,
                Authenticated = User.Identity.IsAuthenticated,
                Likes = likes,
                Favorites = favorites,
                Followees = followees
            };

            return View(viewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();
            var link = _context.Links.Single(l => l.Id == id && l.UserId == userId);

            var viewModel = new LinkFormViewModel
            {
                Id = id,
                Url = link.Url,
                Message = link.Message,
                Categories = _context.Categories.ToList(),
                Category = link.Category,
                Permission = link.Permission
            };

            return View("LinkForm", viewModel);
        }
    }
}