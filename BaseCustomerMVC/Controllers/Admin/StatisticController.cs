using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Controllers.Admin
{
    [IndefindCtrlAttribulte("Trang thống kê", "StatisticController", "admin")]
    public class StatisticController : AdminController
    {
        private readonly AccountService _accountService;
        public StatisticController(AccountService accountService)
        {
            _accountService = accountService;
        }
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Data = _accountService.GetAll();
            return View();
        }
        //[HttpPost]
        [Route("/getlist-account")]
        public IEnumerable<AccountEntity> GetListAccount()
        {
            return _accountService.GetAll()?.ToList();
        }

        [HttpPost]
        public ActionResult Add(string SearchText)
        {
            TempData["Error"] = RouteData.Values["Error"];
            ViewBag.Model = SearchText;
            return RedirectToAction("index");
        }

        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
